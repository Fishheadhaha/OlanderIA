using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlanderIA
{
    public class CommandCArgs : EventArgs
    {
        private string stringData;
        private string[][] string2DArrayData;
        private int[] intArrayData;

        public CommandCArgs(int[] Data)
        {
            intArrayData = Data;
        }
        public CommandCArgs(string Data)
        {
            stringData = Data;
        }
        public CommandCArgs(string[][] Data)
        {
            string2DArrayData = Data;
        }
        
        public int[] GetIntArrayData()
        {
            return intArrayData;
        }
        public string[][] GetString2DArrayData()
        {
            return string2DArrayData;
        }

        public string GetStringData()
        {
            return stringData;
        }
    }
}
