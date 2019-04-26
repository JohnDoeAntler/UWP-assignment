using System;
using System.Collections.Generic;
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

namespace CherryProject.Panel.Other
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndexPage : Page
    {
        public IndexPage()
        {
            this.InitializeComponent();

            Window.Current.SetTitleBar(titleBar);

            contentFrame.Navigate(typeof(EmitNotification));

            header.Text = "Emit Notification";
        }

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
            else
            {
                Frame.Navigate(typeof(Panel.IndexPage), null, new DrillInNavigationTransitionInfo());
            }
        }

        private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
            }
            else
            {
                switch (args.InvokedItem)
                {
                    case "Emit Notification":
                        contentFrame.Navigate(typeof(EmitNotification), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Calendar":
                        contentFrame.Navigate(typeof(Calendar), null, new DrillInNavigationTransitionInfo());
                        break;
                }
                header.Text = args.InvokedItem.ToString();
            }
        }

        private void OnSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;

                switch (item.Tag)
                {
                    case "en":
                        contentFrame.Navigate(typeof(EmitNotification), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Emit Notification";
                        break;

                    case "ca":
                        contentFrame.Navigate(typeof(Calendar), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Calendar";
                        break;
                }
            }
        }
    }
}
