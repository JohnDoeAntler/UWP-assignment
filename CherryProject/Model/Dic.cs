using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Dic
	{
		private DateTime lastTimeModified = DateTime.UtcNow;
		private string concurrencyStamp;

		public Dic()
		{
			Did = new HashSet<Did>();
		}

		public Guid Id { get; set; }
		public Guid OrderId { get; set; }
		public string Status { get; set; }
		public DateTime LastTimeModified { get => lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Order Order { get; set; }
		public virtual ICollection<Did> Did { get; set; }
	}
}
