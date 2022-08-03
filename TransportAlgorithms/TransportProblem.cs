using System;
using System.Linq;
using TransportAlgorithms.Algorithms;

namespace TransportAlgorithms
{
    /// <summary>
    ///     Доступные методы поиска решений транспортной проблемы
    /// </summary>
    public enum TypeAlgorithm
    {
        NorthWest,
        Potentials
    }

    /// <summary>
    ///     Класс, представляющий возможность получить необходимое решение транспортной задачи
    /// </summary>
    public class TransportProblem
    {
        #region Methods

        public double[,] FindSolution(double[,] Matrix, int[] Suppliers, int[] Shops, TypeAlgorithm ta)
        {
            var suppliers = Suppliers.ToArray();
            var shops = new int[Shops.Length];
            shops = Shops.ToArray();
            var Solution = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            switch (ta)
            {
                case TypeAlgorithm.NorthWest:
                    NorthWest = new NorthWest(Matrix, suppliers, shops);
                    Solution = NorthWest.ReturnSolution();
                    MathematicalPrice = NorthWest.GetMathematicalModel();
                    break;
                case TypeAlgorithm.Potentials:
                    NorthWest = new NorthWest(Matrix, suppliers, shops);
                    for (var i = 0; i < Matrix.GetLength(0); i++)
                    for (var j = 0; j < Matrix.GetLength(1); j++)
                        Matrix[i, j] = (int)Math.Round(Matrix[i, j]);
                    Potentials = new Potentials(Matrix, Suppliers, Shops, NorthWest.ReturnSolution());
                    Solution = Potentials.ReturnSolution();
                    MathematicalPrice = Potentials.GetMathematicalModel();
                    break;
            }

            return Solution;
        }

        #endregion

        #region Properties

        public double MathematicalPrice { get; private set; }
        public NorthWest NorthWest { get; private set; }
        public Potentials Potentials { get; private set; }

        #endregion
    }
}