using System;
using System.Linq;

namespace Advent2021
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 04");

            var subOpts = new SubmarineOpts();
            var sub = new Submarine(subOpts);
            sub.DisplayBingoScore();
        }
    }

    class Submarine
    {
        BingoSystem bingoSystem;

        public Submarine(SubmarineOpts opts)
        {
            this.bingoSystem = new BingoSystem(opts);
        }

        public void DisplayBingoScore()
        {
            this.bingoSystem.printScores();
        }
    }

    class SubmarineOpts : BingoSystemOpts {}
}
