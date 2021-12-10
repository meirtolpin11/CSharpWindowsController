using Meir_Tolpin_Project.Connection;
using Meir_Tolpin_Project.Controls;
using Meir_Tolpin_Project.Interface;
using Meir_Tolpin_Project.Logs;
using Meir_Tolpin_Project.Stream;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Meir_Tolpin_Project
{
    class ConnectedUser
    {
        TcpClient clientSocket;                                                                         // socket to accept user connections 
        Socket pictureSocket;                                                                           // socket to transfer and recieve pictures stream
        NetworkStream dataStream;                                                                       // the main network stream 
        Form screen;                                                                                    // Pictures stream form (screen)
        string remoteIP = "127.0.0.1";                                                                  // the remote IP of the server to connect 
        int keyboardPort, mousePort, picPort;                                                            // the keyboardPort to connect to 
        Thread listener;                                                                                // the server's listener thread 
        string username = "000";                                                                        // the name of the computer, as default "000"
        ListeningServer parent;
        static bool controlled = false, recieving = false;

        /**
         * The constractor of ConnectedUser class 
         * input:
         *      TcpClient socket - the socket of the server connection
         * output:
         *      none
         */
        public ConnectedUser(string username, TcpClient socket, ListeningServer parent)
        {
            this.clientSocket = socket;
            dataStream = clientSocket.GetStream();
            this.username = username;
            this.parent = parent;
        }



        /** Opens a connection to the other client by the Protocol 
         * 101 - connection request 
         * 201 - Picture UDP socket creation request 
         * 301 - controlling request 
         * 401 - transmition request
         * 501 - recieving request 
         * input:
         *      none
         * output:
         *      none
         */
        public int openConnection()
        {
            int userChoice;
            // The 'handshake' with the server 
            ConnectionHelper.tcpSend("101", this.dataStream);                                                                     // sending connection request code
            string msg = ConnectionHelper.tcpRecv(this.dataStream);                                                               // waiting to the answer from the other client
            while(msg!="102")                                                                                                     // if the answer is not OK code
            {
                if (msg == "404 Error")
                {
                    // recieved an error from the server, the server is busy
                    Console.WriteLine("Error connecting, The server is busy");              
                    return -1;
                }
                ConnectionHelper.tcpSend("101", this.dataStream);                                                                 // sending again while we will recieve OK code 
                msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                  // recieving an answer from the other client 
            }

            // Sending the name to the server and requesting picture socket keyboardPort 
            ConnectionHelper.tcpSend(this.username, this.dataStream);                                                             // sending my username to the server 
            ConnectionHelper.tcpRecv(this.dataStream);                                                                            // recieving confirmation code
            ConnectionHelper.tcpSend("201", this.dataStream);                                                                     // sending Socket opening request code
            msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                      // waiting for answer 
            while(msg!="202")                                                                                                     // if the answer is not OK, sending the request again 
            {                                                                                       
                ConnectionHelper.tcpSend("201", this.dataStream);
                msg = ConnectionHelper.tcpRecv(this.dataStream);
            }
            

            // creating UDP socket for Video sending
            msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                       // getting the picture Port from the client
            this.picPort = int.Parse(msg); 
            this.remoteIP = ((IPEndPoint)(this.clientSocket.Client.RemoteEndPoint)).Address.ToString();                            // getting the ip of the server
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(this.remoteIP), this.picPort);                                        // creating IPEndPoint for socket creation
            this.pictureSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);                      // creating socket to the server

            // End of UDP socket creation 
            Logger.write("Opened a socket to the server on keyboardPort: " + this.picPort);
            try
            {
                this.pictureSocket.Connect(ipep);                                                                                  // trying to connect to the server 
            }
            catch (SocketException e)
            {
                Console.WriteLine("Unable to connect to server.");                                                                 // error was occured, connection faild
                Console.WriteLine(e.ToString());                                                                                   // printing the error 
                Console.ReadLine();
            }

            ChooseInterface choose = new ChooseInterface(this.dataStream, this.remoteIP, this.clientSocket, this.pictureSocket);
            choose.ShowDialog();

            this.pictureSocket.Close();                                                                                             // closing picture socket 
            this.clientSocket.Close();                                                                                              // closing client data socket 
            ConnectionInterface form = new ConnectionInterface();                                                                                                         // starting the main again 
            return 0;                                                                                                           
        }


        /**
         * Incoming connection handler
         * input:
         *      none
         * output:
         *      none
         */
        private void ConnectionListener()
        {
            Socket pictureServerSocket, client;
            // handshake with the client 
            string msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                  // getting a msg from the potential client 
            while (msg != "101")                                                                                                     // waiting the 101 message - connection request 
            {
                // wrong message 
                ConnectionHelper.tcpSend("103", this.dataStream);
                msg = ConnectionHelper.tcpRecv(this.dataStream);
            }
            ConnectionHelper.tcpSend("102", this.dataStream);                                                                        // connection established 

            // recieving the name from the client 
            username = ConnectionHelper.tcpRecv(this.dataStream);                                                                    // getting the username of the client 
            ConnectionHelper.tcpSend("200OK", this.dataStream);

            msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                         // getting the next message
            while (msg != "201")                                                                                                     // waiting to 201 message - picture socket creationg
            {
                ConnectionHelper.tcpSend("203", this.dataStream);
                msg = ConnectionHelper.tcpRecv(this.dataStream);
            }

            picPort = ConnectionHelper.getPort();                                                                                    // getting next free port 

            while (true)
            {
                // trying to open a socket 
                try
                {
                    IPEndPoint ipep = new IPEndPoint(IPAddress.Any, picPort);
                    pictureServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    pictureServerSocket.Bind(ipep);
                    pictureServerSocket.Listen(10);

                    ConnectionHelper.tcpSend("202", this.dataStream);                                                               // sending confirmation that the socket has opened successfully 
                    ConnectionHelper.tcpSend(picPort.ToString(), this.dataStream);                                                  // sending the port number 
                    client = pictureServerSocket.Accept();
                    this.pictureSocket = client;
                    break;
                }
                catch
                {
                    picPort = ConnectionHelper.getPort();
                    ConnectionHelper.tcpSend("203", this.dataStream);
                }
            }


            this.remoteIP = ((IPEndPoint)(this.clientSocket.Client.RemoteEndPoint)).Address.ToString();                             // getting the ip of the server  
            Logger.write("Opened a socket to User ip: " + this.remoteIP + " on keyboardPort: " + keyboardPort);
            msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                        // getting the next message code 
            while (msg != "301" && msg != "401" && msg != "501")
            {
                ConnectionHelper.tcpSend("303", this.dataStream);
                msg = ConnectionHelper.tcpRecv(this.dataStream);                                                                    // getting the next message code 
            }

            if (msg == "301")
            {
                // User want to control this pc 
                if(controlled || recieving)
                {
                    // the pc can't be controlled 
                    ConnectionHelper.tcpSend("404 Error", this.dataStream);
                }
                else
                {
                    // setting mode to controlled 
                    controlled = true;
                    // activate the sender 
                    ConnectionHelper.tcpSend("302", this.dataStream);   
                    ConnectionHelper.tcpRecv(this.dataStream);
                    keyboardPort = ConnectionHelper.getPort();                                                                      // getting the next free port for keyboard data 
                    ConnectionHelper.tcpSend(keyboardPort.ToString(), this.dataStream);                                             // sending the keyboard port 
                    ConnectionHelper.tcpRecv(this.dataStream);                                                                      // recieving port request 
                    mousePort = ConnectionHelper.getPort();                                                                         // getting the next free port for mouse data 
                    ConnectionHelper.tcpSend(mousePort.ToString(), this.dataStream);                                                // sending the mouse port 
                    Controller listener = new Controller(this.clientSocket, this.keyboardPort, this.mousePort, this.remoteIP);      // creating the listener form 
                    listener.runListenter();                                                                                        // starting the listener 
                    Logger.write("Activating sender to User with ip: " + this.remoteIP);                                
                    while (true)
                    {
                        byte[] data = PicturesHelper.getScreenShot();                               // getting screenShot 
                                                                                                    // sending to the server 
                        if (ConnectionHelper.picSend(data, this.pictureSocket) == -1)   
                        {
                            break;
                        }

                    }
                    listener.terminate();                                                                                           // stoping the listener 
                    controlled = false;                                                                                             // setting the mode to not controlled 
                }        
            }
            if (msg == "401")
            {
                // the used decieded to transmit to this computer 
                if(controlled || recieving)
                {
                    ConnectionHelper.tcpSend("404 Error", this.dataStream);       // cant recieve if controlled
                }
                else
                {
                    recieving = true;                                                                                               // setting to transmitting mode 
                    // activate the listener    
                    ConnectionHelper.tcpSend("402", this.dataStream);
                    Logger.write("Activating listener to User with ip: " + this.remoteIP);
                    screen = new PicStream(this.pictureSocket, null);                                                               // creating picture form 
                    screen.ShowDialog();                                                                                            // showing the picture from 
                    this.pictureSocket.Close();                                                                                     // closing the picture socket 
                    parent.startServer();                                                                                           // starting the server again 
                    recieving = false;                                                                                              // setting mode to regular 
                }
            
            }
            if (msg == "501")
            {
                // decieded to recieve video 
                // activate the sender 
                ConnectionHelper.tcpSend("502", this.dataStream);

                
                Logger.write("Activating sender to User with ip: " + this.remoteIP);
                while (true)
                {
                    byte[] data = PicturesHelper.getScreenShot();                               // getting screenShot 
                                                                                                // sending to the server 
                    if (ConnectionHelper.picSend(data, this.pictureSocket) == -1)
                    {
                        break;
                    }

                }
            }
            
        }
         

        /** 
         * starting the incoming connection handler 
         * input:
         *      none
         * output:
         *      return - true if success, else false 
         */
        public bool startListerner()
        {
            try
            {
                this.listener = new Thread(this.ConnectionListener);                                            // creating listener thread 
                listener.Start();                                                                               // starting the thread 
                return true;
            }
            catch
            {
                return false;
            }
        }


        /** stopping the incoming connection handler 
         * input:
         *      none
         * output:
         *      return - true if stopped, else false
         */
        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public bool terminateListener()
        {
            try
            {
                this.pictureSocket.Close();                                                                     // closing the sockets
                this.clientSocket.Close(); 
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
