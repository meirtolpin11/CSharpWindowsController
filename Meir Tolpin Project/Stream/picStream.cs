using Meir_Tolpin_Project.Connection;
using Meir_Tolpin_Project.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Meir_Tolpin_Project.Stream
{
    public partial class PicStream : Form
    {
        Socket pictureSocket;
        Controller controller;

        /**
         * The Form's constractor 
         * input: 
         *      Socket PictureSocket: the TCP socket for image stream
         * output:
         *      None
         */
        public PicStream(Socket PictureSocket, Controller controller)
        {
            InitializeComponent();
            this.pictureSocket = PictureSocket;
            this.controller = controller;
        }


        /** Set the new image to the form 
         * input:
         *      Bitmap img: the new image
         * output:
         *      None
         */
        public void setImage(Bitmap img)
        {
            this.Size = new Size(img.Size.Width+50, img.Size.Height + 50);
            this.streamPic.Size = Screen.PrimaryScreen.Bounds.Size;
         
            this.streamPic.Image = img;
            
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }


        /** Recieving data from the opened UDP socket for pictures
         * input:
         *      none
         * output:
         *      the recieved data from the socket 
         */
        private byte[] picRecv()
        {
            Socket s = this.pictureSocket;
            int total = 0;
            int recv;
            byte[] datasize = new byte[4];

            try
            {
                recv = s.Receive(datasize, 0, 4, 0);
                int size = BitConverter.ToInt32(datasize, 0);
                int dataleft = size;
                byte[] data = new byte[size];


                while (total < size)
                {
                    recv = s.Receive(data, total, dataleft, 0);
                    if (recv == 0)
                    {
                        break;
                    }
                    total += recv;
                    dataleft -= recv;
                }
                return data;
            }
            catch
            {
                return null; 
            }
            
        }


        /** Updates the image on the screen every 1 miliSecond */
        private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap bmp;
            byte[] data = this.picRecv();
            if(data == null)
            {
                this.timer1.Stop();
                this.pictureSocket.Close();
                this.Close();
                
            }
            else
            {
                using (var ms = new MemoryStream(data))
                {
                    bmp = new Bitmap(ms);
                }
                this.setImage(bmp);
            }
            
        }


        private void PicStream_Load(object sender, EventArgs e)
        {
            this.timer1.Start();
        }

        private void streamPic_Click(object sender, EventArgs e)
        {

        }

        private void PicStream_FormClosed(object sender, FormClosedEventArgs e)
        {
         
        }

        private void PicStream_FormClosing(object sender, FormClosingEventArgs e)
        {
         
        }
    }
    }

