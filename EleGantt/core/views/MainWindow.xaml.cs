using EleGantt.core.models;
using EleGantt.core.viewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
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

            PreviewMouseWheel += Window_PreviewMouseWheel;
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta > 0)
                viewModel.CellWidth += 1;

            else if (e.Delta < 0)
                viewModel.CellWidth -= 1;
        }

        public void FocusDraggingLost(object sender, RoutedEventArgs e)
        {
            isDraggingMilestone = false;
            isDraggingTask = false;
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

        #region task dragg
        GanttTaskViewModel draggingTask;
        public bool isDraggingTask = false;
        double taskDragLastX;
        double taskDragBuffer;
        private void Task_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDraggingTask = true;

            draggingTask = (sender as StackPanel).DataContext as GanttTaskViewModel;
            taskDragLastX = e.GetPosition(this).X;
            taskDragBuffer = 0;
        }

        private void Task_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggingTask)
                return;

            var currentX = e.GetPosition(this).X;
            var delta = currentX - taskDragLastX;

            double dayDelta = delta / viewModel.CellWidth;
            taskDragBuffer += dayDelta;


            if(Math.Abs(taskDragBuffer) >= 1)
            {
                var nextEndTime = draggingTask.DateEnd.AddDays(1 * Math.Sign(taskDragBuffer));
                var nextStartTime = draggingTask.DateStart.AddDays(1 * Math.Sign(taskDragBuffer));
                if (IsDateInProject(nextEndTime) && IsDateInProject(nextStartTime))
                {
                    draggingTask.DateEnd = nextEndTime;
                    draggingTask.DateStart = nextStartTime;
                }
                taskDragBuffer -= 1 * Math.Sign(taskDragBuffer);
            }

            taskDragLastX = currentX;
        }

        private void Task_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDraggingTask)
                return;

            isDraggingTask = false;
        }
        #endregion

        #region milestone dragg
        MilestoneViewModel draggingMilestone;
        public bool isDraggingMilestone = false;
        double milestoneDragLastX;
        double milestoneDragBuffer;

        private void Milestone_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDraggingMilestone = true;

            draggingMilestone = (sender as StackPanel).DataContext as MilestoneViewModel;
            milestoneDragLastX = e.GetPosition(this).X;
            milestoneDragBuffer = 0;
        }

        private void Milestone_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!isDraggingMilestone)
                return;

            var currentX = e.GetPosition(this).X;
            var delta = currentX - milestoneDragLastX;

            double dayDelta = delta / viewModel.CellWidth;
            milestoneDragBuffer += dayDelta;


            if (Math.Abs(milestoneDragBuffer) >= 1)
            {
                var nextDate = draggingMilestone.Date.AddDays(1 * Math.Sign(milestoneDragBuffer));
                if(IsDateInProject(nextDate))
                    draggingMilestone.Date = nextDate;
                milestoneDragBuffer -= 1 * Math.Sign(milestoneDragBuffer);
            }

            milestoneDragLastX = currentX;
        }

        private bool IsDateInProject(DateTime time)
        {
            return (time >= viewModel.Start && time <= viewModel.End);
        }

        private void Milestone_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isDraggingMilestone)
                return;

            isDraggingMilestone = false;
        }

    
    #endregion



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
                var cd = new ColumnDefinition();
                cd.SetBinding(ColumnDefinition.WidthProperty, new Binding("CellWidth"));
                Timeline.ColumnDefinitions.Add(cd);

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
            MainScrollTimeline.ScrollToHome();
            
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
