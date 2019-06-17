using CherryProject.Dialog;
using CherryProject.Extension;
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

namespace CherryProject.Panel.AccountPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DisableAccount : Page
    {
		private User user;

        public DisableAccount()
        {
            this.InitializeComponent();

			Status.ItemsSource = EnumManager.GetEnumList<GeneralStatusEnum>();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is User user)
			{
				Status.SelectedItem = (this.user = user).Status;

				Status.SelectionChanged += Status_SelectionChanged;
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "You have to choose an account before disabling it.",
					CloseButtonText = "OK"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				this.Frame.Navigate(typeof(SearchAccounts), this.GetType(), new DrillInNavigationTransitionInfo());
			}
		}

		private async void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				try
				{
					user = await user.ModifyAsync(x => x.Status = (GeneralStatusEnum) Status.SelectedItem);

					await new SuccessDialog().EnqueueAndShowIfAsync();

					this.Frame.Navigate(typeof(ViewAccount), user, new DrillInNavigationTransitionInfo());
				}
				catch (Exception)
				{
					await new ErrorDialog().EnqueueAndShowIfAsync();
				}
			}
		}
	}
}
