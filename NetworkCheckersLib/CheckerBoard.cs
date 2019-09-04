using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCheckersLib.MoveHandlers;

namespace NetworkCheckersLib
{
    public class CheckerBoard
    {
        private readonly Checker[][] board;

        public int Rows { get; } = 8;
        public int Cols { get; } = 8;

        public Checker this[int x, int y]
        {
            get
            {
                if (!IsInBound(x, y))
                    return null;
                else
                    return board[x][y];
            }
            set
            {
                if (!IsInBound(x, y))
                    return;
                board[x][y] = value;
            }
        }

        public bool IsInBound(int x, int y)
        {
            return !(x < 0 || x >= Rows || y < 0 || y >= Cols);
        }

        public CheckerBoard()
        {
            board = new Checker[Rows][];
            for (int i = 0; i < Rows; i++)
            {
                board[i] = new Checker[Cols];
                for (int j = 0; j < 3; j++)
                {
                    bool evenRow = i % 2 == 0;
                    bool rest = j % 2 == 1;
                    if (evenRow ? rest : !rest)
                    {
                        board[i][j] = new Checker(PlayerType.Black);
                    }
                    else
                    {
                        board[i][Cols - j - 1] = new Checker(PlayerType.White);
                    }
                }
            }
        }

        public MoveBuilder GetMoveBuilder()
        {
            return new MoveBuilder(this);
        }

        public class MoveBuilder
        {
            readonly CheckerBoard board;
            internal MoveBuilder(CheckerBoard board)
            {
                this.board = board;
            }

            public BoardIndex GetLastMove()
            {
                return builtMoves.Last();
            }

            public MoveBuilder Start(BoardIndex index)
            {
                builtMoves = new List<BoardIndex>
                {
                    index
                };
                return this;
            }

            public bool IsStarted => builtMoves != null && builtMoves.Count > 0;

            List<BoardIndex> builtMoves;

            public bool Move(BoardIndex to)
            {
                var possible = GetPossibleMoves(builtMoves.Last());
                if (possible.Count == 0)
                    return false;

                if(!possible.Contains(to))
                    return false;

                builtMoves.Add(to);
                return true;
            }

            public List<BoardIndex> Build()
            {
                return builtMoves;
            }

            public List<BoardIndex> GetPossibleMoves(BoardIndex begin)
            {
                FirstMoveDoneHandler firstMoveDoneHandler = new FirstMoveDoneHandler(board);
                CannotBeatHandler cannotBeatHandler = new CannotBeatHandler(board);
                BeatHandler beatHandler = new BeatHandler(board);
                MoveHandler moveHandler = new MoveHandler(board);
                firstMoveDoneHandler.SetNext(cannotBeatHandler).SetNext(beatHandler).SetNext(moveHandler);
                return firstMoveDoneHandler.GetMoves(begin, new List<BoardIndex>(builtMoves));
            }

        }

    }
}
