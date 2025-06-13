using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


internal class ScoreboardScreen : Screen
{
    private LevelDisplay levelDisplay = new LevelDisplay();
    private Vector2 Resolution = Game.Resolution;
    public ScoreboardScreen(List<Button> buttons, bool popup, string image, Bounds2 bounds) :
        base(buttons, popup, image, bounds)
    {
    }

    public override void Draw()
    {
        base.Draw();
        LoadLeaderboard();
    }

    public void LoadLeaderboard()
    {
        levelDisplay.DisplayStats(new Vector2(Game.Resolution.X * dims.Size.X, Game.Resolution.Y * dims.Size.Y));
    }

}
