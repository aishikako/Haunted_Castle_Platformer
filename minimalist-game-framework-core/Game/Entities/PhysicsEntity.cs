 using System;

/// <summary>
/// Represents a mobile entity in the game, such as enemies or NPCs, with health and gravity interactions.
/// </summary>
internal class PhysicsEntity : MovingEntity
{
    // === Properties ===

    /// <summary>Current health of the mob entity.</summary>
    public int Health { get; set; }

    /// <summary>Callback invoked when the mob entity is killed.</summary>
    public Action OnKill { get; set; }

    // === Fields ===

    protected float gravity = 10f * Game.PixelsPerMeter; // Gravity force applied to the mob

    // === Constructor ===

    /// <summary>
    /// Initializes a new mob entity with the specified parameters.
    /// </summary>
    /// <param name="xPos">X-coordinate of the mob's position.</param>
    /// <param name="yPos">Y-coordinate of the mob's position.</param>
    /// <param name="width">Width of the mob in pixels.</param>
    /// <param name="height">Height of the mob in pixels.</param>
    /// <param name="color">Color of the mob.</param>
    /// <param name="speed">Initial speed of the mob in meters per second.</param>
    /// <param name="movementBounds">Bounds within which the mob can move.</param>
    /// <param name="health">Initial health of the mob.</param>
    public PhysicsEntity(float xPos, float yPos, float width, float height, string color, Vector2 speed, bool isActive, Bounds2? movementBounds = null, float? duration = null)
        : base(xPos, yPos, width, height, color, speed, movementBounds, isActive, duration: duration)
    {
    }

    //constructor just for falling chandelier
    public PhysicsEntity(float xPos, float yPos, float width, float height, string color, Vector2 speed, int id)
        : base(xPos, yPos, width, height, color, speed, null, false, duration: null, ID: id)
    {
        type = "chandelier";
        movingDown = true;
        collisionBox = HelperMethods.getAnimationCollisionBox(type);
    }


    // === Methods ===



    /// <summary>
    /// Updates the movement of the mob entity based on its state and collisions.
    /// </summary>
    /// <param name="deltaTime">Time elapsed since the last update in seconds.</param>
    public virtual void runMovement(float deltaTime)
    {
        if(isActive)
        {
            if (movingUp)
            {
                movingDown = false;
                if (!collidingBottom)
                {
                    // Apply gravity to vertical speed
                    SpeedMpS = new Vector2(SpeedMpS.X, SpeedMpS.Y - gravity * deltaTime);
                }
                if (SpeedMpS.Y <= 0)
                {
                    // Transition to falling when upward velocity reaches zero
                    movingUp = false;
                    movingDown = true;
                }
            }
            else if (movingDown)
            {
                if (!collidingBottom)
                {
                    // Apply gravity to vertical speed
                    SpeedMpS = new Vector2(SpeedMpS.X, SpeedMpS.Y - gravity * deltaTime);
                }
                else
                {
                    // Stop downward motion when hitting the ground
                    movingDown = false;
                    yPos = collidedEntityBottom.yPos - Height;
                    SpeedMpS = new Vector2(SpeedMpS.X, 0);
                }
            }

            // Execute base movement logic
            Move(deltaTime);
        }
        
    }

    protected void enableRightMovement()
    {
        if (movingRight && !collidingRight)
        {
            SpeedMpS = new Vector2(SpeedMpSInit.X, SpeedMpS.Y);
        }
    }

    protected void enableLeftMovement()
    {
        if (movingLeft && !collidingLeft)
        {
            SpeedMpS = new Vector2(SpeedMpSInit.X * -1, SpeedMpS.Y);
        }
    }
}
