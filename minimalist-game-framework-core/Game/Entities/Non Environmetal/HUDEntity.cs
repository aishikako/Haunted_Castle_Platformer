using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


internal class HUDEntity : Entity
{
    public static Font font = Engine.LoadFont("swirly-font.ttf", 50);
    public static string disp = "";
    private int initialPuzzleTimer = 0;

    /// <summary>
    /// Initializes background entities for HUD text boxes
    /// </summary>
    /// float xPos, float yPos, float width, float height, string text, string image, float? cameraSpeed = null, string type)
    public HUDEntity(float xPos, float yPos, float width, float height, string typeOfStat, string image, float? cameraSpeed = null) : base(xPos, yPos, width, height, image)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.color = image;
        this.text = typeOfStat;
    }

    /// <summary>
    /// Draws the entity on screen without any camera offset (position never changes on player screen)
    /// </summary>
    public override void drawEntity(float xCamOffset, float yCamOffset)
    {
        dispText(text);
    }

    public void dispText(string text)
    {

        Vector2 sizeVector = new Vector2(Width * 2, Height);
        Vector2 posVector = new Vector2(xPos, yPos);

        int TimeInt = (int)Time;
        HelperMethods.getDisplayHUD(GameScreen.playerBackend.Score, GameScreen.playerBackend.Gems, Level, Stars, Armor, TimeInt, text);
        Vector2 textPosVector = new Vector2(posVector.X + sizeVector.X / 2, posVector.Y);

        if (disp == "Health" || disp == "Boss Health")
        {
            dispHealth(sizeVector, posVector);
        }

        else if (disp == "PUZZLETIME")
        {
            dispPuzzleCountdown(sizeVector, posVector);
        }

        else if (disp == "ARMOR")
        {
            dispArmor("pickup_shield", sizeVector, posVector);
        }

        else
        {
            if (disp == "Armor")
            {
                // armor text is blue
                Engine.DrawString(disp, textPosVector, Color.RoyalBlue, font, TextAlignment.Center, false);
            }

            else
            {
                Engine.DrawString(disp, textPosVector, Color.AntiqueWhite, font, TextAlignment.Center, false);
            }
        }
    }

    public void dispPuzzleCountdown(Vector2 sizeVector, Vector2 posVector)
    {
        if (initialPuzzleTimer == 0)
        {
            initialPuzzleTimer = (int)Time;
        }

        // countdown from 60
        int countdown = (int)(60 - (Time - initialPuzzleTimer));

        Vector2 textPosVector = new Vector2(posVector.X + sizeVector.X / 2, posVector.Y);

        // if player has 10 seconds to finish the puzzle
        if (countdown <= 10)
        {
            // display text in red
            Engine.DrawString(countdown.ToString(), textPosVector, Color.Red, font, TextAlignment.Center, false);
        }

        else
        {
            Engine.DrawString(countdown.ToString(), textPosVector, Color.AntiqueWhite, font, TextAlignment.Center, false);
        }
    }

    public void dispArmor(String armorTexture, Vector2 sizeVector, Vector2 posVector)
    {
        // Draw the entity using the current color and opacity
        Engine.DrawTexture(
            texture: (texture == null) ? Textures.textures[armorTexture] : texture,
            position: posVector,
            size: sizeVector,
            rotation: 0 // Add rotation if needed
        );
    }
    public void dispHealth(Vector2 sizeVector, Vector2 posVector)
    {
        if (disp == "Health" || (disp == "Boss Health" && GameScreen.playerBackend.Level == 4))
        {
            posVector.Y += sizeVector.Y;
            Vector2 textPosVector = new Vector2(posVector.X + sizeVector.X / 2, posVector.Y);
            // disp is either Health or Boss Health
            Engine.DrawString(disp, textPosVector, Color.AntiqueWhite, font, TextAlignment.Center, false);

            posVector.X += 2 * sizeVector.X;
            posVector.Y += (float)(sizeVector.Y * 0.25);
            sizeVector.Y *= (float)(0.75);

            Bounds2 HealthBar = new Bounds2(new Vector2(posVector.X - sizeVector.X / 2, posVector.Y), new Vector2(sizeVector.X * Health, sizeVector.Y));

            if (disp == "Boss Health")
            {
                HealthBar = new Bounds2(new Vector2(posVector.X - sizeVector.X / 2, posVector.Y), new Vector2(sizeVector.X * BossEntity.BossHealth / 5, sizeVector.Y));
            }

            Engine.DrawRectSolid(HealthBar, Color.PaleVioletRed);
        }
    }
}