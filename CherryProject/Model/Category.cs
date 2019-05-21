using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ProductId { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual Products Product { get; set; }
        public virtual Spare Spare { get; set; }
    }
}
