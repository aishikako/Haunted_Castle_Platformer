using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BossEntity : MovingEntity
{
    private bool isAlive = true;
    // the amount of collectibles the player has to collect to defeat the boss
    public int NumCollectibles = 10;
    public static int BossHealth { get; set; }

    private MovingEntityAnimation bossAnimation;

    /// <summary>
    /// Initializes a new SpiderEntity.
    /// </summary>
    public BossEntity(float xPos, float yPos, float width, float height, string color, Vector2 speed, Bounds2? movementBounds)
        : base(xPos, yPos, width, height, color, speed, movementBounds, true, duration: 2)
    {
        type = "boss";
        BossHealth = NumCollectibles;
        bossAnimation = new MovingEntityAnimation(this, 2);
    }

    public void updateHealth()
    {
        BossHealth = GameScreen.bossNumCollectibles;
    }
    public static void retrieveGem()
    {
        BossHealth--;
    }
    public void debugStuff()
    {
        float currentXpos = xPos + collisionBox.Position.X;
        float currentYpos = yPos + collisionBox.Position.Y;
        float currentWidth = collisionBox.Size.X;
        float currentHeight = collisionBox.Size.Y;

        float currentEntityXpos = GameScreen.player.xPos + GameScreen.player.collisionBox.Position.X;
        float currentEntityYpos = GameScreen.player.yPos + GameScreen.player.collisionBox.Position.Y;
        float currentEntityWidth = GameScreen.player.collisionBox.Size.X;
        float currentEntityHeight = GameScreen.player.collisionBox.Size.Y;

        Debug.WriteLine("player");
        Debug.WriteLine("X: " + currentEntityXpos, "Y: " + currentEntityYpos, "Width: " + currentEntityWidth, "Height: " + currentEntityHeight);

        Debug.WriteLine("boss");
        Debug.WriteLine("X: " + currentXpos, "Y: " + currentYpos, "Width: " + currentWidth, "Height: " + currentHeight);
    }
}

