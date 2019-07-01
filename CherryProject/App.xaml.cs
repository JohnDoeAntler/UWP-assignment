using CherryProject.Model;
using CherryProject.Panel;
using CherryProject.Panel.AccountPages;
using CherryProject.Panel.DispatchPages;
using CherryProject.Panel.OrderPages;
using CherryProject.Panel.ProductPages;
using CherryProject.Service;
using CherryProject.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
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

namespace CherryProject
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

			// set theme color
			RequestedTheme = (bool)(StorageManager.GetApplicationDataContainer().Values["theme"] ?? false) ? ApplicationTheme.Light : ApplicationTheme.Dark;
		}

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

			if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(Panel.LoginPage), e, new DrillInNavigationTransitionInfo());
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
		}

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

		protected override void OnActivated(IActivatedEventArgs e)
		{
			// Get the root frame
			Frame rootFrame = Window.Current.Content as Frame;

			// TODO: Initialize root frame just like in OnLaunched

			// Handle toast activation
			if (e is ToastNotificationActivatedEventArgs && SignInManager.CurrentUser != null)
			{
				var toastActivationArgs = e as ToastNotificationActivatedEventArgs;

				// Parse the query string (using QueryString.NET)
				QueryString args = QueryString.Parse(toastActivationArgs.Argument);

				Func<Type, Guid, Tuple<IndexGridViewItem, Guid>> ToNavigationParameters = (type, id) =>
				{
					var item = IndexPage.GetIndexGridViewItems(SignInManager.CurrentUser.Role).First(x => x.Views.Contains(type));
					item.Views = item.Views.OrderBy(x => x != type);
					return new Tuple<IndexGridViewItem, Guid>(item, id);
				};

				if (args.Contains("Type"))
				{
					switch (args["Type"])
					{
						case "Account":
						{
							rootFrame.Navigate(typeof(PanelPage), ToNavigationParameters(typeof(ViewAccount), Guid.Parse(args["ObjectId"])), new DrillInNavigationTransitionInfo());
							break;
						}
						case "Order":
						{
							rootFrame.Navigate(typeof(PanelPage), ToNavigationParameters(typeof(ViewOrder), Guid.Parse(args["ObjectId"])), new DrillInNavigationTransitionInfo());
							break;
						}
						case "Product":
						{
							rootFrame.Navigate(typeof(PanelPage), ToNavigationParameters(typeof(ViewProduct), Guid.Parse(args["ObjectId"])), new DrillInNavigationTransitionInfo());
							break;
						}
						case "Promotion":
						{
							rootFrame.Navigate(typeof(PanelPage), ToNavigationParameters(typeof(ViewOrder), Guid.Parse(args["ObjectId"])), new DrillInNavigationTransitionInfo());
							break;
						}
						case "Dic":
						{
							rootFrame.Navigate(typeof(PanelPage), ToNavigationParameters(typeof(ViewOrderDispatchStatus), Guid.Parse(args["ObjectId"])), new DrillInNavigationTransitionInfo());
							break;
						}
					}
				}
			}

			// TODO: Handle other types of activation

			// Ensure the current window is active
			Window.Current.Activate();
		}
	}
}
