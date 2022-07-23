using System;
using System.Windows;

namespace TransportAlgorithms.Algorithms
{
    public class Potentials : IAlgorithm
    {
        private Point[] Allowed;
        private bool isFound;
        private readonly double[,] Matrix;
        private int[] Shops;
        private double[,] Solution;
        private int[] Suppliers;

        public Potentials(double[,] Matr, int[] Suppliers, int[] Shops, double[,] Solut)
        {
            Matrix = Matr;
            this.Suppliers = Suppliers;
            this.Shops = Shops;
            Solution = Solut;
            isFound = false;
        }

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
                price += Solution[i, j] * Matrix[i, j];
            return price;
        }

        public double[,] GetMatrixSolution()
        {
            if (isFound) return Solution;
            return null;
        }

        public double[,] ReturnSolution()
        {
            int i = 0, j = 0;
            var HelpMatr = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            for (i = 0; i < Matrix.GetLength(0); i++)
            for (j = 0; j < Matrix.GetLength(1); j++)
                if (Solution[i, j] == Solution[i, j]) HelpMatr[i, j] = Matrix[i, j];
                else HelpMatr[i, j] = float.NaN;
            var U = new double[Matrix.GetLength(0)];
            var V = new double[Matrix.GetLength(1)];
            FindUV(U, V, HelpMatr);
            var SMatr = MakeSMatr(HelpMatr, U, V);
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
            var U1 = new bool[Matrix.GetLength(0)];
            var U2 = new bool[Matrix.GetLength(0)];
            var V1 = new bool[Matrix.GetLength(1)];
            var V2 = new bool[Matrix.GetLength(1)];
            while (!(AllTrue(V1) && AllTrue(U1)))
            {
                var i = -1;
                var j = -1;
                for (var i1 = Matrix.GetLength(1) - 1; i1 >= 0; i1--)
                    if (V1[i1] && !V2[i1])
                        i = i1;
                for (var j1 = Matrix.GetLength(0) - 1; j1 >= 0; j1--)
                    if (U1[j1] && !U2[j1])
                        j = j1;

                if (j == -1 && i == -1)
                    for (var i1 = Matrix.GetLength(1) - 1; i1 >= 0; i1--)
                        if (!V1[i1] && !V2[i1])
                        {
                            i = i1;
                            V[i] = 0;
                            V1[i] = true;
                            break;
                        }

                if (j == -1 && i == -1)
                    for (var j1 = Matrix.GetLength(0) - 1; j1 >= 0; j1--)
                        if (!U1[j1] && !U2[j1])
                        {
                            j = j1;
                            U[j] = 0;
                            U1[j] = true;
                            break;
                        }

                if (i != -1)
                {
                    for (var j1 = 0; j1 < Matrix.GetLength(0); j1++)
                    {
                        if (!U1[j1]) U[j1] = HelpMatr[j1, i] - V[i];
                        if (U[j1] == U[j1]) U1[j1] = true;
                    }

                    V2[i] = true;
                }

                if (j != -1)
                {
                    for (var i1 = 0; i1 < Matrix.GetLength(1); i1++)
                    {
                        if (!V1[i1]) V[i1] = HelpMatr[j, i1] - U[j];
                        if (V[i1] == V[i1]) V1[i1] = true;
                    }

                    U2[j] = true;
                }
            }

            var rt = 0;
        }

        private bool AllPositive(double[,] m)
        {
            var p = true;
            for (var i = 0; i < Matrix.GetLength(0) && p; i++)
            for (var j = 0; j < Matrix.GetLength(1) && p; j++)
                if (m[i, j] < 0)
                    p = false;
            return p;
        }

        private bool AllTrue(bool[] arr)
        {
            return Array.TrueForAll(arr, delegate(bool x) { return x; });
        }

        private double[,] MakeSMatr(double[,] M, double[] U, double[] V)
        {
            var HM = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            for (var i = 0; i < Matrix.GetLength(0); i++)
            for (var j = 0; j < Matrix.GetLength(1); j++)
            {
                HM[i, j] = M[i, j];
                if (HM[i, j] != HM[i, j])
                    HM[i, j] = Matrix[i, j] - (U[i] + V[j]);
            }

            return HM;
        }

        private Point[] GetCycle(int x, int y)
        {
            var Beg = new Point(x, y);
            var fw = new FindWay(x, y, true, Allowed, Beg, null);
            fw.BuildTree();
            var Way = Array.FindAll(Allowed, delegate(Point p) { return p.X != -1 && p.Y != -1; });
            return Way;
        }

        private void Roll(double[,] m, double[,] sm)
        {
            var minInd = new Point();
            double min = float.MaxValue;
            var k = 0;
            Allowed = new Point[Matrix.GetLength(0) + Matrix.GetLength(1)];
            for (var i = 0; i < Matrix.GetLength(0); i++)
            for (var j = 0; j < Matrix.GetLength(1); j++)
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
            var Cycle = GetCycle((int)minInd.X, (int)minInd.Y);
            var Cycles = new double[Cycle.Length];
            var bCycles = new bool[Cycle.Length];
            for (var i = 0; i < bCycles.Length; i++)
                bCycles[i] = i == bCycles.Length - 1 ? false : true;
            min = float.MaxValue;
            for (var i = 0; i < Cycle.Length; i++)
            {
                Cycles[i] = m[(int)Cycle[i].X, (int)Cycle[i].Y];
                if (i % 2 == 0 && Cycles[i] == Cycles[i] && Cycles[i] < min)
                {
                    min = Cycles[i];
                    minInd = Cycle[i];
                }

                if (Cycles[i] != Cycles[i]) Cycles[i] = 0;
            }

            var point1 = 0;
            for (var i = 0; i < Cycle.Length; i++)
                if (i % 2 == 0)
                {
                    Cycles[i] -= min;
                    m[(int)Cycle[i].X, (int)Cycle[i].Y] -= min;
                }
                else
                {
                    Cycles[i] += min;
                    if (m[(int)Cycle[i].X, (int)Cycle[i].Y] != m[(int)Cycle[i].X, (int)Cycle[i].Y])
                        m[(int)Cycle[i].X, (int)Cycle[i].Y] = 0;
                    m[(int)Cycle[i].X, (int)Cycle[i].Y] += min;
                }

            m[(int)minInd.X, (int)minInd.Y] = float.NaN;
        }
    }
}