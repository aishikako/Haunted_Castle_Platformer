using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;  // Make sure to include this for the Color type

// Define the abstract base class for animations
internal abstract class Animations
{
    protected bool isAnimating;

    public bool IsAnimating => isAnimating;

    public abstract void StartAnimation(Entity entity, float duration, string color = null);
    public abstract void UpdateAnimation(Entity entity, float deltaTime);

    public virtual void StopAnimation(Entity entity)
    {
        isAnimating = false;
        // Additional cleanup or reset logic here
    }
}

// Separate class for flash animation
internal class FlashAnimation : Animations
{
    private float timer;
    private float flashInterval = 0.1f; // Time in seconds for each toggle
    private float nextToggleTime;
    private string originalColor;
    private string flashColor;

    public FlashAnimation(string flashColor, float duration)
    {
        this.flashColor = flashColor;
        this.timer = duration;
        this.nextToggleTime = flashInterval; // Initialize next toggle time
    }

    public override void StartAnimation(Entity entity, float duration, string color)
    {
        base.isAnimating = true;
        originalColor = entity.color; 
        entity.color = flashColor; 
    }

    public override void UpdateAnimation(Entity entity, float deltaTime)
    {
        if (!isAnimating) return;  // Correct early exit if animation is not active

        timer -= deltaTime;        // Decrement the overall animation timer
        nextToggleTime -= deltaTime; // Decrement the toggle timer

        // Check if it's time to toggle the color
        if (nextToggleTime <= 0)
        {
            // Toggle the color between original and flash
            entity.color = entity.color == originalColor ? flashColor : originalColor;
            nextToggleTime = flashInterval; // Reset the next toggle time for consistent flashing
           
        }

        // Check if the animation duration has expired
        if (timer <= 0)
        {
            StopAnimation(entity); // Stop the animation if the time is up
        }
    }

    public override void StopAnimation(Entity entity)
    {
        base.StopAnimation(entity);
        entity.color = originalColor; // Reset the color when animation stops
    }
}


internal class MovingEntityAnimation 
{

    MovingEntity entity;
    float timer;
    float duration;

    bool prevUp = false;
    bool prevDown = false;
    bool prevRight = false;
    bool prevLeft = false;
    

    public MovingEntityAnimation(MovingEntity entity, float duration)
    {
        this.entity = entity;
        this.duration = duration;
        timer = 0;
    }

    public void Animate()
    {
        string animationFileName = null;

        if (entity.movingDown) //moving down animation
        {
            String animationPath = HelperMethods.getPathToAnimation(entity.type, "down");
            int animationFramesCount = HelperMethods.getFileCount(animationPath);
            

            if (prevDown) //already moving down (continue animation)
            {
                //time per frame = duration/numFrames
                //current time realtive = timer % duration
                //current frame = timeRelative / 

                float timePerFrame = duration / animationFramesCount;
                float normalizedTime = timer % duration;
                int currentFrameIndex = (int) (normalizedTime / timePerFrame);

                animationFileName = entity.color + "_down_" + currentFrameIndex;

                timer += Engine.TimeDelta;
            }
            else
            {
                timer = 0;
                animationFileName = entity.color + "_down_" + 0;
                prevDown = true;
                prevUp = false;
                prevLeft = false;
                prevRight = false;
            }
        }


        else if (entity.movingUp) //moving up animation
        {
            String animationPath = HelperMethods.getPathToAnimation(entity.type, "up");
            int animationFramesCount = HelperMethods.getFileCount(animationPath);


            if (prevUp) //already moving down (continue animation)
            {
                //time per frame = duration/numFrames
                //current time realtive = timer % duration
                //current frame = timeRelative / 

                float timePerFrame = duration / animationFramesCount;
                float normalizedTime = timer % duration;
                int currentFrameIndex = (int)(normalizedTime / timePerFrame);

                animationFileName = entity.color + "_up_" + currentFrameIndex;

                timer += Engine.TimeDelta;
            }
            else
            {
                timer = 0;
                animationFileName = entity.color + "_up_" + 0;
                prevUp = true;
                prevDown = false;
                prevLeft = false;
                prevRight = false;
            }
        }

        else if (entity.movingRight) //moving right animation
        {

            String animationPath = HelperMethods.getPathToAnimation(entity.type, "right");

            //int animationFramesCount = HelperMethods.getFileCount(animationPath); THIS DOES NOT WORK - FIX

            //int animationFramesCount = HelperMethods.getFileCount(animationPath);
            int animationFramesCount;
            if (entity.color == "player")
            {
                animationFramesCount = 5;
            }
            else if (entity.color.Equals("boss"))
            {
                animationFramesCount = 2;
            }
            else
            {
                animationFramesCount = 1;
            }


            if (prevRight) //already moving down (continue animation)
            {
                //time per frame = duration/numFrames
                //current time realtive = timer % duration
                //current frame = timeRelative / 

                float timePerFrame = duration / animationFramesCount;
                float normalizedTime = timer % duration;
                int currentFrameIndex = (int)(normalizedTime / timePerFrame);

                animationFileName = entity.color + "_right_" + currentFrameIndex;

                timer += Engine.TimeDelta;
                
            }
            else
            {
                timer = 0;
                animationFileName = entity.color + "_right_" + 0;
                prevRight = true;
                prevDown = false;
                prevLeft = false;
                prevUp = false;
            }
        }

        else if (entity.movingLeft) //moving left animation
        {

            //todo: make animation frames length variable
            //int animationFramesCount = HelperMethods.getFileCount(animationPath);
            int animationFramesCount;
            if (entity.color == "player")
            {
                animationFramesCount = 5;
            }
            else if (entity.color.Equals("boss"))
            {
                animationFramesCount = 2;
            }
            else
            {
                animationFramesCount = 1;
            }

            if (prevLeft) //already moving down (continue animation)
            {
                //time per frame = duration/numFrames
                //current time realtive = timer % duration
                //current frame = timeRelative / 

                float timePerFrame = duration / animationFramesCount;
                float normalizedTime = timer % duration;
                int currentFrameIndex = (int)(normalizedTime / timePerFrame);

                animationFileName = entity.color + "_left_" + currentFrameIndex;

                timer += Engine.TimeDelta;
            }
            else
            {
                timer = 0;
                animationFileName = entity.color + "_left_" + 0;
                prevLeft = true;
                prevDown = false;
                prevUp = false;
                prevRight = false;
            }
        }

        if (animationFileName != null)
        {
            entity.collisionBox = HelperMethods.getAnimationCollisionBox(animationFileName);
            entity.texture = Textures.textures[animationFileName];
        }
    }

}

internal class CollectibleEntityAnimation
{
    private CollectibleEntity entity;
    private float timer;
    private float duration;
    private float amplitude; // Maximum bounce height
    private float frequency; // Bounce speed
    private bool isAnimating;

    public CollectibleEntityAnimation(CollectibleEntity entity, float duration, float amplitude, float frequency)
    {
        this.entity = entity;
        this.duration = duration;
        this.amplitude = amplitude;
        this.frequency = frequency;
        this.timer = 0;
        this.isAnimating = false;
    }

    public void Start()
    {
        isAnimating = true;
        timer = 0;
    }

    public void Animate()
    {
        if (!isAnimating) return;

        timer += Engine.TimeDelta;

        // Apply bounce using sinusoidal motion
        entity.yPos += amplitude * (float)Math.Sin(2 * Math.PI * frequency * timer);

        // Check if the animation duration is over
        if (timer >= duration)
        {
            Stop();
        }
    }

    public void Stop()
    {
        isAnimating = false;
        entity.isHidden = true; // Hide the entity after animation ends
    }
}




internal class PlayerEntityAnimation
{

    PlayerEntity entity;
    float timer;
    float duration;

    bool prevWallJumping = false;
    bool prevSliding = false;
    bool prevOnLadder = false;


    public PlayerEntityAnimation(PlayerEntity entity, float duration)
    {
        this.entity = entity;
        this.duration = duration;
        timer = 0;
    }

    public void Animate()
    {
        string animationFileName = null;

        if (entity.isWallJumping) //wall jumping animation
        {
            string animationPath = HelperMethods.getPathToAnimation(entity.type, "walljumping");
            //int animationFramesCount = HelperMethods.getFileCount(animationPath);
            int animationFramesCount = 1;


            if (prevWallJumping) //already moving down (continue animation)
            {
                //time per frame = duration/numFrames
                //current time realtive = timer % duration
                //current frame = timeRelative / 

                float timePerFrame = duration / animationFramesCount;
                float normalizedTime = timer % duration;
                int currentFrameIndex = (int)(normalizedTime / timePerFrame);

                string direction = (entity.movingRight) ? "right" : "left";

                animationFileName = entity.color + "_walljumping_" + direction + "_" + currentFrameIndex;

                timer += Engine.TimeDelta;
            }
            else
            {
                timer = 0;
                string direction = (entity.movingRight) ? "right" : "left";

                animationFileName = entity.color + "_walljumping_" + direction + "_" + 0;
                prevWallJumping = true;
                prevSliding = false;
                prevOnLadder = false;
            }
        }


        else if (entity.isSliding) //sliding animation
        {
            string animationPath = HelperMethods.getPathToAnimation(entity.type, "sliding");
            int animationFramesCount = HelperMethods.getFileCount(animationPath);


            if (prevSliding) //already sliding (continue animation)
            {
                //time per frame = duration/numFrames
                //current time realtive = timer % duration
                //current frame = timeRelative / 

                float timePerFrame = duration / animationFramesCount;
                float normalizedTime = timer % duration;
                int currentFrameIndex = (int)(normalizedTime / timePerFrame);
                
                string direction = (entity.SpeedMpS.X > 0) ? "right" : "left";
                

                animationFileName = entity.color + "_sliding_" + direction + "_" + currentFrameIndex;

                timer += Engine.TimeDelta;
            }
            else
            {
                timer = 0;
                string direction = (entity.movingLeft) ? "left" : "right";

                animationFileName = entity.color + "_sliding_" + direction + "_" + 0;
                prevSliding = true;
                prevOnLadder = false;
                prevWallJumping = false;
            }
        }

        else if (entity.onLadder) //climbing animation
        {
            string animationPath = HelperMethods.getPathToAnimation(entity.type, "climbing");
            int animationFramesCount = HelperMethods.getFileCount(animationPath);


            if (prevOnLadder) //already climbing (continue animation)
            {
                //time per frame = duration/numFrames
                //current time realtive = timer % duration
                //current frame = timeRelative / 

                float timePerFrame = duration / animationFramesCount;
                float normalizedTime = timer % duration;
                int currentFrameIndex = (int)(normalizedTime / timePerFrame);

                string direction = (entity.movingUp) ? "up" : "down";

                animationFileName = entity.color + "_climbing_" + direction + "_" + currentFrameIndex;

                timer += Engine.TimeDelta;
            }
            else
            {
                timer = 0;
                string direction = (entity.movingRight) ? "up" : "down";

                animationFileName = entity.color + "_climbing_" + direction + "_" + 0;
                prevOnLadder = true;
                prevSliding = false;
                prevWallJumping = false;
            }
        }



        if (animationFileName != null)
        {
            Debug.WriteLine(animationFileName);
            entity.collisionBox = HelperMethods.getAnimationCollisionBox(animationFileName);
            
            entity.texture = Textures.textures[animationFileName];
        }
    }


}