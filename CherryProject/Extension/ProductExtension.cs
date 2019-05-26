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
		public static async Task<Products> ModifyAsync(this Products product, Action<Products> action)
		{
			using (var context = new Context())
			{
				var result = context.Products.FirstOrDefault(x => x.Id == product.Id && x.ConcurrencyStamp == product.ConcurrencyStamp);
				action(result);
				result.ConcurrencyStamp = Guid.NewGuid().ToString();
				await context.SaveChangesAsync();

				return result;
			}
		}
	}
}
