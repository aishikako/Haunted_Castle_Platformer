using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// manages all screen flow
/// </summary>
internal class ScreenFlow
{
    public Stack<Screen> stack;
    public Screen gameScreen;
    public Screen startScreen;
    public Screen deadScreen;
    public Screen levelUpScreen;

    //types of screen used
    public enum ScreenType
    {
        startScreen,
        gameScreen,
        deadScreen,
        levelUpScreen
    }

    // adds game and start screen initially
    public ScreenFlow(Screen startScreen, Screen gameScreen, Screen deadScreen, Screen levelUpScreen)
    {
        stack = new Stack<Screen>();
        this.gameScreen = gameScreen;
        this.startScreen = startScreen;
        this.deadScreen = deadScreen;
        this.levelUpScreen = levelUpScreen;
        addScreen(gameScreen);
        addScreen(startScreen);
    }

    //add screen to top
    public void addScreen(Screen screen)
    {
        stack.Push(screen);
    }

    //remove top screen
    public void removeScreen()
    {
        if (stack.Count > 0) stack.Pop();
    }

    //draws the earliest screen, and popups above, then updates top layer and draws
    public void update()
    {
        int firstScreenIdx = 0;
        Screen[] stackArray = stack.ToArray();

        //finding most recent screen
        for (int i = 0; i <stackArray.Length;  i++)
        {
            if (!stackArray[i].isPopup)
            {
                firstScreenIdx = i;
                break;
            }
        }

        //updating from most recent screen to top of stack
        for (int i = firstScreenIdx; i > 0; i--)
        {
            stackArray[i].Draw();
        }

        // update + draw the top of the stack
        stackArray[0].Update();
        stackArray[0].Draw();
        
    }

    //adding specific screen, used from Game Screen
    public void handleAddScreen(ScreenType screenType)
    {
        switch (screenType)
        {
            case ScreenType.startScreen:
                addScreen(startScreen);
                break;
            case ScreenType.deadScreen:
                addScreen(deadScreen);
                break;
            case ScreenType.levelUpScreen:
                addScreen(levelUpScreen);
                break;
        }
    }

}
