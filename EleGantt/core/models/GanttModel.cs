using System;
using System.Collections.ObjectModel;

namespace EleGantt.core.models
{
    /// <summary>
    /// This class is used to represent a project
    /// It contains all the tasks and the milestones
    /// </summary>
    [Serializable]
    class GanttModel
    {
        public string Name;
        public string UUID; // in case we have models with same names
        public ObservableCollection<GanttTaskModel> Tasks;
        public ObservableCollection<MilestoneModel> Milestones;
        public DateTime Start;
        public DateTime End;
        public GanttModel() { }
        public GanttModel(string Name) : this(Name, Guid.NewGuid().ToString()) { }

        public GanttModel(string Name, string UUID)
        {
            this.Name = Name;
            this.UUID = UUID;
            Tasks = new ObservableCollection<GanttTaskModel>();
            Milestones = new ObservableCollection<MilestoneModel>();
            Start = DateTime.Now;
            End = Start.AddMonths(1);
        }
    }
}
