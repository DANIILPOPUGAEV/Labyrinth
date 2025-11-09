using Labyrinths.Models;

namespace Labyrinths.Generators;
/// <summary>
/// Интерфейс для генераторов лабиринтов.
/// Определяет контракт для всех алгоритмов генерации лабиринтов в приложении.
/// </summary>
interface IGenerator
{
    /// <summary>
    /// Генерирует лабиринт указанных размеров.
    /// </summary>
    Maze Generate(int width, int height);
}