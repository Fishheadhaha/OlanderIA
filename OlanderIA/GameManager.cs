using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OlanderIA
{
    class GameManager
    {
        private bool InternalSimulator;
        private TicTacToe Sim;
        private CommunicationCenter CC;
        private CommandCenter[] AIs;
        private MainUI MUI;
        private object CompiledSim;

        private int PlayerMode = -1;

        private int Turn = 1;

        public GameManager()
        {

        }
        public GameManager(bool InternalSimulator, TicTacToe Sim, CommunicationCenter CC, CommandCenter[] AIs, MainUI MUI)
        {
            this.InternalSimulator = InternalSimulator;
            this.Sim = Sim ?? throw new ArgumentNullException("The internal simulator cannot be null");
            this.CC = CC;
            this.AIs = AIs;
            this.MUI = MUI;
        }

        // Setter Methods
        public void SetSimMode(bool Internal)
        {
            if (!Internal && CompiledSim == null)
                throw new ArgumentNullException("Cannot use the compiled simulator if it is null");
            else
                InternalSimulator = Internal;
        }
        public void SetSim(TicTacToe Sim)
        {
            this.Sim = Sim;
        }
        public void SetCommunicationCenter(CommunicationCenter CC)
        {
            this.CC = CC;
        }
        public void SetCommandCenters(CommandCenter[] AIs)
        {
            this.AIs = AIs;
        }
        public void SetUI(MainUI MUI)
        {
            this.MUI = MUI;
        }
        public void RunTimeSimulation(object Sim)
        {
            CompiledSim = Sim;
        }
        // End Setter Methods

        // Player Mode Chart:
        // 0 = Human vs Human
        // 1 = Human vs Ai
        // 2 = Ai vs Ai
        // 3 = Ai vs Baseline
        // 4 = Ai vs Random

        public void UnifiedStart(int PlayerMode)
        {
            this.PlayerMode = PlayerMode;

            if (PlayerMode == 0)
            {
                //This is housed within the UI.
            }
            else if (PlayerMode == 1)
            {
                //Uses the UI, and all but 1 of the main command centers. This odd one out should be consistent.
            }
            else if (PlayerMode == 2)
            {
                //Uses all of main command centers.
            }
            else if (PlayerMode == 3)
            {
                //Uses each of the main command centers, and the baseline center. This should start the baseline center each time, but it should never be set to learn.
                //This is housed within the communication center.
            }
            else if (PlayerMode == 4)
            {
                //Uses each of the main command centers, and the randomizer.
                //This is hosued within the communiation center.
            }
        }
        public void UnifiedEnd()
        {
            Turn = -1;
        }

        public void PlayRound()
        {

        }

        public int SubmitMove(object[] Move)
        {
            if (InternalSimulator)
            {

                return Sim.ReturnChosenCommand();
            }
            else
            {

                return (int)CC.InvokeDictionaryMethod(6, new object[0]);
            }
        }

        public int GetTurn()
        {
            return Turn; ;
        }
    }
}
