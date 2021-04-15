using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace OlanderIA
{
    public partial class MainUI : Form
    {
        public event PingCC LogHumanMove;
        public delegate void PingCC(CommandCArgs e);

        public MainUI()
        {
            InitializeComponent();
        }
        // delegate string del(int i);
        CommunicationCenter CC;
        InternalDataTransfer IDT;
        UltimateTicTacToe UltTTT;
        TicTacToe TicTacToeSim;
        public InternalSimulator CurrentSim;
        string directory = "c:\\";
        int turn = 0;
        Random Rand = new Random();
        bool AIFirst = false;

        bool LoggingState = false;
        public bool Running = false;


        // 1 = TicTacToe
        // 2 = UltimateTicTacToe
        public int SimulatorID = 1;

        // 0 = Human vs Human
        // 1 = Human vs Ai
        // 2 = Ai vs Ai
        // 3 = Ai vs Baseline
        // 4 = Ai vs Random
        int PlayerMode = 0;
        int PlayMode = 1;

        //

        private void MainUI_Load(object sender, EventArgs e)
        {
            IDT = new InternalDataTransfer();
            TicTacToeSim = new TicTacToe();
            CurrentSim = TicTacToeSim;
            CC = new CommunicationCenter(IDT, CurrentSim, this);
            Subscribe(CC);
            // CC.LoadSimulation("Simulator.cs", "");

            // This is how to setup methods with reflection
            //object[] test3 = { 1 };
            //Type test = GetType();
            //Dictionary<int, MethodInfo> temp = new Dictionary<int, MethodInfo>();
            //temp.Add(1, test.GetMethod("printString"));
            //label1.Text = (string)temp[1].Invoke(this, test3);// new object[] { 1 });

        }
        public string PrintString(int i)
        {
            return "" + i;
        }

        private string ReturnFilePath()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = directory;
            openFileDialog1.Filter = "cs files (*.cs)|*.cs|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    directory = Path.GetDirectoryName(openFileDialog1.FileName);
                    return openFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
            
            return "";
        }
        private void TestData1()
        {
            int[][] testint = new int[11][];
            testint[0] = new int[] { 10 };
            for (int i = 1; i < 11; i++)
            {
                testint[i] = new int[4];
            }
            testint[1][0] = 1;
            testint[2][1] = 1;
            testint[3][2] = 1;
            testint[4][3] = 1;

            testint[5][0] = 5;
            testint[6][1] = 5;
            testint[7][2] = 5;
            testint[8][3] = 5;

            testint[9][0] = 1;
            testint[9][1] = 1;
            testint[9][2] = 1;
            testint[9][3] = 1;

            testint[10][0] = 5;
            testint[10][1] = 5;
            testint[10][2] = 5;
            testint[10][3] = 5;
            IDT.setIntArgs(testint);
        }
        private void TestData2()
        {
            int[][] testint = new int[1][];
            testint[0] = new int[3];
            testint[0][0] = 5;
            testint[0][1] = 5;
            testint[0][2] = 5;
            IDT.setIntArgs(testint);
        }
        private void GoTestData()
        {
            int[][] testint = new int[1][];
            testint[0] = new int[3];
            testint[0][0] = 127;
            testint[0][1] = 127;
            testint[0][2] = 127;
            IDT.setIntArgs(testint);
        }
        private void TestCalculationData1()
        {
            object[][] testData = new object[1][];
            testData[0] = new object[15];
            for (int i = 1; i <= 5; i++)
            {
                testData[0][i - 1] = i;
                testData[0][i + 9] = i;
            }
            for (int i = 5; i < 10; i++)
            { testData[0][i] = true; }
            IDT.setObjectArgs(testData);
        }
        private void TestCalculationData2()
        {
            object[][] testData = new object[1][];
            testData[0] = new object[381];
            for (int i = 1; i <= 127; i++)
            {
                testData[0][i - 1] = i;
                testData[0][i + 253] = i;
            }
            for (int i = 127; i < 254; i++)
            { testData[0][i] = true; }
            IDT.setObjectArgs(testData);
        }
        // Some test methods I'm too lazy to call with a bunch of links blah blah blasdfh
        private int[][] GenerateCombinations(int size, int[][] inputArgs)
        {
            if (size <= 0)
                return inputArgs;
            else
            {
                int[][] newArgs = new int[GetSize(inputArgs)][];
                for (int i = 0; i < newArgs.Length; i++)
                    newArgs[i] = new int[inputArgs[0].Length + 1];
                // The ugly looking generation for the next set
                bool complete = false;
                int currentVal = 1;
                int currentIndex = 0;
                while (!complete)
                {
                    complete = true;
                    for (int i = 0; i < inputArgs.Length; i++)
                    {
                        if (inputArgs[i][0] > currentVal)
                        {
                            newArgs[currentIndex][0] = currentVal;
                            for (int j = 0; j < inputArgs[0].Length; j++)
                            {
                                newArgs[currentIndex][j + 1] = inputArgs[i][j];
                                complete = false;
                            }
                            currentIndex++;
                        }
                    }
                    currentVal++;
                }
                return GenerateCombinations((size - 1), newArgs);
            }
        }
        private int GetSize(int[][] inputArgs)
        {
            if (inputArgs.Length > 0)
            {
                int max = 1;
                for (int i = 0; i < inputArgs.Length; i++)
                {
                    if (inputArgs[i][0] > max)
                        max = inputArgs[i][0];
                }
                int count = 0;
                for (int i = 1; i < max; i++)
                {
                    for (int l = 0; l < inputArgs.Length; l++)
                    {
                        if (inputArgs[l][0] > i)
                            count++;
                    }
                }
                return count;
            }
            else
                return 0;
        }
        // End of random methods

        private void LoadThingy_Click(object sender, EventArgs e)
        {
            CC.LoadSimulation(ReturnFilePath(), "");
        }

        private void OutputButton_Click(object sender, EventArgs e)
        {
            object[] temp = CC.PassResults();
        }

        private void SimSetup_Click(object sender, EventArgs e)
        {
            if (!Running)
            {
                try
                {
                    CurrentSim = TicTacToeSim;
                    SimulatorID = 1;
                    int CombinationLevel = Convert.ToInt32(CombLevel.Text);
                    if (CombinationLevel > 0 && CombinationLevel < 10)
                        SimSetupButton(CombinationLevel, CurrentSim);
                }
                catch (FormatException)
                {
                    OtherLogBox.AppendText("Invalid input" + Environment.NewLine);
                }
            }
            else
                OtherLogBox.AppendText("Cannot change Simulator while running" + Environment.NewLine);
        }

        public void SimSetupButton(int CombinationLevel, InternalSimulator InSim)
        {
            int[][] temp = new int[1][];
            temp[0] = new int[1];
            temp[0][0] = CombinationLevel;
            IDT.setIntArgs(temp);
            //IDT.setStringArgs(InSim.GetCommandNames()); This was implemented inside InternalSimulatorSetup;
            CC.InternalSimulatorSetup(LoggingState, InSim);
            foreach (CommandCenter l in CC.ReturnCC())
            {
                Subscribe(l);
                foreach (NodeCluster m in l.ReturnClusters())
                    Subscribe(m);
            }
        }

        private void UL_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (UL.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 0;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 0)
                        {
                            UL.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 0)
                        {
                            UL.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }

        private void UM_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (UM.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 1;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 1)
                        {
                            UM.Text = "O";
                            LogHumanMove(new CommandCArgs( new int[] {TicTacToeSim.ReturnChosenCommand(), 1}));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 1)
                        {
                            UM.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }

        private void UR_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (UR.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 2;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 2)
                        {
                            UR.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 2)
                        {
                            UR.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }

        private void ML_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (ML.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 3;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 3)
                        {
                            ML.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 3)
                        {
                            ML.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }

        private void MM_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (MM.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 4;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 4)
                        {
                            MM.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 4)
                        {
                            MM.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }

        private void MR_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (MR.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 5;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 5)
                        {
                            MR.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 5)
                        {
                            MR.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }

        private void LL_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (LL.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 6;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 6)
                        {
                            LL.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 6)
                        {
                            LL.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)   
                        AiMove();
                }
            }
        }

        private void LM_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (LM.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 7;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 7)
                        {
                            LM.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 7)
                        {
                            LM.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }

        private void LR_Click(object sender, EventArgs e)
        {
            if (!TicTacToeSim.Finished())
            {
                if (LR.Text == "")
                {
                    object[] Decision = new object[1];
                    Decision[0] = new object[1];
                    ((object[])Decision[0])[0] = 8;
                    TicTacToeSim.SetCommand(Decision);
                    if (turn == 1)
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 8)
                        {
                            LR.Text = "O";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn--;
                        }
                    }
                    else
                    {
                        if (TicTacToeSim.ReturnChosenCommand() == 8)
                        {
                            LR.Text = "X";
                            LogHumanMove(new CommandCArgs(new int[] { TicTacToeSim.ReturnChosenCommand(), 1 }));
                            turn++;
                        }
                    }
                    if (TicTacToeSim.Finished())
                        PrintResults(TicTacToeSim.GetUnweightedResult(1));
                    else if (PlayerMode == 1)
                        AiMove();
                }
            }
        }
        private void PrintResults(int result)
        {
            if (result == 0)
                Results.Text = "It was a tie";
            else if (turn%2 == 0)
                Results.Text = "O Player Won!";
            else if (turn%2 == 1)
                Results.Text = "X Player Won!";

            CC.EndRound(CurrentSim); //This ends all of the command centers for us.
            Running = false;
        }
        public void AiMove()
        {
            if (SimulatorID == 1)
            {
                PrintCommand(CC.InternalAITurn(CurrentSim));
                if (turn % 2 == 0)
                    turn++;
                else
                    turn--;
                if (CurrentSim.Finished())
                    PrintResults(CurrentSim.GetUnweightedResult(1));
                Running = false;
            }
            else if (SimulatorID == 2)
            {
                UltTTT.PrintCommand(CC.InternalAITurn(CurrentSim));
                if (CurrentSim.Finished())
                    UltTTT.PrintResults(CurrentSim.GetUnweightedResult(1));
            }
            long[] Temp = CC.GetDecisionTime(0);
            for (int i = 0; i < Temp.Length; i++)
            {
                //OtherLogBox.AppendText("" + Temp[i] + Environment.NewLine);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            StartButton();
        }
        private void StartButton()
        {
            // 0 = Human vs Human
            // 1 = Human vs Ai
            // 2 = Ai vs Ai
            // 3 = Ai vs Baseline
            // 4 = Ai vs Random
            Running = true;
            if (PlayerMode == 2)
            {
                try
                {
                    int temp = Convert.ToInt32(Count.Text);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    for (int i = 1; i <= temp; i++)
                    {
                        CC.StartRound(CurrentSim);
                        LearningPlayingMode();
                        if (SimulatorID == 1)
                            SetupBoard();
                        else if (SimulatorID == 2)
                        {
                            UltTTT.SetupBoard();
                        }

                        if (i % 50 == 0)
                            OtherLogBox.AppendText(i + Environment.NewLine);
                    }
                    sw.Stop();
                    Running = false;
                    OtherLogBox.AppendText(sw.ElapsedMilliseconds + Environment.NewLine);
                }
                catch (FormatException)
                {
                    OtherLogBox.AppendText("Invalid input" + Environment.NewLine);
                    Running = false;
                }
            }
            else if (PlayerMode == 4)
            {
                try
                {
                    CC.AiVRandom(CurrentSim, Convert.ToInt32(Count.Text));
                    Running = false;
                }
                catch (FormatException)
                {
                    OtherLogBox.AppendText("Invalid input" + Environment.NewLine);
                    Running = false;
                }
            }
            else if (PlayerMode == 3)
            {
                CC.BaseLineTest(CurrentSim);
                string[] temp = CC.GetBaseLineResults();
                for (int i = 0; i < temp.Length; i++)
                {
                    if (SimulatorID == 1)
                        LogBox.AppendText(temp[i] + Environment.NewLine);
                    else if (SimulatorID == 2)
                        UltTTT.LogInformation(temp[i]);
                }
                Running = false;
            }
            else
            {
                if (SimulatorID == 1)
                    SetupBoard();
                else if (SimulatorID == 2)
                    UltTTT.SetupBoard();
            }
        }
        private void BaseLineStartButton()
        {
            CC.BaseLineTest(CurrentSim);
        }
        private void LearningPlayingMode()
        {
            if (PlayMode == 0)
            {
                //int temp = Rand.Next(0, 3);
                //CC.SetCommandCenterChoiceMode(temp);
                CC.SetCommandCenterChoiceMode(Rand);
            }
            else if (PlayMode == 1) // If it is playing then we want it to choose the best choice everytime.
                CC.SetCommandCenterChoiceMode(0);
        }
        private void SetupBoard()
        {
            CC.StartRound(TicTacToeSim);

            turn = 0;
            Results.Text = "";
            UL.Text = "";
            UM.Text = "";
            UR.Text = "";
            ML.Text = "";
            MM.Text = "";
            MR.Text = "";
            LL.Text = "";
            LM.Text = "";
            LR.Text = "";

            // 0 = Human vs Human
            // 1 = Human vs Ai
            // 2 = Ai vs Ai
            // 3 = Ai vs Baseline
            // 4 = Ai vs Random
            if (PlayerMode == 1) //Human Vs. AI
            {

                CC.StartRound(0, 2);
                if (TicTacToeSim.Turn() == 2)
                {
                    AiMove();
                    AIFirst = true;
                }
                else AIFirst = false;
            }
            if (PlayerMode == 2)
            {
                CC.StartRound(0, 1);
                CC.StartRound(1, 2);
                int Temporary = 0;
                while (Temporary < 9)
                {
                    if (!TicTacToeSim.Finished())
                        AiMove();

                    Temporary++;
                }
            }
        }
        private void PrintCommand(int CommandNumber)
        {
            if (turn % 2 == 1)
            {
                if (CommandNumber == 0)
                    UL.Text = "O";
                if (CommandNumber == 1)
                    UM.Text = "O";
                if (CommandNumber == 2)
                    UR.Text = "O";
                if (CommandNumber == 3)
                    ML.Text = "O";
                if (CommandNumber == 4)
                    MM.Text = "O";
                if (CommandNumber == 5)
                    MR.Text = "O";
                if (CommandNumber == 6)
                    LL.Text = "O";
                if (CommandNumber == 7)
                    LM.Text = "O";
                if (CommandNumber == 8)
                    LR.Text = "O";
            }
            else if (turn % 2 == 0)
            {
                if (CommandNumber == 0)
                    UL.Text = "X";
                if (CommandNumber == 1)
                    UM.Text = "X";
                if (CommandNumber == 2)
                    UR.Text = "X";
                if (CommandNumber == 3)
                    ML.Text = "X";
                if (CommandNumber == 4)
                    MM.Text = "X";
                if (CommandNumber == 5)
                    MR.Text = "X";
                if (CommandNumber == 6)
                    LL.Text = "X";
                if (CommandNumber == 7)
                    LM.Text = "X";
                if (CommandNumber == 8)
                    LR.Text = "X";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Human vs Human
            CC.SetPlayerMode(0);
            CC.SetInternalGameMode(0);
            PlayerMode = 0;
            UIPlayModeLabel.Text = "Human vs Human";

            try { UltTTT.SetPlayerMode(0); }
            catch (Exception) { }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AiVAiButton();
        }
        private void AiVAiButton()
        {
            // Ai vs Ai
            CC.SetPlayerMode(2);
            CC.SetInternalGameMode(2);
            PlayerMode = 2;
            UIPlayModeLabel.Text = "Ai vs Ai";

            try { UltTTT.SetPlayerMode(2); }
            catch (Exception) { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Human vs Ai
            CC.SetPlayerMode(1);
            CC.SetInternalGameMode(1);
            PlayerMode = 1;
            UIPlayModeLabel.Text = "Ai vs Human";

            try { UltTTT.SetPlayerMode(1); }
            catch (Exception) { }
        }

        private void AiRandom_Click(object sender, EventArgs e)
        {
            // Ai vs Random
            CC.SetPlayerMode(4);
            CC.SetInternalGameMode(4);
            PlayerMode = 4;
            UIPlayModeLabel.Text = "Ai vs Random";

            try { UltTTT.SetPlayerMode(4); }
            catch (Exception) { }
        }

        private void Baseline_Click(object sender, EventArgs e)
        {
            BaseLineButton();
        }
        private void BaseLineButton()
        {
            // Ai vs Baseline
            CC.SetPlayerMode(3);
            CC.SetInternalGameMode(3);
            PlayerMode = 3;
            UIPlayModeLabel.Text = "Ai vs Baseline";

            try { UltTTT.SetPlayerMode(3); }
            catch (Exception) { }
        }

        private void Learning_Click(object sender, EventArgs e)
        {
            LearningButton();
        }
        private void LearningButton()
        {
            CC.SetPlayMode(0);
            PlayMode = 0;
            LPMode.Text = "Learning";
        }
        private void Playing_Click(object sender, EventArgs e)
        {
            PlayingButton();
        }
        private void PlayingButton()
        {
            CC.SetPlayMode(1);
            PlayMode = 1;
            LPMode.Text = "Playing";
        }

        private void Clr_Click(object sender, EventArgs e)
        {
            LogBox.Text = "";
        }

        private void AutoTest_Click(object sender, EventArgs e)
        {
            try
            {
                bool SwapLogging = LoggingState;


                int CycleLength = Convert.ToInt32(CycleL.Text);
                int RunCount = Convert.ToInt32(RunC.Text);
                Count.Text = "" + CycleLength;
                for (int i = 0; i < RunCount; i++)
                {
                    if (SwapLogging)
                        ToggleLogButton();
                    LearningButton();
                    AiVAiButton();
                    StartButton();
                    PlayingButton();
                    BaseLineButton();
                    BaseLineStartButton();
                    if (SwapLogging)
                        ToggleLogButton();

                    int[][] temp = CC.GetBaseLineArray(LoggingState);
                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (j != temp.Length - 1)
                            LogBox.AppendText(temp[j][0] + ", " + temp[j][1] + ", " + temp[j][2] + "  |  ");
                        else
                            LogBox.AppendText(temp[j][0] + ", " + temp[j][1] + ", " + temp[j][2]);
                    }
                    LogBox.AppendText(Environment.NewLine);
                }
            }
            catch(FormatException)
            {
                OtherLogBox.AppendText("Invalid input" + Environment.NewLine);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OtherLogBox.Text = "";
        }
        public void Subscribe(CommandCenter m)
        {
            m.UILog += new CommandCenter.LogUI(LogIt);
            m.SubNodeCluster += new CommandCenter.SubscribeItem(Subscribe);
        }
        public void Subscribe(CommunicationCenter m)
        {
            m.UILog += new CommunicationCenter.LogCommand(LogIt);
            m.SubCC += new CommunicationCenter.SubscribeItem(Subscribe);
        }
        public void Subscribe(NodeCluster m)
        {
            m.UILog += new NodeCluster.LogCommand(LogIt);
        }
        private void LogIt(CommandCenter m, string e)
        {
            OtherLogBox.AppendText(e);
            OtherLogBox.AppendText(Environment.NewLine);
        }
        private void LogIt(CommunicationCenter m, string e)
        {
            OtherLogBox.AppendText(e);
            OtherLogBox.AppendText(Environment.NewLine);
        }
        private void LogIt(NodeCluster m, string e)
        {
            OtherLogBox.AppendText(e);
            OtherLogBox.AppendText(Environment.NewLine);
        }

        private void OtherLogBox_TextChanged(object sender, EventArgs e)
        {

        }
        // This is still in the works
        private void SaveCC_Click(object sender, EventArgs e)
        {
            Stream StreamWriter;
            SaveFileDialog Dialog1 = new SaveFileDialog();

            Dialog1.Filter = "cc files (*.cc)|*.cc|All files (*.*)|*.*";
            Dialog1.FilterIndex = 1;
            Dialog1.RestoreDirectory = true;
            if (Dialog1.ShowDialog() == DialogResult.OK)
            {
                if ((StreamWriter = Dialog1.OpenFile()) != null)
                {
                    CommandCenter[] tempCC = CC.ReturnCC();
                    CommandCenter BaseL = CC.ReturnBaseL();
                    Byte[] Count = new Byte[1];
                    Count[0] = Convert.ToByte(tempCC.Length + 1);
                    StreamWriter.Write(Count, 0, 1);
                    Count = new Byte[1];
                    NodeCluster[] NC = tempCC[0].ReturnClusters();
                    Byte[] temp = BitConverter.GetBytes(NC.Length);
                    Count[0] = Convert.ToByte(temp.Length);
                    StreamWriter.Write(Count, 0, 1);
                    StreamWriter.Write(temp, 0, (temp.Length));
                    temp = BitConverter.GetBytes(tempCC[0].ReturnUntrimmedCount());
                    Count[0] = Convert.ToByte(temp.Length);
                    StreamWriter.Write(Count, 0, 1);
                    StreamWriter.Write(temp, 0, temp.Length);
                    temp = BitConverter.GetBytes(tempCC[0].ReturnTrimmedCount());
                    Count[0] = Convert.ToByte(temp.Length);
                    StreamWriter.Write(Count, 0, 1);
                    StreamWriter.Write(temp, 0, temp.Length);
                    for (int i = 0; i < tempCC.Length + 1; i++)
                    {
                        if (i != tempCC.Length)
                        {
                            NC = tempCC[i].ReturnClusters();
                            for (int j = 0; j < NC.Length; j++)
                            {

                            }
                        }
                        else
                        {

                        }
                    }
                    StreamWriter.Close();
                }
            }
        }

        private void LoadCC_Click(object sender, EventArgs e)
        {

        }

        private void Help_Click(object sender, EventArgs e)
        {
            string Message = "   Welcome to the help thingy. In this concise 26,000 pages, you will learn how to operate this ever so simple thing..."
                + " Well not really, this is pretty straightforward." + Environment.NewLine
                + " First things first, there are two different boxes used for logging. The smaller one on the left"
                + " is used for updates about the generation, and completion of things internally. The one on the right is used for displaying results."
                + " There are two small blank buttons next to each log box, these just clear them." + Environment.NewLine
                + "   Now onto the buttons. There are 3 groups of buttons. One for setup, one for playing, and one for testing." + Environment.NewLine
                + "   The generation buttons are called 'Load Simulation' and 'Setup'. Load Simulation does as expected, it allows you to load your own simulation."
                + " It doesn't really work so I don't recommend trying to use it, although I probably shouldn't say that... Anyway. "
                + " The Setup button has a small box beneath it, this is for setting the combination level of the nodes generated. In short, the higher the number, the more nodes."
                + " The lowest it can be is 1, the highest is the number of commands in the simulater. The internal one, being Tic-Tac-Toe, has 9 decisions."
                + "   Onto the playing buttons. There are 5 of these, each one corresponds to a specific mode the simulator can be run in. They are mostly self explanatory, but" + Environment.NewLine
                + " There is one thing to mention. Ai vs Ai, and Ai vs Random require a number to be in the box below the start button. This corresponds to the number of games to be run"
                + " in this mode. I strongly recommend not going over 1,000, but if you want to, you could go as high as 10,000, or even higher. If your processor is fast it should be okay." + Environment.NewLine
                + "   There are also modes for Playing or Learning. These should make sense, in learning it takes a little longer, but they learn. While playing they will always"
                + " pick what they think is the best move." + Environment.NewLine
                + "   Finally, the testing buttons. These are used to run repeated cycles of Ai vs Ai learning, then Ai vs BaseLine."
                + " Runs corresponds with the number of times to do each cycle, and Cycle Length corresponds to the number of games the Ais will play against eachother. It will print the result of each"
                + " baseline test in the right hand log, in the style of #|#|#. This can be interpretted as Wins|Ties|Losses." + Environment.NewLine
                + "   As for the baseline test itself, this puts each Ai against a third Ai that never learns, and they play 30 differernt games against eachother. The wins/ties/lossses are tallied"
                + "   Then displayed in the right hand log." + Environment.NewLine
                + "   The 3 by 3 of buttons in the center represents the game board. When playing as the person, simply click one of these to play your move." + Environment.NewLine 
                + "   I hope this covers everything, and goodluck!";
            string caption = "The Help Is On The Way... I hope...";
            DialogResult Result = MessageBox.Show(Message, caption);
        }

        private void ToggleLog_Click(object sender, EventArgs e)
        {
            ToggleLogButton();
        }

        private void ToggleLogButton()
        {
            //This doesn't really need to be this way, since now the logging state can be set the moment the CC is created based on what this LoggingState is.
            // So now CC.ToggleLogging() always returns true.
            bool passed = CC.ToggleLogging();
            if (passed)
                LoggingState = !LoggingState;
            if (LoggingState)
                ToggleLog.Text = "Turn Logging Off";
            else
                ToggleLog.Text = "Turn Logging On";
        }

        private void UTTT_Click(object sender, EventArgs e)
        {
            UltTTT = new UltimateTicTacToe(this, CC, PlayerMode);
            UltTTT.Show();
        }
    }
}