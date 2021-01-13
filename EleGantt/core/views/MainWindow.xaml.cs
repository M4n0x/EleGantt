using EleGantt.core.models;
using EleGantt.core.viewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;

namespace EleGantt.core.views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GanttViewModel viewModel;
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        private CultureInfo culture = CultureInfo.CurrentCulture;
        public MainWindow()
        {
            viewModel = new GanttViewModel();

            viewModel.ClosingRequest += delegate { Close(); };

            DataContext = viewModel;
            InitializeComponent();

            ApplyCurrentTheme();

            AdjustTimeline();

            var mainMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
            MainSnackbar.MessageQueue = mainMessageQueue;
        }

        /// <summary>
        /// This function is used to apply the theme on the app, the parameter theme is retrieve through user's app's settings 
        /// </summary>
        private void ApplyCurrentTheme()
        {
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = Properties.Settings.Default.isDark ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
        }

        private void OpenDialog1(object sender, RoutedEventArgs e)
        {
            DialogHost.Show(new GanttTaskViewModel(new GanttTaskModel()), "dialog1");
        }

        private void AdjustTimeline()
        {
            if (StartDate == null || !StartDate.SelectedDate.HasValue || EndDate == null || !EndDate.SelectedDate.HasValue || Timeline == null)
                return;

            int currentMonthDays = 0, index = 0;

            DateTime start = StartDate.SelectedDate.Value;
            DateTime currentDay = start;
            DateTime end = EndDate.SelectedDate.Value;

            Timeline.ColumnDefinitions.Clear();
            Timeline.Children.Clear();

            string currentMonth = currentDay.ToString("MMMM");

            while (currentDay <= end) {

                Timeline.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(50),
                });

                //create "day" textbox
                //TextBlock box = new TextBlock() { Text = $"{currentDay.Day.ToString()} { culture.DateTimeFormat.GetAbbreviatedDayName(currentDay.DayOfWeek)}" };
                TextBlock box = new TextBlock() { Text = currentDay.Day.ToString() };
                box.HorizontalAlignment = HorizontalAlignment.Left;
                box.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(box, index++);
                Grid.SetRow(box, 1);
                Timeline.Children.Add(box);
                currentMonthDays++;

                //analyse - are we on the same month than before ?
                String month = currentDay.ToString("MMMM");
                if (month != currentMonth)
                {
                    AddMonth(currentMonth, index - currentMonthDays, currentMonthDays);
                    currentMonthDays = 0;
                    currentMonth = month;
                }
                currentDay = currentDay.AddDays(1);
            }
            
            AddMonth(currentMonth, index - currentMonthDays, currentMonthDays);
        }

        private void AddMonth(String month, int start, int duration)
        {
            //create "month" textbox
            TextBlock monthBox = new TextBlock { Text = month };
            monthBox.HorizontalAlignment = HorizontalAlignment.Center;
            monthBox.VerticalAlignment = VerticalAlignment.Bottom;
            if (duration!= 0)
                Grid.SetColumnSpan(monthBox, duration);
            Grid.SetColumn(monthBox, start);
            Grid.SetRow(monthBox, 0);
            Timeline.Children.Add(monthBox);
        }

        /// <summary>
        /// This function is used to apply function directly when an inputbox is showed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputTask_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var input = sender as TextBox;
            if (input.Visibility == Visibility.Visible)
            {
                Action focusAction = () => input.Focus(); // delay focus action cause mainwindows can be busy
                Dispatcher.BeginInvoke(focusAction, DispatcherPriority.ApplicationIdle);
            }
        }

        /// <summary>
        /// Apply theme on menu click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ApplyCurrentTheme();
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            // Get scrollviewer
            ScrollViewer scrollViewer = GetDescendantByType(SideListView, typeof(ScrollViewer)) as ScrollViewer;


            if (sender == TimelineScrollView)
            {
                if (e.VerticalChange != 0)
                {
                    if (TimelineScrollView.ScrollableHeight != 0)
                    {
                        double ratio = e.VerticalOffset / TimelineScrollView.ScrollableHeight;
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.ScrollableHeight * ratio);
                    }
                }
                /*if (e.HorizontalOffset != 0)
                {
                    double ratio = e.HorizontalOffset / TimelineScrollView.ScrollableWidth;
                    DatelineScrollView.ScrollToHorizontalOffset(DatelineScrollView.ScrollableWidth * ratio);
                }*/
            }
            else
            {
                if (e.VerticalChange != 0)
                {
                    if (scrollViewer.ScrollableHeight != 0)
                    {
                        double ratio = e.VerticalOffset / scrollViewer.ScrollableHeight;
                        Trace.WriteLine("ratio 2 : " + ratio);
                        TimelineScrollView.ScrollToVerticalOffset(TimelineScrollView.ScrollableHeight * ratio);
                    }
                }
            }
        }
        public static Visual GetDescendantByType(Visual element, Type type)
        {
            if (element == null)
            {
                return null;
            }
            if (element.GetType() == type)
            {
                return element;
            }
            Visual foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType(visual, type);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AdjustTimeline();
        }



        private void MenuItem_Export_Click(object sender, RoutedEventArgs e)
        {
            string filename = "ScreenCapture-" + DateTime.Now.ToString("ddMMyyyy-hhmmss") + ".png";
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "EleGantt Capture (*.png)|*.png",
                DefaultExt = "elegantt",
                AddExtension = true,
                FileName = filename
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                SaveToPng(filepath);
                MainSnackbar.MessageQueue.Enqueue($"Export to {filepath} successfull ! ");
            }

        }

        private void MenuItem_CB_Click(object sender, RoutedEventArgs e)
        {
            SaveToCB();
            MainSnackbar.MessageQueue.Enqueue("Export to clipboard successfull ! ");
        }

        void SaveToPng(string fileName)
        {
            var encoder = new PngBitmapEncoder();
            var frame = PrepareTimelineScreen();

            encoder.Frames.Add(frame);

            using (var stream = File.Create(fileName))
            {
                encoder.Save(stream);
            }
        }

        void SaveToCB()
        {
            var frame = PrepareTimelineScreen();
            Clipboard.SetImage(frame);
        }

        private BitmapFrame PrepareTimelineScreen()
        {
            MainScrollTimeline.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            MainScrollTimeline.UpdateLayout();
            var frame = SaveUsingEncoder(GridTimeline);
            MainScrollTimeline.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            MainScrollTimeline.UpdateLayout();

            return frame;
        }

        BitmapFrame SaveUsingEncoder(FrameworkElement visual)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);

            return frame;
        }
    }
}
