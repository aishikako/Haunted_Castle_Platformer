using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using System.Collections;
using System.Diagnostics;
using static System.Formats.Asn1.AsnWriter;
using System.Drawing;
using System.Xml;

// FileManager handles parsing through the file and creating the entities in a level
internal class FileManager
{
    
    private static Font title = Engine.LoadFont("swirly-font.ttf", 45);
    private static Font heading1 = Engine.LoadFont("swirly-font.ttf", 50);

    // change position of HUD objects relative to other entities
    private const double HUDMultiply = 0.85;
    private static float HUDMultiplier = (float) HUDMultiply;

    private static string levelName;
    public string LevelName
    {
        get { return levelName; }
        set { levelName = value; }
    }
    private static string fileName;
    public static string FileName
    {
        get { return fileName; }
        set { fileName = value; }
    }
    private static string fileNameHUD = "hud.txt";
    private static string scoreboardFilePath = "player-stats.txt";

    public Dictionary<string, List<Entity>> LoadLevel()
    {
        // layers: background, midground, foreground, blocks, characters, particles, items, HUD, pop text
        var entityLayers = HelperMethods.EntitiesDict();

        // add level files
        LoadLevel(false, entityLayers);

        // add HUD
        LoadLevel(true, entityLayers);

        //add background layers
        entityLayers["background"].Add(new BackgroundEntity(1500, 1020, .4f, "background1"));
        entityLayers["midground"].Add(new BackgroundEntity(1500, 1020, .65f, "background2"));
        entityLayers["foreground"].Add(new BackgroundEntity(1500, 1020, .95f, "background3"));

        // entities List of each letter in file
        return entityLayers;
    }

    //parsing through file and creating entities with position
    //public List<Entity> LoadLevel()
    private Dictionary<string, List<Entity>> LoadLevel(bool HUD, Dictionary<string, List<Entity>> entityLayers)
    {
        string totalPath;

        if (!HUD)
        {
            totalPath = fileName;
        }

        else
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            totalPath = Path.Combine(basePath, "..", "..", "..", "Levels", fileNameHUD);
        }

        string[] lines = HelperMethods.getFileContents(totalPath);

        // Parse the title
        if (!HUD)
        {
            levelName = lines[0].Replace("title:", "").Trim();
        }

        // Parse the map
        int mapStartIndex = Array.FindIndex(lines, line => line.StartsWith("map:")) + 1;
        int rows = lines.Length;

        //iterating through text file and creating entities based on char
        for (int i = mapStartIndex; i < rows; i++)
        {
            int xPos = 0;
            foreach (char character in lines[i])
            {
                if (!character.Equals(' '))
                {
                    //convert and use pos as a vector instead of x, y - TODO
                    float currXPosition = xPos * Game.PixelsPerMeter;

                    if (!HUD)
                    {
                        float currYPosition = Game.Resolution.Y - (rows - i) * Game.PixelsPerMeter;
                        HelperMethods.AddEntitiesFromChar(entityLayers, currXPosition, currYPosition, character);
                    }
                    else
                    {
                        float currYPosition = Game.Resolution.Y - (rows + 10 - i) * Game.PixelsPerMeter;
                        HelperMethods.AddHUDEntitiesFromChar(entityLayers, currXPosition, currYPosition * HUDMultiplier, character);
                    }
                }
                xPos++;
            }
            GameScreen.maxPosX = xPos * Game.PixelsPerMeter;
        }
        return entityLayers;
    }

    // Reading HUD data
    public Dictionary<string, List<string>> ReadStatsFromFile()
    {
        // Construct the full file path to the player stats file.
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        string totalPath = Path.Combine(basePath, "..", "..", "..", "PlayerStats", scoreboardFilePath);

        string[] lines = HelperMethods.getFileContents(totalPath);
        Dictionary<string, List<string>> allPlayerStats = StatsDict(lines);

        return allPlayerStats;
    }

    public int FindPlayerNumber(Dictionary<string, List<string>> allPlayerStats)
    {
        int player = 0;
        // iterate through all lines, finding the greatest player
        for (int index = 0; index < allPlayerStats.Count; index++)
        {
            var item = allPlayerStats.ElementAt(index);
            var itemKey = item.Key;
            int currentPlayer = Int32.Parse(item.Key);
            if (currentPlayer > player)
            {
                player = Int32.Parse(itemKey);
            }
        }
        return (player + 50);
    }
    public int DisplayStats(Vector2 Resolution)
    {
        Dictionary<string, List<string>> sortedStats = SortStats(ReadStatsFromFile());
        //Dictionary<string, List<string>> sortedStats = StatsDict("test");

        //Engine.DrawString("Leaderboard", new Vector2(Resolution.X / 2, Resolution.Y / 8), Color.Navy, title);

        Engine.DrawString("Player", new Vector2(1.25f * Resolution.X / 4, Resolution.Y / 3), Color.AntiqueWhite, heading1, TextAlignment.Center);
        Engine.DrawString("Score", new Vector2(Resolution.X / 2, Resolution.Y / 3), Color.AntiqueWhite, heading1, TextAlignment.Center);
        Engine.DrawString("Gems", new Vector2(Resolution.X, Resolution.Y / 3), Color.AntiqueWhite, heading1, TextAlignment.Center);

        float posY = Resolution.Y / 2.4f;
        float interval = Resolution.Y / (2 * (sortedStats.Count + 1));
        for (int index = 0; index < sortedStats.Count; index++)
        {
            var item = sortedStats.ElementAt(index);
            posY += interval;
            // player number
            Engine.DrawString(item.Key, new Vector2(1.25f * Resolution.X / 4, posY), Color.AntiqueWhite, title);
            // score number
            Engine.DrawString(item.Value[0], new Vector2(Resolution.X / 2, posY), Color.AntiqueWhite, title);
            // coins number
            Engine.DrawString(item.Value[1], new Vector2(4 * Resolution.X / 4, posY), Color.AntiqueWhite, title);
        }
        return FindPlayerNumber(sortedStats);
    }

    public Dictionary<string, List<string>> SortStats(Dictionary<string, List<string>> allPlayerStats)
    {
        Dictionary<string, List<string>> sortedStats = new();
        // sort by score (1st value)
        while (allPlayerStats.Count > 0)
        {
            int largestVal = 0;
            int indexOfLargest = 0;
            for (int index = 0; index < allPlayerStats.Count; index++)
            {
                var item = allPlayerStats.ElementAt(index);
                var itemKey = item.Key;
                var itemValue = item.Value;
                if (Int32.Parse(itemValue[0]) > largestVal)
                {
                    indexOfLargest = index;
                    largestVal = Int32.Parse(itemValue[0]);
                }
            }
            sortedStats.Add(allPlayerStats.ElementAt(indexOfLargest).Key, allPlayerStats.ElementAt(indexOfLargest).Value);
            allPlayerStats.Remove(allPlayerStats.ElementAt(indexOfLargest).Key);
        }
        return sortedStats;
    }

    //creating initial dictionary for entities
    public static Dictionary<string, List<string>> StatsDict(string[] lines)
    {
        
        Dictionary<string, List<string>> stats = new();
        //if (new FileInfo(totalPath).Length == 0)
        //{
        //    return stats;
        //}

        // Format: 
        // Player=1,Score=1,Gems=2

        //string[] lines = { "1", "2", "3" };
        
            foreach (string line in lines)
            {
                string player = findKeyInfo(line);
                string score = findKeyInfo(line.Substring(8));
                string coins = findKeyInfo(line.Substring(16));

                if (!stats.ContainsKey(player))
                {
                    stats[player] = new List<string> { score, coins };
                }

            }
        
        
        return stats;
    }

    private static string findKeyInfo(string line)
    {
        if (line.Length > 0)
        {
            // find info after the = sign
            int position = line.IndexOf("=");
            int comma = line.IndexOf(",");
            // TO DO change later to accomodate for double digits
            return line.Substring(position + 1, 1);
        }
        return "";
    }
}