using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.MoveHandlers
{
    class MoveHandler : HandlerBase
    {
        public MoveHandler(CheckerBoard board) : base(board)
        {
        }

        protected override List<BoardIndex> GetMovesInternal(Checker checker, BoardIndex begin, List<BoardIndex> moves)
        {
            return checker.IsQueen ? GetMovesForQueen(begin) : GetMovesForChecker(checker, begin);
        }

        private List<BoardIndex> GetMovesForQueen(BoardIndex begin)
        {
            List<BoardIndex> possibleMoves = new List<BoardIndex>();
            for (int i = -1; i <= 1; i += 2)//for each direction
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    int x = begin.Row + i;
                    int y = begin.Col + j;
                    while (board.IsInBound(x, y))
                    {
                        if (board[x, y] == null)
                            possibleMoves.Add(new BoardIndex(x, y));
                        else
                            break;
                        x += i;
                        y += j;
                    }
                }
            }
            return possibleMoves;
        }
        private List<BoardIndex> GetMovesForChecker(Checker checker, BoardIndex begin)
        {
            List<BoardIndex> possibleMoves = new List<BoardIndex>();
            for (int i = -1; i <= 1; i += 2)//for each direction
            {
                int dy;
                if (checker.PlayerType == PlayerType.White)
                    dy = -1;
                else
                    dy = 1;

                (int x, int y) = (begin.Row + i, begin.Col + dy);
                if (board[x, y] == null && board.IsInBound(x, y))
                    possibleMoves.Add(new BoardIndex(x, y));
            }
            return possibleMoves;
        }
    }
}
