using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Orderproduct
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastTimeModified { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}
