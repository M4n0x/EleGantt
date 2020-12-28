using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using EleGantt.core.models;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using EleGantt.core.utils;
using Newtonsoft.Json;
using Microsoft.Win32;
using System.IO;

namespace EleGantt.core.viewModels
{
    class GanttViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<GanttTaskViewModel> _TaskList;
        private ObservableCollection<MilestoneViewModel> _MilestoneList;
        private GanttModel _project;
        private GanttTaskViewModel _selectedTask;
        private bool _saved = true;
        private string _filePath;

        public GanttViewModel() : this(new GanttModel("New project")) { }

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
                            vm => vm.GanttTaskModel,
                            (vm, m) => vm.GanttTaskModel.Equals(m)); // checks if the ViewModel corresponds to the specified model
            _TaskList.CollectionChanged += (sender, e) => { Saved = false; };
            _MilestoneList = new BoundObservableCollection<MilestoneViewModel, MilestoneModel>(
                            gantt.Milestones,
                            m => new MilestoneViewModel(m),
                            vm => vm.MilestoneModel,
                            (vm,m) => vm.MilestoneModel.Equals(m));
            _MilestoneList.CollectionChanged += (sender, e) => { Saved = false; };
            OnPropertyChanged(null); //update all fields
            _saved = true;
        }

        public string AppName
        {
            get { return "EleGantt - " + (_saved ? _project.Name : _project.Name + " *"); }
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

        public void AddTask(GanttTaskModel task)
        {
            AddTask(new GanttTaskViewModel(task));
        }

        public void AddTask(GanttTaskViewModel task)
        {
            _TaskList.Add(task);
            OnPropertyChanged("Tasks");
        }

        public void RemoveSelectedTasks(object selectedItems)
        {
            
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

        private bool doFilePathSet()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "EleGantt Project (*.elegantt)|*.elegantt";
            saveFileDialog.DefaultExt = "elegantt";
            saveFileDialog.AddExtension = true;

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
            if (ClosingRequest != null)
            {
                ClosingRequest(this, EventArgs.Empty);
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

        #region Commands 
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

        private RelayCommand _createNewProjectCmd;

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

        public ICommand SaveProjectCmd
        {
            get
            {
                return _saveProjectCmd ?? (_saveProjectCmd = new RelayCommand(x =>
                {
                    if (_filePath == null || (x != null && x.ToString().Equals("u")))
                    {
                        if (!doFilePathSet())
                        {
                            return; // no path has been provided
                        }
                    }

                    File.WriteAllText(_filePath, JsonConvert.SerializeObject(_project));
                    _saved = true;
                }));
            }
        }

        private RelayCommand _openProjectCmd;

        public ICommand OpenProjectCmd
        {
            get
            {
                return _openProjectCmd ?? (_openProjectCmd = new RelayCommand(x =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "EleGantt Project (*.elegantt)|*.elegantt";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        var input = File.ReadAllText(openFileDialog.FileName);
                        GanttModel gantt = JsonConvert.DeserializeObject<GanttModel>(input);
                        LoadGanttModel(gantt);
                        _filePath = openFileDialog.FileName;
                    }
                }));
            }
        }


        private RelayCommand _closeCmd;
        public ICommand CloseCmd
        {
            get
            {
                return _closeCmd ?? (_closeCmd = new RelayCommand(x => OnClosingRequest()));
            }
        }

        public event EventHandler ClosingRequest;

        #endregion
    }

}

