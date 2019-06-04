using CherryProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class DicExtension
	{
		public static async Task<Dic> ModifyAsync(this Dic dic, Action<Dic> action)
		{
			using (var context = new Context())
			{
				var result = context.Dic.FirstOrDefault(x => x.Id == dic.Id && x.ConcurrencyStamp == dic.ConcurrencyStamp);
				action(result);
				result.ConcurrencyStamp = Guid.NewGuid().ToString();
				await context.SaveChangesAsync();

				return result;
			}
		}
	}
}
