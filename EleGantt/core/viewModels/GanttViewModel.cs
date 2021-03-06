﻿using System;
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
using MaterialDesignThemes.Wpf;

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
        //width & height linked to property
        private double _cellWidth = 50;
        private double _cellHeight = 40;
        //const used to born the width (changing with zoom) : min, max, base
        public const int CELL_WIDTH_MIN = 10;
        public const int CELL_WIDTH_MAX = 100;
        public const int CELL_WIDTH_BASE = 50;

        public GanttViewModel() : this(new GanttModel("")) { }

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

            UpdateDuration();

            OnPropertyChanged(null); //update all fields

            Saved = true; // no modification has pending !
        }

        public string AppName
        {
            get { return "EleGantt - " + (_saved ? _project.Name : _project.Name + " *"); }
        }

        public double CellWidth
        {
            get { return _cellWidth;  }
            set 
            {
                //verify that the value is whitin bounds
                if (value >= CELL_WIDTH_MIN && value <= CELL_WIDTH_MAX)
                {
                    //zoom is NOT saved in the ganttmodel, thus not saved in memory. If the persistance of the zoom is required,
                    //this is a good place to do it (_project.cellwidth = ...). A fresh start is estimated better for now
                    _cellWidth = value;
                    OnPropertyChanged("CellWidth");
                }
            }
        }

        public double CellHeight
        {
            get { return _cellHeight; }
            set
            {
                _cellHeight = value;
                OnPropertyChanged("CellHeight");
            }
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

        public DateTime End
        {
            get { return _project.End; }
            set
            {
                _project.End = value;
                OnPropertyChanged("End");
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
            //avoir chanding the duration property if the duration is the same
            int updated = (_project.End - _project.Start).Days;
            if(updated != _duration)
            {
                Duration = updated;
            }
        }


        public void AddTask(GanttTaskModel task)
        {
            AddTask(task);
        }

        public void AddTask(GanttTaskViewModel task)
        {
            _TaskList.Add(task);
            OnPropertyChanged("Tasks");
            task.ShowEditForm();
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
            milestone.ShowEditForm();
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
                    var item = new GanttTaskViewModel(new GanttTaskModel { Name = "New Task", DateStart = Start, DateEnd = Start.AddDays(2) });
                    AddTask(item);
                    SelectedTask = item;
                }));
            } 
        }

        private RelayCommand _addMilestoneCmd;
        public ICommand AddMilestoneCmd
        {
            get
            {
                return _addMilestoneCmd ?? (_addMilestoneCmd = new RelayCommand(x =>
                {
                    var milestone = new MilestoneViewModel(new MilestoneModel { Name="Milestone", Date=Start });
                    AddMilestone(milestone);
                }));
            }
        }

        private RelayCommand _removeMilestoneCmd;
        /// <summary>
        /// This command allow to remove the given milestone from the project
        /// </summary>
        public ICommand RemoveMilestoneCmd
        {
            get
            {
                return _removeMilestoneCmd ?? (_removeMilestoneCmd = new RelayCommand(milestone =>
                {
                    RemoveMilestone(milestone as MilestoneViewModel);
                    DialogHost.CloseDialogCommand.Execute(null, null);
                }));
            }
        }

        private RelayCommand _openAbout;
        /// <summary>
        /// This command allow to open the "about" dialog
        /// </summary>
        public ICommand OpenAbout
        {
            get
            {
                return _openAbout ?? (_openAbout = new RelayCommand(x =>
                {
                    DialogHost.Show("test", "dialogAbout");
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

        private RelayCommand _resetCell;
        /// <summary>
        /// This command allow to create a new project
        /// </summary>
        public ICommand ResetCell
        {
            get
            {
                return _resetCell ?? (_resetCell = new RelayCommand(x =>
                {
                    CellWidth = CELL_WIDTH_BASE;
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

