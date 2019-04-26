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

namespace CherryProject.Panel.Account
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

            contentFrame.Navigate(typeof(CreateAccount));
            header.Text = "Create Account";
        }

        private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            } else
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
                    case "View Account":
                        contentFrame.Navigate(typeof(ViewAccount), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Search Accounts":
                        contentFrame.Navigate(typeof(SearchAccounts), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Create Account":
                        contentFrame.Navigate(typeof(CreateAccount), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Modify Account":
                        contentFrame.Navigate(typeof(ModifyAccount), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Disable Account":
                        contentFrame.Navigate(typeof(DisableAccount), null, new DrillInNavigationTransitionInfo());
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
                    case "va":
                        contentFrame.Navigate(typeof(ViewAccount), null, new DrillInNavigationTransitionInfo());
                        header.Text = "View Account";
                        break;

                    case "sa":
                        contentFrame.Navigate(typeof(SearchAccounts), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Search Accounts";
                        break;

                    case "ca":
                        contentFrame.Navigate(typeof(CreateAccount), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Create Account";
                        break;

                    case "ma":
                        contentFrame.Navigate(typeof(ModifyAccount), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Modify Account";
                        break;

                    case "da":
                        contentFrame.Navigate(typeof(DisableAccount), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Disable Account";
                        break;
                }
            }
        }
    }
}
