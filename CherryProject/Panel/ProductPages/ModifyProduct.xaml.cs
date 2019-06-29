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

namespace CherryProject.Panel.ProductPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	[Hidden]
	public sealed partial class ModifyProduct : Page
	{
		private Product product;

		public ModifyProduct()
		{
			this.InitializeComponent();

			Status.ItemsSource = EnumManager.GetEnumList<GeneralStatusEnum>();
		}

		public Product Product { get => product; set => product = value; }

		private void Price_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		private void Weight_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsDoubleNumeric();

		private void ReorderLevel_KeyDown(TextBox sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsIntegerNumeric();

		private void DangerLevel_KeyDown(object sender, TextBoxBeforeTextChangingEventArgs e) => e.Cancel = !e.NewText.IsIntegerNumeric();

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Product product)
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

		private void FillInformation(Product product)
		{
			Guid.Text = product.Id.ToString();
			Name.Text = product.Name;
			Description.Text = product.Description;
			Weight.Text = product.Weight.ToString();
			ReorderLevel.Text = product.ReorderLevel.ToString();
			DangerLevel.Text = product.DangerLevel.ToString();
			Url.Text = product.IconUrl ?? string.Empty;
			Status.SelectedItem = product.Status;

			using (var context = new Context())
			{
				Price.Text = context.PriceHistory.Where(x => x.ProductId == product.Id).OrderByDescending(x => x.Timestamp).FirstOrDefault().Price.ToString();
			}
		}

		private async void Submit_ClickAsync(object sender, RoutedEventArgs e)
		{
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Name.Text)
				|| string.IsNullOrEmpty(Description.Text)
				|| string.IsNullOrEmpty(Price.Text)
				|| string.IsNullOrEmpty(Weight.Text)
				|| string.IsNullOrEmpty(ReorderLevel.Text)
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
						product = await product.ModifyAsync(x =>
							{
								x.Name = Name.Text;
								x.Description = Description.Text;
								x.Weight = weight;
								x.ReorderLevel = reorderLevel;
								x.DangerLevel = dangerLevel;
								x.IconUrl = Url.Text;
								x.Status = (GeneralStatusEnum) Status.SelectedItem;
							}
						);

						using (var context = new Context())
						{
							if (price != context.PriceHistory.Where(x => x.ProductId == product.Id).OrderByDescending(x => x.Timestamp).FirstOrDefault().Price)
							{
								await context.PriceHistory.AddAsync(
									new PriceHistory()
									{
										ProductId = product.Id,
										Price = price
									}
								);

								await context.SaveChangesAsync();
							}
						}

						await new SuccessDialog().EnqueueAndShowIfAsync();

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