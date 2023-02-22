using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Winux.ViewModels.Components;
using Windows.Globalization;
using System;
using Windows.ApplicationModel;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Winux.Views.Components
{
    public sealed partial class TitleBarView : UserControl
    {
        public TitleBarView()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        internal readonly TitleBarViewModel viewModel = new TitleBarViewModel();
        private readonly CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;

        private void UserControl_Loading(FrameworkElement sender, object args)
        {
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            var appTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            appTitleBar.ButtonBackgroundColor = Colors.Transparent;
            appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(CoreTitleBar);

            if (ApplicationLanguages.PrimaryLanguageOverride == "")
            {
                ApplicationLanguages.PrimaryLanguageOverride = "en";
            }

            var language = ApplicationLanguages.PrimaryLanguageOverride;
            ChangeLanguage(language);
            ToggleMenuFlyoutItem toggleMenuFlyoutItem = FindName(language) as ToggleMenuFlyoutItem;
            toggleMenuFlyoutItem.IsChecked = true;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
        }

        private void UpdateStrings()
        {
            viewModel.Settings = App.GetString("Settings");
            viewModel.Language = App.GetString("Language");
            viewModel.English = App.GetString("English");
            viewModel.Turkish = App.GetString("Turkish");
            viewModel.About = App.GetString("About");
            viewModel.Copyright = App.GetString("Copyright");
            viewModel.Close = App.GetString("Close");
            viewModel.Logout = App.GetString("Logout");
        }

        public async static void ChangeLanguage(string language = "en")
        {
            ApplicationLanguages.PrimaryLanguageOverride = language;
            await Task.Delay(1);

            var rootFrame = Window.Current.Content as Frame;
            var mainPage = rootFrame.Content as Page;

            if (mainPage.FindName("TitleBarView") is TitleBarView titleBarView)
                titleBarView.UpdateStrings();

            if (mainPage.FindName("LoginView") is LoginView login)
                login.UpdateStrings();

            if (mainPage.FindName("RegisterView") is RegisterView register)
                register.UpdateStrings();

            if (mainPage.FindName("HomeView") is HomeView home)
                home.UpdateStrings();

            Debug.WriteLine("Changed Language: " + ApplicationLanguages.PrimaryLanguageOverride);
        }

        private void Language_Click(object sender, RoutedEventArgs e)
        {
            ToggleMenuFlyoutItem thisButton = sender as ToggleMenuFlyoutItem;

            foreach (ToggleMenuFlyoutItem toggleMenuFlyoutItem in Languages.Items)
            {
                if (toggleMenuFlyoutItem != thisButton)
                {
                    toggleMenuFlyoutItem.IsChecked = false;
                }
                else
                {
                    toggleMenuFlyoutItem.IsChecked = true;
                }
            }

            ChangeLanguage(thisButton.Name);
        }

        private async void About_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog about = new ContentDialog
            {
                Title = viewModel.About,
                Content = string.Format(viewModel.Copyright, DateTime.Now.Year, AppInfo.Current.DisplayInfo.DisplayName),
                PrimaryButtonText = viewModel.Close
            };

            await about.ShowAsync();
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            HomeView home = App.MainContent.FindName("HomeView") as HomeView;
            if (home != null)
            {
                await Server.Hub.PrivateHub.StopAsync();
                App.LocalSettings.Values["Username"] = null;
                App.LocalSettings.Values["Password"] = null;
                App.LocalSettings.Values["RememberMe"] = false;
                App.MainContent.Children.Remove(home);
                viewModel.AuthVisibility = Visibility.Collapsed;
                LoginView login = new LoginView();
                App.MainContent.Children.Add(login);
            }
        }
    }
}