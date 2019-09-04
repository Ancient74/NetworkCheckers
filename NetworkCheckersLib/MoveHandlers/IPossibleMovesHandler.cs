using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.MoveHandlers
{
    public interface IPossibleMovesHandler
    {
        List<BoardIndex> GetMoves(BoardIndex begin, List<BoardIndex> moves = null);
        IPossibleMovesHandler SetNext(IPossibleMovesHandler handler);
    }
}
