using EleGantt.core.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace EleGantt.core.viewModels
{
    class GanttViewModel : INotifyPropertyChanged
    {

        private IList<GanttTask> _TaskList;
        private IList<Milestone> _MilestoneList;
        private string _name;

        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public IList<GanttTask> Tasks
        {
            get { return _TaskList; }
            set 
            { 
                _TaskList = value;
                OnPropertyChanged("Tasks");
            }
        }

        public IList<Milestone> Milestones
        {
            get { return _MilestoneList; }
            set 
            {
                _MilestoneList = value;
                OnPropertyChanged("Milestones");
            }
        }

        public GanttViewModel()
        {
            _TaskList = new List<GanttTask>
            {
                new GanttTask{Name="test",  DateStart=DateTime.Now, DateEnd=DateTime.Now.AddDays(2)},
                new GanttTask{Name="Test 123",  DateStart=DateTime.Now.AddDays(3), DateEnd=DateTime.Now.AddDays(6)},
            };
        }

        public void AddTask(GanttTask task)
        {
            _TaskList.Add(task);
            OnPropertyChanged("Tasks");
        }

        public void RemoveTask(GanttTask task)
        {
            if (_TaskList.Remove(task))
            {
                OnPropertyChanged("Tasks");
            }
        }

        public void AddMilestone(Milestone milestone)
        {
            _MilestoneList.Add(milestone);
            OnPropertyChanged("Milestones");
        }

        public void RemoveMilestone(Milestone milestone)
        {
            if (_MilestoneList.Remove(milestone))
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

