using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Category
	{
		private string concurrencyStamp;

		public Category()
		{
			Spare = new HashSet<Spare>();
		}

		public Guid Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get => Name.ToUpperInvariant(); set { } }
		public Guid ProductId { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Product Product { get; set; }
		public virtual ICollection<Spare> Spare { get; set; }
	}
}
