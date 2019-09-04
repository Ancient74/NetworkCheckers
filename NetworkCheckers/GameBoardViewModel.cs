using NetworkCheckersLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkCheckers
{
    public class GameBoardViewModel : NotifiableObject, ICheckerSelector 
    {
        public ObservableCollection<CellViewModel> Board { get; }

        private readonly CheckerBoard checkerBoard;
        CheckerViewModel selectedChecker;
        public CheckerViewModel SelectedChecker
        {
            get => selectedChecker;
            set
            {
                if (selectedChecker == value)
                {
                    return;
                }
                if (selectedChecker != null)
                    selectedChecker.Selected = false;
                selectedChecker = value;
                if (selectedChecker != null)
                    selectedChecker.Selected = true;
            }
        }

        public int Rows => checkerBoard.Rows;
        public int Cols => checkerBoard.Cols;

        private PlayerType PlayerType { get; }

        public CellViewModel this[int x, int y]
        {
            get => Board[x + y * Rows];
            set => Board[x + y * Rows] = value;
        }

        public void Remove(BoardIndex index)
        {
            checkerBoard[index.Row, index.Col] = null;
            Init();
        }

        public void Move(BoardIndex from, BoardIndex to)
        {
            var c = checkerBoard[from.Row, from.Col];
            checkerBoard[from.Row, from.Col] = null;
            checkerBoard[to.Row, to.Col] = c;
            Init();
        }

        public CheckerBoard.MoveBuilder GetMoveBuilder()
        {
            return checkerBoard.GetMoveBuilder();
        }

        public event Action<BoardIndex, CheckerViewModel> ChekerSelected;
        public event Action<BoardIndex, CellViewModel> CellSelected;

        public void Init()
        {
            for (int i = 0; i < Rows; i++)
            {
               
                for (int j = 0; j < Cols; j++)
                {
                    NetworkCheckersLib.Checker checker = checkerBoard[i, j];
                    if (checker != null)
                        this[i, j].Checker = new CheckerViewModel(checker, PlayerType==PlayerType.Black, this);
                    else
                        this[i, j].Checker = null;
                }
            }
            OnPropertyChanged("Board");
        }

        public void SelectChecker(CheckerViewModel checker)
        {
            NetworkCheckersLib.Checker inner = checker.Checker;
            BoardIndex index = FindChecker(inner);
            if(!index.Equals(new BoardIndex(-1, -1)))
                ChekerSelected?.Invoke(index, checker);
        }

        BoardIndex FindChecker(NetworkCheckersLib.Checker checker)
        {
            for (int i = 0; i < checkerBoard.Rows; i++)
            {
                for (int j = 0; j < checkerBoard.Cols; j++)
                {
                    if (checker == checkerBoard[i, j])
                        return new BoardIndex(i, j);
                }
            }
            return new BoardIndex(-1, -1);
        }


        BoardIndex FindCell(CellViewModel cell)
        {
            for (int i = 0; i < checkerBoard.Rows; i++)
            {
                for (int j = 0; j < checkerBoard.Cols; j++)
                {
                    if (cell == this[i, j])
                        return new BoardIndex(i, j);
                }
            }
            return new BoardIndex(-1, -1);
        }

        public GameBoardViewModel(PlayerType currentPlayerType)
        {
            PlayerType = currentPlayerType;
            checkerBoard = new CheckerBoard();
            Board = new ObservableCollection<CellViewModel>();
            for (int i = 0; i < Rows * Cols; i++)
            {
                int y = i % Rows;
                int x = i / Cols;
                bool evenRow = y % 2 == 0;
                bool rest = x % 2 == 1;
                Board.Add(new CellViewModel(evenRow ? rest : !rest, this));
            }
            OnPropertyChanged("Board");
        }

        public void HighlightCells(List<BoardIndex> indices)
        {
            UnHighlightAll();
            indices.ForEach(x => HighlightCell(x));
        }

        private void HighlightCell(BoardIndex index)
        {
            this[index.Row, index.Col].IsHighlighted = true;
        }

        public void UnHighlightAll()
        {
            foreach (var cell in Board)
                cell.IsHighlighted = false;
        }

        public void SelectCell(CellViewModel cellViewModel)
        {
            BoardIndex index = FindCell(cellViewModel);
            if (!index.Equals(new BoardIndex(-1, -1)))
                CellSelected?.Invoke(index, cellViewModel);
        }
    }
}
