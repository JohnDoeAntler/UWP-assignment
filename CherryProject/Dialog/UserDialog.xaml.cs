using CherryProject.Extension;
using CherryProject.Model;
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
	public sealed partial class UserDialog : ContentDialog
	{
		private readonly ObservableCollection<string> _searchFilters;
		private readonly ObservableCollection<User> _searchAccountGridViewItems;
		private readonly Dictionary<string, Predicate<User>> keyValuePairs;

		public UserDialog()
		{
			this.InitializeComponent();

			// searching filter list instantiation
			_searchFilters = new ObservableCollection<string>();
			_searchAccountGridViewItems = new ObservableCollection<User>();
			keyValuePairs = new Dictionary<string, Predicate<User>>();

			// update the search result
			UpdateSearchResult();

			// add a search filter for user input their filtering string.
			AddSearchFilter();
		}

		public ObservableCollection<string> SearchFilters => _searchFilters;
		public ObservableCollection<User> SearchAccountGridViewItems => _searchAccountGridViewItems;

		public User User { get; private set; }

		private void AddSearchFilter() => _searchFilters.Add($"Filter {_searchFilters.Count + 1}:");

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

		private void AddSearchingFilterBtn_Tapped(object sender, TappedRoutedEventArgs e)
		{
			AddSearchFilter();
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
				IEnumerable<User> set = context.User;

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
				_searchAccountGridViewItems.Clear();

				// re-update the searching result by using foreach statement
				foreach (var item in set)
				{
					_searchAccountGridViewItems.Add(item);
				}

				// updatae the reminder text
				ResultAlerter.Text = $"There has only found {_searchAccountGridViewItems.Count} result(s).";
			}
		}

		private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			User = (User) ResultListViewControl.SelectedItem;
			SelectedTarget.Text = $"Selected: {User.FirstName} {User.LastName}";
			SelectedTarget.Visibility = Visibility.Visible;
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (User == null)
			{
				SelectedTarget.Text = $"Please select a user or else cancel the select dialog.";
				SelectedTarget.Visibility = Visibility.Visible;
				args.Cancel = true;
			}
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			User = null;
			this.Hide();
		}
	}
}
