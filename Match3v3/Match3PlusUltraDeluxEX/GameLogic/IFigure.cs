using System.Windows.Media.Imaging;

namespace Match3v3.GameLogic
{
    public interface IFigure
    {
        FigureType Type { get; set; }
        Vector2 Position { get; set; }
        bool IsNullObject { get; }
        void Destroy(IFigure[,] list);
        BitmapImage GetBitmapImage();
    }
}