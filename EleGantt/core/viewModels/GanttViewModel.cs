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

namespace EleGantt.core.viewModels
{
    class GanttViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<GanttTaskViewModel> _TaskList;
        private ObservableCollection<MilestoneViewModel> _MilestoneList;
        private GanttModel _project;
        private GanttTaskViewModel _selectedTask;
        private GanttTaskViewModel _editing;
        private bool _saved = true;

        public GanttViewModel() : this(new GanttModel("New project")) { }

        public GanttViewModel(GanttModel gantt)
        {
            LoadGanttModel(gantt);
        }

        private void LoadGanttModel(GanttModel gantt)
        {
            _selectedTask = null;
            _editing = null;
            _project = gantt;
            _TaskList = new BoundObservableCollection<GanttTaskViewModel, GanttTaskModel>(
                            gantt.Tasks,
                            m => new GanttTaskViewModel(m), // creates a ViewModel from a Model
                            (vm, m) => vm.GanttTaskModel.Equals(m)); // checks if the ViewModel corresponds to the specified model
            _MilestoneList = new BoundObservableCollection<MilestoneViewModel, MilestoneModel>(
                            gantt.Milestones,
                            m => new MilestoneViewModel(m),
                            (vm,m) => vm.MilestoneModel.Equals(m));
            OnPropertyChanged(null); //update all fields 
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

        public void DoSerialization()
        {
            //@TODO
            //Use project's UUID to preserve consistency between projects
        }

        protected void OnClosingRequest()
        {
            DoSerialization();
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
                    Trace.WriteLine("Save project... TODO");
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
                    Trace.WriteLine("Open project... TODO");
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

