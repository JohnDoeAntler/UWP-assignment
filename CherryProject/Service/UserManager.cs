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
				return await context.User.FirstOrDefaultAsync(x => predicate(x));
			}
		}
	}
}
