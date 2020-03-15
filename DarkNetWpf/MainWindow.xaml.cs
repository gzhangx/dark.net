using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using veda.darknet.invoke;
using System.Drawing.Imaging;
using System.Drawing;

namespace DarkNetWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread loadingThread;
        IntPtr net = IntPtr.Zero;
        public MainWindow()
        {
            InitializeComponent();
            Dsp("Loading network");
            loadingThread = new Thread(() =>
            {
                net = Core.ggCreateNetwork();
                Dsp("Done Loading network");
            });
            loadingThread.Start();
        }

        void Dsp(string s)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                txtInfo.Text = s;
            }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Core.gFreeNetwork(net);
        }

        private void btnLoadImg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtFileName.Text == "")
                {
                    Dsp("No file name given");
                    return;
                }
                if (net == IntPtr.Zero)
                {
                    Dsp("NEt not ready");
                    return;
                }
                var buf = File.ReadAllBytes(txtFileName.Text);
                if (net != IntPtr.Zero)
                {
                    var res = Core.Detect(net, buf, 0.8f);
                    var img = Bitmap.FromStream(new MemoryStream(buf));
                    using (var graphics = Graphics.FromImage(img))
                    {
                        //graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //graphics.CompositingMode = CompositingMode.SourceCopy;
                        //graphics.DrawImage(image, 0, 0, width, height);
                        res.objects.ForEach(o =>
                        {
                            var cx = (int)(o.relative_coordinates.center_x * img.Width);
                            var cy = (int)(o.relative_coordinates.center_y * img.Width);
                            var w = (int)(o.relative_coordinates.width * img.Width);
                            var h = (int)(o.relative_coordinates.height * img.Height);
                            var left = cx - (w / 2);
                            var top = cy - (h / 2);
                            graphics.DrawRectangle(System.Drawing.Pens.Red, new System.Drawing.Rectangle(left, top, w, h));
                        });
                        img.Save($"c:\\temp\\resized-1.png", ImageFormat.Png);
                    }
                }
            } catch (Exception exc)
            {
                Dsp(exc.Message);
            }
        }
    }
}
