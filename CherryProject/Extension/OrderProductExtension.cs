using CherryProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class OrderProductExtension
	{
		public static async Task<OrderProduct> ModifyAsync(this OrderProduct orderProduct, Action<OrderProduct> action)
		{
			using (var context = new Context())
			{
				var result = context.OrderProduct.FirstOrDefault(x => 
					x.OrderId == orderProduct.OrderId 
					&& x.ProductId == orderProduct.ProductId 
					&& x.ConcurrencyStamp == orderProduct.ConcurrencyStamp
				);

				action(result);

				result.ConcurrencyStamp = Guid.NewGuid().ToString();
				await context.SaveChangesAsync();

				return result;
			}
		}
	}
}
