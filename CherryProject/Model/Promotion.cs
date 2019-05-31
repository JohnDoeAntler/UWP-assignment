using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Promotion
	{
		private string concurrencyStamp;
		private DateTime timestamp;

		public string Id { get; set; }
		public string ProductId { get; set; }
		public string Description { get; set; }
		public DateTime Timestamp { get => timestamp == DateTime.MinValue ? (timestamp = DateTime.Now) : timestamp; set => timestamp = value; }
		public long Duration { get; set; }
		public decimal Discount { get; set; }
		public string ImageUrl { get; set; }
		public string Status { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Product Product { get; set; }
	}
}
