using CherryProject.Model;
using CherryProject.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.ViewModel
{
	public class ProductViewModel
	{
		private Product _product;

		public ProductViewModel(Product product, long stock = 0)
		{
			_product = product;
			AvailableStock = stock;
		}

		public Guid Id { get; }

		public string Name { get => _product.Name; }

		public double Price { get => _product.PriceHistory.Where(x => x.Timestamp < DateTime.UtcNow).OrderByDescending(x => x.Timestamp).FirstOrDefault().Price; }

		public int ReorderLevel { get => _product.ReorderLevel; }

		public int DangerLevel { get => _product.DangerLevel; }

		public long AvailableStock { get; }

		public GeneralStatusEnum Status { get => _product.Status; }
	}
}
