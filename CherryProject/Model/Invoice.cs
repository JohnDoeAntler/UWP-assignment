using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Invoice
	{
		private string concurrencyStamp;

		public string Id { get; set; }
		public string DidId { get; set; }
		public string Status { get; set; }
		public DateTime Timestamp { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Did Did { get; set; }
	}
}
