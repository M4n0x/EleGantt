using System;
using System.Collections.Generic;

namespace EleGantt.core.models
{
    /// <summary>
    /// This model is used to represent a Task
    /// </summary>
    [Serializable]
    public class GanttTaskModel
    {
        public string Name;
        public DateTime DateStart;
        public DateTime DateEnd;
    }
}