using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
	public sealed partial class DeliverOrder : Page
	{
		private Order order; 

		public DeliverOrder()
		{
			this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Order order)
			{
				if (order.Status == OrderStatusEnum.Endorsed.ToString())
				{
					this.order = order;
					Submit.Click += Submit_Click;
				}
				else
				{
					ContentDialog error = new ContentDialog
					{
						Title = "Error",
						Content = $"It is not permitted to endorse a non-endorsed status order.",
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
					Content = "You have to choose a order before delivering it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(SearchOrders), this.GetType(), new DrillInNavigationTransitionInfo());
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = $"Are you ensure to deliver current order?\n\nDelivery Address: {order.DeliveryAddress}",
				PrimaryButtonText = "Deliver Order",
				CloseButtonText = "Cancel"
			};

			// alert user
			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				try
				{
					using (var context = new Context())
					{
						var dic = (await context.Dic.AddAsync(new Dic()
						{
							Id = GuidHelper.CreateNewGuid().ToString(),
							OrderId = order.Id,
							Status = DicStatus.Dispatching.ToString()
						})).Entity;

						var orderProducts = context.OrderProduct.Where(x => x.OrderId == order.Id);

						foreach (var orderProduct in orderProducts)
						{
							var product = await context.Product.FirstOrDefaultAsync(x => x.Id == orderProduct.ProductId);

							// total (sold + existign) spare count
							uint sprcnt = (uint)context.Spare.Include(x => x.Category).Count(x => x.Category.ProductId == orderProduct.ProductId);
							// sold spare count
							uint didsprcnt = (uint)context.DidSpare.Include(x => x.Did).Count(x => x.Did.ProductId == orderProduct.ProductId);

							// only non-danger level product could be added on did
							if (sprcnt - didsprcnt > product.DangerLevel)
							{
								await context.Did.AddAsync(new Did()
								{
									Id = GuidHelper.CreateNewGuid().ToString(),
									DicId = dic.Id,
									ProductId = orderProduct.ProductId,
									Quantity = sprcnt - didsprcnt >= orderProduct.Quantity ? orderProduct.Quantity : sprcnt - didsprcnt,
								});
							}
						}

						await context.SaveChangesAsync();
					}

					ContentDialog message = new ContentDialog
					{
						Title = "Success",
						Content = "Successfully delivered order.",
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
