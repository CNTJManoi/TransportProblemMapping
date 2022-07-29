using System.Linq;
using TransportAlgorithms.Algorithms;

namespace TransportAlgorithms
{
    /// <summary>
    /// Доступные методы поиска решений транспортной проблемы
    /// </summary>
    public enum TypeAlgorithm
    {
        NorthWest,
        Potentials
    }
    /// <summary>
    /// Класс, представляющий возможность получить необходимое решение транспортной задачи
    /// </summary>
    public class TransportProblem
    {
        #region Properties
        public double MathematicalPrice { get; private set; }
        public NorthWest NorthWest { get; private set; }
        public Potentials Potentials { get; private set; }
        #endregion

        #region Methods

        public double[,] FindSolution(double[,] Matrix, int[] Suppliers, int[] Shops, TypeAlgorithm ta)
        {
            var suppliers = new int[Suppliers.Length];
            suppliers = suppliers.Union(Suppliers).Where(x => x != 0).ToArray();
            var shops = new int[Shops.Length];
            shops = shops.Union(Shops).Where(x => x != 0).ToArray();
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
                    Potentials = new Potentials(Matrix, Suppliers, Shops, NorthWest.ReturnSolution());
                    Solution = Potentials.ReturnSolution();
                    MathematicalPrice = Potentials.GetMathematicalModel();
                    break;
            }

            return Solution;
        }
        #endregion
    }
}