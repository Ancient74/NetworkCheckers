using NetworkCheckersLib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersTests
{
    [TestFixture]
    public class BoardIndexTests
    {
        [Test, Combinatorial]
        public void Test_ParseReturnsSameBoardIndex([Values(0, 1, 2, 3, 4, 5, 6, 7)]int x, [Values(0, 1, 2, 3, 4, 5, 6, 7)]int y)
        {
            var index = new BoardIndex(x, y);
            string str = index.ToString();
            var res = BoardIndex.Parse(str);

            Assert.AreEqual(index.Row, res.Row);
            Assert.AreEqual(index.Col, res.Col);
        }

    }
}
