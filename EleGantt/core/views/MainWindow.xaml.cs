using EleGantt.core.models;
using EleGantt.core.viewModels;
using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace EleGantt.core.views
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GanttViewModel viewModel;
        private readonly PaletteHelper _paletteHelper = new PaletteHelper();
        public MainWindow()
        {
            viewModel = new GanttViewModel();

            viewModel.ClosingRequest += delegate { Close(); };

            DataContext = viewModel;
            InitializeComponent();

            ApplyCurrentTheme();

            AdjustTimeline();
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
            if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue)
                return;

            int currentMonthDays = 0, index = 0;

            DateTime currentDay = StartDate.SelectedDate.Value;
            DateTime end = EndDate.SelectedDate.Value;

            string currentMonth = currentDay.ToString("MMMM");

            while (currentDay <= end) {

                Timeline.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(50),
                }) ;
                //create "day" textbox
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
            //add last month
            AddMonth(currentMonth, index - currentMonthDays, currentMonthDays);
        }

        private void AddMonth(String month, int start, int duration)
        {
            //create "month" textbox
            TextBlock monthBox = new TextBlock { Text = month };
            monthBox.HorizontalAlignment = HorizontalAlignment.Center;
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

        private void SeletedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //AdjustTimeline();
        }

        private void ScrollChanged(object sender, ScrollChangedEventArgs e)
        {

            return;
            // Get the border of the listview (first child of a listview)
            Decorator border = VisualTreeHelper.GetChild(SideListView, 0) as Decorator;

            // Get scrollviewer
            ScrollViewer scrollViewer = border.Child as ScrollViewer;

            if (sender == TimelineScrollView)
            {
                scrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
                scrollViewer.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
            else
            {
                TimelineScrollView.ScrollToVerticalOffset(e.VerticalOffset);
                TimelineScrollView.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
        }
    }
}
