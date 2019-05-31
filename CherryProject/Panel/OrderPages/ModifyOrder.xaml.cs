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
			Enum.GetValues(typeof(OrderTypeEnum)).Cast<OrderTypeEnum>().ToList().ForEach(x =>
			{
				Type.Items.Add(x.ToString());
			});

			// fill the role combo box with all role
			Enum.GetValues(typeof(OrderStatusEnum)).Cast<OrderStatusEnum>().ToList().ForEach(x =>
			{
				Status.Items.Add(x.ToString());
			});

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
				// pre define
				var dealer = await UserManager.FindUserAsync(x => x.Id == order.DealerId);
				var modifier = await UserManager.FindUserAsync(x => x.Id == order.ModifierId);

				// non order product information required attribute
				Guid.Text = order.Id;
				DealerGUID.Text = order.DealerId;
				SelectedUser.Text = $"{dealer.FirstName} {dealer.LastName}";
				Type.SelectedItem = order.Type;
				Status.SelectedItem = order.Status;

				this.order = order;

				// required attribute
				using (var context = new Context())
				{
					var result = context.OrderProduct.Include(x => x.Product).Where(x => x.OrderId == order.Id);

					foreach (var element in result)
					{
						items.Add(new OrderProductViewModel(element));
					}
				}
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose a order before modifying it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(SearchOrders), typeof(ModifyOrder), new DrillInNavigationTransitionInfo());
			}
		}

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
				if (items.Count == 0 || items.Any(x => x.Quantity < 1))
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
						var order = await this.order.ModifyAsync(x => {
							x.Type = Type.SelectedItem as string;
							x.Status = Status.SelectedItem as string;
						});

						foreach (var item in items)
						{
							// ikr, it looks weird
							await item.OrderProduct.ModifyAsync(x => x.Quantity = x.Quantity);
						}

						ContentDialog message = new ContentDialog
						{
							Title = "Success",
							Content = "Successfully modified product.",
							CloseButtonText = "OK",
							Width = 400
						};

						await message.EnqueueAndShowIfAsync();

						Frame.Navigate(typeof(ViewOrder), order, new DrillInNavigationTransitionInfo());
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
