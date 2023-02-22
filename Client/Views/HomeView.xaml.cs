using Winux.ViewModels;
using Windows.Gaming.Input.ForceFeedback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Winux.Views
{
    public sealed partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            DataContext = viewModel;
            Name = "HomeView";
        }

        private readonly HomeViewModel viewModel = new HomeViewModel();

        private void UserControl_Loading(FrameworkElement sender, object args)
        {
            UpdateStrings();
        }

        public void UpdateStrings()
        {

        }
    }
}
