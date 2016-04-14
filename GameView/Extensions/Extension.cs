using System;
using System.Windows;
using System.Windows.Threading;

namespace GameView.Extensions
{
    public static class Extension
    {
        private static Action EmptyDelegate = delegate () { };

        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}
