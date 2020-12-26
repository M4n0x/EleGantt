using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace EleGantt.core.models
{
    [Serializable]
    internal class GanttTask
    {
        public string Name;
        public DateTime DateStart;
        public DateTime DateEnd;
        public IList<GanttTask> SubTasks;
        public string UUID = Guid.NewGuid().ToString();
    }
}