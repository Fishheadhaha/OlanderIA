using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace OlanderIA
{
    public class CommandCenter
    {
        // Logger
        private Logger Log;
        // Logging and events
        public event LogUI UILog;
        public event LogCommand GameStart;
        public event LogCommand GameMode;
        public event LogCommand Result;
        public event LogCommand RoundData;
        public event LogCommand RoundNodeDecisions;
        public event LogCommand RoundClusterDecisions;
        public event LogCommand RoundChosenCommand;
        public event LogCommand NodeWeights;
        public event LogCommand NodeDataSources;
        public event LogCommand NodeOperators;
        public event LogCommand EndGame;
        public event LogCommand ToggleLog;
        public event LogCommand StringRepresentation;
        public event LogCommand BaseLineResults;
        public event LogCommand Time;
        public event LogCommand ChosenMove;
        public event LogCommand PlayerID;
        public EventArgs e = null;
        public event SubscribeItem SubNodeCluster;
        public delegate void LogCommand(CommandCenter m, CommandCArgs e);
        public delegate void LogUI(CommandCenter m, string e);
        public delegate void SubscribeItem(NodeCluster e);
        //
        InternalDataTransfer IDT;
        NodeCluster[] NC = new NodeCluster[0];
        int LowEnd = -5;
        int UpEnd = 5;
        //
        public string ID = "Command Center";
        public string Name = "";
        //
        int RunCount = 0;
        int LogLength = 100000; // Really high because something needs to be done about this

        // Mode 0 = Learning
        // Mode 1 = Playing
        // Mode 2 = Userinput (The user inputs all the data, and retrieves the result)
        int PlayMode = 1;
        LearningCenter LC;

        int PlayerMode = 0;

        int[][] intArgs = new int[0][];
        double[][] doubleArgs = new double[0][];
        bool[][] boolArgs = new bool[0][];
        object[][] objectArgs = new object[0][];

        Dictionary<int, string> CommandNames = new Dictionary<int, string>();

        public bool Running = false;

        int UntrimmedCount = 0;
        int TrimmedCount = 0;
        // 0 = Highest, 1 = Middlemost, 2 = Lowest
        int DecisionMode = 0;
        int DecisionIndex = -1;
        bool SpecificIndex = false;
        int DecisionCount = 0;

        public CommandCenter(InternalDataTransfer IDT, string ID, bool InitialLoggingState, CommunicationCenter CTemp)
        {
            CTemp.CCLog += new CommunicationCenter.LogCommandNumber(LogCommandNumber);

            this.ID += " " + ID;
            Name = this.ID;
            this.IDT = IDT;
            LC = new LearningCenter(IDT);
            Log = new Logger(this, this.ID + " - Log", Name, InitialLoggingState, CTemp.GetMoveSet());
        }
        public CommandCenter(InternalDataTransfer IDT, string ID, string Name, bool InitialLoggingState, CommunicationCenter CTemp)
        {
            CTemp.CCLog += new CommunicationCenter.LogCommandNumber(LogCommandNumber);

            this.ID += " - " + Name + " " + ID;
            this.Name = Name + " " + ID;
            this.IDT = IDT;
            LC = new LearningCenter(IDT);
            Log = new Logger(this, this.ID + " - Log", this.Name, InitialLoggingState, CTemp.GetMoveSet());
        }
        // Common Methods
        public void GetArrays()
        {
            intArgs = IDT.getIntArgs();
            doubleArgs = IDT.getDoubleArgs();
            boolArgs = IDT.getBoolArgs();
            objectArgs = IDT.getObjectArgs();
        }
        // A few setup methods
        public void CreateClustersFromTemplate1()
        {
            // The first position in the integer array corresponds to the number of commands.
            // In the int staggered array, in the second+ arrays, the 0th index is the number of doubles needed, 1st is the number of booleans
            // 2nd is the number of ints, 3rd is the number of preset options
            if (IDT != null)
            {
                NC = new NodeCluster[intArgs[0][0]];
                for (int i = 1; i <= intArgs[0][0]; i++)
                {
                    int count = intArgs[i][0] + intArgs[i][1] + intArgs[i][2];
                    if (count != 0)
                    {
                        int pos = 0;
                        int[] types = new int[count];
                        for (int j = 0; j < 3; j++)
                        {
                            for (int k = 0; k < intArgs[i][j]; k++)
                            {
                                types[pos] = j + 1;
                                pos++;
                            }
                        }
                        NC[i - 1] = new NodeCluster(false, count, 0, types, IDT, LowEnd, UpEnd, "" + (i - 1));
                    }
                    else
                    {
                        NC[i - 1] = new NodeCluster(false, 0, IDT, LowEnd, UpEnd, "" + (i - 1));
                    }
                    SubNodeCluster(NC[i - 1]);
                    NC[i - 1].CreatePresetCluster(intArgs[i][3]);
                }
            }
            DecisionCount = NC.Length;
        }
        // This does combination generation for each of the sub nodes
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
        private int[][] RemoveInvalids(int type, int[][] array)
        {
            int[] DataDivisions = new int[3];
            DataDivisions[0] = intArgs[0][0]; // double
            DataDivisions[1] = intArgs[0][1]; // boolean
            DataDivisions[2] = intArgs[0][2]; // int

            if (type == 1)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array[0].Length; j++)
                    {
                        if (array[i][j] >= DataDivisions[0] && array[i][j] < DataDivisions[0] + DataDivisions[1])
                        {
                            TrimmedCount--;
                            for (int l = 0; l < array[0].Length; l++)
                            { array[i][l] = -1; }
                        }
                    }
                }
            }
            return array;
        }
        public int[][][] ThreeDimCopy(int[][][] inputArray)
        {
            int[][][] tempArray = new int[inputArray.Length][][];
            for (int i = 0; i < inputArray.Length; i++)
            {
                tempArray[i] = new int[inputArray[i].Length][];
                for (int l = 0; l < inputArray[i].Length; l++)
                {
                    tempArray[i][l] = new int[inputArray[i][l].Length];
                    for (int j = 0; j < inputArray[i][l].Length; j++)
                    {
                        tempArray[i][l][j] = inputArray[i][l][j];
                    }
                }
            }
            return tempArray;
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
        // End of the combination Generation
        public void GenerateNodes(int NodeCombinationLimit)
        {
            int[][] startArray = new int[intArgs[0][0] + intArgs[0][1] + intArgs[0][2]][];
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
            int[][][] Trimmed = ThreeDimCopy(finalArray);
            UntrimmedCount = finalArray.Length;
            TrimmedCount = UntrimmedCount;
            for (int i = 0; i < Trimmed.Length; i++)
            {
                Trimmed[i] = RemoveInvalids(1, Trimmed[i]);
            }
            foreach (NodeCluster n in NC)
            { n.GenerateNodes(finalArray, Trimmed); }
            StringRepresentation(this, null);
            LogNodes(false);
        }
        public void GenerateNodes(int[][][] finalArray)
        {
            int[][][] Untrimmed = ThreeDimCopy(finalArray);
            int[][][] Trimmed = ThreeDimCopy(finalArray);
            UntrimmedCount = finalArray.Length;
            TrimmedCount = UntrimmedCount;
            for (int i = 0; i < Trimmed.Length; i++)
            {
                Trimmed[i] = RemoveInvalids(1, Trimmed[i]);
            }
            foreach (NodeCluster n in NC)
            { n.GenerateNodes(Untrimmed, Trimmed); }
            StringRepresentation(this, null);
            LogNodes(false);
        }
        public void LegacyGenerateNodes(int NodeCombinationLimit)
        {
            int[][] startArray = new int[intArgs[0][0] + intArgs[0][1] + intArgs[0][2]][];
            for (int i = 0; i < startArray.Length; i++)
            {
                startArray[i] = new int[1];
                startArray[i][0] = i + 1;
            }
            int[][][] finalArray = new int[NodeCombinationLimit][][];
            finalArray[0] = startArray;
            int pos = 1;
            for (int i = 1; i < NodeCombinationLimit; i++)
            {
                startArray = GenerateCombinations(1, startArray);
                finalArray[pos] = startArray;
                pos++;
            }
            foreach (NodeCluster n in NC)
            { n.LegacyGenerateNodes(finalArray); }
        }
        public void OldLegacyGenerateNodes(int NodeCombinationLimit)
        {
            // Node Combination limit should generally be small, but can be up to the size of the number of data inputs
            // There must be an even number of numbered inputs, if there are an odd count we can include a constant value within the node. This would complicate the learning process, and require a different mode.
            foreach (NodeCluster n in NC)
            { n.OldLegacyGenerateNodes(NodeCombinationLimit); }
        }
        public void MakeNodeClusterGetArrays()
        {
            foreach (NodeCluster n in NC)
            {
                n.GetAllData();
                n.MakeSubClustersGetData();
            }
        }
        public void LegacyNodeCalculations()
        {
            foreach (NodeCluster n in NC)
            {
                IndividualCalculation(n);
            }
        }
        public void NodeCalculations()
        {
            CountdownEvent CE = new CountdownEvent(NC.Length);
            foreach (NodeCluster e in NC)
            {
                object temp;
                temp = new object[2];
                ((object[])temp)[0] = e;
                ((object[])temp)[1] = CE;
                ThreadPool.QueueUserWorkItem(IndividualCalculation, temp);
            }
            CE.Wait();
        }
        public void NodeCalculations(bool jkl)
        {
            ForceNodeClustersToGetData();
            int ThreadCount = (2);
            int currentThread = 0;
            Thread[] Threads = new Thread[ThreadCount];
            for (int i = 0; i < NC.Length; i++)
            {
                int temp = i;
                if (Threads[currentThread] == null)
                {
                    Threads[currentThread] = new Thread(() => IndividualCalculation(NC[temp]));
                    Threads[currentThread].Start();
                }
                else if (!Threads[currentThread].IsAlive)
                {
                    Threads[currentThread] = new Thread(() => IndividualCalculation(NC[temp]));
                    Threads[currentThread].Start();
                }
                    currentThread++;
                    currentThread = currentThread % ThreadCount;
                if (Threads[currentThread] != null)
                {
                    while (Threads[currentThread].IsAlive)
                    { }
                }
            }
            currentThread--;
            if (currentThread == -1)
                currentThread = ThreadCount - 1;
            while (Threads[currentThread].IsAlive)
            { }
        }
        public void IndividualCalculation(NodeCluster n)
        {
            n.NodeCalculations();
        }
        public void IndividualCalculation(object n)
        {
            ((NodeCluster)((object[])n)[0]).NodeCalculations();
            ((CountdownEvent)((object[])n)[1]).Signal();
        }
        public int GetFullNodeCount()
        {
            int temp = 0;
            foreach (NodeCluster e in NC)
            { temp += e.GetFullNodeCount(); }
            return temp;
        }
        //
        public override string ToString()
        {
            string s = "Node Count: " + GetFullNodeCount() + Environment.NewLine;
            for (int i = 0; i < NC.Length; i++)
            {
                s += NC[i].ToString();
                s += Environment.NewLine;
            }
            return s;
        }
        public object[] GetAllDecisions()
        {
            object[] temp = new object[NC.Length];
            for (int i = 0; i < NC.Length; i++)
            {
                temp[i] = NC[i].AssembleDecision();
            }
            string Decisions = "";
            for (int i = 0; i < NC.Length; i++)
            {
                Decisions += NC[i].ID + ": " + (int)((object[])temp[i])[0];
                if (i != NC.Length - 1)
                    Decisions += ",  ";
            }
            RoundClusterDecisions(this, new CommandCArgs(Decisions));
            return TailorIndex(TailorDecision(temp));
        }
        public NodeCluster[] ReturnClusters()
        {
            return NC;
        }
        //
        public void ForceNodeClustersToGetData()
        {
            foreach (NodeCluster e in NC)
            {
                e.GetAllData();
                e.MakeSubClustersGetData();
            }
        }
        public void SetBounds(int[] bounds)
        {
            LowEnd = bounds[0];
            UpEnd  = bounds[1];
        }
        //
        public void SetDecisionMode(int mode)
        {
            DecisionMode = mode;
        }
        public void SetDecisionIndex(int Index)
        {
            DecisionIndex = Index;
            SpecificIndex = true;
        }
        public object[] TailorIndex(object[] Decision)
        {
            if (SpecificIndex)
            {
                if (DecisionIndex >= 0 && DecisionIndex < DecisionCount)
                {
                    int LeftBound = DecisionIndex - 1;
                    int RightBound = DecisionIndex + 1;
                    bool LeftFinished = false;
                    bool RightFinished = false;
                    ((object[])Decision[DecisionIndex])[0] = 1;
                    for (int i = 0; i < Decision.Length - 1; i++)
                    {
                        if (LeftBound == -1)
                            LeftFinished = true;
                        if (RightBound == DecisionCount)
                            RightFinished = true;
                        if (!LeftFinished)
                            ((object[])Decision[LeftBound])[0] = (i * -1);
                        if (!RightFinished)
                            ((object[])Decision[RightBound])[0] = (i * -1);
                        LeftBound--;
                        RightBound++;
                    }
                }
            }
            return Decision;
        }
        public object[] TailorDecision(object[] Decision)
        {
            if (DecisionMode == 0)
            {
                int[] Ranking = new int[Decision.Length];
                for (int i = -1; i >= (Decision.Length * -1); i--)
                {
                    int Highest = 0;
                    while (Ranking[Highest] != 0)
                    {
                        Highest++;
                    }
                    for (int l = Highest; l < Decision.Length; l++)
                    {
                        if (Ranking[l] == 0)
                        {
                            if ((int)((object[])Decision[Highest])[0] < (int)((object[])Decision[l])[0])
                                Highest = l;
                        }
                    }
                    Ranking[Highest] = i;
                }
                for (int i = 0; i < Decision.Length; i++)
                {
                    ((object[])Decision[i])[0] = Ranking[i];
                }
                return Decision;
            }
            else if (DecisionMode == 1)
            {
                int[] Ranking = new int[Decision.Length];
                for (int i = -1; i >= (Decision.Length * -1); i--)
                {
                    int Highest = 0;
                    while (Ranking[Highest] != 0)
                    {
                        Highest++;
                    }
                    for (int l = Highest; l < Decision.Length; l++)
                    {
                        if (Ranking[l] == 0)
                        {
                            if ((int)((object[])Decision[Highest])[0] < (int)((object[])Decision[l])[0])
                                Highest = l;
                        }
                    }
                    Ranking[Highest] = i;
                }
                int[] IndexArray = new int[Ranking.Length];
                for (int i = 0; i < Ranking.Length; i++)
                { IndexArray[(Ranking[i] * -1) - 1] = i; }

                int Position = 0;
                if (IndexArray.Length % 2 == 0)
                {
                    int StartingPoint = (IndexArray.Length / 2) - 1;
                    Position = StartingPoint;
                    Ranking[StartingPoint] = -1;
                }
                else
                {
                    int StartingPoint = (int)Math.Floor((double)(IndexArray.Length / 2));
                    Position = StartingPoint;
                    Ranking[StartingPoint] = -1;
                }

                int Round = 0;
                for (int i = -2; i >= (Decision.Length * -1); i--)
                {
                    if (Round % 2 == 0)
                    {
                        Position = Position - (i + 1);
                        Ranking[IndexArray[Position]] = i;
                    }
                    else
                    {
                        Position = Position + (i + 1);
                        Ranking[IndexArray[Position]] = i;
                    }
                    Round++;
                }
                for (int i = 0; i < Decision.Length; i++)
                {
                    ((object[])Decision[i])[0] = Ranking[i];
                }
                return Decision;
            }
            else if (DecisionMode == 2)
            {
                int[] Ranking = new int[Decision.Length];
                for (int i = -1; i >= (Decision.Length * -1); i--)
                {
                    int Highest = 0;
                    while (Ranking[Highest] != 0)
                    {
                        Highest++;
                    }
                    for (int l = Highest; l < Decision.Length; l++)
                    {
                        if (Ranking[l] == 0)
                        {
                            if ((int)((object[])Decision[Highest])[0] > (int)((object[])Decision[l])[0])
                                Highest = l;
                        }
                    }
                    Ranking[Highest] = i;
                }
                for (int i = 0; i < Decision.Length; i++)
                {
                    ((object[])Decision[i])[0] = Ranking[i];
                }
                return Decision;
            }
            else
                return Decision;
        }
        public void SetPlayMode(int mode)
        {
            if (!Running)
                PlayMode = mode;
        }
        public int GetPlayMode()
        {
            return PlayMode;
        }
        public void SetPlayerMode(int mode)
        {
            if(!Running)
                PlayerMode = mode;
        }
        public void InitializeRound(int Player)
        {
            if (PlayMode == 0)
            {
                LC.StartRound();
            }
            foreach (NodeCluster e in NC)
            { e.StartRound(); }
            Running = true;
            SpecificIndex = false;
            GameStart(this, null);
            GameMode(this, null);
            PlayerID(this, new CommandCArgs(new int[] { Player }));
        }
        public void InitializeRound(int Index, int Player)
        {
            if (PlayMode == 0)
            {
                LC.StartRound();
            }
            foreach (NodeCluster e in NC)
            { e.StartRound(); }
            Running = true;
            DecisionIndex = Index;
            SpecificIndex = true;
            GameStart(this, null);
            GameMode(this, null);
            PlayerID(this, new CommandCArgs(new int[] { Player }));
        }
        public void EndRound(int Score, int UnweightedScore)
        {
            Running = false;
            if (PlayMode == 0)
                LC.FinalizeRound(Score);
            CommandCArgs score = new CommandCArgs(new int[] { UnweightedScore });
            
            Result(this, score);
            EndGame(this, null);

            if (PlayMode == 0)
            {
                RunCount++;
                if (RunCount == LogLength)
                {
                    RunCount = 0;
                    LogNodes(false);
                }
            }
        }
        public void LogCluster(int Cluster)
        {
            if (Cluster != -1)
            {
                LC.LogNodeCluster(NC[Cluster], objectArgs[0]);
            }
        }
        public int ReturnUntrimmedCount()
        {
            return UntrimmedCount;
        }
        public int ReturnTrimmedCount()
        {
            return TrimmedCount;
        }
        public long[] GetDecisionTime()
        {
            long[] Temp = new long[NC.Length];
            for (int i = 0; i < Temp.Length; i++)
            {
                Temp[i] = NC[i].GetDecisionTime();
            }
            return Temp;
        }

        private void LogNodes(bool LogWeights)
        {
            if (LogWeights)
            {
                string[][] TempWeights = new string[NC.Length][];
                for (int i = 0; i < NC.Length; i++)
                {
                    TempWeights[i] = NC[i].GetWeights();
                }

                NodeWeights(this, new CommandCArgs(TempWeights));
            }

            NodeDataSources(this, null);
            NodeOperators(this, null);
        }

        public void ToggleLogging()
        {
            ToggleLog(this, null);
        }
        public void LogBaselineResults(string results)
        {
            BaseLineResults(this, new CommandCArgs(results));
        }
        public void LogCommandNumber(CommunicationCenter c, CommandCArgs e)
        {
            ChosenMove(this, e);
        }
        public void SetClusterNames(string[] CommandNames)
        {
            try
            {
                for (int i = 0; i < NC.Length; i++)
                {
                    NC[i].SetID(CommandNames[i]);
                }
            }
            catch
            { }
        }
        // Stuff that needs to be done
        public void Save()
        {
            // Todo
        }
        public void Load(InternalDataTransfer IDT, Operator Oper)
        { 
            // Todo
        }
    }
    public class VoidArgs : EventArgs
    { }
}