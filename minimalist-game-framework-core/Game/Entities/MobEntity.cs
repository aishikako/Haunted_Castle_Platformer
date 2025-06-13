using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class MobEntity : PhysicsEntity
{
    // Additional mob-specific properties
    public bool IsAlive { get; private set; } = true;

    // Constructor
    public MobEntity(float xPos, float yPos, float width, float height, string color, Vector2 speed, Bounds2? movementBounds, int health =5)
        : base(xPos, yPos, width, height, color, speed, true, movementBounds)
    {
        this.Health = health;
    }

    public MobEntity(float xPos, float yPos, float width, float height, string color, Vector2 speed, Bounds2? movementBounds, int health, float? duration = null) : base(xPos, yPos, width, height, color, speed, true, movementBounds, duration)
    {
        this.Health = health;
    }

    /// <summary>
    /// Reduces the mob's health by the specified damage amount. Triggers OnKill if health reaches zero or below.
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            OnKill?.Invoke();
        }
    }
    protected virtual void Die()
    {
        IsAlive = false;
        SpeedMpS = Vector2.Zero; // Stop movement
    }

    public override void runMovement(float deltaTime)
    {
        if (!IsAlive) return; // Prevent movement if dead
        base.runMovement(deltaTime); // Use PhysicsEntity's movement logic
    }
}

