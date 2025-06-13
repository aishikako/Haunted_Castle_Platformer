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
using static System.Formats.Asn1.AsnWriter;
using System.Drawing;
using System.Reflection.Emit;

// Class to handle dispalying entities on screen
internal class LevelDisplay
{
    // === Fields ===
    public string fileName;
    FileManager fileManager = new FileManager();
    SoundEffects music = new SoundEffects();
    public Dictionary<string, List<Entity>> allEntities;
    public Dictionary<string, List<Entity>> currEntities;
    public int playerNumber = 1;

    /// <summary>
    /// Initializes the level by loading entities and setting up camera bounds.
    /// Call this method after creating a LevelDisplay instance.
    /// </summary>
    /// <param name="file">The file path of the level data.</param>
    public void Init(string file)
    {
        Camera.boundx = 0;
        Camera.boundy = 0;
        fileName = file;
        FileManager.FileName = fileName;

        //calling file Manager to parse notepad and storing entities
        Dictionary<string, List<Entity>> entities = fileManager.LoadLevel();
        allEntities = entities;
        var currEntities = HelperMethods.EntitiesDict();


        // Play background music
        music.playBackgroundMusic();
    }

    public void DisplayStats(Vector2 Resolution)
    {
        playerNumber = fileManager.DisplayStats(Resolution);
    }
    public void UpdateStats(int score, int coins, int level, int stars, bool armor, int health, float time)
    {
        if (currEntities != null)
        {
            foreach (var entity in currEntities["poptext"])
            {
                entity.UpdateStats(score, coins, level, stars, armor, health, time);
            }

            foreach (var entity in currEntities["characters"])
            {
                entity.UpdateStats(score, coins, level, stars, armor, health, time);
            }
        }
        
    }

    /// <summary>
    /// Displays the current level by drawing entities within camera bounds.
    /// </summary>
    public void DisplayLevel()
    {
        if (currEntities != null)
        {
            foreach (var layer in currEntities)
            {
                foreach (var entity in layer.Value)
                {
                    entity.UpdateAnimation(Engine.TimeDelta);  // Update the animation for each entity.
                    entity.drawEntity(Camera.boundx, Camera.boundy); // Draw the entity at its position relative to the camera.
                }
            }
        }
        
    }

    /// <summary>
    /// Updates the list of entities currently visible on the screen based on camera bounds.
    /// </summary>
    public void UpdateScreenEntities()
    {
        
            // Clear the current entity list and populate with entities in bounds
            //currEntities = new List<Entity>();

            currEntities = HelperMethods.EntitiesDict();
            //currEntities["characters"].Add(GameScreen.player);

            foreach (var layer in allEntities)
            {
                foreach (var entity in layer.Value)
                {
                    if (Camera.entityInBounds(entity))
                    {


                        currEntities[layer.Key].Add(entity);
                    }

                }
            }
        
        
    }

    //public void UpdateLighting()
    //{
    //    for (int y = 0; y < Game.Resolution.Y; y += 64) // Step through 64-pixel sections
    //    {
    //        for (int x = 0; x < Game.Resolution.X; x += 64)
    //        {
    //            // Calculate the center of the current 64x64 section
    //            float blockCenterX = x + 32; // Half of 64
    //            float blockCenterY = y + 32; // Half of 64

    //            // Adjust for camera bounds
    //            Vector2 blockCenter = new Vector2(blockCenterX, blockCenterY);

    //            // Player's position relative to camera
    //            Vector2 playerPosition = new Vector2(Game.player.xPos - Camera.boundx, Game.player.yPos - Camera.boundy);

    //            // Calculate the distance
    //            float distance = HelperMethods.distanceBetween(blockCenter, playerPosition);

    //            // Limit the distance to half of the screen height
    //            float adjustedDistance = Math.Min(distance, Game.Resolution.Y / 2);

    //            // Scale the distance to get an alpha value
    //            float shiftScale = adjustedDistance / (Game.Resolution.Y / 2);
    //            byte newAlphaValue = (byte)(255 * shiftScale);

    //            //Draw the lighting effect for this block

    //           Engine.DrawRectSolid(
    //               new Bounds2(new Vector2(x, y), new Vector2(64, 64)),
    //               new Color(0, 0, 0, (byte) (newAlphaValue/2))
    //           );

    //           System.Diagnostics.Debug.WriteLine(String.Format("playerx: {0}, playery: {1}, x: {2}, y: {3}", Game.player.xPos - Camera.boundx, Game.player.yPos - Camera.boundy, x, y));
    //        }
    //    }
    //}


    public bool containsPlayer()
    {
        if (currEntities == null)
        {
            return false;
        }
        foreach (Entity entity in currEntities["characters"].AsEnumerable())
        {
            if (entity.type.Equals("player"))
            {
                return true;
            }
        }
        return false;
    }
}

