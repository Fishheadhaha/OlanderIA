using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlanderIA
{
    class Randomizer
    {
        Random Input = new Random();
        int DecisionCount = 0;

        public Randomizer(int DecisionCount)
        {
            this.DecisionCount = DecisionCount;
        }

        public object[] GetRandomDecision()
        {
            object[] Decision = new object[1];
            Decision[0] = new object[1];
            ((object[])Decision[0])[0] = Input.Next(0, DecisionCount);

            return Decision;
        }
        public object[] CreateDecisionFromInt(int input)
        {
            object[] Decision = new object[1];
            Decision[0] = new object[1];
            ((object[])Decision[0])[0] = input;

            return Decision;
        }

        public int ReturnDecisionCount()
        {
            return DecisionCount;
        }

        public int ReturnRandomIntInRange(int lowEnd, int highEnd)
        {
            return Input.Next(lowEnd, highEnd);
        }
    }
}
