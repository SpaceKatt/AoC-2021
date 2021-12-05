namespace Advent2021
{
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
}