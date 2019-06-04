using CherryProject.Extension;
using CherryProject.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingPage : Page
	{
		private readonly IPropertySet applicationDataContainer;

		public SettingPage()
		{
			this.InitializeComponent();

			// set button toggle state
			applicationDataContainer = StorageManager.GetApplicationDataContainer().Values;

			Theme.IsOn = (bool)(applicationDataContainer["theme"] ?? false);

			// set event handler
			Theme.Toggled += Theme_Toggled;

			// set title bar
			Window.Current.SetTitleBar(titleBar);
		}

		private async void Theme_Toggled(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you sure to change the theme?",
				PrimaryButtonText = "Yes",
				CloseButtonText = "Cancel"
			};

			if (await dialog.EnqueueAndShowIfAsync() == ContentDialogResult.Primary)
			{
				if (sender is ToggleSwitch toggle)
				{
					applicationDataContainer["theme"] = toggle.IsOn;
					RequestedTheme = toggle.IsOn ? ElementTheme.Light : ElementTheme.Dark;

					await new ContentDialog
					{
						Title = "Alert",
						Content = "Please restart the system after applying the changed theme.",
						CloseButtonText = "OK"
					}.EnqueueAndShowIfAsync();

					CoreApplication.Exit();
				}
			}
			else if (sender is ToggleSwitch toggle)
			{
				Theme.Toggled -= Theme_Toggled;
				toggle.IsOn = !toggle.IsOn;
				Theme.Toggled += Theme_Toggled;
			}
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			ContentDialog dialog = new ContentDialog
			{
				Title = "Confirmation",
				Content = "Are you sure to sign out from the system?",
				PrimaryButtonText = "Yes",
				CloseButtonText = "Cancel"
			};

			if (await dialog.EnqueueAndShowIfAsync() == ContentDialogResult.Primary)
			{
				SignInManager.SignOutAsync();
				this.Frame.Navigate(typeof(LoginPage), null, new DrillInNavigationTransitionInfo());
			}
		}

		private void OnBackRequested(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(IndexPage), null, new DrillInNavigationTransitionInfo());
		}
	}
}
