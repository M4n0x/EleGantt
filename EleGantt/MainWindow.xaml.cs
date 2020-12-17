using EleGantt.core.viewModels;
using System.Windows;


namespace EleGantt
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var viewModel = new GanttViewModel
            {
                //@Todo replace by data manager
                Name = "test"
            };

            DataContext = viewModel;
            InitializeComponent();

            viewModel.Name = "oopsie";
        }
    }
}
