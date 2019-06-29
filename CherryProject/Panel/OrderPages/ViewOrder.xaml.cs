using CherryProject.Attribute;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Service;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.OrderPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Hidden]
	public sealed partial class ViewPromotion : Page
    {
		private readonly ObservableCollection<ViewTuple> displayItems;

		private readonly ObservableCollection<OrderProductViewModel> items;

		public ViewPromotion()
        {
            this.InitializeComponent();

			displayItems = new ObservableCollection<ViewTuple>();
			items = new ObservableCollection<OrderProductViewModel>();

			// permision control
			ModifyOrder.IsEnabled = PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(ModifyOrder));
			EndorseOrder.IsEnabled = PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(EndorseOrder));
			CancelOrder.IsEnabled = PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(CancelOrder));
		}

		public ObservableCollection<ViewTuple> DisplayItems => displayItems;

		public ObservableCollection<OrderProductViewModel> Items => items;

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Order order)
			{
				SetOrderInformation(order);

				ModifyOrder.Click += (sender, args) => Frame.Navigate(typeof(ModifyOrder), order, new DrillInNavigationTransitionInfo());
				EndorseOrder.Click += (sender, args) => Frame.Navigate(typeof(EndorseOrder), order, new DrillInNavigationTransitionInfo());
				CancelOrder.Click += (sender, args) => Frame.Navigate(typeof(CancelOrder), order, new DrillInNavigationTransitionInfo());
			}
			else if (e.Parameter is Guid id)
			{
				Order tmp;

				using (var context = new Context())
				{
					tmp = context.Order.FirstOrDefault(x => x.Id == id);
				}

				SetOrderInformation(tmp);

				ModifyOrder.Click += (sender, args) => Frame.Navigate(typeof(ModifyOrder), tmp, new DrillInNavigationTransitionInfo());
				EndorseOrder.Click += (sender, args) => Frame.Navigate(typeof(EndorseOrder), tmp, new DrillInNavigationTransitionInfo());
				CancelOrder.Click += (sender, args) => Frame.Navigate(typeof(CancelOrder), tmp, new DrillInNavigationTransitionInfo());
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose a order before viewing it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(SearchOrders), typeof(ViewPromotion), new DrillInNavigationTransitionInfo());
			}
		}

		private async void SetOrderInformation(Order order)
		{
			var dealer = await UserManager.FindUserAsync(x => x.Id == order.DealerId);
			var modifier = await UserManager.FindUserAsync(x => x.Id == order.ModifierId);

			OrderName.Text = $"{dealer.FirstName}'s Order";
			OrderId.Text = order.Id.ToString();

			displayItems.Add(new ViewTuple("Dealer Name", $"{dealer.FirstName} {dealer.LastName}"));
			displayItems.Add(new ViewTuple("Last Modifier Name", $"{modifier.FirstName} {modifier.LastName}"));
			displayItems.Add(new ViewTuple("Delivery Address", order.DeliveryAddress));
			displayItems.Add(new ViewTuple("Type", order.Type));
			displayItems.Add(new ViewTuple("Status", order.Status));
			displayItems.Add(new ViewTuple("Last Time Modified", string.Format("{0:G}", order.LastTimeModified)));
			
			// for printing the list
			using (var context = new Context())
			{
				// cast the ordered items into the binding list
				items.UpdateObservableCollection(context.OrderProduct.Include(x => x.Product).ThenInclude(x => x.PriceHistory).Where(x => x.OrderId == order.Id).Select(x => new OrderProductViewModel(x)));

				IEnumerable<Promotion> promotions = items.SelectMany(x => context.Promotion.Include(y => y.Product).Where(y => y.StartTime <= x.OrderProduct.LastTimeModified && y.EndTime > x.OrderProduct.LastTimeModified && x.OrderProduct.ProductId == y.ProductId)).OrderBy(x => x.Discount);

				var text = promotions.Select(x => $"Promotion: {x.Description}\tName: {x.Product.Name}\tDiscount: {x.Discount}\r\n").Aggregate("", (p, n) => p + n);

				Summary.Visibility = Visibility.Visible;
				Summary.Text = $"Total price: ${items.Sum(x => x.Price * (promotions.FirstOrDefault(y => y.ProductId == x.OrderProduct.ProductId)?.Discount ?? 1.0))}, Total weight: {items.Sum(x => x.Weight)}\r\n{text}";
			}

		}
	}
}
