using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using Microsoft.EntityFrameworkCore;
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
	public sealed partial class AddSpare : Page
	{
		private Category selectedCategory;

		public AddSpare()
		{
			this.InitializeComponent();

			Guid.Text = GuidHelper.CreateNewGuid().ToString();
		}

		private void GenerateGuidBtn_OnClick(object sender, RoutedEventArgs e) => Guid.Text = GuidHelper.CreateNewGuid().ToString();

		private async void Select_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new CategoryDialog();

			var button = await dialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				SelectedCategoryTextBlock.Visibility = Visibility.Visible;
				SelectedCategoryTextBlock.Text = $"Selected category: {(selectedCategory = dialog.Category).Name}";
				CategoryId.Text = selectedCategory.Id;
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to add spare?",
				PrimaryButtonText = "Add Spare",
				CloseButtonText = "Cancel"
			};

			var result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (selectedCategory == null)
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
						using (var context = new Context())
						{
							await context.Spare.AddAsync(
								new Spare()
								{
									Id = Guid.Text,
									CategoryId = CategoryId.Text
								}
							);

							await context.SaveChangesAsync();

							ContentDialog message = new ContentDialog
							{
								Title = "Success",
								Content = "Successfully added spare.",
								CloseButtonText = "OK",
								Width = 400
							};

							await message.EnqueueAndShowIfAsync();

							Frame.Navigate(GetType(), null, new DrillInNavigationTransitionInfo());
						}
					}
					catch (Exception err)
					{
						ContentDialog error = new ContentDialog
						{
							Title = "Error",
							Content = $"The information you typed might duplicated, please try again later.\n{err.ToString()}",
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
