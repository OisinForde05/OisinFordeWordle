using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace OisinFordeWordle
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameEntry.Text))
            {
                await DisplayAlert("Error", "Please enter a valid username.", "OK");
                return;
            }

            string username = UsernameEntry.Text;
            Preferences.Set("Username", username);

            // Initialize stats if they don't already exist
            if (!Preferences.ContainsKey($"{username}_GamesPlayed"))
            {
                Preferences.Set($"{username}_GamesPlayed", 0);
                Preferences.Set($"{username}_TotalScore", 0);
                Preferences.Set($"{username}_HighScore", 0);
            }

            await Navigation.PushAsync(new MainPage());
        }

    }
}
