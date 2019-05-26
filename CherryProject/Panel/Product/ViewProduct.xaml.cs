﻿using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.ViewModel;
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

namespace CherryProject.Panel.Product
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ViewProduct : Page
	{
		private readonly ObservableCollection<ViewTuple> _displayItems;
		public ObservableCollection<ViewTuple> DisplayItems => _displayItems;

		public ViewProduct()
		{
			this.InitializeComponent();

			_displayItems = new ObservableCollection<ViewTuple>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Products product)
			{
				SetProductInformation(product);

				ModifyProduct.Click += (sender, args) => Frame.Navigate(typeof(ModifyProduct), product, new DrillInNavigationTransitionInfo());
				ModifyProductStatus.Click += (sender, args) => Frame.Navigate(typeof(ModifyProductStatus), product, new DrillInNavigationTransitionInfo());
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose a product before viewing it.",
					PrimaryButtonText = "Select Product",
					CloseButtonText = "Cancel"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(SearchProducts), typeof(ViewProduct), new DrillInNavigationTransitionInfo());
			}
		}

		private void SetProductInformation(Products product)
		{
			ProductName.Text = product.Name;
			ProductId.Text = product.Id;

			_displayItems.Add(new ViewTuple("Description", product.Description));
			_displayItems.Add(new ViewTuple("Price", product.Price));
			_displayItems.Add(new ViewTuple("Weight", product.Weight));
			// dealer should not be able to see danger level
			_displayItems.Add(new ViewTuple("Danger Level", product.DangerLevel));
			_displayItems.Add(new ViewTuple("Status", product.Status));
		}
	}
}
