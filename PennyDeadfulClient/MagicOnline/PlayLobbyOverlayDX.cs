using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Automation;
using GameOverlay.Drawing;
using GameOverlay.Windows;


namespace PennyDeadfulClient.MagicOnline
{
	partial class PlayLobbySceneView : IDisposable
	{

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		private readonly GraphicsWindow _window;

		public static System.Drawing.Point[] DeckListErrors { get; internal set; }

		private readonly Dictionary<string, SolidBrush> _brushes;
		private readonly Dictionary<string, Font> _fonts;
		private readonly Dictionary<string, Image> _images;
		private readonly IntPtr hwnd;
		private readonly AutomationElement shinyMain;

		public PlayLobbySceneView(IntPtr hwnd, AutomationElement shinyMain)
		{
			this.hwnd = hwnd;
			this.shinyMain = shinyMain;

			_brushes = new Dictionary<string, SolidBrush>();
			_fonts = new Dictionary<string, Font>();
			_images = new Dictionary<string, Image>();
			var gfx = new Graphics()
			{
				MeasureFPS = true,
				PerPrimitiveAntiAliasing = true,
				TextAntiAliasing = true
			};
			_window = new StickyWindow(hwnd, gfx)
			{
				FPS = 60,
				IsTopmost = true,
				IsVisible = true
			};
			_window.DestroyGraphics += _window_DestroyGraphics;
			_window.DrawGraphics += _window_DrawGraphics;
			_window.SetupGraphics += _window_SetupGraphics;

			_window.Create();
		}
		private void _window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
		{
			var gfx = e.Graphics;

			if (e.RecreateResources)
			{
				foreach (var pair in _brushes) pair.Value.Dispose();
				foreach (var pair in _images) pair.Value.Dispose();
			}

			_brushes["black"] = gfx.CreateSolidBrush(0, 0, 0);
			_brushes["white"] = gfx.CreateSolidBrush(255, 255, 255);
			_brushes["red"] = gfx.CreateSolidBrush(255, 0, 0);
			_brushes["green"] = gfx.CreateSolidBrush(0, 255, 0);
			_brushes["blue"] = gfx.CreateSolidBrush(0, 0, 255);
			_brushes["background"] = gfx.CreateSolidBrush(0x33, 0x36, 0x3F);
			_brushes["grid"] = gfx.CreateSolidBrush(255, 255, 255, 0.2f);
			_brushes["random"] = gfx.CreateSolidBrush(0, 0, 0);

			if (e.RecreateResources) return;

			_fonts["arial"] = gfx.CreateFont("Arial", 12);
			_fonts["consolas"] = gfx.CreateFont("Consolas", 14);
		}

		private void _window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
		{
			foreach (var pair in _brushes) pair.Value.Dispose();
			foreach (var pair in _fonts) pair.Value.Dispose();
			foreach (var pair in _images) pair.Value.Dispose();
		}

		private void _window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
		{
			var abs = new Point((int)shinyMain.Current.BoundingRectangle.X, (int)shinyMain.Current.BoundingRectangle.Y);

			var gfx = e.Graphics;

			var padding = 16;
			var infoText = new StringBuilder()
				.Append("FPS: ").Append(gfx.FPS.ToString().PadRight(padding))
				.Append("FrameTime: ").Append(e.FrameTime.ToString().PadRight(padding))
				.Append("FrameCount: ").Append(e.FrameCount.ToString().PadRight(padding))
				.Append("DeltaTime: ").Append(e.DeltaTime.ToString().PadRight(padding))
				.ToString();

			gfx.ClearScene();

			if (GetForegroundWindow() != hwnd)
				return;


			gfx.DrawTextWithBackground(_fonts["consolas"], _brushes["green"], _brushes["black"], 250, 3, infoText);

			if (DeckListErrors != null)
			{
				foreach (var error in DeckListErrors)
				{
					gfx.DrawText(_fonts["consolas"], _brushes["red"], new Point(error.X - abs.X - 20, error.Y - abs.Y + 2), "❌");
				}
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		~PlayLobbySceneView()
		{
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					_window.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
