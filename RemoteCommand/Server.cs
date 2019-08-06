using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteCommand
{
    public partial class Server : Form
    {
        public Socket _socket, _clsocket;
        public byte[] _buffer;
        public byte[] _feedback;
        public Server()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            startServer();
        }

        public void startServer()
        {
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, 6099));
                _socket.Listen(10);
                _socket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
            }
            catch (SocketException) { }
            catch
            {

            }
        }

        private void AcceptCallBack(IAsyncResult ar)
        {
            try
            {
                _clsocket = _socket.EndAccept(ar);
                _buffer = new byte[_clsocket.ReceiveBufferSize];
                _clsocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " AcceptCallBack");
            }
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int received = _clsocket.EndReceive(ar);
                Array.Resize(ref _buffer, received);
                string text = Encoding.ASCII.GetString(_buffer);
                appendToRichBox(text);
                string command = text;

                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.Arguments = "/C " + command;
                cmd.Start();
                //cmd.Start("cmd.exe", "/C " + command);
                string cmdtext = cmd.StandardOutput.ReadToEnd();

                
                //feedback cmd
                //_socket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), null);

                Array.Resize(ref _buffer, _clsocket.ReceiveBufferSize);
                _clsocket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), null);
                
                //richTextBox1.Text = cmdtext;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " ReceiveCallBack");
                _socket.Close();
            }
        }

        /*public string feedBack()
        {
            string cmdtext;
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = "/any /arguments /if /needed";
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.Start();

            cmdtext = cmd.StandardOutput.ReadToEnd();

            cmd.WaitForExit();
            return cmdtext;
        }*/

        public void appendToRichBox(string text)
        {
            MethodInvoker invoker = new MethodInvoker(delegate
            {
                richTextBox1.Text += text + "\r\n";
            });
            this.Invoke(invoker);
        }
    }
}
