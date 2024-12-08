using Microsoft.Maui.Controls;

namespace OisinFordeWordle
{
    public partial class PlayerStatsPage : ContentPage
    {
        private int highScore = 250; // Example value
        private int gamesPlayed = 12; // Example value
        private int averageScore = 125; // Example value

        public PlayerStatsPage()
        {
            InitializeComponent();
            UpdatePlayerStats();
        }

        protected override void OnDisappearing()
        {
            // Clean up event handlers or any resources here if necessary
            base.OnDisappearing();
        }

        private void UpdatePlayerStats()
        {
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
