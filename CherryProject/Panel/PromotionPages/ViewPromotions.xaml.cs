using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
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

namespace CherryProject.Panel.PromotionPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ViewPromotions : Page
	{
		public ViewPromotions()
		{
			this.InitializeComponent();

			using (var context = new Context())
			{
				if (PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(ModifyPromotion)))
				{
					Promotions.ItemsSource = new ObservableCollection<Promotion>(context.Promotion);
				}
				else
				{
					Promotions.ItemsSource = new ObservableCollection<Promotion>(context.Promotion.Where(x => x.Status == GeneralStatusEnum.Available));
				}
			}

			ModifyPromotion.Click += (sender, args) => Frame.Navigate(typeof(ModifyPromotion), Promotions.SelectedItem, new DrillInNavigationTransitionInfo());
			ModifyPromotionStatus.Click += (sender, args) => Frame.Navigate(typeof(ModifyPromotionStatus), Promotions.SelectedItem, new DrillInNavigationTransitionInfo());
		}
	}
}
