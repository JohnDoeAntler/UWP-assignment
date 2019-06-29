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

namespace CherryProject.Panel.ProductPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AddProduct : Page
	{
		public AddProduct()
		{
			this.InitializeComponent();

			Guid.Text = System.Guid.NewGuid().ToString();

			Status.ItemsSource = EnumManager.GetEnumList<GeneralStatusEnum>();
		}

		/// <summary>
		/// re-generate a new id
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GenerateGuidBtn_OnClick(object sender, RoutedEventArgs e) => Guid.Text = System.Guid.NewGuid().ToString();

		/// <summary>
		/// check the price whether numeric
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Price_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		/// <summary>
		/// check the weight whether numeric
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Weight_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		/// <summary>
		/// check the recorder level whether integer numeric
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ReorderLevel_KeyDown(TextBox sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsIntegerNumeric();

		/// <summary>
		/// check the danger level whether integer numeric
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DangerLevel_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsIntegerNumeric();


		/// <summary>
		/// create an account after tapping the create account button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// alert user
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Name.Text)
				|| string.IsNullOrEmpty(Description.Text)
				|| string.IsNullOrEmpty(Price.Text)
				|| string.IsNullOrEmpty(Weight.Text)
				|| string.IsNullOrEmpty(DangerLevel.Text)
				|| !double.TryParse(Price.Text, out double price)
				|| !double.TryParse(Weight.Text, out double weight)
				|| !int.TryParse(ReorderLevel.Text, out int reorderLevel)
				|| !int.TryParse(DangerLevel.Text, out int dangerLevel)
				|| Status.SelectedItem == null
				)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						Product product;

						using (var context = new Context())
						{
							// add the product
							var p = await context.Product.AddAsync(
								new Product()
								{
									Id = System.Guid.Parse(Guid.Text),
									Name = Name.Text,
									Description = Description.Text,
									Weight = weight,
									ReorderLevel = reorderLevel,
									DangerLevel = dangerLevel,
									IconUrl = Url.Text,
									Status = (GeneralStatusEnum)Status.SelectedItem
								}
							);

							// get the created product entity
							product = p.Entity;

							// add the price
							await context.PriceHistory.AddAsync(
								new PriceHistory()
								{
									ProductId = product.Id,
									Price = price
								}
							);

							// fucking save
							await context.SaveChangesAsync();
						}

						if ((GeneralStatusEnum)Status.SelectedItem == GeneralStatusEnum.Available)
						{
							NotificationManager.CreateNotification(SignInManager.CurrentUser.Id, "New Product Released", "To check it out, press \"View\" button to have a took on it.", NotificationTypeEnum.Product, product.Id);
						}

						// alert
						await new SuccessDialog().EnqueueAndShowIfAsync();

						// navigation
						Frame.Navigate(typeof(ViewProduct), product, new DrillInNavigationTransitionInfo());
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
