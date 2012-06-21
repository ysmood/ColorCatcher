using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Threading;

namespace Display
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			RenderOptions.SetBitmapScalingMode(img_display, BitmapScalingMode.NearestNeighbor);

			Thread th = new Thread(new ThreadStart(() => {
				catcherData = new ColorCatcher.Data();
				Dispatcher.Invoke(new render_delegate(render));
			}));

			th.Start();
		}

		private ColorCatcher.Data catcherData;

		private void img_display_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			Image img = sender as Image;
			if (img.Stretch == Stretch.Uniform)
				img.Stretch = Stretch.None;
			else
				img.Stretch = Stretch.Uniform;
		}

		public delegate void render_delegate();
		private void render()
		{
			int red = 33;
			int blue = 16;
			int margin = 1;
			int width = red + margin + blue;
			UInt32[] pixel_data = new UInt32[width * catcherData.Lottery_list.Count];

			UInt32 pixel_color = 0xff000000;

			for (int i = 0; i < width * catcherData.Lottery_list.Count; i++)
			{
				pixel_data[i] = 0xffffffff;
			}

			for (int y = 0; y < catcherData.Lottery_list.Count; y++)
			{
				long offset = width * y;
				foreach (var item in catcherData.Lottery_list[y].Red_nums)
				{
					pixel_data[offset + item - 1] = pixel_color;
				}
				pixel_data[offset + red + margin - 1] = 0x0;
				pixel_data[offset + red + margin + catcherData.Lottery_list[y].Blue_num - 1] = pixel_color;
			}
			img_display.Source = BitmapSource.Create(width, catcherData.Lottery_list.Count, 72, 72, PixelFormats.Bgra32, null, pixel_data, ((width * 32 + 31) & ~31) / 8);
		}

		private bool is_animation_done = true;
		private void img_display_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (!is_animation_done)
			{
				img_display.RenderTransform.BeginAnimation(TranslateTransform.YProperty, null);
				is_animation_done = true;
				return;
			}

			DoubleAnimation da = new DoubleAnimation()
			{
				Duration = TimeSpan.FromSeconds(img_display.ActualHeight / 1000),
				From = -img_display.ActualHeight,
				To = 0,
				FillBehavior = FillBehavior.Stop
			};
			da.Completed += new EventHandler(da_Completed);
			is_animation_done = false;

			TranslateTransform tf = new TranslateTransform();
			img_display.RenderTransform = tf;
			tf.BeginAnimation(TranslateTransform.YProperty, da);
		}

		private void da_Completed(object sender, EventArgs e)
		{
			is_animation_done = true;
		}

	}
}
