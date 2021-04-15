using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace OlanderIA
{
    public class TicTacToe : InternalSimulator
    {
        int currentCommand = 0;
        // Mode 1 = AI-AI
        // Mode 2 = AI-Human
        // Mode 3 = Human-Human
        int GameMode = 0;
        //
        int TurnCount = 0;
        int CurrentTurn = 0;
        int Player1Turn = 0;
        int Player2Turn = 0;
        int FirstPlayerTurn = 0;
        int SecondPlayerTurn = 0;
        //
        bool GameRunning = false;
        //
        int PlayerCount = 2;
        //
        int[] Board = new int[9];
        //
        Dictionary<int, MethodInfo> GameMethods = new Dictionary<int, MethodInfo>();
        //
        bool Tie = false;
        bool started = false;
        //
        object[] Decisions;
        //
        int upEnd = 2;
        int lowEnd = -2;
        //
        int commandCount = 9;
        //
        int Winner = 0;

        public TicTacToe()
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
        public bool UpperLeft()
        {
            if (Board[0] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[0] = 1;
                    if (Player1Turn != 1)
                        Board[0] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[0] = -1;
                    if (Player2Turn != 2)
                        Board[0] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool UpperMiddle()
        {
            if (Board[1] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[1] = 1;
                    if (Player1Turn != 1)
                        Board[1] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[1] = -1;
                    if (Player2Turn != 2)
                        Board[1] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool UpperRight()
        {
            if (Board[2] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[2] = 1;
                    if (Player1Turn != 1)
                        Board[2] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[2] = -1;
                    if (Player2Turn != 2)
                        Board[2] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool MiddleLeft()
        {
            if (Board[3] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[3] = 1;
                    if (Player1Turn != 1)
                        Board[3] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[3] = -1;
                    if (Player2Turn != 2)
                        Board[3] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool MiddleMiddle()
        {
            if (Board[4] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[4] = 1;
                    if (Player1Turn != 1)
                        Board[4] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[4] = -1;
                    if (Player2Turn != 2)
                        Board[4] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool MiddleRight()
        {
            if (Board[5] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[5] = 1;
                    if (Player1Turn != 1)
                        Board[5] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[5] = -1;
                    if (Player2Turn != 2)
                        Board[5] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool LowerLeft()
        {
            if (Board[6] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[6] = 1;
                    if (Player1Turn != 1)
                        Board[6] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[6] = -1;
                    if (Player2Turn != 2)
                        Board[6] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool LowerMiddle()
        {
            if (Board[7] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[7] = 1;
                    if (Player1Turn != 1)
                        Board[7] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[7] = -1;
                    if (Player2Turn != 2)
                        Board[7] *= -1;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        public bool LowerRight()
        {
            if (Board[8] == 0)
            {
                if (CurrentTurn == Player1Turn)
                {
                    Board[8] = 1;
                    if (Player1Turn != 1)
                        Board[8] *= -1;
                    return true;
                }
                else if (CurrentTurn == Player2Turn)
                {
                    Board[8] = -1;
                    if (Player2Turn != 2)
                        Board[8] *= -1;
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
            if ((int)MoveArgs[0] >= 0)
            {
                if (GameRunning)
                {
                    currentCommand = (int)MoveArgs[0];
                    bool result = (bool)GameMethods[(int)MoveArgs[0]].Invoke(this, new object[] { });
                    if (result && TurnCount == 0)
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            if (Board[i] == -1)
                                Debug.WriteLine("Error");
                        }
                    }

                    return result;
                }
                else
                    return false;
            }
            else
                return false;
        }
        private void CheckEnding()
        {
            if (Board[0] == Board[3] && Board[3] == Board[6] && Board[0] != 0)
                EndRound();
            else if (Board[1] == Board[4] && Board[4] == Board[7] && Board[1] != 0)
                EndRound();
            else if(Board[2] == Board[5] && Board[5] == Board[8] && Board[2] != 0)
                EndRound();
            else if (Board[0] == Board[1] && Board[1] == Board[2] && Board[0] != 0)
                EndRound();
            else if (Board[3] == Board[4] && Board[4] == Board[5] && Board[3] != 0)
                EndRound();
            else if (Board[6] == Board[7] && Board[7] == Board[8] && Board[6] != 0)
                EndRound();
            else if (Board[0] == Board[4] && Board[4] == Board[8] && Board[0] != 0)
                EndRound();
            else if (Board[2] == Board[4] && Board[4] == Board[6] && Board[2] != 0)
                EndRound();
            else if (TurnCount == 8)
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
                // I think this should stay
                if (CurrentTurn == Player1Turn)
                    Winner = 1;
                else if (CurrentTurn == Player2Turn)
                    Winner = 2;
                else Winner = 0;
            }
        }
        private void InitializeRound()
        {
            Random RND = new Random();
            TurnCount = 0;
            Player1Turn = RND.Next(1, 3);
            Player2Turn = 3 - Player1Turn;
            CurrentTurn = 1;
            GameRunning = true;
            Tie = false;
            Winner = 0;
            Board = new int[9];
            Decisions = new object[0];
            started = true;
        }
        private void InitializeRound(int FirstPlayer)
        {
            TurnCount = 0;
            if (FirstPlayer == 1)
            { 
                Player1Turn = 1;
                Player2Turn = 2;
            }
            else
            {
                Player1Turn = 2;
                Player2Turn = 1;
            }
            CurrentTurn = 1;
            GameRunning = true;
            Tie = false;
            Winner = 0;
            Board = new int[9];
            Decisions = new object[0];
            started = true;
        }
        private void NextTurn()
        {
            if (GameRunning)
            {
                if (CurrentTurn % 2 == 0)
                    CurrentTurn--;
                else
                    CurrentTurn++;
                TurnCount++;
            }
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
        public object[] GetData(int Player)
        {
            if (Player == Player1Turn)
            {
                object[] temp = new object[9];
                for (int i = 0; i < 9; i++)
                {
                    if (Player1Turn != 1)
                        temp[i] = -1 * Board[i];
                    else
                        temp[i] = Board[i];
                }

                return temp;
            }
            else if (Player == Player2Turn)
            {
                object[] temp = new object[9];
                for (int i = 0; i < 9; i++)
                {
                    if (Player2Turn != 2)
                        temp[i] = Board[i];
                    else
                        temp[i] = -1 * Board[i];
                }
                return temp;
            }
            else
                return new object[0];
            /*
            if (Player == Player1Turn)
            {
                if (Player1Turn == 1)
                {
                    object[] temp = new object[9];
                    for (int i = 0; i < 9; i++)
                    { temp[i] = Board[i]; }
                    return temp;
                }
                else
                {
                    object[] temp = new object[9];
                    for (int i = 0; i < 9; i++)
                    { temp[i] =  (-1 * Board[i]); }
                    return temp;
                }
            }
            if (Player == Player2Turn)
            {
                if (Player2Turn == 1)
                {
                    object[] temp = new object[9];
                    for (int i = 0; i < 9; i++)
                    { temp[i] = Board[i]; }
                    return temp;
                }
                else
                {
                    object[] temp = new object[9];
                    for (int i = 0; i < 9; i++)
                    { temp[i] = (-1 * Board[i]); }
                    return temp;
                }
            }
            else
            {
                object[] temp = new object[9];
                for (int i = 0; i < 9; i++)
                { temp[i] = 0; }
                return temp;
            }
            */
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
            if (GameRunning)
            {
                bool Finished = false;
                int Count = 0;
                int[] Invalids = new int[commandArgs.Length];
                while (!Finished)
                {
                    int currentHighest = -1;
                    for (int i = 0; i < commandArgs.Length; i++)
                    {
                        if (Invalids[i] == 0)
                        { 
                            currentHighest = i;
                            break;
                        }
                    }
                    if (currentHighest != -1)
                    {
                        for (int i = 0; i < commandArgs.Length; i++)
                        {
                            try
                            {
                                if (Invalids[i] == 0)
                                {
                                    if ((int)((object[])commandArgs[i])[0] > (int)((object[])commandArgs[currentHighest])[0])
                                        currentHighest = i;
                                }
                            }
                            catch (InvalidCastException)
                            { }
                        }
                        if (Count >= commandArgs.Length)
                        {
                            currentCommand = -1;
                            Finished = true;
                        }
                        else
                        {
                            if (commandArgs.Length < commandCount)
                            {
                                object[] temporary = new object[commandArgs.Length];
                                temporary[0] = (int)((object[])commandArgs[currentHighest])[0];
                                Array.Copy(commandArgs, 1, temporary, 1, commandArgs.Length - 1);
                                if (PlayMove(temporary))
                                {
                                    Finished = true;
                                    CheckEnding();
                                    NextTurn();
                                }
                                else
                                {
                                    commandArgs[currentHighest] = new object();
                                    Invalids[currentHighest] = -1;
                                    Count++;
                                }
                            }
                            else
                            {
                                object[] temporary = new object[commandArgs.Length];
                                temporary[0] = currentHighest;
                                Array.Copy(commandArgs, 1, temporary, 1, commandArgs.Length - 1);
                                if (PlayMove(temporary))
                                {
                                    Finished = true;
                                    CheckEnding();
                                    NextTurn();
                                }
                                else
                                {
                                    commandArgs[currentHighest] = new object();
                                    Invalids[currentHighest] = -1;
                                    Count++;
                                }
                            }
                        }
                    }
                    else
                    {
                        currentCommand = -1;
                        Finished = true;
                    }
                }
            }
        }
        public int[] ReturnRecommendedRange()
        {
            int[] temp = new int[2];
            temp[0] = lowEnd;
            temp[1] = upEnd;
            return temp;
        }
        public int ReturnChosenCommand()
        {
            return currentCommand;
        }
        public int Turn()
        {
            if (CurrentTurn == Player1Turn)
                return 1;
            else if (CurrentTurn == Player2Turn)
                return 2;
            else
                return 0;
        }
        public void SetGameMode(int mode)
        {
            GameMode = mode;
        }
        public void Start()
        {
            InitializeRound();
        }
        public void Start(int FirstPlayer)
        {
            InitializeRound(FirstPlayer);
        }
        public bool Finished()
        {
            return !GameRunning;
        }
        public int GetResult(int Player)
        {
            if (started)
            {
                if (Tie)
                    return 20;
                else
                {
                    if (Winner == Player)
                        return 20;
                    else
                        return -1;
                }
            }
            else
                return 0;
        }
        public int GetUnweightedResult(int Player)
        {
            if (started)
            {
                if (Tie)
                    return 0;
                else
                {
                    if (Winner == Player)
                        return 1;
                    else
                        return -1;
                }
            }
            else
                return 0;
        }
        public int GetPlayerCount()
        {
            return PlayerCount;
        }
    }
}