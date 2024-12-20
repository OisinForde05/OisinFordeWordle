using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;
using System.Linq;

namespace OisinFordeWordle
{
    public partial class PlayerStatsPage : ContentPage
    {
        public PlayerStatsPage()
        {
            InitializeComponent();
            DisplayPlayerStats();
            UpdateLeaderboard();
        }

        private void DisplayPlayerStats()
        {
            string username = Preferences.Get("Username", "Guest");
            int gamesPlayed = Preferences.Get($"{username}_GamesPlayed", 0);
            int totalScore = Preferences.Get($"{username}_TotalScore", 0);
            int highScore = Preferences.Get($"{username}_HighScore", 0);

            HighScoreLabel.Text = $"?? High Score: {highScore}";
            GamesPlayedLabel.Text = $"?? Games Played: {gamesPlayed}";
            AverageScoreLabel.Text = $"?? Average Score: {(gamesPlayed > 0 ? totalScore / gamesPlayed : 0)}";
        }

        private void UpdateLeaderboard()
        {
            // Retrieve the list of usernames
            var userList = GetUserList();
            var userScores = new List<(string Username, int TotalScore)>();

            foreach (var username in userList)
            {
                int totalScore = Preferences.Get($"{username}_TotalScore", 0);
                userScores.Add((username, totalScore));
            }

            // Sort by TotalScore in descending order
            var sortedScores = userScores.OrderByDescending(u => u.TotalScore).ToList();

            // Bind the sorted scores to the ListView
            LeaderboardListView.ItemsSource = sortedScores;
            LeaderboardListView.ItemTemplate = new DataTemplate(() =>
            {
                var textCell = new TextCell();
                textCell.SetBinding(TextCell.TextProperty, "Username");
                textCell.SetBinding(TextCell.DetailProperty, "TotalScore");
                return textCell;
            });
        }

        private List<string> GetUserList()
        {
            string users = Preferences.Get("UserList", "");
            return new List<string>(users.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
