using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.DispatchPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ViewOrderDispatchStatus : Page
	{
		private ObservableCollection<Dic> dics;
		private Order order;

		public ViewOrderDispatchStatus()
		{
			this.InitializeComponent();

			dics = new ObservableCollection<Dic>();
		}

		public ObservableCollection<Dic> Dics { get => dics; set => dics = value; }

		private async void SelectOrder_Click(object sender, RoutedEventArgs e)
		{
			OrderDialog dialog = new OrderDialog();

			ContentDialogResult button;

			bool isEndorsed = false;

			do
			{
				// if dialog displays more than 1 times
				if (dialog.Order != null)
				{
					// if the order is not endorsed
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
				}

				button = await dialog.EnqueueAndShowIfAsync();

			} while (button == ContentDialogResult.Primary && !(isEndorsed = dialog.Order.Status.Equals(OrderStatusEnum.Endorsed.ToString())));

			if (button == ContentDialogResult.Primary)
			{
				OrderGUID.Text = (order = dialog.Order).Id;
				SelectedOrder.Text = $"Selected Order: {order.Dealer.FirstName}'s Order";

				using (var context = new Context())
				{
					// var dics = context.Order.Include(x => x.Dic).FirstOrDefault(x => x.Id == order.Id).Dic;
					var dics = context.Dic.Include(x => x.Did).Include(x => x.Order).ThenInclude(x => x.Dealer).Where(x => x.OrderId == order.Id);

					long assembled = 0, dispatched = 0, all = 0;

					foreach (var dic in dics)
					{
						// fullfill assembiled dic quantity:
						var total = dic.Did.Sum(x => x.Quantity);

						// current assembiled
						var now = context.DidSpare.Include(x => x.Did).Count(x => x.Did.DicId == dic.Id);

						dic.Status = total > now ? $"Assembled: {now} / {total}" : dic.Status;

						// sum those value for displaying the result of each progress
						assembled += now;
						dispatched += total > now ? 0 : now;
						all += total;
					}

					// set text
					ProcessingItems.Text = $"Processing Items: {all}";
					OrderedItems.Text = $"Ordered Items: {all = context.OrderProduct.Where(x => x.OrderId == order.Id).Sum(x => x.Quantity)}";

					// setting the progress bar and texts
					AssembledTextBlock.Text = $"Assembled: {assembled} / {all}";
					AssembleProgressBar.Value = (double) assembled / all * 100;
					DispatchedTextBlock.Text = $"Dispatched: {dispatched} / {all}";
					DispatchedProgressBar.Value = (double) dispatched / all * 100;

					// set list
					DicCount.Text = $"There has only found {dics.Count()} result(s).";
					this.dics.UpdateObservableCollection(dics);
				}
			}
		}
	}
}
