using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.MoveHandlers
{
    public class FirstMoveDoneHandler : HandlerBase
    {
        readonly BeatHandler beatHandler;
        public FirstMoveDoneHandler(CheckerBoard board) : base(board)
        {
            beatHandler = new BeatHandler(board);
        }

        protected override List<BoardIndex> GetMovesInternal(Checker checker, BoardIndex begin, List<BoardIndex> moves)
        {
            if(moves.Count <= 1)
                return Next?.GetMoves(begin, moves) ?? new List<BoardIndex>();

            if (moves.Count > 1 && HasOpponentInBetween(checker.PlayerType, moves[moves.Count - 2], moves[moves.Count - 1]))
            {
                var beatMoves = beatHandler.GetMoves(begin, moves);
                if (beatMoves.Count == 0)
                {
                    return new List<BoardIndex>();
                }
                else
                {
                    var from = moves[moves.Count - 2];
                    var to = moves[moves.Count - 1];
                    int diffX = to.Row - from.Row;
                    int diffY = to.Col - from.Col;
                    int dx = Math.Sign(diffX);
                    int dy = Math.Sign(diffY);
                    int x = from.Row;
                    int y = from.Col;
                    while(x != to.Row && y != to.Col)
                    {
                        BoardIndex index = new BoardIndex(x, y);
                        beatMoves.Remove(index);
                        x += dx;
                        y += dy;
                    }
                    return beatMoves;
                }
            }
            return new List<BoardIndex>();
        }

        public bool HasOpponentInBetween(PlayerType playerType, BoardIndex from, BoardIndex to)
        {
            int diffX = to.Row - from.Row;
            int diffY = to.Col - from.Col;

            int dX = Math.Sign(diffX);
            int dY = Math.Sign(diffY);

            int x = from.Row + dX;
            int y = from.Col + dY;

            while ((x - dX) != to.Row && (y - dY) != to.Col)
            {
                Checker checker = board[x, y];
                if (checker != null && checker.PlayerType != playerType)
                    return true;
                x += dX;
                y += dY;
            }
            return false;
        }

    }
}
