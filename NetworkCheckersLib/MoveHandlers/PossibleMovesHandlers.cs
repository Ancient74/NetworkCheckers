using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.MoveHandlers
{
    public abstract class HandlerBase : IPossibleMovesHandler
    {
        public IPossibleMovesHandler Next { get; private set; }

        public List<BoardIndex> GetMoves(BoardIndex begin, List<BoardIndex> moves = null)
        {
            moves = moves ?? new List<BoardIndex>();
            begin = moves != null && moves.Count > 0 ? moves.Last() : begin;
            Checker checker = GetCheckerPreview(begin, moves);
            if (checker == null)
                return new List<BoardIndex>();
            return GetMovesInternal(checker, begin, moves);
        }

        protected abstract List<BoardIndex> GetMovesInternal(Checker checker, BoardIndex begin, List<BoardIndex> moves);

        protected CheckerBoard board;
        public HandlerBase(CheckerBoard board)
        {
            this.board = board;
        }

        protected Checker GetCheckerPreview(BoardIndex index, List<BoardIndex> moves)
        {
            if(moves.Count > 0)
            {
                BoardIndex last = moves.Last();
                if (last.Equals(index))
                    last = moves[0];
                return board[index.Row, index.Col] ?? board[last.Row, last.Col];
            }
            return board[index.Row, index.Col];
        }

        public IPossibleMovesHandler SetNext(IPossibleMovesHandler handler)
        {
            Next = handler;
            return handler;
        }
    }
}
