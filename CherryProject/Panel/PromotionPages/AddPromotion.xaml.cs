using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
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

namespace CherryProject.Panel.PromotionPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AddPromotion : Page
	{
		private Product selectedProduct;

		public AddPromotion()
		{
			this.InitializeComponent();

			Guid.Text = System.Guid.NewGuid().ToString();

			Enum.GetValues(typeof(GeneralStatusEnum)).Cast<GeneralStatusEnum>().ToList().ForEach(x =>
			{
				Status.Items.Add(x.ToString());
			});
		}

		private void GenerateGuidBtn_OnClick(object sender, RoutedEventArgs e) => Guid.Text = System.Guid.NewGuid().ToString();

		private async void Select_Click(object sender, RoutedEventArgs e)
		{
			ProductDialog dialog = new ProductDialog();

			var button = await dialog.EnqueueAndShowIfAsync();
			
			if (button == ContentDialogResult.Primary)
			{
				SelectedProductTextBlock.Visibility = Visibility.Visible;
				SelectedProductTextBlock.Text = $"Selected product: {(selectedProduct = dialog.Product).Name}";
			}
		}

		private void Discount_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args) => args.Cancel = !args.NewText.IsDoubleNumeric();

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// instantiate a dialog object
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to add promotion?",
				PrimaryButtonText = "Add Promotion",
				CloseButtonText = "Cancel"
			};

			// alert user
			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();
			if (result == ContentDialogResult.Primary)
			{
				if (selectedProduct == null
				|| string.IsNullOrEmpty(Description.GetText())
				|| string.IsNullOrEmpty(ImageUrl.Text)
				|| !decimal.TryParse(Discount.Text, out decimal discount)
				|| StartTime.Date == null
				|| EndTime.Date == null
				|| string.IsNullOrEmpty(Status.SelectedItem as string)
				)
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
						var timestamp = StartTime.Date.Date;
						var duration = EndTime.Date.ToUnixTimeSeconds() - StartTime.Date.ToUnixTimeSeconds();

						Promotion promotion;

						using (var context = new Context())
						{
							var p = await context.Promotion.AddAsync(
								new Promotion()
								{
									Id = Guid.Text,
									ProductId = selectedProduct.Id,
									Description = Description.GetText(),
									ImageUrl = ImageUrl.Text,
									Discount = discount,
									Timestamp = timestamp,
									Duration = duration,
									Status = Status.SelectedItem as string
								}
							);

							promotion = p.Entity;

							await context.SaveChangesAsync();
						}

						ContentDialog message = new ContentDialog
						{
							Title = "Success",
							Content = "Successfully created product.",
							CloseButtonText = "OK",
							Width = 400
						};

						await message.EnqueueAndShowIfAsync();

						Frame.Navigate(typeof(ViewPromotions), null, new DrillInNavigationTransitionInfo());
					}
					catch (Exception err)
					{
						ContentDialog error = new ContentDialog
						{
							Title = "Error",
							Content = $"The information you typed might duplicated, please verify each input information validation.\n{err.ToString()}",
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
