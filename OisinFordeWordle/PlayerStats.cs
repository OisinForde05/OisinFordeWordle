using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OisinFordeWordle
{
    
        public class PlayerStats
        {
            public int HighScore { get; set; }
            public int GamesPlayed { get; set; }
            public int AverageScore { get; set; }
        }

    public class PlayerStatsService
    {
        // Manual Serialization: Convert PlayerStats to a JSON-like string
        public string SerializePlayerStats(PlayerStats playerStats)
        {
            return $"{{ \"HighScore\": {playerStats.HighScore}, \"GamesPlayed\": {playerStats.GamesPlayed}, \"AverageScore\": {playerStats.AverageScore} }}";
        }

        // Manual Deserialization: Convert JSON-like string to PlayerStats
        public PlayerStats DeserializePlayerStats(string serializedData)
        {
            serializedData = serializedData.Trim(new char[] { '{', '}' }).Trim();
            var keyValuePairs = serializedData.Split(',');

            PlayerStats playerStats = new PlayerStats();

            foreach (var pair in keyValuePairs)
            {
                var keyValue = pair.Split(':');
                var key = keyValue[0].Trim().Trim('"');
                var value = keyValue[1].Trim();

                if (key == "HighScore")
                {
                    playerStats.HighScore = int.Parse(value);
                }
                else if (key == "GamesPlayed")
                {
                    playerStats.GamesPlayed = int.Parse(value);
                }
                else if (key == "AverageScore")
                {
                    playerStats.AverageScore = int.Parse(value);
                }
            }

            return playerStats;
        }

        // Save PlayerStats to file
        public void SavePlayerStats(PlayerStats playerStats)
        {
            string serializedData = SerializePlayerStats(playerStats);
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PlayerStats.txt");
            File.WriteAllText(filePath, serializedData);
        }

        // Load PlayerStats from file
        public PlayerStats LoadPlayerStats()
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PlayerStats.txt");

            if (File.Exists(filePath))
            {
                string serializedData = File.ReadAllText(filePath);
                return DeserializePlayerStats(serializedData);
            }

            // If file doesn't exist, return default stats
            return new PlayerStats { HighScore = 0, GamesPlayed = 0, AverageScore = 0 };
        }
    }


}
