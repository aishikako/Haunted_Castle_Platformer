using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

/// <summary>
/// Represents an entity capable of movement, such as a player or a mobile enemy.
/// Includes properties for velocity, acceleration, and movement bounds.
/// </summary>
internal class MovingEntity : CollidableEntity
{
    // === Properties ===

    /// <summary>Indicates if the entity is moving to the right.</summary>
    public bool movingRight;

    /// <summary>Indicates if the entity is moving to the left.</summary>
    public bool movingLeft;

    /// <summary>Indicates if the entity is moving upward.</summary>
    public bool movingUp;

    /// <summary>Indicates if the entity is moving downward.</summary>
    public bool movingDown;

    /// <summary>Current velocity of the entity in meters per second.</summary>
    public Vector2 SpeedMpS;

    /// <summary>Initial velocity of the entity in meters per second.</summary>
    public Vector2 SpeedMpSInit { get; set; }

    

    /// <summary>Bounds within which the entity can move.</summary>
    protected Bounds2 MovementBounds { get; set; }

    // === Fields ===

    public bool isActive; // Determines if the entity is currently active
    protected float deceleration = 15f * Game.PixelsPerMeter; // Deceleration rate
    public int ID;
    public bool isLinked = false;

    public MovingEntityAnimation movingEntityAnimation;

    // === Constructor ===

    /// <summary>
    /// Initializes a moving entity with the specified parameters.
    /// </summary>
    public MovingEntity(float xPos, float yPos, float width, float height, string color, Vector2 speedMetersPerSecond, Bounds2? movementBounds, bool isActive, int ID=-1, float? duration = null, string type = "moving")
        : base(xPos, yPos, width, height, color, type)
    {
        SpeedMpSInit = new Vector2(Game.PixelsPerMeter * speedMetersPerSecond.X, Game.PixelsPerMeter * speedMetersPerSecond.Y); //stores the og velocity (max velocity of object)
        SpeedMpS = SpeedMpSInit; //sets current speed to max speed
        this.isActive = isActive; //activates movement if entity should always be moving
        

        
        //setting initial movement directions

        if (movementBounds != null) //stores movement bounds as field if it exists
        {
            MovementBounds = (Bounds2)movementBounds;
            if (isActive) //if entity is active, set initial movements
            {
                movingRight = MovementBounds.Size.X != 0; //starts horizontal movement in the right direction if x-bound is not 0
                movingDown = MovementBounds.Size.Y != 0;   //starts vertical movement in the down direction if x-bound is not 0
            }
        }
        else //sets no initial movement otherwise
        {
            movingRight = false;
            movingDown = false;
            MovementBounds = new Bounds2(new Vector2(-1,-1), new Vector2(-1,-1));
        }

        //left and up is always false at start as if it's active, it moves right and down
        movingLeft = false;
        movingUp = false;

        if (ID != -1)
        {
            this.ID = ID;
            isLinked = true;
        }

        //setup animation if it has a duration
        if (duration != null)
        {
            movingEntityAnimation = new MovingEntityAnimation(this, (float) duration);
        }
    }

    // === Methods ===

    /// <summary>
    /// Updates the movement of the entity based on its velocity and direction.
    /// </summary>
    public void Move(float deltaTime)
    {
        if (isActive)
        {
            checkBounds(deltaTime); // Update state based on bounds and direction if currently moving

            if (movingRight || movingLeft)
            {
                
                xPos += deltaTime * SpeedMpS.X; // Update horizontal position if moving horizontally
                if (!this.type.Equals("player") && this.CheckCollision(GameScreen.player)[1] && collidedEntityTop != null && collidedEntityTop.type == "player" && GameScreen.player.SpeedMpS.X == 0)
                {
                    //Game.player.xPos += deltaTime * SpeedMpS.X;
                    GameScreen.player.xPos += SpeedMpS.X * deltaTime;
                }
            }
            else if (SpeedMpS.X != 0) //makes the horizontal speed jawn go to 0
            {
                if (SpeedMpS.X > 0)
                {
                    SpeedMpS = new Vector2(Math.Max(SpeedMpS.X - deceleration * deltaTime, 0), SpeedMpS.Y); //de-accelerates by subtracting if moving right
                }
                else if (SpeedMpS.X < 0)
                {
                    SpeedMpS = new Vector2(Math.Min(SpeedMpS.X + deceleration * deltaTime, 0), SpeedMpS.Y); //de-accelerates by adding if moving left
                }
                xPos += deltaTime * SpeedMpS.X;
            }

            if (movingUp || movingDown)
            {
                yPos -= deltaTime * SpeedMpS.Y; // Update vertical position if moving vertically
            }
        }
    }

    /// <summary>
    /// Handles collisions and updates direction and velocity based on the collision side.
    /// </summary>
    public void handleCollision(bool[] collisions)
    {
        if (collisions[0]) // Horizontal collision happens
        {
            SpeedMpS = new Vector2(0, SpeedMpS.Y); // Stop horizontal movement

            //stops movement in left and right direction
            movingRight = false; 
            movingLeft = false;
            
            //resets position based on direction of collision
            if (collidingRight)
            {
                

                Bounds2 thisEntityBounds = getCollisionCoords(this);
                Bounds2 collidedEntityBounds = getCollisionCoords(collidedEntityRight);
                Debug.WriteLine(collidedEntityRight.yPos + collidedEntityBounds.Position.X * 1.1f);
                Debug.WriteLine((yPos + thisEntityBounds.Position.Y + thisEntityBounds.Size.X) * 0.9f);
                Debug.WriteLine("");
                if (collidedEntityRight != null && (collidedEntityRight.yPos + collidedEntityBounds.Position.Y * 1.1f) > (yPos + thisEntityBounds.Position.Y + thisEntityBounds.Size.Y) * 0.9f && type.Equals("player") && collidingBottom)
                {
                    
                    yPos = collidedEntityRight.yPos + (collidedEntityBounds.Position.Y - collidedEntityRight.yPos) - thisEntityBounds.Size.Y - (thisEntityBounds.Position.Y - yPos) / 2;
                    collidingRight = false;                    
                }
                else if (collidedEntityLeft != null && (collidedEntityLeft.yPos + collidedEntityBounds.Position.Y * 1.1f) > (yPos + thisEntityBounds.Position.Y + thisEntityBounds.Size.Y) * 0.9f && type.Equals("player") && collidingBottom)
                {
                    yPos = collidedEntityLeft.yPos + (collidedEntityBounds.Position.Y - collidedEntityLeft.yPos) - thisEntityBounds.Size.Y - (thisEntityBounds.Position.Y - yPos) / 2;
                    collidingLeft = false;
                }
                else
                {
                    xPos = collidedEntityRight.xPos + (collidedEntityBounds.Position.X - collidedEntityRight.xPos) - thisEntityBounds.Size.X - (thisEntityBounds.Position.X - xPos);
                   
                }
            }
            else if (collidingLeft)
            {
                Bounds2 thisEntityBounds = getCollisionCoords(this);
                Bounds2 collidedEntityBounds = getCollisionCoords(collidedEntityLeft);

                xPos = collidedEntityLeft.xPos + (collidedEntityBounds.Position.X - collidedEntityLeft.xPos) + collidedEntityBounds.Size.X - (thisEntityBounds.Position.X - xPos);
            }
        }
        if (collisions[1]) // Vertical collision
        {
            // Stop vertical movement
            SpeedMpS = new Vector2(SpeedMpS.X, 0); 
            movingUp = false;

            //resets y-position based on direction of collision
            if (collidingTop)
            {
                
                movingDown = true; //starts falling
                yPos = collidedEntityTop.yPos + collidedEntityTop.Height;
            }
            else if (collidingBottom)
            {
                
                movingDown = false; //stops falling
                

                Bounds2 thisEntityBounds = getCollisionCoords(this);
                Bounds2 collidedEntityBounds = getCollisionCoords(collidedEntityBottom);

                yPos = collidedEntityBottom.yPos + (collidedEntityBounds.Position.Y - collidedEntityBottom.yPos)  - thisEntityBounds.Size.Y - (thisEntityBounds.Position.Y - yPos) / 2;
            }  
        }
    }

    /// <summary>
    /// Updates the movement state of the entity based on its bounds.
    /// </summary>
    private void checkBounds(float deltaTime)
    {
        if (MovementBounds.Position.X == -1)
        {
            return;
        }
        if (movingRight && xPos + Width + SpeedMpS.X * deltaTime > MovementBounds.Position.X + MovementBounds.Size.X) //checks if about to leave bounds in the right direction
        {
            xPos = MovementBounds.Position.X + MovementBounds.Size.X - Width; //sets position to end of bounds in the right direction
            ToggleDirection(ref movingRight, ref movingLeft, ref SpeedMpS.X); //switches direction from right to left and switches x velocity
        }
        else if (movingLeft && xPos - SpeedMpS.X * deltaTime < MovementBounds.Position.X) //checks if about to leave bounds in the left direction
        {
            xPos = MovementBounds.Position.X; //sets position to end of bounds in the left direction
            ToggleDirection(ref movingLeft, ref movingRight, ref SpeedMpS.X); //switches direction from left to right and switches x velocity
        } 

        if (movingDown && yPos + Height + SpeedMpS.Y * deltaTime > MovementBounds.Position.Y + MovementBounds.Size.Y) //checks if about to leave bounds in the down direction
        {
            yPos = MovementBounds.Position.Y + MovementBounds.Size.Y - Height; //sets position to end of bounds in the down direction
            ToggleDirection(ref movingDown, ref movingUp, ref SpeedMpS.Y); //switches direction from down to up and switches y velocity
        }
        else if (movingUp && yPos - SpeedMpS.Y * deltaTime < MovementBounds.Position.Y) //checks if about to leave bounds in the up direction
        {
            yPos = MovementBounds.Position.Y; //sets position to end of bounds in the up direction
            ToggleDirection(ref movingUp, ref movingDown, ref SpeedMpS.Y); //switches direction from up to down and switches y velocity
        }
    }

    /// <summary>
    /// Toggles the movement direction and reverses velocity along the specified axis (when about to leave bounds).
    /// </summary>
    private void ToggleDirection(ref bool currentDirection, ref bool oppositeDirection, ref float speedComponent)
    {
        currentDirection = false; //stops movement in current direction
        oppositeDirection = true; //starts movement in opposite direction
        speedComponent *= -1; //flips speed in a given axis
        if (isLinked)
        {
            isActive = false;
            
        }
    }

    //iterating through levers, and linking corresponding one
    public void addLinkedEntity(Dictionary<string, List<Entity>> EntityLayers)
    {
        foreach (leverButtonEntity entity in EntityLayers["items"].OfType<leverButtonEntity>())
        {
            if (entity.ID == ID-1)
            {
                //entity.linkedEntity = this;
                entity.LinkedEntities.Add(this);
                isActive = false;
                entity.isActive = false;
            }
        }
    }
}
