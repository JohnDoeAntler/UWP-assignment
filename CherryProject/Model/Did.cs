using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Did
	{
		private string concurrencyStamp;

		public Did()
		{
			DidSpare = new HashSet<DidSpare>();
			Invoice = new HashSet<Invoice>();
		}

		public string Id { get; set; }
		public string DicId { get; set; }
		public string ProductId { get; set; }
		public uint Quantity { get; set; }
		public DateTime LastTimeModified { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Dic Dic { get; set; }
		public virtual Product Product { get; set; }
		public virtual ICollection<DidSpare> DidSpare { get; set; }
		public virtual ICollection<Invoice> Invoice { get; set; }
	}
}
