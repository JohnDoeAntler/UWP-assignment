using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Service;
using CherryProject.ViewModel;
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

namespace CherryProject.Panel.AccountPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewAccount : Page
    {
		private readonly ObservableCollection<ViewTuple> _displayItems;

		public ViewAccount()
        {
            this.InitializeComponent();

			_displayItems = new ObservableCollection<ViewTuple>();

			// permision control
			ModifyAccount.IsEnabled = PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(ModifyAccount));
			DisableAccount.IsEnabled = PermissionManager.GetPermission(SignInManager.CurrentUser.Role).Contains(typeof(DisableAccount));
		}

		public ObservableCollection<ViewTuple> DisplayItems => _displayItems;

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			var user = e.Parameter is User ? e.Parameter as User: SignInManager.CurrentUser;

			SetProfileInformation(user);
			ModifyAccount.Click += (sender, args) => Frame.Navigate(typeof(ModifyAccount), user, new DrillInNavigationTransitionInfo());
			DisableAccount.Click += (sender, args) => Frame.Navigate(typeof(DisableAccount), user, new DrillInNavigationTransitionInfo());
		}

		private void SetProfileInformation(User user)
		{
			FirstName.Text = user.FirstName;
			LastName.Text = user.LastName;
			Username.Text = user.UserName;

			_displayItems.Add(new ViewTuple("Email",		user.Email));
			_displayItems.Add(new ViewTuple("Phone Number",	user.PhoneNumber));
			_displayItems.Add(new ViewTuple("Region",		user.Region));
			_displayItems.Add(new ViewTuple("Address",		user.Address));
			_displayItems.Add(new ViewTuple("Role",			user.Role));
			_displayItems.Add(new ViewTuple("Status",		user.Status));

			if (Uri.TryCreate(user.IconUrl, UriKind.Absolute, out Uri iconUrl) && iconUrl != null && (iconUrl.Scheme == Uri.UriSchemeHttp || iconUrl.Scheme == Uri.UriSchemeHttps))
			{
				Icon.ImageSource = new BitmapImage(iconUrl);
			}
		}
	}
}
