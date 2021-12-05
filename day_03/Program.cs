﻿using System;
using System.Text;

namespace Advent2021
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Day 03");

            var opts = new SubmarineOptions();
            var sub = new Submarine(opts);

            sub.ReadDiagnosticReport();
        }
    }

    class Submarine
    {
        private DiagnosticReport report;

        public Submarine(SubmarineOptions opts)
        {
            this.report = new DiagnosticReport(opts.reportFilePath, opts.diagnosticNumberLength);
        }

        public void ReadDiagnosticReport()
        {
            var delimiter = "\t";

            var gamma = $"Gamma:{delimiter}{delimiter}{this.report.GammaNumber}";
            Console.WriteLine(gamma);
            var epsilon = $"Epsilon:{delimiter}{this.report.EpsilonNumber}";
            Console.WriteLine(epsilon);
            var powerCons = $"Power Cons:{delimiter}{this.report.PowerConsumption}";
            Console.WriteLine(powerCons);
        }
    }

    class DiagnosticReport
    {
        private List<string> binaryNumbers;
        private int instructionLength;
        public string GammaNumber { get; }
        public string EpsilonNumber { get; }
        public int PowerConsumption { get; }

        public DiagnosticReport(string reportPath, int instructionLength)
        {
            this.binaryNumbers = Utils.LoadLineSeparatedLiteralsFromFile<string>(reportPath);
            this.instructionLength = instructionLength;

            this.GammaNumber = this.GenerateGammaNumber();
            this.EpsilonNumber = this.GenerateEpsilonNumber(this.GammaNumber);
            this.PowerConsumption = this.GeneratePowerConsumption(this.GammaNumber, this.EpsilonNumber);
        }

        private string GenerateGammaNumber()
        {
            var gammaBuilder = new StringBuilder("", this.instructionLength);

            for (int i = 0; i < this.instructionLength; i++)
            {
                var trueBitsInPositionI = this.CountTrueBitsInPosition(i);
                int falseBits = this.binaryNumbers.Count() - trueBitsInPositionI;

                var gammaBit = trueBitsInPositionI > falseBits ? "1" : "0";
                gammaBuilder.Append(gammaBit);
            }

            return gammaBuilder.ToString();
        }

        public string GenerateEpsilonNumber(string gammaNumber)
        {
            var epsilonBuilder = new StringBuilder("", this.instructionLength);

            for (int i = 0; i < this.instructionLength; i++)
            {
                var epsilonChar = gammaNumber[i] == '1' ? "0" : "1";
                epsilonBuilder.Append(epsilonChar);
            }

            return epsilonBuilder.ToString();
        }

        private int GeneratePowerConsumption(string gamma, string epsilon)
        {
            return Convert.ToInt32(gamma, 2) * Convert.ToInt32(epsilon, 2);
        }

        private int CountTrueBitsInPosition(int position)
        {
            int count = this.binaryNumbers.Aggregate(0, (int sum, string number) =>
                sum + (this.IsTrueBitInPosition(number, position) ? 1 : 0)
            );
            return count;
        }

        private bool IsTrueBitInPosition(string binaryNumber, int position)
        {
            return binaryNumber[position].ToString() == "1";
        }
    }

    class SubmarineOptions : DiagnosticReportOpts {}

    class DiagnosticReportOpts
    {
        public string reportFilePath { get; set; }
        public int diagnosticNumberLength { get; set; }

        public DiagnosticReportOpts()
        {
            this.reportFilePath = "diagnostic_report.csv";
            this.diagnosticNumberLength = 12;
        }
    }

    class Utils
    {
        public static List<T> LoadLineSeparatedLiteralsFromFile<T>(string filePath)
        {
            return File.ReadLines(filePath)
                .Select(line => {
                        return (T)Convert.ChangeType(line, typeof(T));
                })
                .ToList();
        }
    }
}
