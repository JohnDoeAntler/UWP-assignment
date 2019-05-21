using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Orders
    {
        public Orders()
        {
            Invoices = new HashSet<Invoices>();
            Orderproduct = new HashSet<Orderproduct>();
        }

        public string Id { get; set; }
        public string DealerId { get; set; }
        public string ModifierId { get; set; }
        public string Status { get; set; }
        public DateTime LastTimeModified { get; set; }
        public string ConcurrencyStamp { get; set; }

        public virtual Users Dealer { get; set; }
        public virtual ICollection<Invoices> Invoices { get; set; }
        public virtual ICollection<Orderproduct> Orderproduct { get; set; }
    }
}
