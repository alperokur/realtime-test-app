using Winux.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Winux.Views;
using Winux;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Winux.Server;

namespace Winux.Views
{
    public sealed partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent();
            DataContext = viewModel;
            Name = "RegisterView";
        }

        private readonly RegisterViewModel viewModel = new RegisterViewModel();
        private static LoginView loginView = App.MainContent.FindName("LoginView") as LoginView;

        private void UserControl_Loading(FrameworkElement sender, object args)
        {
            UpdateStrings();
        }

        public void UpdateStrings()
        {
            viewModel.Username = App.GetString("Username");
            viewModel.Password = App.GetString("Password");
            viewModel.FirstName = App.GetString("FirstName");
            viewModel.LastName = App.GetString("LastName");
            viewModel.Email = App.GetString("Email");
            viewModel.Phone = App.GetString("Mobile");
            viewModel.Register = App.GetString("Register");
            viewModel.HaveAccount = App.GetString("HaveAccount");
        }

        public async Task<bool> RegisterUser(string username, string password, string firstName, string lastName, string email, string phone)
        {
            bool IsRegister = await RestApi.Register(username, password, firstName, lastName, email, phone);

            if (IsRegister)
            {
                App.MainContent.Children.Remove(this);

                if (App.MainContent.FindName("LoginView") is LoginView login)
                {
                    login.Visibility = Visibility.Visible;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private async void Register_Click(object sender, RoutedEventArgs e)
        {
            await RegisterUser(Username.Text, Password.Password, FirstName.Text, LastName.Text, Email.Text, Phone.Text);
        }

        private void HaveAccount_Click(object sender, RoutedEventArgs e)
        {
            if (App.MainContent != null)
            {
                Visibility = Visibility.Collapsed;
                loginView.Visibility = Visibility.Visible;
            }
        }
    }
}
