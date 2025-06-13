using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class LevelUpScreen : Screen, IScreen
{
    public LevelUpScreen(List<Button> buttons, bool popup, string image, Bounds2 bounds) :
        base(buttons, popup, image, bounds)
    {
    }

    public override void Draw()
    {
        Debug.WriteLine(GameScreen.playerBackend.Time);
        Debug.WriteLine(GameScreen.playerBackend.Level);
        base.Draw();
        bool isCurrLevelPuzzle = (GameScreen.Autoscroll);
        int numStarsToShow;

        if (isCurrLevelPuzzle) //make it so it shows the current number of stars based on time in puzzle
        {
            float currentPlayerTime = GameScreen.playerBackend.puzzleTime;
            
            int numStarsLost = (int) currentPlayerTime / 10;

            numStarsToShow = int.Max(0, 5- numStarsLost);
        }
        else
        {
            numStarsToShow = GameScreen.playerBackend.Gems;
        }

        BackendPlayerEntity.Stars = numStarsToShow;
        drawStars(numStarsToShow);
    }

    private void drawStars(int numStars)
    {
        float width = dims.Size.X * Game.Resolution.X;
        float height = dims.Size.Y * Game.Resolution.Y;
        float absoluteX = Game.Resolution.X * 0.15f; 
        float absoluteY = (Game.Resolution.Y) * 0.15f;

        Vector2 starSizeVector = new Vector2(width / 7 * 0.9f,  width / 7 * 0.9f);

        for (int i = 0; i  < numStars; i++)
        {
            Engine.DrawTexture(
            texture: Textures.textures["starfilled"],
            position: new Vector2(absoluteX + i * width/7, absoluteY),
            size: starSizeVector,
            rotation: 0 // Add rotation if needed
        );
        }
        for (int i = numStars; i < 5; i++)
        {
            Engine.DrawTexture(
            texture: Textures.textures["starnotfilled"],
            position: new Vector2(absoluteX + i * width / 7, absoluteY),
            size: starSizeVector,
            rotation: 0 // Add rotation if needed
        );
        }
    }
}
