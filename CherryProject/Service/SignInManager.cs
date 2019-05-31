using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Service.SignStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Service
{
	namespace SignStatus{
		public enum Status
		{
			Success,
			UsernameFailure,
			PasswordFailure,
			Disabled
		}
	}

	public class SignInManager
	{
		public static User CurrentUser { get; private set; }

		public static async Task<Status> SignInAsync(string username, string password)
		{
			using (var context = new Context())
			{
				var result = await UserManager.FindUserAsync(x => x.UserName == username);

				// if user in not null.
				if (result != null)
				{
					// if the password is matched
					if (result.PasswordHash.Equals(password.GetMD5hash()))
					{
						// if the account is not disabled.
						if (!result.Status.Equals("Disabled", StringComparison.OrdinalIgnoreCase))
						{
							CurrentUser = result;
							return Status.Success;
						}else return Status.Disabled;
					}else return Status.PasswordFailure;
				}else return Status.UsernameFailure;
			}
		}

		public static void SignOutAsync() => CurrentUser = null;

		public static async Task<bool> ValidateSecurityStamp() => (await UserManager.FindUserAsync(x => x.Id == CurrentUser.Id)).SecurityStamp == CurrentUser.SecurityStamp;
	}
}
