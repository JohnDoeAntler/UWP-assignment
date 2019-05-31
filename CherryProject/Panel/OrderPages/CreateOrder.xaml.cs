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

			// items.Add(new OrderProductViewModel(
			// 	new OrderProduct() {
			// 		ProductId = 
			// 	})
			// );

			// fill the role combo box with all role
			Enum.GetValues(typeof(OrderTypeEnum)).Cast<OrderTypeEnum>().ToList().ForEach(x =>
			{
				Type.Items.Add(x.ToString());
			});

			Guid.Text = System.Guid.NewGuid().ToString();

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

			Summary.Visibility = Visibility.Visible;
			Summary.Text = $"Total price: {items.Sum(x => x.Price)}, Total weight: {items.Sum(x => x.Weight)}";
		}

		private void GenerateGuidBtn_Click(object sender, RoutedEventArgs e) => Guid.Text = System.Guid.NewGuid().ToString();

		private async void Add_Click(object sender, RoutedEventArgs e)
		{
			ProductDialog dialog = new ProductDialog();

			var button = await dialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				string id = dialog.Product.Id;

				if (items.Any(x => x.OrderProduct.Product.Id == id))
				{
					ContentDialog error = new ContentDialog
					{
						Title = "Error",
						Content = "The product type has exisited on the order items list already.",
						CloseButtonText = "OK",
						Width = 400
					};

					await error.EnqueueAndShowIfAsync();
				}
				else
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

			do
			{
				button = await dialog.EnqueueAndShowIfAsync();
			} while (button == ContentDialogResult.Primary && dialog.User.RoleId != (await RoleManager.FindRoleAsync(x => x.NormalizedName == RoleEnum.Dealer.ToString().ToUpperInvariant())).Id);

			if (button == ContentDialogResult.Primary)
			{
				DealerGUID.Text = (dealer = dialog.User).Id;
				SelectedUser.Text = $"Selected Dealer: {dealer.FirstName} {dealer.LastName}";
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to create an order?",
				PrimaryButtonText = "Create Order",
				CloseButtonText = "Cancel"
			};

			// alert user
			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (dealer == null
				|| Type.SelectedItem == null
				|| items.Count == 0
				|| items.Any(x => x.Quantity < 1))
				{
					ContentDialog error = new ContentDialog
					{
						Title = "Error",
						Content = "The information you typed has mistakes, please ensure the input data validation is correct.",
						CloseButtonText = "OK",
						Width = 400
					};

					await error.EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							var order = (await context.Order.AddAsync(new Order()
							{
								Id = Guid.Text,
								DealerId = dealer.Id,
								ModifierId = SignInManager.CurrentUser.Id,
								Type = Type.SelectedItem.ToString(),
								Status = OrderStatusEnum.Pending.ToString()
							})).Entity;

							foreach (var element in items)
							{
								var orderProduct = element.OrderProduct;
								orderProduct.Product = null;
								orderProduct.OrderId = order.Id;

								await context.OrderProduct.AddAsync(orderProduct);
							}

							await context.SaveChangesAsync();

							ContentDialog message = new ContentDialog
							{
								Title = "Success",
								Content = "Successfully created order.",
								CloseButtonText = "OK",
								Width = 400
							};

							await message.EnqueueAndShowIfAsync();

							Frame.Navigate(typeof(ViewOrder), order, new DrillInNavigationTransitionInfo());
						}
					}
					catch (Exception err)
					{
						ContentDialog error = new ContentDialog
						{
							Title = "Error",
							Content = $"The information you typed might duplicated, please try again later.\n{err.ToString()}",
							CloseButtonText = "OK",
							Width = 400
						};

						await error.EnqueueAndShowIfAsync();
					}
				}
			}
		}
	}
}
