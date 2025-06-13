using System;
using System.Diagnostics;
using System.Numerics;

/// <summary>
/// Represents the player character in the game, with specialized movement and interaction mechanics.
/// </summary>
internal class PlayerEntity : MobEntity
{
    // === Fields ===

    private static Vector2 initialSpeed = new Vector2(2.5f, 5f); // Default player speed
    public bool isWallJumping; // Indicates if the player is performing a wall jump
    private bool activateWallJumping;

    public bool isSliding; //Indicates if the player is sliding
    private bool activateSliding;

    private bool jumpHeld = false;
    private float jumpHeldTime = 0f;

    public bool onLadder = false;

    public PlayerEntityAnimation playerEntityAnimation;

    // === Constructor ===

    public Vector2 Position { get; internal set; }

    /// <summary>
    /// Initializes a new player entity with the specified parameters.
    /// </summary>
    public PlayerEntity(float xPos, float yPos, float width, float height, string color, int health, float duration)
        : base(xPos, yPos, width, height, color, initialSpeed, null, health: 4, duration: duration)
    {
        playerEntityAnimation = new PlayerEntityAnimation(this, duration);
        type = "player";
    }

    // === Methods ===

    /// <summary>
    /// Updates the player's movement states based on key inputs and collisions.
    /// </summary>
    public void updateStates()
    {
        if (Health <= 0)
        {
            movingLeft = false;
            movingRight = false;
            movingUp = false;
            movingDown = false;
            SpeedMpS = Vector2.Zero; // Stop all movement
            return; // Skip further updates if health is 0
        }
        if (!isWallJumping && !isSliding) //lock horizontal movement when in wall jump state
        {
            // Update movement directions based on key inputs
            movingRight = (Engine.GetKeyHeld(Key.D) || Engine.GetKeyHeld(Key.Right)) && !collidingRight;
            movingLeft = (Engine.GetKeyHeld(Key.A) || Engine.GetKeyHeld(Key.Left)) && !collidingLeft;
        }
        
        // Sound effect logic
        if (collidingLeft || movingUp)
        {
            if (collidingLeft) GameScreen.music.playSoundEffect("chime.wav");
            else GameScreen.music.playSoundEffect("whimsical-chime.wav");

            GameScreen.music.SoundPlayed = true;
        }

        else
        {
            GameScreen.music.SoundPlayed = false;
        }

        if (((collidingRight) || (collidingLeft )))
        {
            if (((collidedEntityLeft != null && collidedEntityLeft.type.Equals("pinkwall")) || (collidedEntityRight != null && collidedEntityRight.type.Equals("pinkwall")) && !collidingBottom))
            {
                // Apply wall sliding mechanics
                if (!movingUp) //only decrease gravity if falling
                {
                    gravity = 1f * Game.PixelsPerMeter;
                }
                
                activateWallJumping = Engine.GetKeyHeld(Key.W) && ((collidingLeft && Engine.GetKeyHeld(Key.D) || (collidingRight && Engine.GetKeyHeld(Key.A)))); // Check for wall jump input
            }
        }
        else if ((!movingUp && !movingDown) && Engine.GetKeyDown(Key.LeftControl))
        {
           
            if (!isSliding)
            {
                activateSliding = true;
            }
        }
        else
        {
            // Reset gravity and state when not wall sliding
            gravity = 10f * Game.PixelsPerMeter;
            //color = "yellow";
            activateWallJumping = false;

            deceleration = 20f * Game.PixelsPerMeter;
        }

        if ((Engine.GetKeyHeld(Key.W) || Engine.GetKeyHeld(Key.Up)) && collidingBottom) //normal jump
        {
            movingUp = true;
            collidingBottom = false; // Reset collision state
            SpeedMpS = new Vector2(SpeedMpS.X, SpeedMpSInit.Y * 0.75f); // Apply initial jump velocity
        }

        if (Engine.GetKeyUp(Key.Space) && collidingBottom) //long jump
        {
            float normalJumpHeldTime = 0.5f;
            float maxJumpHeldTimeFactor = 1f;
            float minVelocityFactor = 0.5f;

            float jumpVelocityFactor = Math.Max((Math.Min(jumpHeldTime, normalJumpHeldTime * maxJumpHeldTimeFactor) / normalJumpHeldTime), minVelocityFactor);

            jumpHeldTime = 0;

            movingUp = true;
            collidingBottom = false; // Reset collision state
            SpeedMpS = new Vector2(SpeedMpS.X, SpeedMpSInit.Y * jumpVelocityFactor); // Apply initial jump velocity

            Move(Engine.TimeDelta);
        }

        if (Engine.GetKeyHeld(Key.Space) && collidingBottom)
        {
            jumpHeldTime += Engine.TimeDelta;
        }


        onLadder = ((collidedEntityLeft != null && collidedEntityLeft.type.Equals("vines")) || (collidedEntityRight != null && collidedEntityRight.type.Equals("vines")));

        movingDown = !collidingBottom; // Update falling state based on collisions

        enableRightMovement();
        enableLeftMovement();

    }

    /// <summary>
    /// Resets collision states for the player.
    /// </summary>
    public void resetStates()
    {
        collidingTop = false;
        collidingBottom = false;
        collidingRight = false;
        collidingLeft = false;
    }

    /// <summary>
    /// Overrides the base movement logic to handle wall jumping mechanics.
    /// </summary>
    public override void runMovement(float deltaTime)
    {
        if (GameScreen.playerBackend.isSpeedBoost)
        {
            SpeedMpSInit = new Vector2(3.5f*Game.PixelsPerMeter, SpeedMpSInit.Y);
        }
        if (GameScreen.playerBackend.isJumpBoost)
        {
            SpeedMpSInit = new Vector2(SpeedMpSInit.X, 6.5f * Game.PixelsPerMeter);
        }
        if (Health <= 0)
        {
            SpeedMpS = Vector2.Zero; // Stop movement
            return; // Skip further movement updates if health is 0
        }
        if (activateWallJumping)
        {
            isWallJumping = true;
            deceleration = 1f * Game.PixelsPerMeter;
            // Perform wall jump
            movingUp = true;
            movingDown = false;

            if (collidingLeft)
            {
                SpeedMpS = new Vector2(SpeedMpSInit.X, SpeedMpSInit.Y); // Jump away from the left wall
                movingRight = true;
                movingLeft = false;
            }
            else
            {
                SpeedMpS = new Vector2(SpeedMpSInit.X * -1, SpeedMpSInit.Y); // Jump away from the right wall
                movingLeft = true;
                movingRight = false;
            }

            Move(deltaTime); // Execute movement
        }
        else if (isWallJumping)
        {
            if (collidingRight || collidingLeft || collidingBottom)
            {
                SpeedMpS = new Vector2(0, SpeedMpS.Y);
                movingLeft = false;
                movingRight = false;
                movingUp = false;
                movingDown = false;

                isWallJumping = false;
                deceleration = 20f * Game.PixelsPerMeter;
                gravity = 1f * Game.PixelsPerMeter;
            }
            deceleration = 1f * Game.PixelsPerMeter;
            base.runMovement(deltaTime);
        }
        else if (activateSliding)
        {
            
            SpeedMpS = new Vector2(SpeedMpS.X * 3, SpeedMpS.Y);
            isSliding = true;
            activateSliding = false;
            movingRight = false;
            movingLeft= false;
            yPos -= Height/7;

        }
        else if (isSliding)
        {
            deceleration = 0f ;
            activateSliding = false;

            base.runMovement(deltaTime); // Default movement behavior

            if (SpeedMpS.X == 0)
            {
                isSliding = false;
            }
        }
        else if (onLadder)
        {
            if (Engine.GetKeyHeld(Key.A) || Engine.GetKeyHeld(Key.D) || ((!collidingLeft && !collidingRight && !collidingBottom)))
            {
                onLadder = false;
                movingDown = true;
                collidedEntityLeft = null;
                collidedEntityRight = null;
            }

            else if (Engine.GetKeyHeld(Key.W))
            {
                movingUp = true;
                movingDown=false;
                SpeedMpS = new Vector2(0, SpeedMpSInit.Y * 0.3f);
            }
            else if (Engine.GetKeyHeld(Key.S))
            {
                movingDown = true;
                movingUp = false;
                SpeedMpS = new Vector2(0, SpeedMpSInit.Y * -0.3f);
                Debug.WriteLine("onladder");
            }
            
            else
            {
                SpeedMpS = new Vector2(0, 0);
            }
            
            Move(deltaTime);
        }
        else
        {
            base.runMovement(deltaTime); // Default movement behavior
        }
    }

    /// <summary>
    /// Debugging function to log the player's current movement states.
    /// </summary>
    public void checkState()
    {
        System.Diagnostics.Debug.WriteLine(String.Format("x: {0}, y: {1}", xPos - Camera.boundx, yPos - Camera.boundy));
    }
}
