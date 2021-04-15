using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace OlanderIA
{
    class Logger
    {
        CommandCenter CC;
        ArrayList Data = new ArrayList();
        
        bool toggleQueue = false;
        bool Started = false;
        bool Enabled = false;
        private string path = @"C:\Users\Ethan\Desktop\Programming Tools\Projects\IA Log Files\";
        private StreamWriter StrW;
        private string[] CommandNames;

        private string RoundMoves = "";

        public Logger(CommandCenter CC, string identifier, string Name, bool InitialLoggingState, string[] CommandNames)
        {
            this.CC = CC;
            Subscribe(CC);

            this.CommandNames = CommandNames;
            
            path += identifier + ".txt";

            Enabled = InitialLoggingState;

            if (File.Exists(path))
                File.Delete(path);

            StreamWriter SW = new StreamWriter(path);

            SW.Write(Name + Environment.NewLine);

            SW.Flush();
            SW.Close();
        }

        public void Subscribe(CommandCenter c)
        {
            c.GameStart += new CommandCenter.LogCommand(LogStart);
            c.GameMode += new CommandCenter.LogCommand(LogMode);
            c.Result += new CommandCenter.LogCommand(LogResult);
            c.RoundData += new CommandCenter.LogCommand(LogData);
            c.RoundNodeDecisions += new CommandCenter.LogCommand(LogNodeDecision);
            c.RoundClusterDecisions += new CommandCenter.LogCommand(LogClusterDecision);
            c.RoundChosenCommand += new CommandCenter.LogCommand(LogCommand);
            c.NodeWeights += new CommandCenter.LogCommand(LogWeights);
            c.NodeDataSources += new CommandCenter.LogCommand(LogDataSources);
            c.NodeOperators += new CommandCenter.LogCommand(LogOperators);
            c.EndGame += new CommandCenter.LogCommand(LogEnd);
            c.ToggleLog += new CommandCenter.LogCommand(QueueToggle);
            c.StringRepresentation += new CommandCenter.LogCommand(LogGeneral);
            c.BaseLineResults += new CommandCenter.LogCommand(LogString);
            c.Time += new CommandCenter.LogCommand(LogString);
            c.ChosenMove += new CommandCenter.LogCommand(LogMove);
            c.PlayerID += new CommandCenter.LogCommand(LogPlayer);
        }
        private void QueueToggle(CommandCenter c, EventArgs e)
        {
            if (Started)
                toggleQueue = !toggleQueue;
            else
                Enabled = !Enabled;
        }
        private void LogStart(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                RoundMoves = "";
                Started = true;
                StrW = new StreamWriter(path, true);
                StrW.WriteLine(Environment.NewLine + "Game Start");
            }
        }
        private void LogGeneral(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                StreamWriter SW = new StreamWriter(path, true);

                SW.Write(CC.ToString() + Environment.NewLine);

                SW.Flush();
                SW.Close();
            }
        }
        private void LogPlayer(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                if (Started)
                {
                    StrW.WriteLine("Player: " + e.GetIntArrayData()[0]);
                }
            }
        }
        private void LogMode(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                if (Started)
                {
                    StrW.Write("Mode: ");
                    int mode = c.GetPlayMode();
                    if (mode == 0)
                        StrW.WriteLine("Learning");
                    else if (mode == 1)
                        StrW.WriteLine("Playing");
                    else if (mode == 2)
                        StrW.WriteLine("User Input");
                }
            }
        }
        private void LogString(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                try
                {
                    StreamWriter STemp = new StreamWriter(path, true);
                    STemp.WriteLine(e.GetStringData());
                    STemp.Flush();
                    STemp.Close();
                }
                catch { }
            }
        }
        private void LogResult(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                if (Started)
                {
                    StrW.WriteLine(RoundMoves);
                    int Res = e.GetIntArrayData()[0];
                    if (Res > 0)
                        StrW.WriteLine("Win: " + Res);
                    else if (Res < 0)
                        StrW.WriteLine("Loss: " + Res);
                    else
                        StrW.WriteLine("Tie: " + Res);
                }
            }
        }
        private void LogData(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {

            }
            if (Started)
            {

            }
        }
        private void LogNodeDecision(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {

            }
            if (Started)
            {

            }
        }
        private void LogClusterDecision(CommandCenter c, CommandCArgs e)
        {
            if (Enabled && Started)
            {
                StrW.WriteLine(e.GetStringData());
            }
        }
        private void LogCommand(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {

            }
            if (Started)
            {

            }

        }
        private void LogWeights(CommandCenter c, CommandCArgs e)
        {
            if (Enabled && !Started)
            {
                StreamWriter SW = new StreamWriter(path, true);

                string[][] TempWeights = e.GetString2DArrayData();
                for (int i = 0; i < TempWeights.Length; i++)
                {
                    for (int k = 0; k < TempWeights[i].Length; k++)
                    {
                        SW.WriteLine(TempWeights[i][k]);
                    }
                }

                SW.Flush();
                SW.Close();
            }
        }
        private void LogDataSources(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {

            }
        }
        private void LogOperators(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {

            }
        }
        private void LogMove(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                if (Started)
                {
                    if (RoundMoves.Length != 0)
                        RoundMoves += ", ";

                    try
                    {
                        RoundMoves += e.GetIntArrayData()[1] + ": " + CommandNames[e.GetIntArrayData()[0]];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        StrW.WriteLine("Error: The Command Names array did not match with the decision numbers");
                    }
                }
            }
        }
        private void LogEnd(CommandCenter c, CommandCArgs e)
        {
            if (Enabled)
            {
                if (Started)
                {
                    RoundMoves = "";

                    StrW.WriteLine("Game End" + Environment.NewLine);
                    Started = false;
                    SwapState();
                    StrW.Flush();
                    StrW.Close();
                }
            }
        }

        private void SwapState()
        {
            if (toggleQueue)
                Enabled = !Enabled;

            toggleQueue = false;
        }
    }
}