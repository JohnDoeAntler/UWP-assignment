using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Did
	{
		private string concurrencyStamp;
		private DateTime lastTimeModified;

		public Did()
		{
			DidSpare = new HashSet<DidSpare>();
			InverseDic = new HashSet<Did>();
			Invoice = new HashSet<Invoice>();
		}

		public string Id { get; set; }
		public string DicId { get; set; }
		public string ProductId { get; set; }
		public int Quantity { get; set; }
		public DateTime LastTimeModified { get => lastTimeModified == DateTime.MinValue ? (lastTimeModified = DateTime.Now) : lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Did Dic { get; set; }
		public virtual Product Product { get; set; }
		public virtual ICollection<DidSpare> DidSpare { get; set; }
		public virtual ICollection<Did> InverseDic { get; set; }
		public virtual ICollection<Invoice> Invoice { get; set; }
	}
}
