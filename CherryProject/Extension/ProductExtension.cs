using CherryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class ProductExtension
	{
		public static async Task<Product> ModifyAsync(this Product product, Action<Product> action)
		{
			using (var context = new Context())
			{
				var result = context.Product.FirstOrDefault(x => x.Id == product.Id && x.ConcurrencyStamp == product.ConcurrencyStamp);
				action(result);
				result.ConcurrencyStamp = Guid.NewGuid().ToString();
				await context.SaveChangesAsync();

				return result;
			}
		}
	}
}
