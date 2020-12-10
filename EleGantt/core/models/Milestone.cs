using System;
using System.ComponentModel;

namespace EleGantt.core.models
{
    internal class Milestone : INotifyPropertyChanged
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
        private DateTime Date
        {
            set
            {
                Date = value;
                OnPropertyChanged("date");
            }
            get
            {
                return Date;
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