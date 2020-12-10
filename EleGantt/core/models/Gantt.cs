using System.Collections.Generic;
using System.ComponentModel;

namespace EleGantt.core.models
{
    class Gantt : INotifyPropertyChanged
    {
        private string Name
        {
            set
            {
                Name = value;
                OnPropertyChanged("Name");
            }
            get
            {
                return Name;
            }
        }
        private List<GanttTask> Tasks;
        private List<Milestone> Milestones;

        public void AddTask(GanttTask task)
        {
            Tasks.Add(task);
            OnPropertyChanged("Tasks");
        }

        public void RemoveTask(GanttTask task)
        {
            if (Tasks.Remove(task))
            {
                OnPropertyChanged("Tasks");
            }
        }

        public void AddMilestone(Milestone milestone)
        {
            Milestones.Add(milestone);
            OnPropertyChanged("Milestones");
        }

        public void RemoveMilestone(Milestone milestone)
        {
            if (Milestones.Remove(milestone))
            {
                OnPropertyChanged("Milestones");
            }
        }


        #region INotifyPropertyChanged Members  

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
