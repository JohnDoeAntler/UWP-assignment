using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Promotions
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public long Duration { get; set; }
        public decimal Discount { get; set; }
        public string Status { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual Products Product { get; set; }
    }
}
