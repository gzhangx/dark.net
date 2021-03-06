﻿using System;
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
using Emgu.CV;

namespace DarkNetWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread loadingThread;
        IntPtr net = IntPtr.Zero;
        VideoCapture cap = new VideoCapture();
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
            
            cap.ImageGrabbed += Cap_ImageGrabbed;
            cap.Start();
        }


        bool inprocessing = false;
        bool inProcessingSleep = false;
        DateTime processingStart = DateTime.Now;
        DateTime processingEnd = DateTime.Now;
        private void Cap_ImageGrabbed(object sender, EventArgs e)
        {
            if (inprocessing) return;
            if (inProcessingSleep)
            {
                if (DateTime.Now.Subtract(processingEnd).TotalMilliseconds < 0)
                    return;
            }
            inProcessingSleep = false;
            processingStart = DateTime.Now;
            processingEnd = processingStart.AddSeconds(1);
            inprocessing = true;
            //cap.Retrieve()
            using (Mat mat = new Mat())
            {
                cap.Retrieve(mat);

                if (mat == null)
                {
                    inprocessing = false;
                    inProcessingSleep = true;
                    return;
                }
                var buf = mat.matToImageBuf();
                buf = ProcessImage(buf);
                if (buf == null)
                {
                    inprocessing = false;
                    inProcessingSleep = true;
                    return;
                }
                DspAct(() =>
                {                    
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    //bitmap.UriSource = new Uri(selectedFileName);                                        
                    bitmap.StreamSource = new MemoryStream(buf);
                    bitmap.EndInit();
                    imgResult.Source = bitmap;
                    //imgResult.Source = src;
                    mat.Dispose();
                    inProcessingSleep = true;
                    processingEnd = processingStart.AddSeconds(5);
                    inprocessing = false;
                });
            }
        }


        void DspAct(Action act)
        {
            Dispatcher.BeginInvoke(new Action(act));
        }
        void Dsp(string s)
        {
            DspAct(() =>
            {
                txtInfo.Text = s;
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Core.gFreeNetwork(net);
            cap.Stop();
        }

        byte[] ProcessImage(byte[] buf)
        {
            if (net != IntPtr.Zero)
            {
                var res = Core.Detect(net, buf, 0.8f);
                using (var img = Bitmap.FromStream(new MemoryStream(buf)))
                {
                    using (var graphics = Graphics.FromImage(img))
                    {
                        //graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        //graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        //graphics.CompositingMode = CompositingMode.SourceCopy;
                        //graphics.DrawImage(image, 0, 0, width, height);
                        res.ForEach(o =>
                        {
                            var cx = (int)(o.box.x * img.Width);
                            var cy = (int)(o.box.y * img.Height);
                            var w = (int)(o.box.w * img.Width);
                            var h = (int)(o.box.h * img.Height);
                            var left = cx - (w / 2);
                            var top = cy - (h / 2);
                            graphics.DrawRectangle(System.Drawing.Pens.Red, new System.Drawing.Rectangle(left, top, w, h));
                            graphics.DrawString($"name {o.name} prob {o.confidence.ToString("N2")}", new Font("Arial", 8), System.Drawing.Brushes.White,
                                new PointF(left, top +10));
                        });
                        //img.Save($"c:\\temp\\resized-1.png", ImageFormat.Png);
                    }
                    using (var ms = new MemoryStream())
                    {
                        img.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
            }
            return null;
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
                        res.ForEach(o =>
                        {
                            var cx = (int)(o.box.x * img.Width);
                            var cy = (int)(o.box.y * img.Height);
                            var w = (int)(o.box.w * img.Width);
                            var h = (int)(o.box.h * img.Height);
                            var left = cx - (w / 2);
                            var top = cy - (h / 2) ;
                            graphics.DrawRectangle(System.Drawing.Pens.Red, new System.Drawing.Rectangle(left, top, w, h));
                        });
                        img.Save($"c:\\temp\\resized-1.png", ImageFormat.Png);
                    }
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    //bitmap.UriSource = new Uri(selectedFileName);
                    var ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    bitmap.StreamSource = ms;
                    bitmap.EndInit();
                    imgResult.Source = bitmap;
                }
            } catch (Exception exc)
            {
                Dsp(exc.Message);
            }
        }
    }
}
