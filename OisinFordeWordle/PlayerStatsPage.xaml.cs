using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;  // For Preferences

namespace OisinFordeWordle
{
    public partial class PlayerStatsPage : ContentPage
    {
        public PlayerStatsPage()
        {
            InitializeComponent();
            DisplayStats();
        }

        private void DisplayStats()
        {
            // Retrieve saved stats from Preferences
            int gamesPlayed = Preferences.Get("GamesPlayed", 0);
            int totalScore = Preferences.Get("TotalScore", 0);
            int highScore = Preferences.Get("HighScore", 0);

            // Calculate average score, avoid division by zero
            double averageScore = gamesPlayed > 0 ? (double)totalScore / gamesPlayed : 0;

            // Update the labels with the retrieved data
            HighScoreLabel.Text = $"?? High Score: {highScore}";
            GamesPlayedLabel.Text = $"?? Games Played: {gamesPlayed}";
            AverageScoreLabel.Text = $"?? Average Score: {averageScore:F2}";  // F2 limits to 2 decimal places
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to the MainPage
            await Navigation.PopAsync();
        }
    }
}
