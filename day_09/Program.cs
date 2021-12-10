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
            smoke.PrintBasinMap();
            smoke.PrintBasinStats();

            Console.WriteLine("End Day 09");
        }
    }

    class SmokeSimulation
    {
        List<List<int>> HeightMap;
        List<List<int>> BasinMap;

        private static int NO_BASIN = 0;

        public SmokeSimulation(string heightMapConfigPath)
        {
            this.HeightMap = this.GenerateHeightMapFromConfig(heightMapConfigPath);
            this.BasinMap = this.GenerateBasinMap(this.HeightMap);
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

        public void PrintBasinMap()
        {
            Console.WriteLine("Basinmap");
            for (int i = 0; i < this.BasinMap.Count(); i++)
            {
                for (int j = 0; j < this.BasinMap[0].Count(); j++)
                {
                    Console.Write($"{this.BasinMap[i][j]:000}");
                }
                Console.WriteLine();
            }
        }

        public void PrintBasinStats()
        {
            var basinCounter = new Dictionary<int, int>();

            foreach (var row in this.BasinMap)
            {
                foreach (var basinId in row)
                {
                    if (basinId == SmokeSimulation.NO_BASIN) continue;
                    if (!basinCounter.ContainsKey(basinId)) basinCounter.Add(basinId, 0);
                    basinCounter[basinId]++;
                }
            }

            var sortedBasinSize = basinCounter.Values.ToList<int>();
            sortedBasinSize.Sort();

            Console.WriteLine("Basin sizes:");
            foreach (var size in sortedBasinSize)
            {
                Console.WriteLine($"{size}");
            }
        }

        private List<List<int>> GenerateBasinMap(List<List<int>> heightMap)
        {
            var basinMap = Utils.InitializeMatrix<int>(heightMap.Count(), heightMap[0].Count(), 0);
            var marked = Utils.InitializeMatrix<bool>(heightMap.Count(), heightMap[0].Count(), false);
            if (heightMap.Count() != marked.Count() || heightMap[0].Count() != heightMap[0].Count())
                throw new Exception("Marked space should have same dimensions as height space");

            int basinCount = 0;

            for (int i = 0; i < heightMap.Count(); i++)
            {
                for (int j = 0; j < heightMap.Count(); j++)
                {
                    if (marked[i][j])
                    {
                        continue;
                    }
                    else if (heightMap[i][j] == 9)
                    {
                        marked[i][j] = true;
                        basinMap[i][j] = SmokeSimulation.NO_BASIN;
                    }
                    else
                    {
                        basinCount += 1;
                        this.FloodFillBasin(basinMap, heightMap, marked, basinCount, new Coordinate(i, j));
                    }
                }
            }

            return basinMap;
        }

        private void FloodFillBasin(List<List<int>> basinMap, List<List<int>> heightMap, List<List<bool>> marked, int basinId, Coordinate fillPoint)
        {
            if (fillPoint.X < 0 || fillPoint.X >= heightMap.Count() || fillPoint.Y < 0 || fillPoint.Y >= heightMap[0].Count())
                return;
            if (marked[fillPoint.X][fillPoint.Y])
                return;

            marked[fillPoint.X][fillPoint.Y] = true;

            if (heightMap[fillPoint.X][fillPoint.Y] == 9)
            {
                basinMap[fillPoint.X][fillPoint.Y] = SmokeSimulation.NO_BASIN;
            }
            else
            {
                basinMap[fillPoint.X][fillPoint.Y] = basinId;
                this.FloodFillBasin(basinMap, heightMap, marked, basinId, new Coordinate(fillPoint.X + 1, fillPoint.Y));
                this.FloodFillBasin(basinMap, heightMap, marked, basinId, new Coordinate(fillPoint.X - 1, fillPoint.Y));
                this.FloodFillBasin(basinMap, heightMap, marked, basinId, new Coordinate(fillPoint.X, fillPoint.Y + 1));
                this.FloodFillBasin(basinMap, heightMap, marked, basinId, new Coordinate(fillPoint.X, fillPoint.Y - 1));
            }
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

    class Utils
    {
        public static List<List<T>> InitializeMatrix<T>(int rows, int columns, T defaultValue)
        {
            var matrix = new List<List<T>>();

            var matrixRow = new List<T>();
            for (int i = 0; i < columns; i++)
            {
                matrixRow.Add(defaultValue);
            }

            for (int i = 0; i < rows; i++)
            {
                matrix.Add(new List<T>(matrixRow));
            }
            return matrix;
        }
    }
}
