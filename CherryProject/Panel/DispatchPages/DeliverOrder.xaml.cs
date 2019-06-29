using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Panel.OrderPages;
using CherryProject.Service;
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

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Order order)
			{
				using (var context = new Context())
				{
					order = await context.Order.Include(x => x.Dealer).FirstOrDefaultAsync(x => x.Id == order.Id && x.ConcurrencyStamp == order.ConcurrencyStamp);

					if (order != null)
					{
						OrderGUID.Text = (this.order = order).Id.ToString();
						SelectedOrder.Text = $"Selected Order: {order.Dealer.FirstName}'s Order";
						SelectedOrder.Visibility = Visibility.Visible;
					}
				}
			}
		}

		private async void SelectOrder_Click(object sender, RoutedEventArgs e)
		{
			OrderDialog dialog = new OrderDialog(x => x.Status == OrderStatusEnum.Endorsed && x.Type == OrderTypeEnum.Purchase);

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
					&& !((isEndorsed = dialog.Order.Status == OrderStatusEnum.Endorsed)
					&& (isIncomplete = dialog.Order.OrderProduct.Sum(x => x.Quantity) > context.Did.Include(x => x.Dic).Where(x => x.Dic.OrderId == dialog.Order.Id).Sum(x => x.Quantity))));
			}

			if (button == ContentDialogResult.Primary)
			{
				OrderGUID.Text = (order = dialog.Order).Id.ToString();
				SelectedOrder.Text = $"Selected Order: {order.Dealer.FirstName}'s Order";
				SelectedOrder.Visibility = Visibility.Visible;
				Submit.IsEnabled = true;
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// alert user
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (order == null)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						var completion = true;

						var isZero = true;

						using (var context = new Context())
						{
							var dic = (await context.Dic.AddAsync(new Dic()
							{
								Id = GuidHelper.CreateNewGuid(),
								OrderId = order.Id,
								Status = DicStatusEnum.Dispatching.ToString()
							})).Entity;

							var orderProducts = context.OrderProduct.Include(x => x.Product).Where(x => x.OrderId == order.Id);

							foreach (var orderProduct in orderProducts)
							{
								// dispatched items
								uint dispatched = (uint) context.Did.Include(x => x.Dic).Where(x => x.Dic.OrderId == orderProduct.OrderId && x.ProductId == orderProduct.ProductId).Sum(x => x.Quantity);

								uint quantity = 0;

								// if the product of order is not equeueing completely, still missing something to dispatch
								if ((quantity = orderProduct.Quantity - dispatched) > 0)
								{
									var product = orderProduct.Product;

									// total (sold + existign) spare count
									uint sprcnt = (uint)context.Spare.Include(x => x.Category).Count(x => x.Category.ProductId == orderProduct.ProductId);

									// sold spare count
									// uint didsprcnt = (uint)context.DidSpare.Include(x => x.Did).Count(x => x.Did.ProductId == orderProduct.ProductId);
									uint didcnt = (uint)context.Did.Where(x => x.ProductId == orderProduct.ProductId).Sum(x => x.Quantity);

									uint remaining = 0;

									// only non-danger level product could be added on did
									if ((remaining = sprcnt - didcnt) > product.DangerLevel)
									{
										uint real;

										// if remaining stock does not able to complete the ordered quantity
										if (remaining < quantity)
										{
											// move all the remaining stock to roder.
											real = remaining;
											// the order is not completed.
											completion = false;
										}
										else
										{
											real = quantity;
										}

										// if there has at least 1 DID
										isZero = false;

										// add the did into database
										await context.Did.AddAsync(
											new Did()
											{
												Id = GuidHelper.CreateNewGuid(),
												DicId = dic.Id,
												ProductId = orderProduct.ProductId,
												Quantity = real,
											}
										);
									}
									else
									{
										// system ignored order because of insufficient stocking and reached danger-level
										completion = false;
									}
								}
							}

							// if there has at least 1 DID and 1 DIC
							if (!isZero)
							{
								await context.SaveChangesAsync();
							}
						}
						
						// if the dic exists on database
						if (!isZero)
						{
							if (!completion)
							{
								await new ContentDialog()
								{
									Title = "ALERT",
									Content = "Some of ordered items might be missing because of the system has no sufficient stock to supply.",
									CloseButtonText = "OK",
									Width = 400
								}.EnqueueAndShowIfAsync();
							}

							NotificationManager.CreateNotification(order.DealerId, "An Order Is Being Processing", $"{SignInManager.CurrentUser.FirstName} {SignInManager.CurrentUser.LastName} has decided to process one of your order requests.", NotificationTypeEnum.Dic, order.Id);

							await new SuccessDialog().EnqueueAndShowIfAsync();

							Frame.Navigate(typeof(ViewOrderDispatchStatus), order, new DrillInNavigationTransitionInfo());
						}
						else
						{
							await new ContentDialog()
							{
								Title = "ALERT",
								Content = "Because of insufficient stock supplyment, there has no order could be requested to be delivered.",
								CloseButtonText = "OK",
								Width = 400
							}.EnqueueAndShowIfAsync();
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
