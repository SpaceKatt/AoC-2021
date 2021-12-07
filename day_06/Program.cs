﻿using System;

namespace Advent2021
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 06");

            var simulation = new FishSimulator("./lanternfish.csv");
            simulation.PrintSimulationState();

            simulation.SimulateDays(80);
            simulation.PrintSimulationState();

            // simulation.SimulateDays(1);
            // simulation.PrintSimulationState();
        }
    }

    class FishSimulator
    {
        private int MaturityCost = 2;
        private int ReproductiveCycleLength = 7;

        private List<int> FishAtAge;

        public FishSimulator(string simulationConfigPath)
        {
            this.FishAtAge = this.GenerateFishAtAgeFromConfig(simulationConfigPath);
        }

        public void PrintSimulationState()
        {
            Console.WriteLine("--- Simulation State");
            Console.WriteLine("Age\t\tPopulation Count");
            for (int i = 0; i < this.FishAtAge.Count(); i++)
            {
                Console.WriteLine($"{i}\t\t{this.FishAtAge[i]}");
            }
            Console.WriteLine();
            this.PrintTotalPopulation();
            Console.WriteLine();
        }

        public void PrintTotalPopulation()
        {
            Console.WriteLine($"Total:\t\t{this.GetTotalPopulation()}");
        }

        public int GetTotalPopulation()
        {
            var popSum = 0;
            foreach (int count in this.FishAtAge)
            {
                popSum += count;
            }
            return popSum;
        }

        public void SimulateDays(int dayCount)
        {
            for (int i = 0; i < dayCount; i++)
            {
                this.SimulateDay();
            }
        }

        private void SimulateDay()
        {
            var fishBirthingToday = this.FishAtAge[0];
            var newFishAtAge = new List<int>();

            for (int i = 1; i < this.FishAtAge.Count(); i++)
            {
                newFishAtAge.Add(this.FishAtAge[i]);
            }

            newFishAtAge[this.ReproductiveCycleLength - 1] += fishBirthingToday;
            newFishAtAge.Add(fishBirthingToday);

            this.FishAtAge = newFishAtAge;
        }

        private List<int> GenerateFishAtAgeFromConfig(string simulationConfigPath)
        {
            var individualFish = File.ReadLines(simulationConfigPath)
                .First()
                .Split(',')
                .Select(line => int.Parse(line))
                .ToList();

            var fishAtAge = new List<int>(new int[this.ReproductiveCycleLength + this.MaturityCost]);

            foreach (var fish in individualFish)
            {
                fishAtAge[fish]++;
            }

            return fishAtAge;
        }
    }
}
