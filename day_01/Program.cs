using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace day_01
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath = "./input.csv";
            var sub = new Submarine(inputPath);
            Console.WriteLine(sub.CountIncreasingDepth());
        }


    }

    class Submarine
    {
        private string sonarFilePath;

        public Submarine(string sonarFilePath)
        {
            this.sonarFilePath = sonarFilePath;
        }

        public List<int> ReadSonar(string path) {
            var readings = File.ReadLines(path)
                .Select(line => int.Parse(line))
                .ToList();

            return readings;
        }

        public int CountIncreasingDepth() {
            var sonarReadings = this.ReadSonar(this.sonarFilePath);

            var currentDepth = sonarReadings.First();
            sonarReadings.RemoveAt(0);

            int increasingDepthCount = 0;
            foreach (int nextDepth in sonarReadings) {
                if (nextDepth > currentDepth) {
                    increasingDepthCount += 1;
                }

                currentDepth = nextDepth;
            }

            return increasingDepthCount;
        }
    }
}
