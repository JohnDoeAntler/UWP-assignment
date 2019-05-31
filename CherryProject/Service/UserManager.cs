using CherryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Service
{
	public class UserManager
	{
		public static async Task<User> FindUserAsync(Predicate<User> predicate)
		{
			using (var context = new Context())
			{
				return await context.User.Include(x => x.Role).FirstOrDefaultAsync(x => predicate(x));
			}
		}

		public static async Task<User> CreateAsync(User user)
		{
			using (var context = new Context())
			{
				var entity = await context.User.AddAsync(user);
				await context.SaveChangesAsync();
				return entity.Entity;
			}
		}
	}
}
