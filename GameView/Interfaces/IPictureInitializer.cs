using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Controller.Interfaces;

namespace GameView.Interfaces
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
