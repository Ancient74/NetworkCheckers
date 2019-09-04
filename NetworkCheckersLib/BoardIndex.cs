using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckersLib
{
    public struct BoardIndex
    {
        private readonly static Dictionary<char, int> charToIntDictionary = new Dictionary<char, int>();
        static BoardIndex()
        {
            charToIntDictionary.Add('a', 0);
            charToIntDictionary.Add('b', 1);
            charToIntDictionary.Add('c', 2);
            charToIntDictionary.Add('d', 3);
            charToIntDictionary.Add('e', 4);
            charToIntDictionary.Add('f', 5);
            charToIntDictionary.Add('g', 6);
            charToIntDictionary.Add('h', 7);
        }
        public static BoardIndex Parse(string index)
        {
            if (string.IsNullOrEmpty(index))
                return default;

            int col = 8 - int.Parse(index[1].ToString());
            int row = charToIntDictionary[char.ToLower(index[0])];

            return new BoardIndex(row, col);
        }

        public int Col { get; set; }
        public int Row { get; set; }

        public BoardIndex(int row, int col)
        {
            Col = col;
            Row = row;
        }

        public override string ToString()
        {
            var self = this;
            char character = charToIntDictionary.FirstOrDefault(x => x.Value == self.Row).Key;
            char number = (8 - Col).ToString()[0];
            return character.ToString() + number.ToString();
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;

            if (!(obj is BoardIndex boardIndex))
                return false;

            return boardIndex.Col == Col && boardIndex.Row == Row;
        }

        public override int GetHashCode()
        {
            return (Col, Row).GetHashCode();
        }
    }
}
