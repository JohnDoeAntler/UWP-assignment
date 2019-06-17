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

namespace CherryProject.Panel.SparePages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AddSpares : Page
	{
		private Category selectedCategory;

		public AddSpares()
		{
			this.InitializeComponent();
		}

		private async void Select_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new CategoryDialog();

			var button = await dialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				SelectedCategoryTextBlock.Visibility = Visibility.Visible;
				SelectedCategoryTextBlock.Text = $"Selected category: {(selectedCategory = dialog.Category).Name}";
				CategoryId.Text = selectedCategory.Id.ToString();
			}
		}

		private void Quantity_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args) => args.Cancel = !args.NewText.IsIntegerNumeric();

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			var result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (selectedCategory == null
				|| string.IsNullOrEmpty(Quantity.Text)
				|| !uint.TryParse(Quantity.Text, out uint quantity)
				|| quantity < 1)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							for (int i = 0; i < quantity; i++)
							{
								await context.Spare.AddAsync(
									new Spare()
									{
										Id = Guid.NewGuid(),
										CategoryId = Guid.Parse(CategoryId.Text)
									}
								);
							}

							await context.SaveChangesAsync();

							await new SuccessDialog().EnqueueAndShowIfAsync();

							Frame.Navigate(GetType(), null, new DrillInNavigationTransitionInfo());
						}
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
