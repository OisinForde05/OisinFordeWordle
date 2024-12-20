using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System.Collections.Generic;

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

            // Add username to the list of usernames if not already added
            var userList = GetUserList();
            if (!userList.Contains(username))
            {
                userList.Add(username);
                Preferences.Set("UserList", string.Join(",", userList));
            }

            await Navigation.PushAsync(new MainPage());
        }

        private List<string> GetUserList()
        {
            string users = Preferences.Get("UserList", "");
            return new List<string>(users.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
