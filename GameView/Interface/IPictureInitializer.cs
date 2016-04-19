using System.Windows.Controls;

using Controller.Interface;

namespace GameView.Interface
{
    public interface IPictureInitializer
    {
        void DrawImagesToCanvas(ICoreController core, Canvas fieldCan);

        void LoadPictures();

        void Redraw(ICoreController core, Canvas fieldCan);

        void SetGhostPictures(Canvas fieldCan);

        void PreLevelRenderedAction(ICoreController core, Canvas fieldCan);
    }
}
