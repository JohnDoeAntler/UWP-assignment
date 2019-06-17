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
						DidId.Text = Did.Id.ToString();
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
					DidId.Text = Did.Id.ToString();
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
				DidId.Text = Did.Id.ToString();
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
						 		&& EF.Functions.Like(x.Id.ToString(), sender.Text + "%")
						 		&& x.DidSpare == null)
							.Take(5);

						foreach (var spare in spares)
						{
							list.Add(new ListViewItem()
							{
								Content = spare.Id.ToString()
							});
						}

						sender.ItemsSource = list;
					}
				}
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			var result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (Did == null	|| string.IsNullOrEmpty(SpareId.Text))
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							var did = context.Did.Include(x => x.DidSpare).FirstOrDefault(x => x.Id == this.did.Id);

							if (did.Quantity > did.DidSpare.Count)
							{
								// if the spare has not been assembled and the did product match the spare product
								if (await context.Spare.Include(x => x.Category).Include(x => x.DidSpare).AnyAsync(x => x.Id == Guid.Parse(SpareId.Text) && x.Category.ProductId == Did.ProductId && x.DidSpare == null))
								{
									await context.DidSpare.AddAsync(new DidSpare()
									{
										DidId = Did.Id,
										SpareId = Guid.Parse(SpareId.Text)
									});

									await context.SaveChangesAsync();

									await new SuccessDialog().EnqueueAndShowIfAsync();

									Frame.Navigate(GetType(), null, new DrillInNavigationTransitionInfo());
								}
								else throw new Exception();
							}
							else
							{
								await new ContentDialog()
								{
									Title = "Alert",
									Content = "You are not permitted to assemble spare into this D.I.D owing to it has been completed already.",
									CloseButtonText = "OK",
									Width = 400
								}.EnqueueAndShowIfAsync();
							}
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
