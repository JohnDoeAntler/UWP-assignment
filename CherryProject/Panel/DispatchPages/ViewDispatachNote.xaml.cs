using CherryProject.Dialog;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.DispatchPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ViewDispatachNote : Page
	{
		private Dic dic;
		private readonly ObservableCollection<Did> searchDidGridViewItems;

		public ViewDispatachNote()
		{
			this.InitializeComponent();

			this.searchDidGridViewItems = new ObservableCollection<Did>();
		}

		public Dic Dic { get => dic; set => dic = value; }

		public ObservableCollection<Did> SearchDidGridViewItems => searchDidGridViewItems;

		private async void SelectDic_Click(object sender, RoutedEventArgs e)
		{
			DicDialog dialog = new DicDialog();

			if (await dialog.EnqueueAndShowIfAsync() == ContentDialogResult.Primary)
			{
				DicGUID.Text = (dic = dialog.Dic).Id.ToString();
				SelectedDic.Text = $"Selected DIC: {dic.Id}";
				SelectedDic.Visibility = Visibility.Visible;

				UpdateDids(SearchBox.Text);
			}
		}

		private void UpdateDids(string str = null)
		{
			var list = new ObservableCollection<Did>();

			using (var context = new Context())
			{
				// searchDidGridViewItems = new ObservableCollection<Did>(context.Dic.Include(x => x.Did).FirstOrDefault(x => x.Id == dic.Id).Did);
				list = new ObservableCollection<Did>(
					context
						.Did
						.Include(x => x.DidSpare)
						.Include(x => x.Product)
						.Where(x => x.DicId == dic.Id && EF.Functions.Like(x.Product.Name, (str ?? string.Empty) + "%"))
				);
			}

			searchDidGridViewItems.UpdateObservableCollection(list);
			SearchBox.IsEnabled = true;
			ResultAlerter.Text = $"There has only found {searchDidGridViewItems.Count} result(s).";
		}

		private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args) => UpdateDids(sender.Text);

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Dic dic)
			{
				DicGUID.Text = (this.dic = dic).Id.ToString();
				SelectedDic.Text = $"Selected DIC: {dic.Id}";
				SelectedDic.Visibility = Visibility.Visible;

				UpdateDids();
			}
		}
	}
}
