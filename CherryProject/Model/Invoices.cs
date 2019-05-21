using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Invoices
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual Orders Order { get; set; }
    }
}
