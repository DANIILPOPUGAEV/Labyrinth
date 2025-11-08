using Labyrinths.Models;

namespace Labyrinths.Generators;
interface IGenerator
{
    Maze Generate(int width, int height);
}