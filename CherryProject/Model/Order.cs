using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Order
	{
		private string concurrencyStamp;

		public Order()
		{
			Dic = new HashSet<Dic>();
			OrderProduct = new HashSet<OrderProduct>();
		}

		public string Id { get; set; }
		public string DealerId { get; set; }
		public string ModifierId { get; set; }
		public string Type { get; set; }
		public string Status { get; set; }
		public string DeliveryAddress { get; set; }
		public DateTime LastTimeModified { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual User Dealer { get; set; }
		public virtual User Modifier { get; set; }
		public virtual ICollection<Dic> Dic { get; set; }
		public virtual ICollection<OrderProduct> OrderProduct { get; set; }
	}
}
