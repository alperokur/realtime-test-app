using Winux.Models;
using Winux.ViewModels;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Winux;
using Winux.Server;
using Hub = Winux.Server.Hub;

namespace Winux.Views
{
    public sealed partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
            DataContext = viewModel;
            Name = "LoginView";
        }

        public readonly LoginViewModel viewModel = new LoginViewModel();

        private void UserControl_Loading(FrameworkElement sender, object args)
        {
            UpdateStrings();
        }

        public void UpdateStrings()
        {
            viewModel.Username = App.GetString("Username");
            viewModel.Password = App.GetString("Password");
            viewModel.RememberMe = App.GetString("RememberMe");
            viewModel.Login = App.GetString("Login");
            viewModel.HaventAccount = App.GetString("HaventAccount");
        }

        public async Task<bool> LoginUser(string username, string password)
        {
            if (await RestApi.Login(username, password))
            {
                await Hub.PrivateHubConnection();
                if (RememberMe.IsChecked == true || (bool)App.LocalSettings.Values["RememberMe"] == true)
                {
                    App.LocalSettings.Values["Username"] = username;
                    App.LocalSettings.Values["Password"] = password;
                    App.LocalSettings.Values["RememberMe"] = true;
                }
                else
                {
                    App.LocalSettings.Values["Username"] = null;
                    App.LocalSettings.Values["Password"] = null;
                    App.LocalSettings.Values["RememberMe"] = false;
                }
                App.MainContent.Children.Remove(this);

                if (App.MainContent.FindName("RegisterView") is RegisterView register)
                {
                    App.MainContent.Children.Remove(register);
                }

                HomeView home = new HomeView();
                App.MainContent.Children.Add(home);
                App.titleBarView.viewModel.AuthVisibility = Visibility.Visible;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void WrongUsernameOrPassword()
        {
            InfoMessage("WrongUsernameOrPassword");
            Username.BorderBrush = (SolidColorBrush)Application.Current.Resources["DangerColor"];
            Password.BorderBrush = (SolidColorBrush)Application.Current.Resources["DangerColor"];
        }

        public void InfoMessage(string message)
        {
            viewModel.Info = App.GetString(message);
            Info.Visibility = Visibility.Visible;
            Info.Foreground = (SolidColorBrush)Application.Current.Resources["DangerColor"];
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            await LoginUser(Username.Text, Password.Password);
        }

        private void HaventAccount_Click(object sender, RoutedEventArgs e)
        {
            if (App.MainContent != null)
            {
                
                Visibility = Visibility.Collapsed;
                RegisterView register = App.MainContent.FindName("RegisterView") as RegisterView;
                if (register == null)
                {
                    register = new RegisterView();
                    App.MainContent.Children.Add(register);
                }
                else
                {
                    register.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
