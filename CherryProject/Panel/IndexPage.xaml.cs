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

namespace CherryProject.Panel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IndexPage : Page
    {
        public IndexPage()
        {
            this.InitializeComponent();
            Window.Current.SetTitleBar(null);
        }

        private void NavigateAccountPage(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Account.IndexPage), null, new DrillInNavigationTransitionInfo());
        }

        private void NavigateOrderPage(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Order.IndexPage), null, new DrillInNavigationTransitionInfo());
        }

        private void NavigateProductPage(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Product.IndexPage), null, new DrillInNavigationTransitionInfo());
        }

        private void NavigatePromotionPage(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Promotion.IndexPage), null, new DrillInNavigationTransitionInfo());
        }

        private void NavigateSparePage(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Spare.IndexPage), null, new DrillInNavigationTransitionInfo());
        }

        private void NavigateInvoicePage(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Invoice.IndexPage), null, new DrillInNavigationTransitionInfo());
        }

        private void NavigateOtherPage(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Other.IndexPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
