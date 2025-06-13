using System;
using System.Diagnostics;

/// <summary>
/// Represents an entity with collision detection capabilities, extending the base entity class.
/// </summary>
internal class CollidableEntity : Entity
{
    // === Properties ===

    /// <summary>Indicates collision on the top side of the entity.</summary>
    protected bool collidingTop { get; set; }

    /// <summary>Indicates collision on the bottom side of the entity.</summary>
    protected bool collidingBottom { get; set; }

    /// <summary>Indicates collision on the left side of the entity.</summary>
    protected bool collidingLeft { get; set; }

    /// <summary>Indicates collision on the right side of the entity.</summary>
    protected bool collidingRight { get; set; }

    /// <summary>Entity that collided on the right side.</summary>
    public CollidableEntity collidedEntityRight { get; set; }

    /// <summary>Entity that collided on the left side.</summary>
    public CollidableEntity collidedEntityLeft { get; set; }

    /// <summary>Entity that collided on the bottom side.</summary>
    public CollidableEntity collidedEntityBottom { get; set; }

    /// <summary>Entity that collided on the top side.</summary>
    public CollidableEntity collidedEntityTop { get; set; }
    public bool IsAnimating { get; internal set; }

    public Bounds2 collisionBox;

    // === Constructor ===

    /// <summary>
    /// Initializes a collidable entity with the specified parameters.
    /// </summary>
    public CollidableEntity(float xPos, float yPos, float width, float height, string color, string type)
        : base(xPos, yPos, width, height, color, type)
    {
        collisionBox = HelperMethods.getAnimationCollisionBox(type);
    }

    // === Methods ===

    /// <summary>
    /// Checks for a collision between this entity and another entity.
    /// </summary>
    public bool[] CheckCollision(CollidableEntity entity)
    {
        
        float currentXpos = getCollisionCoords(this).Position.X;
        float currentYpos = getCollisionCoords(this).Position.Y;
        float currentWidth = getCollisionCoords(this).Size.X;
        float currentHeight = getCollisionCoords(this).Size.Y;

        float currentEntityXpos = getCollisionCoords(entity).Position.X;
        float currentEntityYpos = getCollisionCoords(entity).Position.Y;
        float currentEntityWidth = getCollisionCoords(entity).Size.X;
        float currentEntityHeight = getCollisionCoords(entity).Size.Y;



        float tolerance = 0.001f; // Tolerance factor for collision detection

        // Determine if there is overlap on the X and Y axes
        bool isXOverlap = (currentXpos + currentWidth + tolerance > currentEntityXpos && currentXpos - tolerance < currentEntityXpos + currentEntityWidth);
        bool isYOverlap = (currentYpos + currentHeight + tolerance > currentEntityYpos && currentYpos - tolerance < currentEntityYpos + currentEntityHeight);

        if (isXOverlap && isYOverlap)
        {
            Debug.WriteLine($"Collision detected between {this.type} and {entity.type}");
            // Calculate the amount of overlap in both directions
            float xOverlap = Math.Min(currentXpos + currentWidth, currentEntityXpos + currentEntityWidth) - Math.Max(currentXpos, currentEntityXpos) + tolerance;
            float yOverlap = Math.Min(currentYpos + currentHeight, currentEntityYpos + currentEntityHeight) - Math.Max(currentYpos, currentEntityYpos);

            // Determine which side the collision occurred on
            if (yOverlap < xOverlap) // More overlap in X direction
            {
                if (currentYpos + currentHeight <= currentEntityYpos + yOverlap) // Collision from the bottom
                {
                    collidingBottom = true;
                    collidedEntityBottom = entity;

                    
                }
                else if (currentYpos >= currentEntityYpos + currentEntityHeight - yOverlap) // Collision from the top
                {
                    collidingTop = true;
                    collidedEntityTop = entity;
                }
                return new bool[] { false, true };
            }
            else // More overlap in Y direction
            {
                if (currentXpos + currentWidth <= currentEntityXpos + xOverlap) // Collision from the left
                {
                    collidingRight = true;
                    collidedEntityRight = entity;
                }
                else if (currentXpos >= currentEntityXpos + currentEntityWidth - xOverlap) // Collision from the right
                {
                    collidingLeft = true;
                    collidedEntityLeft = entity;
                }
                return new bool[] { true, false };
            }
        }

        return new bool[] { false, false }; // No collision detected


    }

    public static Bounds2 getCollisionCoords(CollidableEntity entity)
    {
        float currentEntityXpos = entity.xPos + entity.collisionBox.Position.X;
        float currentEntityYpos = entity.yPos + entity.collisionBox.Position.Y;
        float currentEntityWidth = (entity.collisionBox.Size.X == 0) ? entity.Width : entity.collisionBox.Size.X;
        float currentEntityHeight = (entity.collisionBox.Size.Y == 0) ? entity.Height : entity.collisionBox.Size.Y;

        return new Bounds2(new Vector2(currentEntityXpos, currentEntityYpos), new Vector2(currentEntityWidth, currentEntityHeight));
    }

    private bool withinX(Bounds2 bound1, Bounds2 bound2)
    {
        return (bound1.Position.X > bound2.Position.X && bound1.Position.X < bound2.Position.X + bound2.Size.X) ||
            (bound1.Position.X + bound1.Size.X > bound2.Position.X && bound1.Position.X + bound1.Size.X < bound2.Position.X + bound2.Size.X);
    }

    private bool withinY(Bounds2 bound1, Bounds2 bound2)
    {
        return (bound1.Position.Y > bound2.Position.Y && bound1.Position.Y < bound2.Position.Y + bound2.Size.Y) ||
            (bound1.Position.Y + bound1.Size.Y > bound2.Position.Y && bound1.Position.Y + bound1.Size.Y < bound2.Position.Y + bound2.Size.Y);
    }

}
