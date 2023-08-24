using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfAppOpenCV
{
    public class FrameArgs : EventArgs
    {
        public BitmapSource bmp;
    }

    public class rtspCamera
    {
        bool isProcessing = false;

        private static object locker = new object();
        public event EventHandler OnFrameReceived;

        private Mat frameCache;
        FrameArgs args;
        VideoCapture capture;
        CancellationTokenSource cts, cts2;

        public rtspCamera()
        {
            args = new FrameArgs();
            capture = new VideoCapture();
        }

        public void Start(string _url)
        {
            cts = new CancellationTokenSource();
            cts2 = new CancellationTokenSource();

            capture.Open(_url, VideoCaptureAPIs.ANY);

            Task task = new Task(() => 
            {
                while(true)
                {
                    if (cts.IsCancellationRequested)
                    {
                        capture.Release();
                        return;
                    }

                    lock (locker)
                    {
                        isProcessing = true;

                        using (Mat frame = new Mat())
                        {
                            bool success = capture.Read(frame);
                            if (frame.Empty()) continue;
                            frameCache = frame.Clone();
                        }

                        isProcessing = false;
                    }
                }
            }, cts.Token);

            Task task2 = new Task(() => 
            {
                while (true)
                {
                    if (cts2.IsCancellationRequested)
                    {
                        return;
                    }

                    if (!isProcessing && frameCache != null)
                    {
                        using (Mat frame = frameCache.Clone())
                        {
                            args.bmp = frame.ToBitmapSource();
                            OnFrameReceived(this, args);
                        }
                    }
                }
            }, cts2.Token);

            task.Start();
            task2.Start();
        }

        public void Stop()
        {
            cts2?.Cancel();
            cts2?.Dispose();

            cts?.Cancel();
            cts?.Dispose();
        }
    }
}
