using System;
using System.Windows;

namespace TransportAlgorithms.Algorithms
{
    public class Potentials : IAlgorithm
    {
        /// <summary>
        ///     Поиск оптимального плана методом наименьших квадратов (метод потенциалов)
        /// </summary>
        /// <param name="Matr">Матрица цен</param>
        /// <param name="Suppliers">Возможности складов</param>
        /// <param name="Shops">Потребности потребителей</param>
        /// <param name="Solut">Найденное опорное решение</param>
        public Potentials(double[,] Matr, int[] Suppliers, int[] Shops, double[,] Solut)
        {
            mC = new float[Matr.GetLength(0), Matr.GetLength(1)];
            for (var i = 0; i < Matr.GetLength(0); i++)
            for (var j = 0; j < Matr.GetLength(1); j++)
                mC[i, j] = (float)Matr[i, j];
            SupArr = new float[Solut.GetLength(0), Solut.GetLength(1)];
            for (var i = 0; i < Solut.GetLength(0); i++)
            for (var j = 0; j < Solut.GetLength(1); j++)
                SupArr[i, j] = (float)Solut[i, j];
            this.Suppliers = Suppliers;
            this.Shops = Shops;
            isFound = false;
        }

        #region Fields

        private readonly float[,] mC;
        private Point[] Allowed;
        private bool isFound;
        private int[] Shops;
        private float[,] SupArr;
        private int[] Suppliers;

        #endregion

        #region IAlgorithm

        public void Clear()
        {
            SupArr = new float[mC.GetLength(0), mC.GetLength(1)];
            isFound = false;
        }

        public double GetMathematicalModel()
        {
            double price = 0;
            for (var i = 0; i < SupArr.GetLength(0); i++)
            for (var j = 0; j < SupArr.GetLength(1); j++)
                price += SupArr[i, j] * mC[i, j];
            return price;
        }

        public double[,] GetMatrixSolution()
        {
            if (isFound)
            {
                var res = new double[SupArr.GetLength(0), SupArr.GetLength(1)];
                for (var z = 0; z < SupArr.GetLength(0); z++)
                for (var x = 0; x < SupArr.GetLength(1); x++)
                    res[z, x] = SupArr[z, x];
                return res;
            }

            return null;
        }

        public double[,] ReturnSolution()
        {
            var ASize = mC.GetLength(0);
            var BSize = mC.GetLength(1);
            int i = 0, j = 0;
            var HelpMatr = new float[ASize, BSize];
            for (i = 0; i < ASize; i++)
            for (j = 0; j < BSize; j++)
                if (SupArr[i, j] == SupArr[i, j]) HelpMatr[i, j] = mC[i, j];
                else HelpMatr[i, j] = float.NaN;
            var U = new float[ASize];
            var V = new float[BSize];
            FindUV(U, V, HelpMatr);
            var SMatr = MakeSMatr(HelpMatr, U, V);
            while (!AllPositive(SMatr))
            {
                Roll(SupArr, SMatr);
                for (i = 0; i < ASize; i++)
                for (j = 0; j < BSize; j++)
                {
                    if (SupArr[i, j] == float.PositiveInfinity)
                    {
                        HelpMatr[i, j] = mC[i, j];
                        SupArr[i, j] = 0;
                        continue;
                    }

                    if (SupArr[i, j] == SupArr[i, j]) HelpMatr[i, j] = mC[i, j];
                    else HelpMatr[i, j] = float.NaN;
                }

                FindUV(U, V, HelpMatr);
                SMatr = MakeSMatr(HelpMatr, U, V);
            }

            NullToZero();
            var res = new double[SupArr.GetLength(0), SupArr.GetLength(1)];
            for (var z = 0; z < SupArr.GetLength(0); z++)
            for (var x = 0; x < SupArr.GetLength(1); x++)
                res[z, x] = SupArr[z, x];
            return res;
        }

        #endregion

        #region Additional for method Potentials

        private void FindUV(float[] U, float[] V, float[,] HelpMatr)
        {
            var ASize = mC.GetLength(0);
            var BSize = mC.GetLength(1);
            var U1 = new bool[ASize];
            var U2 = new bool[ASize];
            var V1 = new bool[BSize];
            var V2 = new bool[BSize];
            while (!(AllTrue(V1) && AllTrue(U1)))
            {
                var i = -1;
                var j = -1;
                for (var i1 = BSize - 1; i1 >= 0; i1--)
                    if (V1[i1] && !V2[i1])
                        i = i1;
                for (var j1 = ASize - 1; j1 >= 0; j1--)
                    if (U1[j1] && !U2[j1])
                        j = j1;

                if (j == -1 && i == -1)
                    for (var i1 = BSize - 1; i1 >= 0; i1--)
                        if (!V1[i1] && !V2[i1])
                        {
                            i = i1;
                            V[i] = 0;
                            V1[i] = true;
                            break;
                        }

                if (j == -1 && i == -1)
                    for (var j1 = ASize - 1; j1 >= 0; j1--)
                        if (!U1[j1] && !U2[j1])
                        {
                            j = j1;
                            U[j] = 0;
                            U1[j] = true;
                            break;
                        }

                if (i != -1)
                {
                    for (var j1 = 0; j1 < ASize; j1++)
                    {
                        if (!U1[j1]) U[j1] = HelpMatr[j1, i] - V[i];
                        if (U[j1] == U[j1]) U1[j1] = true;
                    }

                    V2[i] = true;
                }

                if (j != -1)
                {
                    for (var i1 = 0; i1 < BSize; i1++)
                    {
                        if (!V1[i1]) V[i1] = HelpMatr[j, i1] - U[j];
                        if (V[i1] == V[i1]) V1[i1] = true;
                    }

                    U2[j] = true;
                }
            }

            var rt = 0;
        }

        private bool AllPositive(float[,] m)
        {
            var ASize = mC.GetLength(0);
            var BSize = mC.GetLength(1);
            var p = true;
            for (var i = 0; i < ASize && p; i++)
            for (var j = 0; j < BSize && p; j++)
                if (m[i, j] < 0)
                    p = false;
            return p;
        }

        private bool AllTrue(bool[] arr)
        {
            return Array.TrueForAll(arr, delegate(bool x) { return x; });
        }

        private float[,] MakeSMatr(float[,] M, float[] U, float[] V)
        {
            var ASize = mC.GetLength(0);
            var BSize = mC.GetLength(1);
            var HM = new float[ASize, BSize];
            for (var i = 0; i < ASize; i++)
            for (var j = 0; j < BSize; j++)
            {
                HM[i, j] = M[i, j];
                if (HM[i, j] != HM[i, j])
                    HM[i, j] = mC[i, j] - (U[i] + V[j]);
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

        private void Roll(float[,] m, float[,] sm)
        {
            var ASize = mC.GetLength(0);
            var BSize = mC.GetLength(1);
            var minInd = new Point();
            var min = float.MaxValue;
            var k = 0;
            Allowed = new Point[ASize + BSize];
            for (var i = 0; i < ASize; i++)
            for (var j = 0; j < BSize; j++)
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
            var Cycles = new float[Cycle.Length];
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

        private void NullToZero()
        {
            for (var i = 0; i < SupArr.GetLength(0); i++)
            for (var j = 0; j < SupArr.GetLength(1); j++)
                if (float.IsNaN(SupArr[i, j]))
                    SupArr[i, j] = 0;
        }

        #endregion
    }
}