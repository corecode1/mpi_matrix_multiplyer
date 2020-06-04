using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace MPI_Lab3
{
    class InputData
    {
        public Matrix y1;
        public Matrix y2;
        public Matrix Y3;
        public int n { get; }

        public InputData(int n = 5)
        {
            this.n = n;

            y1 = Calcalatey1();
            y2 = Calcalatey2();
            Y3 = CalcalateY3();

            Console.WriteLine("Generated Input :");
            
            Console.WriteLine("y1 : ");
            y1.Print();

            Console.WriteLine("y2 : ");
            y2.Print();

            Console.WriteLine("Y3 : ");
            Y3.Print();
        }

        Matrix Calcalatey1()
        {
            Matrix b = new Matrix(n, 1, false, bGenerator);
            Matrix A = new Matrix(n, n, true);

            return A * b;
        }

        Matrix Calcalatey2()
        {
            Matrix A1 = new Matrix(n, n, true);
            Matrix b1 = new Matrix(1, n, true);
            Matrix c1 = new Matrix(1, n, true);

           return (A1 * (b1.Transpose() + c1.Transpose())).Transpose();
        }

        Matrix CalcalateY3()
        {
            Matrix C2 = new Matrix(n, n, false, C2Generator);
            Matrix A2 = new Matrix(n, n, true);
            Matrix B2 = new Matrix(n, n, true);

            return A2 * (23 * B2 + C2);
        }

        private float bGenerator(int row, int col) {
            float i = row;
            float val = row % 2 == 0 ? i / 21 : 21 / i * i;
            val = (float) Math.Round(val, 0);
            
            return val;
        }

        private float C2Generator(int row, int col)
        {
            float val = 23 / (float)Math.Pow(3 * row + col, 2);

            if (float.IsInfinity(val))
                val = 0;

            return (float)Math.Round(val, 0);
        }
    }
}
