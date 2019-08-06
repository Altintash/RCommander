using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemComClient
{
    public partial class Client : Form
    {
        private Socket _socket;
        public Client()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                string ip;
                ip = textBox1.Text;
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.BeginConnect(ip, 6099, new AsyncCallback(ConnectCallBack), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " Button");
            }

        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                _socket.EndConnect(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " ConnectCallBack");
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] _buffer = Encoding.ASCII.GetBytes(richTextBox1.Text); //Манипуляции с ричбоксами делать надо
                listBox1.Items.Add(richTextBox1.Text);
                richTextBox1.Clear();
                _socket.BeginSend(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " on send");
            }
        }

        private void SendCallBack(IAsyncResult ar)
        {
            try
            {
                _socket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " SendCallBack");
            }
        }
    }
}
