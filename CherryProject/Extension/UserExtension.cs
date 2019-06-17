using CherryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class UserExtension
	{
		public static async Task<User> ModifyAsync(this User user, Action<User> action)
		{
			using (var context = new Context())
			{
				var result = context.User.FirstOrDefault(x => x.Id == user.Id && x.ConcurrencyStamp == user.ConcurrencyStamp);

				var username = result.UserName;
				var password = result.PasswordHash;
				var roleId = result.Role;

				action(result);

				if (username != result.UserName || password != result.PasswordHash || roleId != result.Role)
				{
					result.SecurityStamp = Guid.NewGuid().ToString("D");
				}

				result.ConcurrencyStamp = Guid.NewGuid().ToString();

				await context.SaveChangesAsync();

				return result;
			}
		}
	}
}
