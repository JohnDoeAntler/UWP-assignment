using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	public sealed partial class ViewSellingVolume : Page
	{
		private readonly ObservableCollection<SellingVolumeViewModel> products;

		public ViewSellingVolume()
		{
			this.InitializeComponent();

			products = new ObservableCollection<SellingVolumeViewModel>();

			UpdateResult((Year.Date = DateTime.Now).Value.Year);

			Year.DateChanged += (sender, args) => UpdateResult(sender.Date.Value.Year);
		}

		private void UpdateResult(int year)
		{
			using (var context = new Context())
			{
				products.UpdateObservableCollection(context.Product.SelectMany(x => context.OrderProduct.Include(y => y.Product).Include(y => y.Order).ThenInclude(y => y.Dealer).Where(y => x.Id == y.ProductId && y.Order.Status == OrderStatusEnum.Endorsed && y.LastTimeModified.Year == year)).GroupBy(x => new { x.ProductId, x.Order.DealerId }, (i, j) => new SellingVolumeViewModel() {
					Customer = $"{j.FirstOrDefault().Order.Dealer.FirstName} {j.FirstOrDefault().Order.Dealer.LastName}",
					Product = $"{j.FirstOrDefault().Product.Name}",
					Qty1 = j.Where(x => x.LastTimeModified.Month <= 3).Sum(x => x.Quantity),
					Qty2 = j.Where(x => x.LastTimeModified.Month > 3 && x.LastTimeModified.Month <= 6).Sum(x => x.Quantity),
					Qty3 = j.Where(x => x.LastTimeModified.Month > 6 && x.LastTimeModified.Month <= 9).Sum(x => x.Quantity),
					Qty4 = j.Where(x => x.LastTimeModified.Month > 9).Sum(x => x.Quantity),
				}));
			}
		}

		public ObservableCollection<SellingVolumeViewModel> Products { get => products; }
	}
}
