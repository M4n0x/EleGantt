using System;
using System.Collections.Generic;

namespace EleGantt.core.models
{
    [Serializable]
    class Gantt
    {
        public string Name;
        public string UUID;
        public IList<GanttTask> Tasks;
        public IList<Milestone> Milestones;

        public Gantt(string Name) : this(Name, Guid.NewGuid().ToString()) { }

        public Gantt(string Name, string UUID)
        {
            this.Name = Name;
            this.UUID = UUID;
        }
    }
}
