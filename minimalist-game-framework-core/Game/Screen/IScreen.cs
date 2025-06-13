using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents an interface for objects that interact with the player/platform and needs collision detection (gem, potion, lever).
/// </summary>
internal interface IScreen
{
    // === Properties ===
    abstract void Update();
    abstract void Draw();
}