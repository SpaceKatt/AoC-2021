namespace Advent2021
{
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


    class BingoSystemOpts
    {
        public string bingoGameFilePath { get; set; }

        public BingoSystemOpts()
        {
            this.bingoGameFilePath = "./data/bingo_game.csv";
        }
    }

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
}