using EleGantt.core.viewModels;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EleGantt
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GanttViewModel viewModel;
        public MainWindow()
        {
            viewModel = new GanttViewModel(); // TODO load model in the view direct 

            //@Todo replace by data manager
            viewModel.ClosingRequest += delegate { Close(); };

            DataContext = viewModel;
            InitializeComponent();
        }

        private void inputTask_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var input = sender as TextBox;
            if (input.Visibility == Visibility.Visible)
            {
                Action focusAction = () => input.Focus(); //delay focus action cause mainwindows can be busy
                this.Dispatcher.BeginInvoke(focusAction, DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
