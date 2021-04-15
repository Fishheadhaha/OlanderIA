using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;

namespace OlanderIA
{
    public class CommunicationCenter
    {
        private int CommandCenterCount = 0;
        private int BaseLineCCCount = 0;
        // Logging
        public event LogCommand UILog;
        public event LogCommandNumber CCLog;
        public event SubscribeItem SubCC;
        public delegate void LogCommand(CommunicationCenter m, string e);
        public delegate void LogCommandNumber(CommunicationCenter m, CommandCArgs e);
        public delegate void SubscribeItem(CommandCenter e);
        //
        // The other objects that the Communication Center references
        CommandCenter[] CC;
        CommandCenter BaseL;
        Random Rnd;
        Assembly sim;
        Type test;
        InternalDataTransfer IDT;
        Dictionary<int, MethodInfo> SimMethods = new Dictionary<int, MethodInfo>();
        Randomizer RandomInput;

        int[][] BaseLineResults;
        // Temporary
        InternalSimulator TestSimulator;

        // Simulator Mode
        // 1 = Internal Simulator
        public int SimulatorMode = 1;

        object Instance;


        int[][][] UntrimmedCombinations;
        bool RecalculateFlag = true;
        
        // Mode 0 = Learning
        // Mode 1 = Playing
        // Mode 2 = Userinput (The user inputs all the data, and retrieves the result)
        int PlayMode = 1;

        // 0 = Human vs Human
        // 1 = Human vs Ai
        // 2 = Ai vs Ai
        int PlayerMode = 0;

        int currentCommandNumber = -1;

        public CommunicationCenter(MainUI M)
        {
            IDT = new InternalDataTransfer();
            Rnd = new Random();
            M.LogHumanMove += new MainUI.PingCC(LogHumanMove);
        }
        public CommunicationCenter(InternalDataTransfer IDT, MainUI M)
        {
            this.IDT = IDT;
            Rnd = new Random();
            M.LogHumanMove += new MainUI.PingCC(LogHumanMove);
        }
        public CommunicationCenter(InternalDataTransfer IDT, InternalSimulator testSim, MainUI M)
        {
            this.IDT = IDT;
            TestSimulator = testSim;
            Rnd = new Random();
            M.LogHumanMove += new MainUI.PingCC(LogHumanMove);
        }
        // Test Methods
        public void CreateCommandCenter(bool InitialLoggingState)
        {
            CC = new CommandCenter[1];
            CC[0] = new CommandCenter(IDT, "" + CommandCenterCount, InitialLoggingState, this);
            CommandCenterCount++;
            SubCC(CC[0]);
        }
        public void CreateFullCommandCenter(bool InitialLoggingState)
        {
            CC = new CommandCenter[1];
            CC[0] = new CommandCenter(IDT, "" + CommandCenterCount, InitialLoggingState, this);
            CommandCenterCount++;
            SubCC(CC[0]);
            CC[0].GetArrays();
            CC[0].CreateClustersFromTemplate1();
        }
        //
        public void UpdateSim(InternalSimulator NewSim)
        {
            TestSimulator = NewSim;
            RecalculateFlag = true;
        }
        // Some method group
        private int NodeCount(int inputCount, int combinationLevel)
        {
            int count = inputCount;
            int factorial = 1;
            for (int i = 1; i < combinationLevel; i++)
            {
                count *= (inputCount - i);
                factorial *= (i + 1);
            }
            count /= factorial;
            if (combinationLevel < 1)
                count = 1;
            return count;
        }
        public int EstimateNodeCount(int inputCount, int combinationLevel)
        {
            int count = 0;
            for (int i = 1; i <= combinationLevel; i++)
            {
                count += NodeCount(inputCount, i);
            }
            return count;
        }
        // 
        public void LoadSimulation(string path, string nameSpace)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            parameters.ReferencedAssemblies.Add(Assembly.GetEntryAssembly().Location);
            try
            {
                UILog(this, path + " | " + nameSpace);
                try
                {
                    CompilerResults results = provider.CompileAssemblyFromFile(parameters, @path);
                    if (results.Errors.HasErrors)
                    {
                        string errors = "";
                        foreach (CompilerError error in results.Errors)
                        {
                            errors += string.Format("Error #{0}: {1}\n", error.ErrorNumber, error.ErrorText);
                        }
                        UILog(this, errors);
                    }
                    else
                    {
                        sim = results.CompiledAssembly;
                        if (nameSpace != "")
                            nameSpace += ".";
                        // 
                        bool next = true;
                        // This is redundant, but it throws an error if it isn't like this.
                        test = this.GetType();
                        //
                        try
                        {
                            test = sim.GetType(nameSpace + Path.GetFileNameWithoutExtension(@path));
                        }
                        catch (Exception)
                        {
                            UILog(this, "Error: Either incorrect NameSpace (leave blank if there is none) or the name of the .cs file doesn't match the name of the class");
                            next = false;
                        }
                        if (next)
                        {
                            SimMethods = new Dictionary<int, MethodInfo>();
                            try
                            {
                                SimMethods.Add(1, test.GetMethod("GetCommandSignature"));
                                SimMethods.Add(2, test.GetMethod("GetDataSignature"));
                                SimMethods.Add(3, test.GetMethod("GetCommandNames"));
                                SimMethods.Add(4, test.GetMethod("GetData", new Type[] { typeof(int) } ));
                                SimMethods.Add(5, test.GetMethod("SetCommand"));
                                SimMethods.Add(6, test.GetMethod("ReturnChosenCommand"));
                                SimMethods.Add(7, test.GetMethod("Turn"));
                                SimMethods.Add(8, test.GetMethod("SetGameMode"));
                                SimMethods.Add(9, test.GetMethod("Start", new Type[] { } ));
                                SimMethods.Add(10, test.GetMethod("Start", new Type[] { typeof(int) } ));
                                SimMethods.Add(11, test.GetMethod("Finished"));
                                SimMethods.Add(12, test.GetMethod("GetResult"));
                                SimMethods.Add(13, test.GetMethod("ReturnRecommendedRange"));
                                SimMethods.Add(14, test.GetMethod("GetPlayerCount"));
                                SimMethods.Add(15, test.GetMethod("GetUnweightedResult"));
                                object Instance = Activator.CreateInstance(test);
                            }
                            catch (Exception)
                            {
                                UILog(this, "Error: Make sure the simulator class contains all the required methods.");
                            }
                        }
                    }
                }
                catch(Exception)
                {
                    UILog(this, "Error");
                }
            }
            catch (IOException)
            {
                UILog(this, "Error: .cs File not found");
            }
        }
        public CommandCenter[] ReturnCC()
        {
            return CC;
        }
        // Copy Methods
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
        public void CalculateCombinations(int NodeCombinationLimit, int[][] DataSig)
        {
            int[][] startArray = new int[DataSig[0][0] + DataSig[0][1] + DataSig[0][2]][];
            for (int i = 0; i < startArray.Length; i++)
            {
                startArray[i] = new int[1];
                startArray[i][0] = i + 1;
            }
            int[][][] finalArray = new int[NodeCombinationLimit][][];
            finalArray[0] = TwoDimCopy(startArray);
            int pos = 1;
            for (int i = 1; i < NodeCombinationLimit; i++)
            {
                startArray = GenerateCombinations(1, startArray);
                finalArray[pos] = TwoDimCopy(startArray);
                pos++;
            }
            for (int i = 0; i < finalArray.Length; i++)
            {
                for (int l = 0; l < finalArray[i].Length; l++)
                {
                    for (int k = 0; k < finalArray[i][l].Length; k++)
                    {
                        finalArray[i][l][k] -= 1;
                    }
                }
            }
            UntrimmedCombinations = finalArray;
            RecalculateFlag = false;
        }
        public int[][] TwoDimCopy(int[][] inputArray)
        {
            int[][] tempArray = new int[inputArray.Length][];
            for (int i = 0; i < inputArray.Length; i++)
            {
                tempArray[i] = new int[inputArray[i].Length];
                for (int l = 0; l < inputArray[i].Length; l++)
                {
                    tempArray[i][l] = inputArray[i][l];
                }
            }
            return tempArray;
        }
        public void SetupCommandCenters(bool InitialLoggingState)
        {
            if (SimMethods.Count > 0)
            {
                CC = new CommandCenter[((int)InvokeDictionaryMethod(13, new object[0]))];
                for (int i = 0; i < CC.Length; i++)
                {
                    CC[i] = new CommandCenter(IDT, "" + CommandCenterCount, InitialLoggingState, this);
                    CommandCenterCount++;
                    SubCC(CC[i]);
                    int temp = (int)IDT.getItem(3, 0, 0);
                    int[][] CommandSig = (int[][])InvokeDictionaryMethod(1, new object[0]);
                    IDT.setIntArgs(CommandSig);
                    CC[i].GetArrays();
                    CC[i].CreateClustersFromTemplate1();
                    int[][] DataSig = new int[1][];
                    DataSig[0] = (int[])InvokeDictionaryMethod(2, new object[0]);
                    IDT.setIntArgs(DataSig);
                    CC[i].GetArrays();
                    CC[i].MakeNodeClusterGetArrays();
                    CC[i].SetBounds((int[])InvokeDictionaryMethod(12, new object[0]));

                    if (RecalculateFlag)
                        CalculateCombinations(temp, DataSig);
                    CC[i].GenerateNodes(UntrimmedCombinations);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                UILog(this, CC.ToString());
            }
        }
        public void SetupSingleCommandCenter(bool InitialLoggingState)
        {
            CC = new CommandCenter[1];
            CC[0] = new CommandCenter(IDT, "" + CommandCenterCount, InitialLoggingState, this);
            CommandCenterCount++;
            SubCC(CC[0]);
            int temp = (int)IDT.getItem(3, 0, 0);
            int[][] CommandSig = (int[][])InvokeDictionaryMethod(1, new object[0]);
            IDT.setIntArgs(CommandSig);
            CC[0].GetArrays();
            CC[0].CreateClustersFromTemplate1();
            int[][] DataSig = new int[1][];
            DataSig[0] = (int[])InvokeDictionaryMethod(2, new object[0]);
            IDT.setIntArgs(DataSig);
            CC[0].GetArrays();
            CC[0].MakeNodeClusterGetArrays();
            CC[0].SetBounds((int[])InvokeDictionaryMethod(12, new object[0]));

            if (RecalculateFlag)
                CalculateCombinations(temp, DataSig);

            CC[0].GenerateNodes(UntrimmedCombinations);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            UILog(this, CC.ToString());
        }
        public void SetGameMode(int temp)
        {
            if (SimMethods.Count > 0)
                InvokeDictionaryMethod(8, new object[] { temp });
        }
        public void SetPlayMode(int mode)
        {
            if (CC != null)
            {
                if (mode >= 0 && mode < 3)
                {
                    PlayMode = mode;
                    foreach (CommandCenter e in CC)
                        e.SetPlayMode(mode);
                }
                else
                {
                    PlayMode = 1;
                    foreach (CommandCenter e in CC)
                        e.SetPlayMode(1);
                }
            }
        }
        public void SetPlayerMode(int mode)
        {
            if (CC != null)
            {
                if (mode >= 0 && mode < 5)
                {
                    PlayerMode = mode;
                    foreach (CommandCenter e in CC)
                        e.SetPlayerMode(mode);
                }
                else
                {
                    PlayerMode = 0;
                    foreach (CommandCenter e in CC)
                        e.SetPlayerMode(0);
                }
            }
        }
        public void OldLegacyPassNodeGeneration(int combinationLevel)
        {
            foreach (CommandCenter e in CC)
            {
                e.GetArrays();
                e.MakeNodeClusterGetArrays();
                e.OldLegacyGenerateNodes(combinationLevel);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            UILog(this, CC.ToString());
        }
        public void LegacyPassNodeGeneration(int combinationLevel)
        {
            foreach (CommandCenter e in CC)
            {
                e.GetArrays();
                e.MakeNodeClusterGetArrays();
                e.LegacyGenerateNodes(combinationLevel);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            UILog(this, CC.ToString());
        }
        public void PassNodeGeneration(int combinationLevel)
        {
            foreach (CommandCenter e in CC)
            {
                e.GetArrays();
                e.MakeNodeClusterGetArrays();
                if (RecalculateFlag)
                    e.GenerateNodes(combinationLevel);
                else
                    e.GenerateNodes(UntrimmedCombinations);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            UILog(this, CC.ToString());
        }

        public object InvokeDictionaryMethod(int MethodNumber, object[] args)
        {
            if (MethodNumber > 0 && MethodNumber <= SimMethods.Count)
            {
                try
                {
                    object temp = SimMethods[MethodNumber].Invoke(Instance, args);
                    return temp;
                }
                catch (ArgumentException)
                {
                    return new object();
                }
            }
            else
                return new object();
        }
        public void SetCurrentCommandString()
        {
            //currentCommand = (string)InvokeDictionaryMethod(3)[0];
        }
        public void CallNodeCalculation()
        {
            foreach (CommandCenter e in CC)
                e.NodeCalculations();
        }
        public void PrintNodeCount()
        {
            foreach (CommandCenter e in CC)
                UILog(this, "" + e.GetFullNodeCount());
        }
        public object[][] PassResults()
        {
            object[][] temp = new object[CC.Length][];
            for (int i = 0; i < CC.Length; i++)
            {
                temp[i] = CC[i].GetAllDecisions();
            }
            return temp;
        }
        public void LoadCommandCenter(string path)
        {
            // Todo
        }
        public void SetCommandCenterChoiceMode(int mode)
        {
            if (CC != null)
            {
                foreach (CommandCenter e in CC)
                    e.SetDecisionMode(mode);
            }
        }
        public void SetCommandCenterChoiceMode(Random R)
        {
            if (CC != null)
            {
                foreach (CommandCenter e in CC)
                    e.SetDecisionMode(R.Next(0, 3));
            }
        }
        // Simulation Tester Methods
        public void InternalSimulatorSetup(bool LoggingState, InternalSimulator TestSimulator)
        {
            IDT.setStringArgs(TestSimulator.GetCommandNames());
            CC = new CommandCenter[TestSimulator.GetPlayerCount()];
            //int ThreadCount = Environment.ProcessorCount * 2;
            //int currentProcessor = 0;
            Thread[] GenerationThreads = new Thread[CC.Length + 1];
            int CombinationLevel = (int)IDT.getItem(3, 0, 0);
            for (int i = 0; i < CC.Length; i++)
            {
                CC[i] = new CommandCenter(IDT, "" + CommandCenterCount, LoggingState, this);
                CommandCenterCount++;
                SubCC(CC[i]);
            }
            BaseL = new CommandCenter(IDT, "" + BaseLineCCCount, "Baseline", LoggingState, this);
            BaseLineCCCount++;
            SubCC(BaseL);

            int[][] CommandSig = TestSimulator.GetCommandSignature();
            IDT.setIntArgs(CommandSig);
            RandomInput = new Randomizer((int)IDT.getItem(3, 0, 0));
            for (int i = 0; i < CC.Length; i++)
            {
                CC[i].GetArrays();
                CC[i].SetBounds(TestSimulator.ReturnRecommendedRange());
                CC[i].CreateClustersFromTemplate1();
            }
            BaseL.GetArrays();
            BaseL.SetBounds(TestSimulator.ReturnRecommendedRange());
            BaseL.CreateClustersFromTemplate1();
            int[][] DataSig = new int[1][];
            DataSig[0] = TestSimulator.GetDataSignature();
            IDT.setIntArgs(DataSig);
            string[] Names = IDT.getStringArgs()[0];
            for (int i = 0; i < CC.Length; i++)
            {
                CC[i].SetClusterNames(Names);
            }

            for (int i = -1; i < CC.Length; i++)
            {
                int temp = i;
                GenerationThreads[temp + 1] = new Thread(() => NodeCreation(temp, CombinationLevel, GenerationThreads[temp + 1]));
                GenerationThreads[temp + 1].Start();
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            UILog(this, CC.ToString());
        }
        public void NodeCreation(int Index, int CombinationLevel, Thread t)
        {
            if (Index == -1)
            {
                BaseL.GetArrays();
                BaseL.MakeNodeClusterGetArrays();
                if (RecalculateFlag)
                    BaseL.GenerateNodes(CombinationLevel);
                else
                    BaseL.GenerateNodes(UntrimmedCombinations);
                BaseL.SetPlayMode(PlayMode);
                BaseL.SetPlayerMode(PlayerMode);
                t.Abort();
            }
            else if (Index >= 0)
            {
                CC[Index].GetArrays();
                CC[Index].MakeNodeClusterGetArrays();
                if (RecalculateFlag)
                    CC[Index].GenerateNodes(CombinationLevel);
                else
                    CC[Index].GenerateNodes(UntrimmedCombinations);
                CC[Index].SetPlayMode(PlayMode);
                CC[Index].SetPlayerMode(PlayerMode);
                t.Abort();
            }
        }
        public void SingleCommandCenterSetup(bool InitialLoggingState)
        {
            CC = new CommandCenter[1];
            CC[0] = new CommandCenter(IDT, "" + CommandCenterCount, InitialLoggingState, this);
            CommandCenterCount++;
            SubCC(CC[0]);
            int temp = (int)IDT.getItem(3, 0, 0);
            int[][] CommandSig = TestSimulator.GetCommandSignature();
            IDT.setIntArgs(CommandSig);
            RandomInput = new Randomizer((int)IDT.getItem(3, 0, 0));
            CC[0].GetArrays();
            CC[0].SetBounds(TestSimulator.ReturnRecommendedRange());
            CC[0].CreateClustersFromTemplate1();
            int[][] DataSig = new int[1][];
            DataSig[0] = TestSimulator.GetDataSignature();
            IDT.setIntArgs(DataSig);
            CC[0].GetArrays();
            CC[0].MakeNodeClusterGetArrays();
            if (RecalculateFlag)
                CalculateCombinations(temp, DataSig);

            CC[0].GenerateNodes(UntrimmedCombinations);
            CC[0].SetPlayMode(PlayMode);
            CC[0].SetPlayerMode(PlayerMode);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            UILog(this, CC.ToString());
        }
        // Internal Start
        public void StartRound(InternalSimulator TestSim, bool ThisIsBeingBad)
        {
            TestSim.Start();
            //if (PlayMode == 0)
            // The reason this will never run is becaus it seems to make everything worse. It will most likely be added back in in a later version, but for now it is obsolete
            if (false)
            {
                if (PlayerMode == 1)
                {
                    for (int i = 0; i < CC.Length - 1; i++)
                    {
                        if (Rnd.Next(0, 101) == 0)
                        {
                            int Random = RandomInput.ReturnRandomIntInRange(0, RandomInput.ReturnDecisionCount());
                            CC[i].InitializeRound(Random);
                        }
                        else
                            CC[i].InitializeRound(0);
                    }
                }
                else
                {
                    foreach (CommandCenter e in CC)
                    {
                        if (Rnd.Next(0, 101) == 0)
                        {
                            int Random = RandomInput.ReturnRandomIntInRange(0, RandomInput.ReturnDecisionCount());
                            e.InitializeRound(Random);
                        }
                        else
                            e.InitializeRound(0);
                    }
                }
            }
            else
            {
                if (CC != null)
                {
                    if (PlayerMode == 1)
                    {
                        for (int i = 0; i < CC.Length - 1; i++)
                        {
                            CC[i].InitializeRound(i + 2);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < CC.Length; i++)
                        {
                            CC[i].InitializeRound(i + 1);
                        }
                    }
                }
            }
        }
        public void StartRound(InternalSimulator TestSim, int CCN, int PlayerID)
        {
            TestSim.Start();
            if (false)
            {
                int Random = RandomInput.ReturnRandomIntInRange(0, RandomInput.ReturnDecisionCount());
                CC[CCN].InitializeRound(Random);
            }
            else if (CCN >= 0)
            {
                CC[CCN].InitializeRound(PlayerID);
            }
            else if (CCN == -1)
            {
                BaseL.InitializeRound(PlayerID);
            }
        }
        public void StartRound(int CCN, int PlayerID)
        {
            if (CCN >= 0 && CC[CCN].Running)
                CC[CCN].EndRound(0, 0);
            if (CCN >= 0)
                CC[CCN].InitializeRound(PlayerID);
            else if (CCN == -1)
                BaseL.InitializeRound(PlayerID);
        }
        public void StartRound(InternalSimulator TestSim, int FirstPlayer)
        {
            TestSim.Start(FirstPlayer);
        }
        public void StartRound(InternalSimulator TestSim)
        {
            TestSim.Start();
        }
        public void EndCC(InternalSimulator TestSim)
        {
            // Each CommandCenter's player number is 1 greater than it's ID. This is only for AI vs AI.
            for (int i = 0; i < CC.Length; i++)
            {
                CC[i].EndRound(TestSim.GetResult(i + 1), TestSim.GetUnweightedResult(i + 1));
            }
        }
        public void StartCC(int Index, int PlayerID)
        {
            // The Player ID needs to be properly implemented, it shouldn't be 0
            if (Index >= 0 && Index < CC.Length)
                CC[Index].InitializeRound(PlayerID);
        }
        public void EndCC(int Index, int Score, int UnweightedScore)
        {
            if (Index >= 0 && Index < CC.Length)
                CC[Index].EndRound(Score, UnweightedScore);
        }
        public void AiVRandom(InternalSimulator TestSim, int RunCount)
        {
            if (CC != null)
            {
                for (int j = 0; j < CC.Length; j++)
                {
                    for (int i = 0; i < RunCount; i++)
                    {

                        if (PlayMode == 0)
                            CC[j].SetDecisionMode(RandomInput.ReturnRandomIntInRange(0, 3));

                        TestSim.Start();

                        int Turn1 = TestSim.Turn();
                        StartRound(j, Turn1);

                        while (!TestSim.Finished())
                        {
                            SingleCCTurn(TestSim, j, Turn1);
                            RandomizerTurn(TestSim);
                        }
                        if ((i + 1) % 50 == 0)
                            UILog(this, "" + (i + 1));
                        EndRound(TestSim, Turn1, j);
                    }
                    for (int i = 0; i < RunCount; i++)
                    {
                        if (PlayMode == 0)
                            CC[j].SetDecisionMode(RandomInput.ReturnRandomIntInRange(0, 3));
                        TestSim.Start();

                        int Turn1 = TestSim.Turn();
                        int Turn2 = TestSim.GetPlayerCount() - Turn1;

                        if (Turn2 == 0)
                            Turn2++;
                        StartRound(j, Turn2);

                        while (!TestSim.Finished())
                        {
                            RandomizerTurn(TestSim);
                            SingleCCTurn(TestSim, j, Turn2);
                        }
                        if ((i + 1) % 50 == 0)
                            UILog(this, "" + (i + 1));
                        EndRound(TestSim, Turn2, j);
                    }
                    UILog(this, "CC[" + j + "] finished.");
                }
            }
        }
        public void BaseLineTest(InternalSimulator TestSim)
        {
            BaseLineResults = new int[CC.Length][];
            for (int i = 0; i < BaseLineResults.Length; i++)
            { BaseLineResults[i] = new int[3]; }
            int TurnCount = TestSim.GetPlayerCount();
            int DecisionCount = RandomInput.ReturnDecisionCount();
            int Player1 = 1;
            int[] BaseLPlayers = new int[0];

            for (int j = 0; j < CC.Length; j++)
            {
                CC[j].SetDecisionMode(0);
                for (int i = 0; i < DecisionCount; i++)
                {
                    for (int o = 0; o < 3; o++)
                    {
                        int AITurnCount = 0;

                        StartRound(j, Player1); // Player 1 is CC
                        StartRound(-1, 2); // Player 2 is Baseline
                        StartRound(TestSim, 2);

                        BaseLPlayers = new int[TurnCount - 1];

                        for (int q = 1; q <= TurnCount - 1; q++)
                        {
                            // The first index is 0, but the first player is 2
                            BaseLPlayers[q - 1] = q + 1;
                        }
                        while (TestSim.Turn() == 2 && !TestSim.Finished())
                        {
                            TestSim.SetCommand(RandomInput.CreateDecisionFromInt(i));
                            CCLog(this, new CommandCArgs(new int[] { TestSim.ReturnChosenCommand(), 2 }));
                        }

                        BaseL.SetDecisionMode(o);

                        int count = 0;
 
                        while (!TestSim.Finished())
                        {
                            SingleCCTurn(TestSim, j, Player1);
                            AITurnCount++;

                            for (int l = 0; l < BaseLPlayers.Length; l++)
                            {
                                int temp = BaseLPlayers[l];
                                SingleCCTurn(TestSim, -1, temp);
                            }
                            if (count >= DecisionCount)
                                break;
                            count++;
                        }
                        int Result = TestSim.GetUnweightedResult(Player1);

                        if (Result == 1)
                            BaseLineResults[j][0]++;
                        else if (Result == 0)
                            BaseLineResults[j][1]++;
                        if (Result == -1)
                            BaseLineResults[j][2]++;
                        EndRound(TestSim, Player1, j);
                        EndRound(TestSim, BaseLPlayers[0], -1);
                    }
                }
                StartRound(j, Player1); // Player 1 is CC
                StartRound(-1, 2); // Player 2 is Baseline
                StartRound(TestSim, 1);

                int AITurnCount2 = 0;

                int count2 = 0;
                BaseL.SetDecisionMode(0);

                BaseLPlayers = new int[TurnCount - 1];

                for (int q = 1; q <= TurnCount - 1; q++)
                {
                    // The first index is 0, but the first player is 2
                    BaseLPlayers[q - 1] = q + 1;
                }
                int[] PlayedMoves1 = new int[9];

                while (!TestSim.Finished())
                {
                    SingleCCTurn(TestSim, j, Player1);
                    AITurnCount2++;
                    if (TestSim.Finished())
                        break;

                    for (int l = 0; l < BaseLPlayers.Length; l++)
                    {
                        int temp = BaseLPlayers[l];
                        SingleCCTurn(TestSim, -1, temp);
                    }
                    if (count2 >= DecisionCount)
                        break;
                    count2++;
                }

                int Result2 = TestSim.GetUnweightedResult(Player1);
                
                if (Result2 == 1)
                    BaseLineResults[j][0]++;
                else if (Result2 == 0)
                    BaseLineResults[j][1]++;
                if (Result2 == -1)
                    BaseLineResults[j][2]++;
                EndRound(TestSim, Player1, j);
                EndRound(TestSim, BaseLPlayers[0], -1);
            }
        }
        public string[] GetBaseLineResults()
        {
            if (CC != null)
            {
                if (BaseLineResults != null)
                {
                    string[] temp = new string[BaseLineResults.Length];
                    for (int i = 0; i < BaseLineResults.Length; i++)
                    {
                        CC[i].LogBaselineResults("Baseline Results: " + BaseLineResults[i][0] + ", " + BaseLineResults[i][1] + ", " + BaseLineResults[i][2]);
                        int count = BaseLineResults[i][0] + BaseLineResults[i][1] + BaseLineResults[i][2];
                        temp[i] = "CC[" + i + "] scored: " + BaseLineResults[i][0] + " wins | " + BaseLineResults[i][1] + " ties | " + BaseLineResults[i][2] + " losses. That is " + (((double)BaseLineResults[i][0] * 100) / count) + "% wins, " + (((double)BaseLineResults[i][1] * 100) / count) + "% ties, " + (((double)BaseLineResults[i][2] * 100) / count) + "% losses.";
                    }
                    return temp;
                }
                else
                    return new string[CC.Length];
            }
            else
                return new String[0];
        }
        public int[][] GetBaseLineArray(bool AutoLog)
        {
            if (CC != null)
            {
                if (BaseLineResults != null)
                {
                    if (AutoLog)
                    {
                        for (int i = 0; i < BaseLineResults.Length; i++)
                        {
                            CC[i].LogBaselineResults("Baseline Results: " + BaseLineResults[i][0] + ", " + BaseLineResults[i][1] + ", " + BaseLineResults[i][2]);
                        }
                    }
                    return BaseLineResults;
                }
                else
                    return new int[CC.Length][];
            }
            else
                return new int[0][];
        }
        public void EndRound(InternalSimulator TestSim)
        {
            if (PlayerMode == 1)
            {
                for (int i = 0; i < CC.Length - 1; i++)
                { CC[i].EndRound(TestSim.GetResult(i + 2), TestSim.GetUnweightedResult(i + 2)); }
            }
            else if (PlayerMode == 2)
            {
                for (int i = 0; i < CC.Length; i++)
                { CC[i].EndRound(TestSim.GetResult(i + 1), TestSim.GetUnweightedResult(i + 1)); }
            }
        }
        public void EndRound(InternalSimulator TestSim, int Turn, int CCN)
        {
            if (PlayerMode == 4 || PlayerMode == 3)
            {
                if (CCN == -1)
                    BaseL.EndRound(TestSim.GetResult(Turn), TestSim.GetUnweightedResult(Turn));
                else
                    CC[CCN].EndRound(TestSim.GetResult(Turn), TestSim.GetUnweightedResult(Turn));
            }
        }
        public void SetInternalGameMode(int mode)
        {
            TestSimulator.SetGameMode(mode);
        }
        public void RandomizerTurn(InternalSimulator TestSim)
        {
            if (!TestSim.Finished())
            {
                object[] temp = RandomInput.GetRandomDecision();
                int temp2 = ((int)((object[])temp[0])[0]);
                TestSim.SetCommand(temp);
                int i = RandomInput.ReturnDecisionCount();
                int j = 0;
                while (TestSim.ReturnChosenCommand() != temp2)
                {
                    temp = RandomInput.GetRandomDecision();
                    temp2 = ((int)((object[])temp[0])[0]);
                    TestSim.SetCommand(temp);
                    j++;
                    if (i == j)
                        break;
                }
            }
        }
        public void SingleCCTurn(InternalSimulator TestSim, int cc, int Player)
        {
            if (cc == -1)
            {
                while (TestSim.Turn() == Player && !TestSim.Finished())
                {
                    IDT.setObjectArgs(TestSim.GetData(Player));
                    IDT.SetState(true);
                    BaseL.GetArrays();
                    BaseL.ForceNodeClustersToGetData();
                    BaseL.NodeCalculations();
                    TestSim.SetCommand(BaseL.GetAllDecisions());
                    IDT.SetState(false);
                    currentCommandNumber = TestSim.ReturnChosenCommand();
                    CCLog(this, new CommandCArgs(new int[] { currentCommandNumber, Player }));
                }
            }
            else
            {
                while (TestSim.Turn() == Player && !TestSim.Finished())
                {

                }
                IDT.setObjectArgs(TestSim.GetData(Player));
                IDT.SetState(true);
                CC[cc].GetArrays();
                CC[cc].ForceNodeClustersToGetData();
                CC[cc].NodeCalculations();
                TestSim.SetCommand(CC[cc].GetAllDecisions());
                IDT.SetState(false);
                currentCommandNumber = TestSim.ReturnChosenCommand();
                CCLog(this, new CommandCArgs(new int[] { currentCommandNumber, Player }));
                if (PlayMode == 0)
                    CC[cc].LogCluster(currentCommandNumber);
            }
        }
        public int InternalAITurn(InternalSimulator TestSim)
        {
            if (CC != null)
            {
                if (!TestSim.Finished())
                {
                    if (PlayerMode == 1)
                    {
                        // The human is always player 1, the AI take up the remaining players
                        int Move = 0;
                        for (int i = 0; i < CC.Length; i++)
                        {
                            if (TestSim.Turn() == i + 2)
                                Move = i;
                        }
                        while (TestSim.Turn() == Move + 2 && !TestSim.Finished())
                        {
                            IDT.setObjectArgs(TestSim.GetData(Move + 2));
                            IDT.SetState(true);
                            CC[Move].GetArrays();
                            CC[Move].ForceNodeClustersToGetData();
                            CC[Move].NodeCalculations();
                            //CC[Move].LegacyNodeCalculations();
                            TestSim.SetCommand(CC[Move].GetAllDecisions());
                            IDT.SetState(false);
                            currentCommandNumber = TestSim.ReturnChosenCommand();
                            CCLog(this, new CommandCArgs(new int[] { currentCommandNumber, Move + 2 }));
                            if (PlayMode == 0)
                                CC[Move].LogCluster(currentCommandNumber);
                        }
                    }
                    else if (PlayerMode == 2)
                    {
                        int Move = 0;
                        for (int i = 0; i < CC.Length; i++)
                        {
                            if (TestSim.Turn() == i + 1)
                                Move = i;
                        }
                        while (TestSim.Turn() == Move + 1 && !TestSim.Finished())
                        {
                            IDT.setObjectArgs(TestSim.GetData(Move + 1));
                            IDT.SetState(true);
                            CC[Move].GetArrays();
                            CC[Move].ForceNodeClustersToGetData();
                            CC[Move].NodeCalculations();
                            TestSim.SetCommand(CC[Move].GetAllDecisions());
                            IDT.SetState(false);
                            currentCommandNumber = TestSim.ReturnChosenCommand();
                            CCLog(this, new CommandCArgs(new int[] { currentCommandNumber, Move + 1 }));
                            if (PlayMode == 0)
                                CC[Move].LogCluster(currentCommandNumber);
                        }
                    }
                    else if (PlayerMode == 3)
                    {
                        int Move = 0;
                        for (int i = 0; i < CC.Length; i++)
                        {
                            if (TestSim.Turn() == i + 2)
                                Move = i;
                        }
                        while (TestSim.Turn() == Move + 2 && !TestSim.Finished())
                        {
                            IDT.setObjectArgs(TestSim.GetData(Move + 2));
                            IDT.SetState(true);
                            CC[Move].GetArrays();
                            CC[Move].ForceNodeClustersToGetData();
                            CC[Move].NodeCalculations();
                            TestSim.SetCommand(CC[Move].GetAllDecisions());
                            IDT.SetState(false);
                            currentCommandNumber = TestSim.ReturnChosenCommand();
                            CCLog(this, new CommandCArgs(new int[] { currentCommandNumber, Move }));
                            if (PlayMode == 0)
                                CC[Move].LogCluster(currentCommandNumber);
                        }
                    }
                    return currentCommandNumber;
                }
                else
                    return -1;
            }
            else
                return -1;
        }
        public CommandCenter ReturnBaseL()
        {
            return BaseL;
        }
        public long[] GetDecisionTime(int NCC)
        {
            return CC[NCC].GetDecisionTime();
        }

        public bool ToggleLogging()
        {
            if (CC != null)
            {
                foreach (CommandCenter c in CC)
                {
                    c.ToggleLogging();
                }
                BaseL.ToggleLogging();
                return true;
            }
            else
                return true;
        }

        public string[] GetMoveSet()
        {
            if (SimulatorMode == 1)
                return TestSimulator.GetCommandNames();
            else
                return (string[])InvokeDictionaryMethod(2, new object[] { });
        }

        private void LogHumanMove(CommandCArgs e)
        {
            if (PlayerMode != 0)
                CCLog(this, e);
        }

        public void UnifiedStart()
        {

        }
        public void UnifiedEnd()
        {

        }
    }
}