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
using Windows.UI.Popups;
using Windows.UI.StartScreen;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace WinStoreTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        int appRunCount = 0;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private DataTransferManager dataTransferManager;

        public MainPage()
        {
            this.InitializeComponent();

            try
            {
                appRunCount = (int)localSettings.Values["AppRunCount"];
            }
            catch { }

            appRunCount++;

            try
            {
                localSettings.Values["AppRunCount"] = appRunCount;
            }
            catch { }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                // register this page as share source
                this.dataTransferManager = DataTransferManager.GetForCurrentView();
                this.dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            }
            catch { }

            if (SecondaryTile.Exists("MyUnicTileID"))
            {
                ToggleAppBarButton(false);
            }
            else
            {
                ToggleAppBarButton(true);
            }
        }

   protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            try
            {
                // unregister as share source
                this.dataTransferManager.DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
            }
            catch { }
        }

        private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            Uri dataPackageUri = new Uri("http://apps.microsoft.com/windows/ru-ru/app/mp3/0122ffd9-585f-4b3d-a2ad-571f74231d14");
            DataPackage requestData = e.Request.Data;
            requestData.Properties.Title = "Funny App";
            requestData.SetWebLink(dataPackageUri);
            requestData.Properties.Description = "You can add some description there. Usually recommendation to install app";
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

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (appRunCount == 5)
            {
                var messageDialog = new Windows.UI.Popups.MessageDialog("Please vote for this App", "Write a review!");
 
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Vote for app", new Windows.UI.Popups.UICommandInvokedHandler(CommandHandler)));
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("I have already voted", new Windows.UI.Popups.UICommandInvokedHandler(CommandHandler)));
                messageDialog.Commands.Add(new Windows.UI.Popups.UICommand("Next time", new Windows.UI.Popups.UICommandInvokedHandler(CommandHandler)));

                messageDialog.DefaultCommandIndex = 0;
                messageDialog.CancelCommandIndex = 2;
                await messageDialog.ShowAsync();
            }
        }

        private async void CommandHandler(IUICommand command)
        {
            if (command.Label== "Vote for app")
            {
                var uri = new Uri("ms-windows-store:PDP?PFN=9cc1e6dd-9d82-4736-aee5-acb9a01d9c39_jx7smx7qqfhe4");
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        public void ToggleAppBarButton(bool showPinButton)
        {
            if (showPinButton)
            {
                btnSecTile.Label = "Pin";
                btnSecTile.Icon = new SymbolIcon(Symbol.Pin);
            }
            else
            {
                btnSecTile.Label = "UnPin";
                btnSecTile.Icon = new SymbolIcon(Symbol.UnPin);
            }
            this.btnSecTile.UpdateLayout();
        }

        private async void btnSecTile_Click(object sender, RoutedEventArgs e)
        {

            Windows.Foundation.Rect rect = GetElementRect((FrameworkElement)sender);

            if (SecondaryTile.Exists("MyUnicTileID"))
            {
                SecondaryTile secondaryTile = new SecondaryTile("MyUnicTileID");

                bool isUnpinned = await secondaryTile.RequestDeleteForSelectionAsync(rect, Windows.UI.Popups.Placement.Above);
                ToggleAppBarButton(isUnpinned);
            }
            else
            {
                // Pin
                Uri square150x150Logo = new Uri("ms-appx:///Assets/Logo.scale-100.png");
                string tileActivationArguments = "Secondary tile was pinned at = " + DateTime.Now.ToLocalTime().ToString();
                string displayName = "App Template";

                TileSize newTileDesiredSize = TileSize.Square150x150;
                SecondaryTile secondaryTile = new SecondaryTile("MyUnicTileID",
                                                                displayName,
                                                                tileActivationArguments,
                                                                square150x150Logo,
                                                                newTileDesiredSize);

                secondaryTile.VisualElements.Square30x30Logo = new Uri("ms-appx:///Assets/SmallLogo.scale-100.png");
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                secondaryTile.VisualElements.ForegroundText = ForegroundText.Light;

                bool isPinned = await secondaryTile.RequestCreateForSelectionAsync(rect, Windows.UI.Popups.Placement.Above);
                ToggleAppBarButton(!isPinned);
            }

        }

        public static Rect GetElementRect(FrameworkElement element)
        {
            GeneralTransform buttonTransform = element.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            return new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
        }

    }
}
