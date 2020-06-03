using Overlay.NET.Common;
using Overlay.NET.Wpf;
using Process.NET;
using Process.NET.Memory;
using Process.NET.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PennyDeadfulClient.MagicOnline
{
    partial class PlayLobbySceneView
    {
        private static ProcessSharp _processSharp;
        private static PlayLobbyOverlay _overlay;
        public void StartOverlay(System.Diagnostics.Process process)
        {
            _processSharp = new ProcessSharp(process, MemoryType.Remote);
            _overlay = new PlayLobbyOverlay();
            _overlay.Initialize(_processSharp.WindowFactory.MainWindow);
            _overlay.Enable();
            System.Threading.Tasks.Task.Run(() =>
            {
                while (true)
                {
                    _overlay?.Update();
                }
            });
        }
    }


    public class PlayLobbyOverlay : WpfOverlayPlugin
    {
        private readonly TickEngine _tickEngine = new TickEngine();
        private Polygon _polygon;
        private bool _isSetup;

        public static Point[] DeckListErrors { get; set; }

        public override void Enable()
        {
            _tickEngine.IsTicking = true;
            base.Enable();
        }
        public override void Disable()
        {
            _tickEngine.IsTicking = false;
            base.Disable();
        }
        public override void Initialize(IWindow targetWindow)
        {
            // Set target window by calling the base method
            base.Initialize(targetWindow);

            OverlayWindow = new OverlayWindow(targetWindow);
            // Set up update interval and register events for the tick engine.
            _tickEngine.Interval = TimeSpan.FromSeconds(1);
            _tickEngine.PreTick += OnPreTick;
            _tickEngine.Tick += OnTick;

            OverlayWindow.Draw += OverlayWindow_Draw;
        }

        private void OverlayWindow_Draw(object sender, DrawingContext context)
        {
            if (DeckListErrors.Any())
                context.DrawRectangle(null, new Pen(Brushes.Red, 10), new Rect(0, 0, 20, 20));
            foreach (var point in DeckListErrors)
            {
                //context.DrawText(new FormattedText("X", CultureInfo.CurrentCulture, FlowDirection.RightToLeft, new Typeface("Verdana"), 36, Brushes.BlueViolet), point);
                context.DrawRectangle(null, new Pen(Brushes.Blue, 10), new Rect(point, new Size(10,10)));
            }
        }

        private void OnTick(object sender, EventArgs eventArgs)
        {
            // This will only be true if the target window is active
            // (or very recently has been, depends on your update rate)
            if (OverlayWindow.IsVisible)
            {
                Application.Current.Dispatcher.Invoke(OverlayWindow.Update);
            }
        }

        private void OnPreTick(object sender, EventArgs eventArgs)
        {
            var activated = TargetWindow.IsActivated;
            var visible = OverlayWindow.IsVisible;

            // Ensure window is shown or hidden correctly prior to updating
            if (!activated && visible)
            {
                Application.Current.Dispatcher.Invoke(OverlayWindow.Hide);
            }

            else if (activated && !visible)
            {
                Application.Current.Dispatcher.Invoke(OverlayWindow.Show);
            }
        }

        public override void Update() => _tickEngine.Pulse();
    }
}
