using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Roles
    {
        public Roles()
        {
            Users = new HashSet<Users>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get => Name.ToUpperInvariant() ; set { } }
        public string ConcurrencyStamp { get; set; }

        public virtual ICollection<Users> Users { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}
