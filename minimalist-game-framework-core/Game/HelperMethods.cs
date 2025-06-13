using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

internal static class HelperMethods
{
  //rendering scale factor
  public static float renderScaleFactor = 2;

    public static string[] getFileContents(string path)
    {

        if (!File.Exists(path))
        {
            Debug.WriteLine($"Error: Level file '{path}' not found.");
            return null;
        }

        return File.ReadAllLines(path);
    }

    //creating initial dictionary for entities
    public static Dictionary<string, List<Entity>> EntitiesDict()
    {
        return new Dictionary<string, List<Entity>>
        {
            { "background", new List<Entity>() },
            { "midground", new List<Entity>() },
            { "foreground", new List<Entity>() },
            { "blocks", new List<Entity>() },
            { "characters", new List<Entity>() },
            { "items", new List<Entity>() },
            // HUD has entities that don't change while poptext 
            //  can change (e.g. numerical score) but is within HUD boxes
            { "instructions", new List<Entity>() },
            { "HUD", new List<Entity>() },
            { "poptext", new List<Entity>() }
        };
    }

    public static void AddHUDEntitiesFromChar(Dictionary<string, List<Entity>> entityLayers, float currXPosition, float currYPosition, char character)
    {
        switch (character)
        {
            case 'S':
                entityLayers["HUD"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Score", "purple"));
                break;
            case 'C':
                entityLayers["HUD"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Coins", "purple"));
                break;
            //case 'H':
            //    entityLayers["HUD"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Health", "black"));
            //    break;
            case 'L':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Level", "purple"));
                break;
            case 'T':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Time", "purple"));
                break;
            case 'A':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Armor", "purple"));
                break;
            case 's':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Stars", "purple"));
                break;
            case 'B': // Boss Health Bar
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Boss Health", "crimson"));
                break;
            case 'o':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "Health", "crimson"));
                break;
            case 'n':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "coins", "crimson"));
                break;
            case 'e':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "level", "crimson"));
                break;
            case 't':
                entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "time", "crimson"));
                break;
                //case 'h':
                //    entityLayers["poptext"].Add(new HUDEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "health", "black"));
                //    break;
        }
    }
  
    public static void AddEntitiesFromChar(Dictionary<string, List<Entity>> entityLayers, float currXPosition, float currYPosition, char character)
    {
        switch (character)
        {
            //player
            case 'x':
                GameScreen.player = new PlayerEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "player", 10, 0.5f);
                entityLayers["characters"].Add(GameScreen.player);
                break;

            //boss
            case 'z':
                entityLayers["characters"].Add(new BossEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "boss", new Vector2(3, 3), new Bounds2(new Vector2(currXPosition, currYPosition), new Vector2(Game.PixelsPerMeter * 5, 0))));
                break;
            //spike
            case 'm':
                entityLayers["blocks"].Add(new MovingEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "spikes", new Vector2(2, 2), new Bounds2(new Vector2(currXPosition, currYPosition), new Vector2(Game.PixelsPerMeter * 5, 0)), true, duration: 2, type: "spikes"));
                break;
            //vertical
            case 'n':
                entityLayers["blocks"].Add(new MovingEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "movingplatform", new Vector2(2, -2), new Bounds2(new Vector2(currXPosition, currYPosition), new Vector2(0, Game.PixelsPerMeter * 5)), true, duration: 2));
                break;
            //horizontal
            case 'h':
                entityLayers["blocks"].Add(new MovingEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "movingplatform", new Vector2(2, -2), new Bounds2(new Vector2(currXPosition, currYPosition), new Vector2(Game.PixelsPerMeter * 5,0)), true, duration: 2));
                break;
            //diagonal
            case 'b':
                entityLayers["blocks"].Add(new MovingEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "movingplatform", new Vector2(3, -3), new Bounds2(new Vector2(currXPosition, currYPosition), new Vector2(Game.PixelsPerMeter * 5, Game.PixelsPerMeter * 5)), true, duration: 2));
                break;
            //spider
            case 's':
                Bounds2 spiderBounds = new Bounds2(new Vector2(0, currYPosition), new Vector2(currXPosition, currYPosition));
                entityLayers["characters"].Add(new SpiderEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "lightgray", new Vector2(40, 3), spiderBounds));
                break;
            //ghost
            case 'o':
                entityLayers["characters"].Add(new GhostEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "white", new Vector2(40, 3), null));
                break;
            case '0': //invis box
                leverButtonEntity collisionBox = new leverButtonEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "transparent", character - '0', "collisionbox");
                collisionBox.addLinkedEntity(entityLayers);
                entityLayers["items"].Add(collisionBox);
                break;
            case '2':  //button (linked to regular platform)
                leverButtonEntity button = new leverButtonEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, ConvertCharToTexture('b'), character - '0', "button");
                button.addLinkedEntity(entityLayers);
                entityLayers["items"].Add(button);
                break;
            case '4': //lever
            case '6':
            case '8':
                leverButtonEntity lever = new leverButtonEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, ConvertCharToTexture('l'), character - '0', "lever");
                lever.addLinkedEntity(entityLayers);
                entityLayers["items"].Add(lever);
                break;
            //linked moving platforms (odd)
            case '1':
                PhysicsEntity chandelier = new PhysicsEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "chandelier", new Vector2(2, -2), character - '0');
                chandelier.addLinkedEntity(entityLayers);
                entityLayers["blocks"].Add(chandelier);
                break;
            case '3':
            case '5':
            case '7':
            case '9':
                MovingEntity platform = new MovingEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, "platform", new Vector2(2, -1.5f), new Bounds2(new Vector2(currXPosition, currYPosition), new Vector2(0, Game.PixelsPerMeter * 5)), true, character - '0');
                platform.addLinkedEntity(entityLayers);
                entityLayers["blocks"].Add(platform);
                break;
            case 'g': //gem
            case 'd': //door
            case 'u': //p(u)zzle door
            case 'j': //jump pickup
            case 'a': //speed pickup
            case 'y': //armor
                entityLayers["items"].Add(new CollectibleEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, ConvertCharToTexture(character), ConvertCharToType(character)));
                break;
            case '!': // in game instruction

                // add in during instruction implementation
                //entityLayers["instructions"].Add(new BackgroundEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, ConvertCharToTexture(character), ConvertCharToType(character)));
                break;
            default:
                entityLayers["blocks"].Add(new CollidableEntity(currXPosition, currYPosition, Game.PixelsPerMeter, Game.PixelsPerMeter, ConvertCharToTexture(character), ConvertCharToType(character)));
                break;
        }
    }

    // returns the color of a given type
    // m,n,h are moving platforms
    // z boss, x player
    public static string ConvertCharToTexture(char type)
    {
        return type switch
        {
            'p' => "platform",
            'l' => "leverleft",
            'f' => "fire",
            'o' => "ghost",
            's' => "spider_1",
            'w' => "wall",
            'W' => "pinkwall",
            'y' => "pickup_shield",
            'b' => "button",
            'v' => "vines",
            'g' => "gem",
            'd' => "door",
            'u' => "door",
            'c' => "chandelier",
            'i' => "intervalplatform",
            'j' => "pickup_jump",
            'a' => "pickup_speed",
            // TODO h 5,3
            _ => "black",
        };
    }

    public static string ConvertCharToType(char type)
    {
        return type switch
        {
            'a' => "speed",
            'b' => "button",
            'c' => "chandelier",
            'd' => "door",
            'u' => "puzzledoor",
            'f' => "fire",
            'o' => "ghost_left_0",
            'g' => "gem",
            'i' => "interval",
            'j' => "jump",
            'k' => "key",
            'l' => "lever",
            'p' => "platform",
            's' => "spider",
            'v' => "vines",
            'w' => "wall",
            'W' => "pinkwall",
            'y' => "armor",
            '!' => "Use WASD to move and hold space to jump higher",

            _ => "nothing",
        };
    }

    public static void getDisplayHUD(int Score, int Coins, int Level, int Stars, bool Armor, int Time, string text)
    {
        switch (text)
        {
            //player
            case "Score":
                HUDEntity.disp = $"Score {Score.ToString()}";
                break;
            case "Coins":
                HUDEntity.disp = $"Coins {Coins.ToString()}";
                break;
            case "Level":
                if (Level == 4)
                {
                    HUDEntity.disp = "Boss Level";
                }
                else
                {
                    if (Level > 10)
                    {
                        HUDEntity.disp = "Puzzle Room";
                    }
                    HUDEntity.disp = "Level " + Level.ToString();
                }
                break;
            case "Stars":
                if (Level < 10) // if main level
                {
                    HUDEntity.disp = "";
                }
                else
                {
                    HUDEntity.disp = $"Stars {Stars.ToString()}";
                }
                break;
            case "Armor":
                if (Armor)
                {
                    HUDEntity.disp = "ARMOR";
                }
                else
                {
                    HUDEntity.disp = "";
                }
                break;
            case "time":
                // if a main level (not puzzle level)
                if (Level < 10)
                {
                    if (Time > 59)
                    {
                        int minutes = Time / 60;
                        int seconds = Time % 60;

                        if (seconds < 10)
                        {
                            HUDEntity.disp = $"{minutes.ToString()}:0{seconds.ToString()}";
                        }
                        else
                        {
                            HUDEntity.disp = $"{minutes.ToString()}:{seconds.ToString()}";
                        }
                    }
                    else
                    {
                        HUDEntity.disp = $"Time {Time.ToString()}";
                    }
                }
                // if puzzle room
                else
                {
                    HUDEntity.disp = "PUZZLETIME";
                }
                break;
            default:
                HUDEntity.disp = text;
                break;
        }
    }

    public static int getRandomNum()
    {
        Random random = new Random();
        return random.Next(0, 10);
    }

    public static Bounds2 getAnimationCollisionBox(string relativePath)
    {

        string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Textures");
        string filePath = Path.Combine(basePath, relativePath + ".png");

        // padding field
        float padding = 0f;  

        
        int startX = int.MaxValue;
        int startY = int.MaxValue;
        int endX = int.MinValue;
        int endY = int.MinValue;


        Bitmap bitmap = new Bitmap(filePath);
        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                if (bitmap.GetPixel(x, y).A > 0) // Check for non-transparent pixel (uses alpha value of pixel)
                {
                    startX = Math.Min(startX, x);
                    endX = Math.Max(endX, x);
                    startY = Math.Min(startY, y);
                    endY = Math.Max(endY, y);
                }
            }
        }
        
            
        
        // generate collision box using render scale and padding
        Vector2 position = new Vector2(startX * renderScaleFactor - padding, startY * renderScaleFactor - padding);
        Vector2 size = new Vector2((endX - startX + padding) * renderScaleFactor, (endY - startY + padding) * renderScaleFactor);

        bitmap.Dispose();

        return new Bounds2(position, size);
    }


    public static float distanceBetween(Vector2 pos1, Vector2 pos2)
    {
        
        float distance = (float) Math.Pow(Math.Pow(pos1.X + pos2.X, 2) + Math.Pow(pos1.Y + pos2.Y, 2), 0.5);
        return distance;
        
        
    }

    public static String getPathToAnimation(String animationEntity, String animationType)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Textures", animationEntity, animationType);
    }


    public static int getFileCount(string folderPath)
    {
        return 1;

        //FIX
        int fileCount = 0;

        try
        {
            // Get all files in the folder and subfolders
            string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);
            fileCount = files.Length;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"file counting messed up");
        }

        return fileCount;
    }
}
