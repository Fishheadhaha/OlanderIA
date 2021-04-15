using System;
using System.Collections.Generic;
using System.Reflection;

public class Simulator
{
    int currentCommand = 0;
    // Mode 1 = AI-AI
    // Mode 2 = AI-Human
    // Mode 3 = Human-Human
    int GameMode = 0;
    //
    int TurnNumber = 0;
    int TurnCounter = 0;
    int Player1Turn = 0;
    int Player2Turn = 0;
    //
    bool GameRunning = false;
    //
    int[] Board = new int[9];
    //
    Dictionary<int, MethodInfo> GameMethods = new Dictionary<int, MethodInfo>();
    //
    bool Player1Victory = false;
    bool Tie = false;
    //
    object[] Decisions;

    public Simulator()
    {
        Type temp = this.GetType();
        GameMethods.Add(0, temp.GetMethod("UpperLeft"));
        GameMethods.Add(1, temp.GetMethod("UpperMiddle"));
        GameMethods.Add(2, temp.GetMethod("UpperRight"));
        GameMethods.Add(3, temp.GetMethod("MiddleLeft"));
        GameMethods.Add(4, temp.GetMethod("MiddleMiddle"));
        GameMethods.Add(5, temp.GetMethod("MiddleRight"));
        GameMethods.Add(6, temp.GetMethod("LowerLeft"));
        GameMethods.Add(7, temp.GetMethod("LowerMiddle"));
        GameMethods.Add(8, temp.GetMethod("LowerRight"));
    }
    // Moves
    private bool UpperLeft()
    {
        if (Board[0] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[0] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[0] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool UpperMiddle()
    {
        if (Board[1] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[1] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[1] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool UpperRight()
    {
        if (Board[2] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[2] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[2] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool MiddleLeft()
    {
        if (Board[3] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[3] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[3] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool MiddleMiddle()
    {
        if (Board[4] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[4] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[4] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool MiddleRight()
    {
        if (Board[5] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[5] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[5] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool LowerLeft()
    {
        if (Board[6] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[6] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[6] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool LowerMiddle()
    {
        if (Board[7] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[7] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[7] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    private bool LowerRight()
    {
        if (Board[8] == 0)
        {
            if (TurnCounter == 1)
            {
                Board[8] = 1;
                return true;
            }
            else if (TurnCounter == 2)
            {
                Board[8] = -1;
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }
    //
    // Game methods
    private bool PlayMove(object[] MoveArgs)
    {
        if (Move >= 0)
        {
            if (GameRunning)
            {
                currentCommand = Move;
                return (bool)GameMethods[(int)MoveArgs[0]].Invoke(this, new object[] { });
            }
            else
                return false;
        }
        else
            return false;
    }
    private void CheckEnding()
    {
        if (Board[0] == Board[3] && Board[3] == Board[6])
            EndRound();
        if (Board[1] == Board[4] && Board[4] == Board[7])
            EndRound();
        if (Board[2] == Board[5] && Board[5] == Board[8])
            EndRound();
        if (Board[0] == Board[1] && Board[1] == Board[2])
            EndRound();
        if (Board[3] == Board[4] && Board[4] == Board[5])
            EndRound();
        if (Board[6] == Board[7] && Board[7] == Board[8])
            EndRound();
        if (Board[0] == Board[1] && Board[3] == Board[6])
            EndRound();
        if (Board[0] == Board[4] && Board[4] == Board[8])
            EndRound();
        if (Board[2] == Board[4] && Board[4] == Board[6])
            EndRound();
        if (TurnNumber == 9)
        {
            Tie = true;
            EndRound();
        }
    }
    private void EndRound()
    {
        GameRunning = false;
        if (!Tie)
        {
            if (TurnCounter == Player1Turn)
                Player1Victory = true;
            else
                Player1Victory = false;
        }
    }
    private void InitializeRound()
    {
        Random RND = new Random();
        TurnCounter = 1;
        TurnNumber = 0;
        Player1Turn = RND.Next(1, 3);
        Player2Turn = 3 - Player1Turn;
        GameRunning = true;
        Tie = false;
        Player1Victory = false;
        Board = new int[9];
        Decisions = new object[0];
    }
    private void NextTurn()
    {
        if (TurnCounter == 1)
            TurnCounter++;
        else if (TurnCounter == 2)
            TurnCounter--;
        TurnNumber++;
    }
    // Public Access Methods
    public int[][] GetCommandSignature()
    {
        int[][] temp = new int[10][];
        for (int i = 0; i < temp.Length; i++)
        { temp[i] = new int[4]; }
        temp[0][0] = 9;
        return temp;
    }
    public object[] GetData()
    {
        object[] temp = new object[9];
        for (int i = 0; i < 9; i++)
        { temp[i] = Board[i]; }
        return temp;
    }
    public int[] GetDataSignature()
    {
        int[] temp = new int[3];
        temp[0] = 0;
        temp[1] = 0;
        temp[2] = 9;
        return temp;
    }
    public string[] GetCommandNames()
    {
        string[] temp = new string[9];
        temp[0] = "UpperLeft";
        temp[1] = "UpperMiddle";
        temp[2] = "UpperRight";
        temp[3] = "MiddleLeft";
        temp[4] = "MiddleMiddle";
        temp[5] = "MiddleRight";
        temp[6] = "LowerLeft";
        temp[7] = "LowerMiddle";
        temp[8] = "LowerRight";
        return temp;
    }
    public void SetCommand(object[] commandArgs)
    {
        bool Finished = false;
        int Count = 0;
        while (!Finished)
        {
            int currentHighest = 0;
            for (int i = 0; i < commandArgs.Length; i++)
            {
                try
                {
                    if ((int)((object[])commandsArgs[i])[0] > (int)((object[])commandsArgs[currentHighest])[0])
                        currentHighest = i;
                }
                catch (Exception)
                { }
            }
            if (PlayMove((object[])commandArgs[currentHighest]))
            {
                Finished = true;
                CheckEnding();
                NextTurn();
            }
            else if (Count >= commandArgs.Length)
                Finished = true;
            else
            {
                commandArgs[currentHighest] = new object();
                Count++;
            }
        }
    }
    public int ReturnChosenCommand()
    {
        return currentCommand;
    }
    public bool IsAITurn()
    {
        if (GameMode == 1)
            return true;
        else if (GameMode == 2)
        {
            if (TurnCounter == Player1Turn)
                return true;
            else
                return false;
        }
        else
            return false;
    }
    public void SetGameMode(int mode)
    {
        GameMode = mode;
    }
    public void Start()
    {
        InitializeRound();
    }
    public bool Finished()
    {
        return !GameRunning;
    }
    public int GetResult(int Player)
    {
        if (!GameRunning)
        {
            if (Tie)
                return 0;
            else
            {
                if (Player == 1)
                {
                    if (Player1Victory)
                        return 1;
                    else
                        return -1;
                }
                else
                {
                    if (Player1Victory)
                        return -1;
                    else
                        return 1;
                }
            }
        }
        else
            return 0;
    }
}