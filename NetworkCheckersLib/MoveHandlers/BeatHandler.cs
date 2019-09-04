using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkCheckersLib.MoveHandlers
{
    public class BeatHandler : HandlerBase
    {
        public BeatHandler(CheckerBoard board) : base(board)
        {
        }

        protected override List<BoardIndex> GetMovesInternal(Checker checker, BoardIndex begin, List<BoardIndex> moves)
        {
            List<BoardIndex> beatMoves = new List<BoardIndex>();
            for (int i = -1; i <= 1; i += 2)//for each direction
            {
                for (int j = -1; j <= 1; j += 2)
                {
                    bool stopMovingInCurrentDirection = false;
                    int x = begin.Row + i;
                    int y = begin.Col + j;
                    while(board.IsInBound(x, y))
                    {
                        if (CanBeat(checker, begin, new BoardIndex(x, y)))
                        {
                            if(!checker.IsQueen)
                                beatMoves.Add(new BoardIndex(begin.Row + i * 2, begin.Col + j * 2));
                            else
                            {
                                int xq = x + i;
                                int yq = y + j;
                                while(board.IsInBound(xq, yq))
                                {
                                    if (board[xq, yq] == null)
                                        beatMoves.Add(new BoardIndex(xq, yq));
                                    else
                                    {
                                        stopMovingInCurrentDirection = true;
                                        break;
                                    }
                                    xq += i;
                                    yq += j;
                                }
                            }
                        }
                        else if(stopMovingInCurrentDirection || (board[x, y] != null && board[x, y].PlayerType == checker.PlayerType))
                        {
                            break;
                        }
                        x += i;
                        y += j;
                    }
                }
            }

            if (beatMoves.Count > 0)
                return beatMoves;

            return Next?.GetMoves(begin, moves) ?? new List<BoardIndex>();
        }

        

        protected bool CanBeat(Checker checker, BoardIndex from, BoardIndex test)
        {
            if (!board.IsInBound(test.Row, test.Col))
                return false;

            PlayerType playerType = checker.PlayerType;

            Checker toTest = board[test.Row, test.Col];
            if (toTest == null || toTest.PlayerType == playerType)
                return false;

            int diffX = test.Row - from.Row;
            int diffY = test.Col - from.Col;

            int dirX = Math.Sign(diffX);
            int dirY = Math.Sign(diffY);

            int absDistanceX = Math.Abs(diffX);
            int absDistanceY = Math.Abs(diffY);

            bool checkFront(int dx, int dy)
            {
                (int frontX, int frontY) = (test.Row + dx, test.Col + dy);
                Checker inFront = board[frontX, frontY];
                if (inFront == null && board.IsInBound(frontX, frontY))
                    return true;
                else
                    return false;
            }
            if (absDistanceX > 1 && absDistanceY > 1 && !checker.IsQueen)
            {
                return false;
            }

            return checkFront(dirX, dirY);
        }
    }
}
