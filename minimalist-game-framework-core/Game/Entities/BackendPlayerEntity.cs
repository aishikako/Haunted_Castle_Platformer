using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using System.IO.Enumeration;
using System.Collections;
using System.Diagnostics;

internal class BackendPlayerEntity
{
    // Stats
    public int Score { get; set; }
    public int Gems { get; set; }
    public int Level { get; set; }
    public int PlayerNumber { get; set; }
    public int Health { get; set; }
    public bool Armor { get; set; }
    public static int Stars { get; set; }
    public float Time { get; set; }
    public float puzzleTime;
    public bool isJumpBoost { get; set; }
    public bool isSpeedBoost { get; set; }

    public string fileName = "player-stats.txt";
    FileManager fileManager = new FileManager();

    // Other variables
    //public bool collided { get; set; }
    public BackendPlayerEntity(float xPos, float yPos, int level)
    {
        Score = 0;
        Gems = 0;
        Time = 0;
        Stars = 0;
        PlayerNumber = 1;
        Armor = false;
        Health = 2;
        Level = level;
        isJumpBoost = false;
        isSpeedBoost = false;
    }

    public void retrieveGem()
    {
        Gems++;
    }

    public void retrieveArmor()
    {
        Armor = true;
    }

    public void InGameDisplayStats()
    {

    }

    public void EndScreenDisplayStats()
    {

    }

    public void ResetStats()
    {
        Score = 0;
        Gems = 0;
        Stars = 0;
        Armor = false;
        Health = 2;
    }

    // Reading from + writing to file
    public void SaveStatsToFile()
    {
        // Construct the full file path to the player stats file.
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        string totalPath = Path.Combine(basePath, "..", "..", "..", "PlayerStats", fileName);
        if (!File.Exists(totalPath))
        {
            Debug.WriteLine($"Error: File '{fileName}' not found.");
            throw new FileNotFoundException();
        }
    }

    //creating initial dictionary for entities
    public static Dictionary<string, List<string>> StatsDict(string totalPath)
    {
        Dictionary<string, List<string>> stats = new();
        // Format: 
        // Player=1,Score=1,Gems=2
        string[] lines = File.ReadAllLines(totalPath);
        foreach (string line in lines)
        {
            string player = findKeyInfo(line);
            string score = findKeyInfo(line.Substring(8));
            string coins = findKeyInfo(line.Substring(16));
            stats.Add(player, new List<string> { score, coins });
        }
        return stats;
    }

    public void WriteToFile(string totalPath)
    {
        File.AppendAllText(totalPath, Environment.NewLine + $"Player={PlayerNumber},Score={Score},Gems={Gems}");
    }

    private static string findKeyInfo(string line)
    {
        // find info after the = sign
        int position = line.IndexOf("=");
        int comma = line.IndexOf(",");
        return line.Substring(position + 1, 1);
    }
}