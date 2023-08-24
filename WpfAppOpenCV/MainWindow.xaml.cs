using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
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
using System.Windows.Threading;
using Window = System.Windows.Window;

namespace WpfAppOpenCV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string client = "";

        rtspCamera camera;
        bool isSaving = false;

        public MainWindow()
        {
            InitializeComponent();

            camera = new rtspCamera();
            camera.OnFrameReceived += Camera_OnFrameReceived;
        }

        private void Camera_OnFrameReceived(object? sender, EventArgs e)
        {
            image.Dispatcher.Invoke(delegate 
            { 
                image.Source = ((FrameArgs)e).bmp; 
            });

        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            client = $"rtsp://{tbxIp.Text}:{tbxPort.Text}/avix1";

            camera.Start(client);
        }

        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            camera.Stop();
        }
    }
}
