using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using CherryProject.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Uwp.UI.Controls;
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
    public sealed partial class ModifyOrder : Page
	{
		private Order order;
		private readonly ObservableCollection<OrderProductViewModel> items;

		public ModifyOrder()
        {
            this.InitializeComponent();

			items = new ObservableCollection<OrderProductViewModel>();

			// fill the role combo box with all role
			Type.ItemsSource = EnumManager.GetEnumList<OrderTypeEnum>();

			DataGridViewControl.CellEditEnded += DataGridViewControl_CellEditEnded;
		}

		public ObservableCollection<OrderProductViewModel> Items => items;

		private void DataGridViewControl_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
		{
			// remove quantity == 0 order item
			if (items.Any(x => x.Quantity < 1))
			{
				var tmp = items.Where(x => x.Quantity > 0).ToList();

				items.Clear();

				tmp.ForEach(x => items.Add(x));
			}

			//
			Summary.Visibility = Visibility.Visible;
			Summary.Text = $"Total price: {items.Sum(x => x.Price)}, Total weight: {items.Sum(x => x.Weight)}";
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Order order)
			{
				if (order.Status == OrderStatusEnum.Pending)
				{
					// pre define
					var dealer = await UserManager.FindUserAsync(x => x.Id == order.DealerId);
					var modifier = await UserManager.FindUserAsync(x => x.Id == order.ModifierId);

					// non order product information required attribute
					Guid.Text = order.Id.ToString();
					DealerGUID.Text = order.DealerId.ToString();
					SelectedUser.Text = $"{dealer.FirstName} {dealer.LastName}";
					Address.Document.SetText(Windows.UI.Text.TextSetOptions.None, order.DeliveryAddress);
					Type.SelectedItem = order.Type;

					this.order = order;

					// required attribute
					using (var context = new Context())
					{
						// cast the ordered items into the binding list
						items.UpdateObservableCollection(context.OrderProduct.Include(x => x.Product).ThenInclude(x => x.PriceHistory).Where(x => x.OrderId == order.Id).Select(x => new OrderProductViewModel(x)));
					}
				}
				else
				{
					ContentDialog error = new ContentDialog
					{
						Title = "Error",
						Content = $"It is not permitted to modify a non-pending status order.",
						CloseButtonText = "OK",
						Width = 400
					};

					await error.EnqueueAndShowIfAsync();

					Frame.Navigate(typeof(SearchOrders), this.GetType(), new DrillInNavigationTransitionInfo());
				}
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose a pending order before modifying it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(SearchOrders), this.GetType(), new DrillInNavigationTransitionInfo());
			}
		}

		private async void Add_Click(object sender, RoutedEventArgs e)
		{
			ProductDialog dialog = new ProductDialog(true);

			var button = await dialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				if (!items.Any(x => x.OrderProduct.Product.Id == dialog.Product.Id))
				{
					items.Add(
						new OrderProductViewModel(
							new OrderProduct()
							{
								ProductId = dialog.Product.Id,
								Product = dialog.Product
							}
						)
					);
				}
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// alert user
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (Type.SelectedItem == null
				|| string.IsNullOrEmpty(Address.GetText())
				|| items.Count == 0
				|| items.Any(x => x.Quantity < 1)
				)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						var order = await this.order.ModifyAsync(x => {
							x.ModifierId = SignInManager.CurrentUser.Id;
							x.DeliveryAddress = Address.GetText();
							x.Type = (OrderTypeEnum) Type.SelectedItem;
						});

						foreach (var item in items)
						{
							// ikr, it looks weird
							await item.OrderProduct.ModifyAsync(x => x.Quantity = item.OrderProduct.Quantity);
						}

						await items.Select(x => x.OrderProduct).RemoveExceptAsync();

						await new SuccessDialog().EnqueueAndShowIfAsync();

						Frame.Navigate(typeof(ViewOrder), order, new DrillInNavigationTransitionInfo());
					}
					catch (Exception)
					{
						await new ErrorDialog().EnqueueAndShowIfAsync();
					}
				}
			}
		}
	}
}