using System;
using System.Collections.Generic;

namespace EleGantt.core.models
{
    [Serializable]
    class GanttTaskModel
    {
        public string Name;
        public DateTime DateStart;
        public DateTime DateEnd;
    }
}