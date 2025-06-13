using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using System;
using System.Drawing;
using System.Data;
using System.Runtime.InteropServices.JavaScript;


/// <summary>
/// Main game class that handles initialization, updates, and rendering.
/// </summary>
internal class Game
{
    //// === Constants ===
    public static readonly string Title = "Haunted Castle Escape";
    public static readonly Point2 Resolution = new(1500, 1020); // Resolution of the game window
    public static readonly float PixelsPerMeter = 64; // Conversion factor: 64 pixels = 1 meter
    //// === Fields ===
    public static Font font = Engine.LoadFont("swirly-font.ttf", 50);
    public static ScreenFlow screenManager;

    /// <summary>
    /// Default constructor to initialize the game.
    /// </summary>
    public Game() { }

    // === Methods ===

    /// <summary>
    /// Initializes the game by setting up the first level.
    /// </summary>
    public void Initialize()
    {
        //playerBackend.SaveStatsToFile();

        Bounds2 defaultBounds = new Bounds2(new Vector2(0.5f, 0.5f), new Vector2(1, 1));  // Default size if null
        Bounds2 popUpBounds = new Bounds2(new Vector2(0.5f, 0.5f), new Vector2(0.75f, 0.75f));  // Default size if null

        //pausebutton-> pause screen-> playButton
        Button playButton = new Button(false, "button_play", new Bounds2(new Vector2(.5f, .5f), new Vector2(1f / 6.5f, 0.08f)));
        Screen pauseScreen = new Screen(new List<Button> { playButton }, true, "screen_pause", defaultBounds); //replace with screen_pause

        Button pauseButton = new Button(true, "button_pause", new Bounds2(new Vector2(.92f, .13f), new Vector2(.07f, .09f)), pauseScreen);
        //game Screen
        Screen gameScreen = new GameScreen(new List<Button>{pauseButton});
      
        //dead screen ->retry button
        Button tryAgain = new Button(false, "button_tryagain", new Bounds2(new Vector2(.5f, .6f), new Vector2(.2f, .4f)));
        Screen deadScreen = new Screen(new List<Button> { tryAgain }, true, "deadScreen", new Bounds2(new Vector2(.5f, .5f), new Vector2(.7f, .7f)));
      
        Screen levelUpScreen = new LevelUpScreen(new List<Button> {getScreenExitButton()}, false, "screen_levelup", defaultBounds);
        Screen scoreScreen = new ScoreboardScreen(new List<Button> {getScreenExitButton()}, true, "screen_scoreboard", popUpBounds);
        Screen rulesScreen = new Screen(new List<Button> { getScreenExitButton() }, true, "screen_instructions", popUpBounds);
        Screen creditsScreen = new Screen(new List<Button> { getScreenExitButton() }, true, "screen_gamecredits", popUpBounds);

        //start screen intializing
        List<Button> startScreenButtons = new List<Button>();
        startScreenButtons.Add(new Button(false, "button_play", new Bounds2(new Vector2(3f / 12, 0.65f), new Vector2(1f / 6.5f, 0.08f)))); //start game button
        startScreenButtons.Add(new Button(true, "button_scoreboard", new Bounds2(new Vector2(5f / 12, 0.65f), new Vector2(1f / 6.5f, 0.08f)), screen: scoreScreen)); //view score button
        startScreenButtons.Add(new Button(true, "button_instructions", new Bounds2(new Vector2(7f / 12, 0.65f), new Vector2(1f / 6.5f, 0.08f)), screen: rulesScreen)); //rules score button
        startScreenButtons.Add(new Button(true, "button_credits", new Bounds2(new Vector2(9f / 12, 0.65f), new Vector2(1f / 6.5f, 0.08f)), screen: creditsScreen)); //credit score button
        Screen startScreen = new Screen(startScreenButtons, false, "screen_start", defaultBounds);

        screenManager = new ScreenFlow(startScreen, gameScreen, deadScreen, levelUpScreen);
    }

    private Button getScreenExitButton ()
    {
        return new Button(false, "button_exitscreen", new Bounds2(new Vector2(0.87f, 0.19f), new Vector2(0.06f, 0.08f)));
    }

    /// <summary>s
    /// Loads required assets like textures.
    /// </summary>
    public void LoadContent()
    {
        Textures.loadTextures(); // Load textures for the game
    }

    /// <summary>
    /// Main update loop for the game. Handles logic, movement, and rendering updates.
    /// </summary>
    public void Update()
    {
        screenManager.update();
    }
    // methods to keep
    // loadleaderboard
}
