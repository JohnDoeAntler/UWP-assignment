using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Dialog
{
	public sealed partial class OrderDialog : ContentDialog
	{
		private readonly ObservableCollection<string> _searchFilters;
		private readonly ObservableCollection<Order> _searchOrderGridViewItems;
		private readonly Dictionary<string, Predicate<Order>> keyValuePairs;
		private Type searchStatus;

		public OrderDialog()
		{
			this.InitializeComponent();

			// searching filter list instantiation
			_searchFilters = new ObservableCollection<string>();
			_searchOrderGridViewItems = new ObservableCollection<Order>();
			keyValuePairs = new Dictionary<string, Predicate<Order>>();

			// update the search result
			UpdateSearchResult();

			// add a search filter for user input their filtering string.
			AddSearchFilter();
		}

		public ObservableCollection<string> SearchFilters => _searchFilters;
		public ObservableCollection<Order> SearchOrderGridViewItems => _searchOrderGridViewItems;
		public Order Order { get; set; }

		private void AddSearchFilter() => _searchFilters.Add($"Filter {_searchFilters.Count + 1}:");

		private void AddSearchingFilterBtn_Tapped(object sender, TappedRoutedEventArgs e) => AddSearchFilter();

		private void Button_Refresh(object sender, RoutedEventArgs e) => UpdateSearchResult();

		private void Button_Reset(object sender, RoutedEventArgs e)
		{
			// clear both
			keyValuePairs.Clear();
			_searchFilters.Clear();
			// remain a empty searching filter input to user
			AddSearchFilter();
			// update the result to display all products
			UpdateSearchResult();
		}

		private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// if the sender object is ListViewItem # bullshit
			if (sender is ListViewItem item)
			{
				// get the main child (stackpanel) of ListViewItem
				var sp = item.Content as StackPanel;

				// get the elements of stackpanel
				// textblock: show the filter number
				var t1 = (sp.Children[0] as TextBlock).Text;
				// textbox: conditional string
				var t2 = (sp.Children[3] as TextBox).Text;
				// combobox: the attribute selected from user object
				var c1 = ((sp.Children[1] as ComboBox)?.SelectedItem as ComboBoxItem)?.Content as string;
				// combobox: the filter
				var c2 = ((sp.Children[2] as ComboBox)?.SelectedItem as ComboBoxItem)?.Content as string;

				// if the key pairs contains pressed search filter
				if (keyValuePairs.ContainsKey(t1))
				{
					// remove it
					keyValuePairs.Remove(t1);
				}

				// if user is selecting the listviewitem
				if (item.IsSelected)
				{
					// if user inputted incompletely
					if (string.IsNullOrEmpty(c1)
					|| string.IsNullOrEmpty(c2)
					|| string.IsNullOrEmpty(t2))
					{
						// add null to keyparis
						keyValuePairs.Add(t1, null);
					}
					else
					{
						// delegate a method
						Func<string, string, bool> strategy = null;

						// using switch to choose approriate method strategy of searching target user
						switch (c2)
						{
							case "Equals":
								strategy = (x, y) => x.Equals(y);
								break;

							case "Contains":
								strategy = (x, y) => x.Contains(y);
								break;

							case "Not equals":
								strategy = (x, y) => !x.Equals(y);
								break;

							case "Not contains":
								strategy = (x, y) => !x.Contains(y);
								break;
						}

						// add the searching filter to keypairs by mapping key "filter {int}:"
						keyValuePairs.Add(t1, x => strategy(x.GetType().GetProperty(c1.Replace(" ", string.Empty)).GetValue(x, null).ToString(), t2));
					}
				}
			}

			// update the result after adding a new searching filter
			UpdateSearchResult();
		}

		private void UpdateSearchResult()
		{
			// instantiate a disposable context object
			using (var context = new Context())
			{
				// store user
				IEnumerable<Order> set = context.Order.Include(x => x.Dealer).Include(x => x.OrderProduct);

				// user filtering
				foreach (var predicate in keyValuePairs)
				{
					// if the searching filter is a completed method strategy
					if (predicate.Value != null)
					{
						// filter the set
						set = set.Where(x => predicate.Value(x));
					}
				}

				// clear the result for renew the searching result
				_searchOrderGridViewItems.Clear();

				// re-update the searching result by using foreach statement
				foreach (var item in set)
				{
					_searchOrderGridViewItems.Add(item);
				}

				// updatae the reminder text
				ResultAlerter.Text = $"There has only found {_searchOrderGridViewItems.Count} result(s).";
			}
		}

		private async void SelectDealer_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new UserDialog();

			ContentDialogResult button;

			do
			{
				button = await dialog.EnqueueAndShowIfAsync();
			} while (button == ContentDialogResult.Primary && dialog.User.RoleId != (await RoleManager.FindRoleAsync(x => x.NormalizedName == RoleEnum.Dealer.ToString().ToUpperInvariant())).Id);

			if (button == ContentDialogResult.Primary)
			{
				User dealer = dialog.User;
				DealerGUID.Text = dealer.Id;

				SelectedDealer.Text = $"Selected Dealer: {dealer.FirstName} {dealer.LastName}";

				if (keyValuePairs.ContainsKey("Dealer"))
				{
					keyValuePairs.Remove("Dealer");
				}

				keyValuePairs.Add("Dealer", x => x.DealerId == dealer.Id);

				UpdateSearchResult();
			}
		}

		private async void SelectModifier_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new UserDialog();

			var button = await dialog.EnqueueAndShowIfAsync();

			if (button == ContentDialogResult.Primary)
			{
				User modifier = dialog.User;
				ModifierGUID.Text = modifier.Id;

				SelectedModifier.Text = $"Selected Modifier: {modifier.FirstName} {modifier.LastName}";

				if (keyValuePairs.ContainsKey("Modifier"))
				{
					keyValuePairs.Remove("Modifier");
				}

				keyValuePairs.Add("Modifier", x => x.ModifierId == modifier.Id);

				UpdateSearchResult();
			}
		}

		private void ResetDealer_Click(object sender, RoutedEventArgs e)
		{
			DealerGUID.Text = string.Empty;
			SelectedDealer.Text = string.Empty;

			if (keyValuePairs.ContainsKey("Dealer"))
			{
				keyValuePairs.Remove("Dealer");

				UpdateSearchResult();
			}
		}

		private void ResetModifier_Click(object sender, RoutedEventArgs e)
		{
			ModifierGUID.Text = string.Empty;
			SelectedModifier.Text = string.Empty;

			if (keyValuePairs.ContainsKey("Modifier"))
			{
				keyValuePairs.Remove("Modifier");

				UpdateSearchResult();
			}
		}

		private async void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Order = (Order)ResultListViewControl.SelectedItem;
			SelectedTarget.Text = $"Selected: {Order.Dealer.FirstName}'s Order";
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (Order == null)
			{
				SelectedTarget.Text = $"Please select an order or else cancel the select dialog.";
				args.Cancel = true;
			}
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Order = null;
			this.Hide();
		}
	}
}
