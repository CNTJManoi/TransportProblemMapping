
namespace TransportAlgorithms.Algorithms
{
    interface IAlgorithm
    {
        /// <summary>
        /// Произвести рассчет решения по выбранному алгоритму
        /// </summary>
        /// <returns>Опорный план</returns>
        double[,] ReturnSolution();
        /// <summary>
        /// Возвращает опорный план решенной ранее задачи. Если задача не была решена, то вовзращает null
        /// </summary>
        /// <returns>Опорный план</returns>
        double[,] GetMatrixSolution();
        /// <summary>
        /// Рассчитывает математическую модель решения
        /// </summary>
        /// <returns>Математическая модель цены</returns>
        double GetMathematicalModel();
        /// <summary>
        /// Очищает все полученные решения
        /// </summary>
        void Clear();
    }
}
