using System.Diagnostics;
using System.IO;
using System.Linq;

using System;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices.JavaScript;
using System.Collections.Generic;

internal class GameScreen : Screen, IScreen
{
    // === Constants ===
    
    public static readonly Point2 Resolution = new(1500, 1020); // Resolution of the game window
    public static readonly float PixelsPerMeter = 64; // Conversion factor: 64 pixels = 1 meter

    // === Fields ===
    public static LevelDisplay levelDisplay = new LevelDisplay(); // Displays the level
    public static BackendPlayerEntity playerBackend = new BackendPlayerEntity(0, 0, 1); // Default stats: 0 score, 0 coins, level 1
    public static Font font = Engine.LoadFont("swirly-font.ttf", 50);

    // Public player entity accessible throughout the game
    public static PlayerEntity player { get; set; }
    public static bool initialized = false;

    public static bool Autoscroll = true; // Controls automatic camera scrolling
    public static bool endOfLevel = false; // When true, displays the player's stats at the end of the game
    private static int maxLevels = 4;
    public static float gamePosX = 0;
    public static float gamePosY = 0;
    public static float maxPosX = 0;
    public static int bossNumCollectibles = 10;

    public static SoundEffects music = new SoundEffects();

    // === Constructors ===
    public GameScreen(List<Button> buttons)
        : base(buttons, false, null, new Bounds2(new Vector2(0.5f,0.5f), new Vector2(1,1)))
    {
        playerBackend.SaveStatsToFile();
    }

    // === Methods ===

    /// <summary>
    /// Main update loop for the game. Handles logic, movement, and rendering updates.
    /// </summary>
    public override void Update()
    {
        if (!(initialized))
        {
            initialized = true;

            levelDisplay.Init(GetFile(playerBackend.Level)); // Load the player's current level
            playerBackend.Time = 0;
            playerBackend.puzzleTime = 0;

            playerBackend.Gems = 0;
        }

        else
        {
            refreshLevel(); // Restart the current level if needed
            levelDisplay.UpdateScreenEntities(); // Update entities on the screen

            float deltaTime = Engine.TimeDelta;

            if (playerBackend.Level >= 10)
            {
                playerBackend.puzzleTime += deltaTime;
            }

            if (player.Health <= 0)
            {
                HandleGameOver();
                //return;
            }

            HandleScrolling(deltaTime); // Scroll the camera if necessary

            updateIntervalPlatforms(deltaTime);

            HandleNumericKeyInputs(); // Handle key inputs for switching levels

            player.updateStates(); // Update the player's state

            updatedMovingEntities(deltaTime); // Update movement of entities

            updatingMobEntities(deltaTime); // Update the player's movement

            updateButtons(deltaTime);

            updateChandeliers();

            HandleMovingEntityCollisions(); // Check for collisions between entities
                                            // (Also checks whether door is reached, i.e. end of level)

            //run moving entity animations
            foreach (var layer in levelDisplay.currEntities)
            {
                foreach (var entity in layer.Value)
                {
                    if (entity is MovingEntity)
                    {
                        MovingEntity movingEntity = (MovingEntity)entity;
                        if (movingEntity.movingEntityAnimation != null)
                        {
                            movingEntity.movingEntityAnimation.Animate();

                        }
                    }
                }
            }

            //run boss entity animations
            foreach (var layer in levelDisplay.currEntities)
            {
                foreach (var entity in layer.Value)
                {
                    if (entity is BossEntity)
                    {
                        BossEntity bossEntity = (BossEntity)entity;
                        bossEntity.updateHealth();
                        if (bossNumCollectibles < 1)
                        {
                            EndLevel();
                        }
                        if (bossEntity.movingEntityAnimation != null)
                        {
                            bossEntity.movingEntityAnimation.Animate();

                        }
                        if ((bossEntity.collidedEntityLeft != null && bossEntity.collidedEntityLeft.type.Equals("player")) || (bossEntity.collidedEntityRight != null && bossEntity.collidedEntityRight.type.Equals("player"))
                            || (bossEntity.collidedEntityTop != null && bossEntity.collidedEntityTop.type.Equals("player")) || (bossEntity.collidedEntityBottom != null && bossEntity.collidedEntityBottom.type.Equals("player")))
                        {
                            player.Health = 0;
                        }
                    }
                }
            }
            
            //run player animation
            player.playerEntityAnimation.Animate();

            UpdatePlayerStats(deltaTime); // Update player's score and stats
            
            // End of level
            EndLevel();

            base.Update();
        }
    }

    public override void Draw()
    {
        UpdateDisplay();
        base.DrawButtons();
    }

    private void HandleGameOver()
    {
        Console.WriteLine("Game Over! The player has died.");
        // Optionally restart the level or end the game:
        levelDisplay.Init(GetFile(playerBackend.Level)); // Restart the current level
    }

    /// <summary>
    /// Handles collisions between moving entities and the player.
    /// </summary>
    private void HandleMovingEntityCollisions()
    {
        player.resetStates(); // Reset collision states


        foreach (var layer in levelDisplay.currEntities)
        {
            foreach (var entity2 in layer.Value)
            {
                if (entity2 is CollidableEntity collidableEntity && collidableEntity != player && !collidableEntity.isHidden)
                {
                    bool[] collides = player.CheckCollision(collidableEntity); // Check collision with player

                    if (collides.Contains(true))
                    {
                        if (collidableEntity.type.Equals("spikes") || collidableEntity.type.Equals("fire")) player.Health = 0;
                        player.handleCollision(collides); // Handle collision response
                    }
                }
                else if (entity2 is IInteractables interactable && !entity2.isHidden)
                {
                    interactable.IsCollided(player);
                }
            }
        }
    }

    private void updatingMobEntities(float deltaTime)
    {
        foreach (var layer in levelDisplay.currEntities)
        {
            foreach (var entity in layer.Value)
            {
                if (entity is PhysicsEntity mobEntity)
                {
                    mobEntity.runMovement(deltaTime);
                }
            }
        }

        foreach (var layer in levelDisplay.currEntities)
        {
            foreach (var entity in layer.Value)
            {
                if (entity is SpiderEntity spider)
                {
                    bool[] collides = player.CheckCollision(spider); // Check collision with spider

                    if (collides.Contains(true)) // If there's a collision
                    {
                        spider.HandleCollision(player); // Stop player movement on collision
                    }
                }
            }
        }
        
        foreach (var layer in levelDisplay.currEntities)
        {
            foreach (var entity in layer.Value)
            {
                if (entity is GhostEntity ghost)
                {
                    bool[] collides = player.CheckCollision(ghost); // Check collision with spider

                    if (collides.Contains(true)) // If there's a collision
                    {
                        ghost.HandleCollision(player); // Stop player movement on collision
                    }
                }
            }
        }

    }

    /// <summary>
    /// Updates all moving entities in the level.
    /// </summary>
    private void updatedMovingEntities(float deltaTime)
    {
        foreach (var layer in levelDisplay.currEntities)
        {
            foreach (var entity in layer.Value)
            {
                if (entity is MovingEntity movingEntity)
                {
                    // Now you can work with movingEntity
                    movingEntity.Move(deltaTime); // Example method
                }
            }
        }
    }

    private void updateChandeliers()
    {
        foreach (PhysicsEntity chandelier in levelDisplay.currEntities["blocks"].OfType<PhysicsEntity>())
        {
            if (chandelier.type.Equals("chandelier"))
            {
                foreach (CollidableEntity entity in levelDisplay.currEntities["blocks"].OfType<CollidableEntity>())
                {
                    if (!entity.type.Equals("chandelier"))
                    {
                        bool[] collides = chandelier.CheckCollision(entity); // Check collision with block
                        chandelier.CheckCollision(player);
                        if (chandelier.collidedEntityBottom != null && chandelier.collidedEntityBottom.type.Equals("player"))
                        {
                            player.Health = 0;
                        }
                    }
                }
            }
        }
    }

    private void updateIntervalPlatforms(float deltaTime)
    {
        int[] values = { 1, 2, 3, 4 };

        foreach (var entity in levelDisplay.currEntities["blocks"])
        {
            if (entity.type.Equals("interval"))
            {
                // Now you can work with movingEntity
                entity.time += deltaTime; // Example method
                                            //setting the interval platform to hidden or shown
                entity.isHidden = !(values.Contains((int)entity.time % 5));
            }
        }
    }

    private void updateButtons(float deltaTime)
    {
        foreach (leverButtonEntity entity in levelDisplay.currEntities["items"].OfType<leverButtonEntity>())
        {
            if (entity.type.Equals("button") && entity.LinkedEntities[0].isHidden)
            {
                entity.time += deltaTime;
                entity.checkButtonTimer();
            }
        }
    }

    // used to handle collision between puzzle door and player
    public static void HandlePuzzles()
    {
        // if the player is currently in a puzzle "level"
        if (playerBackend.Level > 10)
        {
            Game.screenManager.handleAddScreen(ScreenFlow.ScreenType.levelUpScreen);
            playerBackend.Level -= 9;
            levelDisplay.Init(GetFile(playerBackend.Level));
        }

        // if the player is currently in a main level
        else
        {
            playerBackend.Level += 10;
            levelDisplay.Init(GetFile(playerBackend.Level));
        }
    }

    public void TriggerAnimationForEntity(CollidableEntity entity, string type)
    {
        switch (type)
        {
            case "platform":
                entity.TriggerAnimation(new FlashAnimation("white", 1.0f), 1.0f);
                break;
            case "door":
                // go to the next level
                playerBackend.Level -= (10 - 1);
                //playerBackend.Level++;
                levelDisplay.Init(GetFile(playerBackend.Level));
                break;
            case "puzzledoor":
                // puzzles are like mini-levels within a level
                // puzzle levels are the same number as normal levels +10
                // e.g. the puzzle for level 3 returns 13 for playerBackend.Level

                // if the player is currently in a puzzle "level"
                if (playerBackend.Level > 10)
                {
                    // turn on autoscroll
                    Autoscroll = true;
                    // switch to main level
                    playerBackend.Level -= 10;
                    levelDisplay.Init(GetFile(playerBackend.Level));
                    break;
                }

                // if the player is currently in a main level
                else
                {
                    // turn off autoscroll
                    Autoscroll = false;
                    // switch to puzzle level
                    playerBackend.Level += 10;
                    levelDisplay.Init(GetFile(playerBackend.Level));
                    break;
                }
        }
    }

    private void HandlePlayerCollision()
    {
        player.resetStates();
        foreach (var layer in levelDisplay.currEntities)
        {
            foreach (CollidableEntity entity in layer.Value)
            {
                if (entity != player && !entity.isHidden && entity is CollidableEntity collidableEntity)
                {
                    bool[] collisionResults = player.CheckCollision(collidableEntity);
                    if (collisionResults.Contains(true) && !collidableEntity.IsAnimating)
                    {
                        TriggerAnimationForEntity(collidableEntity, collidableEntity.type);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles camera scrolling, either automatically or via user input.
    /// </summary>
    private void HandleScrolling(float deltaTime)
    {
        if (Autoscroll && levelDisplay.currEntities != null)
        {
            // Change autoscroll speed depending on number of entities
            int numEntities = levelDisplay.currEntities.Keys.Count();

            Camera.UpdateBounds(Direction.RIGHT, deltaTime, player.xPos, maxPosX); // Auto-scroll camera

            // ***Implement vertical autoscroll
            float currentY = player.yPos;
            Camera.UpdateBoundsFollowingPlayer(Direction.UP, currentY); // Scroll camera up
        }
        else
        {
            HandleDirectionalKeys(deltaTime); // Scroll based on user input
        }
    }

    /// <summary>
    /// Handles directional input keys for manual camera scrolling.
    /// </summary>
    private void HandleDirectionalKeys(float deltaTime)
    {
        var directionKeyMapping = new Dictionary<Key, Direction>
    {
        { Key.Right, Direction.RIGHT },
        { Key.Left, Direction.LEFT },
        { Key.Up, Direction.UP },
        { Key.Down, Direction.DOWN }
    };

        foreach (var key in directionKeyMapping.Keys)
        {
            if (Engine.GetKeyHeld(key))
            {
                Camera.UpdateBounds(directionKeyMapping[key], deltaTime);
                return;
            }
        }
    }

    /// <summary>
    /// Handles numeric key inputs to switch levels.
    /// </summary>
    private void HandleNumericKeyInputs()
    {
        for (int i = 1; i <= 6; i++)
        {
            if (Engine.GetKeyDown((Key)Enum.Parse(typeof(Key), $"NumRow{i}")))
            {
                levelDisplay.Init(GetFile(i)); // Load the selected level
                playerBackend.Level = i; // Update the player's current level
                playerBackend.ResetStats(); // Reset player's score and coins
                return;
            }
        }
    }

    /// <summary>
    /// Updates the player's score and stats based on elapsed time.
    /// </summary>
    private void UpdatePlayerStats(float deltaTime)
    {
        // playerBackend.PlayerNumber = levelDisplay.playerNumber;
        playerBackend.Time += deltaTime;
        playerBackend.Score += (int)(10 * deltaTime); // Increment score by 10 points per second
        playerBackend.InGameDisplayStats();
    }

    /// <summary>
    /// Updates the screen display, including level and player stats.
    /// </summary>
    private void UpdateDisplay()
    {
        levelDisplay.UpdateStats(playerBackend.Score, playerBackend.Gems, playerBackend.Level, BackendPlayerEntity.Stars, playerBackend.Armor, playerBackend.Health, playerBackend.Time);
        levelDisplay.DisplayLevel(); // Render the current level

        DisplayPlayerStats(); // Render player stats
    }

    /// <summary>
    /// Displays player stats on the screen or console.
    /// </summary>
    private void DisplayPlayerStats()
    {
        Console.WriteLine($"Score: {playerBackend.Score}");
        Console.WriteLine($"Gems: {playerBackend.Gems}");
        Console.WriteLine($"Level: {playerBackend.Level}");
        Console.WriteLine($"Time: {playerBackend.Time}");
    }

    /// <summary>
    /// Gets the file path for the specified level.
    /// </summary>
    public static string GetFile(int level)
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var levelFiles = new[] { "level1.txt", "level2.txt", "level3.txt", "boss.txt", "test.txt", "hud.txt" };
        var puzzleFiles = new[] { "puzzle1.txt", "puzzle2.txt", "puzzle3.txt" };
        if (level >= 1 && level <= levelFiles.Length)
        {
            Autoscroll = true;
            return Path.Combine(basePath, "..", "..", "..", "Levels", levelFiles[level - 1]);
        }

        // puzzles are thought of as mini-levels within a main level
        // a puzzle's level is the current level + 10
        // e.g. the puzzle for level 3 is "level 13"

        // if a puzzle level
        if (level > 10)
        {
            Autoscroll = false;
            level -= 10;
            return Path.Combine(basePath, "..", "..", "..", "Levels", puzzleFiles[level - 1]);
            //return Path.Combine(basePath, "..", "..", "..", "Levels", puzzleFiles[(level - 1) - 10]);
        }
        return string.Empty;
    }

    /// <summary>
    /// Gets the file path for the specified level.
    /// </summary>
    public string GetHUD()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var levelFiles = new[] { "level1.txt", "level2.txt", "level3.txt", "boss.txt", "test.txt", "hud.txt" };

        return Path.Combine(basePath, "..", "..", "..", "Levels", levelFiles[6 - 1]);
    }

    public static void EndLevel()
    {
        
        if (endOfLevel)
        {
            initialized = false;
            endOfLevel = false;

            Game.screenManager.handleAddScreen(ScreenFlow.ScreenType.levelUpScreen);

            if (playerBackend.Level == maxLevels+1)
            {
                Environment.Exit(0);
            }
        }
    }

    /// <summary>
    /// Restarts the current level if the reset key is pressed.
    /// </summary>
    private void refreshLevel()
    {
        if (Engine.GetKeyDown(Key.R))
        {
            playerBackend.SaveStatsToFile();
            playerBackend.ResetStats();
            initialized = false;
        }
        if (levelDisplay != null && !levelDisplay.containsPlayer())
        {
            restartLevel();

        }
    }

    public void restartLevel()
    {
        playerBackend.SaveStatsToFile();
        playerBackend.ResetStats();
        initialized = false;
    }
}