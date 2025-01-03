﻿using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OisinFordeWordle
{
    public partial class MainPage : ContentPage
    {
        private List<string> wordList;
        private string correctWord;
        private int currentAttempt;
        private int score = 0; // Variable to keep track of the score

        // Declare and initialize the missing variables
        private int gamesPlayed;
        private int totalScore;
        private int highScore;

        public MainPage()
        {
            InitializeComponent();
            wordList = new List<string>();
            currentAttempt = 0;
            gamesPlayed = Preferences.Get("GamesPlayed", 0); // Load gamesPlayed from Preferences
            totalScore = Preferences.Get("TotalScore", 0);   // Load totalScore from Preferences
            highScore = Preferences.Get("HighScore", 0);     // Load highScore from Preferences
            LoadWords();

            // Retrieve the username from preferences and display it
            var username = Preferences.Get("Username", "Player");

            // Display welcome message
            WelcomeLabel.Text = $"Welcome, {username}!";
        }

        private async void LoadWords()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "words.txt");
            if (File.Exists(filePath))
            {
                var lines = await File.ReadAllLinesAsync(filePath);
                wordList = lines.Select(word => word.Trim().ToUpper()).ToList();
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync("https://raw.githubusercontent.com/DonH-ITS/jsonfiles/main/words.txt");
                    wordList = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(word => word.Trim().ToUpper()).ToList();
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
                        FontSize = 24,
                        WidthRequest = 50,
                        HeightRequest = 50
                    };
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);
                    GuessGrid.Children.Add(label);
                }
            }

            FeedbackLabel.IsVisible = false;
            GuessEntry.Text = string.Empty;
            GuessEntry.IsVisible = true;
            SubmitButton.IsVisible = true;
            NewGameButton.IsVisible = false;
            NewGameButton.Opacity = 0;

            // Initially hide cheat button at the start of a new game
            CheatButton.IsVisible = true; // Show the cheat button when a new game starts
        }

        private void OnCheatButtonClicked(object sender, EventArgs e)
        {
            FeedbackLabel.Text = $"The correct word is: {correctWord}";
            FeedbackLabel.IsVisible = true;
            CheatButton.IsVisible = false; // Hide the cheat button after showing the word
        }

        // Event handler for the submit guess button
        private async void OnGuessButtonClicked(object sender, EventArgs e)
        {
            string guess = GuessEntry.Text?.ToUpper();

            // Validate the guess
            if (string.IsNullOrWhiteSpace(guess) || guess.Length != 5 || !guess.All(char.IsLetter))
            {
                FeedbackLabel.Text = "Please enter a valid 5-letter word.";
                FeedbackLabel.IsVisible = true;
                return;
            }

            if (!wordList.Contains(guess))
            {
                FeedbackLabel.Text = "The word you entered is not in the valid word list.";
                FeedbackLabel.IsVisible = true;
                return;
            }

            if (currentAttempt < 6)
            {
                // Create an array to track letters used for green marking
                bool[] greenMatched = new bool[5];
                // Create a dictionary to count occurrences of each letter in the correct word
                Dictionary<char, int> letterCounts = correctWord.GroupBy(c => c)
                                                                .ToDictionary(g => g.Key, g => g.Count());

                // First pass: Check for correct letters in the correct position (green)
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
                            greenMatched[i] = true;
                            letterCounts[guess[i]]--; // Mark this letter as used
                        }
                    }
                }

                // Second pass: Check for correct letters in the wrong position (yellow)
                for (int i = 0; i < 5; i++)
                {
                    var label = GuessGrid.Children
                        .OfType<Label>()
                        .FirstOrDefault(l => Grid.GetRow(l) == currentAttempt && Grid.GetColumn(l) == i);

                    if (label != null && !greenMatched[i]) // Only process if not marked green
                    {
                        if (letterCounts.ContainsKey(guess[i]) && letterCounts[guess[i]] > 0)
                        {
                            label.BackgroundColor = Color.FromHex("#FFFF00"); // Yellow
                            letterCounts[guess[i]]--; // Mark this letter as used
                        }
                        else
                        {
                            label.BackgroundColor = Color.FromHex("#3C3D3F"); // Gray
                        }
                    }
                }

                // Increment the attempt counter
                currentAttempt++;

                // Check if the game is over
                if (guess == correctWord)
                {
                    FeedbackLabel.Text = $"Congratulations! You've guessed the word: {correctWord}.";
                    FeedbackLabel.IsVisible = true;

                    SubmitButton.IsVisible = false;
                    GuessEntry.IsVisible = false;
                    await NewGameButton.FadeTo(1, 500);
                    NewGameButton.IsVisible = true;

                    // Save the score
                    SaveGameStats(true);

                    // Hide the cheat button after the game is over (win)
                    CheatButton.IsVisible = false;
                }
                else if (currentAttempt >= 6)
                {
                    FeedbackLabel.Text = $"Game Over! The correct word was {correctWord}.";
                    FeedbackLabel.IsVisible = true;

                    SubmitButton.IsVisible = false;
                    GuessEntry.IsVisible = false;
                    await NewGameButton.FadeTo(1, 500);
                    NewGameButton.IsVisible = true;

                    // Save the score as game over
                    SaveGameStats(false);

                    // Hide the cheat button after the game is over (loss)
                    CheatButton.IsVisible = false;
                }
                else
                {
                    // Show the cheat button after the first guess
                    if (currentAttempt == 1)
                    {
                        CheatButton.IsVisible = true;
                    }
                }
            }

            // Clear the entry field for the next guess
            GuessEntry.Text = string.Empty;
        }

        private void SaveGameStats(bool gameWon)
        {
            string username = Preferences.Get("Username", "Player");

            // Get the current score for the game (example: based on attempts remaining)
            int scoreForThisGame = gameWon ? (6 - currentAttempt) * 10 : 0;

            // Load existing stats
            int gamesPlayed = Preferences.Get($"{username}_GamesPlayed", 0);
            int totalScore = Preferences.Get($"{username}_TotalScore", 0);
            int highScore = Preferences.Get($"{username}_HighScore", 0);

            // Update stats
            gamesPlayed++;
            totalScore += scoreForThisGame;

            if (scoreForThisGame > highScore)
            {
                highScore = scoreForThisGame;
            }

            // Save updated stats
            Preferences.Set($"{username}_GamesPlayed", gamesPlayed);
            Preferences.Set($"{username}_TotalScore", totalScore);
            Preferences.Set($"{username}_HighScore", highScore);
        }


        private void OnNewGameButtonClicked(object sender, EventArgs e)
        {
            StartNewGame();
        }

        private async void OnInfoButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GameInfoPage());
        }

        private async void OnPlayerStatsButtonClicked(object sender, EventArgs e)
        {
            // Check if the page is already on the navigation stack
            var playerStatsPage = new PlayerStatsPage();
            if (!Navigation.NavigationStack.Contains(playerStatsPage))
            {
                await Navigation.PushAsync(playerStatsPage);
            }
        }


    }
}
