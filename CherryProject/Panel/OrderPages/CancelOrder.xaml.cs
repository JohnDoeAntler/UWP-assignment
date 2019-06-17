using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
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
    public sealed partial class CancelOrder : Page
    {
		private Order order;

		public CancelOrder()
        {
            this.InitializeComponent();
        }

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Order order)
			{
				if (order.Status == OrderStatusEnum.Pending)
				{
					this.order = order;
					Submit.Click += Submit_Click;
				}
				else
				{
					ContentDialog error = new ContentDialog
					{
						Title = "Error",
						Content = $"It is not permitted to cancel a non-pending status order.",
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
					Content = "You have to choose a order before cancelling it.",
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
				Content = "Are you ensure to cancel an order?",
				PrimaryButtonText = "Cancel Order",
				CloseButtonText = "Cancel"
			};

			// alert user
			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				order = await order.ModifyAsync(x => x.Status = OrderStatusEnum.Cancelled);

				ContentDialog message = new ContentDialog
				{
					Title = "Success",
					Content = "Successfully cancelled order.",
					CloseButtonText = "OK",
					Width = 400
				};

				await message.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(ViewOrder), order, new DrillInNavigationTransitionInfo());
			}
		}
	}
}
