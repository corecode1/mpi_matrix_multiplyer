using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace MPI_Lab3
{
    class LevelsData
    {
        private static Dictionary<string, Func<Matrix, Matrix, Matrix>> operations = new Dictionary<string, Func<Matrix, Matrix, Matrix>>();
        private ComputationData[][] levels;

        public int Length { get { return levels.Length; }  }

        public string FinalOperationName { 
            get
            {
                var lastLevel = levels[levels.Length - 1];
                return lastLevel[lastLevel.Length - 1].blockName;
            } 
        }

        public ComputationData[] this[int index]
        {
            get
            { 
                return levels[index];
            }
        }

        public static void RegisterOperation(string operation, Func<Matrix, Matrix, Matrix> executor)
        {
            if (operations.ContainsKey(operation))
                throw new Exception($"Error! Operation {operation} already added!");

            operations.Add(operation, executor);
        }

        public static Matrix ExecuteOperation(string operation, Matrix operandA, Matrix operandB)
        {
            if (!operations.ContainsKey(operation))
                throw new Exception($"Error! Operation {operation} not found");

            return operations[operation](operandA, operandB);
        }

        public LevelsData()
        {
            string empty = string.Empty;

            levels = new ComputationData[][] {
                new ComputationData[] {
                    new ComputationData("y2y1", "y2", "y1", (Matrix a, Matrix b) => a * b),
                    new ComputationData("y2'", "y2", empty, (Matrix a, Matrix b) => a.Transpose())
                },
                new ComputationData[] {
                    new ComputationData("y2'y2y1", "y2'", "y2y1", (Matrix a, Matrix b) => a*b),
                    new ComputationData("(y1Y32)'", "y1", "Y3", (Matrix a, Matrix b) => (a.Transpose() * (b * b)).Transpose())
                },
                new ComputationData[] {
                    new ComputationData("y'2 + y2'y2y1", "y2'", "y2'y2y1", (Matrix a, Matrix b) => a + b),
                    new ComputationData("y1 + (y1Y32)'", "y1", "(y1Y32)'", (Matrix a, Matrix b) => a + b)
                },
                new ComputationData[] {
                    new ComputationData("y'2 +y1 + (y1Y32)' + y2'y2y1", "y'2 + y2'y2y1", "y1 + (y1Y32)'", (Matrix a, Matrix b) => a + b),
                },
            };
        }
    }
}
