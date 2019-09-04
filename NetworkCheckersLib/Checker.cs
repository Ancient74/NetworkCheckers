using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib
{
    public class Checker
    {
        public PlayerType PlayerType { get; }

        public bool IsQueen { get; set; } = false;

        public Checker(PlayerType playerType)
        {
            PlayerType = playerType;
        }
    }
}
