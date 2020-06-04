using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;


namespace MPI_Lab3
{
   public class ComputationData
    {
        public ComputationBlock computationBlock { get; private set; }
        public string blockName;
        public string operandAName;
        public string operandBName;

        public ComputationData(string blockName, string operandA, string operandB, Func<Matrix, Matrix, Matrix> execute)
        {
            this.blockName = blockName;
            this.operandAName = operandA;
            this.operandBName = operandB;

            LevelsData.RegisterOperation(blockName, execute);

            computationBlock = new ComputationBlock(blockName);
        }
    }
}
