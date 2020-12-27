using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using EleGantt.core.models;

namespace EleGantt.core.viewModels
{
    internal class GanttTaskViewModel : INotifyPropertyChanged
    {
        private GanttTaskModel _task;
        private bool _isSelected;
        private bool _isEdition;

        public GanttTaskViewModel(GanttTaskModel task)
        {
            this._task = task;
        }


        public Visibility ShowTextBlock
        {
            get { return _isEdition ? Visibility.Collapsed : Visibility.Visible; }
        }
        public Visibility ShowTextBox
        {
            get { return _isEdition ? Visibility.Visible : Visibility.Collapsed; }
        }

        public GanttTaskModel GanttTaskModel
        {
            get
            {
                return _task;
            }
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
                OnPropertyChanged("ShowTextBlock");
                OnPropertyChanged("ShowTextBox");
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


        #region COMMANDS 

        private RelayCommand _enableEditionCmd;
        public ICommand EnableEditionCmd
        {
            get
            {
                return _enableEditionCmd ?? (_enableEditionCmd = new RelayCommand(x =>
                {
                    Trace.WriteLine("Enable edit");
                    if (!IsEdition)
                        IsEdition = true;
                }));
            }
        }

        private RelayCommand _disableEditionCmd;

        public ICommand DisableEditionCmd
        {
            get
            {
                return _disableEditionCmd ?? (_disableEditionCmd = new RelayCommand(x =>
                {
                    Trace.WriteLine("Disable edit");
                    if (IsEdition)
                        IsEdition = false;
                }));
            }
        }
        #endregion
    }
}