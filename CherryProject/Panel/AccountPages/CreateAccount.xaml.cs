using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CherryProject.Panel.AccountPages
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
			Region.ItemsSource = new ObservableCollection<string>(CountryProvider.Instance.RegionList);

			// fill the role combo box with all role
			Role.ItemsSource = EnumManager.GetEnumList<RoleEnum>();

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
			// alert user
			ContentDialogResult result = await new ConfirmationDialog().EnqueueAndShowIfAsync();

			if (result == ContentDialogResult.Primary)
			{
				if (string.IsNullOrEmpty(Username.Text) || Username.Text.Length < 6
				|| string.IsNullOrEmpty(Password.Password) || Password.Password.Length < 8
				|| string.IsNullOrEmpty(FirstName.Text)
				|| string.IsNullOrEmpty(LastName.Text)
				|| !Email.Text.IsEmail()
				|| !PhoneNumber.Text.IsPhoneNumber()
				|| string.IsNullOrEmpty(Region.SelectedItem as string)
				|| Role.SelectedItem == null
				)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							var user = await context.User.AddAsync(new User()
							{
								Id = System.Guid.Parse(Guid.Text),
								UserName = Username.Text,
								PasswordHash = Password.Password.GetMD5hash(),
								FirstName = FirstName.Text,
								LastName = LastName.Text,
								Email = Email.Text,
								PhoneNumber = PhoneNumber.Text,
								Region = Region.SelectedItem as string,
								Role = (RoleEnum)Role.SelectedItem,
								Status = GeneralStatusEnum.Available,
								Address = Address.GetText(),
								IconUrl = Url.Text
							});

							await context.SaveChangesAsync();

							await new SuccessDialog().EnqueueAndShowIfAsync();

							this.Frame.Navigate(typeof(ViewAccount), user.Entity, new DrillInNavigationTransitionInfo());
						}
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
