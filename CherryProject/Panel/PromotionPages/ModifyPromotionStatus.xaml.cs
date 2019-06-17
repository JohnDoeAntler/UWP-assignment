using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using System;
using System.Collections.Generic;
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
	public sealed partial class ModifyPromotionStatus : Page
	{
		private Promotion promotion;

		public ModifyPromotionStatus()
		{
			this.InitializeComponent();

			Status.ItemsSource = EnumManager.GetEnumList<GeneralStatusEnum>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Promotion promotion)
			{
				Status.SelectedItem = (this.promotion = promotion).Status;

				Status.SelectionChanged += Status_SelectionChanged;
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose an product before modifying its status.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				this.Frame.Navigate(typeof(ViewPromotions), this.GetType(), new DrillInNavigationTransitionInfo());
			}
		}

		private async void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				try
				{
					await promotion.ModifyAsync(x => x.Status = (GeneralStatusEnum)Status.SelectedItem);

					await new SuccessDialog().EnqueueAndShowIfAsync();

					this.Frame.Navigate(typeof(ViewPromotions), null, new DrillInNavigationTransitionInfo());
				}
				catch (Exception)
				{
					await new ErrorDialog().EnqueueAndShowIfAsync();
				}
			}
		}
	}
}
