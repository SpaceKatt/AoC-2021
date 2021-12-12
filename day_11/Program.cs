using System;

namespace Advent2021
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 11");
            Console.WriteLine();

            var dumbOctoConfig = File.ReadAllText("./dumbo-octo.txt");
            var dumbOctopusSim = new DumboOctopusSimulation(dumbOctoConfig);

            dumbOctopusSim.PrintState();
            Console.WriteLine();

            dumbOctopusSim.AdvanceSteps(100);
            dumbOctopusSim.PrintState();
            Console.WriteLine();

            dumbOctopusSim.PrintFlashCount();
            Console.WriteLine();

            Console.WriteLine("End Day 11");
        }
    }

    class DumboOctopusSimulation
    {
        List<List<int>> DumboState;
        public int FlashCount { get; private set; }

        private static int FlashThreshold = 9;

        public DumboOctopusSimulation(string dumboOctopusConfig)
        {
            this.DumboState = this.GenerateInitialState(dumboOctopusConfig);
            this.FlashCount = 0;
        }

        public void PrintFlashCount()
        {
            Console.WriteLine($"Flashes:\t\t{this.FlashCount}");
        }

        public void PrintState()
        {
            foreach (var line in this.DumboState)
            {
                foreach (var state in line)
                {
                    Console.Write(state);
                }
                Console.WriteLine();
            }
        }

        public void AdvanceSteps(int stepCount)
        {
            for (int i = 0; i < stepCount; i++)
            {
                this.AdvanceStep();
            }
        }

        private void AdvanceStep()
        {
            var patch = Utils.GenerateMatrixWithDefault<int>(this.DumboState.Count, this.DumboState[0].Count, 1);
            var flashed = Utils.GenerateMatrixWithDefault<bool>(this.DumboState.Count, this.DumboState[0].Count, false);
            this.ApplyPatch(patch, flashed);
            for (int i = 0; i < this.DumboState.Count; i++)
            {
                for (int j = 0; j < this.DumboState[0].Count; j++)
                {
                    if (this.DumboState[i][j] > DumboOctopusSimulation.FlashThreshold)
                    {
                        this.DumboState[i][j] = 0;
                    }
                }
            }
        }

        private void ApplyPatch(List<List<int>> patch, List<List<bool>> flashed)
        {
            var needsPatch = false;
            var nextPatch = Utils.GenerateMatrixWithDefault<int>(this.DumboState.Count, this.DumboState[0].Count, 0);
            for (int i = 0; i < this.DumboState.Count; i++)
            {
                for (int j = 0; j < this.DumboState[0].Count; j++)
                {
                    if (flashed[i][j]) continue;
                    this.DumboState[i][j] += patch[i][j];
                    if (this.DumboState[i][j] > DumboOctopusSimulation.FlashThreshold)
                    {
                        needsPatch = true;
                        flashed[i][j] = true;
                        this.IncrementNeighbors(nextPatch, i, j);
                        this.FlashCount += 1;
                    }
                }
            }
            if (needsPatch)
            {
                this.ApplyPatch(nextPatch, flashed);
            }

        }

        private bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < this.DumboState.Count && y >= 0 && y < this.DumboState[0].Count;
        }

        private void IncrementNeighbors(List<List<int>> patch, int x, int y)
        {
            if (this.IsWithinBounds(x - 1, y)) patch[x - 1][y]++;
            if (this.IsWithinBounds(x - 1, y + 1)) patch[x - 1][y + 1]++;
            if (this.IsWithinBounds(x - 1, y - 1)) patch[x - 1][y - 1]++;

            if (this.IsWithinBounds(x + 1, y)) patch[x + 1][y]++;
            if (this.IsWithinBounds(x + 1, y + 1)) patch[x + 1][y + 1]++;
            if (this.IsWithinBounds(x + 1, y - 1)) patch[x + 1][y - 1]++;

            if (this.IsWithinBounds(x, y + 1)) patch[x][y + 1]++;
            if (this.IsWithinBounds(x, y - 1)) patch[x][y - 1]++;
        }

        private List<List<int>> GenerateInitialState(string dumboOctopusConfig)
        {
            var state = new List<List<int>>();
            foreach (var line in dumboOctopusConfig.Split('\n'))
            {
                var stateRow = new List<int>();
                foreach (var energyChar in line.ToCharArray())
                {
                    int energyState = energyChar - '0';
                    stateRow.Add(energyState);
                }
                state.Add(stateRow);
            }
            return state;
        }
    }

    class Utils
    {

        public static List<List<T>> GenerateMatrixWithDefault<T>(int rows, int columns, T defaultValue)
        {
            var matrix = new List<List<T>>();

            var matrixRow = new List<T>();
            for (int j = 0; j < columns; j++)
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
