using System;
using System.ComponentModel;
using System.Windows.Input;
using EleGantt.core.models;
using MaterialDesignThemes.Wpf;

namespace EleGantt.core.viewModels
{

    /// <summary>
    /// This viewModel is used to hold task'clas
    /// </summary>
    public class GanttTaskViewModel : INotifyPropertyChanged
    {
        private GanttTaskModel _task;
        private bool _isSelected;
        private bool _isEdition;

        public GanttTaskViewModel(GanttTaskModel task)
        {
            _task = task;
        }

        public GanttTaskModel GanttTaskModel
        {
            get { return _task; }
        }


        public int Length
        {
            get { return (_task.DateEnd - _task.DateStart).Days; }
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
                OnPropertyChanged("Length");
            }
            get { return _task.DateStart; }
        }
        public DateTime DateEnd
        {
            set
            {
                _task.DateEnd = value;
                OnPropertyChanged("DateEnd");
                OnPropertyChanged("Length");
            }
            get { return _task.DateEnd; }
        }

        public void ShowEditForm()
        {
            DialogHost.Show(this, "dialog1");
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
        
        /// <summary>
        /// This command is used to enable the edition of this task
        /// </summary>
        private RelayCommand _enableEditionCmd;
        public ICommand EnableEditionCmd
        {
            get
            {
                return _enableEditionCmd ?? (_enableEditionCmd = new RelayCommand(x =>
                {
                    if (!IsEdition) // avoid firing unncessary OnPropertyChange if it's already in edit mode
                        IsEdition = true;
                }));
            }
        }

        private RelayCommand _showTaskDialog;
        public ICommand ShowTaskDialog
        {
            get
            {
                return _showTaskDialog ?? (_showTaskDialog = new RelayCommand(x =>
                {
                    ShowEditForm();
                }));
            }
        }

        /// <summary>
        /// This command is used to disable edition of the task 
        /// </summary>
        private RelayCommand _disableEditionCmd;
        public ICommand DisableEditionCmd
        {
            get
            {
                return _disableEditionCmd ?? (_disableEditionCmd = new RelayCommand(x =>
                {
                    if (IsEdition) // avoid firing unncessary OnPropertyChange if it's not in edit mode 
                        IsEdition = false;
                }));
            }
        }
        #endregion
    }
}