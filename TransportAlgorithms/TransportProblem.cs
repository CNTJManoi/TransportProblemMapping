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
                    break;
                case TypeAlgorithm.Potentials:
                    break;
                default:
                    break;
            }
            return Solution;
        }
    }
}
