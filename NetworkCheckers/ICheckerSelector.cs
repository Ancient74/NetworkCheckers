using NetworkCheckersLib;

namespace NetworkCheckers
{
    public interface ICheckerSelector
    {
        void SelectChecker(CheckerViewModel checker);

        void SelectCell(CellViewModel cellViewModel);
    }
}