using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Spare
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; }
        public virtual DidSpare DidSpare { get; set; }
    }
}
