using System;

namespace Advent2021
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Day 12");
            Console.WriteLine();

            var caveConfig = File.ReadAllText("./cave-system.txt");
            var caveSystem = new CaveGraph(caveConfig);
            caveSystem.PrintPaths();

            Console.WriteLine();
            caveSystem.PrintPathCount();

            Console.WriteLine("End Day 12");
        }
    }

    class CaveGraph
    {
        Dictionary<string, HashSet<Cave>> AdjacencyList;
        Dictionary<string, bool> Visited;

        public List<List<Cave>> Paths { get; private set; }

        public CaveGraph(string caveConfig)
        {
            this.Visited = new Dictionary<string, bool>();
            this.AdjacencyList = this.GenerateAdjacencyList(caveConfig);
            this.Paths = this.FindAllPaths();
        }

        public void PrintPathCount()
        {
            Console.WriteLine($"Path count:\t\t{this.Paths.Count}");
        }

        public void PrintPaths()
        {
            foreach (var path in this.Paths)
            {
                Console.Write("Path:\t");
                foreach (var cave in path)
                {
                    Console.Write($"{cave.Name},\t");
                }
                Console.WriteLine();
            }
        }

        private void InsertCavePair(Dictionary<string, HashSet<Cave>> adjacencyList, Cave a, Cave b)
        {
            if (!adjacencyList.ContainsKey(a.Name)) adjacencyList.Add(a.Name, new HashSet<Cave>());
            adjacencyList[a.Name].Add(b);
            if (!adjacencyList.ContainsKey(b.Name)) adjacencyList.Add(b.Name, new HashSet<Cave>());
            adjacencyList[b.Name].Add(a);

            if (!this.Visited.ContainsKey(a.Name)) this.Visited[a.Name] = false;
            if (!this.Visited.ContainsKey(b.Name)) this.Visited[b.Name] = false;
        }

        private Dictionary<string, HashSet<Cave>> GenerateAdjacencyList(string caveConfig)
        {
            var adjacencyList = new Dictionary<string, HashSet<Cave>>();

            foreach (var line in caveConfig.Split('\n'))
            {
                var startAndEnd = line.Split('-');
                var start = new Cave(startAndEnd[0]);
                var end = new Cave(startAndEnd[1]);
                this.InsertCavePair(adjacencyList, start, end);
            }

            return adjacencyList;
        }

        private List<List<Cave>> FindAllPaths()
        {
            var paths = new List<List<Cave>>();
            var pathBuilder = new List<Cave>();

            this.SearchPath(paths, pathBuilder, new Cave(Cave.StartCave));

            return paths;
        }

        private void SearchPath(List<List<Cave>> paths, List<Cave> pathBuilder, Cave cave)
        {
            pathBuilder.Add(cave);
            this.Visited[cave.Name] = true;

            if (cave.Name == Cave.EndCave)
            {
                paths.Add(new List<Cave>(pathBuilder));
            }
            else
            {
                var neighbors = this.AdjacencyList[cave.Name];
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.IsBigCave || !this.Visited[neighbor.Name])
                    {
                        this.SearchPath(paths, pathBuilder, neighbor);
                    }

                }
            }

            pathBuilder.RemoveAt(pathBuilder.Count - 1);
            this.Visited[cave.Name] = false;
        }
    }

    class Cave
    {
        public bool IsBigCave { get; private set;}
        public string Name { get; set; }

        public static string EndCave = "end";
        public static string StartCave = "start";

        public Cave(string caveName)
        {
            this.IsBigCave = Utils.IsStringUppercase(caveName);
            this.Name = caveName;
        }
    }

    class Utils
    {
        public static bool IsStringUppercase(string str)
        {
            foreach (var chr in str) if (!Char.IsUpper(chr)) return false;
            return true;
        }
    }
}
