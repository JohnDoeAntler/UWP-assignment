using CherryProject.Model;
using CherryProject.Service;
using CherryProject.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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

namespace CherryProject.Panel.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchAccounts : Page
    {
		private readonly ObservableCollection<string> _searchFilters;
		private readonly ObservableCollection<Users> _searchAccountGridViewItems;
		private readonly Dictionary<string, Predicate<Users>> keyValuePairs = new Dictionary<string, Predicate<Users>>();
		private Type searchStatus;

		public ObservableCollection<string> SearchFilters => _searchFilters;
		public ObservableCollection<Users> SearchAccountGridViewItems => _searchAccountGridViewItems;

		public SearchAccounts()
        {
            this.InitializeComponent();

			_searchFilters = new ObservableCollection<string>();

			using (var context = new Context())
			{
				_searchAccountGridViewItems = new ObservableCollection<Users>(
					context.Users.Include(x => x.Role)
				);

				ResultAlerter.Text = $"There has only found {_searchAccountGridViewItems.Count} result(s).";
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
			Debug.WriteLine((sender as ListViewItem).IsSelected);

			if (sender is ListViewItem item)
			{
				var sp = item.Content as StackPanel;

				var t1 = (sp.Children[0] as TextBlock).Text;
				var t2 = (sp.Children[3] as TextBox).Text;
				var c1 = ((sp.Children[1] as ComboBox)?.SelectedItem as ComboBoxItem)?.Content as string;
				var c2 = ((sp.Children[2] as ComboBox)?.SelectedItem as ComboBoxItem)?.Content as string;

				if (keyValuePairs.ContainsKey(t1))
				{
					keyValuePairs.Remove(t1);
				}

				if (item.IsSelected)
				{
					if (string.IsNullOrEmpty(c1)
					|| string.IsNullOrEmpty(c2)
					|| string.IsNullOrEmpty(t2))
					{
						keyValuePairs.Add(t1, null);
					}
					else
					{
						Func<string, string, bool> strategy = null;

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

						keyValuePairs.Add(t1, x => strategy(x.GetType().GetProperty(c1.Replace(" ", string.Empty)).GetValue(x, null).ToString(), t2));
					}
				}
			}

			UpdateSearchResult();
		}

		private void UpdateSearchResult()
		{
			using (var context = new Context())
			{
				IEnumerable<Users> set = context.Users.Include(x => x.Role);

				foreach (var predicate in keyValuePairs)
				{
					if (predicate.Value != null)
					{
						set = set.Where(x => predicate.Value(x));
					}
				}

				_searchAccountGridViewItems.Clear();

				foreach (var item in set)
				{
					_searchAccountGridViewItems.Add(item);
				}

				ResultAlerter.Text = $"There has only found {_searchAccountGridViewItems.Count} result(s).";
			}
		}

		private void Button_Refresh(object sender, RoutedEventArgs e)
		{
			UpdateSearchResult();
		}

		private void Button_Reset(object sender, RoutedEventArgs e)
		{
			keyValuePairs.Clear();
			_searchFilters.Clear();
			AddSearchFilter();
			UpdateSearchResult();
		}

		private async void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			string str = (searchStatus == null ? "View" : "Select");

			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = $"Are you ensure to {str.ToLower()} tapped account?",
				PrimaryButtonText = $"{str} Account",
				CloseButtonText = "Cancel"
			};

			ContentDialogResult result = await dialog.ShowAsync();

			if (result == ContentDialogResult.Primary)
			{
				Frame.Navigate(searchStatus ?? typeof(ViewAccount), ResultListViewControl.SelectedItem, new DrillInNavigationTransitionInfo());
			}
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Type type)
			{
				searchStatus = type;

				ContentDialog Reminder = new ContentDialog
				{
					Title = "Reminder",
					Content = "Tap on the account which you want to modify on.",
					CloseButtonText = "Got it"
				};

				await Reminder.ShowAsync();
			}
		}
	}
}