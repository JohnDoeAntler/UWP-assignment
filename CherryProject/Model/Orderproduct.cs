using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class OrderProduct
	{
		private string concurrencyStamp;
		private uint quantity = 1;
		private DateTime lastTimeModified;

		public string OrderId { get; set; }
		public string ProductId { get; set; }
		public uint Quantity { get => quantity; set => quantity = value; }
		public DateTime LastTimeModified { get => lastTimeModified == DateTime.MinValue ? (lastTimeModified = DateTime.Now) : lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Order Order { get; set; }
		public virtual Product Product { get; set; }
	}
}
