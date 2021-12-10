using Meir_Tolpin_Project.Connection;
using Meir_Tolpin_Project.Controls;
using Meir_Tolpin_Project.Logs;
using Meir_Tolpin_Project.Stream;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Meir_Tolpin_Project.Interface
{
    public partial class ChooseInterface : Form
    {
        ConnectedUser parent;
        int keyboardPort, mousePort;
        Socket pictureSocket;
        TcpClient clientSocket;
        Form screen;
        NetworkStream dataStream;
        string remoteIP;
        public ChooseInterface(NetworkStream dataStream, string remoteIP,
                TcpClient clientSocket, Socket pictureSocket)
        {
            this.pictureSocket = pictureSocket;
            this.clientSocket = clientSocket;
            this.remoteIP = remoteIP;
            this.dataStream = dataStream;
            InitializeComponent();
        }

        

        private void btn0_Click(object sender, EventArgs e)
        {
            this.Hide();
            string msg;
            ConnectionHelper.tcpSend("501", this.dataStream);                                                               // sending request for connection                                                                              // opening keyboard controller 
            msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                // waiting for answer 
            if (msg == "404 Error")
            {
                Console.WriteLine("Error connecting, The server is busy");                                                    // the server is busy.  
                this.Close();
            }
            while (msg != "502")
            {
                ConnectionHelper.tcpSend("501", this.dataStream);
                msg = ConnectionHelper.tcpRecv(this.dataStream);
            }
            Console.WriteLine("Started the BroadCast Listener");
            Logger.write("User with ip: " + this.remoteIP + " has opened broadcast listener");
            this.screen = new PicStream(this.pictureSocket, null);                                                             // opening pictures screen 
            this.screen.ShowDialog();                                                                                            // showing the screen 
            this.Close();
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            this.Hide();
            string msg;
            ConnectionHelper.tcpSend("301", this.dataStream);                                                             // sending request for connection                                                                              // opening keyboard controller 
            msg = ConnectionHelper.tcpRecv(this.dataStream);                                                              // waiting to an answer (correct answer is 302)
            if (msg == "404 Error")                                                                                         // can't connect the server is busy 
            {
                Console.WriteLine("Error connecting, the server is down or busy");
                this.Close();
            }

            ConnectionHelper.tcpSend("304", this.dataStream);                                                                     // requesting a new port 
            this.keyboardPort = int.Parse(ConnectionHelper.tcpRecv(this.dataStream));                                           // recieving and saving as keyboard port
            ConnectionHelper.tcpSend("304", this.dataStream);                                                                     // requestion a new port 
            this.mousePort = int.Parse(ConnectionHelper.tcpRecv(this.dataStream));                                              // recieving and saving as mouse port 
            Controller controller = new Controller(this.clientSocket, this.keyboardPort, this.mousePort, this.remoteIP);    // creating the keyboard and mouse controller
            controller.runController();                                                                                             // starting the mouse and keyboard controller        
            while (msg != "302")                                                                                                    // error message
            {
                ConnectionHelper.tcpSend("301", this.dataStream);
                msg = ConnectionHelper.tcpRecv(this.dataStream);
            }
            Console.WriteLine("Started the Controller");
            Logger.write("User with ip: " + this.remoteIP + " has opened broadcast listener");
            this.screen = new PicStream(this.pictureSocket, controller);                                                // Creating picture Form 
            this.screen.ShowDialog();                                                                                     // opening the picture screen form 
            controller.terminate();                                                                                         // stoping the keyboard and the mouse controllers
            this.Close();
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            this.Hide();
            string msg;
            ConnectionHelper.tcpSend("401", this.dataStream);                                                               // sending transmitting request 
            msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                // waiting for answer 
            if (msg == "404 Error")
            {
                Console.WriteLine("Error connecting, The server is busy");                                                  // the server in busy or used.
                this.Close();
            }
            while (msg != "402")
            {
                ConnectionHelper.tcpSend("401", this.dataStream);
                msg = ConnectionHelper.tcpRecv(this.dataStream);
            }

            Console.WriteLine("Started the Sender");
            Logger.write("User with ip: " + this.remoteIP + " has opened transmition sender");
            while (true)
            {
                byte[] data = PicturesHelper.getScreenShot();                                                               // getting screenShot 
                // sending to the server 
                if (ConnectionHelper.picSend(data, this.pictureSocket) == -1)                                               // sending the picture 
                {
                    break;
                }
            }
            this.Close();
        }

        private void ChooseInterface_Load(object sender, EventArgs e)
        {

        }
    }
}
