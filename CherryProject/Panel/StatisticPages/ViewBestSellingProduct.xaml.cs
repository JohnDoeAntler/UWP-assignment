using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private readonly ObservableCollection<BestSellingProductViewModel> products;

		public ViewBestSellingProduct()
		{
			this.InitializeComponent();

			From.IsEnabled = !Toggle.IsOn;
			To.IsEnabled = !Toggle.IsOn;

			products = new ObservableCollection<BestSellingProductViewModel>();
		}

		public ObservableCollection<BestSellingProductViewModel> Products => products;

		private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleSwitch toggle)
			{
				From.IsEnabled = !toggle.IsOn;
				To.IsEnabled = !toggle.IsOn;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			using (var context = new Context())
			{
				if (Toggle.IsOn || !From.Date.HasValue || !To.Date.HasValue)
				{
					products.UpdateObservableCollection(context.Product.Select(x => new BestSellingProductViewModel()
					{
						Name = x.Name,
						Quantity = context.OrderProduct.Include(y => y.Order).Where(y => y.Order.Status == OrderStatusEnum.Endorsed && y.ProductId == x.Id).Sum(y => y.Quantity),
						Icon = ValidOrDefault(x.IconUrl)
					}).OrderByDescending(x => x.Quantity).Take((int)Slider.Value));
				}
				else
				{
					products.UpdateObservableCollection(context.Product.Select(x => new BestSellingProductViewModel()
					{
						Name = x.Name,
						Quantity = context.OrderProduct.Include(y => y.Order).Where(y => y.Order.Status == OrderStatusEnum.Endorsed && y.ProductId == x.Id && x.LastTimeModified >= From.Date.Value && x.LastTimeModified <= To.Date).Sum(y => y.Quantity),
						Icon = ValidOrDefault(x.IconUrl)
					}).OrderByDescending(x => x.Quantity).Take((int)Slider.Value));
				}
			}
		}

		private static string ValidOrDefault(string uri)
		{
			if (Uri.TryCreate(uri, UriKind.Absolute, out Uri iconUrl) && iconUrl != null && (iconUrl.Scheme == Uri.UriSchemeHttp || iconUrl.Scheme == Uri.UriSchemeHttps))
			{
				return uri;
			}

			return "ms-appx:///Assets/Account.jpg";
		}
	}
}
