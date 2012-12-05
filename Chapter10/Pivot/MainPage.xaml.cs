using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace Pivot
{
    public partial class MainPage : PhoneApplicationPage
    {
        IEnumerable<SampleData> data;

        public MainPage()
        {
            InitializeComponent();
            data = SampleData.GenerateSampleData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bool loadAllData = false;
            IsolatedStorageSettings.ApplicationSettings.TryGetValue("loadAllData", out loadAllData);
            allDataOption.IsChecked = loadAllData;
            asNeededOption.IsChecked = !loadAllData;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            IsolatedStorageSettings.ApplicationSettings["loadAllData"] = allDataOption.IsChecked.Value;
            State["selection"] = pivot.SelectedIndex;
        }

        private void pivot_Loaded(object sender, RoutedEventArgs e)
        {
            if (State.ContainsKey("selection"))
            {
                pivot.SelectedIndex = (int)State["selection"];
            }

            if (allDataOption.IsChecked.Value)
            {
                allDataList.ItemsSource = data;
                filteredDataList.ItemsSource = from d in data
                                               where d.Category == SampleCategory.Even
                                               select d;
            }
        }

        private void pivot_LoadedPivotItem(object sender, PivotItemEventArgs e)
        {
            if (e.Item == allDataItem && allDataList.ItemsSource == null)
            {
                allDataList.ItemsSource = data;
            }
            else if (e.Item == filteredDataItem && filteredDataList.ItemsSource == null)
            {
                filteredDataList.ItemsSource = from d in data
                                               where d.Category == SampleCategory.Even
                                               select d;
            }
        }

        private void pivot_UnloadedPivotItem(object sender, PivotItemEventArgs e)
        {
            if (!allDataOption.IsChecked.Value)
            {
                if (e.Item == allDataItem)
                {
                    allDataList.ItemsSource = null;
                }
                else if (e.Item == filteredDataItem)
                {
                    filteredDataList.ItemsSource = null;
                }
            }
        }
    }
}