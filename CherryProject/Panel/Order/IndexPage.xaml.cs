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

namespace CherryProject.Panel.Order
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

            contentFrame.Navigate(typeof(CreateOrder));

            header.Text = "Create Order";
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
                    case "View Order":
                        contentFrame.Navigate(typeof(ViewOrder), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Search Orders":
                        contentFrame.Navigate(typeof(SearchOrders), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Create Order":
                        contentFrame.Navigate(typeof(CreateOrder), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Modify Order":
                        contentFrame.Navigate(typeof(ModifyOrder), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Endorse Order":
                        contentFrame.Navigate(typeof(EndorseOrder), null, new DrillInNavigationTransitionInfo());
                        break;

                    case "Cancel Order":
                        contentFrame.Navigate(typeof(CancelOrder), null, new DrillInNavigationTransitionInfo());
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
                    case "vo":
                        contentFrame.Navigate(typeof(ViewOrder), null, new DrillInNavigationTransitionInfo());
                        header.Text = "View Order";
                        break;

                    case "so":
                        contentFrame.Navigate(typeof(SearchOrders), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Search Orders";
                        break;

                    case "co":
                        contentFrame.Navigate(typeof(CreateOrder), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Create Order";
                        break;

                    case "mo":
                        contentFrame.Navigate(typeof(ModifyOrder), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Modify Order";
                        break;

                    case "eo":
                        contentFrame.Navigate(typeof(EndorseOrder), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Endorse Order";
                        break;

                    case "ca":
                        contentFrame.Navigate(typeof(CancelOrder), null, new DrillInNavigationTransitionInfo());
                        header.Text = "Cancel Order";
                        break;
                }
            }
        }
    }
}
