using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace UDP_module
{
    public partial class Form1 : Form
    {
        private UdpClient client;
        private UdpClient server;
        private string ori;
        private string dst;
        private string text;
        private int count;
        private static int port;
        private static string ip = "127.0.0.1";
        public Form1()
        {
            ori = "Bochao  ";
            dst = "Botang   ";
            text = "";
            count = 0;
            InitializeComponent();
            port = 8000;
            server_init();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        /********** server *********/

        private void server_init() {
            this.server = new UdpClient(port);
            Thread server_thread = new Thread(new ThreadStart(server_start));
            server_thread.Start();
        }

        private void server_start() {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, port);
            while (true) {
                
                byte[] data = server.Receive(ref ipep);
                if (data.Length == 0) continue;
                else {
                    text = text +"\r\n"+dst+"1\r\n"+Encoding.ASCII.GetString(data);
                    this.textBox1.Invoke(new Action(() => { this.textBox1.Text = text; }));
                }
            }
        }

        /********** client *********/
        private void button4_Click(object sender, EventArgs e)
        {
            if (count == 0) {
                count++;
                client = new UdpClient();
                client.Connect(ip, port);
            }
            string tmp = textBox2.Text;
            text = text + "\r\n" + ori + "2\r\n" + tmp;
            textBox1.Text = text;
            byte[] msg = Encoding.ASCII.GetBytes(tmp);
            client.Send(msg, msg.Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            videochat md = new videochat();
            md.Show();
        }
        

    }
}
