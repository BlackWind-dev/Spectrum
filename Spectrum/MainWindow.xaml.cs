using Spectrum.Renderer;
using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Spectrum
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		int Width => 400;
		int Height => 400;
		bool Work = true;

		public MainWindow()
		{
			InitializeComponent();
			image.Source = bitmap;

			Thread thread = new Thread(Render);
			thread.Start();
		}
		WriteableBitmap bitmap = new(400, 400, 96, 96, PixelFormats.Gray8, null);

		private void Render()
		{
			var ray = new RayMarching();
			byte[] data = new byte[(int)(Width * Height)];

			while (Work)
			{
				RayMarching.Tick++;
				if (RayMarching.Tick >= 180)
				{
					RayMarching.Tick = 0;
				}

				DateTime prev = DateTime.Now;

				Parallel.For(0, Height, delegate (int y)
				{
					for (int x = 0; x < Width; x++)
					{
						Vector3 position = new(x / (float)Width * 2.0f - 1.0f, y / (float)Height * 2.0f - 1.0f, 1.0f);

						Vector3 c = new(0.0f, 0.0f, -5.0f);

						byte col = (byte)ray.Render(c, position);

						data[(int)(x + y * Width)] = col;
						//data[(int)(x + y * Width) + 1] = col;
						//data[(int)(x + y * Width) + 2] = col;
						//data[(int)(x + y * Width) + 3] = 0;
					}
				});
				
				this.Dispatcher.Invoke(() =>
				{
					bitmap.Lock();
					bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), data, bitmap.BackBufferStride, 0);
					fps.Text = $"{1000 / (DateTime.Now - prev).TotalMilliseconds}";
					bitmap.Unlock();
				});

				Thread.Sleep(16);
			}
		}

		private void Window_Closed(object sender, EventArgs e)
		{
			Work = false;
		}
	}
}
