using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OlanderIA
{
    public partial class UltimateTicTacToe : Form
    {
        // Drawing Variables and events
        Graphics GR;
        Rectangle Background;
        Rectangle[] SubTiles;
        Rectangle[] LargeTiles;
        SolidBrush BackgroundBrush;
        SolidBrush SubButton;
        SolidBrush SelectedSubButton;
        // End drawing
        private int buttonSize = 40;
        private int buttonPadding = 10;
        private int boardPadding = 20;


        Point[] PlacedX = new Point[81];
        Point[] PlacedO = new Point[81];

        int PointerX = 0;
        int PointerO = 0;
        int CurrentBoard = -1;
        // 0 = Human vs Human
        // 1 = Human vs Ai
        // 2 = Ai vs Ai
        // 3 = Ai vs Baseline
        // 4 = Ai vs Random
        public int PlayerMode = 0;
        UltTicTacToe UltSim;
        MainUI MUI;
        CommunicationCenter CC;

        int TurnID = 0;

        public UltimateTicTacToe(MainUI MUI, CommunicationCenter CC, int PlayerMode)
        {
            InitializeComponent();

            this.MUI = MUI;
            this.CC = CC;
            this.PlayerMode = PlayerMode;

            // Board Setup
            BackgroundBrush = new SolidBrush(Color.FromArgb(40, 40, 40));
            SubButton = new SolidBrush(Color.FromArgb(100, 100, 100));
            SelectedSubButton = new SolidBrush(Color.FromArgb(255, 204, 0));

            SubTiles = new Rectangle[81];

            int yPointer = buttonPadding;
            for (int i = 0; i < 9; i++)
            {
                int xPointer = buttonPadding;
                for (int k = 0; k < 9; k++)
                {
                    SubTiles[(i * 9) + k] = new Rectangle(yPointer, xPointer, buttonSize, buttonSize);
                    xPointer += buttonSize;
                    if (k == 2 || k == 5)
                        xPointer += boardPadding;
                    else
                        xPointer += buttonPadding;
                }
                yPointer += buttonSize;
                if (i == 2 || i == 5)
                    yPointer += boardPadding;
                else
                    yPointer += buttonPadding;
            }
            // End setup
        }
        public void SetPlayerMode(int PlayerMode)
        {
            // 0 = Human vs Human
            // 1 = Human vs Ai
            // 2 = Ai vs Ai
            // 3 = Ai vs Baseline
            // 4 = Ai vs Random
            this.PlayerMode = PlayerMode;
            if (PlayerMode == 0)
                ModeDisplayer.Text = "Human vs Human";
            else if (PlayerMode == 1)
                ModeDisplayer.Text = "Human vs Ai";
            else if (PlayerMode == 2)
                ModeDisplayer.Text = "Ai vs Ai";
            else if (PlayerMode == 3)
                ModeDisplayer.Text = "Ai vs Baseline";
            else if (PlayerMode == 4)
                ModeDisplayer.Text = "Ai vs Random";
        }
        private void Setup_Click(object sender, EventArgs e)
        {
            if (!MUI.Running)
            {
                try
                {
                    MUI.SimulatorID = 2;
                    MUI.CurrentSim = UltSim;
                    int CombLevel = Convert.ToInt32(CombinationLevel.Text);
                    if (CombLevel > 0 && CombLevel < 91)
                        MUI.SimSetupButton(CombLevel, MUI.CurrentSim);
                }
                catch
                {
                    UltLogBox.AppendText("Error parsing combination level" + Environment.NewLine);
                }
            }
            else
                UltLogBox.AppendText("Cannot change Simulator while running" + Environment.NewLine);
        }

        public void SetupBoard()
        {
            PointerX = 0;
            PointerO = 0;
            CurrentBoard = -1;
            TurnID = 0;
            SelectFlag.Text = "Choose a board";
            UltSim.Start();
            BoardBox.Invalidate();

            if (PlayerMode == 2)
            {
                CC.StartRound(0, 1);
                CC.StartRound(1, 2);
                int Temporary = 0;
                while (Temporary < 150)
                {
                    if (!MUI.CurrentSim.Finished())
                        MUI.AiMove();
                    else
                        break;

                    Temporary++;
                }
            }
        }

        public void PrintResults(int WinnerID)
        {
            Winner.Text = "WinnerID";
            CC.EndRound(UltSim);
        }

        private void UltimateTicTacToe_Load(object sender, EventArgs e)
        {
            GR = CreateGraphics();
            BoardBox.Size = new Size((buttonSize * 9) + (buttonPadding * 8) + (boardPadding * 2), (buttonSize * 9) + (buttonPadding * 8) + (boardPadding * 2));
            UltSim = new UltTicTacToe();
            //DrawBoard(10, 10);
        }

        public void LogInformation(string Text)
        {
            UltLogBox.AppendText(Text + Environment.NewLine);
        }

        private void BoardBox_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Background = new Rectangle(0, 0, (buttonSize * 9) + (buttonPadding * 8) + (boardPadding * 2), (buttonSize * 9) + (buttonPadding * 8) + (boardPadding * 2));
            //e.Graphics.DrawRectangle(new Pen(Color.FromArgb(10, 10, 10)), Background);
            e.Graphics.FillRectangle(BackgroundBrush, Background);
            for (int i = 0; i < SubTiles.Length; i++)
            {
                e.Graphics.FillRectangle(SubButton, SubTiles[i]);
            }
            if (CurrentBoard != -1)
            {
                int BasePoint = ((CurrentBoard % 3) * 27) + ((CurrentBoard / 3) * 3);
                for (int i = 0; i < 3; i++)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        int Index = BasePoint + (k * 9);
                        e.Graphics.FillRectangle(SelectedSubButton, SubTiles[Index]);
                    }
                    BasePoint++;
                }
            }

            for (int i = 0; i < PointerX; i++)
                e.Graphics.DrawString("X", new Font(FontFamily.GenericMonospace, 30), Brushes.White, PlacedX[i]);
            for (int i = 0; i < PointerO; i++)
                e.Graphics.DrawString("O", new Font(FontFamily.GenericMonospace, 30), Brushes.White, PlacedO[i]);
        }

        private void UltimateTicTacToe_MouseDown(object sender, MouseEventArgs e)
        {

        }

        public void PrintCommand(int CommandNumber)
        {
            if (CommandNumber < 9)
            {
                int LargeX = CurrentBoard % 3;
                int LargeY = CurrentBoard / 3;
                int SmallX = CommandNumber % 3;
                int SmallY = CommandNumber / 3;

                int BoardSize = (3 * buttonSize) + (2 * buttonPadding) + boardPadding;
                int PointX = (SmallX + 1) * buttonPadding + SmallX * buttonSize + LargeX * BoardSize;
                int PointY = (SmallY + 1) * buttonPadding + SmallY * buttonSize + LargeY * BoardSize;

                Point Temp = new Point(PointX, PointY);

                if (TurnID == 0)
                {
                    PlacedX[PointerX] = Temp;
                    PointerX += 1;
                    TurnID++;
                }
                else if (TurnID == 1)
                {
                    PlacedO[PointerO] = Temp;
                    PointerO += 1;
                    TurnID--;
                }
                CurrentBoard = CommandNumber;
            }
            else
                CurrentBoard = (CommandNumber - 9);
            BoardBox.Invalidate();
        }

        private void SwapBoard(int Board)
        {
            if (!UltSim.Finished())
            {
                if (PlayerMode == 0 || PlayerMode == 1)
                {
                    object[] Move = new object[1];
                    object[] TempMove = new object[1];
                    TempMove[0] = (double)(Board + 9);
                    Move[0] = TempMove;

                    UltSim.SetCommand(Move);
                    if (UltSim.ReturnChosenCommand() == Board + 9)
                    {
                        CurrentBoard = Board;
                        SelectFlag.Text = "";
                        BoardBox.Invalidate();
                    }
                }
            }
        }

        private bool PlayMove(int SelectedMove)
        {
            if (!UltSim.Finished())
            {
                if (PlayerMode == 0 || PlayerMode == 1)
                {
                    object[] Move = new object[1];
                    object[] TempMove = new object[1];
                    TempMove[0] = (double)SelectedMove;
                    Move[0] = TempMove;

                    UltSim.SetCommand(Move);
                    return (UltSim.ReturnChosenCommand() == SelectedMove);
                }
            }
            return false;
        }

        private void ULBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(0);
        }

        private void UMBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(1);
        }

        private void URBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(2);
        }

        private void MLBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(3);
        }

        private void MMBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(4);
        }

        private void MRBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(5);
        }

        private void LLBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(6);
        }

        private void LMBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(7);
        }

        private void LRBoard_Click(object sender, EventArgs e)
        {
            SwapBoard(8);
        }

        private void BoardBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (CurrentBoard != -1)
            {
                int PositionX = e.X - BoardBox.Location.X - buttonPadding;
                int PositionY = e.Y - BoardBox.Location.Y - buttonPadding;

                int BoardOffset = buttonSize * 3 + buttonPadding * 2 + boardPadding;

                int XOffset = CurrentBoard % 3;
                int YOffset = CurrentBoard / 3;

                PositionX -= (BoardOffset * XOffset);
                PositionY -= (BoardOffset * YOffset);

                int XLocation = -1;
                int YLocation = -1;

                int Counter = 0;
                while (PositionX >= 0)
                {
                    if (Counter == 1)
                        break;
                    for (int i = 0; i < 3; i++)
                    {
                        if (PositionX <= buttonSize && PositionX >= 0)
                            XLocation = i;
                        PositionX -= (buttonSize + buttonPadding);
                    }
                    PositionX = (PositionX - boardPadding) + buttonPadding;
                    Counter++;
                }
                Counter = 0;
                while (PositionY >= 0)
                {
                    if (Counter == 1)
                        break;
                    for (int i = 0; i < 3; i++)
                    {
                        if (PositionY <= buttonSize && PositionY >= 0)
                            YLocation = i;
                        PositionY -= (buttonSize + buttonPadding);
                    }
                    PositionY = (PositionY - boardPadding) + buttonPadding;
                    Counter++;
                }
                if (YLocation > -1 && XLocation > -1)
                {
                    int AssembledMove = (YLocation * 3) + XLocation;
                    if (PlayMove(AssembledMove))
                        PrintCommand(AssembledMove);
                }
            }
        }

        private void TempStart_Click(object sender, EventArgs e)
        {
            UltSim.Start();
            SetupBoard();
        }
    }
}