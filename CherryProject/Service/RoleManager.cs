using CherryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Service
{
	public class RoleManager
	{
		public static async Task<Role> FindRoleAsync(Predicate<Role> predicate)
		{
			using (var context = new Context())
			{
				return await context.Role.FirstOrDefaultAsync(x => predicate(x));
			}
		}
	}
}
