//using Overlay.NET.Common;
//using Overlay.NET.Wpf;
//using Process.NET;
//using Process.NET.Memory;
//using Process.NET.Windows;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text;

//namespace PennyDeadfulClient.MagicOnline
//{
//    partial class PlayLobbySceneView
//    {
//        private static ProcessSharp _processSharp;
//        private static PlayLobbyOverlay _overlay;
//        public void StartOverlay(System.Diagnostics.Process process)
//        {
//            _processSharp = new ProcessSharp(process, MemoryType.Remote);
//            _overlay = new PlayLobbyOverlay();
//            _overlay.Initialize(_processSharp.WindowFactory.MainWindow);
//            _overlay.Enable();
//        }
//    }


//    public class PlayLobbyOverlay : WpfOverlayPlugin
//    {
//        private readonly TickEngine _tickEngine = new TickEngine();
//        public override void Enable()
//        {
//            _tickEngine.IsTicking = true;
//            base.Enable();
//        }
//        public override void Disable()
//        {
//            _tickEngine.IsTicking = false;
//            base.Disable();
//        }
//        public override void Initialize(IWindow targetWindow)
//        {
//            // Set target window by calling the base method
//            base.Initialize(targetWindow);

//            OverlayWindow = new OverlayWindow(targetWindow);
//            // Set up update interval and register events for the tick engine.
//            _tickEngine.Interval = TimeSpan.FromSeconds(1);
//            _tickEngine.PreTick += OnPreTick;
//            _tickEngine.Tick += OnTick;
//        }

//        private void OnTick(object sender, EventArgs eventArgs)
//        {
//            // This will only be true if the target window is active
//            // (or very recently has been, depends on your update rate)
//            if (OverlayWindow.IsVisible)
//            {
//                OverlayWindow.Update();
//            }
//        }

//        private void OnPreTick(object sender, EventArgs eventArgs)
//        {
//            // Only want to set them up once.
//            //if (!_isSetup)
//            //{
//            //    SetUp();
//            //    _isSetup = true;
//            //}

//            var activated = TargetWindow.IsActivated;
//            var visible = OverlayWindow.IsVisible;

//            // Ensure window is shown or hidden correctly prior to updating
//            if (!activated && visible)
//            {
//                OverlayWindow.Hide();
//            }

//            else if (activated && !visible)
//            {
//                OverlayWindow.Show();
//            }
//        }

//        public override void Update() => _tickEngine.Pulse();
//    }
//}
