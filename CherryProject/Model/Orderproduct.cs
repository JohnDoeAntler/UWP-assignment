using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class OrderProduct
	{
		private string concurrencyStamp;
		private uint quantity = 1;

		public string OrderId { get; set; }
		public string ProductId { get; set; }
		public uint Quantity { get => quantity; set => quantity = value; }
		public DateTime LastTimeModified { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Order Order { get; set; }
		public virtual Product Product { get; set; }
	}
}
