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
using Windows.UI.Xaml.Navigation;

using Windows.ApplicationModel.DataTransfer;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinStoreAdvancedDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private DataTransferManager dataTransferManager;

        public MainPage()
        {
            this.InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                //регистрируем страницу как источник share
                this.dataTransferManager = DataTransferManager.GetForCurrentView();
                this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            }
            catch { }
        }

   protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                // удаляем регистрацию
                this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            }
            catch { }
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            Uri dataPackageUri = new Uri("http://apps.microsoft.com/windows/ru-ru/app/mp3/0122ffd9-585f-4b3d-a2ad-571f74231d14");
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = "Прикольное приложение";
            requestData.SetWebLink(dataPackageUri);
            requestData.Properties.Description = "И здесь добавить какое-то описание.Как правило, текст содержит рекомендацию установить приложение.";
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataTransferManager.ShowShareUI();
        }

        private async void ReviewButton_Click(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("ms-windows-store:PDP?PFN=9cc1e6dd-9d82-4736-aee5-acb9a01d9c39_jx7smx7qqfhe4");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
