using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


internal class BackgroundEntity: Entity
{
    private readonly float cameraSpeed;
    private string type;
    /// <summary>
    /// Initializes an BackgroundEntity starting at top left with the specified height and scroll speed.
    /// </summary>
    public BackgroundEntity(float width, float height, float cameraSpeed, string image) : base(0, 0, width, height, image)
    {
        this.color = image;
        this.cameraSpeed = cameraSpeed;
    }

    // Used to display leaderboard
    public BackgroundEntity(float width, float height, string image) : base(0, 0, width, height, image)
    {
        this.color = image;
    }

    public BackgroundEntity(float xPos, float yPos, float width, float height, string color, string type) : base(xPos, yPos, width, height, color, type)
    {
        this.type = type;
    }

    /// <summary>
    /// Draws the entity at the adjusted position based on camera offsets and repeats it
    /// </summary>
    public void drawEntity()
    {
        Vector2 sizeVector = new Vector2(Width, Height);

        // Find the horizontal position where the texture starts and translate to camera position
        Vector2 posVector = new Vector2(xPos, yPos);
        posVector = new Vector2(posVector.X % Width, yPos);
        if (posVector.X > 0) posVector = new Vector2(posVector.X -= Width, yPos);

        //iterates through camera screen, and draws the image at each width
        for (float x = posVector.X; x < Game.Resolution.X; x += Width)
        {
            Vector2 tempPosVector = new Vector2(x, yPos);
            if (!color.Equals(Color.Transparent))
            {
                Engine.DrawTexture(Textures.textures[color], tempPosVector, size: sizeVector);
            }
        }
    }

    /// <summary>
    /// Draws the entity at the adjusted position based on camera offsets and repeats it
    /// </summary>
    public override void drawEntity(float xCamOffset, float yCamOffset)
    {
        Vector2 sizeVector = new Vector2(Width, Height);

        // Find the horizontal position where the texture starts and translate to camera position
        Vector2 posVector = new Vector2(xPos - xCamOffset * cameraSpeed, yPos);
        posVector = new Vector2(posVector.X % Width, yPos);
        if (posVector.X > 0) posVector = new Vector2(posVector.X -=Width, yPos);

        //iterates through camera screen, and draws the image at each width
        for (float x = posVector.X; x < Game.Resolution.X; x += Width)
        {
            Vector2 tempPosVector = new Vector2(x, yPos);
            if (!color.Equals(Color.Transparent))
            {
                Engine.DrawTexture(Textures.textures[color], tempPosVector, size: sizeVector);
            }
        }
    }
}
