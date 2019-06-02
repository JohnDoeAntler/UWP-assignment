using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CherryProject.Panel.SparePages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AssembleSpare : Page
	{
		private Did did;

		public AssembleSpare()
		{
			this.InitializeComponent();

			this.NavigationCacheMode = NavigationCacheMode.Enabled;
		}

		private Did Did
		{
			get => did;
			set {
				did = value;

				if (did == null)
				{
					SpareId.IsEnabled = false;
				}else
				{
					SpareId.IsEnabled = true;
				}
			}
		}

		private async void SelectDid_Click(object sender, RoutedEventArgs e)
		{
			var orderDialog = new OrderDialog();

			var button = await orderDialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				var dicDialog = new DicDialog(orderDialog.Order);

				button = await dicDialog.EnqueueAndShowIfAsync();

				if (button == ContentDialogResult.Primary)
				{
					var didDialog = new DidDialog(dicDialog.Dic);

					button = await didDialog.EnqueueAndShowIfAsync();

					if (button == ContentDialogResult.Primary)
					{
						Did = didDialog.Did;
						DidId.Text = Did.Id;
						SelectedDidTextBlock.Text = $"Selected {orderDialog.Order.Dealer.FirstName}'s DID";
					}
				}
			}
		}
		private async void SelectDid2_Click(object sender, RoutedEventArgs e)
		{
			var dicDialog = new DicDialog();

			var button = await dicDialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				var didDialog = new DidDialog(dicDialog.Dic);

				button = await didDialog.EnqueueAndShowIfAsync();

				if (button == ContentDialogResult.Primary)
				{
					Did = didDialog.Did;
					DidId.Text = Did.Id;
					SelectedDidTextBlock.Text = $"Selected {dicDialog.Dic.Order.Dealer.FirstName}'s DID";
				}
			}
		}

		private async void SelectDid3_Click(object sender, RoutedEventArgs e)
		{
			var didDialog = new DidDialog();

			var button = await didDialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				Did = didDialog.Did;
				DidId.Text = Did.Id;
				SelectedDidTextBlock.Text = $"Selected {didDialog.Did.Dic.Order.Dealer.FirstName}'s DID";
			}
		}

		private void SpareId_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
			{
				//Set the ItemsSource to be your filtered dataset
				//sender.ItemsSource = dataset;
				if (string.IsNullOrEmpty(sender.Text))
				{
					sender.ItemsSource = null;
				}
				else
				{
					using (var context = new Context())
					{
						var list = new ObservableCollection<ListViewItem>();

						var spares = context.Spare
							.Include(x => x.Category)
							.Include(x => x.DidSpare)
							.Where(x => x.Category.ProductId == Did.ProductId 
								&& x.Id.Contains(sender.Text, StringComparison.OrdinalIgnoreCase) 
								&& !x.DidSpare.Any(y => y.SpareId == x.Id));

						foreach (var spare in spares)
						{
							list.Add(new ListViewItem()
							{
								Content = spare.Id
							});
						}

						sender.ItemsSource = list;
					}
				}
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to assemble spare?",
				PrimaryButtonText = "Assemble Spare",
				CloseButtonText = "Cancel"
			};

			var result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (Did == null
				|| string.IsNullOrEmpty(SpareId.Text))
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
							if (context.Spare.Any(x => x.Id == SpareId.Text))
							{
								await context.DidSpare.AddAsync(new DidSpare()
								{
									DidId = Did.Id,
									SpareId = SpareId.Text
								});

								await context.SaveChangesAsync();

								ContentDialog message = new ContentDialog
								{
									Title = "Success",
									Content = "Successfully assembled spare.",
									CloseButtonText = "OK",
									Width = 400
								};

								await message.EnqueueAndShowIfAsync();

								Frame.Navigate(GetType(), null, new DrillInNavigationTransitionInfo());
							}
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
