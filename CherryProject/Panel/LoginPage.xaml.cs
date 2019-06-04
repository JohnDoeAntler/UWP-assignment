using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media.Animation;
using CherryProject.Model;
using CherryProject.Service;
using CherryProject.Service.SignStatus;
using CherryProject.Extension;
using System.Threading.Tasks;
using System.Timers;
using CherryProject.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();

            var formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
		}

        private void OnClick(object sender, RoutedEventArgs e)
		{
			Submit();
		}

		private void OnKeyDown(object sender, KeyRoutedEventArgs e)
		{
			if (e.Key == Windows.System.VirtualKey.Enter)
			{
				Submit();
			}
		}

		private async void Submit()
		{
			PassportSignInButton.Visibility = Visibility.Collapsed;
			ProgressBar.Visibility = Visibility.Visible;
			UsernameTextBox.IsEnabled = false;
			PasswordTextBox.IsEnabled = false;
			ValidationAlerter.Text = string.Empty;

			string username = UsernameTextBox.Text;
			string password = PasswordTextBox.Password;

			// The await causes the handler to return immediately.
			var validation = await Task.Run(async () => await SignInManager.SignInAsync(username, password));

			switch (validation)
			{
				case Status.Success:
					Frame.Navigate(typeof(IndexPage), null, new DrillInNavigationTransitionInfo());
					break;

				case Status.PasswordFailure:
					ValidationAlerter.Text = "Password mismatch, please enter the correct information.";
					break;

				case Status.Disabled:
					ValidationAlerter.Text = "This account has been disabled already. To recovery your account, please contact area manager.";
					break;

				case Status.UsernameFailure:
					ValidationAlerter.Text = "Invalid username has been entered. please make sure the above information is valid.";
					break;

				case Status.DatabaseFailure:
					ValidationAlerter.Text = "Unable to connect to database, please contact our technical support department.";
					break;
			}

			PassportSignInButton.Visibility = Visibility.Visible;
			ProgressBar.Visibility = Visibility.Collapsed;
			UsernameTextBox.IsEnabled = true;
			PasswordTextBox.IsEnabled = true;
		}
	}
}
