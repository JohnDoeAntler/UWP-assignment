using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class DidSpare
	{
		private DateTime timestamp = DateTime.UtcNow;

		public Guid DidId { get; set; }
		public Guid SpareId { get; set; }
		public DateTime Timestamp { get => timestamp; set => timestamp = value; }

		public virtual Did Did { get; set; }
		public virtual Spare Spare { get; set; }
	}
}
