using System;

namespace Advent2021
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 10");
            var configString = File.ReadAllText("./navigation-config.txt");

            var parser = new NavigationParser(configString);
            parser.PrintCorruptionScore();

            Console.WriteLine("Finish Day 10");
        }
    }

    class NavigationParser
    {
        private List<NavigationInstruction> corruptedInstructions;
        private List<NavigationInstruction> incompleteInstructions;
        private List<NavigationInstruction> completeInstructions;

        public NavigationParser(string navigationConfig)
        {
            this.corruptedInstructions = new List<NavigationInstruction>();
            this.incompleteInstructions = new List<NavigationInstruction>();
            this.completeInstructions = new List<NavigationInstruction>();

            foreach (var instructionConfig in navigationConfig.Split('\n'))
            {
                var instruction = new NavigationInstruction(instructionConfig);
                switch (instruction.InstructionState)
                {
                    case NavigationInstructionState.CORRUPT:
                        this.corruptedInstructions.Add(instruction);
                        break;
                    case NavigationInstructionState.COMPLETE:
                        this.completeInstructions.Add(instruction);
                        break;
                    case NavigationInstructionState.INCOMPLETE:
                        this.incompleteInstructions.Add(instruction);
                        break;
                    default:
                        throw new Exception("Instruction in unknown state");
                }
            }
        }

        public void PrintCorruptionScore()
        {
            if (this.corruptedInstructions.Count < 1)
            {
                Console.WriteLine("No corruption detected!");
                return;
            }

            int corruptionSum = 0;

            foreach (var corruptedInstruction in this.corruptedInstructions)
            {
                corruptionSum += corruptedInstruction.GetCorruptionScore();
            }

            Console.WriteLine($"Total corruption score:\t\t{corruptionSum}");
        }
    }

    class NavigationInstruction
    {
        public NavigationInstructionState InstructionState {get; private set; }
        private char FirstIllegalChar;

        private static Dictionary<char, int> ScoreMap = new Dictionary<char, int>
        {
           {')', 3},
           {']', 57},
           {'}', 1197},
           {'>', 25137}
        };

        private static HashSet<char> ValidChars = new HashSet<char>
        {
            '{', '}',
            '<', '>',
            '[', ']',
            '(', ')',
        };

        private static Dictionary<char, char> CloseCharToOpen = new Dictionary<char, char>
        {
            {'}', '{'},
            {')', '('},
            {']', '['},
            {'>', '<'},
        };

        public NavigationInstruction(string instruction)
        {
            this.InstructionState = NavigationInstructionState.INCOMPLETE;
            this.FirstIllegalChar = '\0';
            this.ParseInstruction(instruction.Trim());
        }

        public bool IsState(NavigationInstructionState state)
        {
            return this.InstructionState == state;
        }

        public int GetCorruptionScore()
        {
            if (!this.IsState(NavigationInstructionState.CORRUPT))
            {
                throw new Exception("Corruption score does not exist on non-corrupt instruction");
            }

            return NavigationInstruction.ScoreMap[this.FirstIllegalChar];
        }

        private void ParseInstruction(string instruction)
        {
            List<char> instructionChars = instruction.ToList();
            var parenStack = new Stack<char>();

            foreach (var character in instructionChars)
            {
                if (!NavigationInstruction.ValidChars.Contains(character))
                {
                    throw new Exception("Invalid char detecting during parsing");
                }

                if (NavigationInstruction.CloseCharToOpen.ContainsKey(character))
                {
                    var closer = character;
                    var candidateOpener = parenStack.Pop();

                    if (candidateOpener != NavigationInstruction.CloseCharToOpen[closer])
                    {
                        this.InstructionState = NavigationInstructionState.CORRUPT;
                        this.FirstIllegalChar = closer;
                        return;
                    }
                }
                else
                {
                    parenStack.Push(character);
                }
            }

            if (parenStack.Count == 0)
            {
                this.InstructionState = NavigationInstructionState.COMPLETE;
            }
        }
    }

    enum NavigationInstructionState
    {
        CORRUPT,
        INCOMPLETE,
        COMPLETE,
    }
}
