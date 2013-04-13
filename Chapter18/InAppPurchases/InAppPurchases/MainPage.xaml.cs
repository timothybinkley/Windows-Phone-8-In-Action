using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
#if DEBUG
using Windows.ApplicationModel.Store;
using Store = MockIAPLib;
#else
using Windows.ApplicationModel.Store;
#endif

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        public List<KeyValuePair<string, Windows.ApplicationModel.Store.ProductListing>> Products { get; set; }
        public Windows.ApplicationModel.Store.ListingInformation Listing { get; set; }
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            if (App.Player == null)
            {
                App.Player = new Player();                
            }
            
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Listing = await Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync();
            Products = Listing.ProductListings.ToList();
            PlayerGold.Text = App.Player.Gold.ToString();
            InAppItemsListBox.ItemsSource = Products;

            base.OnNavigatedTo(e);
        }

        private async void UIElement_OnTap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            StackPanel sp = (StackPanel) ((Button) sender).Parent;
            var Product = (KeyValuePair<string, Windows.ApplicationModel.Store.ProductListing>) sp.DataContext;

            Windows.ApplicationModel.Store.ProductLicense licence =
                Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses.SingleOrDefault(
                    l => l.Value.ProductId == Product.Value.ProductId).Value;
            if (Product.Value.ProductType == ProductType.Durable)
            {
                bool PlayerOwned = App.Player.Items.SingleOrDefault(i => i.ProductId == Product.Value.ProductId).Purchased;
            
                if (licence != null && (PlayerOwned || licence.IsActive))
                {
                    MessageBox.Show("You own this item.");
                }
                else
                {
                    if (licence == null || licence.IsActive == false)
                    {
                        NavigationService.Navigate(new Uri("/BuyItem.xaml?ProductId=" + Product.Value.ProductId,
                                                           UriKind.RelativeOrAbsolute));
                    }
                }
            }
            else if (Product.Value.ProductType == ProductType.Consumable)
            {
                NavigationService.Navigate(new Uri("/BuyItem.xaml?ProductId=" + Product.Value.ProductId,
                                                   UriKind.RelativeOrAbsolute));

            }
        }
    }
}