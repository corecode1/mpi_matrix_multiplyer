using System;
using System.Collections.Generic;
using System.Text;

namespace MPI_Lab3
{
    [Serializable]
    public class ComputationBlock
    {
        string blockName = null;
        public Matrix operandA { get; private set; }
        public Matrix operandB { get; private set; }

        public ComputationBlock(string blockName)
        {
            this.blockName = blockName;
        }

        public void SetOperands(Matrix A, Matrix B)
        {
            operandA = A;
            operandB = B;
        }

        public Matrix Execute()
        {
            if (operandA == null)
                throw new Exception("Error! Operands not set.");

            try
            {
                Matrix result = LevelsData.ExecuteOperation(blockName, operandA, operandB);
                return result;
            }

            catch(Exception e)
            {
                Console.WriteLine("Error in block :" + blockName);
                throw e;
            }
        }
    }
}
