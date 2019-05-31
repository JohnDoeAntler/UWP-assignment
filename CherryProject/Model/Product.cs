using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Product
	{
		private string concurrencyStamp;
		private DateTime lastTimeModified;

		public Product()
		{
			Category = new HashSet<Category>();
			Did = new HashSet<Did>();
			OrderProduct = new HashSet<OrderProduct>();
			Promotion = new HashSet<Promotion>();
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get => Name.ToUpperInvariant(); set { } }
		public double Price { get; set; }
		public string Description { get; set; }
		public double Weight { get; set; }
		public int DangerLevel { get; set; }
		public string IconUrl { get; set; }
		public string Status { get; set; }
		public DateTime LastTimeModified { get => lastTimeModified == DateTime.MinValue ? (lastTimeModified = DateTime.Now) : lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual ICollection<Category> Category { get; set; }
		public virtual ICollection<Did> Did { get; set; }
		public virtual ICollection<OrderProduct> OrderProduct { get; set; }
		public virtual ICollection<Promotion> Promotion { get; set; }
	}
}
