using System;

namespace Advent2021
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 08");

            var segmentDisplay = new SegmentDisplay("./seven-segment.csv");
            segmentDisplay.PrintSignalCount();

            Console.WriteLine("End Day 08");
        }
    }

    class SegmentDisplay
    {
        List<SegmentSignal> Signals;

        public SegmentDisplay(string signalConfigPath)
        {
            this.Signals = this.GenerateSegmentSignals(signalConfigPath);
        }

        public void PrintSignals()
        {
            foreach (var signal in this.Signals)
            {
                signal.PrintSignal();
            }
        }

        public void PrintSignalCount()
        {
            var counter = new int[10];
            foreach (var signal in this.Signals)
                foreach (var output in signal.OutputNumbers)
                    counter[output]++;

            Console.WriteLine("Output\t\tCount");
            for (int i = 0; i < 10; i++)
                Console.WriteLine($"{i}\t\t{counter[i]}");
        }

        private List<SegmentSignal> GenerateSegmentSignals(string signalConfigPath)
        {
            return File.ReadLines(signalConfigPath)
                .Select(line => {
                    var patternAndSignal = line.Split('|');
                    var uniquePatterns = patternAndSignal[0];
                    var outputCode = patternAndSignal[1];
                    return new SegmentSignal(uniquePatterns, outputCode);
                })
                .ToList();
        }
    }

    class SegmentSignal
    {
        public static int UniquePatternCount = 10;
        public static int OutputCodeCount = 4;

        public static HashSet<char> One = new HashSet<char> { 'c', 'f' };
        public static HashSet<char> Four = new HashSet<char> { 'b', 'c', 'd', 'f' };
        public static HashSet<char> Seven = new HashSet<char> { 'a', 'c', 'f' };
        public static HashSet<char> Eight = new HashSet<char> {'a', 'b', 'c', 'd', 'e', 'f', 'g' };

        public static HashSet<char> Zero = new HashSet<char> { 'a', 'b', 'c', 'e', 'f', 'g' };
        public static HashSet<char> Six = new HashSet<char> { 'a', 'b', 'd', 'e', 'f', 'g' };
        public static HashSet<char> Nine = new HashSet<char> { 'a', 'b', 'c', 'd', 'f', 'g' };

        public static HashSet<char> Two = new HashSet<char> { 'a', 'c', 'd', 'e', 'g' };
        public static HashSet<char> Three = new HashSet<char> { 'a', 'c', 'd', 'f', 'g' };
        public static HashSet<char> Five = new HashSet<char> { 'a', 'b', 'd', 'f', 'g' };

        List<string> UniquePatterns;
        List<string> OutputCodes;

        public List<int> OutputNumbers { get; private set; }

        Dictionary<char, char> SignalMapping;

        public SegmentSignal(string uniquePatternsString, string outputCodesString)
        {
            this.UniquePatterns = new List<string>(uniquePatternsString.Trim().Split(' '));
            if (this.UniquePatterns.Count() != SegmentSignal.UniquePatternCount)
            {
                throw new Exception($"SegmentSignal should have {SegmentSignal.UniquePatternCount} unique patterns");
            }

            this.OutputCodes = new List<string>(outputCodesString.Trim().Split(' '));
            if (this.OutputCodes.Count() != SegmentSignal.OutputCodeCount)
            {
                throw new Exception($"SegmentSignal should have {SegmentSignal.OutputCodeCount} output codes");
            }

            this.SignalMapping = this.GenerateSignalMapping(this.UniquePatterns);
            this.OutputNumbers = this.GenerateOutputSignal(this.OutputCodes, this.SignalMapping);
        }

        public void PrintSignal()
        {
            foreach (var output in this.OutputNumbers)
            {
                Console.Write($"{output} ");
            }
            Console.WriteLine();
        }

        private List<int> GenerateOutputSignal(List<string> outputCodes, Dictionary<char, char> signalMapping)
        {
            var output = new List<int>();

            foreach (var code in outputCodes)
            {
                var encodedSegment = code.ToArray();
                var segments = new HashSet<char>();
                foreach (var codedChar in encodedSegment)
                {
                    segments.Add(signalMapping[codedChar]);
                }
                output.Add(this.GetOutputFromSegments(segments));
            }

            return output;
        }

        private int GetOutputFromSegments(HashSet<char> segments)
        {
            if (segments.SetEquals(SegmentSignal.Zero)) return 0;
            if (segments.SetEquals(SegmentSignal.One)) return 1;
            if (segments.SetEquals(SegmentSignal.Two)) return 2;
            if (segments.SetEquals(SegmentSignal.Three)) return 3;
            if (segments.SetEquals(SegmentSignal.Four)) return 4;
            if (segments.SetEquals(SegmentSignal.Five)) return 5;
            if (segments.SetEquals(SegmentSignal.Six)) return 6;
            if (segments.SetEquals(SegmentSignal.Seven)) return 7;
            if (segments.SetEquals(SegmentSignal.Eight)) return 8;
            if (segments.SetEquals(SegmentSignal.Nine)) return 9;

            throw new Exception("GetOutputFromSegments: Segment definition not found.");
        }

        private Dictionary<char, char> GenerateSignalMapping(List<string> uniquePatterns)
        {
            var signalMapping = new Dictionary<char, char>();

            HashSet<char> oneSet = new HashSet<char>();
            HashSet<char> fourSet = new HashSet<char>();
            HashSet<char> sevenSet = new HashSet<char>();
            HashSet<char> eightSet = new HashSet<char>();

            List<HashSet<char>> zeroSixNine = new List<HashSet<char>>();
            List<HashSet<char>> twoThreeFive = new List<HashSet<char>>();

            int[] charCount = new int[7];

            foreach (var pattern in uniquePatterns)
            {
                char[] chars = pattern.ToArray();
                foreach (var chr in chars) { charCount[chr - 'a']++; }
                var currentPattern = new HashSet<char>(chars);

                switch (currentPattern.Count())
                {
                    case 2:
                        oneSet = currentPattern;
                        break;
                    case 4:
                        fourSet = currentPattern;
                        break;
                    case 3:
                        sevenSet = currentPattern;
                        break;
                    case 7:
                        eightSet = currentPattern;
                        break;
                    case 5:
                        twoThreeFive.Add(currentPattern);
                        break;
                    case 6:
                        zeroSixNine.Add(currentPattern);
                        break;
                    default:
                        throw new Exception("GenerateSignalMapping should never get here");
                }
            }

            // 'f' is the most common segment
            var fInx = -1;
            var maxCount = -1;
            for (int i = 0; i < 7; i++)
            {
                if (charCount[i] > maxCount)
                {
                    fInx = i;
                    maxCount = charCount[i];
                }
            }
            var fSet = new HashSet<char>();
            fSet.Add((char)('a' + fInx));
            char f = fSet.First();
            signalMapping.Add(f, 'f');

            var aSet = new HashSet<char>(sevenSet);
            aSet.ExceptWith(oneSet);
            if (aSet.Count() != 1) throw new Exception("Logic error getting 'a'");
            char a = aSet.First();
            signalMapping.Add(a, 'a');

            var gSet = new HashSet<char>(zeroSixNine.First());
            foreach (var set in zeroSixNine) gSet.IntersectWith(set);
            foreach (var set in twoThreeFive) gSet.IntersectWith(set);
            gSet.ExceptWith(aSet);
            if (gSet.Count() != 1) throw new Exception("Logic error getting 'g'");
            char g = gSet.First();
            signalMapping.Add(g, 'g');

            var bSet = new HashSet<char>(zeroSixNine.First());
            foreach (var set in zeroSixNine) bSet.IntersectWith(set);
            bSet.ExceptWith(aSet);
            bSet.ExceptWith(gSet);
            bSet.ExceptWith(fSet);
            if (bSet.Count() != 1) throw new Exception("Logic error getting 'b'");
            char b = bSet.First();
            signalMapping.Add(b, 'b');

            var dSet = new HashSet<char>(twoThreeFive.First());
            foreach (var set in twoThreeFive) dSet.IntersectWith(set);
            dSet.ExceptWith(aSet);
            dSet.ExceptWith(gSet);
            if (dSet.Count() != 1) throw new Exception("Logic error getting 'd'");
            char d = dSet.First();
            signalMapping.Add(d, 'd');

            var cSet = new HashSet<char>(sevenSet);
            cSet.ExceptWith(aSet);
            cSet.ExceptWith(fSet);
            if (cSet.Count() != 1) throw new Exception("Logic error getting 'c'");
            char c = cSet.First();
            signalMapping.Add(c, 'c');

            var eSet = new HashSet<char>(eightSet);
            eSet.ExceptWith(aSet);
            eSet.ExceptWith(bSet);
            eSet.ExceptWith(cSet);
            eSet.ExceptWith(dSet);
            eSet.ExceptWith(fSet);
            eSet.ExceptWith(gSet);
            if (eSet.Count() != 1) throw new Exception("Logic error getting 'e'");
            char e = eSet.First();
            signalMapping.Add(e, 'e');

            return signalMapping;
        }
    }
}