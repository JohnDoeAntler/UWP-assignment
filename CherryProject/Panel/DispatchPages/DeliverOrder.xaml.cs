using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Panel.OrderPages;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.DispatchPages
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

		private async void SelectOrder_Click(object sender, RoutedEventArgs e)
		{
			OrderDialog dialog = new OrderDialog();

			ContentDialogResult button;

			using (var context = new Context())
			{
				bool isEndorsed = false, isIncomplete = false;

				do
				{
					// if dialog displays more than 1 times
					if (dialog.Order != null)
					{
						if (!isEndorsed)
						{
							ContentDialog error = new ContentDialog
							{
								Title = "Alert",
								Content = "The order has not been endorsed, please wait till area manager endorses it.",
								CloseButtonText = "OK",
								Width = 400
							};

							await error.EnqueueAndShowIfAsync();
						}
						else if (!isIncomplete)
						{
							ContentDialog error = new ContentDialog
							{
								Title = "Alert",
								Content = "The selected order has been completed already. Please select another order.",
								CloseButtonText = "OK",
								Width = 400
							};

							await error.EnqueueAndShowIfAsync();
						}
					}

					button = await dialog.EnqueueAndShowIfAsync();
				} while (button == ContentDialogResult.Primary
					&& !((isEndorsed = dialog.Order.Status.Equals(OrderStatusEnum.Endorsed.ToString()))
					&& (isIncomplete = dialog.Order.OrderProduct.Sum(x => x.Quantity) > context.Did.Include(x => x.Dic).Where(x => x.Dic.OrderId == dialog.Order.Id).Sum(x => x.Quantity))));
			}

			if (button == ContentDialogResult.Primary)
			{
				OrderGUID.Text = (order = dialog.Order).Id;
				SelectedOrder.Text = $"Selected Order: {order.Dealer.FirstName}'s Order";
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to deliver an order?",
				PrimaryButtonText = "Deliver Order",
				CloseButtonText = "Cancel"
			};

			// alert user
			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (order == null)
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
							var dic = (await context.Dic.AddAsync(new Dic()
							{
								Id = GuidHelper.CreateNewGuid().ToString(),
								OrderId = order.Id,
								Status = DicStatusEnum.Dispatching.ToString()
							})).Entity;

							var orderProducts = context.OrderProduct.Include(x => x.Product).Where(x => x.OrderId == order.Id);

							foreach (var orderProduct in orderProducts)
							{
								// dispatched items
								uint dispatched = (uint)context.Did.Include(x => x.Dic).Include(x => x.DidSpare).Where(x => x.Dic.OrderId == orderProduct.OrderId && x.ProductId == orderProduct.ProductId).Sum(x => x.DidSpare.Count);

								Debug.WriteLine($"Ordered item count : {orderProduct.Quantity}");
								Debug.WriteLine($"Dispatched item count : {dispatched}");

								uint quantity = 0;
								if ((quantity = orderProduct.Quantity - dispatched) > 0)
								{
									var product = orderProduct.Product;

									// total (sold + existign) spare count
									uint sprcnt = (uint)context.Spare.Include(x => x.Category).Count(x => x.Category.ProductId == orderProduct.ProductId);

									// sold spare count
									// uint didsprcnt = (uint)context.DidSpare.Include(x => x.Did).Count(x => x.Did.ProductId == orderProduct.ProductId);
									uint didcnt = (uint)context.Did.Where(x => x.ProductId == orderProduct.ProductId).Sum(x => x.Quantity);

									Debug.WriteLine($"Total item count : {sprcnt}");
									Debug.WriteLine($"Sold item count : {didcnt}\n");

									uint remaining = 0;

									// only non-danger level product could be added on did
									if ((remaining = sprcnt - didcnt) > product.DangerLevel)
									{
										await context.Did.AddAsync(
											new Did()
											{
												Id = GuidHelper.CreateNewGuid().ToString(),
												DicId = dic.Id,
												ProductId = orderProduct.ProductId,
												Quantity = remaining >= quantity ? quantity : remaining,
											}
										);
									}
								}
							}

							await context.SaveChangesAsync();
						}

						ContentDialog message = new ContentDialog
						{
							Title = "Success",
							Content = "Successfully sent a deliver order request.",
							CloseButtonText = "OK",
							Width = 400
						};

						await message.EnqueueAndShowIfAsync();
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
