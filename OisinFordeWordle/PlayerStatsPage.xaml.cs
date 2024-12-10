using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage; // For Preferences

namespace OisinFordeWordle
{
    public partial class PlayerStatsPage : ContentPage
    {
        // Private fields to store stats
        private int highScore;
        private int gamesPlayed;
        private int totalScore;

        public PlayerStatsPage()
        {
            InitializeComponent();
            LoadPlayerStats();
            UpdatePlayerStats();
        }

        protected override void OnDisappearing()
        {
            // Optionally, save stats when page is about to disappear
            SavePlayerStats();
            base.OnDisappearing();
        }

        private void LoadPlayerStats()
        {
            // Load stats from preferences, or default if not available
            highScore = Preferences.Get("HighScore", 0);
            gamesPlayed = Preferences.Get("GamesPlayed", 0);
            totalScore = Preferences.Get("TotalScore", 0);
        }

        private void SavePlayerStats()
        {
            // Save stats back to preferences
            Preferences.Set("HighScore", highScore);
            Preferences.Set("GamesPlayed", gamesPlayed);
            Preferences.Set("TotalScore", totalScore);
        }

        private void UpdatePlayerStats()
        {
            // Calculate average score
            int averageScore = gamesPlayed > 0 ? totalScore / gamesPlayed : 0;

            // Update the labels with current stats
            HighScoreLabel.Text = $"?? High Score: {highScore}";
            GamesPlayedLabel.Text = $"?? Games Played: {gamesPlayed}";
            AverageScoreLabel.Text = $"?? Average Score: {averageScore}";
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            // Navigate back to MainPage
            await Navigation.PopAsync();
        }
    }
}

