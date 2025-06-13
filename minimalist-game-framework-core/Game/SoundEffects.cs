using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using System.Collections;
using System.Diagnostics;

// FileManager handles parsing through the file and creating the entities in a level
internal class SoundEffects
{

    public bool soundPlayed = false;
    public bool SoundPlayed
    {
        get { return soundPlayed; }
        set { soundPlayed = value; }
    }

    /// <summary>
    /// Plays a one-time sound effect from the specified file path.
    /// </summary>
    /// <param name="soundPath">The relative path to the sound file.</param>
    public void playSoundEffect(string soundPath, Boolean repeat)
    {
        if (!soundPlayed)
        {
            // Construct the full file path to the sound effect.
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            string totalPath = Path.Combine(basePath, "..", "..", "..", "Sounds", soundPath);
            Engine.PlaySound(Engine.LoadSound(totalPath), repeat, 0); // Load and play the sound effect.
        }
    }

    /// <summary>
    /// Plays a one-time sound effect from the specified file path.
    /// </summary>
    /// <param name="soundPath">The relative path to the sound file.</param>
    public void playSoundEffect(string soundPath)
    {
        playSoundEffect(soundPath, false);
    }

    /// <summary>
    /// Plays a one-time sound effect from the specified file path.
    /// </summary>
    /// <param name="soundPath">The relative path to the sound file.</param>
    public void playBackgroundMusic()
    {
        playSoundEffect("background-music.wav", true);
    }
}