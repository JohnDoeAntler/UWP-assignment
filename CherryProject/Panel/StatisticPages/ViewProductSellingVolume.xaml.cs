using CherryProject.Dialog;
using CherryProject.Extension;
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
	public sealed partial class ViewProductSellingVolume : Page
	{
		private class ViewModel
		{
			public string Name { get; set; }

			public long Value { get; set; }
		}

		private Product selectedProduct;

		public ViewProductSellingVolume()
		{
			this.InitializeComponent();

			From.IsEnabled = !Toggle.IsOn;
			To.IsEnabled = !Toggle.IsOn;
		}

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleSwitch toggle)
			{
				From.IsEnabled = !toggle.IsOn;
				To.IsEnabled = !toggle.IsOn;
			}
		}

		private async void Select_Click(object sender, RoutedEventArgs e)
		{
			ProductDialog dialog = new ProductDialog();

			var button = await dialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				SelectedProductTextBlock.Visibility = Visibility.Visible;
				SelectedProductTextBlock.Text = $"Selected product: {(selectedProduct = dialog.Product).Name}";
				ProductGUID.Text = selectedProduct.Id.ToString();
				View.IsEnabled = true;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			using (var context = new Context())
			{
				if (Toggle.IsOn || !From.Date.HasValue || !To.Date.HasValue)
				{
					radChart.DataContext = context.OrderProduct.Include(x => x.Order).Where(x => x.ProductId == selectedProduct.Id && x.Order.Status == OrderStatusEnum.Endorsed).OrderBy(x => x.LastTimeModified).GroupBy(x => x.LastTimeModified.Date, (x, y) => new { Name = x.ToShortDateString(), Value = y.Sum(z => z.Quantity) }).Select(x => new ViewModel() { Name = x.Name, Value = x.Value }).ToList();
				}
				else
				{
					radChart.DataContext = context.OrderProduct.Include(x => x.Order).Where(x => x.ProductId == selectedProduct.Id && x.Order.Status == OrderStatusEnum.Endorsed && x.LastTimeModified >= From.Date.Value && x.LastTimeModified <= To.Date.Value).OrderBy(x => x.LastTimeModified).GroupBy(x => x.LastTimeModified.Date, (x, y) => new { Name = x.ToShortDateString(), Value = y.Sum(z => z.Quantity) }).Select(x => new ViewModel() { Name = x.Name, Value = x.Value }).ToList();
				}
			}
		}
	}
}
