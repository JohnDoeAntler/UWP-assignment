using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class DidSpare
    {
        public string DidId { get; set; }
        public string SpareId { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual Did Did { get; set; }
        public virtual Spare Spare { get; set; }
    }
}
