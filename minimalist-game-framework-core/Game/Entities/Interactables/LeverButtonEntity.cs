using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

/// <summary>
/// Represents an entity that detects collision, but has no physics, and is linked to a platform (lever).
/// </summary>
internal class leverButtonEntity : Entity, IInteractables
{
    //public MovingEntity linkedEntity { get; set; }
    public List<MovingEntity> LinkedEntities { get; set; } = new List<MovingEntity>();

    public bool isActive { get; set; }
    public int ID;
    private double timePressed = -1;

    public leverButtonEntity(float xPos, float yPos, float width, float height, string color, int ID, string type)
        : base(xPos, yPos, width, height, color, type)
    {
        this.ID = ID;
        time = 0;
    }

    //called when collision is detected
    public void Interact()
    {
        //switching direction of lever when moving
        if (!LinkedEntities[0].isActive && type.Equals("lever"))
        {
            color = color.Equals("leverright") ? "leverleft" : "leverright";
            GameScreen.music.playSoundEffect("collision-reverb.wav");
        }

        //switching activeness of lever and platform
        foreach (var linkedEntity in LinkedEntities)
        {
            if (!linkedEntity.isActive && type.Equals("lever"))
            {
                linkedEntity.isActive = true;
                isActive = true;
            }
            else if(type.Equals("button") && !linkedEntity.isHidden)
            {
                linkedEntity.isHidden = true;
                isActive = true;
                timePressed = this.time;
            }
            else if (type.Equals("collisionbox"))
            {
                isActive = true;
                linkedEntity.isActive = true;
            }
        }
    }

    public void IsCollided(PlayerEntity player)
    {
        float tolerance = 0.001f;
        bool isXOverlap = (xPos + Width + tolerance > player.xPos && xPos - tolerance < player.xPos + player.Width);
        bool isYOverlap = (yPos + Height + tolerance > player.yPos && yPos - tolerance < player.yPos + player.Height);
        if (isXOverlap && isYOverlap)
        {
            if (Engine.GetKeyDown(Key.E)) Interact();
            else if (this.type.Equals("collisionbox")) Interact();
        }
    }

    public void addLinkedEntity(Dictionary<string, List<Entity>> EntityLayers)
    {
        foreach (MovingEntity entity in EntityLayers["blocks"].OfType<MovingEntity>())
        {
            if (entity.isLinked && entity.ID - 1 == ID)
            {
                isActive = false;
                entity.isActive = false;
                LinkedEntities.Add(entity);
            }
        }
    }

    public void checkButtonTimer()
    {
        //switching activeness of lever and platform
        foreach (var linkedEntity in LinkedEntities)
        {
            if (type.Equals("button") && time >= timePressed + 6)
            {
                linkedEntity.isHidden = false;
                isActive = false;
            }
        }

    }
}
