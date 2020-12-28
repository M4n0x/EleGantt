using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace EleGantt.core.models
{
    [Serializable]
    class GanttModel
    {
        public string Name;
        public string UUID;
        public ObservableCollection<GanttTaskModel> Tasks;
        public ObservableCollection<MilestoneModel> Milestones;
        public GanttModel() { }
        public GanttModel(string Name) : this(Name, Guid.NewGuid().ToString()) { }

        public GanttModel(string Name, string UUID)
        {
            this.Name = Name;
            this.UUID = UUID;
            Tasks = new ObservableCollection<GanttTaskModel>();
            Milestones = new ObservableCollection<MilestoneModel>();
        }
    }
}
