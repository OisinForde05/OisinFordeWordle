using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace OisinFordeWordle
{
    public partial class MainPage : ContentPage
    {
        private List<string> _wordList;
        private string _correctWord;
        private int _currentAttempt;
        private int _score = 0; // Variable to keep track of the score

        public MainPage()
        {
            InitializeComponent();
            _wordList = new List<string>();
            _currentAttempt = 0;
            LoadWords();
        }

        private async void LoadWords()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "words.txt");
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                _wordList = lines.ToList();
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync("https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt");
                    _wordList = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    await File.WriteAllLinesAsync(filePath, _wordList);
                }
            }
            StartNewGame();
        }

        private void StartNewGame()
        {
            Random random = new Random();
            _correctWord = _wordList[random.Next(_wordList.Count)].ToUpper();
            _currentAttempt = 0;
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
        }

        private async void OnGuessButtonClicked(object sender, EventArgs e)
        {
            // Animate the button when clicked
            var button = sender as Button;
            if (button != null)
            {
                await button.ScaleTo(0.9, 100); // Scale down
                await button.ScaleTo(1, 100);   // Scale back to original size
            }

            string guess = GuessEntry.Text?.ToUpper();
            if (string.IsNullOrWhiteSpace(guess) || guess.Length != 5 || guess.Contains(" "))
            {
                FeedbackLabel.Text = "Please enter a valid 5-letter word without spaces.";
                FeedbackLabel.IsVisible = true;
                return;
            }

            if (_currentAttempt < 6)
            {
                // Display the guess in the grid
                for (int i = 0; i < 5; i++)
                {
                    var label = GuessGrid.Children
                        .OfType<Label>()
                        .FirstOrDefault(l => Grid.GetRow(l) == _currentAttempt && Grid.GetColumn(l) == i);
                    if (label != null)
                    {
                        label.Text = guess[i].ToString();
                        // Determine the color based on the correctness of the letter
                        if (guess[i] == _correctWord[i])
                        {
                            label.BackgroundColor = Color.FromHex("#00FF00"); // Green
                            _score += 2; // Add points for correct letter and position
                        }
                        else if (_correctWord.Contains(guess[i]))
                        {
                            label.BackgroundColor = Color.FromHex("#FFFF00"); // Yellow
                            _score += 1; // Add points for correct letter but wrong position
                        }
                        else
                        {
                            label.BackgroundColor = Color.FromHex("#808080"); // Gray
                        }

                        // Animate the label
                        await label.FadeTo(1, 200); // Fade in effect
                    }
                }

                // Check if the guess is correct
                if (guess == _correctWord)
                {
                    FeedbackLabel.Text = $"Congratulations! You've guessed the word! Your score: {_score}";
                    FeedbackLabel.IsVisible = true;
                    ResetGame(); // Reset the game after winning
                }
                else
                {
                    _currentAttempt++;
                    if (_currentAttempt >= 6)
                    {
                        FeedbackLabel.Text = $"Game Over! The correct word was: {_correctWord}. Your score: {_score}";
                        FeedbackLabel.IsVisible = true;
                        ResetGame(); // Reset the game after losing
                    }
                }

                // Clear the entry for the next guess
                GuessEntry.Text = string.Empty;
            }
        }

        private void ResetGame()
        {
            // Reset the game state
            _currentAttempt = 0;
            _score = 0; // Reset score
            FeedbackLabel.IsVisible = false; // Hide feedback label
            GuessEntry.Text = string.Empty; // Clear the entry

            // Clear the grid
            foreach (var label in GuessGrid.Children.OfType<Label>())
            {
                label.Text = string.Empty;
                label.BackgroundColor = Color.FromArgb("#00000000"); // Fully transparent
            }

            // Start a new game
            StartNewGame();
        }
    }
}