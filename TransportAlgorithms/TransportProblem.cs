using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TransportAlgorithms.Algorithms;

namespace TransportAlgorithms
{
    public enum TypeAlgorithm
    {
        NorthWest,
        Potentials
    }
    public class TransportProblem
    {
        public double MathematicalPrice { get; private set; }
        public NorthWest NorthWest { get; private set; }
        public Potentials Potentials { get; private set; }
        public TransportProblem()
        {

        }
        public double[,] FindSolution(double[,] Matrix, double[] Suppliers, double[] Shops, TypeAlgorithm ta)
        {
            double[,] Solution = new double[Matrix.GetLength(0), Matrix.GetLength(1)];
            switch (ta)
            {
                case TypeAlgorithm.NorthWest:
                    NorthWest = new NorthWest(Matrix, Suppliers, Shops);
                    Solution = NorthWest.ReturnSolution();
                    MathematicalPrice = NorthWest.GetMathematicalModel();
                    break;
                case TypeAlgorithm.Potentials:
                    NorthWest = new NorthWest(Matrix, Suppliers, Shops);
                    Potentials = new Potentials(Matrix, Suppliers, Shops, NorthWest.ReturnSolution());
                    Solution = Potentials.ReturnSolution();
                    MathematicalPrice = Potentials.GetMathematicalModel();
                    break;
                default:
                    break;
            }
            return Solution;
        }
    }
}
