using System;

namespace TransportAlgorithms.Algorithms
{
    public class NorthWest : IAlgorithm
    {
        private double[,] Matrix;
        private double[] Suppliers;
        private double[] Shops;
        private double[,] Solution;
        private bool isFound;
        public NorthWest(double[,] Matr, double[] Suppliers, double[] Shops)
        {
            Matrix = Matr;
            this.Suppliers = Suppliers;
            this.Shops = Shops;
            Clear();
        }
        public void Clear()
        {
            Solution = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            isFound = false;
        }

        public double GetMathematicalModel()
        {
            double price = 0;
            for (int i = 0; i < Solution.GetLength(0); i++)
            {
                for (int j = 0; j < Solution.GetLength(1); j++)
                {
                    price += Solution[i, j] * Matrix[i, j];
                }
            }
            return price;
        }

        public double[,] GetMatrixSolution()
        {
            if (isFound) return Solution;
            else return null;
        }

        public double[,] ReturnSolution()
        {
            double[] Ahelp = Suppliers;
            double[] Bhelp = Shops;
            int i = 0, j = 0;
            while (!(isEmpty(Ahelp) && isEmpty(Bhelp)))
            {
                double Dif = Math.Min(Ahelp[i], Bhelp[j]);
                Solution[i, j] = Dif;
                Ahelp[i] -= Dif; Bhelp[j] -= Dif;
                if ((Ahelp[i] == 0) && (Bhelp[j] == 0) && (j + 1 < Solution.GetLength(1))) Solution[i, j + 1] = 0;
                if (Ahelp[i] == 0) i++;
                if (Bhelp[j] == 0) j++;
            }
            isFound = true;
            return Solution;
        }
        bool isEmpty(double[] arr)
        {
            return Array.TrueForAll(arr, delegate (double x) { return x == 0; });
        }
    }
}
