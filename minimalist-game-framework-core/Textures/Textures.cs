using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


internal class Textures
{

    public static Dictionary<string, Texture> textures { get; set; }

    public static void loadTextures()
    {

        textures = new Dictionary<string, Texture>();

        string[] colors = new[]
        {
            "black", "blue", "brown", "crimson", "gold", "gray", "green",
            "lightgray", "lightgreen", "star", "orange", "peach",
            "pink", "purple", "red", "white", "yellow", "background1", "pinkwall", "wall",
            "gem", "door", "fire", "platform", "intervalplatform", "vines", "transparent",
            "leverleft", "leverright", "button", "chandelier", "spikes", "spikes_right_0" , "spikes_left_0", "spikes_up_0" , "spikes_down_0",
            "pickup_speed", "pickup_jump", "pickup_shield",
            "background2", "background3", "player", "yellowhalf", "BorderPic", "squigle", "UpGround", "lightingImage", "movingplatform", "resume", "starfilled", "starnotfilled",
            //buttons
            "button_credits", "button_exitscreen", "button_instructions", "button_pause", "button_play", "button_scoreboard",
            // screens
            "screen_start", "screen_scoreboard", "screen_instructions", "screen_gamecredits", "screen_levelup", "screen_pause",
            // buttons
            "button_credits", "button_instructions", "button_play", "button_scoreboard",

            //player animations
            //moving down
            "player_down_0", "player_down_1", "player_down_2",
            //moving up
            "player_up_0", "player_up_1", "player_up_2",
            //moving right
            "player_right_0", "player_right_1", "player_right_2", "player_right_3", "player_right_4", "player_right_5",
            //moving left
            "player_left_0", "player_left_1", "player_left_2","player_left_3", "player_left_4", "player_left_5",

            //sliding right
            "player_sliding_right_0", "player_sliding_right_1", "player_sliding_right_2",
            //sliding left
            "player_sliding_left_0", "player_sliding_left_1", "player_sliding_left_2",

            //climbing up
            "player_climbing_up_0", "player_climbing_up_1", "player_climbing_up_2",
            //climbing down
            "player_climbing_down_0", "player_climbing_down_1", "player_climbing_down_2",

            //walljumping right
            "player_walljumping_right_0", "player_walljumping_right_1", "player_walljumping_right_2",
            //walljumping left
            "player_walljumping_left_0", "player_walljumping_left_1", "player_walljumping_left_2",

            //boss
            "boss_left_0", "boss_left_1" , "boss_right_0", "boss_right_1",

            //ghost
            "ghost_right_0", "ghost_left_0",

            // spider
            "spider_1", "spider_2",
            //moving platform animations
            //moving down
            "movingplatform_down_0", "movingplatform_down_1", "movingplatform_down_2",
            //moving up
            "movingplatform_up_0", "movingplatform_up_1", "movingplatform_up_2",
            //moving right
            "movingplatform_right_0", "movingplatform_right_1", "movingplatform_right_2",
            //moving left
            "movingplatform_left_0", "movingplatform_left_1", "movingplatform_left_2",
        };


        string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Textures");


        foreach (var color in colors)
        {


            string texturePath = Path.Combine(basePath, $"{color}.png");
            if (File.Exists(texturePath))
            {
                textures[color] = Engine.LoadTexture(texturePath);
            }

            //////loading player animations
            //if (color.Contains("player"))
            //{
            //    var directionpattern = @"_(up|down|left|right)";
            //    var direction = Regex.Match(color, directionpattern);

            //    if (color.Contains("sliding") || color.Contains("climbing") || color.Contains("walljumping")) //player specific movements
            //    {


            //        var movetypepattern = @"_(sliding|climbing|walljumping)_";
            //        var movetype = Regex.Match(color, movetypepattern);

            //        string texturePath = Path.Combine(basePath, "player", movetype.Groups[1].Value + direction, color + ".png");
            //        Debug.WriteLine(texturePath);
            //        if (File.Exists(texturePath))
            //        {

            //            textures[color] = Engine.LoadTexture(texturePath);
            //        }
            //    }
            //    else //defaults movements like up, down, left, right
            //    {
            //        string texturePath = Path.Combine(basePath, "player", direction.Groups[1].Value, color + ".png");
            //        if (File.Exists(texturePath))
            //        {
            //            textures[color] = Engine.LoadTexture(texturePath);
            //        }
            //    }
            //}

            ////loading moving platform animations
            //else if (color.Contains("movingplatform"))
            //{
            //    var pattern = @"_(up|down|left|right)_";
            //    var direction = Regex.Match(color, pattern);
            //    string texturePath = Path.Combine(basePath, "movingplatform", direction.Groups[1].Value, color + ".png");
            //    if (File.Exists(texturePath))
            //    {
            //        textures[color] = Engine.LoadTexture(texturePath);
            //    }
            //}


            //else
            //{
            //    string texturePath = Path.Combine(basePath, $"{color}.png");
            //    if (File.Exists(texturePath))
            //    {
            //        textures[color] = Engine.LoadTexture(texturePath);
            //    }
            //}

        }
    }

}






//Legacy file finding

////loading player animations
//if (color == "player_down_0" || color == "player_down_1" || color == "player_down_2")
//{
//    string texturePath = Path.Combine(basePath, "player", "down", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}
//else if (color == "player_up_0" || color == "player_up_1" || color == "player_up_2")
//{
//    string texturePath = Path.Combine(basePath, "player", "up", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}
//else if (color == "player_right_0" || color == "player_right_1" || color == "player_right_2")
//{
//    string texturePath = Path.Combine(basePath, "player", "right", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}
//else if (color == "player_left_0" || color == "player_left_1" || color == "player_left_2")
//{
//    string texturePath = Path.Combine(basePath, "player", "left", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}


//loading moving platform animations
//else if (color == "movingplatform_down_0" || color == "movingplatform_down_1" || color == "movingplatform_down_2")
//{
//    string texturePath = Path.Combine(basePath, "movingplatform", "down", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}
//else if (color == "movingplatform_up_0" || color == "movingplatform_up_1" || color == "movingplatform_up_2")
//{
//    string texturePath = Path.Combine(basePath, "movingplatform", "up", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}
//else if (color == "movingplatform_right_0" || color == "movingplatform_right_1" || color == "movingplatform_right_2")
//{
//    string texturePath = Path.Combine(basePath, "movingplatform", "right", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}
//else if (color == "movingplatform_left_0" || color == "movingplatform_left_1" || color == "movingplatform_left_2")
//{
//    string texturePath = Path.Combine(basePath, "movingplatform", "left", color + ".png");
//    if (File.Exists(texturePath))
//    {
//        textures[color] = Engine.LoadTexture(texturePath);
//    }
//}