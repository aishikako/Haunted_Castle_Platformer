using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class GhostEntity : MobEntity
{
    private bool isAlive = true;
    // tests whether armor defended the player from this ghost
    private bool playerArmored = false;
    public GhostEntity(float xPos, float yPos, float width, float height, string color, Vector2 speed, Bounds2? movementBounds)
        : base(xPos, yPos, width, height, color, speed, null, health: 10)
    {
        // Initialize SpeedMpSInit to the provided speed
        SpeedMpSInit = 4 * speed;
    }

    public override void runMovement(float deltaTime)
    {
        //temp animation
        if (SpeedMpS.X < 0)
        {
            texture = Textures.textures["ghost_left_0"];
        }
        else
        {
            texture = Textures.textures["ghost_right_0"];
        }


        if (!isAlive) return; // Ensure the ghost is alive before moving

        // Calculate the direction toward the player
        Vector2 toPlayer = new Vector2(GameScreen.player.Position.X - xPos, -(GameScreen.player.Position.Y - yPos)).Normalized();

        // Calculate a tangential component (perpendicular to the direct path)
        Vector2 tangentialDirection = new Vector2(-toPlayer.Y, toPlayer.X); // Perpendicular vector

        // Introduce variability in the tangential movement
        float oscillationFactor = (float)Math.Sin(deltaTime * 2); // Oscillates between -1 and 1
        tangentialDirection *= oscillationFactor;

        // Combine the direct and tangential components
        Vector2 combinedDirection = (toPlayer + tangentialDirection * 0.5f).Normalized();

        // Update the ghost's speed based on the combined direction
        SpeedMpS = combinedDirection * SpeedMpSInit.Length();

        // Prevent the spider from moving past the player
        bool[] collisions = CheckCollision(GameScreen.player);
        if (collisions.Any(collision => collision))
        {
            SpeedMpS = Vector2.Zero; // Stop movement
        }

        // Update position based on speed
        xPos += SpeedMpS.X * deltaTime;
        yPos += SpeedMpS.Y * deltaTime;

        // Execute base movement
        base.runMovement(deltaTime);
    }
    public void HandleCollision(PlayerEntity player)
    {
        if ((Armor == false) && !(playerArmored))
        {
            //the player takes damage
            player.Health = 0;// Player loses health


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

        else
        {
            Armor = false;
            playerArmored = true;
        }
    }
}

