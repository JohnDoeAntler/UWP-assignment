using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
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

namespace CherryProject.Panel.CategoryPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AddCategory : Page
	{
		private Product selectedProduct;

		public AddCategory()
		{
			this.InitializeComponent();

			Guid.Text = System.Guid.NewGuid().ToString();
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

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			var result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Name.Text)	|| selectedProduct == null)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							await context.Category.AddAsync(new Category()
							{
								Id = System.Guid.Parse(Guid.Text),
								Name = Name.Text,
								ProductId = selectedProduct.Id
							});

							await context.SaveChangesAsync();
						}

						await new SuccessDialog().EnqueueAndShowIfAsync();

						Frame.Navigate(typeof(AddCategory), null, new DrillInNavigationTransitionInfo());
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
