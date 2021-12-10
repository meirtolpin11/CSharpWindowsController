using Meir_Tolpin_Project.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using System.Text;
using System.Threading;


namespace Meir_Tolpin_Project
{

    // Here I listen to the incoming connections and requests 
    class ListeningServer
    {
        private TcpListener serverSocket;                                      // MultiThreading server's listening socket 
        private TcpClient clientSocket;                                        // Temp socket for every new connected user 
        private Thread server;                                                 // the MultiThreading server's Thread
        private int counter = 0;                                               // counter of connected users 

        private void start()
        {
            string ip;
            try
            {
                this.serverSocket = new TcpListener(8888);                      // creating the Server TCP socket 
                this.clientSocket = default(TcpClient);                         // creating the User socket variable  

                // starting the server
                this.serverSocket.Start();

                Console.WriteLine("->> server started");                        // printing to log 

                // for always 
                while (true)
                {
                    counter += 1;                                               // new user 
                    this.clientSocket = this.serverSocket.AcceptTcpClient();    // accepting user 
                    Console.WriteLine(" ->> User connected");                   // printing to log 
                    // User creatinon
                    ConnectedUser user = new ConnectedUser("", this.clientSocket,this);
                    ip =  ((IPEndPoint)(this.clientSocket.Client.RemoteEndPoint)).Address.ToString();
                    Logger.write("User with ip:" + ip + " has been connected");
                    user.startListerner();
                }

                this.clientSocket.Close();
                this.serverSocket.Stop();
                Console.WriteLine(" >> " + "exit");
                Console.ReadLine();
            }
            catch (SocketException x)
            {
                if(x.ErrorCode == 10004)
                {
                    // The socket has been closed and that was the reason for server terminating 
                    Console.WriteLine("The server has been closed");
                }
                else
                {
                    // The default port of the server is used, so the server can't bind on that port
                    Console.WriteLine("Error starting the server");
                }
            }
            
        }

        

        /*
         * Starting the server as a thread
         * return: True if successed, else false
         */
        public bool startServer()
        {
            try
            { 
                this.server = new Thread(start);
                server.Start();
                Logger.write("Server Started");
                return true;
            }
            catch
            {
                return false;
            }
            
        }


        /*
         * Terminating the server
         * return: True if successed, else false
         */
        [SecurityPermissionAttribute(SecurityAction.Demand, ControlThread = true)]
        public bool terminateServer()
        {
            try
            {
                this.serverSocket.Stop();       // closing the server socket 
                Logger.write("Server Terminated");
                return true;
            }
            catch
            {
                return false; 
            }
        }

    }
}
