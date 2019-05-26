using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Products
	{
		private string concurrencyStamp;

		public Products()
		{
			Category = new HashSet<Category>();
			Orderproduct = new HashSet<Orderproduct>();
			Promotions = new HashSet<Promotions>();
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get => Name.ToUpperInvariant(); set { } }
		public double Price { get; set; }
		public string Description { get; set; }
		public double Weight { get; set; }
		public int DangerLevel { get; set; }
		public string Status { get; set; }
		public DateTime LastTimeModified { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual ICollection<Category> Category { get; set; }
		public virtual ICollection<Orderproduct> Orderproduct { get; set; }
		public virtual ICollection<Promotions> Promotions { get; set; }
	}
}
