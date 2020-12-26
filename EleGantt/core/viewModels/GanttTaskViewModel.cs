using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using EleGantt.core.models;

namespace EleGantt.core.viewModels
{
    internal class GanttTaskViewModel : INotifyPropertyChanged
    {
        private GanttTask _task;
        private bool _isSelected;
        private bool _isEdition;

        public GanttTaskViewModel(GanttTask task)
        {
            this._task = task;
        }


        public Visibility showTextBlock
        {
            get { return _isEdition ? Visibility.Collapsed : Visibility.Visible; }
        }
        public Visibility showTextBox
        {
            get { return _isEdition ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool IsSelected
        {
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
            get { return _isSelected; }
        }
        public bool IsEdition
        {
            set
            {
                _isEdition = value;
                OnPropertyChanged("IsEdition");
                OnPropertyChanged("showTextBlock");
                OnPropertyChanged("showTextBox");
            }
            get { return _isEdition; }
        }
        public string Name 
        { 
            set 
            { 
                _task.Name = value;
                OnPropertyChanged("Name");
            }
            get { return _task.Name; }
        }
        public DateTime DateStart 
        {
            set
            {
                _task.DateStart = value;
                OnPropertyChanged("DateStart");
            }
            get { return _task.DateEnd; }
        }
        public DateTime DateEnd
        {
            set
            {
                _task.DateEnd = value;
                OnPropertyChanged("DateEnd");
            }
            get { return _task.DateEnd; }
        }

        public IList<GanttTask> SubTasks
        {
            get { return _task.SubTasks; }
            set { _task.SubTasks = value; }
        }

        public void AddSubTask(GanttTask task)
        {
            _task.SubTasks.Add(task);
            OnPropertyChanged("SubTasks");
        }

        public void RemoveSubTask(GanttTask task)
        {
            if (_task.SubTasks.Remove(task))
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