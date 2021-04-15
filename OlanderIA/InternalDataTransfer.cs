namespace OlanderIA
{
    public class InternalDataTransfer
    {
        bool CalculationsInProgress = false;
        int[][] intArgs = new int[0][];
        double[][] doubleArgs = new double[0][];
        bool[][] boolArgs = new bool[0][];
        object[][] objectArgs = new object[0][];
        string[][] stringArgs = new string[0][];
        
        // Constructor: No instance data needs to be made
        public InternalDataTransfer() {}

        // Output
        public int[][] getIntArgs()
        {
            return intArgs;
        }
        public double[][] getDoubleArgs()
        {
            return doubleArgs;
        }
        public bool[][] getBoolArgs()
        {
            return boolArgs;
        }
        public object[][] getObjectArgs()
        {
            return objectArgs;

        }
        public string[][] getStringArgs()
        {
            return stringArgs;
        }

        // Input
        public void setIntArgs(int[][] inputInt)
        {
            if (!CalculationsInProgress)
                intArgs = inputInt;
        }
        public void setIntArgs(int[] inputInt)
        {
            if (!CalculationsInProgress)
            {
                intArgs = new int[1][];
                intArgs[0] = inputInt;
            }
        }
        public void setDoubleArgs(double[][] inputDouble)
        {
            if (!CalculationsInProgress)
                doubleArgs = inputDouble;
        }
        public void setDoubleArgs(double[] inputDouble)
        {
            if (!CalculationsInProgress)
            {
                doubleArgs = new double[1][];
                doubleArgs[0] = inputDouble;
            }
        }
        public void setBoolArgs(bool[][] inputBool)
        {
            if (!CalculationsInProgress)
                boolArgs = inputBool;
        }
        public void setBoolArgs(bool[] inputBool)
        {
            if (!CalculationsInProgress)
            {
                boolArgs = new bool[1][];
                boolArgs[0] = inputBool;
            }
        }
        public void setStringArgs(string[][] inputString)
        {
            if (!CalculationsInProgress)
                stringArgs = inputString;
        }
        public void setStringArgs(string[] inputString)
        {
            if (!CalculationsInProgress)
            {
                stringArgs = new string[1][];
                stringArgs[0] = inputString;
            }
        }
        public void setObjectArgs(object[][] inputObject)
        {
            if (!CalculationsInProgress)
                objectArgs = inputObject;
        }
        public void setObjectArgs(object[] inputObject)
        {
            if (!CalculationsInProgress)
            {
                objectArgs = new object[1][];
                objectArgs[0] = inputObject;
            }
        }

        public void SetState(bool state)
        {
            CalculationsInProgress = state;
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
                return objectArgs[index1][index2];
        }
    }
}
