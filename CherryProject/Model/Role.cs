using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Role
	{
		private string concurrencyStamp;

		public Role()
		{
			User = new HashSet<User>();
		}

		public string Id { get; set; }
		public string Name { get; set; }
		public string NormalizedName { get => Name.ToUpperInvariant(); set { } }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual ICollection<User> User { get; set; }

		public override string ToString() => Name;
	}
}
