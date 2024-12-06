using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace OisinFordeWordle
{
    public partial class MainPage : ContentPage
    {
        private List<string> wordList;
        private string correctWord;
        private int currentAttempt;
        private int score = 0; // Variable to keep track of the score

        public MainPage()
        {
            InitializeComponent();
            wordList = new List<string>();
            currentAttempt = 0;
            LoadWords();
            LoadGameStateAsync(); // Load the saved state
        }

        private async void LoadWords()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "words.txt");
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                wordList = lines.Select(word => word.Trim().ToUpper()).ToList(); // Ensure words are trimmed and uppercase
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync("https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt");
                    wordList = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(word => word.Trim().ToUpper()).ToList(); // Ensure words are trimmed and uppercase
                    await File.WriteAllLinesAsync(filePath, wordList);
                }
            }
            if (wordList.Count == 0)
            {
                StartNewGame();
            }
        }

        private void StartNewGame()
        {
            Random random = new Random();
            correctWord = wordList[random.Next(wordList.Count)].ToUpper();
            currentAttempt = 0;
            score = 0; // Reset score
            GuessGrid.Children.Clear();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var label = new Label
                    {
                        BackgroundColor = Color.FromHex("#3C3D3F"),
                        TextColor = Colors.White,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = 24
                    };
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);
                    GuessGrid.Children.Add(label);
                }
            }
            FeedbackLabel.IsVisible = false; // Hide feedback label at the start of a new game
            GuessEntry.Text = string.Empty; // Clear the input field
            GuessEntry.IsVisible = true; // Show the Guess Entry for the new game
            SaveGameStateAsync(); // Save initial state
        }

        private async Task SaveGameStateAsync()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gameState.json");

            var gameState = new
            {
                correctWord,
                currentAttempt,
                guesses = GuessGrid.Children.OfType<Label>()
                            .GroupBy(l => Grid.GetRow(l))
                            .Select(g => string.Concat(g.OrderBy(l => Grid.GetColumn(l)).Select(l => l.Text)))
                            .Take(currentAttempt)
                            .ToList()
            };

            var json = JsonSerializer.Serialize(gameState);
            await File.WriteAllTextAsync(filePath, json);
        }

        private async Task LoadGameStateAsync()
        {
            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "gameState.json");
            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                var gameState = JsonSerializer.Deserialize<dynamic>(json);

                correctWord = gameState.correctWord;
                currentAttempt = (int)gameState.currentAttempt;

                // Restore guesses
                var guesses = gameState.guesses as List<string>;
                for (int i = 0; i < guesses.Count; i++)
                {
                    var guess = guesses[i];
                    for (int j = 0; j < guess.Length; j++)
                    {
                        var label = GuessGrid.Children.OfType<Label>()
                            .FirstOrDefault(l => Grid.GetRow(l) == i && Grid.GetColumn(l) == j);
                        if (label != null)
                        {
                            label.Text = guess[j].ToString();
                            // Restore tile colors if needed
                        }
                    }
                }
            }
        }

        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GameInfoPage());
        }

        private void OnNewGameButtonClicked(object sender, EventArgs e)
        {
            StartNewGame(); // Call the method to start a new game
        }
    }
}
