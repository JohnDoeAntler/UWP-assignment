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
	public sealed partial class CategoryDialog : ContentDialog
	{
		private ObservableCollection<Category> searchResult;

		public CategoryDialog()
		{
			this.InitializeComponent();

			searchResult = new ObservableCollection<Category>();

			UpdateResultReocrds(string.Empty);
		}

		public ObservableCollection<Category> SearchResult { get => searchResult; set => searchResult = value; }
		public Category Category { get; set; }

		private void UpdateResultReocrds(string str)
		{
			searchResult.Clear();

			using (var context = new Context())
			{
				var result = context.Category.Include(x => x.Product).Where(x => EF.Functions.Like(x.NormalizedName, str + "%"));

				foreach (var category in result)
				{
					searchResult.Add(category);
				}
			}
		}

		private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) => UpdateResultReocrds(sender.Text);

		private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			Category = (Category)ResultListViewControl.SelectedItem;
			SelectedTarget.Text = $"Selected: {Category.Name}";
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (Category == null)
			{
				SelectedTarget.Text = $"Please select a category or else cancel the select dialog.";
				args.Cancel = true;
			}
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Category = null;
			this.Hide();
		}
	}
}
