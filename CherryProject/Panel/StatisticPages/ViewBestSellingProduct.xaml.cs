using CherryProject.Model;
using CherryProject.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.StatisticPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ViewBestSellingProduct : Page
	{
		public ViewBestSellingProduct()
		{
			this.InitializeComponent();

			setTop();
		}

		private void setTop(TimeSpan? days = null)
		{
			using (var context = new Context())
			{
				if (days != null)
				{
					var products = context.Product.Select(x => new
					{
						x.Name,
						Quantity = context.OrderProduct.Include(y => y.Order).Where(y => y.Order.Status == OrderStatusEnum.Endorsed && y.ProductId == x.Id && DateTime.UtcNow - y.LastTimeModified < days).Sum(y => y.Quantity)
					}).OrderByDescending(x => x.Quantity).Take(5);

					for (int i = 0; i < products.Count(); i++)
					{
						var tb = (TextBlock)FindName($"top{i + 1}");
						tb.Text = $"{products.Skip(i).First().Name} (Sales Volume: {products.Skip(i).First().Quantity})";
					}
				}
				else
				{
					var products = context.Product.Select(x => new
					{
						x.Name,
						Quantity = context.OrderProduct.Include(y => y.Order).Where(y => y.Order.Status == OrderStatusEnum.Endorsed && y.ProductId == x.Id).Sum(y => y.Quantity)
					}).OrderByDescending(x => x.Quantity).Take(5);

					for (int i = 0; i < products.Count(); i++)
					{
						var tb = (TextBlock)FindName($"top{i + 1}");
						tb.Text = $"{products.Skip(i).First().Name} (Sales Volume: {products.Skip(i).First().Quantity})";
					}
				}
			}
		}

		private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
		{
			setTop();
		}

		private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine(new TimeSpan(31, 0, 0, 0).TotalDays);
			setTop(new TimeSpan(31, 0, 0, 0));
		}

		private void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
		{
			setTop(new TimeSpan(7, 0, 0, 0));
		}
	}
}
