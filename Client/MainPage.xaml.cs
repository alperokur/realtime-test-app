using System.Net.Http;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.NetworkInformation;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Winux.ViewModels;
using Winux.Views;

namespace Winux
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        public readonly MainPageViewModel viewModel = new MainPageViewModel();

        private async void Page_Loading(FrameworkElement sender, object args)
        {
            LoginView login = new LoginView();
            if (App.LocalSettings.Values["RememberMe"] != null && (bool)App.LocalSettings.Values["RememberMe"] == true)
            {
                bool IsLogin = await login.LoginUser((string)App.LocalSettings.Values["Username"], (string)App.LocalSettings.Values["Password"]);
                if (IsLogin)
                {
                    HomeView home = new HomeView();
                    MainContent.Children.Add(home);
                }
                else
                {
                    MainContent.Children.Add(login);
                }
            }
            else
            {
                MainContent.Children.Add(login);
            }
        }
    }
}
