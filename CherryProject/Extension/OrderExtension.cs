using CherryProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class OrderExtension
	{
		public static async Task<Order> ModifyAsync(this Order order, Action<Order> action)
		{
			using (var context = new Context())
			{
				var result = context.Order.FirstOrDefault(x => x.Id == order.Id && x.ConcurrencyStamp == order.ConcurrencyStamp);
				action(result);
				result.ConcurrencyStamp = Guid.NewGuid().ToString();
				await context.SaveChangesAsync();

				return result;
			}
		}
	}
}
