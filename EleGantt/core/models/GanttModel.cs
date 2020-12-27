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
        public string Path;
        public ObservableCollection<GanttTaskModel> Tasks = new ObservableCollection<GanttTaskModel>
            {
                new GanttTaskModel{Name="test",  DateStart=DateTime.Now, DateEnd=DateTime.Now.AddDays(2)},
                new GanttTaskModel{Name="Test 123",  DateStart=DateTime.Now.AddDays(3), DateEnd=DateTime.Now.AddDays(6)},
            };
        public ObservableCollection<MilestoneModel> Milestones = new ObservableCollection<MilestoneModel>
            {
                new MilestoneModel{Name="test",  Date=DateTime.Now},
                new MilestoneModel{Name="Test 123",  Date=DateTime.Now.AddDays(3)},
            };

        public GanttModel(string Name) : this(Name, Guid.NewGuid().ToString()) { }

        public GanttModel(string Name, string UUID)
        {
            this.Name = Name;
            this.UUID = UUID;
        }
    }
}
