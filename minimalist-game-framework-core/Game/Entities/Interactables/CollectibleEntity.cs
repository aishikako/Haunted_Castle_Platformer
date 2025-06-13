using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents an entity that detects collision and can be collected (gem).
/// </summary>
internal class CollectibleEntity: Entity, IInteractables
{
    public CollectibleEntity(float xPos, float yPos, float width, float height, string color, string type)
        : base(xPos, yPos, width, height, color, type)
    {
    }

    /// <summary>
    /// Checks type of object, and does an interaction depending on type.
    /// </summary>
    public void Interact()
    {
        switch (type)
        {
            //gems increase players backend stats
            case "gem":
                var bounceAnimation = new CollectibleEntityAnimation(this, duration: 1f, amplitude: 10f, frequency: 2f);
                bounceAnimation.Start();
                GameScreen.playerBackend.retrieveGem();
                GameScreen.levelDisplay.allEntities["items"].Remove(this);
                GameScreen.music.playSoundEffect("chime.wav");
                if (GameScreen.playerBackend.Level == 4)
                {
                    GameScreen.bossNumCollectibles--;
                }

                break;
            case "armor":
                var armorBounceAnimation = new CollectibleEntityAnimation(this, duration: 1f, amplitude: 10f, frequency: 2f);
                armorBounceAnimation.Start();
                GameScreen.playerBackend.retrieveArmor();
                GameScreen.levelDisplay.allEntities["items"].Remove(this);
                GameScreen.music.playSoundEffect("chime.wav");
                if (GameScreen.playerBackend.Level == 4)
                {
                    GameScreen.bossNumCollectibles--;
                }

                break;
            case "door":
                // End the level if a door is reached
                GameScreen.HandlePuzzles();
                GameScreen.endOfLevel = true;
                break;
            case "puzzledoor":
                // puzzles are like mini-levels within a level
                // puzzle levels are the same number as normal levels +10
                // e.g. the puzzle for level 3 returns 13 for playerBackend.Level
                GameScreen.endOfLevel = true;
                GameScreen.HandlePuzzles();
                break;
            case "jump":
                GameScreen.levelDisplay.allEntities["items"].Remove(this);
                GameScreen.playerBackend.isJumpBoost = true;

                // if boss level
                if (GameScreen.playerBackend.Level == 4)
                {
                    GameScreen.bossNumCollectibles--;
                }

                break;
            case "speed":
                GameScreen.levelDisplay.allEntities["items"].Remove(this);
                GameScreen.playerBackend.isSpeedBoost = true;

                if (GameScreen.playerBackend.Level == 4)
                {
                    GameScreen.bossNumCollectibles--;
                }

                break;
        }

    }

    public void IsCollided(PlayerEntity player)
    {
        float tolerance = 0.001f;
        bool isXOverlap = (xPos + Width + tolerance > player.xPos && xPos - tolerance < player.xPos + player.Width);
        bool isYOverlap = (yPos + Height + tolerance > player.yPos && yPos - tolerance < player.yPos + player.Height);
        if (isXOverlap && isYOverlap)
        {
            Interact();
        }
    }
}
