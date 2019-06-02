using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Category
	{
		public Category()
		{
			Spare = new HashSet<Spare>();
		}

		private string concurrencyStamp;

		public string Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get => Name.ToUpperInvariant(); set { } }
		public string ProductId { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Product Product { get; set; }
		public virtual ICollection<Spare> Spare { get; set; }
	}
}
