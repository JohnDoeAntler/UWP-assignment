using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Dic
	{
		private string concurrencyStamp;

		public Dic()
		{
			Did = new HashSet<Did>();
		}

		public string Id { get; set; }
		public string OrderId { get; set; }
		public string Status { get; set; }
		public DateTime LastTimeModified { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Order Order { get; set; }
		public virtual ICollection<Did> Did { get; set; }
	}
}
