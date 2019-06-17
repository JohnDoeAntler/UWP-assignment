using CherryProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class PromotionExtension
	{
		public static async Task<Promotion> ModifyAsync(this Promotion product, Action<Promotion> action)
		{
			using (var context = new Context())
			{
				var result = context.Promotion.FirstOrDefault(x => x.Id == product.Id && x.ConcurrencyStamp == product.ConcurrencyStamp);
				action(result);
				result.ConcurrencyStamp = Guid.NewGuid().ToString();
				await context.SaveChangesAsync();

				return result;
			}
		}
	}
}
