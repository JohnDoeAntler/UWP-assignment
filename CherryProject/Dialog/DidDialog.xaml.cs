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
	public sealed partial class DidDialog : ContentDialog
	{
		private readonly ObservableCollection<Did> searchDidGridViewItems;
		public DidDialog()
		{
			this.InitializeComponent();

			searchDidGridViewItems = new ObservableCollection<Did>();

			using (var context = new Context())
			{
				searchDidGridViewItems = new ObservableCollection<Did>(
					context
						.Did
						.Include(x => x.DidSpare)
						.Include(x => x.Product)
						.Include(x => x.Dic)
							.ThenInclude(x => x.Order)
								.ThenInclude(x => x.Dealer)
								.Where(x => x.Quantity > x.DidSpare.Count)
				);
			}
		}

		public DidDialog(Dic dic)
		{
			this.InitializeComponent();

			using (var context = new Context())
			{
				searchDidGridViewItems = new ObservableCollection<Did>(
					context
						.Did
						.Include(x => x.DidSpare)
						.Include(x => x.Product)
						.Where(x => x.DicId == dic.Id && x.Quantity > x.DidSpare.Count)
				);
			}
		}

		public Did Did { get; set; }

		public ObservableCollection<Did> SearchDidGridViewItems => searchDidGridViewItems;

		private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (ResultListViewControl.SelectedItem is Did did)
			{
				if (did.Quantity > did.DidSpare.Count)
				{
					Did = did;
					SelectedTarget.Text = $"Selected: {Did.Id}";
				}
				else
				{
					Did = null;
					SelectedTarget.Text = $"The tapped despatch instruction detail has been assembled completely. Please choose another one.";
				}
			}
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (Did == null)
			{
				SelectedTarget.Text = $"Please select a despatch instruction detail or else cancel the select dialog.";
				args.Cancel = true;
			}
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Did = null;
		}
	}
}
