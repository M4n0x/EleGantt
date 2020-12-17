using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EleGantt.core.models
{
    internal class GanttTask : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _dateStart;
        private DateTime _dateEnd;
        private IList<GanttTask> _subTasks;
        public string Name 
        { 
            set 
            { 
                _name = value;
                OnPropertyChanged("Name");
            }
            get { return _name; }
        }
        public DateTime DateStart 
        {
            set
            {
                _dateStart = value;
                OnPropertyChanged("DateStart");
            }
            get { return _dateStart; }
        }
        public DateTime DateEnd
        {
            set
            {
                _dateEnd = value;
                OnPropertyChanged("DateEnd");
            }
            get { return _dateEnd; }
        }

        public IList<GanttTask> SubTasks
        {
            get { return _subTasks; }
            set { _subTasks = value; }
        }

        public void AddSubTask(GanttTask task)
        {
            _subTasks.Add(task);
            OnPropertyChanged("SubTasks");
        }

        public void RemoveSubTask(GanttTask task)
        {
            if (_subTasks.Remove(task))
            {
                OnPropertyChanged("SubTasks");
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