using System;
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

            Console.WriteLine();
            var oxySupply = $"Oxy Supply:{delimiter}{this.report.OxygenSupplyRating}";
            Console.WriteLine(oxySupply);

            var coScrubber = $"CO2 Scrub:{delimiter}{this.report.CO2ScrubberRating}";
            Console.WriteLine(coScrubber);

            var lifeSupport = $"Life Supp:{delimiter}{this.report.LifeSupportRating}";
            Console.WriteLine(lifeSupport);
        }
    }

    class DiagnosticReport
    {
        private List<string> binaryNumbers;
        private int instructionLength;

        public string GammaNumber { get; }
        public string EpsilonNumber { get; }
        public int PowerConsumption { get; }

        public string OxygenSupplyRating { get; }
        public string CO2ScrubberRating { get; }
        public int LifeSupportRating { get; }

        public DiagnosticReport(string reportPath, int instructionLength)
        {
            this.binaryNumbers = Utils.LoadLineSeparatedLiteralsFromFile<string>(reportPath);
            this.instructionLength = instructionLength;

            this.GammaNumber = this.GenerateGammaNumber();
            this.EpsilonNumber = this.GenerateEpsilonNumber(this.GammaNumber);
            this.PowerConsumption = this.GeneratePowerConsumption(this.GammaNumber, this.EpsilonNumber);

            this.OxygenSupplyRating = this.GenerateOxygenSupplyRating();
            this.CO2ScrubberRating = this.GenerateCO2ScrubberRating();
            this.LifeSupportRating = this.GenerateLifeSupportRating(this.OxygenSupplyRating, this.CO2ScrubberRating);
        }

        private int GenerateLifeSupportRating(string oxyRating, string coRating)
        {
            return Convert.ToInt32(oxyRating, 2) * Convert.ToInt32(coRating, 2);
        }

        private string GenerateOxygenSupplyRating()
        {
            var nums = new List<string>(this.binaryNumbers);

            for (int i = 0; i < this.instructionLength; i++)
            {
                var majorityChar = this.GetMajorityCharInPosition(nums, i);
                nums = nums.Where(x => this.IsBitInPosition(x, majorityChar, i)).ToList();

                if (nums.Count() == 1) break;
            }

            if (nums.Count() != 1) {
                throw new Exception("Oxygen Supply Rating did not filter properly...");
            }

            return nums[0];
        }

        private string GenerateCO2ScrubberRating()
        {
            var nums = new List<string>(this.binaryNumbers);

            for (int i = 0; i < this.instructionLength; i++)
            {
                var minorityChar = this.GetMinorityCharInPosition(nums, i);
                nums = nums.Where(x => this.IsBitInPosition(x, minorityChar, i)).ToList();

                if (nums.Count() == 1) break;
            }

            if (nums.Count() != 1) {
                throw new Exception("CO2 Scrubber Rating did not filter properly...");
            }

            return nums[0];
        }

        private char GetMinorityCharInPosition(List<string> nums, int position)
        {
            var majorityChar = this.GetMajorityCharInPosition(nums, position);
            return majorityChar == '1' ? '0' : '1';
        }

        private char GetMajorityCharInPosition(List<string> nums, int position)
        {
            var trueBitsInPositionI = this.CountTrueBitsInPosition(nums, position);
            int falseBits = nums.Count() - trueBitsInPositionI;

            return trueBitsInPositionI >= falseBits ? '1' : '0';
        }

        private string GenerateGammaNumber()
        {
            var gammaBuilder = new StringBuilder("", this.instructionLength);

            for (int i = 0; i < this.instructionLength; i++)
            {
                var gammaBit = this.GetMajorityCharInPosition(this.binaryNumbers, i);
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

        private int CountTrueBitsInPosition(List<string> nums, int position)
        {
            int count = nums.Aggregate(0, (int sum, string number) =>
                sum + (this.IsTrueBitInPosition(number, position) ? 1 : 0)
            );
            return count;
        }

        private bool IsTrueBitInPosition(string binaryNumber, int position)
        {
            return this.IsBitInPosition(binaryNumber, '1', position);
        }

        private bool IsBitInPosition(string binaryNumber, char bit, int position)
        {
            return binaryNumber[position] == bit;
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
