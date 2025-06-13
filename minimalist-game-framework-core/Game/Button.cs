using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

internal class Button
{
    // Fields
    public bool add { get; set; }    // Specifies if the button adds a screen
    public Screen screen;            // The screen associated with this button

    private Bounds2 dims;
    private string image;             // The visual representation of the button (e.g., path to a texture file)

    // Constructor
    public Button(bool add, string image, Bounds2 dims, Screen screen = null)
    {
        if(screen!=null) this.screen = screen;
        this.add = add;
        this.image = image;
        this.dims = dims;
    }

    //// Set the position of the button as a percentage of the screen size
    //public void SetPosition(float relativeX, float relativeY)
    //{
    //    this.relativePosition = new Vector2(relativeX, relativeY);
    //}

    // Check if the button is clicked
    public bool isClicked()
    {
        float mouseX = Engine.MousePosition.X;
        float mouseY = Engine.MousePosition.Y;
        // Convert relative position to absolute position
        float width = dims.Size.X * Game.Resolution.X;
        float height = dims.Size.Y * Game.Resolution.Y;
        float absoluteX = (dims.Position.X * Game.Resolution.X) - width / 2;
        float absoluteY = (dims.Position.Y * Game.Resolution.Y) - height / 2;

        // Check if the mouse is within the button's bounds
        bool inBounds = mouseX >= absoluteX && mouseX <= absoluteX + width &&
                        mouseY >= absoluteY && mouseY <= absoluteY + height;

        
        
        // Return true if the mouse is in bounds and a click occurred
        return inBounds;

    }

    // Draw the button on the screen
    public void Draw()
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
        
        // Render the button using the provided drawTexture function
        //drawTexture(image, new Vector2(absoluteX, absoluteY), new Vector2(width, height));
    }

    // Update the button and trigger actions if clicked

    public void Update()
    {
        if (isClicked())
        {
            
            
            if (add)
            {
                //add back in once screen manager is added
                Game.screenManager.addScreen(screen);
            }
            else
            {
                Game.screenManager.removeScreen();
            }
        }
    }
}