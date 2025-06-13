using System;
using System.IO;

/// <summary>
/// Represents a generic entity in the game with properties like position, dimensions, and animations.
/// </summary>
internal class Entity
{
    // === Properties ===
    public string type { get; set; }
    public float xPos { get; set; }
    public float yPos { get; set; }
    public float Width { get; protected set; }
    public float Height { get; protected set; }

    /// Color of the entity (used for rendering)
    public string color { get; set; }

    /// Text of the entity (used for HUD text boxes)
    public string text { get; set; }

    /// Scaling factor for rendering the entity
    public float Scale { get; private set; } = 1.0f;

    /// Opacity level of the entity (range: 0 to 1)
    public float Opacity { get; private set; } = 1.0f;

    ///Current Sprite
    public Texture texture { get; set; }

    
    /// The current animation strategy used by the entity.
    public Animations CurrentAnimation { get; set; }
    public bool isHidden = false;
    //setting time to a random number, so interval platforms have time offset
    public float time = (HelperMethods.getRandomNum());

    /// Player stats updated throughout the game.
    public int Level = 0;
    public int Score = 0;
    public int Coins = 0;
    // ranges from 0 to 2. 0 = player restart
    public int Health = 2;
    public bool Armor = false;
    public float Time = 0;
    public static int Stars = 0;


    // === Constructor ===

    /// <summary>
    /// Initializes an Entity with the specified position, dimensions, color, and type.
    /// </summary>
    /// <param name="xPos">X-coordinate of the entity's position.</param>
    /// <param name="yPos">Y-coordinate of the entity's position.</param>
    /// <param name="width">Width of the entity in pixels.</param>
    /// <param name="height">Height of the entity in pixels.</param>
    /// <param name="color">Color of the entity.</param>
    /// <param name="type">Type of the entity.</param>
    public Entity(float xPos, float yPos, float width, float height, string color, string type)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.Width = width;
        this.Height = height;
        this.color = color;
        this.type = type;
        texture = null;
    }

    /// <summary>
    /// Initializes background entities for parallax scrolling and HUD
    /// </summary>
    public Entity(float xPos, float yPos, float width, float height, string image, float? cameraSpeed = null)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.Width = width;
        this.Height = height;
        this.color = image;
    }

    /// <summary>
    /// Initializes background entities for HUD text boxes
    /// </summary>
    public Entity(float xPos, float yPos, float width, float height, string text, string image, string type, float? cameraSpeed = null)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.color = image;
        this.text = text;
        this.type = type;
    }

    // === Methods ===

    /// <summary>
    /// Triggers an animation on the entity with the specified strategy, duration, and optional color.
    /// </summary>
    /// <param name="strategy">Animation strategy to use.</param>
    /// <param name="duration">Duration of the animation in seconds.</param>
    /// <param name="color">Optional color for the animation.</param>
    public void TriggerAnimation(Animations animation, float duration)
    {
        // If there is an ongoing animation and it's currently active, stop it
        if (CurrentAnimation != null && CurrentAnimation.IsAnimating)
        {
            CurrentAnimation.StopAnimation(this);
        }
        // Set the new animation and start it
        CurrentAnimation = animation;
        CurrentAnimation.StartAnimation(this, duration);
    }

    /// <summary>
    /// Updates the animation state based on the elapsed time.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last update in seconds.</param>
    // Update the entity and its animation
    public void UpdateAnimation(float deltaTime)
    {
        CurrentAnimation?.UpdateAnimation(this, deltaTime);
    }

    /// <summary>
    /// Draws the entity at the adjusted position based on camera offsets.
    /// </summary>

    public virtual void drawEntity(float xCamOffset, float yCamOffset)

    /// <param name="xCamOffset">Horizontal offset of the camera.</param>
    /// <param name="yCamOffset">Vertical offset of the camera.</param>
    {
        time += Engine.TimeDelta;
        if (isHidden) return;
        // Calculate scaled size and centered position
        float scaledWidth = Width * Scale;
        float scaledHeight = Height * Scale;
        Vector2 sizeVector = new Vector2(scaledWidth, scaledHeight);
        Vector2 posVector = new Vector2(
            xPos - xCamOffset + (Width - scaledWidth) / 2,
            yPos - yCamOffset + (Height - scaledHeight) / 2
        );

        // Draw the entity using the current color and opacity
        Engine.DrawTexture(
            texture: (texture == null) ? Textures.textures[color] : texture,
            position: posVector,
            size: sizeVector,
            rotation: 0 // Add rotation if needed
        );
    }

    public void UpdateStats(int score, int coins, int level, int stars, bool armor, int health, float time)
    {
        Score = score;
        Coins = coins;
        Level = level;
        Stars = stars;
        Armor = armor;
        Health = health;
        Time = time;
    }
}

