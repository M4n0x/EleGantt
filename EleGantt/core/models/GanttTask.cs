using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EleGantt.core.models
{
    internal class GanttTask : INotifyPropertyChanged
    {
        private string Name 
        { 
            set 
            { 
                Name = value;
                OnPropertyChanged("Name");
            }
            get { return Name; }
        }
        private DateTime DateStart 
        {
            set
            {
                DateStart = value;
                OnPropertyChanged("DateStart");
            }
            get { return DateStart; }
        }
        private DateTime DateEnd
        {
            set
            {
                DateEnd = value;
                OnPropertyChanged("DateEnd");
            }
            get { return DateStart; }
        }
        private List<GanttTask> Subtasks;

        public void AddSubTask(GanttTask task)
        {
            Subtasks.Add(task);
            OnPropertyChanged("tasks");
        }

        public void RemoveSubTask(GanttTask task)
        {
            if (Subtasks.Remove(task))
            {
                OnPropertyChanged("tasks");
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