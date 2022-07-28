using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TransportAlgorithms.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestTransportPotentials()
        {
            var transport = new TransportAlgorithms.TransportProblem();
            var matr = new double[3, 3] { { 3,5,2 },
            { 8,8,12 },
            { 4,6,10 }
            };
            var zapas = new int[3] { 100,230,140 };
            var potbr = new int[3] { 200,150,120 };
            var result = transport.FindSolution(matr, zapas, potbr, TypeAlgorithm.Potentials);
            var solut = new double[3, 3] { { 0,0,100},
                { 60, 150, 20},
                { 140, 0, 0} };
            Assert.AreEqual(solut, result);
        }
    }
}
