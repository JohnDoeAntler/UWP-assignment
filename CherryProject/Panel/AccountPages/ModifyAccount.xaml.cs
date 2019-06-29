using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using Microsoft.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
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
    public sealed partial class ModifyAccount : Page
    {
		private User user;

        public ModifyAccount()
        {
            this.InitializeComponent();

			CountryProvider.Instance.RegionList.ForEach(x =>
			{
				Region.Items.Add(x);
			});

			Role.ItemsSource = EnumManager.GetEnumList<RoleEnum>();

			Status.ItemsSource = EnumManager.GetEnumList<GeneralStatusEnum>();

			// permission control
			if (SignInManager.CurrentUser.Role != RoleEnum.AreaManager && SignInManager.CurrentUser.Role != RoleEnum.Administrator)
			{
				Role.IsEnabled = false;
				Status.IsEnabled = false;
			}
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is User user)
			{
				FillInformation(this.user = user);
			}
			else if (!PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(SearchAccounts)))
			{
				FillInformation(this.user = SignInManager.CurrentUser);
			}
			else
			{
				ContentDialog dialog = new ContentDialog
				{
					Title = "Alert",
					Content = "Which account would you want to modify?",
					PrimaryButtonText = "Modify Other People Account",
					CloseButtonText = "Modify Your Account"
				};

				ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

				if (result == ContentDialogResult.Primary)
				{
					Frame.Navigate(typeof(SearchAccounts), this.GetType(), new DrillInNavigationTransitionInfo());
				}
				else
				{
					FillInformation(this.user = SignInManager.CurrentUser);
				}
			}
		}

		private void FillInformation(User user)
		{
			Guid.Text = user.Id.ToString();
			Username.Text = user.UserName;
			Password.Password = user.PasswordHash;
			FirstName.Text = user.FirstName;
			LastName.Text = user.LastName;
			Email.Text = user.Email;
			PhoneNumber.Text = user.PhoneNumber;
			Role.SelectedItem = user.Role;
			Region.SelectedItem = user.Region;
			Address.SetText(user.Address);
			Status.SelectedItem = user.Status;
			Url.Text = user.IconUrl ?? string.Empty;
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Username.Text)
					|| Username.Text.Length < 6
					|| string.IsNullOrEmpty(Password.Password)
					|| Password.Password.Length < 8
					|| string.IsNullOrEmpty(FirstName.Text)
					|| string.IsNullOrEmpty(LastName.Text)
					|| !Email.Text.IsEmail()
					|| !PhoneNumber.Text.IsPhoneNumber()
					|| string.IsNullOrEmpty(Region.SelectedItem as string)
					|| Role.SelectedValue == null
					|| Status.SelectedValue == null
				)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						user = await user.ModifyAsync(x =>
						{
							x.UserName = Username.Text;
							x.PasswordHash = x.PasswordHash == Password.Password ? x.PasswordHash : Password.Password.GetMD5hash();
							x.FirstName = FirstName.Text;
							x.LastName = LastName.Text;
							x.Email = Email.Text;
							x.PhoneNumber = PhoneNumber.Text;
							x.Region = Region.SelectedItem as string;
							x.Role = (RoleEnum)Role.SelectedItem;
							x.Status = (GeneralStatusEnum)Status.SelectedItem;
							x.Address = Address.GetText();
							x.IconUrl = Url.Text;
						});

						// if the modifier != modified user
						if (user.Id != SignInManager.CurrentUser.Id)
						{
							NotificationManager.CreateNotification(user.Id, "Your Account Has Been Modified", $"{SignInManager.CurrentUser.FirstName} {SignInManager.CurrentUser.LastName} has modified your account information.", NotificationTypeEnum.Account, user.Id);
						}

						await new SuccessDialog().EnqueueAndShowIfAsync();

						Frame.Navigate(typeof(ViewAccount), user, new DrillInNavigationTransitionInfo());
					}
					catch (Exception)
					{
						await new ErrorDialog().EnqueueAndShowIfAsync();
					}
				}
			}
		}
	}
}
