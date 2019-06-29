using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.InvoicePages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class PrintInvoice : Page
	{
		private Order order;

		public PrintInvoice()
		{
			this.InitializeComponent();
		}

		private async void SelectDic_Click(object sender, RoutedEventArgs e)
		{
			OrderDialog dialog = new OrderDialog(x => x.Status == OrderStatusEnum.Endorsed);

			ContentDialogResult button;

			bool isEndorsed = false;

			using (var context = new Context())
			{
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

				} while (button == ContentDialogResult.Primary && !(isEndorsed = dialog.Order.Status == OrderStatusEnum.Endorsed));
			}

			if (button == ContentDialogResult.Primary)
			{
				DicGUID.Text = (order = dialog.Order).Id.ToString();
				SelectedDic.Text = $"Selected DIC: {order.Id}";
				SelectedDic.Visibility = Visibility.Visible;
				Submit.IsEnabled = true;
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			StorageFile sFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Template.html"));

			byte[] result;

			using (Stream fileStream = await sFile.OpenStreamForReadAsync())
			{
				result = new byte[fileStream.Length];
				await fileStream.ReadAsync(result, 0, (int)fileStream.Length);
			}

			var htmlText = System.Text.Encoding.ASCII.GetString(result);

			htmlText = htmlText.Replace("???", string.Empty);
			htmlText = htmlText.Replace("__Now__", DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString());
			htmlText = htmlText.Replace("__DealerId__", order.DealerId.ToString());
			htmlText = htmlText.Replace("__FirstName__", order.Dealer.FirstName);
			htmlText = htmlText.Replace("__LastName__", order.Dealer.LastName);
			htmlText = htmlText.Replace("__Email__", order.Dealer.Email);
			htmlText = htmlText.Replace("__PhoneNumber__", order.Dealer.PhoneNumber);
			htmlText = htmlText.Replace("__Address__", order.DeliveryAddress);
			htmlText = htmlText.Replace("__Type__", order.Type.ToString());
			htmlText = htmlText.Replace("__Status__", order.Status.ToString());

			using (var context = new Context())
			{
				var orderProducts = context.OrderProduct.Include(x => x.Product).ThenInclude(x => x.PriceHistory).Where(x => x.OrderId == order.Id);
				var promotions = orderProducts.SelectMany(x => context.Promotion.Include(y => y.Product).Where(y => y.StartTime <= x.LastTimeModified && y.EndTime > x.LastTimeModified && y.ProductId == x.ProductId)).OrderBy(x => x.Discount);
				var prices = orderProducts.Select(x => new { x.ProductId, Price = x.Quantity * x.Product.PriceHistory.Where(y => y.Timestamp < x.LastTimeModified).OrderByDescending(y => y.Timestamp).FirstOrDefault().Price });

				// table filling

				htmlText = htmlText.Replace("__orderProducts__", Newtonsoft.Json.JsonConvert.SerializeObject(orderProducts.Select(x => new
				{
					Id = x.ProductId,
					Name = x.Product.Name,
					Quantity = x.Quantity,
					Dispatched = context.Did.Include(y => y.Dic).Where(y => y.Dic.OrderId == order.Id && y.ProductId == x.ProductId && (y.Dic.Status == DicStatusEnum.Dispatched.ToString() || y.Dic.Status == DicStatusEnum.Accepted.ToString())).Sum(y => y.Quantity),
					TotalPrice = prices.FirstOrDefault(y => y.ProductId == x.ProductId).Price
				})));

				// table filling

				htmlText = htmlText.Replace("__promotions__", Newtonsoft.Json.JsonConvert.SerializeObject(promotions.Select(x => new
				{
					Id = x.Id,
					Description = x.Description,
					Discount = x.Discount,
					DiscountedProduct = x.Product.Name
				})));

				// total price calculation

				var orignal = prices.Sum(x => x.Price);
				var discounted = prices.Select(x => x.Price * (promotions.FirstOrDefault(y => y.ProductId == x.ProductId) == null ? 1.0 : promotions.FirstOrDefault(y => y.ProductId == x.ProductId).Discount)).Sum();

				htmlText = htmlText.Replace("__TotalPrice__", $"from {orignal.ToString("C")} to {(discounted == 0.0 ? orignal : discounted).ToString("C")}");
			}

			var picker = new Windows.Storage.Pickers.FileSavePicker();
			picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
			picker.FileTypeChoices.Add("Website", new List<string>() { ".htm", ".html" });
			picker.SuggestedFileName = $"{order.Id.ToString("N")}_{DateTime.UtcNow.Ticks}.html";

			var file = await picker.PickSaveFileAsync();

			if (file != null)
			{
				// Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
				CachedFileManager.DeferUpdates(file);
				// write to file
				await FileIO.WriteTextAsync(file, htmlText);
				// Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
				// Completing updates may require Windows to ask for user input.
				FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);

				if (status == FileUpdateStatus.Complete)
				{
					await new SuccessDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					await new ErrorDialog().EnqueueAndShowIfAsync();
				}
			}
		}
	}
}
