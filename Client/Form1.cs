using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnReceive_Click(object sender, EventArgs e)
        {
            // ��������� ����� � ������� 
            IPAddress address = IPAddress.Parse(textBoxIP.Text);
            IPEndPoint endPoint = new IPEndPoint(address, 1024);

            // ��������� ��������� ������ �� ������ �볺���
            Socket client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                // ����� ��������� �'�������
                // ϳ��������� ������ - � ��������� ���������� ������ �����
                client_socket.Connect(endPoint);

                // �������� ����� �������� �� ���� �� ����� ���������� �볺��
                if (client_socket.Connected)
                {
                    // ³������� ����� � ��� ������� ����������
                    string querry = "GET\r\n\r\n";
                    client_socket.Send(Encoding.Default.GetBytes(querry));

                    // ��������� ��������� ������, �� ��������������������� ��� �������� ��� �� �������
                    byte[] buffer = new byte[1024];

                    // �����, �� ���������� ����� - ������� ��������� �����
                    int len;

                    // ���� ��� ��������� ����� - ������������ �� �������� ��������� �����
                    // - ���� ���������� ������ Available > 0 - ��� ������� � �� ����� ��������
                    do
                    {
                        // ��������� �����
                        len = client_socket.Receive(buffer);
                        // �������� ���������� ����������� (�������) �� ���������� ���������� ���� � ��������� ������������(���������� ������������� ��������� �� ������)
                        textBoxMessage.Text += Encoding.Default.GetString(buffer, 0, len);

                    } while (client_socket.Available > 0);
                    textBoxMessage.Text += "----------------------------\r\n";
                }
                else
                {
                    MessageBox.Show("Error connection !");
                }

            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client_socket.Shutdown(SocketShutdown.Both);
                client_socket.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Client";

            // �� ����'������ - ������ ������� ���� ������� ����� (��� ��������) - ������ Server �� ���� ������ �� �����������
            Process.Start("Server.exe"); 
        }
    }
}