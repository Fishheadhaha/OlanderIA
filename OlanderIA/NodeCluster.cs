using System;
using System.Collections;
using System.Diagnostics;


namespace OlanderIA
{
    public class NodeCluster
    {
        //Logging
        public event LogCommand UILog;
        public delegate void LogCommand(NodeCluster m, string e);
        //
        // This should be the same number stored in the Node Class
        int NodeTypeCount = 4;
        int lowerBound = -3;
        int upperBound = 3;
        private static Random rTemp = new Random();
        // The list of nodes belonging to this groups
        // There is one array for each type of node
        ArrayList NodeGroup = new ArrayList();
        // Whether or not it is a sub cluster, and if it isn't, how many sub clusters it has, if any
        bool subCluster;
        NodeCluster[] SubClusters = new NodeCluster[0];
        // 0 = Choice node, 1 = double, 2 = boolean, 3 = int, 4 = preset (Works almost the same as a choice node)
        int returnType;
        // Operator
        public Operator Oper;
        // InternalTransfer
        public InternalDataTransfer IDT;
        // If it isn't a subcluster, and has any preset node clusters
        NodeCluster[] PresetClusters = new NodeCluster[0];
        // Data divisions. This marks the length of each of the three data types in the object array, in the following order: double, boolean, int
        int[] DataDivisions = new int[3];
        //
        bool OpeningCalculation = false;
        //
        BitArray BA;
        //
        bool Learning = false;
        //
        long DeltaTime = 0;

        public string ID = "";

        int MalleabilityConstant = 10;
        int CurrentMalleability = -1;

        int PreviousDecisionWeight = 0;

        // Arrays that contain data
        int[][] intArgs;
        double[][] doubleArgs;
        bool[][] boolArgs;
        object[][] objectArgs1;
        object[][] objectArgs2;
        bool Loc = true;
        // Getting data
        public void GetAllData()
        {
            intArgs = IDT.getIntArgs();
            doubleArgs = IDT.getDoubleArgs();
            boolArgs = IDT.getBoolArgs();
            if (Learning)
            {
                if (Loc)
                {
                    objectArgs1 = IDT.getObjectArgs();
                    Loc = !Loc;
                }
                else
                {
                    objectArgs2 = IDT.getObjectArgs();
                    Loc = !Loc;
                }

            }
            else
            {
                if (OpeningCalculation)
                {
                    if (Loc)
                    {
                        objectArgs1 = IDT.getObjectArgs();
                        if (objectArgs1.Length != 0)
                        {
                            BA = new BitArray((objectArgs1[0]).Length);
                            Loc = !Loc;
                        }
                        else
                            BA = new BitArray(0);
                    }
                    else
                    {
                        objectArgs2 = IDT.getObjectArgs();
                        if (objectArgs2.Length != 0)
                        {
                            BA = new BitArray((objectArgs2[0]).Length);
                            Loc = !Loc;
                        }
                        else
                            BA = new BitArray(0);
                    }
                }
                else
                {
                    if (Loc)
                    {
                        objectArgs1 = IDT.getObjectArgs();
                        if (objectArgs1.Length != 0)
                        {
                            BA = new BitArray((objectArgs1[0]).Length);
                            Loc = !Loc;
                        }
                        else
                            BA = new BitArray(0);
                    }
                    else
                    {
                        objectArgs2 = IDT.getObjectArgs();
                        if (objectArgs2.Length != 0)
                        {
                            BA = new BitArray((objectArgs2[0]).Length);
                            Loc = !Loc;
                        }
                        else
                            BA = new BitArray(0);
                    }
                    for (int i = 0; i < BA.Length; i++)
                    {
                        if (i < DataDivisions[0])
                            BA.Set(i, (double)objectArgs1[0][i] == (double)objectArgs2[0][i]);
                        else if (i < DataDivisions[1] + DataDivisions[0])
                            BA.Set(i, (bool)objectArgs1[0][i] == (bool)objectArgs2[0][i]);
                        else if (i < DataDivisions[2] + DataDivisions[1] + DataDivisions[0])
                            BA.Set(i, (int)objectArgs1[0][i] == (int)objectArgs2[0][i]);
                    }
                }
            }
        }
        public object getItem(int array, int index1, int index2)
        {
            if (array == 1)
                return doubleArgs[index1][index2];
            else if (array == 2)
                return boolArgs[index1][index2];
            else if (array == 3)
                return intArgs[index1][index2];
            else
            {
                if (!Loc)
                    return objectArgs1[index1][index2];
                else
                    return objectArgs2[index1][index2];
            }
        }
        public void MakeSubClustersGetData()
        {
            foreach (NodeCluster nc in SubClusters)
            { nc.GetAllData(); }
            foreach (NodeCluster nc in PresetClusters)
            { nc.GetAllData(); }
        }

        // Creates the clusters, and any sub clusters they have, and sets the right return type to each one
        public NodeCluster(bool IsSubCluster, int returnType, InternalDataTransfer IDT, int lowerEnd, int upperEnd, string ID)
        {
            subCluster = IsSubCluster;
            Oper = new Operator();
            this.IDT = IDT;
            this.returnType = returnType;
            lowerBound = lowerEnd;
            upperBound = upperEnd;
            this.ID = ID;
        }
        public NodeCluster(bool IsSubCluster, int subClusterCount, int returnType, int[] SubClusterReturnTypes, InternalDataTransfer IDT, int lowerEnd, int upperEnd, string ID)
        {
            subCluster = IsSubCluster;
            Oper = new Operator();
            this.IDT = IDT;
            lowerBound = lowerEnd;
            upperBound = upperEnd;
            this.ID = ID;
            if (!subCluster)
            {
                SubClusters = new NodeCluster[subClusterCount];
                for (int i = 0; i < subClusterCount; i++)
                {
                    SubClusters[i] = new NodeCluster(true, SubClusterReturnTypes[i], IDT, lowerEnd, upperEnd, "" + i);
                }
            }
            this.returnType = returnType;
        }
        public void CreatePresetCluster(int count)
        {
            if (count > 0)
            {
                PresetClusters = new NodeCluster[count];
                for (int i = 0; i < count; i++)
                {
                    PresetClusters[i] = new NodeCluster(true, 4, IDT, upperBound, lowerBound, "" + i);
                }
            }
        }
        public void SetID(string ID)
        {
            this.ID = ID;
        }
        //
        public void StartRound()
        {
            OpeningCalculation = true;
            Learning = false; // Should this really be getting set to false? I think so
        }
        public string ToString()
        {
            string s = "";
            int nodeCount = 0;
                foreach (Node e in NodeGroup)
                {
                    nodeCount++;
                }
            int clusterCount = 0;
            if (!subCluster)
            {
                clusterCount = SubClusters.Length + PresetClusters.Length;
                string type = "";
                if (returnType == 0)
                    type = "default";
                else if (returnType == 1)
                    type = "double";
                else if (returnType == 2)
                    type = "boolean";
                else if (returnType == 3)
                    type = "int";
                else if (returnType == 4)
                    type = "preset";
                s = "Node Cluster, type: " + type + ", with " + clusterCount + " sub clusteres. There are " + nodeCount + " nodes.";
                for (int i = 0; i < SubClusters.Length; i++)
                {
                    s += Environment.NewLine;
                    s += SubClusters[i].ToString();
                }
                for (int i = 0; i < PresetClusters.Length; i++)
                {
                    s += Environment.NewLine;
                    s += PresetClusters[i].ToString();
                }
            }
            else
            {
                string type = "";
                if (returnType == 0)
                    type = "default";
                else if (returnType == 1)
                    type = "double";
                else if (returnType == 2)
                    type = "boolean";
                else if (returnType == 3)
                    type = "int";
                else if (returnType == 4)
                    type = "preset";
                s = "Node Sub Cluster, type: " + type + ", with " + nodeCount + " nodes.";
            }
            return s;
        }
        // Setup methods
        public void GenerateNodes(int[][][] Untrimmed, int[][][] Trimmed)
        {
            DataDivisions[0] = intArgs[0][0]; // double
            DataDivisions[1] = intArgs[0][1]; // boolean
            DataDivisions[2] = intArgs[0][2]; // int

            int count = 0;
            for (int i = 0; i < Untrimmed.Length; i++)
            { count += Untrimmed[i].Length; }
            NodeGroup.Capacity = count;
            //Random rTemp = new Random();
            Random rTemp = new Random(Guid.NewGuid().GetHashCode());
            if (Math.Abs(returnType - 2) == 1)
            {
                for (int i = 0; i < Trimmed.Length; i++)
                {
                    CreateNodes(2, Trimmed[i], rTemp);
                }
            }
            else
            {
                for (int i = 0; i < Untrimmed.Length; i++)
                {
                    CreateNodes(1, Untrimmed[i], rTemp);
                }
            }
            ShortenNodeArray();
            //UILog(this, "Cluster Complete: " + NodeGroup.Count);
            foreach (NodeCluster e in SubClusters)
            { e.GenerateNodes(Untrimmed, Trimmed); }
            foreach (NodeCluster e in PresetClusters)
            { e.GenerateNodes(Untrimmed, Trimmed); }
        }
        public void LegacyGenerateNodes(int[][][] nodeGroups)
        {
            //if (true)
            //Setup

            DataDivisions[0] = intArgs[0][0]; // double
            DataDivisions[1] = intArgs[0][1]; // boolean
            DataDivisions[2] = intArgs[0][2]; // int
            
            int count = 0;
            for (int i = 0; i < nodeGroups.Length; i++)
            { count += nodeGroups[i].Length; }
            NodeGroup.Capacity = count;
            //
            int[][][] tempArray = StupidCopy(nodeGroups);
            // Current Generation
            for (int i = 0; i < tempArray.Length; i++)
            { tempArray[i] = RemoveInvalids(returnType, tempArray[i]); }
           // Random rTemp = new Random();
            Random rTemp = new Random(Guid.NewGuid().GetHashCode());
            for (int i = 0; i < tempArray.Length; i++)
            {
                if (Math.Abs(returnType - 2) == 1)
                    CreateNodes(2, tempArray[i], rTemp);
                else
                    CreateNodes(1, tempArray[i], rTemp);
            }
            // Sub Generation
            ShortenNodeArray();
            foreach (NodeCluster e in SubClusters)
            { e.LegacyGenerateNodes(nodeGroups); }
            foreach (NodeCluster e in PresetClusters)
            { e.LegacyGenerateNodes(nodeGroups); }
        }
        public int[][][] StupidCopy(int[][][] inputArray)
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
        // Generates the nodes for itself, and all subclusters it has //
        public void OldLegacyGenerateNodes(int combinationLevel)
        {
            int count = 0;
            for (int i = 1; i <= combinationLevel; i++)
            { count += GetSize(i); }
            DataDivisions[0] = intArgs[0][0]; // double
            DataDivisions[1] = intArgs[0][1]; // boolean
            DataDivisions[2] = intArgs[0][2]; // int
            NodeGroup.Capacity = count;
            if (combinationLevel >= 1)
            {
                // This generates the first generation
                int[][] startArray = new int[intArgs[0][0] + intArgs[0][1] + intArgs[0][2]][];
                for (int i = 0; i < startArray.Length; i++)
                {
                    startArray[i] = new int[1];
                    startArray[i][0] = (i + 1);
                }
                // Debug.WriteLine(startArray.Length + "- Length1");
                startArray = RemoveInvalids(returnType, startArray);
                Random r = new Random();
                if (returnType == 1 || returnType == 3)
                    CreateNodes(2, startArray, r);
                else
                    CreateNodes(1, startArray, r);

                for (int i = 1; i < combinationLevel; i++)
                {
                    startArray = GenerateCombinations(1, startArray);
                    startArray = RemoveInvalids(returnType, startArray);
                    if (returnType == 1 || returnType == 3)
                        CreateNodes(2, startArray, r);
                    else
                        CreateNodes(1, startArray, r);
                }
            }
            ShortenNodeArray();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            foreach (NodeCluster e in SubClusters)
            { e.OldLegacyGenerateNodes(combinationLevel); }
            foreach (NodeCluster e in PresetClusters)
            { e.OldLegacyGenerateNodes(combinationLevel); }
        }
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
                // Debug.WriteLine(newArgs.Length + " - Length");
                return GenerateCombinations((size - 1), newArgs);
            }
        }
        private int[][] RemoveInvalids(int type, int[][] array)
        {
            if (type == 1 || type == 3)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    for (int j = 0; j < array[0].Length; j++)
                    {
                        if (array[i][j] > DataDivisions[0] && array[i][j] <= DataDivisions[0] + DataDivisions[1])
                        {
                            // Debug.WriteLine(i + "- Invalid");
                            for (int l = 0; l < array[0].Length; l++)
                            { array[i][l] = -1; }
                        }
                    }
                }
            }
            return array;
        }
        private int GetSize(int level)
        {
            int inputCount = intArgs[0][0] + intArgs[0][1] + intArgs[0][2];
            int count = inputCount;
            int factorial = 1;
            for (int i = 1; i < level; i++)
            {
                count *= (inputCount - i);
                factorial *= (i + 1);
            }
            count /= factorial;
            if (level < 1)
                count = 1;
            return count;
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
        // This isn't the type of the node, but what the subcluster it belongs to returns
        // It's either 1 if it returns a boolean, or 2 if it returns a number
        private void CreateNodes(int type, int[][] dataSources, Random rand)
        {
            short temp = 0;
            for (int i = 0; i < dataSources.Length; i++)
            {
                if (dataSources[i][0] != -1)
                {
                    temp = GetNodeTypeFromDataSources(type, dataSources[i]);
                    if (temp != -1)
                    {
                        double constant = 0;
                        double constant1 = 0;
                        bool bConstant = false;
                        Random random = rand;
                        if (temp == 1 || temp == 3 || temp == 4)
                        {
                            constant = 0;
                            //constant = (random.NextDouble() * (upperBound - lowerBound)) + lowerBound;
                            //constant1 = (random.NextDouble() * (upperBound - lowerBound)) + lowerBound;
                        }
                        //if (random.NextDouble() >= 0.5 && (temp == 3 || temp == 4))
                        //    bConstant = true;
                        if (temp == 1 || temp == 4)
                        { 
                            NodeGroup.Add(new Node(temp, dataSources[i], NodeTypeCount, constant, rand, 2, lowerBound, upperBound, DataDivisions[0]));
                            NodeGroup.Add(new Node(temp, dataSources[i], NodeTypeCount, constant1, rand, 4, lowerBound, upperBound, DataDivisions[0]));
                        }
                        else if (temp == 2)
                            NodeGroup.Add(new Node(temp, dataSources[i], NodeTypeCount, bConstant, rand));
                        else if (temp == 3)
                            NodeGroup.Add(new Node(temp, dataSources[i], NodeTypeCount, constant, bConstant, rand, lowerBound, upperBound, DataDivisions[0], DataDivisions[1]));
                    }
                }
            }
        }
        private void ShortenNodeArray()
        {
            NodeGroup.TrimToSize();
        }
        // Type: 1 returns boolean, 2 returns number
        private short GetNodeTypeFromDataSources(int type, int[] sources)
        {
            int numberCount = 0;
            int booleanCount = 0;
            for (int i = 0; i < sources.Length; i++)
            {
                if (sources[i] != -1)
                {
                    if (sources[i] < DataDivisions[0])
                        numberCount++;
                    else if (sources[i] < DataDivisions[0] + DataDivisions[1])
                        booleanCount++;
                    else if (sources[i] < DataDivisions[0] + DataDivisions[1] + DataDivisions[2])
                        numberCount++;
                } 
            }
            short nodeType = -1;
            if (type == 1)
            {

                if (numberCount > 0 && booleanCount > 0)
                {
                    nodeType = 3;
                }
                else if (numberCount > 0)
                {
                    nodeType = 1;
                }
                else if (booleanCount > 0)
                {
                    nodeType = 2;
                }
            }
            else if (type == 2)
            {
                nodeType = 4;
            }
            return nodeType;
        }
        public int GetReturnType()
        { return returnType; }
        public void NodeCalculations()
        {
            Stopwatch SW = new Stopwatch();
            SW.Start();
            if (NodeGroup.Count > 0)
            {
                foreach (Node e in NodeGroup)
                {
                    e.Calculate(DataDivisions[0], DataDivisions[1], true, this, !OpeningCalculation);
                    //e.Calculate(DataDivisions[0], DataDivisions[1], true, this, false);
                }
                foreach (NodeCluster e in SubClusters)
                { e.NodeCalculations(); }
                foreach (NodeCluster e in PresetClusters)
                { e.NodeCalculations(); }
                if (OpeningCalculation)
                    OpeningCalculation = false;
            }
            SW.Stop();
            DeltaTime = SW.ElapsedMilliseconds;
        }

        public int GetNodeCount()
        {
            return NodeGroup.Count;
        }
        public int GetFullNodeCount()
        {
            int temp = NodeGroup.Count;
            foreach (NodeCluster e in SubClusters)
            { temp += e.GetFullNodeCount(); }
            foreach (NodeCluster e in PresetClusters)
            { temp += e.GetFullNodeCount(); }
            return temp;
        }
        public int GetDecisionWeight()
        {
            int temp = 0;
            foreach (Node e in NodeGroup)
            { temp += e.GetDecision(); }
            PreviousDecisionWeight = temp;
            return temp;
        }
        public object GetDecisionValue()
        {
            if (returnType == 1)
            {
                double Decision = 0;
                foreach (Node e in NodeGroup)
                { Decision += e.GetValue(); }
                return Decision;
            }
            else if (returnType == 2)
            {
                int Weight = 0;
                foreach (Node e in NodeGroup)
                { Weight += e.GetDecision(); }
                if (Weight >= 0)
                    return true;
                else
                    return false;
            }
            else
            {
                double Decision = 0;
                foreach (Node e in NodeGroup)
                { Decision += e.GetValue(); }
                int temp = (int)Math.Round(Decision);
                return temp;
            }
        }
        public int FindHighestPreset()
        {
            int temp = 0;
            int currentVal = PresetClusters[0].GetDecisionWeight();
            for (int i = 1; i < PresetClusters.Length; i++)
            {
                int l = PresetClusters[i].GetDecisionWeight();
                if (l > temp)
                    temp = l;
            }
            return temp + 1;
        }
        public object[] AssembleDecision()
        {
            // The order of this is: double, bool, int, preset
            int temp = 0;
            temp += SubClusters.Length;
            if (PresetClusters.Length > 0)
                temp++;
            object[] Decision = new object[1 + temp];
            Decision[0] = GetDecisionWeight();
            for (int i = 0; i < SubClusters.Length; i++)
            { Decision[i + 1] = SubClusters[i].GetDecisionValue(); }
            if (PresetClusters.Length > 0)
                Decision[Decision.Length - 1] = FindHighestPreset();
            return Decision;
        }
        public BitArray ReturnNodeDecisions()
        {
            BitArray temp = new BitArray(NodeGroup.Count * 2);
            for (int i = 0; i < NodeGroup.Count; i++)
            {
                if (((Node)NodeGroup[i]).GetDecision() >= 0)
                    temp.Set((i * 2), true);
                else if (((Node)NodeGroup[i]).GetDecision() == 0)
                {
                    temp.Set((i * 2), true);
                    temp.Set((i * 2) + 1, true);
                }
                //if (((Node)NodeGroup[i]).DeltaDecision())
                //    temp.Set((i * 2) + 1, true);
            }
            return temp;
        }
        public short[][] ReturnNodeOperators()
        {
            short[][] temp = new short[NodeGroup.Count][];
            for (int i = 0; i < temp.Length; i++)
            { temp[i] = ((Node)NodeGroup[i]).GetOperatorList(); }
            return temp;
        }
        public long GetDecisionTime()
        {
            return DeltaTime;
        }

        // Learning Functions
        public void StepTowardsTrue(BitArray BA, double weight)
        {
            bool Increment = false;
            if (MalleabilityConstant == CurrentMalleability)
            {
                CurrentMalleability = 0;
                Increment = true;
            }
            else
                CurrentMalleability++;

            for (int i = 0; i < NodeGroup.Count; i++)
            {
                //if (BA[(i * 2) + 1])
                //    ((Node)NodeGroup[i]).StepTowardsTrue(BA[i * 2], weight);
                if (!(BA[i * 2] && BA[(i * 2) + 1]))
                    ((Node)NodeGroup[i]).StepTowardsTrue(BA[i * 2], weight, this, Increment);
            }
        }
        public void StepTowardsFalse(BitArray BA, double weight)
        {
            bool Increment = false;
            if (MalleabilityConstant == CurrentMalleability)
            {
                CurrentMalleability = 0;
                Increment = true;
            }
            else
                CurrentMalleability++;

            for (int i = 0; i < NodeGroup.Count; i++)
            {
                //if (BA[(i * 2) + 1])
                //    ((Node)NodeGroup[i]).StepTowardsFalse(BA[i * 2], weight);
                if (!(BA[i * 2] && BA[(i * 2) + 1]))
                    ((Node)NodeGroup[i]).StepTowardsFalse(BA[i * 2], weight, this, Increment);
            }
        }
        public void StepOpposite(BitArray BA, double weight)
        {
            bool Increment = false;
            if (MalleabilityConstant == CurrentMalleability)
            {
                CurrentMalleability = 0;
                Increment = true;
            }
            else
                CurrentMalleability++;

            for (int i = 0; i < NodeGroup.Count; i++)
            {
                //if (BA[(i * 2) + 1])
                //    ((Node)NodeGroup[i]).StepOpposite(BA[i * 2], weight);
                if (!(BA[i * 2] && BA[(i * 2) + 1]))
                    ((Node)NodeGroup[i]).StepOpposite(BA[i * 2], weight, this, Increment);
            }
        }
        public bool DeltaData(int Index)
        {
            // Well this has a gigantic flaw in it, and needs to be fixed
            if (BA.Length != 0)
                return !BA.Get(Index);
            return true;
        }
        public void SetLearning(bool L)
        {
            Learning = L;
        }

        public string[] GetWeights()
        {
            int WeightsPerLine = 20;

            string[] Weights = new string[NodeGroup.Count];
            int maxLength = -1;
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = i + ": " + ((Node)NodeGroup[i]).GetWeight();
                if (Weights[i].Length > maxLength)
                    maxLength = Weights[i].Length;
            }

            for (int i = 0; i < Weights.Length; i++)
                Weights[i] = PaddText(maxLength, 2, Weights[i]);

            string Output = "";
            if (!subCluster)
                Output += "Node Cluster " + ID + Environment.NewLine;
            else
                Output += "Sub Cluster " + ID + Environment.NewLine;

            for (int i = 0; i < Weights.Length; i++)
            {
                Output += Weights[i];
                if ((i + 1) % WeightsPerLine == 0)
                    Output += Environment.NewLine;
            }
            string[] Result = new string[1 + SubClusters.Length];
            Result[0] = Output;
            for (int i = 1; i <= SubClusters.Length; i++)
            {
                Result[i] = SubClusters[i - 1].GetWeights()[0];
            }
            return Result;
        }
        public string PaddText(int MaxLength, int Padding, string Input)
        {
            int AddPadding = Padding + (MaxLength - Input.Length);
            for (int i = 0; i < AddPadding; i++)
                Input += " ";
            return Input;
        }
        //public NodeCluster[] GetSubClusters()
        //{ }
        //public NodeCluster GetChosenPresetCluster()
        //{ }
    }
}