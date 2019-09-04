using System.Windows;

namespace NetworkCheckers
{
    public class MessageViewModel
    {
        public string Text { get; }

        public HorizontalAlignment Alignment { get; }

        public MessageViewModel(string text, bool me)
        {
            if (me)
                Alignment = HorizontalAlignment.Right;
            else
                Alignment = HorizontalAlignment.Left;

            Text = text;
        }
    }
}