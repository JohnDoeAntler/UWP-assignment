using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
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

			Status.ItemsSource = EnumManager.GetEnumList<GeneralStatusEnum>();
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
				ProductGUID.Text = selectedProduct.Id.ToString();
			}
		}

		private void Discount_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args) => args.Cancel = !args.NewText.IsDoubleNumeric();

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// alert user
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (selectedProduct == null
				|| string.IsNullOrEmpty(Description.GetText())
				|| string.IsNullOrEmpty(ImageUrl.Text)
				|| !double.TryParse(Discount.Text, out double discount)
				|| !StartTime.SelectedDate.HasValue
				|| !EndTime.SelectedDate.HasValue
				|| Status.SelectedItem == null
				)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							await context.Promotion.AddAsync(
								new Promotion()
								{
									Id = System.Guid.Parse(Guid.Text),
									ProductId = selectedProduct.Id,
									Description = Description.GetText(),
									ImageUrl = ImageUrl.Text,
									Discount = discount,
									StartTime = StartTime.SelectedDate.Value.Date,
									EndTime = EndTime.SelectedDate.Value.Date,
									Status = (GeneralStatusEnum) Status.SelectedItem
								}
							);

							await context.SaveChangesAsync();
						}

						await new SuccessDialog().EnqueueAndShowIfAsync();

						Frame.Navigate(typeof(ViewPromotions), null, new DrillInNavigationTransitionInfo());
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
