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

namespace CherryProject.Panel.PromotionPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ModifyPromotion : Page
	{
		private Promotion promotion;

		public ModifyPromotion()
		{
			this.InitializeComponent();

			Status.ItemsSource = EnumManager.GetEnumList<GeneralStatusEnum>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			
			if (e.Parameter is Promotion promotion)
			{
				FillInformation(this.promotion = promotion);
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose a promotion before modifying it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(ViewPromotions), this.GetType(), new DrillInNavigationTransitionInfo());
			}
		}

		private void FillInformation(Promotion promotion)
		{
			Guid.Text = promotion.Id.ToString();
			ProductGUID.Text = promotion.ProductId.ToString();
			Description.SetText(promotion.Description);
			ImageUrl.Text = promotion.ImageUrl;
			Discount.Text = promotion.Discount.ToString();
			StartTime.SelectedDate = promotion.StartTime.Date;
			EndTime.SelectedDate = promotion.EndTime.Date;
			Status.SelectedItem = promotion.Status;
		}

		private void GenerateGuidBtn_OnClick(object sender, RoutedEventArgs e) => Guid.Text = System.Guid.NewGuid().ToString();

		private void Discount_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args) => args.Cancel = !args.NewText.IsDoubleNumeric();

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// alert user
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Description.GetText())
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
						await promotion.ModifyAsync(x => {
							x.Id = System.Guid.Parse(Guid.Text);
							x.Description = Description.GetText();
							x.ImageUrl = ImageUrl.Text;
							x.Discount = discount;
							x.StartTime = StartTime.SelectedDate.Value.Date;
							x.EndTime = EndTime.SelectedDate.Value.Date;
							x.Status = (GeneralStatusEnum)Status.SelectedItem;
						});

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
