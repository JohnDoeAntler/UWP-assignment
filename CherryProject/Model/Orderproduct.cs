using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class OrderProduct
	{
		private uint quantity = 1;
		private DateTime lastTimeModified = DateTime.UtcNow;
		private string concurrencyStamp;

		public Guid OrderId { get; set; }
		public Guid ProductId { get; set; }
		public uint Quantity { get => quantity; set => quantity = value; }
		public DateTime LastTimeModified { get => lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Order Order { get; set; }
		public virtual Product Product { get; set; }
	}
}
