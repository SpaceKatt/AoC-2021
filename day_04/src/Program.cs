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

    class BingoSystem
    {
        List<int> bingoNumbers;
        List<BingoBoard> bingoBoards;
        List<BingoWinRecord> bingoWinRecords;

        public BingoSystem(BingoSystemOpts opts)
        {
            var bingoState = loadInitialGameState(opts.bingoGameFilePath);
            this.bingoBoards = bingoState.bingoBoards;
            this.bingoNumbers = bingoState.bingoNumbers;
            this.bingoWinRecords = new List<BingoWinRecord>();

            this.playGame();
        }

        public void printScores()
        {
            var delim = '\t';
            Console.WriteLine($"BoardId{delim}WinTurn{delim}Score");
            foreach (var win in this.bingoWinRecords)
            {
                Console.WriteLine($"{win.board.boardId}{delim}{win.turn}{delim}{win.score}");
            }
        }

        private void playGame()
        {
            int turnNumber = 0;
            foreach (var number in this.bingoNumbers)
            {
                foreach (var board in this.bingoBoards)
                {
                    this.takeTurn(number, board, turnNumber);
                }
                turnNumber++;
            }
        }

        private void takeTurn(int number, BingoBoard board, int turnNumber)
        {
            if (board.HasWon)
            {
                return;
            }

            board.MarkNumber(number);

            if (board.HasWon)
            {
                var bingoWin = new BingoWinRecord(board, turnNumber, board.GetScore());
                this.bingoWinRecords.Add(bingoWin);
            }

        }

        private BingoSystemState loadInitialGameState(string gameConfigPath)
        {
            List<string> gameConfig = File.ReadLines(gameConfigPath).ToList();

            string stepsConfig = gameConfig[0];
            gameConfig.RemoveAt(0);
            gameConfig.RemoveAt(0);

            var state = new BingoSystemState();
            state.bingoNumbers = this.LoadBingoNumbersFromConfig(stepsConfig);
            state.bingoBoards = this.LoadBoardsFromGameConfig(gameConfig);

            return state;
        }

        private List<int> LoadBingoNumbersFromConfig(string stepsConfig)
        {
            var steps = stepsConfig.Split(',').Select(step => int.Parse(step)).ToList();
            return steps;
        }

        private List<BingoBoard> LoadBoardsFromGameConfig(List<string> gameConfig)
        {
            var boards = new List<BingoBoard>();
            var boardConfigs = new List<List<List<string>>>();

            var boardConfigBuilder = new List<List<string>>();
            for (int i = 0; i < gameConfig.Count(); i++)
            {
                string currentConfigLine = gameConfig[i];

                if (currentConfigLine.Count() < 1)
                {
                    boardConfigs.Add(new List<List<string>>(boardConfigBuilder));
                    boardConfigBuilder.Clear();
                }
                else
                {
                    var boardLine = currentConfigLine.Split(' ').Where(x => x.Count() > 0).ToList();
                    boardConfigBuilder.Add(boardLine);
                }
            }

            if (boardConfigBuilder.Count() > 1)
            {
                boardConfigs.Add(new List<List<string>>(boardConfigBuilder));
            }

            int boardId = 0;
            foreach (var boardConfig in boardConfigs)
            {
                boards.Add(new BingoBoard(boardConfig, boardId++));
            }


            return boards;
        }
    }

    struct BingoSystemState
    {
        public List<int> bingoNumbers { get; set; }
        public List<BingoBoard> bingoBoards { get; set; }
    }

    class BingoWinRecord
    {
        public BingoBoard board { get; }
        public int turn { get; }
        public int score { get; }

        public BingoWinRecord(BingoBoard board, int turn, int score)
        {
            this.board = board;
            this.turn = turn;
            this.score = score;
        }
    }

    class BingoBoard
    {
        int boardSize;
        int winningNumber;
        public int boardId { get; }
        List<List<int>> board;
        List<List<bool>> marked;
        IDictionary<int, Coordinate> numberToCoordMap;

        public bool HasWon { get; set; }

        public BingoBoard(List<List<string>> board, int boardId)
        {
            List<List<int>> intBoard = board.Select(list => list.Select(value => int.Parse(value)).ToList()).ToList();
            this.HasWon = false;
            this.winningNumber = -1;

            this.board = intBoard;
            this.boardId = boardId;
            this.boardSize = board.Count();
            this.marked = Utils.InitializeBooleanMatrix(this.boardSize);

            this.numberToCoordMap = this.GenerateNumberToCoordMap(this.board);
        }

        public BingoBoard(List<List<int>> board)
        {
            this.HasWon = false;
            this.winningNumber = -1;

            this.board = board;
            this.boardSize = board.Count();
            this.marked = Utils.InitializeBooleanMatrix(this.boardSize);

            this.numberToCoordMap = this.GenerateNumberToCoordMap(this.board);
        }

        public int GetCoordinateValue(Coordinate coord)
        {
            return this.board[coord.X][coord.Y];
        }

        public bool IsCoordinateMarked(Coordinate coord)
        {
            return this.marked[coord.X][coord.Y];
        }

        public void MarkNumber(int number)
        {
            if (this.HasWon)
            {
                return;
            }

            if (this.numberToCoordMap.ContainsKey(number))
            {
                var coord = this.numberToCoordMap[number];
                this.marked[coord.X][coord.Y] = true;
            }

            if (this.IsWinner())
            {
                this.HasWon = true;
                this.winningNumber = number;
            }
        }

        private bool IsWinner()
        {
            var isWinner = false;
            int i = -1;
            while (++i < this.boardSize && !isWinner)
            {
                isWinner = this.IsWinningCol(i) || this.IsWinningRow(i);
            }
            return isWinner;
        }

        private bool IsWinningCol(int colIdx)
        {
            return this.marked[colIdx].Aggregate(true, (agg, x) => agg && x);
        }

        private bool IsWinningRow(int rowIdx)
        {
            var isWinner = true;
            for (int i = 0; i < this.boardSize; i++)
            {
                isWinner = isWinner && this.marked[i][rowIdx];
                if (isWinner == false) break;
            }
            return isWinner;
        }

        public int GetScore()
        {
            int unmarkedSum = 0;
            for (int i = 0; i < this.boardSize; i++)
            {
                for (int j = 0; j < this.boardSize; j++)
                {
                    var coord = new Coordinate(i, j);
                    if (!this.IsCoordinateMarked(coord))
                    {
                        unmarkedSum += this.GetCoordinateValue(coord);
                    }
                }
            }

            return unmarkedSum * this.winningNumber;
        }

        private IDictionary<int, Coordinate> GenerateNumberToCoordMap(List<List<int>> board)
        {
            var numberToCoordMap = new Dictionary<int, Coordinate>();

            for (int i = 0; i < this.boardSize; i++)
            {
                for (int j = 0; j < this.boardSize; j++)
                {
                    var number = this.board[i][j];
                    var coord = new Coordinate(i, j);

                    numberToCoordMap.Add(number, coord);
                }
            }
            return numberToCoordMap;
        }
    }

    class Utils
    {
        public static List<List<bool>> InitializeBooleanMatrix(int size)
        {
            var matrix = new List<List<bool>>();

            var matrixRow = new List<bool>();
            for (int i = 0; i < size; i++)
            {
                matrixRow.Add(false);
            }

            for (int i = 0; i < size; i++)
            {
                matrix.Add(new List<bool>(matrixRow));
            }
            return matrix;
        }
    }

    class SubmarineOpts : BingoSystemOpts {}

    struct Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    class BingoSystemOpts
    {
        public string bingoGameFilePath { get; set; }

        public BingoSystemOpts()
        {
            this.bingoGameFilePath = "./data/bingo_game.csv";
        }
    }
}
