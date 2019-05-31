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

			Enum.GetValues(typeof(GeneralStatusEnum)).Cast<GeneralStatusEnum>().ToList().ForEach(x =>
			{
				Status.Items.Add(x.ToString());
			});
		}

		private void GenerateGuidBtn_OnClick(object sender, RoutedEventArgs e) => Guid.Text = System.Guid.NewGuid().ToString();

		private void Price_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		private void Weight_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		private void DangerLevel_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsIntegerNumeric();


		/// <summary>
		/// create an account after tapping the create account button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// instantiate a dialog object
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to create an account?",
				PrimaryButtonText = "Create Account",
				CloseButtonText = "Cancel"
			};

			// alert user
			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Name.Text)
				|| string.IsNullOrEmpty(Description.Text)
				|| string.IsNullOrEmpty(Price.Text)
				|| string.IsNullOrEmpty(Weight.Text)
				|| string.IsNullOrEmpty(DangerLevel.Text)
				|| !double.TryParse(Price.Text, out double price)
				|| !double.TryParse(Weight.Text, out double weight)
				|| !int.TryParse(DangerLevel.Text, out int dangerLevel)
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
						Product product;
						using (var context = new Context())
						{
							var p = await context.Product.AddAsync(
								new Model.Product()
								{
									Name = Name.Text,
									Description = Description.Text,
									Price = price,
									Weight = weight,
									DangerLevel = dangerLevel,
									Status = Status.SelectedItem as string
								}
							);

							product = p.Entity;

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

						Frame.Navigate(typeof(ViewProduct), product, new DrillInNavigationTransitionInfo());
					}
					catch (Exception err)
					{
						ContentDialog error = new ContentDialog
						{
							Title = "Error",
							Content = $"The information you typed might duplicated, please make sure the product name has not been registered before.\n{err.ToString()}",
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
