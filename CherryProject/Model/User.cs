using CherryProject.Model.Enum;
using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class User
	{
		private bool emailConfirmed;
		private bool phoneNumberConfirmed;
		private string securityStamp;
		private string concurrencyStamp;

		public User()
		{
			OrderDealer = new HashSet<Order>();
			OrderModifier = new HashSet<Order>();
		}

		public Guid Id { get; set; }
		public string UserName { get; set; }
		public string NormalizedUserName { get => UserName.ToUpperInvariant(); set { } }
		public string FirstName { get; set; }
		public string NormalizedFirstName { get => FirstName.ToUpperInvariant(); set { } }
		public string LastName { get; set; }
		public string NormalizedLastName { get => LastName.ToUpperInvariant(); set { } }
		public string Email { get; set; }
		public string NormalizedEmail { get => Email.ToUpperInvariant(); set { } }
		public bool EmailConfirmed { get => emailConfirmed; set => emailConfirmed = value; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get => securityStamp ?? (securityStamp = Convert.ToBase64String(Guid.NewGuid().ToByteArray())); set => securityStamp = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }
		public string PhoneNumber { get; set; }
		public bool PhoneNumberConfirmed { get => phoneNumberConfirmed; set => phoneNumberConfirmed = value; }
		public string Region { get; set; }
		public string Address { get; set; }
		public string IconUrl { get; set; }
		public GeneralStatusEnum Status { get; set; }
		public RoleEnum Role { get; set; }

		public virtual ICollection<Order> OrderDealer { get; set; }
		public virtual ICollection<Order> OrderModifier { get; set; }
		public virtual ICollection<Notification> NotificationRecipient { get; set; }
		public virtual ICollection<Notification> NotificationSender { get; set; }
	}
}
