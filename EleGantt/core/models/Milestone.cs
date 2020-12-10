using System;
using System.ComponentModel;

namespace EleGantt.core.models
{
    internal class Milestone : INotifyPropertyChanged
    {
        private string _name;
        private DateTime _date;
        private string Name 
        {
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
            get
            {
                return _name;
            }
        }
        private DateTime Date
        {
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
            get
            {
                return _date;
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