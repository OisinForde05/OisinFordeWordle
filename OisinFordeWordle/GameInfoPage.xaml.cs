using Microsoft.Maui.Controls;

namespace OisinFordeWordle
{
    public partial class GameInfoPage : ContentPage
    {
        public GameInfoPage()
        {
            InitializeComponent();
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Return to the MainPage
        }
    }
}
