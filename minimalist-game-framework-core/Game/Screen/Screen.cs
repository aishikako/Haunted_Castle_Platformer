using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

internal class Screen: IScreen
{
    public bool isPopup;
    public List<Button> buttons;
    protected Bounds2 dims;
    protected string image;
    public Screen(List<Button> buttons, bool popup, string image, Bounds2 bounds)
    {
        this.isPopup = popup;
        this.image = image;
        this.dims = bounds;
        this.buttons = buttons;
    }

    public virtual void Update() 
    {
        if (Engine.GetMouseButtonDown(MouseButton.Left) )
        {
            foreach (var button in buttons)
            {
                button.Update();
            }
        }
        
    }

    public virtual void Draw()
    {
        // Convert relative position to absolute position
        float width = dims.Size.X * Game.Resolution.X;
        float height = dims.Size.Y * Game.Resolution.Y;
        float absoluteX = (dims.Position.X * Game.Resolution.X) - width / 2; //aligning the center to topLeft for drawing
        float absoluteY = (dims.Position.Y * Game.Resolution.Y) - height / 2; //aligning the center to topRight for drawing

        Vector2 posVector = new Vector2(absoluteX, absoluteY);
        Vector2 sizeVector = new Vector2(width, height);

        Engine.DrawTexture(
            texture: Textures.textures[image],
            position: posVector,
            size: sizeVector,
            rotation: 0 // Add rotation if needed
        );

        // draw buttons
        if (buttons != null)
        {
            DrawButtons();
        }
        
    }

    protected void DrawButtons()
    {
        foreach (var button in buttons)
        {
            button.Draw();
        }
    }
}
