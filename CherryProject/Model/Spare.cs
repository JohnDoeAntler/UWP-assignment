using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Spare
    {
        public string Id { get; set; }
        public decimal PositionXoffset { get; set; }
        public decimal PositionYoffset { get; set; }
        public string CategoryId { get; set; }
        public string Status { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual Category Category { get; set; }
    }
}
