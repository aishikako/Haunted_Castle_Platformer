using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents a spider enemy that moves toward the player and interacts based on player actions.
/// </summary>
internal class SpiderEntity : MobEntity
{
    private bool isAlive = true;
    private bool currTexture = false;
    private Texture texture1 = Textures.textures["spider_1"];
    private Texture texture2 = Textures.textures["spider_2"];
    private readonly float initialYPos; // Store initial Y position

    public SpiderEntity(float xPos, float yPos, float width, float height, string color, Vector2 speed, Bounds2? movementBounds)
        : base(xPos, yPos, width, height, color, speed, movementBounds, health: 2)
    {
        SpeedMpSInit = 2 * speed; // Initialize SpeedMpSInit to the provided speed
        initialYPos = yPos; // Lock initial Y position
    }

    public override void runMovement(float deltaTime)
    {
        if (!isAlive) return; // Stop movement if dead

        // Temporary animation toggle
        if (currTexture)
        {
            texture = Textures.textures["spider_1"];
            currTexture = false;
        }
        else
        {
            texture = Textures.textures["spider_2"];
            currTexture = true;
        }

        // Check if the player presses the slash key
        if (Engine.GetKeyHeld(Key.Slash))
        {
            TakeDamage(1);
            if (Health <= 0) Die();
            return;
        }

        // Horizontal movement toward the player
        float xDifference = GameScreen.player.Position.X - xPos;
        float directionX = xDifference > 0 ? 1 : (xDifference < 0 ? -1 : 0);

        // Update only horizontal speed, ignore vertical speed
        SpeedMpS = new Vector2(directionX * SpeedMpSInit.Length(), 0); // No vertical movement

        // Prevent the spider from moving past the player
        bool[] collisions = CheckCollision(GameScreen.player);
        if (collisions.Any(collision => collision))
        {
            SpeedMpS = Vector2.Zero; // Stop all movement
        }

        // Update position based on speed
        xPos += SpeedMpS.X * deltaTime;

        // Lock Y position to initial value
        yPos = initialYPos; // Prevent vertical movement

        // Skip inherited vertical physics by not calling base.runMovement
    }

    /// <summary>
    /// Handles the spider's interaction with the player on collision.
    /// </summary>
    public void HandleCollision(PlayerEntity player)
    {
        if (Engine.GetKeyHeld(Key.LeftControl) || Engine.GetKeyHeld(Key.RightControl))
        {
            TakeDamage(1); // Spider loses health

            if (Health <= 0)
            {
                Die(); // Kill the spider if health drops to 0
            }
            return; 
        }
        else
        {
            // If Slash key is not held, the player takes damage
            if (Armor)
            {
                player.TakeDamage(1);
            }
            else
            {
                player.TakeDamage(2); // Player loses health
            }
        }

        // Restrict the player's X position
        if (player.xPos < xPos) // Player is colliding from the left
        {
            player.xPos = xPos - player.Width; 
        }
        else if (player.xPos > xPos) // Player is colliding from the right
        {
            player.xPos = xPos + Width; 
        }

        // Restrict the player's Y position
        if (player.yPos < yPos) // Player is colliding from below
        {
            player.yPos = yPos - player.Height; 
        }
        else if (player.yPos > yPos) // Player is colliding from above
        {
            player.yPos = yPos + Height; 
        }

        // Stop all player movement
        player.SpeedMpS = Vector2.Zero; // Stop player velocity
        player.movingLeft = false;
        player.movingRight = false;
        player.movingUp = false;
        player.movingDown = false;
    }

    /// <summary>
    /// Kills the spider and removes it from the screen.
    /// </summary>
    private void Die()
    {
        isAlive = false; // Mark the spider as dead
        SpeedMpS = Vector2.Zero; // Stop movement
    }

}
