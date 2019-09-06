using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib.Network
{
    public enum MessageType
    {
        StartGame = 0,
        PlayerType,
        StartTurn,
        Step,
        TurnEnd,
        QueenAppeared,
        Result,
        Name,
        Ping,
        TextMessage
    }
}
