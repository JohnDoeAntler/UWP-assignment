using CherryProject.Model.Enum;
using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Product
	{
		private DateTime lastTimeModified = DateTime.UtcNow;
		private string concurrencyStamp;

		public Product()
		{
			Category = new HashSet<Category>();
			Did = new HashSet<Did>();
			OrderProduct = new HashSet<OrderProduct>();
			PriceHistory = new HashSet<PriceHistory>();
			Promotion = new HashSet<Promotion>();
		}

		public Guid Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get => Name.ToUpperInvariant(); set { } }
		public string Description { get; set; }
		public double Weight { get; set; }
		public int ReorderLevel { get; set; }
		public int DangerLevel { get; set; }
		public string IconUrl { get; set; }
		public GeneralStatusEnum Status { get; set; }
		public DateTime LastTimeModified { get => lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual ICollection<Category> Category { get; set; }
		public virtual ICollection<Did> Did { get; set; }
		public virtual ICollection<OrderProduct> OrderProduct { get; set; }
		public virtual ICollection<PriceHistory> PriceHistory { get; set; }
		public virtual ICollection<Promotion> Promotion { get; set; }
	}
}
