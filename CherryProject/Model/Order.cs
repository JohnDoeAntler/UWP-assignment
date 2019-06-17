using CherryProject.Model.Enum;
using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Order
	{
		private DateTime lastTimeModified = DateTime.UtcNow;
		private string concurrencyStamp;

		public Order()
		{
			Dic = new HashSet<Dic>();
			OrderProduct = new HashSet<OrderProduct>();
		}

		public Guid Id { get; set; }
		public Guid DealerId { get; set; }
		public Guid ModifierId { get; set; }
		public string DeliveryAddress { get; set; }
		public OrderTypeEnum Type { get; set; }
		public OrderStatusEnum Status { get; set; }
		public DateTime LastTimeModified { get => lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual User Dealer { get; set; }
		public virtual User Modifier { get; set; }
		public virtual ICollection<Dic> Dic { get; set; }
		public virtual ICollection<OrderProduct> OrderProduct { get; set; }
	}
}
