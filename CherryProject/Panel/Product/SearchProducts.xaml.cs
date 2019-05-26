using CherryProject.Extension;
using CherryProject.Model;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.Product
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchProducts : Page
	{
		private readonly ObservableCollection<string> _searchFilters;
		private readonly ObservableCollection<Products> _searchProductGridViewItems;
		private readonly Dictionary<string, Predicate<Products>> keyValuePairs = new Dictionary<string, Predicate<Products>>();
		private Type searchStatus;

		public ObservableCollection<string> SearchFilters => _searchFilters;
		public ObservableCollection<Products> SearchProductGridViewItems => _searchProductGridViewItems;
		public SearchProducts()
        {
            this.InitializeComponent();

			_searchFilters = new ObservableCollection<string>();

			using (var context = new Context())
			{
				_searchProductGridViewItems = new ObservableCollection<Products>(
					context.Products
				);

				ResultAlerter.Text = $"There has only found {_searchProductGridViewItems.Count} result(s).";
			}

			AddSearchFilter();
		}

		private void AddSearchFilter() => _searchFilters.Add($"Filter {_searchFilters.Count + 1}:");

		private void AddSearchingFilterBtn_Tapped(object sender, TappedRoutedEventArgs e)
		{
			AddSearchFilter();
		}

		private void ListViewItem_Tapped(object sender, TappedRoutedEventArgs e)
		{

		}

		private void UpdateSearchResult()
		{

		}

		private void Button_Refresh(object sender, RoutedEventArgs e)
		{
			UpdateSearchResult();
		}

		private void Button_Reset(object sender, RoutedEventArgs e)
		{
		}

		private async void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
		}
	}
}
