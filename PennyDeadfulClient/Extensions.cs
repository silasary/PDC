using System;
using System.Collections.Generic;
using System.Text;

namespace PennyDeadfulClient
{
    public static class Extensions
    {
        public static GameOverlay.Drawing.Point ToDrawing(this System.Windows.Point point)
        {
            return new GameOverlay.Drawing.Point((float)point.X, (float)point.Y);
        }

        public static GameOverlay.Drawing.Point Subtract(this GameOverlay.Drawing.Point a, GameOverlay.Drawing.Point b)
        {
            return new GameOverlay.Drawing.Point(a.X - b.X, a.Y - b.Y);
        }

    }
}
