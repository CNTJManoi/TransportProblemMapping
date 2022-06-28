using System;
using System.Windows;

namespace TransportAlgorithms.Algorithms
{
    public class Potentials : IAlgorithm
    {
        private double[,] Matrix;
        private double[] Suppliers;
        private double[] Shops;
        private double[,] Solution;
        private bool isFound;
        public Potentials(double[,] Matr, double[] Suppliers, double[] Shops)
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
            int i = 0, j = 0;
            double[,] HelpMatr = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            for (i = 0; i < Matrix.GetLength(0); i++)
                for (j = 0; j < Matrix.GetLength(1); j++)
                    if (Solution[i, j] == Solution[i, j]) HelpMatr[i, j] = Matrix[i, j];
                    else HelpMatr[i, j] = float.NaN;
            double[] U = new double[Matrix.GetLength(0)];
            double[] V = new double[Matrix.GetLength(1)];
            FindUV(U, V, HelpMatr);
            double[,] SMatr = MakeSMatr(HelpMatr, U, V);
            while (!AllPositive(SMatr))
            {
                Roll(Solution, SMatr);
                for (i = 0; i < Matrix.GetLength(0); i++)
                    for (j = 0; j < Matrix.GetLength(1); j++)
                    {
                        if (Solution[i, j] == float.PositiveInfinity)
                        {
                            HelpMatr[i, j] = Matrix[i, j];
                            Solution[i, j] = 0;
                            continue;
                        }
                        if (Solution[i, j] == Solution[i, j]) HelpMatr[i, j] = Matrix[i, j];
                        else HelpMatr[i, j] = float.NaN;
                    }
                FindUV(U, V, HelpMatr);
                SMatr = MakeSMatr(HelpMatr, U, V);
            }

            return Solution;
        }
        private void FindUV(double[] U, double[] V, double[,] HelpMatr)
        {
            bool[] U1 = new bool[Matrix.GetLength(0)];
            bool[] U2 = new bool[Matrix.GetLength(0)];
            bool[] V1 = new bool[Matrix.GetLength(1)];
            bool[] V2 = new bool[Matrix.GetLength(1)];
            while (!(AllTrue(V1) && AllTrue(U1)))
            {
                int i = -1;
                int j = -1;
                for (int i1 = Matrix.GetLength(1) - 1; i1 >= 0; i1--)
                    if (V1[i1] && !V2[i1]) i = i1;
                for (int j1 = Matrix.GetLength(0) - 1; j1 >= 0; j1--)
                    if (U1[j1] && !U2[j1]) j = j1;

                if ((j == -1) && (i == -1))
                    for (int i1 = Matrix.GetLength(1) - 1; i1 >= 0; i1--)
                        if (!V1[i1] && !V2[i1])
                        {
                            i = i1;
                            V[i] = 0;
                            V1[i] = true;
                            break;
                        }
                if ((j == -1) && (i == -1))
                    for (int j1 = Matrix.GetLength(0) - 1; j1 >= 0; j1--)
                        if (!U1[j1] && !U2[j1])
                        {
                            j = j1;
                            U[j] = 0;
                            U1[j] = true;
                            break;
                        }

                if (i != -1)
                {
                    for (int j1 = 0; j1 < Matrix.GetLength(0); j1++)
                    {
                        if (!U1[j1]) U[j1] = HelpMatr[j1, i] - V[i];
                        if (U[j1] == U[j1]) U1[j1] = true;
                    }
                    V2[i] = true;
                }

                if (j != -1)
                {
                    for (int i1 = 0; i1 < Matrix.GetLength(1); i1++)
                    {
                        if (!V1[i1]) V[i1] = HelpMatr[j, i1] - U[j];
                        if (V[i1] == V[i1]) V1[i1] = true;
                    }
                    U2[j] = true;
                }

            }
            int rt = 0;
        }
        private bool AllPositive(double[,] m)
        {
            Boolean p = true;
            for (int i = 0; (i < Matrix.GetLength(0)) && p; i++)
                for (int j = 0; (j < Matrix.GetLength(1)) && p; j++)
                    if (m[i, j] < 0) p = false;
            return p;
        }
        private bool AllTrue(bool[] arr)
        {
            return Array.TrueForAll(arr, delegate (bool x) { return x; });
        }
        private double[,] MakeSMatr(double[,] M, double[] U, double[] V)
        {

            double[,] HM = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            for (int i = 0; i < Matrix.GetLength(0); i++)
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    HM[i, j] = M[i, j];
                    if (HM[i, j] != HM[i, j])
                        HM[i, j] = Matrix[i, j] - (U[i] + V[j]);
                }
            return HM;
        }
        private Point[] GetCycle(int x, int y)
        {
            Point Beg = new Point(x, y);
            FindWay fw = new FindWay(x, y, true, Allowed, Beg, null);
            fw.BuildTree();
            Point[] Way = Array.FindAll<Point>(Allowed, delegate (Point p) { return (p.X != -1) && (p.Y != -1); });
            return Way;
        }
        private Point[] Allowed;// хранит координаты клеток, в которых есть груз
        private void Roll(double[,] m, double[,] sm)
        {
            Point minInd = new Point();
            double min = float.MaxValue;
            int k = 0;
            Allowed = new Point[Matrix.GetLength(0) + Matrix.GetLength(1)];
            for (int i = 0; i < Matrix.GetLength(0); i++)
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    if (m[i, j] == m[i, j])
                    {
                        Allowed[k].X = i;
                        Allowed[k].Y = j;
                        k++;
                    }
                    if (sm[i, j] < min)
                    {
                        min = sm[i, j];
                        minInd.X = i;
                        minInd.Y = j;
                    }
                }
            Allowed[Allowed.Length - 1] = minInd;
            Point[] Cycle = GetCycle((int)minInd.X, (int)minInd.Y);
            double[] Cycles = new double[Cycle.Length];
            Boolean[] bCycles = new Boolean[Cycle.Length];
            for (int i = 0; i < bCycles.Length; i++)
                bCycles[i] = i == bCycles.Length - 1 ? false : true;
            min = float.MaxValue;
            for (int i = 0; i < Cycle.Length; i++)
            {
                Cycles[i] = m[(int)Cycle[i].X, (int)Cycle[i].Y];
                if ((i % 2 == 0) && (Cycles[i] == Cycles[i]) && (Cycles[i] < min))
                {
                    min = Cycles[i];
                    minInd = Cycle[i];
                }
                if (Cycles[i] != Cycles[i]) Cycles[i] = 0;
            }
            int point1 = 0;
            for (int i = 0; i < Cycle.Length; i++)
            {
                if (i % 2 == 0)
                {
                    Cycles[i] -= min;
                    m[(int)Cycle[i].X, (int)Cycle[i].Y] -= min;
                }
                else
                {
                    Cycles[i] += min;
                    if (m[(int)Cycle[i].X, (int)Cycle[i].Y] != m[(int)Cycle[i].X, (int)Cycle[i].Y]) m[(int)Cycle[i].X, (int)Cycle[i].Y] = 0;
                    m[(int)Cycle[i].X, (int)Cycle[i].Y] += min;
                }
            }
            m[(int)minInd.X, (int)minInd.Y] = float.NaN;
        }
    }
}
