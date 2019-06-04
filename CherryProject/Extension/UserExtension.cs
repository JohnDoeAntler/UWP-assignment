﻿using CherryProject.Model;
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
				var result = context.User.Include(x => x.Role).FirstOrDefault(x => x.Id == user.Id && x.ConcurrencyStamp == user.ConcurrencyStamp);

				var username = result.UserName;
				var password = result.PasswordHash;

				action(result);

				Debug.WriteLine(result.RoleId);

				if (username != result.UserName || password != result.PasswordHash)
				{
					result.SecurityStamp = Guid.NewGuid().ToString("D");
				}

				result.ConcurrencyStamp = Guid.NewGuid().ToString();

				await context.SaveChangesAsync();

				result.Role = await context.Role.FirstOrDefaultAsync(x => x.Id == result.RoleId);

				return result;
			}
		}
	}
}
