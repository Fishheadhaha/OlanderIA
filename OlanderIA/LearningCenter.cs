using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace OlanderIA
{
    class LearningCenter
    {
        InternalDataTransfer IDT;
        // If this is true, it means that there are more than one sources of data coming in per round
        bool SuccessiveLogging = false;

        ArrayList Rounds = new ArrayList();
        ArrayList Round1 = new ArrayList();
        //
        public LearningCenter(InternalDataTransfer IDT)
        {
            this.IDT = IDT;
        }
        public void StartRound()
        {
            Round1 = new ArrayList();
        }
        public void LogNodeCluster(NodeCluster ChosenCluster, object[] RunData)
        {
                Round1.Add(ChosenCluster);
                Round1.Add(ChosenCluster.ReturnNodeDecisions());
                Round1.Add(RunData);
        }
        public void FinalizeRound(int Score)
        {
            Round1.Add(Score);
            Round1.TrimToSize();

            if (SuccessiveLogging)
            {
                // This will eventually be updated
                InternalLearning();
            }
            else
                InternalLearning();
            Round1 = new ArrayList();
        }
        public void InternalLearning()
        {
            double temp = ((int)Round1[Round1.Count - 1]); // The score from that round
            double Weight = temp / 100;
            if (temp > 0) // Means it won | Temporary change, fix this | it should be (temp > 0)
            {
                IDT.SetState(false);
                for (int i = 0; i < (Round1.Count - 1) / 3; i++)
                {
                    ((NodeCluster)Round1[i * 3]).SetLearning(true);
                    IDT.setObjectArgs((object[])Round1[(i * 3) + 2]);
                    IDT.SetState(true);
                    ((NodeCluster)Round1[i * 3]).GetAllData();
                    ((NodeCluster)Round1[i * 3]).StepTowardsTrue((BitArray)Round1[(i * 3) + 1], (Weight * (i + 1)));
                    IDT.SetState(false);
                }
            }
            else if (temp < 0) // Means it lost | Also fix this one (temp < 0)
            {
                IDT.SetState(false);
                for (int i = 0; i < (Round1.Count - 1) / 3; i++)
                {
                    ((NodeCluster)Round1[i * 3]).SetLearning(true);
                    IDT.setObjectArgs((object[])Round1[(i * 3) + 2]);
                    IDT.SetState(true);
                    ((NodeCluster)Round1[i * 3]).GetAllData();
                    ((NodeCluster)Round1[i * 3]).StepTowardsFalse((BitArray)Round1[(i * 3) + 1], (Weight * -(i + 1)));
                    IDT.SetState(false);
                }
            }
            else // Means it tied
            {
                IDT.SetState(false);
                for (int i = 0; i < (Round1.Count - 1) / 3; i++)
                {
                    ((NodeCluster)Round1[i * 3]).SetLearning(true);
                    IDT.setObjectArgs((object[])Round1[(i * 3) + 2]);
                    IDT.SetState(true);
                    ((NodeCluster)Round1[i * 3]).GetAllData();
                    ((NodeCluster)Round1[i * 3]).StepOpposite((BitArray)Round1[(i * 3) + 1], (1 * (i + 1)));
                    IDT.SetState(false);
                }
            }
            IDT.SetState(false);
        }
        // These will come later
        public void WriteRound()
        {

        }
        public void ReadRound(String filePath)
        {

        }
    }
}