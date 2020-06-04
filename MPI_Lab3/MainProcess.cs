using System;
using System.Collections.Generic;
using System.Text;
using MPI;


namespace MPI_Lab3
{
    class MainProcess : Process
    {
        private InputData inputData;
        private LevelsData levelsData;
        private Queue<int> availableProcesses;
        private Queue<KeyValuePair<int, ComputationData>> busyProcesses;
        private Dictionary<string, Matrix> computationResults;
        private Timer timer;

        public MainProcess(Intracommunicator communicator, int size = 55) : base(communicator)
        {
            busyProcesses = new Queue<KeyValuePair<int, ComputationData>>();
            computationResults = new Dictionary<string, Matrix>();
            availableProcesses = new Queue<int>();
            levelsData = new LevelsData();
            inputData = new InputData(size);
            timer = new Timer();

            SaveResult("y1", inputData.y1);
            SaveResult("y2", inputData.y2);
            SaveResult("Y3", inputData.Y3);
            SaveResult(string.Empty, null);

            Console.WriteLine("Operations logging: " + (Config.logOperations ? "enabled" : "disabled"));
            Console.WriteLine("Execution Type: " + (Config.multiProcess ? "parallel" : "single"));

            WriteTitle("COMPUTATION START");
            timer.Start();

            if (Config.multiProcess)
                ComputeInMultiple();
            else
                ComputeInSingle();

            timer.Stop();
            WriteTitle("COMPUTATION END");
            
            Console.WriteLine("Final Result:");
            computationResults[levelsData.FinalOperationName].Print();

            Console.WriteLine();
            Console.WriteLine($"Execution Time in ms: {timer.Elapsed}.0");
            Console.WriteLine();
        }

        private void ComputeInSingle()
        {
            ComputationData[] level; ;
            ComputationData cData;
            ComputationBlock cBlock;
            Matrix result;

            for (int i = 0; i < levelsData.Length; i++)
            {
                level = levelsData[i];

                for (int j = 0; j < level.Length; j++)
                {
                    cData = level[j];
                    cBlock = cData.computationBlock;
                    cBlock.SetOperands(computationResults[cData.operandAName], computationResults[cData.operandBName]);
                    DisplayOperation(cData.blockName, cData.operandAName, cData.operandBName, cBlock.operandA, cBlock.operandB);

                    result = cBlock.Execute();
                    DisplayResult(cData.blockName, result);
                    SaveResult(cData.blockName, result);
                }

                Console.WriteLine($"Level {i + 1} / {levelsData.Length} completed");
            }
        }

        private void ComputeInMultiple()
        {
            InitializeProcesses();
            PerformCalaculation();
            TerminateProcesses();
        }

        private void WriteTitle(string title)
        {
            Console.WriteLine($"\n****************************************\n          {title} \n**************************************** \n");
        }

        private void InitializeProcesses()
        {
            IterateProcesses((int i) => availableProcesses.Enqueue(i));
        }

        private void PerformCalaculation()
        {

            for (int i = 0; i < levelsData.Length; i++)
            {
                SendData(levelsData[i]);
                CollectResults();

                Console.WriteLine($"Level {i + 1} / {levelsData.Length} completed");
            }
        }

        private void SendData(ComputationData[] level)
        {
            for (int j = 0; j < level.Length; j++)
            {
                ComputationData cData = level[j];
                ComputationBlock cBlock = cData.computationBlock;
                cBlock.SetOperands(computationResults[cData.operandAName], computationResults[cData.operandBName]);

                if (availableProcesses.Count > 0)
                {
                    int processRank = availableProcesses.Dequeue();
                    SendMessage(Message.BlockSend, processRank);
                    communicator.Send(cBlock, processRank, 0);
                    busyProcesses.Enqueue(new KeyValuePair<int, ComputationData>(processRank, cData));
                }

                else
                {
                    DisplayOperation(cData.blockName, cData.operandAName, cData.operandBName, cBlock.operandA, cBlock.operandB);
                    Matrix result = cBlock.Execute();
                    DisplayResult(cData.blockName, result);
                    SaveResult(cData.blockName, result);
                }
            }
        }

        private void CollectResults()
        {
            while (busyProcesses.Count > 0)
            {
                var pair = busyProcesses.Dequeue();
                int processRank = pair.Key;
                ComputationData cData = pair.Value;
                ComputationBlock cBlock = cData.computationBlock;
                Matrix result = communicator.Receive<Matrix>(processRank, 0);

                availableProcesses.Enqueue(processRank);
                DisplayOperation(cData.blockName, cData.operandAName, cData.operandBName, cBlock.operandA, cBlock.operandB);
                DisplayResult(cData.blockName, result);

                SaveResult(pair.Value.blockName, result);
            }
        }

        private void TerminateProcesses()
        {
            IterateProcesses((int i) => SendMessage(Message.TerminateProcess, i));
        }

        private void IterateProcesses(System.Action<int> action)
        {
            for (int i = 0; i < communicator.Size; i++)
            {
                if (i != communicator.Rank)
                    action(i);
            }
        }

        private void SaveResult(string name, Matrix result)
        {
            if(computationResults.ContainsKey(name))
            {
                throw new Exception($"Error! comcomputationResults already contains result {name}");
            }

            computationResults.Add(name, result);
        }

        private void SendMessage(Message msg, int receiverRank)
        {
            communicator.Send((int)msg, receiverRank, 0);
        }
    }
}
