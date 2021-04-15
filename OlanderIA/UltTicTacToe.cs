using System;

namespace OlanderIA
{
    class UltTicTacToe : InternalSimulator
    {
        // There isn't actually a game mode for this, but it needs to be included anyway.
        int GameMode = 0;

        bool GameRunning = false;
        bool GameStarted = false;

        int[][] Board = new int[9][];
        int[] LargeBoard = new int[9];

        int ChosenCommand = -1;
        int SelectedBoard = -1;

        int FirstPlayerID = 0;
        int SecondPlayerID = 0;
        int WinnerID = 0;

        int CurrentTurn = 0;
        int TurnCount = 0;

        bool SelectBoardFlag = false;
        bool TieFlag = false;
        bool PlayAgainFlag = false;

        Random Rand = new Random();

        public UltTicTacToe()
        {
            for (int i = 0; i < Board.Length; i++)
                Board[i] = new int[9];
        }

        private void PlayMove(double[] Moves)
        {
            if (Moves.Length == 1)
            {
                // This for loop is redundant, but I don't feel like replacing it.
                for (int i = 0; i < Moves.Length; i++)
                {
                    if (Moves[i] >= 0 && Moves[i] < 9 && !SelectBoardFlag)
                        PlayTile((int)Moves[i]);
                    else if (Moves[i] >= 9 && Moves[i] < 18 && SelectBoardFlag)
                        if (!SwapBoard((int)Moves[i] - 9))
                            PlayAgainFlag = true;
                        else
                        {
                            SelectBoardFlag = false;
                            ChosenCommand = (int)Moves[i];
                        }
                }
            }
            else if (Moves.Length == 18)
            {
                if (SelectBoardFlag)
                {
                    double Lowest = 0;
                    for (int k = 9; k < 18; k++)
                        if (Moves[k] < Lowest)
                            Lowest = Moves[k] - 1;

                    for (int i = 0; i < 9; i++)
                    {
                        int Highest = 9;
                        for (int k = 10; k < 18; k++)
                            if (Moves[Highest] < Moves[k])
                                Highest = k;

                        if (LargeBoard[Highest - 9] == 0)
                        {
                            SelectedBoard = (Highest - 9);
                            SelectBoardFlag = false;
                            ChosenCommand = Highest;
                            break;
                        }
                        else
                            Moves[Highest] = Lowest;
                    }
                }
                else
                {
                    double Lowest = 0;
                    for (int k = 0; k < 9; k++)
                        if (Moves[k] < Lowest)
                            Lowest = Moves[k] - 1;

                    for (int i = 0; i < 9; i++)
                    {
                        int Highest = 0;
                        for (int k = 0; k < 9; k++)
                            if (Moves[Highest] < Moves[k])
                                Highest = k;

                        if (Board[SelectedBoard][Highest] == 0)
                            PlayTile(Highest);
                    }
                }
            }
            else
                PlayAgainFlag = true;
        }

        private void PlayTile(int Tile)
        {
            // This needs to be fixed so that player 1 plays a 1, and player 2 plays a -1
            if (CurrentTurn == FirstPlayerID)
                Board[SelectedBoard][Tile] = 1;
            else if (CurrentTurn == SecondPlayerID)
                Board[SelectedBoard][Tile] = -1;

            ChosenCommand = Tile;

            CheckGrid(SelectedBoard);

            if (!SwapBoard(Tile))
                SelectBoardFlag = true;
        }

        private bool SwapBoard(int Board)
        {
            if (LargeBoard[Board] == 0)
                SelectedBoard = Board;
            else
                return false;
            return true;
        }

        private void NextTurn()
        {
            if (SelectBoardFlag)
                PlayAgainFlag = true;
            TurnCount++;
            if (CurrentTurn == 1)
                CurrentTurn++;
            else
                CurrentTurn--;
        }

        private void CheckGrid(int Grid)
        {
            // This doesn't actually need Grid, that part can be replaced by SelectedBoard, but I'm lazy
            if (Board[Grid][0] == Board[Grid][3] && Board[Grid][3] == Board[Grid][6] && Board[Grid][0] != 0)
                SetGridState(false);
            else if (Board[Grid][1] == Board[Grid][4] && Board[Grid][4] == Board[Grid][7] && Board[Grid][1] != 0)
                SetGridState(false);
            else if (Board[Grid][2] == Board[Grid][5] && Board[Grid][5] == Board[Grid][8] && Board[Grid][2] != 0)
                SetGridState(false);
            else if (Board[Grid][0] == Board[Grid][1] && Board[Grid][1] == Board[Grid][2] && Board[Grid][0] != 0)
                SetGridState(false);
            else if (Board[Grid][3] == Board[Grid][4] && Board[Grid][4] == Board[Grid][5] && Board[Grid][3] != 0)
                SetGridState(false);
            else if (Board[Grid][6] == Board[Grid][7] && Board[Grid][7] == Board[Grid][8] && Board[Grid][6] != 0)
                SetGridState(false);
            else if (Board[Grid][0] == Board[Grid][4] && Board[Grid][4] == Board[Grid][8] && Board[Grid][0] != 0)
                SetGridState(false);
            else if (Board[Grid][2] == Board[Grid][4] && Board[Grid][4] == Board[Grid][6] && Board[Grid][2] != 0)
                SetGridState(false);
            else
            {
                bool Passed = true;
                for (int i = 0; i < 9; i++)
                {
                    if (Board[Grid][i] == 0)
                    {
                        Passed = false;
                        break;
                    }
                }

                if (Passed)
                    SetGridState(true);
            }
        }

        private void SetGridState(bool Tie)
        {
            if (Tie)
                LargeBoard[SelectedBoard] = 1;
            else if (CurrentTurn == FirstPlayerID)
                LargeBoard[SelectedBoard] = 2;
            else if (CurrentTurn == SecondPlayerID)
                LargeBoard[SelectedBoard] = -2;
        }

        private void CheckWinner()
        {
            if (LargeBoard[0] == LargeBoard[3] && LargeBoard[3] == LargeBoard[6] && LargeBoard[0] != 0 && LargeBoard[0] != 1)
                EndRound();
            else if (LargeBoard[1] == LargeBoard[4] && LargeBoard[4] == LargeBoard[7] && LargeBoard[1] != 0 && LargeBoard[1] != 1)
                EndRound();
            else if (LargeBoard[2] == LargeBoard[5] && LargeBoard[5] == LargeBoard[8] && LargeBoard[2] != 0 && LargeBoard[2] != 1)
                EndRound();
            else if (LargeBoard[0] == LargeBoard[1] && LargeBoard[1] == LargeBoard[2] && LargeBoard[0] != 0 && LargeBoard[0] != 1)
                EndRound();
            else if (LargeBoard[3] == LargeBoard[4] && LargeBoard[4] == LargeBoard[5] && LargeBoard[3] != 0 && LargeBoard[3] != 1)
                EndRound();
            else if (LargeBoard[6] == LargeBoard[7] && LargeBoard[7] == LargeBoard[8] && LargeBoard[6] != 0 && LargeBoard[6] != 1)
                EndRound();
            else if (LargeBoard[0] == LargeBoard[4] && LargeBoard[4] == LargeBoard[8] && LargeBoard[0] != 0 && LargeBoard[0] != 1)
                EndRound();
            else if (LargeBoard[2] == LargeBoard[4] && LargeBoard[4] == LargeBoard[6] && LargeBoard[2] != 0 && LargeBoard[2] != 1)
                EndRound();
            else if (TurnCount == 81)
            {
                TieFlag = true;
                EndRound();
            }
        }

        private void EndRound()
        {
            GameRunning = false;
            if (!TieFlag)
            {
                if (CurrentTurn == FirstPlayerID)
                    WinnerID = FirstPlayerID;
                else if (CurrentTurn == SecondPlayerID)
                    WinnerID = SecondPlayerID;
                else
                    WinnerID = 0;
            }

        }

        // Public Access Methods
        public bool Finished()
        {
            return !GameRunning;
        }

        public string[] GetCommandNames()
        {
            string[] temp = new string[18];
            temp[0] = "UpperLeft";
            temp[1] = "UpperMiddle";
            temp[2] = "UpperRight";
            temp[3] = "MiddleLeft";
            temp[4] = "MiddleMiddle";
            temp[5] = "MiddleRight";
            temp[6] = "LowerLeft";
            temp[7] = "LowerMiddle";
            temp[8] = "LowerRight";
            temp[9] = "Grid - UpperLeft";
            temp[10] = "Grid - UpperMiddle";
            temp[11] = "Grid - UpperRight";
            temp[12] = "Grid - MiddleLeft";
            temp[13] = "Grid - MiddleMiddle";
            temp[14] = "Grid - MiddleRight";
            temp[15] = "Grid - LowerLeft";
            temp[16] = "Grid - LowerMiddle";
            temp[17] = "Grid - LowerRight";
            return temp;
        }

        public int[][] GetCommandSignature()
        {
            int[][] temp = new int[19][];
            for (int i = 0; i < temp.Length; i++)
            { temp[i] = new int[4]; }
            temp[0][0] = 18;
            return temp;
        }

        public object[] GetData(int Player)
        {
            object[] Data = new object[91];
            if (Player == FirstPlayerID)
            {
                int Pointer = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        Data[Pointer] = Board[i][k];
                        Pointer++;
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    Data[Pointer] = LargeBoard[i];
                    Pointer++;
                }

                Data[90] = SelectedBoard;

            }
            else if (Player == SecondPlayerID)
            {
                int Pointer = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int k = 0; k < 9; k++)
                    {
                        Data[Pointer] = -1 * Board[i][k];
                        Pointer++;
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    if (LargeBoard[i] == 1)
                        Data[Pointer] = LargeBoard[i];
                    else
                        Data[Pointer] = -1 * LargeBoard[i];
                    Pointer++;
                }

                Data[90] = SelectedBoard;
            }
            return Data;
        }

        public int[] GetDataSignature()
        {
            int[] temp = new int[3];
            temp[0] = 0;
            temp[1] = 0;
            temp[2] = 91;
            return temp;
        }

        public int GetPlayerCount()
        {
            return 2;
        }

        public int GetResult(int Player)
        {
            if (GameStarted)
            {
                if (TieFlag)
                    return 0;
                else if (WinnerID == Player)
                    return 1;
                else
                    return -1;
            }
            else
                return 0;
        }

        public int GetUnweightedResult(int Player)
        {
            if (GameStarted)
            {
                if (TieFlag)
                    return 0;
                else if (WinnerID == Player)
                    return 1;
                else
                    return -1;
            }
            else
                return 0;

        }

        public int ReturnChosenCommand()
        {
            return ChosenCommand;
        }

        public int[] ReturnRecommendedRange()
        {
            int[] Range = new int[2];
            Range[0] = -3;
            Range[1] = 3;
            return Range;
        }

        public void SetCommand(object[] commandArgs)
        {
            if (GameRunning)
            {
                // For ease of access
                double[] CommandChoices = new double[commandArgs.Length];
                for (int i = 0; i < commandArgs.Length; i++)
                    CommandChoices[i] = Convert.ToDouble((((object[])commandArgs[i])[0]));

                PlayMove(CommandChoices);
                CheckWinner();
                if (!PlayAgainFlag)
                    NextTurn();
                else
                    PlayAgainFlag = false;
            }
        }

        public void SetGameMode(int mode)
        {
            GameMode = mode;
        }

        public void Start()
        {
            GameRunning = true;

            WinnerID = -1;
            ChosenCommand = -1;

            TieFlag = false;
            SelectBoardFlag = true;
            PlayAgainFlag = true;
            GameStarted = true;

            FirstPlayerID = Rand.Next(1, 3);
            SecondPlayerID = 3 - FirstPlayerID;
            CurrentTurn = FirstPlayerID;

            for (int i = 0; i < 9; i++)
                Board[i] = new int[9];
            LargeBoard = new int[9];
        }

        public void Start(int FirstPlayer)
        {
            GameRunning = true;

            ChosenCommand = -1;
            WinnerID = -1;

            TieFlag = false;
            SelectBoardFlag = true;
            PlayAgainFlag = true;
            GameStarted = true;

            FirstPlayerID = FirstPlayer;
            SecondPlayerID = 3 - FirstPlayerID;

            for (int i = 0; i < 9; i++)
                Board[i] = new int[9];
            LargeBoard = new int[9];
        }

        public int Turn()
        {
            if (CurrentTurn == FirstPlayerID)
                return FirstPlayerID;
            else
                return SecondPlayerID;
        }
        // End Public Access Methods
    }
}
