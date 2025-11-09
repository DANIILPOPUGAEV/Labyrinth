using 
using System.Drawing;

namespace LabyrinthsTests;

public class GeneratorTests
{
    [Fact]
    public void DfsGenerator_CreatesMazeWithWallsAndPaths()
    {
        // Arrange
        var generator = new DfsGenerator();

        // Act
        var maze = generator.Generate(11, 11);
            
        // Assert
        Assert.NotNull(maze);
        Assert.Equal(11, maze.Width);
        Assert.Equal(11, maze.Height);

        int wallCount = 0;
        int pathCount = 0;

        for (int y = 0; y < maze.Height; y++)
        {
            for (int x = 0; x < maze.Width; x++)
            {
                var cell = maze.Get(new Point(x, y));
                if (cell == Enums.CellType.Wall) wallCount++;
                if (cell == Enums.CellType.Empty) pathCount++;
            }
        }

        Assert.True(wallCount > 0, "Должны быть стены");
        Assert.True(pathCount > 0, "Должны быть проходы");
    }
}