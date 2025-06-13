using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// directions for which way the camera is moving
enum Direction
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

//camera class has a set of bounds and size that determines what portion of the screen is shown
internal class Camera
{
    //bounds represent the top right corner of camera
    public static float boundx = 0;
    public static float boundy = 0;
    private static float width = Game.Resolution.X;
    private static float height = Game.Resolution.Y;

    private static float speedMpS = 0.8f;//0.5f;
    private static float verticalScale = 7 / 10;
    private static float initialCameraSpeed = 0.8f;
    private static int initialNumEntities = 0;
    private static bool initialized = false;

    public static void UpdateBounds(Direction direction, float deltaTime, float xPos, float maxPosX)
    {
        float relativePosX = xPos - boundx;
        // autoscroll speed is proportional to the player's x position on the screen
        double speedRatio = (double)(MathF.Pow((float)((relativePosX + maxPosX * 0.1) / (maxPosX / 1.5)), (float)2));
        if (speedRatio > .5)
        {
            speedMpS = (float)(speedRatio * initialCameraSpeed);
        }
        else
        {
            if( (float)(speedRatio * initialCameraSpeed) < 1.2){
                speedMpS = 1.2f;
            }
            else
            {
                speedMpS = (float)(speedRatio * initialCameraSpeed);
            }
        }

        UpdateBounds(direction, deltaTime);
    }

    //changing the camera position based on what key pressed
    public static void UpdateBounds(Direction direction, float deltaTime)
    {
        float cameraSpeed = speedMpS * Game.PixelsPerMeter * deltaTime;

        switch (direction)
        {
            case Direction.UP:
                boundy -= cameraSpeed;
                break;
            case Direction.DOWN:
                boundy += cameraSpeed;
                break;
            case Direction.LEFT:
                boundx -= cameraSpeed;
                if (boundx < 0) boundx = 0;
                break;
            case Direction.RIGHT:
                boundx += cameraSpeed;
                break;
        }
    }

    //changing the camera position based on what key pressed
    public static void UpdateBoundsFollowingPlayer(Direction direction, float pos)
    {
        switch (direction)
        {
            case Direction.UP:
                boundy = -7 * height / 10 + pos;
                break;
            case Direction.DOWN:
                boundy = pos;
                break;
            case Direction.LEFT:
                boundx = pos;
                if (boundx < 0) boundx = 0;
                break;
            case Direction.RIGHT:
                boundx = pos;
                break;
        }
    }

    // returns if an entity is within the camera bounds
    public static bool entityInBounds(Entity entity)
    {
        if (entity is BackgroundEntity || entity is HUDEntity) return true;
        return (boundx < entity.xPos + entity.Width && boundx + width > entity.xPos &&
                boundy < entity.yPos + entity.Height && boundy + height > entity.yPos);
    }
}