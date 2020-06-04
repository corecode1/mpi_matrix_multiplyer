using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Intrinsics.X86;

namespace MPI_Lab3
{
    [Serializable]
    public class Matrix
    {
        static Random r = new Random();
        private Matrix<float> source;

        public int rows { get { return source.RowCount; } }
        public int cols { get { return source.ColumnCount; } }

        public float this[int i, int j]
        {
            get
            {
                return source[i, j];
            }
        }

        public Matrix(int rows = 3, int columns = 3, bool randomFill = false, Func<int, int, float> init = null)
        {
            if (randomFill)
            {
                source = Matrix<float>.Build.Dense(rows, columns);
                Traverse(GetRandom, true);
            }
            else if (init != null)
            {
                source = Matrix<float>.Build.Dense(rows, columns, init);
                Traverse(init);
            }
            else
                source = Matrix<float>.Build.Dense(rows, columns);
        }

        public void Traverse(Func<int, int, float> func, bool updateValues = false)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (updateValues)
                        source[i, j] = func(i, j);
                    else
                        func(i, j);
                }
            }
            
        }

        private static float GetRandom(int i, int j)
        {
            double range = 10f;
            return (float)Math.Floor(r.NextDouble() * range);
        }

        public void Print()
        {
            Console.WriteLine();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{source[i, j]} ");
                }

                Console.WriteLine();
            }

            Console.WriteLine();
        }

        private Matrix(Matrix<float> source)
        {
            this.source = source;
        }

        public Matrix Transpose()
        {
            //source = source.Transpose();
            
            return new Matrix(source.Transpose());
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            ValidateAddition(a, b);

            return new Matrix(a.source + b.source);
        }

        public static Matrix operator *(Matrix a, Matrix b)
        {
            ValidateMultiplication(a, b);

            return new Matrix(a.source * b.source);
        }

        public static Matrix operator *(int a, Matrix b)
        {
            return new Matrix(a * b.source);
        }

        private static void ValidateMultiplication(Matrix a, Matrix b)
        {
            if (a.cols != b.rows)
                throw new Exception($"Error! first matrix cols: {a.cols} and second matrix rows: {b.rows} must be equal.");
        }

        private static void ValidateAddition(Matrix a, Matrix b)
        {
            if (a.cols != b.cols || a.rows != b.rows)
                throw new Exception($"Error! first matrix size: {a.rows}x{a.cols} and second matrix size: {b.rows}x{b.cols} must be equal.");
        }
    }
}
