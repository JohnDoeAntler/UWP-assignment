using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using Microsoft.Toolkit.Extensions;
using System;
using System.Collections.Generic;
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

namespace CherryProject.Panel.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ModifyAccount : Page
    {
		private Users user;

        public ModifyAccount()
        {
            this.InitializeComponent();

			CountryProvider.Instance.RegionList.ForEach(x =>
			{
				Region.Items.Add(x);
			});

			Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>().ToList().ForEach(x =>
			{
				Role.Items.Add(x.ToString());
			});

			Enum.GetValues(typeof(StatusEnum)).Cast<StatusEnum>().ToList().ForEach(x =>
			{
				Status.Items.Add(x.ToString());
			});
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Users user)
			{
				FillInformation(this.user = user);
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
					this.Frame.Navigate(typeof(SearchAccounts), this.GetType(), new DrillInNavigationTransitionInfo());
				}
				else
				{
					FillInformation(this.user = SignInManager.CurrentUser);
				}
			}
		}

		private void FillInformation(Users user)
		{
			Guid.Text = user.Id;
			Username.Text = user.UserName;
			Password.Password = user.PasswordHash;
			FirstName.Text = user.FirstName;
			LastName.Text = user.LastName;
			Email.Text = user.Email;
			PhoneNumber.Text = user.PhoneNumber;
			Role.SelectedItem = user.Role.Name;
			Region.SelectedItem = user.Region;
			Address.Document.SetText(Windows.UI.Text.TextSetOptions.None, user.Address);
			Status.SelectedItem = user.Status;
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to modify current account?",
				PrimaryButtonText = "Modify Account",
				CloseButtonText = "Cancel"
			};

			ContentDialogResult result = await dialog.EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Username.Text) || Username.Text.Length < 6
				|| string.IsNullOrEmpty(Password.Password) || Password.Password.Length < 8
				|| string.IsNullOrEmpty(FirstName.Text)
				|| string.IsNullOrEmpty(LastName.Text)
				|| !Email.Text.IsEmail()
				|| !PhoneNumber.Text.IsPhoneNumber()
				|| string.IsNullOrEmpty(Region.SelectedItem as string)
				|| string.IsNullOrEmpty(Role.SelectedItem as string)
				|| string.IsNullOrEmpty(Status.SelectedItem as string)
				)
				{
					ContentDialog error = new ContentDialog
					{
						Title = "Error",
						Content = "The information you typed has mistakes, please ensure the input data validation is correct.",
						CloseButtonText = "OK",
						Width = 400
					};

					await error.EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						var roleId = (await Role.SelectedItem.ToString().ToRoleAsync()).Id;

						user = await user.ModifyAsync(x =>
							{
								x.UserName = Username.Text;
								x.PasswordHash = x.PasswordHash == Password.Password ? x.PasswordHash : Password.Password.GetMD5hash();
								x.FirstName = FirstName.Text;
								x.LastName = LastName.Text;
								x.Email = Email.Text;
								x.PhoneNumber = PhoneNumber.Text;
								x.Region = Region.SelectedItem as string;
								x.RoleId = roleId;
								x.Status = Status.SelectedItem as string;
								x.Address = Address.GetText();
							}
						);

						ContentDialog message = new ContentDialog
						{
							Title = "Success",
							Content = "Successfully modified user.",
							CloseButtonText = "OK",
							Width = 400
						};

						await message.EnqueueAndShowIfAsync();

						this.Frame.Navigate(typeof(ViewAccount), user, new DrillInNavigationTransitionInfo());
					}
					catch (Exception)
					{
						ContentDialog error = new ContentDialog
						{
							Title = "Error",
							Content = "The information you typed might duplicated, please re-type the username and make sure the email has not been registered before.",
							CloseButtonText = "OK",
							Width = 400
						};

						await error.EnqueueAndShowIfAsync();
					}
				}
			}
		}
	}
}
