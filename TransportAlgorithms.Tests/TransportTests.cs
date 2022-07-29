using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TransportAlgorithms.Tests
{
    [TestClass]
    public class TransportTests
    {
        [TestMethod]
        public void TestTransportPotentials()
        {
            var transport = new TransportAlgorithms.TransportProblem();
            var matr = new double[3, 5] {
            { 20, 23, 20, 15, 24 },
            { 29, 15, 16, 19, 29 },
            { 6, 11, 10, 9, 8 },
            };
            var zapas = new int[3] { 320, 280, 250 };
            var potbr = new int[5] { 150, 140, 110, 230, 220 };
            var result = transport.FindSolution(matr, zapas, potbr, TypeAlgorithm.Potentials);
            var solut = new double[3, 5] { { 120, 0, 0, 200, 0},
                { 0, 140, 110, 30, 0 },
                { 30, 0, 0, 0, 220} };
            Assert.AreEqual(result.GetLength(0), solut.GetLength(0), "Не равны размеры массивов!");
            Assert.AreEqual(result.GetLength(1), solut.GetLength(1), "Не равны размеры массивов!");
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Assert.AreEqual(result[i,j], solut[i,j], 0.00000001);
                }
            }
        }
    }
}
