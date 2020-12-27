using System;
using System.ComponentModel;
using EleGantt.core.models;

namespace EleGantt.core.viewModels
{
    internal class MilestoneViewModel : INotifyPropertyChanged
    {
        private MilestoneModel _milestone;

        public MilestoneViewModel(MilestoneModel milestone)
        {
            _milestone = milestone;
        }

        public MilestoneModel MilestoneModel
        {
            get { return _milestone; }
        }

        public string Name 
        {
            set
            {
                _milestone.Name = value;
                OnPropertyChanged("Name");
            }
            get
            {
                return _milestone.Name;
            }
        }
        public DateTime Date
        {
            set
            {
                _milestone.Date = value;
                OnPropertyChanged("Date");
            }
            get
            {
                return _milestone.Date;
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