using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OlanderIA
{
    public class Node
    {
        // Instance Data
        int[] DataSources;
        short[] currentOperators;
        short NodeType;
        //
        int CurrentAverageWeight = 1;

        // The resulting values
        double SelfValue;
        int SelfDecision = 0;
        bool neutral;
        // The internal weights
        double[] InternalWeight = new double[0];
        bool[] InternalBool = new bool[0];
        //
        bool Delta = true;
        // Instance Data
        int doubleLength = 0;

        // Integer Specific Optimized Node
        public Node(short type, int[] Sources, int NodeTypeCount, double constant, Random rTemp, short OperatorType, int lowerBound, int upperBound, int doubleLength)
        {
            // This sets the default node type to be 1, in case a non-valid type is sent in
            if (type <= 0 || type > NodeTypeCount)
                type = 3;
            // Other stuff
            NodeType = type;
            DataSources = Sources;

            InternalWeight = new double[Sources.Length];
            double Temp = 0;
            for (int i = 0; i < InternalWeight.Length; i++)
                InternalWeight[i] = (rTemp.NextDouble() * (upperBound - lowerBound)) + lowerBound;

            SizeOperatorBasedOnType(type);
            SetOperator(new short[] { OperatorType });
            this.doubleLength = doubleLength;
        }

        public Node(short type, int[] Sources, int NodeTypeCount, double constant, bool bConstant, Random rTemp, int lowerBound, int upperBound, int doubleLength, int boolLength)
        {
            // This sets the default node type to be 1, in case a non-valid type is sent in
            if (type <= 0 || type > NodeTypeCount)
                type = 3;
            NodeType = type;
            DataSources = Sources;

            InternalWeight = new double[Sources.Length - boolLength];
            InternalBool = new bool[boolLength];

            for (int i = 0; i < DataSources.Length; i++)
            {
                if (DataSources[i] < doubleLength)
                    InternalWeight[i] = (rTemp.NextDouble() * (upperBound - lowerBound)) + lowerBound;
                else if (DataSources[i] < doubleLength + boolLength)
                    InternalBool[i - doubleLength] = (rTemp.NextDouble() >= 0.5);
                else
                    InternalWeight[i - boolLength] = (rTemp.NextDouble() * (upperBound - lowerBound)) + lowerBound;
            }

            SizeOperatorBasedOnType(type);
            SetOperator(GetRandomOperatorFromType(type, rTemp));

            this.doubleLength = doubleLength;
        }
        // Normal Double/Int Node
        public Node(short type, int[] Sources, int NodeTypeCount, double constant, Random rTemp, int lowerBound, int upperBound, int doubleLength)
        {
            // This sets the default node type to be 1, in case a non-valid type is sent in
            if (type <= 0 || type > NodeTypeCount)
                type = 1;
            NodeType = type;
            DataSources = Sources;

            InternalWeight = new double[Sources.Length];
            for (int i = 0; i < InternalWeight.Length; i++)
                InternalWeight[i] = (rTemp.NextDouble() * (upperBound - lowerBound)) + lowerBound;

            SizeOperatorBasedOnType(type);
            SetOperator(GetRandomOperatorFromType(type, rTemp));
            this.doubleLength = doubleLength;
        }
        public Node(short type, int[] Sources, int NodeTypeCount, bool constant, Random rTemp)
        {
            // This sets the default node type to be 1, in case a non-valid type is sent in
            if (type <= 0 || type > NodeTypeCount)
                type = 2;
            NodeType = type;
            DataSources = Sources;

            InternalBool = new bool[Sources.Length];
            for (int i = 0; i < InternalBool.Length; i++)
                InternalBool[i] = (rTemp.NextDouble() >= 0.5);


            SizeOperatorBasedOnType(type);
            SetOperator(GetRandomOperatorFromType(type, rTemp));
        }
        public void SizeOperatorBasedOnType(int type)
        {
            if (type != 3)
                currentOperators = new short[1];
            else
                currentOperators = new short[2];
        }
        public short[] GetRandomOperatorFromType(int type, Random rTemp)
        {
            short[] TempOperator = new short[1];
            Random rnd = rTemp;
            if (type == 1)
            { // Int | Double
                TempOperator[0] = (short)rnd.Next(1, 5);
            }
            else if (type == 2)
            { // Boolean
                TempOperator[0] = (short)rnd.Next(5, 11);
            }
            else if (type == 3)
            { // Both
                TempOperator = new short[2];
                TempOperator[0] = (short)rnd.Next(1, 5);
                TempOperator[1] = (short)rnd.Next(5, 11);
            }
            else if (type == 4)
            { // Int | Double
                TempOperator[0] = (short)rnd.Next(11, 15);
            }
            return TempOperator;
        }
        public void SetOperator(short[] Operators)
        {
            try
            {
                Array.Copy(Operators, currentOperators, Operators.Length);
            }
            catch (IndexOutOfRangeException)
            {
                Debug.WriteLine("Error setting operator");
            }
        }
        public void Calculate(int doubleLength, int boolLength, bool RemoveRedundantCalculations, NodeCluster e, bool WeightDeltaDecisions)
        {
            // This will need to really be fixed
            int DeltaScale = 81;
            bool Continue = false;
            if (RemoveRedundantCalculations)
            {
                for (int i = 0; i < DataSources.Length; i++)
                {
                    if (e.DeltaData(DataSources[i]))
                    {
                        Continue = true;
                        break;
                    }
                }
            }
            else
                Continue = true;

            if (Continue)
            {
                if (NodeType != 4)
                {
                    bool[] Results = new bool[DataSources.Length];
                    if (NodeType == 3)
                    {
                        for (int i = 0; i < DataSources.Length; i++)
                        {
                            // Never ending screams
                            if (DataSources[i] < doubleLength)
                                Results[i] = (bool)e.Oper.CallFunction(currentOperators[0], e.getItem(4, 0, DataSources[i]), InternalWeight[i]);
                            else if (DataSources[i] < doubleLength + boolLength)
                                Results[i] = (bool)e.Oper.CallFunction(currentOperators[1], e.getItem(4, 0, DataSources[i]), InternalBool[i - doubleLength]);
                            else
                                Results[i] = (bool)e.Oper.CallFunction(currentOperators[0], e.getItem(4, 0, DataSources[i]), InternalWeight[i - boolLength]);
                        }
                    }
                    else if (NodeType == 2)
                    {
                        for (int i = 0; i < DataSources.Length; i++)
                        {
                            Results[i] = (bool)e.Oper.CallFunction(currentOperators[0], e.getItem(4, 0, DataSources[i]), InternalBool[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < DataSources.Length; i++)
                        {
                            Results[i] = (bool)e.Oper.CallFunction(currentOperators[0], e.getItem(4, 0, DataSources[i]), InternalWeight[i]);
                        }
                    }
                    int temp = 0;
                    for (int i = 0; i < Results.Length; i++)
                    {
                        if (Results[i])
                            temp++;
                        else
                            temp--;
                    }
                    SelfDecision = temp;
                    neutral = false;
                    if (SelfDecision > 0)
                        SelfDecision = DataSources.Length;
                    else if (SelfDecision < 0)
                        SelfDecision = -1 * DataSources.Length;
                    else
                        neutral = true;
                    if (WeightDeltaDecisions)
                        SelfDecision *= DeltaScale;
                    Delta = true;
                }
                else
                {
                    double temp = 0;
                    for (int i = 0; i < DataSources.Length; i++)
                    {
                        temp += (double)e.Oper.CallFunction(currentOperators[0], e.IDT.getItem(4, 0, DataSources[i]), InternalWeight[i]);
                    }
                    SelfValue = temp;
                }
            }
            else
            {
                //if (WeightDeltaDecisions)
                //   SelfDecision = 0;
                if (Delta && WeightDeltaDecisions)
                    SelfDecision /= DeltaScale;
                 Delta = false;
            }
        }
        public bool DeltaDecision()
        {
            return Delta;
        }
        public int GetDecision()
        {
            return SelfDecision;
        }
        public double GetValue()
        {
            return SelfValue;
        }
        public double GetInternalWeight()
        {
            // Decide what to do here.
            return InternalWeight[0];
        }
        public short[] GetOperatorList()
        {
            return currentOperators;
        }
        // Learning Functions
        public void StepTowardsTrue(bool Dir, double weight, NodeCluster e, bool Increment)
        {
            // Fix this so that it works with an array of weights
            if (Dir)
                IncorporateNewWeight(Increment);
            else
            {
                if (currentOperators[0] == 1 || currentOperators[0] == 2)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight += weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false); // This is false so it doesn't increase the increment count each time
                            }
                        }
                        IncorporateNewWeight(Increment); // This happens so that the increment count is increased by just 1 after it is trained
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight += weight;
                        IncorporateNewWeight(TempWeight, Increment);
                    }
                }
                else if (currentOperators[0] == 3 || currentOperators[0] == 4)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight -= weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false);
                            }
                        }
                        IncorporateNewWeight(Increment);
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight -= weight;
                        IncorporateNewWeight(TempWeight, Increment);
                    }
                }
            }
        }
        public void StepTowardsFalse(bool Dir, double weight, NodeCluster e, bool Increment)
        {
            // Fix this so that it works with an array of weights
            if (Dir)
            {
                if (currentOperators[0] == 1 || currentOperators[0] == 2)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight -= weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false);
                            }
                        }
                        IncorporateNewWeight(Increment);
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight -= weight;
                        IncorporateNewWeight(TempWeight, Increment);
                    }
                }
                else if (currentOperators[0] == 3 || currentOperators[0] == 4)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight += weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false);
                            }
                        }
                        IncorporateNewWeight(Increment);
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight += weight;
                        IncorporateNewWeight(TempWeight, Increment);
                    }
                }
            }
            else
                IncorporateNewWeight(Increment);
        }
        public void StepOpposite(bool Dir, double weight, NodeCluster e, bool Increment)
        {
            // Fix this so that it works with an array of weights
            // Also so it adjusts in the correct direction
            if (Dir)
            {
                if (currentOperators[0] == 1 || currentOperators[0] == 2)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight += weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false);
                            }
                        }
                        IncorporateNewWeight(Increment);
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight += weight;
                        IncorporateNewWeight(TempWeight, Increment);
                    }
                }
                else if (currentOperators[0] == 3 || currentOperators[0] == 4)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight -= weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false);
                            }
                        }
                        IncorporateNewWeight(Increment);
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight -= weight;
                        IncorporateNewWeight(TempWeight, Increment);
                    }
                }
            }
            else
            {
                if (currentOperators[0] == 1 || currentOperators[0] == 2)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight -= weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false);
                            }
                        }
                        IncorporateNewWeight(Increment);
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight -= weight;
                        IncorporateNewWeight(TempWeight, Increment);
                    }
                }
                else if (currentOperators[0] == 3 || currentOperators[0] == 4)
                {
                    if (DataSources.Length > 1)
                    {
                        double TempWeight = 0;
                        for (int i = 0; i < DataSources.Length - InternalBool.Length; i++)
                        {
                            if (!(DataSources[i] >= doubleLength && DataSources[i] < doubleLength + InternalBool.Length))
                            {
                                if (DataSources[i] < doubleLength)
                                    TempWeight = (double)(e.IDT.getItem(4, 0, DataSources[i]));
                                else
                                    TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[i]));
                                TempWeight += weight; // Double check this at some point
                                IncorporateNewWeight(TempWeight, i, false);
                            }
                        }
                        IncorporateNewWeight(true);
                    }
                    else
                    {
                        double TempWeight = (int)(e.IDT.getItem(4, 0, DataSources[0]));
                        TempWeight += weight;
                        IncorporateNewWeight(TempWeight, true);
                    }
                }
            }
        }
        public string ToString()
        {
            return "";
        }

        public string GetWeight()
        {
            if (NodeType == 1 || NodeType == 4)
                return "" + InternalWeight[0];
            else if (NodeType == 2)
                return "" + InternalBool[0];
            else
                return "" + InternalWeight[0] + ", " + InternalBool[0];
        }

        // Setting Values
        public void IncorporateNewWeight(double weight, int index, bool Increment)
         {
            double temp = InternalWeight[index] * CurrentAverageWeight;
            temp += weight;
            temp /= (CurrentAverageWeight + 1);
            InternalWeight[index] = temp;

            if (Increment)
                IncrementAverageWeight();
        }
        public void IncorporateNewWeight(double weight, bool Increment)
        {
            for (int i = 0; i < InternalWeight.Length; i++)
            {
                double temp = InternalWeight[i] * CurrentAverageWeight;
                temp += weight;
                temp /= (CurrentAverageWeight + 1);
                InternalWeight[i] = temp;
            }

            if (Increment)
                IncrementAverageWeight();
        }
        public void IncorporateNewWeight(bool Increment)
        {
            // Think of this is simply solidifying how confident it should be with this decision
            if (Increment)
                IncrementAverageWeight();
        }
        private void IncrementAverageWeight()
        {
            if (CurrentAverageWeight < 1000)
                CurrentAverageWeight++;
        }
    }
}