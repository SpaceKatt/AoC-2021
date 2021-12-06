using System;

namespace AoC2021
{
    using HydroVentMatrix = List<List<int>>;

    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 04");
            string hydroVentsConfigPath = "./hydro_vents.txt";
            var hydroVentMap = new HydroVentMap(hydroVentsConfigPath);

            Console.WriteLine($"Number of danger-ey bois: {hydroVentMap.CountDangerPoints()}");
        }
    }

    class HydroVentMap
    {
        HydroVentMatrix ventMatrix;

        public HydroVentMap(string ventConfigPath)
        {
            this.ventMatrix = this.GenerateVentMatrixFromFile(ventConfigPath);
        }

        public int CountDangerPoints()
        {
            var count = 0;
            foreach (var row in this.ventMatrix)
            {
                foreach(var value in row)
                {
                    if (value > 1)
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private HydroVentMatrix GenerateVentMatrixFromFile(string ventConfigPath)
        {
            var ventConfig = new VentMatrixConfig(ventConfigPath);
            var matrix = Utils.GenerateIntMatrix(ventConfig.MaxCoordinateValue, ventConfig.MaxCoordinateValue, 0);

            foreach (var line in ventConfig.Lines)
            {
                var coords = line.GetCoordinatesOnLine();
                foreach (var coord in coords)
                {
                    matrix[coord.X][coord.Y]++;
                }
            }

            return matrix;
        }
    }

    class VentMatrixConfig
    {
        public List<Line> Lines { get; }
        public int MaxCoordinateValue { get; private set; }

        public VentMatrixConfig(string ventConfigPath)
        {
            this.MaxCoordinateValue = -1;
            this.Lines = File.ReadLines(ventConfigPath)
              .Select(line => {
                  var points = line.Split(" -> ").ToList();
                  var start = this.GenerateCoordFromStringPoint(points[0]);
                  var end = this.GenerateCoordFromStringPoint(points[1]);
                  return new Line(start, end);
              })
              .Where(line => line.IsHorizontal() || line.IsVertical())
              .ToList();
            this.MaxCoordinateValue += 1;
        }

        private Coordinate GenerateCoordFromStringPoint(string point)
        {
            var coords = point.Split(',').ToList();
            var first = int.Parse(coords[0]);
            var second = int.Parse(coords[1]);

            if (first > this.MaxCoordinateValue)
            {
                this.MaxCoordinateValue = first;
            }

            if (second > this.MaxCoordinateValue)
            {
                this.MaxCoordinateValue = second ;
            }

            return new Coordinate(first, second);
        }
    }

    class Line
    {
        public Coordinate Start { get; set; }
        public Coordinate End { get; set; }

        public Line()
        {
            this.Start = new Coordinate(0, 0);
            this.End = new Coordinate(0, 0);
        }

        public Line(Coordinate Start, Coordinate End)
        {
            this.Start = Start;
            this.End = End;
        }

        public bool IsVertical()
        {
            return this.Start.Y == this.End.Y;
        }

        public bool IsHorizontal()
        {
            return this.Start.X == this.End.X;
        }

        public List<Coordinate> GetCoordinatesOnLine()
        {
            if (!this.IsHorizontal() && !this.IsVertical())
            {
                throw new Exception("Cannot get coordinates for diagonal lines");
            }

            var coords = new List<Coordinate>();

            if (this.IsHorizontal() && this.IsVertical())
            {
                // single point
                coords.Add(this.Start);
            }
            else if (this.IsHorizontal())
            {
                var constX = this.Start.X;
                var largerY = this.Start.Y > this.End.Y ? this.Start.Y : this.End.Y;
                var smallerY = this.Start.Y < this.End.Y ? this.Start.Y : this.End.Y;
                for (int y = smallerY; y <= largerY; y++)
                {
                    coords.Add(new Coordinate(constX, y));
                }
            }
            else if (this.IsVertical())
            {
                var constY = this.Start.Y;
                var largerX = this.Start.X > this.End.X ? this.Start.X : this.End.X;
                var smallerX = this.Start.X < this.End.X ? this.Start.X : this.End.X;
                for (int x = smallerX; x <= largerX; x++)
                {
                    coords.Add(new Coordinate(x, constY));
                }
            }
            else
            {
                throw new Exception("This should never happen (Line)");
            }

            return coords;
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
        public static List<List<int>> GenerateIntMatrix(int width, int length, int initValue)
        {
            var matrix = new List<List<int>>();
            var matrixRow = new List<int>();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    matrixRow.Add(initValue);
                }
                matrix.Add(new List<int>(matrixRow));
                matrixRow.Clear();
            }

            return matrix;
        }
    }
}