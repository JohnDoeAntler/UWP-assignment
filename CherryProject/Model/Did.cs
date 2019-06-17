using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Did
	{
		private DateTime lastTimeModified = DateTime.UtcNow;
		private string concurrencyStamp;

		public Did()
		{
			DidSpare = new HashSet<DidSpare>();
		}

		public Guid Id { get; set; }
		public Guid DicId { get; set; }
		public Guid ProductId { get; set; }
		public uint Quantity { get; set; }
		public DateTime LastTimeModified { get => lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Dic Dic { get; set; }
		public virtual Product Product { get; set; }
		public virtual ICollection<DidSpare> DidSpare { get; set; }
	}
}
