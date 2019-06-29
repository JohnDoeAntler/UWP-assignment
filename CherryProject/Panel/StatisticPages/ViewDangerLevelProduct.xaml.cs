﻿using CherryProject.Model;
using CherryProject.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

namespace CherryProject.Panel.StatisticPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ViewDangerLevelProduct : Page
	{
		private readonly ObservableCollection<ProductViewModel> products;

		public ViewDangerLevelProduct()
		{
			this.InitializeComponent();

			using (var context = new Context())
			{
				products = new ObservableCollection<ProductViewModel>();

				foreach (var item in context.Product.Include(x => x.PriceHistory))
				{
					long stock;

					if (item.DangerLevel > (stock = context.Spare.Include(y => y.Category).Count(y => y.Category.ProductId == item.Id) - context.Did.Where(y => y.ProductId == item.Id).Sum(x => x.Quantity)))
					{
						products.Add(new ProductViewModel(item, stock));
					}
				}
			}
		}

		public ObservableCollection<ProductViewModel> Products { get => products; }
	}
}
