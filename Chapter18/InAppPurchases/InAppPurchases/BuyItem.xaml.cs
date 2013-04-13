using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Xml.Linq;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
#if DEBUG
using Windows.ApplicationModel.Store;
using MockIAPLib;
using Store = MockIAPLib;
#else
using Windows.ApplicationModel.Store;
#endif

namespace PhoneApp1
{
    public partial class BuyItem : PhoneApplicationPage
    {
        public Windows.ApplicationModel.Store.ProductListing Product { get; set; }

        public BuyItem()
        {
            InitializeComponent();

        }

        protected override async void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string productId = NavigationContext.QueryString["ProductId"];

            var listings = await Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync();

            Product = listings.ProductListings.Single(l => l.Value.ProductId == productId).Value;
            this.DataContext = Product;

            base.OnNavigatedTo(e);
        }
        private async void IAP_BuyConsumables(object sender, RoutedEventArgs e)
        {
            var listing = await Windows.ApplicationModel.Store.CurrentApp.LoadListingInformationAsync();
            var firstConsumable =
                listing.ProductListings.FirstOrDefault(p => p.Value.ProductType == ProductType.Consumable);

            var recipet = await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync(firstConsumable.Value.ProductId, true);

            if (Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[firstConsumable.Value.ProductId].IsActive)
            {
                // do something
                Windows.ApplicationModel.Store.CurrentApp.ReportProductFulfillment(firstConsumable.Value.ProductId);
            }

            MessageBox.Show(recipet, "Reciept", MessageBoxButton.OK);
        }
        private async void BuyButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var reciept = await Windows.ApplicationModel.Store.CurrentApp.RequestProductPurchaseAsync(Product.ProductId, true);

                if (Windows.ApplicationModel.Store.CurrentApp.LicenseInformation.ProductLicenses[Product.ProductId].IsActive)
                {
                    if (Product.ProductType == ProductType.Consumable)
                    {
                        if (Product.ProductId == "GoldCoins100")
                        {
                            App.Player.Gold += 100;
                            Windows.ApplicationModel.Store.CurrentApp.ReportProductFulfillment(Product.ProductId);
                        }
                    }
                    else if (Product.ProductType == ProductType.Durable)
                    {
                        var item = App.Player.Items.Single(i => i.ProductId == Product.ProductId);
                        if (item != null)
                        {
                            item.Purchased = true;
                        }
                    }
                }

                ProcessPurchaseReciept(reciept);
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.RelativeOrAbsolute));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private void ProcessPurchaseReciept(string ProductPurchaseResponse)
        {
            try
            {
                MessageBox.Show(ProductPurchaseResponse);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                
                //throw;
            }
        }
    }
}