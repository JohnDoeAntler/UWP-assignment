using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Spare
    {
        public Spare()
        {
            DidSpare = new HashSet<DidSpare>();
        }

        public string Id { get; set; }
        public double PositionXoffset { get; set; }
        public double PositionYoffset { get; set; }
        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<DidSpare> DidSpare { get; set; }
    }
}
