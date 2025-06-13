using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;



internal class VideoGameTesting
{
    public static void RunTests()
    {
        Console.WriteLine(TestLoadLevelFileExists());
        Console.WriteLine(TestLoadLevelFileDoesNotExist());
    }

    private static bool TestLoadLevelFileExists()
    {
        bool testSuccess = false;
        FileManager fileManager = new FileManager();
        FileManager.FileName = "C:\\platformer-game-team1\\minimalist-game-framework-core\\Game\\level1.txt";
        Dictionary<string, List<Entity>> entities = fileManager.LoadLevel();
        if (entities != null  && entities.Count > 0)
        {
            testSuccess = true;
        }

        return testSuccess;

    }

    private static bool TestLoadLevelFileDoesNotExist()
    {
        bool testSuccess = false;
        FileManager fileManager = new FileManager();
        FileManager.FileName = "nonexistentfile.txt";
        Dictionary<string, List<Entity>> entities = fileManager.LoadLevel();

        if (entities == null)
        {
            testSuccess = true;
        }
        return testSuccess;
    }
}

