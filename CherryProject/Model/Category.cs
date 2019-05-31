using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Category
	{
		private string concurrencyStamp;

		public string Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get => Name.ToUpperInvariant(); set { } }
		public string ProductId { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Product Product { get; set; }
		public virtual Spare Spare { get; set; }
	}
}
