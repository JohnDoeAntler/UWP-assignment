﻿using CherryProject.Extension;
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
	public sealed partial class DicDialog : ContentDialog
	{
		private readonly ObservableCollection<Dic> searchDicGridViewItems;

		public DicDialog()
		{
			this.InitializeComponent();

			searchDicGridViewItems = new ObservableCollection<Dic>();

			using (var context = new Context())
			{
				// var dics = context.Order.Include(x => x.Dic).FirstOrDefault(x => x.Id == order.Id).Dic;
				IEnumerable<Dic> dics = context.Dic.Include(x => x.Did).Include(x => x.Order).ThenInclude(x => x.Dealer);

				if (SignInManager.CurrentUser.Role == RoleEnum.Dealer)
				{
					dics = dics.Where(x => x.Order.DealerId == SignInManager.CurrentUser.Id);
				}

				foreach (var dic in dics)
				{
					// fullfill assembiled dic quantity:
					var total = dic.Did.Sum(x => x.Quantity);

					// current assembiled
					var now = context.DidSpare.Include(x => x.Did).Count(x => x.Did.DicId == dic.Id);

					dic.Status = total > now ? $"Assembled: {now} / {total}" : dic.Status;
				}

				searchDicGridViewItems = new ObservableCollection<Dic>(dics);
			}
		}

		public DicDialog(Order order)
		{
			this.InitializeComponent();

			using (var context = new Context())
			{
				// var dics = context.Order.Include(x => x.Dic).FirstOrDefault(x => x.Id == order.Id).Dic;
				var dics = context.Dic.Include(x => x.Did).Include(x => x.Order).ThenInclude(x => x.Dealer).Where(x => x.OrderId == order.Id);

				foreach (var dic in dics)
				{
					// fullfill assembiled dic quantity:
					var total = dic.Did.Sum(x => x.Quantity);

					// current assembiled
					var now = context.DidSpare.Include(x => x.Did).Count(x => x.Did.DicId == dic.Id);

					dic.Status = total > now ? $"Assembled: {now} / {total}" : dic.Status;
				}

				searchDicGridViewItems = new ObservableCollection<Dic>(dics);
			}
		}

		public Dic Dic { get; set; }

		private ObservableCollection<Dic> SearchDicGridViewItems => searchDicGridViewItems;

		private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
		{
			if (ResultListViewControl.SelectedItem is Dic dic)
			{
				Dic = dic;
				SelectedTarget.Text = $"Selected: {Dic.Id}";
				SelectedTarget.Visibility = Visibility.Visible;
			}
		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			if (Dic == null)
			{
				SelectedTarget.Text = $"Please select a despatch instruction cover or else cancel the select dialog.";
				SelectedTarget.Visibility = Visibility.Visible;
				args.Cancel = true;
			}
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
			Dic = null;
			Hide();
		}
	}
}
