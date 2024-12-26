# Wordle Game by Oisin Forde

## Game Overview
- A word-guessing game inspired by the popular Wordle.
- Players attempt to guess a **five-letter word** within **six tries**.
- Each guess provides feedback via color-coded hints:
  - **Green:** Correct letter in the correct position.
  - **Yellow:** Correct letter in the wrong position.
  - **Gray:** Letter is not in the word.

## Features
- **Dynamic Word List:**
  - Words are fetched from an online source and cached locally.
- **Player Stats:**
  - Tracks individual player's performance, including:
    - Games Played
    - Total Score
    - High Score
    - Average Score
- **Leaderboard:**
  - Displays the top players with the highest scores.
- **Cheat Mode:**
  - Option to reveal the correct word (disabled after the game ends).
- **Light & Dark Modes:**
  - Switch between light and dark themes manually or based on system settings.

## How to Play
1. **Log In:**
   - Enter a username to start tracking stats.
2. **Start a New Game:**
   - The game begins with a randomly selected five-letter word.
3. **Make Your Guesses:**
   - Enter a valid five-letter word.
   - Receive color-coded feedback to refine your guesses.
4. **Win or Lose:**
   - Win by guessing the correct word within six attempts.
   - Lose if you use all six attempts without guessing correctly.
5. **View Stats:**
   - Check your personal stats and see how you rank on the leaderboard.

## Scoring
- Points are awarded based on attempts remaining:
  - 6 attempts remaining: **60 points**
  - 5 attempts remaining: **50 points**
  - ...down to 1 attempt remaining: **10 points**
- No points are awarded for a loss.

## How It Works
- **Word Fetching:**
  - A list of valid words is fetched and saved locally.
- **Preferences:**
  - Player stats and preferences are stored locally using `Preferences`.
- **Leaderboard:**
  - Aggregates scores from all players and sorts them in descending order.

## Customization
- **Light/Dark Mode:**
  - Toggle manually using a button on the settings page.
  - Automatically adjusts based on the system theme.
- **Player-Specific Stats:**
  - Each player's stats are saved and loaded uniquely based on their username.
