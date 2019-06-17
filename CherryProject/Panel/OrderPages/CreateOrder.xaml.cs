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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Timers;
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
    public sealed partial class CreateOrder : Page
    {
		private readonly ObservableCollection<OrderProductViewModel> items;
		private User dealer;

		public CreateOrder()
        {
            this.InitializeComponent();

			items = new ObservableCollection<OrderProductViewModel>();

			// fill the role combo box with all role
			Type.ItemsSource = EnumManager.GetEnumList<OrderTypeEnum>();

			Guid.Text = System.Guid.NewGuid().ToString();

			DataGridViewControl.CellEditEnded += DataGridViewControl_CellEditEnded;

			// permission control
			if (SignInManager.CurrentUser.Role == RoleEnum.Dealer)
			{
				DealerSelector.Visibility = Visibility.Collapsed;
				dealer = SignInManager.CurrentUser;
			}
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

			Summary.Visibility = Visibility.Visible;
			Summary.Text = $"Total price: {items.Sum(x => x.Price)}, Total weight: {items.Sum(x => x.Weight)}";
		}

		private void GenerateGuidBtn_Click(object sender, RoutedEventArgs e) => Guid.Text = System.Guid.NewGuid().ToString();

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

		private async void SelectDealer_Click(object sender, RoutedEventArgs e)
		{
			UserDialog dialog = new UserDialog();

			ContentDialogResult button;

			// select till the selected user is playing a dealer role.
			do
			{
				button = await dialog.EnqueueAndShowIfAsync();
			} while (button == ContentDialogResult.Primary && dialog.User.Role != RoleEnum.Dealer);

			if (button == ContentDialogResult.Primary)
			{
				DealerGUID.Text = (dealer = dialog.User).Id.ToString();
				SelectedUser.Visibility = Visibility.Visible;
				SelectedUser.Text = $"Selected Dealer: {dealer.FirstName} {dealer.LastName}";
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// alert user
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (dealer == null
				|| Type.SelectedItem == null
				|| string.IsNullOrEmpty(Address.GetText())
				|| items.Count == 0
				|| items.Any(x => x.Quantity < 1))
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							var order = (await context.Order.AddAsync(new Order()
							{
								Id = System.Guid.Parse(Guid.Text),
								DealerId = dealer.Id,
								ModifierId = SignInManager.CurrentUser.Id,
								DeliveryAddress = Address.GetText(),
								Type = (OrderTypeEnum) Type.SelectedItem,
								Status = OrderStatusEnum.Pending
							})).Entity;

							foreach (var element in items)
							{
								var orderProduct = element.OrderProduct;
								orderProduct.Product = null;
								orderProduct.OrderId = order.Id;

								await context.OrderProduct.AddAsync(orderProduct);
							}

							await context.SaveChangesAsync();

							await new SuccessDialog().EnqueueAndShowIfAsync();

							Frame.Navigate(typeof(ViewOrder), order, new DrillInNavigationTransitionInfo());
						}
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
