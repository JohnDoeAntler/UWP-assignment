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
        public decimal PositionXoffset { get; set; }
        public decimal PositionYoffset { get; set; }
        public string CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<DidSpare> DidSpare { get; set; }
    }
}
