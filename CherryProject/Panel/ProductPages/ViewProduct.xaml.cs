using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Service;
using CherryProject.ViewModel;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.ProductPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ViewProduct : Page
	{
		private readonly ObservableCollection<ViewTuple> displayItems;

		public ViewProduct()
		{
			this.InitializeComponent();

			displayItems = new ObservableCollection<ViewTuple>();

			// permission control
			ModifyProduct.IsEnabled = PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(ModifyProduct));
			ModifyProductStatus.IsEnabled = PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(ModifyProductStatus));
		}

		public ObservableCollection<ViewTuple> DisplayItems => displayItems;

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Product product)
			{
				SetProductInformation(product);

				// add event listener
				ModifyProduct.Click += (sender, args) => Frame.Navigate(typeof(ModifyProduct), product, new DrillInNavigationTransitionInfo());
				ModifyProductStatus.Click += (sender, args) => Frame.Navigate(typeof(ModifyProductStatus), product, new DrillInNavigationTransitionInfo());
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose a product before viewing it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				Frame.Navigate(typeof(SearchProducts), typeof(ViewProduct), new DrillInNavigationTransitionInfo());
			}
		}

		private void SetProductInformation(Product product)
		{
			ProductName.Text = product.Name;
			ProductId.Text = product.Id.ToString();

			displayItems.Add(new ViewTuple("Description", product.Description));

			using (var context = new Context())
			{
				// display price
				displayItems.Add(new ViewTuple("Price", context.PriceHistory.Where(x => x.ProductId == product.Id).OrderByDescending(x => x.Timestamp).FirstOrDefault().Price));
				// displayItems.Add(new ViewTuple("Price", $"$ {product.Price}"));
				displayItems.Add(new ViewTuple("Weight", $"{product.Weight} kg"));

				if (SignInManager.CurrentUser.Role != Model.Enum.RoleEnum.Dealer)
				{
					// dealer should not be able to see danger level
					displayItems.Add(new ViewTuple("Reorder Level", product.ReorderLevel));
					displayItems.Add(new ViewTuple("Danger Level", product.DangerLevel));
					displayItems.Add(new ViewTuple("Status", product.Status));

					// total (sold + existign) spare count
					uint sprcnt = (uint)context.Spare.Include(x => x.Category).Count(x => x.Category.ProductId == product.Id);
					// sold spare count
					uint didsprcnt = (uint)context.DidSpare.Include(x => x.Did).Count(x => x.Did.ProductId == product.Id);

					displayItems.Add(new ViewTuple("Remaining Stock", sprcnt - didsprcnt));

					uint didcnt = (uint)context.Did.Where(x => x.ProductId == product.Id).Sum(x => x.Quantity);

					displayItems.Add(new ViewTuple("Available Stock", sprcnt - didcnt));
				}
			}

			if (Uri.TryCreate(product.IconUrl, UriKind.Absolute, out Uri iconUrl) && iconUrl != null && (iconUrl.Scheme == Uri.UriSchemeHttp || iconUrl.Scheme == Uri.UriSchemeHttps))
			{
				Icon.ImageSource = new BitmapImage(iconUrl);
			}
		}
	}
}
