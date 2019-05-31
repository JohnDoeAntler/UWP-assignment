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
	public sealed partial class ModifyProduct : Page
	{
		private Product product;

		public ModifyProduct()
		{
			this.InitializeComponent();

			Enum.GetValues(typeof(GeneralStatusEnum)).Cast<GeneralStatusEnum>().ToList().ForEach(x =>
			{
				Status.Items.Add(x.ToString());
			});
		}

		public Product Product { get => product; set => product = value; }

		private void Price_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		private void Weight_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		private void DangerLevel_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsIntegerNumeric();

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Model.Product product)
			{
				FillInformation(this.product = product);
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose a product before modifying it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				this.Frame.Navigate(typeof(SearchProducts), this.GetType(), new DrillInNavigationTransitionInfo());	
			}
		}

		private void FillInformation(Model.Product product)
		{
			Guid.Text = product.Id;
			Name.Text = product.Name;
			Description.Text = product.Description;
			Price.Text = product.Price.ToString();
			Weight.Text = product.Weight.ToString();
			DangerLevel.Text = product.DangerLevel.ToString();
			Status.SelectedItem = product.Status;
		}

		private async void Submit_ClickAsync(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to modify current product?",
				PrimaryButtonText = "Modify Product",
				CloseButtonText = "Cancel"
			};

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
						product = await product.ModifyAsync(x =>
							{
								x.Name = Name.Text;
								x.Description = Description.Text;
								x.Price = price;
								x.Weight = weight;
								x.DangerLevel = dangerLevel;
								x.Status = Status.SelectedItem as string;
							}
						);

						ContentDialog message = new ContentDialog
						{
							Title = "Success",
							Content = "Successfully modified product.",
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