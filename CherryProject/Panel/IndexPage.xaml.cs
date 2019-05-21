using CherryProject.Extension;
using CherryProject.Model.Enum;
using CherryProject.Service;
using CherryProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndexPage : Page
	{
		private static IEnumerable<IndexGridViewItem> IndexGridViewItems
		{
			get => new[]{
				new IndexGridViewItem{
					Title = "Account Management",
					Description = "Account observation, creation, modification and remove control.",
					Icon = Symbol.ContactInfo,
					Views = PermissionManager.GetTypesInNamespace("CherryProject.Panel.Account")
				},
				new IndexGridViewItem{
					Title = "Order Processing",
					Description = "Order processing view, statistics and trend calculation, creation, modification and remove.",
					Icon = Symbol.MoveToFolder,
					Views = PermissionManager.GetTypesInNamespace("CherryProject.Panel.Order")
				},
				new IndexGridViewItem{
					Title = "Product Management",
					Description = "Product supplement, modification and view.",
					Icon = Symbol.Page2,
					Views = PermissionManager.GetTypesInNamespace("CherryProject.Panel.Product")
				},
				new IndexGridViewItem{
					Title = "Promotion Management",
					Description = "Promotion view, supplement and modification.",
					Icon = Symbol.Pictures,
					Views = PermissionManager.GetTypesInNamespace("CherryProject.Panel.Promotion")
				},
				new IndexGridViewItem{
					Title = "Spare Management",
					Description = "Spare information view and spare status modification.",
					Icon = Symbol.PreviewLink,
					Views = PermissionManager.GetTypesInNamespace("CherryProject.Panel.Spare")
				},
				new IndexGridViewItem{
					Title = "Invoice Management",
					Description = "Invoice generation and invoice content and status modification.",
					Icon = Symbol.AlignCenter,
					Views = PermissionManager.GetTypesInNamespace("CherryProject.Panel.Invoice")
				},
				new IndexGridViewItem{
					Title = "Other",
					Description = "Emit notification, etc.",
					Icon = Symbol.Comment,
					Views = PermissionManager.GetTypesInNamespace("CherryProject.Panel.Other")
				}
			};
		}

		public static ObservableCollection<IndexGridViewItem> GetIndexGridViewItems(RoleEnum role)
			=> new ObservableCollection<IndexGridViewItem>(IndexGridViewItems.Where(x => x.Views.Any(y => PermissionManager.GetPermission(role).Any(z => y.Name == z.Name))));

		private ObservableCollection<IndexGridViewItem> _items;

		private ObservableCollection<IndexGridViewItem> Items { get => this._items; }

		public IndexPage()
        {
            this.InitializeComponent();

			_items = GetIndexGridViewItems(SignInManager.CurrentUser.Role.ToRoleEnum());

			_items.Add(new IndexGridViewItem()
			{
				Title = "Setting",
				Description = "Login out, theme customization and language selection.",
				Icon = Symbol.Setting,
			});

			Window.Current.SetTitleBar(null);
        }

		private void Navigate(object sender, TappedRoutedEventArgs e)
		{
			var item = sender as GridViewItem;

			for (int i = 0; i < _items.Count; i++)
			{
				if (item.Tag as string == _items[i].Tag)
				{
					Frame.Navigate(typeof(PanelPage), _items[i], new DrillInNavigationTransitionInfo());
				}
			}
		}

		private void OnTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
		{
			if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
			{
				if (string.IsNullOrEmpty(sender.Text))
				{
					sender.ItemsSource = null;
				}else
				{
					var list = new ObservableCollection<NavigationViewItemBase>(
						PermissionManager
							.GetPermission(SignInManager.CurrentUser.Role.ToRoleEnum())
							.Where(x => x.ClassNameToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase))
							.Take(5)
							.Select(x => new NavigationViewItem()
							{
								Content = x.ClassNameToString(),
								Icon = new SymbolIcon(
									_items
										.FirstOrDefault(y => y.Views.Any(z => z.Name == x.Name))
										.Icon
								)
							}
						)
					);

					if (list.Count != 0)
					{
						list.Add(new NavigationViewItemSeparator());
						list.Add(new NavigationViewItem()
						{
							Content = "Show all results"
						});
					}
					else
					{
						list.Add(new NavigationViewItem()
						{
							Content = $"No results for {sender.Text}",
							IsEnabled = false
						});
					}

					sender.ItemsSource = list;
				}
			}
		}

		private void OnQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (args.ChosenSuggestion != null)
			{
				// User selected an item from the suggestion list, take an action on it here.

				var selected = args.ChosenSuggestion as string;

				foreach (var item in _items)
				{
					foreach (var view in item.Views)
					{
						if (selected == view.ClassNameToString())
						{
							item.Views = item.Views.OrderBy(x => x != view);

							Frame.Navigate(typeof(PanelPage), item, new DrillInNavigationTransitionInfo());
						}
					}
				}
			}
			else
			{
				// Use args.QueryText to determine what to do.

				var result = PermissionManager
						.GetPermission(RoleEnum.Administrator)
						.FirstOrDefault(x => x.ClassNameToString().Contains(sender.Text, StringComparison.OrdinalIgnoreCase));

				if (result != null)
				{
					var item = _items.FirstOrDefault(x => x.Views.Any(y => y == result));
					item.Views = item.Views.OrderBy(x => x != result);

					Frame.Navigate(typeof(PanelPage), item, new DrillInNavigationTransitionInfo());
				}
			}
		}
	}
}
