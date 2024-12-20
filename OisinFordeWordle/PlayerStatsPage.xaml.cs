using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace OisinFordeWordle
{
    public partial class PlayerStatsPage : ContentPage
    {
        private string currentUser;

        public PlayerStatsPage()
        {
            InitializeComponent();
            LoadUserStats();
        }

        private void LoadUserStats()
        {
            // Get the current user
            currentUser = Preferences.Get("Username", "Player");

            // Load user-specific stats
            int gamesPlayed = Preferences.Get($"{currentUser}_GamesPlayed", 0);
            int totalScore = Preferences.Get($"{currentUser}_TotalScore", 0);
            int highScore = Preferences.Get($"{currentUser}_HighScore", 0);

            // Display the stats on the labels
            GamesPlayedLabel.Text = $"?? Games Played: {gamesPlayed}";
            TotalScoreLabel.Text = $"?? Total Score: {totalScore}";
            HighScoreLabel.Text = $"?? High Score: {highScore}";
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
