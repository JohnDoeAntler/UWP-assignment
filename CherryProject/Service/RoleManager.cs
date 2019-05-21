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
		public static async Task<Roles> FindRoleAsync(Predicate<Roles> predicate)
		{
			using (var context = new Context())
			{
				return await context.Roles.FirstOrDefaultAsync(x => predicate(x));
			}
		}
	}
}
