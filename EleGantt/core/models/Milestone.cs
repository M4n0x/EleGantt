using System;
using System.ComponentModel;

namespace EleGantt.core.models
{
    [Serializable]
    internal class Milestone
    {
        public string Name;
        public DateTime Date;
        public string UUID = Guid.NewGuid().ToString();
    }

}