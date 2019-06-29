using CherryProject.Dialog;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.OrderPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SearchOrders : Page
	{
		private readonly ObservableCollection<string> _searchFilters;
		private readonly ObservableCollection<Order> _searchOrderGridViewItems;
		private readonly Dictionary<string, Predicate<Order>> keyValuePairs;
		private Type searchStatus;

		public SearchOrders()
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

			// permission control
			if (SignInManager.CurrentUser.Role == RoleEnum.Dealer)
			{
				DealerSelector.Visibility = Visibility.Collapsed;
				ModifierSelector.Visibility = Visibility.Collapsed;
			}
		}

		public ObservableCollection<string> SearchFilters => _searchFilters;
		public ObservableCollection<Order> SearchOrderGridViewItems => _searchOrderGridViewItems;

		private void AddSearchFilter() => _searchFilters.Add($"Filter {_searchFilters.Count + 1}:");

		private void AddSearchingFilterBtn_Tapped(object sender, TappedRoutedEventArgs e) => AddSearchFilter();

		private void Button_Refresh(object sender, RoutedEventArgs e) => UpdateSearchResult();

		private void Button_Reset(object sender, RoutedEventArgs e)
		{
			// clear both
			keyValuePairs.Clear();
			_searchFilters.Clear();
			// reset 
			ResetDealer_Click(null, null);
			ResetModifier_Click(null, null);
			// remain a empty searching filter input to user
			AddSearchFilter();
			// update the result to display all products
			UpdateSearchResult();
		}

		private async void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			// abstract a common factor
			string str = (searchStatus == null ? "View" : "Select");

			// create a dialog
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = $"Are you ensure to {str.ToLower()} tapped order?",
				PrimaryButtonText = $"{str} Order",
				CloseButtonText = "Cancel"
			};

			// display the dialog to user
			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

			// if user selected "{str} account" button
			if (result == ContentDialogResult.Primary)
			{
				// if the user enter searching user UI by clicking nav bar, it would direct user to view account UI
				// else if the user was instructed to select an account, it would return a user previous UI
				Frame.Navigate(searchStatus ?? typeof(ViewPromotion), ResultListViewControl.SelectedItem, new DrillInNavigationTransitionInfo());
			}
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

				// check if dealer
				if (SignInManager.CurrentUser.Role == RoleEnum.Dealer)
				{
					set = set.Where(x => x.DealerId == SignInManager.CurrentUser.Id);
				}

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
				_searchOrderGridViewItems.UpdateObservableCollection(set);

				// updatae the reminder text
				ResultAlerter.Text = $"There has only found {_searchOrderGridViewItems.Count} result(s).";
			}
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			// if the parameter is type "Type"
			if (e.Parameter is Type type)
			{
				// store it to instance variable
				searchStatus = type;

				// instantiate a dialog to remind user becauses of their initiative intention was not searching account
				ContentDialog Reminder = new ContentDialog
				{
					Title = "Reminder",
					Content = "Tap on the order which you want to modify on.",
					CloseButtonText = "Got it"
				};

				// show the dialog
				await Reminder.EnqueueAndShowIfAsync();
			}
		}

		private async void SelectDealer_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new UserDialog();

			ContentDialogResult button;

			do
			{
				button = await dialog.EnqueueAndShowIfAsync();
			} while (button == ContentDialogResult.Primary && dialog.User.Role != RoleEnum.Dealer);

			if (button == ContentDialogResult.Primary)
			{
				User dealer = dialog.User;
				DealerGUID.Text = dealer.Id.ToString();

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
				ModifierGUID.Text = modifier.Id.ToString();

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
	}
}
