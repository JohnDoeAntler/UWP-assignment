using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
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
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Dialog
{
	public sealed partial class ProductDialog : ContentDialog
	{
		private readonly ObservableCollection<string> _searchFilters;
		private readonly ObservableCollection<Product> _searchProductGridViewItems;
		private readonly Dictionary<string, Predicate<Product>> keyValuePairs;

		private bool isOrdering;

		public ProductDialog(bool isOrdering = false)
		{
			this.InitializeComponent();

			// searching filter list instantiation
			_searchFilters = new ObservableCollection<string>();
			_searchProductGridViewItems = new ObservableCollection<Product>();
			keyValuePairs = new Dictionary<string, Predicate<Product>>();

			this.isOrdering = isOrdering;

			// update the search result
			UpdateSearchResult();

			// add a search filter for user input their filtering string.
			AddSearchFilter();
		}

		public Product Product { get; set; }

		public ObservableCollection<string> SearchFilters => _searchFilters;
		public ObservableCollection<Product> SearchProductGridViewItems => _searchProductGridViewItems;

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
				IEnumerable<Product> set;

				if (isOrdering)
				{
					set = context
							.Product
							.Include(x => x.Category)
							.Include(x => x.Did)
							.Include(x => x.PriceHistory)
							.Where(x => 
								x.DangerLevel 
								< context.Spare.Include(y => y.Category).Where(y => y.Category.ProductId == x.Id).LongCount() 
								- (context.Did.Any(y => y.ProductId == x.Id) 
									? context.Did.Where(y => y.ProductId == x.Id).Sum(y => y.Quantity) 
									: 0)
							);
				}
				else
				{
					set = context.Product;
				}

				// filter all non-avaliable product
				if (SignInManager.CurrentUser.Role == RoleEnum.Dealer || isOrdering)
				{
					set = set.Where(x => x.Status == GeneralStatusEnum.Available);
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
				_searchProductGridViewItems.UpdateObservableCollection(set);

				// updatae the reminder text
				ResultAlerter.Text = $"There has only found {_searchProductGridViewItems.Count} result(s).";
			}
		}

		private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Product = (Product) ResultListViewControl.SelectedItem;
			SelectedTarget.Text = $"Selected: {Product.Name}";
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (Product == null)
			{
				SelectedTarget.Text = $"Please select a product or else cancel the select dialog.";
				args.Cancel = true;
			}
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args) => Product = null;
	}
}
