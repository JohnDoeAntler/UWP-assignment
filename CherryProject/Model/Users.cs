using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
    public partial class Users
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get => UserName.ToUpperInvariant(); set { } }
        public string FirstName { get; set; }
        public string NormalizedFirstName { get => FirstName.ToUpperInvariant(); set { } }
        public string LastName { get; set; }
        public string NormalizedLastName { get => LastName.ToUpperInvariant(); set { } }
        public string Email { get; set; }
        public string NormalizedEmail { get => Email.ToUpperInvariant(); set { } }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string RoleId { get; set; }

        public virtual Roles Role { get; set; }
        public virtual Orders Orders { get; set; }
    }
}
