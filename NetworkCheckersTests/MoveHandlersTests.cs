using NetworkCheckersLib;
using NetworkCheckersLib.MoveHandlers;
using NUnit.Framework;
using System.Collections.Generic;

namespace NetworkCheckersTests
{
    [TestFixture]
    public class MoveHandlersTests
    {
        CheckerBoard board;
        [SetUp]
        public void SetUp()
        {
            board = new CheckerBoard();
            for(int i = 0;i<board.Rows;i++)
            {
                for (int j = 0; j < board.Cols; j++)
                {
                    board[i, j] = null;
                }
            }
        }

        [Test]
        public void Test_BeatHandler_ReturnsPossibleMovesCorrectly_WhenMoverIsQueen()
        {
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex c5 = BoardIndex.Parse("c5");
            Checker white = new Checker(PlayerType.White)
            {
                IsQueen = true
            };
            Checker black = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[e3.Row, e3.Col] = white;
            board[c5.Row, c5.Col] = black;
            BeatHandler beatHandler = new BeatHandler(board);
            var moves = beatHandler.GetMoves(c5, new List<BoardIndex>() { e3 });
            var expectedMoves = new List<BoardIndex>
            {
                BoardIndex.Parse("b6"),
                BoardIndex.Parse("a7")
            };
            Assert.AreEqual(expectedMoves, moves);
        }

        [Test]
        public void Test_BeatHandler_ReturnsPossibleMovesCorrectly_WhenMoverIsCheker()
        {
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex c5 = BoardIndex.Parse("c5");
            Checker white = new Checker(PlayerType.White)
            {
                IsQueen = false
            };
            Checker black = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[e3.Row, e3.Col] = white;
            board[c5.Row, c5.Col] = black;
            BeatHandler beatHandler = new BeatHandler(board);
            var moves = beatHandler.GetMoves(c5, new List<BoardIndex>() { e3 });
            var expectedMoves = new List<BoardIndex>();
            Assert.AreEqual(expectedMoves, moves);
        }

        [Test]
        public void Test_FirstMoveDoneHandler_ReturnEmptyMoves_When_CannotBeat_AfterMove()
        {
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex d4 = BoardIndex.Parse("d4");
            BoardIndex c5 = BoardIndex.Parse("c5");
            BoardIndex b6 = BoardIndex.Parse("b6");
            Checker white = new Checker(PlayerType.White)
            {
                IsQueen = false
            };
            Checker black = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[e3.Row, e3.Col] = white;
            board[b6.Row, b6.Col] = black;
            FirstMoveDoneHandler firstMoveDoneHandler = new FirstMoveDoneHandler(board);
            var moves = firstMoveDoneHandler.GetMoves(c5, new List<BoardIndex> { e3, d4 });

            Assert.AreEqual(new List<BoardIndex>(), moves);
        }

        [Test]
        public void Test_FirstMoveDoneHandler_ReturnMovesThatAllowsToBeat()
        {
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex d4 = BoardIndex.Parse("d4");
            BoardIndex c5 = BoardIndex.Parse("c5");
            BoardIndex b6 = BoardIndex.Parse("b6");
            BoardIndex a7 = BoardIndex.Parse("a7");
            Checker white = new Checker(PlayerType.White)
            {
                IsQueen = false
            };
            Checker black1 = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[e3.Row, e3.Col] = white;
            board[d4.Row, d4.Col] = black1;
            board[b6.Row, b6.Col] = black1;
            FirstMoveDoneHandler firstMoveDoneHandler = new FirstMoveDoneHandler(board);
            var moves = firstMoveDoneHandler.GetMoves(c5, new List<BoardIndex> { e3, c5 });
            Assert.AreEqual(new List<BoardIndex>() { a7 }, moves);
        }

        [Test]
        public void TestBeatHandler_CannotBeatTwoInOneRow()
        {
            BoardIndex g1 = BoardIndex.Parse("g1");
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex c5 = BoardIndex.Parse("c5");
            BoardIndex d4 = BoardIndex.Parse("d4");
            Checker white = new Checker(PlayerType.White)
            {
                IsQueen = true
            };
            Checker black1 = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            Checker black2 = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[g1.Row, g1.Col] = white;
            board[e3.Row, e3.Col] = black1;
            board[c5.Row, c5.Col] = black2;
            BeatHandler beatHandler = new BeatHandler(board);
            var moves = beatHandler.GetMoves(g1, new List<BoardIndex> { g1 });
            Assert.AreEqual(new List<BoardIndex>() { d4 }, moves);
        }

        [Test]
        public void TestBeatHandler_CannotBeatThroughtTheAlly()
        {
            BoardIndex g1 = BoardIndex.Parse("g1");
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex c5 = BoardIndex.Parse("c5");
            Checker white = new Checker(PlayerType.White)
            {
                IsQueen = true
            };
            Checker white2 = new Checker(PlayerType.White)
            {
                IsQueen = false
            };
            Checker black2 = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[g1.Row, g1.Col] = white;
            board[e3.Row, e3.Col] = white2;
            board[c5.Row, c5.Col] = black2;
            BeatHandler beatHandler = new BeatHandler(board);
            var moves = beatHandler.GetMoves(g1, new List<BoardIndex> { g1 });
            Assert.AreEqual(new List<BoardIndex>(), moves);
        }

        [Test]
        public void Test_HasOpponentInBetween_DiffIsEqualToOne()
        {
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex d4 = BoardIndex.Parse("d4");
            BoardIndex c5 = BoardIndex.Parse("c5");
            Checker black = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[d4.Row, d4.Col] = black;
            FirstMoveDoneHandler firstMoveDoneHandler = new FirstMoveDoneHandler(board);
            bool actual = firstMoveDoneHandler.HasOpponentInBetween(PlayerType.White, e3, c5);
            Assert.IsTrue(actual);
        }

        [Test]
        public void Test_HasOpponentInBetween_DiffIsMoreThanOne()
        {
            BoardIndex e3 = BoardIndex.Parse("e3");
            BoardIndex d4 = BoardIndex.Parse("d4");
            BoardIndex b6 = BoardIndex.Parse("b6");
            Checker black = new Checker(PlayerType.Black)
            {
                IsQueen = false
            };
            board[d4.Row, d4.Col] = black;
            FirstMoveDoneHandler firstMoveDoneHandler = new FirstMoveDoneHandler(board);
            bool actual = firstMoveDoneHandler.HasOpponentInBetween(PlayerType.White, e3, b6);
            Assert.IsTrue(actual);
        }
    }
}
