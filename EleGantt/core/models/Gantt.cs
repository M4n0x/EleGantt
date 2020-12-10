using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleGantt.core.models
{
    class Gantt
    {
        private string Name { set; get; }
        private List<GanttTask> Tasks;
        private List<Milestone> Milestones;
    }
}
