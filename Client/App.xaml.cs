using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Winux.Views;
using Winux.Views.Components;

namespace Winux
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (!(Window.Current.Content is Frame rootFrame))
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Önceki askıya alınmış uygulamadan durumu yükle
                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                Window.Current.Activate();
            }
            MainPage mainPage = rootFrame.Content as MainPage;
            titleBarView = mainPage.FindName("TitleBarView") as TitleBarView;
            MainContent = mainPage.FindName("MainContent") as Grid;
            LoginView = MainContent.FindName("LoginView") as LoginView;
            await Task.Run(async () => await Server.Hub.PublicHubConnection());
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        /* ADDED */

        public static ApplicationDataContainer LocalSettings = ApplicationData.Current.LocalSettings;

        public static MainPage MainPage;
        public static Grid MainContent;
        public static LoginView LoginView;

        public static TitleBarView titleBarView;

        internal static string ServerAddress = "http://localhost:5224";

        public static bool IsLogin = false;

        public static string GetString(string tag)
        {
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            return resourceLoader.GetString(tag);
        }
    }
}
