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
            StartNewGame();
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
            if (string.IsNullOrWhiteSpace(guess) || guess.Length != 5 || !guess.All(char.IsLetter))
            {
                FeedbackLabel.Text = "Please enter a valid 5-letter word without spaces.";
                FeedbackLabel.IsVisible = true;
                return;
            }

            // Check if the guessed word is in the valid word list
            if (!wordList.Contains(guess))
            {
                FeedbackLabel.Text = "The word you entered is not in the valid word list.";
                FeedbackLabel.IsVisible = true;
                return;
            }

            if (currentAttempt < 6)
            {
                // Create a list to track which letters have been matched correctly
                bool[] matchedCorrectly = new bool[5];

                // First pass: Check for correct letters in the correct position
                for (int i = 0; i < 5; i++)
                {
                    var label = GuessGrid.Children
                        .OfType<Label>()
                        .FirstOrDefault(l => Grid.GetRow(l) == currentAttempt && Grid.GetColumn(l) == i);
                    if (label != null)
                    {
                        label.Text = guess[i].ToString();
                        if (guess[i] == correctWord[i])
                        {
                            label.BackgroundColor = Color.FromHex("#00FF00"); // Green
                            score += 2; // Add points for correct letter and position
                            matchedCorrectly[i] = true; // Mark this position as matched
                        }
                    }
                }

                // Second pass: Check for correct letters in the wrong position
                for (int i = 0; i < 5; i++)
                {
                    var label = GuessGrid.Children
                        .OfType<Label>()
                        .FirstOrDefault(l => Grid.GetRow(l) == currentAttempt && Grid.GetColumn(l) == i);
                    if (label != null && !matchedCorrectly[i])
                    {
                        if (correctWord.Contains(guess[i]))
                        {
                            label.BackgroundColor = Color.FromHex("#FFFF00"); // Yellow
                            score += 1; // Add points for correct letter but wrong position
                        }
                        else
                        {
                            label.BackgroundColor = Color.FromHex("#3C3D3F"); // Gray
                        }
                    }
                }

                currentAttempt++;

                // Check if the game is over
                if (guess == correctWord)
                {
                    FeedbackLabel.Text = $"Congratulations! You've guessed the word: {correctWord}. Your score is {score}.";
                    FeedbackLabel.IsVisible = true;

                    // Hide the Submit button and show the New Game button
                    SubmitButton.IsVisible = false;
                    GuessEntry.IsVisible = false; // Hide the Guess Entry
                    await NewGameButton.FadeTo(1, 500); // Fade in the New Game button
                    NewGameButton.IsVisible = true; // Make it visible
                }
                else if (currentAttempt >= 6)
                {
                    FeedbackLabel.Text = $"Game Over! The correct word was {correctWord}. Your score is {score}.";
                    FeedbackLabel.IsVisible = true;

                    // Hide the Submit button and show the New Game button
                    SubmitButton.IsVisible = false;
                    GuessEntry.IsVisible = false; // Hide the Guess Entry
                    await NewGameButton.FadeTo(1, 500); // Fade in the New Game button
                    NewGameButton.IsVisible = true; // Make it visible
                }
            }

            GuessEntry.Text = string.Empty; // Clear the input field for the next guess
        }

        private void OnNewGameButtonClicked(object sender, EventArgs e)
        {
            StartNewGame(); // Call the method to start a new game
        }
    }
}