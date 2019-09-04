using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.MoveHandlers
{
    public class CannotBeatHandler : HandlerBase
    {
        readonly BeatHandler beatHandler;
        public CannotBeatHandler(CheckerBoard board) : base(board)
        {
            beatHandler = new BeatHandler(board);
        }

        protected override List<BoardIndex> GetMovesInternal(Checker checker, BoardIndex begin, List<BoardIndex> moves)
        {
            Stack<Checker> checkersThatCanBeat = new Stack<Checker>();
            for (int i = 0; i < board.Rows; i++)
            {
                for (int j = 0; j < board.Cols; j++)
                {
                    Checker toTest = board[i, j];
                    if (toTest == null || toTest.PlayerType != checker.PlayerType)
                        continue;

                    var otherMoves = beatHandler.GetMoves(new BoardIndex(i, j));
                    if (otherMoves.Count > 0)
                        checkersThatCanBeat.Push(toTest);
                }
            }
            if(checkersThatCanBeat.Count == 0 || checkersThatCanBeat.Contains(checker))
                return Next?.GetMoves(begin, moves) ?? new List<BoardIndex>();
            return new List<BoardIndex>();
        }
    }
}
