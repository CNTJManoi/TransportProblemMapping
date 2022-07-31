using System;

namespace TransportAlgorithms.Algorithms
{
    public class NorthWest : IAlgorithm
    {
        /// <summary>
        ///     Поиск опорного плана методом северо-западного угла
        /// </summary>
        /// <param name="Matr">Матрица цен</param>
        /// <param name="Suppliers">Возможности складов</param>
        /// <param name="Shops">Потребности потребителей</param>
        public NorthWest(double[,] Matr, int[] Suppliers, int[] Shops)
        {
            Matrix = Matr;
            this.Suppliers = Suppliers;
            this.Shops = Shops;
            Clear();
        }

        #region Additional for NorthWest method

        private bool isEmpty(float[] arr)
        {
            return Array.TrueForAll(arr, delegate(float x) { return x == 0; });
        }

        private void NanToEmpty(float[,] outArr)
        {
            var ASize = Matrix.GetLength(0);
            var BSize = Matrix.GetLength(1);
            int i = 0, j = 0;
            for (i = 0; i < ASize; i++)
            for (j = 0; j < BSize; j++)
                if (outArr[i, j] == 0)
                    outArr[i, j] = float.NaN;
        }

        #endregion

        #region Fields

        private readonly double[,] Matrix;
        private readonly int[] Shops;
        private readonly int[] Suppliers;
        private bool isFound;
        private double[,] Solution;

        #endregion

        #region IAlgorithm

        public void Clear()
        {
            Solution = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            isFound = false;
        }

        public double GetMathematicalModel()
        {
            double price = 0;
            for (var i = 0; i < Solution.GetLength(0); i++)
                for (var j = 0; j < Solution.GetLength(1); j++)
                {
                    if (double.IsNaN(Solution[i, j])) price += 0;
                    else price += Solution[i, j] * Matrix[i, j];
                }
            return price;
        }

        public double[,] GetMatrixSolution()
        {
            if (isFound) return Solution;
            return null;
        }

        public double[,] ReturnSolution()
        {
            var ASize = Matrix.GetLength(0);
            var BSize = Matrix.GetLength(1);
            var Ahelp = new float[ASize];
            var Bhelp = new float[BSize];
            for (var z = 0; z < ASize; z++) Ahelp[z] = Suppliers[z];
            for (var z = 0; z < BSize; z++) Bhelp[z] = Shops[z];
            int i = 0, j = 0;
            var outArr = new float[ASize, BSize];
            NanToEmpty(outArr);
            while (!(isEmpty(Ahelp) && isEmpty(Bhelp)))
            {
                var Dif = Math.Min(Ahelp[i], Bhelp[j]);
                outArr[i, j] = Dif;
                Ahelp[i] -= Dif;
                Bhelp[j] -= Dif;
                if (Ahelp[i] == 0 && Bhelp[j] == 0 && j + 1 < BSize) outArr[i, j + 1] = 0;
                if (Ahelp[i] == 0) i++;
                if (Bhelp[j] == 0) j++;
            }

            Solution = new double[outArr.GetLength(0), outArr.GetLength(1)];
            for (var z = 0; z < outArr.GetLength(0); z++)
            for (var x = 0; x < outArr.GetLength(1); x++)
                Solution[z, x] = outArr[z, x];
            return Solution;
        }

        #endregion
    }
}