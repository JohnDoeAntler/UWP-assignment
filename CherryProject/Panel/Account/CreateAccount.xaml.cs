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

namespace CherryProject.Panel.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreateAccount : Page
    {
        public CreateAccount()
        {
            this.InitializeComponent();

			// fill the region combo box
			CountryProvider.Instance.RegionList.ForEach(x =>
			{
				Region.Items.Add(x);
			});

			// fill the role combo box with all role
			Enum.GetValues(typeof(RoleEnum)).Cast<RoleEnum>().ToList().ForEach(x =>
			{
				Role.Items.Add(x.ToString());
			});

			// automatically generate a new guid
			Guid.Text = System.Guid.NewGuid().ToString();
		}

		/// <summary>
		/// generate a new guid when user tapped generate button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GenerateGuidBtn_OnClick(object sender, RoutedEventArgs e)
		{
			Guid.Text = System.Guid.NewGuid().ToString();
		}

		/// <summary>
		/// create an account after tapping the create account button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			// instantiate a dialog object
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you ensure to create an account?",
				PrimaryButtonText = "Create Account",
				CloseButtonText = "Cancel"
			};

			// alert user
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
					var role = await Role.SelectedItem.ToString().ToRoleAsync();

					try
					{
						var user = await UserManager.CreateAsync(new Users()
						{
							Id = Guid.Text,
							UserName = Username.Text,
							PasswordHash = Password.Password.GetMD5hash(),
							FirstName = FirstName.Text,
							LastName = LastName.Text,
							Email = Email.Text,
							PhoneNumber = PhoneNumber.Text,
							Region = Region.SelectedItem as string,
							RoleId = role.Id,
							Status = StatusEnum.Available.ToString(),
							Address = Address.GetText(),
							ConcurrencyStamp = GuidHelper.CreateNewGuid().ToString(),
							SecurityStamp = GuidHelper.CreateNewGuid().ToString(),
							EmailConfirmed = false,
							PhoneNumberConfirmed = false
						});

						user.Role = role;

						ContentDialog message = new ContentDialog
						{
							Title = "Success",
							Content = "Successfully created user.",
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
					}
				}
			}
		}
	}
}
