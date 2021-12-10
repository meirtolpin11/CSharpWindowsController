using Meir_Tolpin_Project.Logs;
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
    public partial class ConnectionInterface : Form
    {
        public ConnectionInterface()
        {
            InitializeComponent();
        }

        /*string remoteIP;
            ListeningServer server = new ListeningServer();
            server.startServer();
            Console.WriteLine("Type any key to connect to the other client");
            Console.ReadKey();
            server.terminateServer();
            Console.WriteLine("Enter the IP of remote computer:");
            remoteIP = Console.ReadLine();
            TcpClient client = new TcpClient(remoteIP, 8888);
            ConnectedUser user = new ConnectedUser(client, null);
            user.openConnection();*/

        ListeningServer server;
        private void ConnectionInterface_Load(object sender, EventArgs e)
        {
            server = new ListeningServer();
            server.startServer();
            this.lblServerStatus.Text = "Running";
            this.lblServerStatus.BackColor = Color.Green;
            this.btnConnect.Enabled = false;
            this.btnStopServer.Enabled = true;
            this.txtIP.Enabled = false;
            this.txtUsername.Enabled = false;
            Logger.init("log.txt");
        }

        public void again()
        {
            this.Show();
            server = new ListeningServer();
            server.startServer();
            this.lblServerStatus.Text = "Running";
            this.lblServerStatus.BackColor = Color.Green;
            this.btnConnect.Enabled = false;
            this.btnStopServer.Enabled = true;
            this.txtIP.Enabled = false;
            this.txtUsername.Enabled = false;
            Logger.init("log.txt");
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            server.terminateServer();
            this.lblServerStatus.Text = "Stopped";
            this.lblServerStatus.BackColor = Color.Red;
            this.btnStopServer.Enabled = false;
            this.btnConnect.Enabled = true;
            this.txtIP.Enabled = true;
            this.txtUsername.Enabled = true;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.Hide();
            string remoteIP, username;
            remoteIP = this.txtIP.Text;
            username = this.txtUsername.Text;
            // check that all the data is legal 
            TcpClient client = new TcpClient(remoteIP, 8888);
            ConnectedUser user = new ConnectedUser(username, client, null);
            user.openConnection();
            again();

        }

    }
}
