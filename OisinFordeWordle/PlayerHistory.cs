using System;
using System.Collections.Generic;

public class PlayerHistory
{
    public DateTime Timestamp { get; set; }
    public string CorrectWord { get; set; }
    public int GuessesTaken { get; set; }
    public string EmojiGrid { get; set; }  // Represents the emoji grid for the history page
}
