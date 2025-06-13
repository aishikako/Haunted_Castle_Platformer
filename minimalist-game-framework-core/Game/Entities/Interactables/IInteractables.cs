using System;
using System.IO;

/// <summary>
/// Represents an interface for objects that interact with the player/platform and needs collision detection (gem, potion, lever).
/// </summary>
internal interface IInteractables 
{
    // === Properties ===
    void Interact();
    void IsCollided(PlayerEntity player);
}
