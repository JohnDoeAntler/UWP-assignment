using CherryProject.Model;
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

namespace CherryProject.Panel.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewAccount : Page
    {
		private readonly ObservableCollection<Tuple<string, string>> _displayItems;
		public ObservableCollection<Tuple<string, string>> DisplayItems => _displayItems;

		public ViewAccount()
        {
            this.InitializeComponent();

			_displayItems = new ObservableCollection<Tuple<string, string>>();

			DataContext = _displayItems;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var user = e.Parameter is Users ? e.Parameter as Users: SignInManager.CurrentUser;

			SetProfileInformation(user);
			ModifyAccount.Click += (sender, args) => Frame.Navigate(typeof(ModifyAccount), user, new DrillInNavigationTransitionInfo());
			DisableAccount.Click += (sender, args) => Frame.Navigate(typeof(DisableAccount), user, new DrillInNavigationTransitionInfo());
		}

		private void SetProfileInformation(Users user)
		{
			FirstName.Text = user.FirstName;
			LastName.Text = user.LastName;
			Username.Text = user.UserName;

			_displayItems.Add(("Email",			user.Email).ToTuple<string, string>());
			_displayItems.Add(("Phone Number",	user.PhoneNumber).ToTuple<string, string>());
			_displayItems.Add(("Region",		user.Region).ToTuple<string, string>());
			_displayItems.Add(("Address",		user.Address).ToTuple<string, string>());
			_displayItems.Add(("Role",			user.Role.Name).ToTuple<string, string>());
			_displayItems.Add(("Status",		user.Status).ToTuple<string, string>());
		}
	}
}
