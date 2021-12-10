using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Meir_Tolpin_Project.Connection
{
    static class ConnectionHelper
    {

        private static int port = 10000;                                                    // the first port to use 


        /*
         * returning the next unused port
         */
        public static int getPort()
        {
            port += 1;
            return port;
        }

        /**
         * This function sends the msg to the opened keyboardStream of the class
         * input:
         *      string msg - the message to tcpSend 
         * output:
         *      return - none
         */
        public static void tcpSend(string msg, NetworkStream stream)
        {
            try
            {
                Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);                            // converting string to bytes
                stream.Write(sendBytes, 0, sendBytes.Length);                               // sending the data over the socket 
                stream.Flush();                                                             // clearing the socket 
            }
            catch
            {
                Console.WriteLine("Error sending Data");
            }

        }


        /** This function recieves messages from the messages Stream
         * input:
         *      none
         * output:
         *      return - the message
         */
        public static string tcpRecv(NetworkStream stream)
        {
            try
            {
                byte[] buffer = new byte[2048];                                                 // the recieving buffer 
                stream.Read(buffer, 0, 2048);                                                   // reading to the buffer 
                string request = Encoding.ASCII.GetString(buffer, 0, buffer.Length);            // converting bytes to string 
                request = request.Replace("\0", string.Empty);                                  // clearing the string 
                return request;                                                                 // returning the string 
            }
            catch
            {
                Console.WriteLine("Error recieving data");
                return null;
            }

        }

        /** Sending data to opened UDP socket for pictures
        * input: 
        *      Byte[] sendBytes - the data to send 
        * output:
        *      return - none
        */
        public static int picSend(byte[] data, Socket pictureSocket)
        {
            try
            {
                Socket s = pictureSocket;                                                               // getting the socket to use 
                int total = 0;                                                                          // total sent data 
                int size = data.Length;                                                                 // total data to send
                int dataleft = size;                                                                    // how many data left to send 
                int sent;                                                                               // how many data sent this time

                byte[] datasize = new byte[4];                                                          // represents the lenght of the data
                datasize = BitConverter.GetBytes(size);                                                 // getting the lenght of the data
                sent = s.Send(datasize);                                                                // sending the data     

                // while there is unsent data
                while (total < size)        
                {
                    sent = s.Send(data, total, dataleft, SocketFlags.None);                             // sending the data (max that we can)
                    total += sent;                                                                      // updating the total sent data size 
                    dataleft -= sent;                                                                   // updating how much data left to sent 
                }
                return total;                                                                           // returning the lenght of sent data 
            }
            catch
            {
                return -1;
            }
        }


        /** Recieving data from the opened UDP socket for pictures
         * input:
         *      none
         * output:
         *      the recieved data from the socket 
         */
        public static byte[] picRecv(Socket pictureSocket)
        {

            Socket s = pictureSocket;                                           // the socket to read from 
            int total = 0;                                                      // total recieved data 
            int recv;                                                           // data recieved at one time
            byte[] datasize = new byte[4];                                      // represents the size of the data
            try
            {
                recv = s.Receive(datasize, 0, 4, 0);                            // getting the lenght of the data 
                int size = BitConverter.ToInt32(datasize, 0);                   // converting the lenght of data to int 
                int dataleft = size;                                            // updating how much data left to recieve 
                byte[] data = new byte[size];                                   // creating array that represents the data 

                // while there is data to recieve 
                while (total < size)
                {
                    recv = s.Receive(data, total, dataleft, 0);                 // recieving new data 
                    if (recv == 0)                                              // if no data recieved 
                    {
                        break;
                    }
                    total += recv;                                              // updating the total data 
                    dataleft -= recv;                                           // updating how much data left 
                }
                return data;
            }
            catch
            {
                return null;
            }

        }
    }
}
