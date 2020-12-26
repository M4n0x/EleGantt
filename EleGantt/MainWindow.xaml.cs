using EleGantt.core.viewModels;
using System;
using System.Windows;

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
            viewModel = new GanttViewModel();

            //@Todo replace by data manager
            viewModel.Name = "test";
            viewModel.ClosingRequest += delegate { Close(); };

            DataContext = viewModel;
            InitializeComponent();

            viewModel.Name = "oopsie";
        }

        public void OnListDoubleClick(object sender, EventArgs e)
        {
            viewModel.EnableEditionCmd.Execute(null);
        }

    }
}
