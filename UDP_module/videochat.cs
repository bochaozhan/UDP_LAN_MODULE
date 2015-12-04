using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WebCam_Capture;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Speech;
using System.Speech.Synthesis;
namespace UDP_module
{
    public partial class videochat : Form
    { 
        
        private WebCam webcam;
        private UdpClient server;
        private UdpClient client;
        private static int port = 5000;
        private static string ip = "127.0.0.1";
        public videochat()
        {
            InitializeComponent();
            server_init();
        }
        // open source code from https://easywebcam.codeplex.com/ 
        // //Design by Pongsakorn Poosankam
        class WebCam
        {
            private WebCamCapture webcam;
            private System.Windows.Forms.PictureBox _FrameImage;
            private int FrameNumber = 30;
            public void InitializeWebCam(ref System.Windows.Forms.PictureBox ImageControl)
            {
                webcam = new WebCamCapture();
                webcam.FrameNumber = ((ulong)(0ul));
                webcam.TimeToCapture_milliseconds = FrameNumber;
                webcam.ImageCaptured += new WebCamCapture.WebCamEventHandler(webcam_ImageCaptured);
                _FrameImage = ImageControl;
            }

            void webcam_ImageCaptured(object source, WebcamEventArgs e)
            {
                    _FrameImage.Image = e.WebCamImage;
            }

            public void Start()
            {
                webcam.TimeToCapture_milliseconds = FrameNumber;
                webcam.Start(0);
            }

            public void Stop()
            {
                webcam.Stop();
            }

            public void Continue()
            {
                // change the capture time frame
                webcam.TimeToCapture_milliseconds = FrameNumber;

                // resume the video capture from the stop
                webcam.Start(this.webcam.FrameNumber);
            }

            public void ResolutionSetting()
            {
                webcam.Config();
            }

            public void AdvanceSetting()
            {
                webcam.Config2();
            }

        }



        private void videochat_Load(object sender, EventArgs e)
        {
            webcam = new WebCam();
            webcam.InitializeWebCam(ref pictureBox2);
            webcam.Start();
            server_init(); 
        }


        //server

        private void server_init()
        {
            this.server = new UdpClient(port);
            Thread server_thread = new Thread(new ThreadStart(server_start));
            server_thread.Start();
        }



        private void server_start()
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            while (true)
            {
                try
                {
                    byte[] data = server.Receive(ref ipep);
                    if (data.Length == 0) continue;
                    else
                    {
                        MemoryStream ms = new MemoryStream(data);
                        Image tmp = Image.FromStream(ms);
                        this.pictureBox1.Invoke(new Action(() => { this.pictureBox1.Image = tmp; }));
                    }
                }
                catch{}

            }
        }



        // client
        private void button1_Click(object sender, EventArgs e)
        {
                this.button1.Enabled = false;
                client = new UdpClient();
                client.Connect(ip, port);
                Thread client_thread = new Thread(new ThreadStart(client_start));
                client_thread.Start();
        }

        private void client_start() {
            while (true)
            {
                Image tmp = null;
                MemoryStream ms = new MemoryStream();
                try
                {
                    this.pictureBox2.Invoke(new Action(() => { tmp = (Image)pictureBox2.Image.Clone(); }));
                    tmp.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
                    byte[] msg = ms.ToArray();
                    client.Send(msg, msg.Length);
                }
                catch { 
                }
            }
        
        }

    }
}
