using EleGantt.core.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EleGantt.core.views
{
    /// <summary>
    /// Logique d'interaction pour TaskWindow.xaml
    /// </summary>
    public partial class TaskDialog : Window
    {
        private GanttTaskViewModel taskViewModel;
        public TaskDialog(GanttTaskViewModel ganttTaskViewModel)
        {
            taskViewModel = ganttTaskViewModel;
            DataContext = taskViewModel;
            InitializeComponent();
        }

        public void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        public static void CreateTaskWindowModal(GanttTaskViewModel ganttTaskViewModel)
        {
            var dialog = new TaskDialog(ganttTaskViewModel);
            dialog.ShowDialog();
            //add next code snippet if the modal is supposed to give result back
            /*if (dialog.DialogResult == true)
                Console.WriteLine("over");//*/
        }
    }
}
