﻿using CherryProject.Model;
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
		public static async Task<Users> FindUserAsync(Predicate<Users> predicate)
		{
			using (var context = new Context())
			{
				return await context.Users.Include(x => x.Role).FirstOrDefaultAsync(x => predicate(x));
			}
		}

		public static async Task<Users> CreateAsync(Users user)
		{
			using (var context = new Context())
			{
				var entity = await context.Users.AddAsync(user);
				await context.SaveChangesAsync();
				return entity.Entity;
			}
		}
	}
}
