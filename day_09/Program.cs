using System;

namespace Advent2021
{
    class  Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 09");

            var smoke = new SmokeSimulation("smoke.csv");
            smoke.PrintLowPoints();

            Console.WriteLine("End Day 09");
        }
    }

    class SmokeSimulation
    {
        List<List<int>> HeightMap;

        public SmokeSimulation(string heightMapConfigPath)
        {
            this.HeightMap = this.GenerateHeightMapFromConfig(heightMapConfigPath);
        }

        public void PrintLowPoints()
        {
            Console.WriteLine("X\tY\t\tLow Point Height");
            var lowPoints = this.FindLowPoints();
            var lowPointSum = 0;
            foreach (var point in lowPoints)
            {
                var lowPointValue = this.HeightMap[point.X][point.Y];
                Console.WriteLine($"{point.X}\t{point.Y}\t\t{lowPointValue}");
                lowPointSum += lowPointValue + 1;
            }
            Console.WriteLine();
            Console.WriteLine($"Low point sum\t\t{lowPointSum}");
        }

        private List<Coordinate> FindLowPoints()
        {
            var lowPoints = new List<Coordinate>();
            for (int i = 0; i < this.HeightMap.Count(); i++)
            {
                for (int j = 0; j < this.HeightMap[0].Count(); j++)
                {
                    var currentCoordinate = new Coordinate(i, j);
                    if (this.IsLowPoint(this.HeightMap, currentCoordinate))
                    {
                        lowPoints.Add(currentCoordinate);
                    }
                }
            }
            return lowPoints;
        }

        private bool IsInsideBounds(List<List<int>> heightMap, int x, int y)
        {
            return x >= 0 && x < HeightMap.Count() && y >= 0 && y < HeightMap[0].Count();
        }

        private bool IsLowPoint(List<List<int>> heightMap, Coordinate point)
        {
            // check neighbors to see if they are lower
            if (IsInsideBounds(heightMap, point.X - 1, point.Y) && heightMap[point.X - 1][point.Y] <= heightMap[point.X][point.Y])
                return false;
            if (IsInsideBounds(heightMap, point.X + 1, point.Y) && heightMap[point.X + 1][point.Y] <= heightMap[point.X][point.Y])
                return false;
            if (IsInsideBounds(heightMap, point.X, point.Y - 1) && heightMap[point.X][point.Y - 1] <= heightMap[point.X][point.Y])
                return false;
            if (IsInsideBounds(heightMap, point.X, point.Y + 1) && heightMap[point.X][point.Y + 1] <= heightMap[point.X][point.Y])
                return false;

            return true;
        }

        private List<List<int>> GenerateHeightMapFromConfig(string heightMapConfigPath)
        {
            return File.ReadLines(heightMapConfigPath)
                .Select(line => {
                    return line.Trim()
                        .ToArray()
                        .Select((char chr) => (int)(chr - '0'))
                        .ToList();
                })
                .ToList();
        }
    }

    class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
