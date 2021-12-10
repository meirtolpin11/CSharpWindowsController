using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MouseKeyboardLibrary;
using System.Net.Sockets;
using System.Threading;
using Meir_Tolpin_Project.Connection;

namespace Meir_Tolpin_Project.Controls
{
    public partial class Controller : Form
    {
        TcpClient mainConnection, stream, mouseClient;
        TcpListener server;
        NetworkStream keyboardStream, mouseStream;
        Thread listenerThread;
        int port, mport;
        string ip;
        MouseHook mouseHook = new MouseHook();
        KeyboardHook keyboardHook = new KeyboardHook();

        public Controller(TcpClient connection, int port, int mport, string remoteIP)
        {
            this.mainConnection = connection;                       // the main data socket, not sure if will use it 
            this.port = port;                                       // the port for the new connection
            this.mport = mport; 
            this.ip = remoteIP;                                     // the ip of remote server 
            InitializeComponent();
        }

        private void Keyboard_Load(object sender, EventArgs e)
        {
            /* 
               mouseHook.MouseMove += new MouseEventHandler(mouseHook_MouseMove);
               mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
               mouseHook.MouseUp += new MouseEventHandler(mouseHook_MouseUp);
               mouseHook.MouseWheel += new MouseEventHandler(mouseHook_MouseWheel);
               mouseHook.Start();
            */
            SetXYLabel(MouseSimulator.X, MouseSimulator.Y);
        }

        /** 
         * Starting the mouse and keyboard controllers 
         * to terminate call "terminate" function
         */
        public void runController()
        {
            this.startKeyboardController();
            this.startMouseController(); ;
        }

        /** 
         * Starting the mouse and keyboard listeners 
         * to terminate call "terminate function"
         */
        public void runListenter()
        {
            this.startKeyboardListener();
            this.startMouseListerner();
        }

        /** 
         * Terminates both controllers and losteners 
         */
        public void terminate()
        {
            mouseHook.Stop();
            keyboardHook.Stop();
            this.stop = true;
            this.mouseStream.Close();
            this.keyboardStream.Close();
            this.Close();
        }

        /* KEYBOARD CONTROLLER */

        /**
         * starting the Keyboard controller and opening a TcpClient socket to the server
         * input:
         *      none
         * output:
         *      none
         */
        private void startKeyboardController()
        {
            stream = new TcpClient(ip, port);               // opening connection to the server 
            this.keyboardStream = stream.GetStream();        // getting connection stream

            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);
            keyboardHook.Start();                                                           // starting the hook 
        }

        private void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            Keys _key = e.KeyCode;
            string key = _key.ToString();
            ConnectionHelper.tcpSend((char)3 + key, this.keyboardStream);                                  // sending the key 
        }

        private void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            Keys _key = e.KeyCode;
            string key = _key.ToString();
            ConnectionHelper.tcpSend((char)2 + key, this.keyboardStream);                                  // sending the key 
        }

       



        /** starting the Keyboard simulator and creating a server socket 
         * input:
         *      none
         * output:
         *      none
         */
        private void startKeyboardListener()
        {
            try
            {
                server = new TcpListener(port);                  // creating server socket 
                server.Start();
                stream = server.AcceptTcpClient();               // accepting client (only one client)
                this.keyboardStream = stream.GetStream();         // getting connection stream 
                listenerThread = new Thread(listener);           // staring the listener as a thread
                listenerThread.Start();                          // starting the thread    
            }
            catch {}
        }



        private List<string> parseKeys(string key)
        {
            List<string> keys = new List<string>(); 
            string temp;
            int index1, index2, index3, index = -1; ;
            do
            {
                index = -1;
                // UasdPasdUasd 
                index1 = key.IndexOf((char)2);
                index2 = key.IndexOf((char)3);
                if (index1 == -1 && index2 != -1)
                {
                    index = index2;
                }
                else if (index2 == -1 && index1 != -1)
                {
                    index = index1;
                }
                else
                {
                    index = Math.Min(index1, index2);
                }
                if (index != -1)
                {
                    temp = key.Substring(index);

                    index3 = -1;
                    index1 = key.Substring(1).IndexOf((char)2);
                    index2 = key.Substring(1).IndexOf((char)3);
                    if (index1 == -1 && index2 != -1)
                    {
                        index3 = index2;
                    }
                    else if (index2 == -1 && index1 != -1)
                    {
                        index3 = index1;
                    }
                    else
                    {
                        index3 = Math.Min(index1, index2);
                    }

                    if (index3 != -1)
                    {
                        temp = temp.Substring(0, index3 + 1);
                        keys.Add(temp);
                        key = key.Substring(index3+1); 
                    }
                    else
                    {
                        keys.Add(temp);
                        return keys;
                    }

                }
            } while (index != -1);
            return keys;
        }

        bool stop = false;
        /** 
         * Keyboard simulator
         * recieving the key from the server and simulates it 
         */
        private void listener()
        {
            List<string> keys;
            string key;                                         // the string that represents the key 
            Keys a;
            while (this.stop==false)
            {
                key = ConnectionHelper.tcpRecv(this.keyboardStream);                           // recieving the key 
                if(key == null)
                {
                    continue;
                }
                keys = parseKeys(key);
                for (int i = 0; i < keys.Count(); i++)
                {
                    key = keys[i];
                    if (key[0] == (char)2)
                    {
                        if (char.IsDigit(key[1]))
                        {
                            a = (Keys)Enum.Parse(typeof(Keys), "NumPad" + key[1]);
                            KeyboardSimulator.KeyDown(a);
                        }
                        else
                        {
      
                            a = (Keys)Enum.Parse(typeof(Keys), key.Substring(1));
                            KeyboardSimulator.KeyDown(a);
                        }

                    }
                    else if (key[0] == (char)3)
                    {
                        if (char.IsDigit(key[1]))
                        {
                            a = (Keys)Enum.Parse(typeof(Keys), "NumPad" + key[1]);
                            KeyboardSimulator.KeyUp(a);
                        }
                        else
                        {
                            a = (Keys)Enum.Parse(typeof(Keys), key.Substring(1));
                            KeyboardSimulator.KeyUp(a);
                        }

                    }
                }
            }
        }

        void SetXYLabel(int x, int y)
        {

            curXYLabel.Text = String.Format("Current Mouse Point: X={0}, y={1}", x, y);

        }

        
        /* KEYBOARD CONTROLLER END */

        /* MOUSE CONTROLLER */
        private void startMouseController()
        {
            this.mouseClient = new TcpClient(ip, mport);               // opening connection to the server 
            this.mouseStream = mouseClient.GetStream();        // getting connection stream

            mouseHook.MouseMove += new MouseEventHandler(mouseHook_MouseMove);
            mouseHook.MouseDown += new MouseEventHandler(mouseHook_MouseDown);
            mouseHook.Start();
        }

        private void mouseHook_MouseDown(object sender, MouseEventArgs e)
        {
            ConnectionHelper.tcpSend(((char)1).ToString(), this.mouseStream);
        }

        private void mouseHook_MouseMove(object sender, MouseEventArgs e)
        {
            string cor;
        //    Point p = this.PointToClient(new Point(e.X, e.Y));
         //   cor = "X" + p.X.ToString() + "Y" + p.Y.ToString() + "Z";
            cor = "X" + e.X.ToString() + "Y" + e.Y.ToString() + "Z";
            ConnectionHelper.tcpSend(cor, this.mouseStream);
        }

        private void mouseListener()
        {
            string msg;
            while (true)
            {
                msg = ConnectionHelper.tcpRecv(this.mouseStream);
                if(msg==null)
                {
                    break;
                }
                if (msg == ((char)1).ToString())
                {
                    MouseSimulator.Click(MouseButton.Left);
                }
                else
                {
                    string[] pos = msg.Split('Z');
                    foreach (string s in pos)
                    {
                        if (s.Length > 1)
                        {
                            string x, y;
                            x = s.Substring(1, msg.IndexOf("Y") - 1);
                            y = s.Substring(msg.IndexOf("Y") + 1);

                            try
                            {
                                MouseSimulator.X = int.Parse(x);
                                MouseSimulator.Y = int.Parse(y);
                            }
                            catch
                            { }
                        }

                    }

                }

            }
        }

        private void startMouseListerner()
        {
            try
            {
                server = new TcpListener(mport);                  // creating server socket 
                server.Start();
                this.mouseClient = server.AcceptTcpClient();               // accepting client (only one client)
                this.mouseStream = this.mouseClient.GetStream();         // getting connection stream 
                listenerThread = new Thread(mouseListener);           // staring the listener as a thread
                listenerThread.Start();                          // starting the thread   
            }
            catch { }
        }


        /* MOUSE CONTROLLER END */





        private void Keyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.keyboardHook.Stop();
            this.mouseHook.Stop();
            this.stop = true;
        }

        private void Keyboard_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Not necessary anymore, will stop when application exits

            mouseHook.Stop();
            keyboardHook.Stop();
            this.stop = true;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

       
    }
}
