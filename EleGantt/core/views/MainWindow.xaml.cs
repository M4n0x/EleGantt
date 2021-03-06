﻿using EleGantt.core.models;
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
        private readonly PaletteHelper _paletteHelper = new PaletteHelper(); //dark/white theme
        private CultureInfo culture = CultureInfo.CurrentCulture; //used to put the days as 3 letters, currently not in use

        public MainWindow()
        {
            //init viewmodel
            viewModel = new GanttViewModel();
            viewModel.ClosingRequest += delegate { Close(); };
            DataContext = viewModel;
            InitializeComponent();

            //init theme and timeline
            ApplyCurrentTheme();
            AdjustTimeline();

            //message system
            var mainMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(5000));
            MainSnackbar.MessageQueue = mainMessageQueue;

            //listen to mouse wheel for zoom
            PreviewMouseWheel += Window_PreviewMouseWheel;

            Focus(); //used to bind the command to the "about" menuitem
        }

        /// <summary>
        /// Listen to the wheel rotation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            //zoom++
            if (e.Delta > 0)
                viewModel.CellWidth += 1;

            //zoom--
            else if (e.Delta < 0)
                viewModel.CellWidth -= 1;
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

        #region task dragg
        //dragging attributes
        GanttTaskViewModel draggingTask;
        public bool isDraggingTask = false;
        double taskDragLastX;
        double taskDragBuffer;
        private void Task_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //set dragging to true
            isDraggingTask = true;
            //init dragging variables
            draggingTask = (sender as StackPanel).DataContext as GanttTaskViewModel;
            taskDragLastX = e.GetPosition(this).X;
            taskDragBuffer = 0;
        }

        private void Task_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //verify dragging
            if (!isDraggingTask)
                return;

            //calculate position delta between last mouve and current move
            var currentX = e.GetPosition(this).X;
            var delta = currentX - taskDragLastX;
            double dayDelta = delta / viewModel.CellWidth; //transform position value to day value
            taskDragBuffer += dayDelta; //add to buffer

            //if buffer is more than 1 day : apply the delta to start/end
            //This allows a "cranted", discrete movement day-by-day and the task is never in the middle of 2 days
            if(Math.Abs(taskDragBuffer) >= 1)
            {
                var nextEndTime = draggingTask.DateEnd.AddDays(1 * Math.Sign(taskDragBuffer));
                var nextStartTime = draggingTask.DateStart.AddDays(1 * Math.Sign(taskDragBuffer));
                if (IsDateInProject(nextEndTime) && IsDateInProject(nextStartTime))
                {
                    draggingTask.DateEnd = nextEndTime;
                    draggingTask.DateStart = nextStartTime;
                }
                taskDragBuffer -= 1 * Math.Sign(taskDragBuffer); //sub delta from bufer
            }

            //save current x
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
        //SIMILAR TO TASK DRAGG but with 1 date instead of 2
        //Please refer to the taskdrag region to see the commented version
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


    /// <summary>
    /// This function is used to display days and months in the timeline. This is not possible in binding because there need to be
    /// an iteration between the start date and end date of the project, as well as a calcul for the month's case length
    /// </summary>
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

        /// <summary>
        /// Add a label to the timeline display
        /// </summary>
        /// <param name="month"></param>
        /// <param name="start"></param>
        /// <param name="duration"></param>
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

        /// <summary>
        /// Synchornize scroll between two scrollviewer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// This function change the timeline's display to prepare for taking a full capture of the main grid element
        /// </summary>
        /// <returns>Bitmap of the grid</returns>
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

        /// <summary>
        /// This function take a capute of an element
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        BitmapFrame SaveUsingEncoder(FrameworkElement visual)
        {
            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(visual);
            BitmapFrame frame = BitmapFrame.Create(bitmap);

            return frame;
        }
    }
}
