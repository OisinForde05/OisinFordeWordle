using Microsoft.Maui.Controls;

namespace OisinFordeWordle
{
    public partial class GameInfoPage : ContentPage
    {
        public GameInfoPage()
        {
            InitializeComponent();
        }

        // Event handler for the Back button click
        private void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to the previous page (for example, the main game page)
            Navigation.PopAsync(); // Assumes you're using a NavigationPage
        }
    }
}
