using CherryProject.Attribute;
using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
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
	[Hidden]
	public sealed partial class UnreserveOrder : Page
	{
		private Order order;

		public UnreserveOrder()
		{
			this.InitializeComponent();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Order order)
			{
				if (order.Status != OrderStatusEnum.Cancelled)
				{
					if (order.Type == OrderTypeEnum.Reserve)
					{
						this.order = order;
						Submit.Click += Submit_Click;
					}
					else
					{
						ContentDialog error = new ContentDialog
						{
							Title = "ERROR",
							Content = $"It is not permitted to unreserve a purchase type order.",
							CloseButtonText = "OK",
							Width = 400
						};

						await error.EnqueueAndShowIfAsync();

						Frame.Navigate(typeof(SearchOrders), this.GetType(), new DrillInNavigationTransitionInfo());
					}
				}
				else
				{
					ContentDialog error = new ContentDialog
					{
						Title = "ERROR",
						Content = $"It is not permitted to unreserve an order with cancelled status.",
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
					Title = "ALERT",
					Content = "You have to choose a order before unreserving it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(SearchOrders), this.GetType(), new DrillInNavigationTransitionInfo());
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			if (await new ConfirmationDialog().EnqueueAndShowIfAsync() == ContentDialogResult.Primary)
			{
				order = await order.ModifyAsync(x => x.Type = OrderTypeEnum.Purchase);

				if (order.DealerId != SignInManager.CurrentUser.Id)
				{
					NotificationManager.CreateNotification(order.DealerId, "An Order Has Been Unreserved.", $"{SignInManager.CurrentUser.FirstName} {SignInManager.CurrentUser.LastName} has unreserved one of your order pending requests.", NotificationTypeEnum.Order, order.Id);
				}

				await new SuccessDialog().EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(ViewPromotion), order, new DrillInNavigationTransitionInfo());
			}
		}
	}
}
