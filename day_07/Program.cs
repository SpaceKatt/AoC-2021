using System;

namespace Advent2021
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 06");
            var crabOptimizeer = new CrabSubmarineOptimizer("crab_subs.csv");
            crabOptimizeer.PrintInfo();
        }
    }

    class CrabSubmarineOptimizer
    {
        List<int> CrabPositions;
        List<long> FuelCostAtPosition;

        public CrabSubmarineOptimizer(string crabConfigPath)
        {
            this.CrabPositions = this.GenerateCrabPositions(crabConfigPath);
            this.FuelCostAtPosition = this.GenerateFuelCostAtPosition(this.CrabPositions);
        }

        public void PrintInfo()
        {
            var minCost = this.GetMinimumCost();
            Console.WriteLine($"{minCost}");
        }

        public long GetMinimumCost()
        {
            var min = this.FuelCostAtPosition[0];
            foreach (var cost in this.FuelCostAtPosition)
            {
                if (cost < min)
                {
                    min = cost;
                }
            }
            return min;
        }

        private List<long> GenerateFuelCostAtPosition(List<int> crabPositions)
        {
            var maxPosition = -1;
            foreach (var position in this.CrabPositions)
            {
                if (position > maxPosition) maxPosition = position;
            }
            var numberOfPositions = maxPosition + 1;
            var fuelCostAtPosition = new List<long>(new long[numberOfPositions]);
            foreach (var position in crabPositions)
            {
                var crabFuelCost = this.GenerateSingleCrabFuelCost(position, numberOfPositions);
                for (int i = 0; i < numberOfPositions; i++)
                {
                    fuelCostAtPosition[i] += crabFuelCost[i];
                }
            }
            return fuelCostAtPosition;
        }

        private List<int> GenerateCrabPositions(string crabConfigPath)
        {
            return File.ReadLines(crabConfigPath)
                .First()
                .Split(',')
                .Select(line => int.Parse(line))
                .ToList();
        }

        private List<long> GenerateSingleCrabFuelCost(int crabPosition, int numberOfPositions)
        {
            var fuelCostAtPosition = new List<long>(new long[numberOfPositions]);
            for (int i = 0; i < numberOfPositions; i++)
            {
                var cost = this.GeometricSum(Math.Abs(crabPosition - i));
                fuelCostAtPosition[i] = cost;
            }
            return fuelCostAtPosition;
        }

        private long GeometricSum(int n)
        {
            long sum = 0;
            for (long i = 1; i <= n; i++)
            {
                sum += i;
            }
            return sum;
        }
    }
}