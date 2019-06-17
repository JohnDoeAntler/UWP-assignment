using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class PriceHistory
	{
		private DateTime timestamp = DateTime.UtcNow;

		public Guid ProductId { get; set; }
		public DateTime Timestamp { get => timestamp; set => timestamp = value; }
		public double Price { get; set; }

		public virtual Product Product { get; set; }
	}
}
