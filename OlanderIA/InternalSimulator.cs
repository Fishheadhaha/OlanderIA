using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlanderIA
{
    public interface InternalSimulator
    {
        int[][] GetCommandSignature();
        object[] GetData(int Player);
        int[] GetDataSignature();
        string[] GetCommandNames();
        void SetCommand(object[] commandArgs);
        int[] ReturnRecommendedRange();
        int ReturnChosenCommand();
        int Turn();
        void SetGameMode(int mode);
        void Start();
        void Start(int FirstPlayer);
        bool Finished();
        int GetResult(int Player);
        int GetUnweightedResult(int Player);
        int GetPlayerCount();
    }
}
