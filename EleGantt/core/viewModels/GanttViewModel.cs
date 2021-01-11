using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using EleGantt.core.models;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using EleGantt.core.utils;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace EleGantt.core.viewModels
{
    class GanttViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<GanttTaskViewModel> _TaskList;
        private ObservableCollection<MilestoneViewModel> _MilestoneList;
        private GanttModel _project;
        private GanttTaskViewModel _selectedTask;
        private int _duration;
        private bool _saved = true; // allow to know if there is pending modifications
        private string _filePath; // save the path of the current loaded project

        public GanttViewModel() : this(new GanttModel("Project Name")) { }

        public GanttViewModel(GanttModel gantt)
        {
            LoadGanttModel(gantt);
        }

        private void LoadGanttModel(GanttModel gantt)
        {
            _selectedTask = null;
            _project = gantt;
            _filePath = null;
            _TaskList = new BoundObservableCollection<GanttTaskViewModel, GanttTaskModel>(
                            gantt.Tasks,
                            m => new GanttTaskViewModel(m), // creates a ViewModel from a Model
                            vm => vm.GanttTaskModel, // retrieve the source object
                            (vm, m) => vm.GanttTaskModel.Equals(m)); // checks if the ViewModel corresponds to the specified model
            _MilestoneList = new BoundObservableCollection<MilestoneViewModel, MilestoneModel>(
                            gantt.Milestones,
                            m => new MilestoneViewModel(m), // creates a ViewModel from a Model
                            vm => vm.MilestoneModel, // retrieve the source object
                            (vm,m) => vm.MilestoneModel.Equals(m)); // checks if the ViewModel corresponds to the specified model

            // this allow to track modifications on project (this is basic, should be improved)
            _TaskList.CollectionChanged += (sender, e) => { Saved = false; }; 
            _MilestoneList.CollectionChanged += (sender, e) => { Saved = false; };

            OnPropertyChanged(null); //update all fields

            Saved = false; // no modification has pending !
        }

        public string AppName
        {
            get { return "EleGantt - " + (_saved ? _project.Name : _project.Name + " *"); }
        }

        public bool IsDark
        {
            get { return Properties.Settings.Default.isDark; } // special we return the settings provided by the user
            set
            {
                Properties.Settings.Default.isDark = value; // if the user change the theme we change it in the settings to retrieve it when the app is closed
                Properties.Settings.Default.Save();
                OnPropertyChanged("IsDark");
            }
        }
        public bool Saved
        {
            get { return _saved; }
            set
            {
                _saved = value;
                OnPropertyChanged("AppName");
            }
        }
        public string Name
        {
            get { return _project.Name; }
            set 
            { 
                _project.Name = value;
                OnPropertyChanged("Name");
                OnPropertyChanged("AppName");
            }
        }
        public GanttTaskViewModel SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        public ObservableCollection<GanttTaskViewModel> Tasks
        {
            get { return _TaskList; }
            set 
            { 
                _TaskList = value;
                OnPropertyChanged("Tasks");
            }
        }

        public ObservableCollection<MilestoneViewModel> Milestones
        {
            get { return _MilestoneList; }
            set 
            {
                _MilestoneList = value;
                OnPropertyChanged("Milestones");
                UpdateDuration();
            }
        }

        public DateTime Start
        {
            get { return _project.Start; }
            set
            {
                _project.Start = value;
                OnPropertyChanged("Start");
                UpdateDuration();
            }
        }

        public int Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                OnPropertyChanged("Duration");
            }
        }

        private void UpdateDuration()
        {
            int updated = (_project.End - _project.Start).Days;
            if(updated != _duration)
            {
                Duration = updated;
            }
        }

        public DateTime End
        {
            get { return _project.End; }
            set
            {
                _project.End = value;
                OnPropertyChanged("End");
            }
        }

        public void AddTask(GanttTaskModel task)
        {
            AddTask(new GanttTaskViewModel(task));
        }

        public void AddTask(GanttTaskViewModel task)
        {
            _TaskList.Add(task);
            OnPropertyChanged("Tasks");
        }

        public void RemoveTask(GanttTaskViewModel task)
        {
            if (_TaskList.Remove(task))
            {
                OnPropertyChanged("Tasks");
            }
        }

        public void AddMilestone(MilestoneModel milestone)
        {
            AddMilestone(new MilestoneViewModel(milestone));
        }

        public void AddMilestone(MilestoneViewModel milestone)
        {
            _MilestoneList.Add(milestone);
            OnPropertyChanged("Milestones");
        }

        public void RemoveMilestone(MilestoneViewModel milestone)
        {
            if (_MilestoneList.Remove(milestone))
            {
                OnPropertyChanged("Milestones");
            }
        }

        /// <summary>
        /// This function ask the user a file path and save for later use
        /// </summary>
        /// <returns>Return true if user selected a path, false otherwise</returns>
        private bool DoFilePathSet()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "EleGantt Project (*.elegantt)|*.elegantt",
                DefaultExt = "elegantt",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                _filePath = saveFileDialog.FileName;
                return true;
            } else
            {
                return false;
            }
        }

        protected void OnClosingRequest()
        {
            ClosingRequest?.Invoke(this, EventArgs.Empty);
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

        #region Commands

        /// <summary>
        /// these commands can be called by any component in WPF's view where this modelView is accessible
        /// </summary>
        private RelayCommand _addTaskCmd;
        public ICommand AddTaskCmd { 
            get 
            {
                return _addTaskCmd ?? (_addTaskCmd = new RelayCommand(x => 
                {
                    //TODO Adjust datestart date related to the project, same for the end date 
                    var item = new GanttTaskViewModel(new GanttTaskModel { Name = "New Task", DateStart = DateTime.Now, DateEnd = DateTime.Now.AddDays(2) });
                    AddTask(item);
                    SelectedTask = item;
                }));
            } 
        }

        private RelayCommand _removeSelectedTasksCmd;
        /// <summary>
        /// This command allow to remove current selected task from the list
        /// </summary>
        public ICommand RemoveSelectedTasksCmd
        {
            get
            {
                return _removeSelectedTasksCmd ?? (_removeSelectedTasksCmd = new RelayCommand(selectedItems => 
                {
                    var listItems = selectedItems as IList;
                    List<GanttTaskViewModel> items = listItems.Cast<GanttTaskViewModel>().ToList();
                    if (items != null && items.Count > 0)
                    {
                        foreach (GanttTaskViewModel item in items) _TaskList.Remove(item);
                    }
                }));
            }
        }

        private RelayCommand _editSelectedTasksCmd;
        /// <summary>
        /// This command allow to edit the current selected task
        /// </summary>
        public ICommand EditSelectedTasksCmd
        {
            get
            {
                return _editSelectedTasksCmd ?? (_editSelectedTasksCmd = new RelayCommand(selectedItems =>
                {
                    var listItems = selectedItems as IList;
                    List<GanttTaskViewModel> items = listItems.Cast<GanttTaskViewModel>().ToList();
                    if (items != null && items.Count > 0)
                    {
                        items[0].IsEdition = true;
                    }
                }));
            }
        }

        private RelayCommand _createNewProjectCmd;
        /// <summary>
        /// This command allow to create a new project
        /// </summary>
        public ICommand CreateNewProjectCmd
        {
            get
            {
                return _createNewProjectCmd ?? (_createNewProjectCmd = new RelayCommand(x =>
                {
                    LoadGanttModel(new GanttModel("New project"));
                }));
            }
        }

        private RelayCommand _saveProjectCmd;
        /// <summary>
        /// This command allow to save the current project, specify CommandParameter with "u" to save on a different location
        /// </summary>
        public ICommand SaveProjectCmd
        {
            get
            {
                return _saveProjectCmd ?? (_saveProjectCmd = new RelayCommand(x =>
                {
                    if (_filePath == null || (x != null && x.ToString().Equals("u"))) // if the parameter u is set, that mean we want to specify the path
                    {
                        if (!DoFilePathSet())
                        {
                            return; // no path has been provided so we do nothing
                        }
                    }

                    File.WriteAllText(_filePath, JsonConvert.SerializeObject(_project));
                    Saved = true;
                }));
            }
        }

        private RelayCommand _openProjectCmd;
        /// <summary>
        /// This command allow to open a project
        /// </summary>
        public ICommand OpenProjectCmd
        {
            get
            {
                return _openProjectCmd ?? (_openProjectCmd = new RelayCommand(x =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Filter = "EleGantt Project (*.elegantt)|*.elegantt"
                    };
                    if (openFileDialog.ShowDialog() == true)
                    {
                        var input = File.ReadAllText(openFileDialog.FileName);
                        GanttModel gantt = JsonConvert.DeserializeObject<GanttModel>(input);
                        LoadGanttModel(gantt);
                        _filePath = openFileDialog.FileName; // we save the path used to allow quick save (CTRL + S)
                    }
                }));
            }
        }


        private RelayCommand _closeCmd;
        /// <summary>
        /// This command is used to close the project
        /// </summary>
        public ICommand CloseCmd
        {
            get { return _closeCmd ?? (_closeCmd = new RelayCommand(x => OnClosingRequest())); }
        }

        public event EventHandler ClosingRequest;

        #endregion


    }

}

