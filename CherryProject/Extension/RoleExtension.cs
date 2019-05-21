using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class RoleExtension
	{
		public static RoleEnum ToRoleEnum(this string str)
		{
			Enum.TryParse(str, out RoleEnum result);
			return result;
		}

		public static RoleEnum ToRoleEnum(this Roles role)
		{
			Enum.TryParse(role.Name, out RoleEnum result);
			return result;
		}

		public static async Task<Roles> ToRoleAsync(this RoleEnum role)
		{
			return await RoleManager.FindRoleAsync(x => x.NormalizedName == role.ToString().ToUpper());
		}

		public static async Task<Roles> ToRoleAsync(this string str)
		{
			return await RoleManager.FindRoleAsync(x => x.NormalizedName == str.ToUpper());
		}
	}
}
