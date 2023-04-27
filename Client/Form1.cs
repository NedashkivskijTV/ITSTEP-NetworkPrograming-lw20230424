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
            // Отримання даних з сервера 
            IPAddress address = IPAddress.Parse(textBoxIP.Text);
            IPEndPoint endPoint = new IPEndPoint(address, 1024);

            // Створення активного сокета на стороні клієнта
            Socket client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            try
            {
                // Логіка створення з'єднання
                // Підключення сокета - в параметри передається кінцева точка
                client_socket.Connect(endPoint);

                // Подальша логіка залежить від того чи вдало підключився клієнт
                if (client_socket.Connected)
                {
                    // Відправка даних у разі вдалого підключення
                    string querry = "GET\r\n\r\n";
                    client_socket.Send(Encoding.Default.GetBytes(querry));

                    // Створення байтового буфера, що використовуватиметься при отриманні інф від сервера
                    byte[] buffer = new byte[1024];

                    // Змінна, що зберігатиме число - кількість отриманих даних
                    int len;

                    // Цикл для отримання даних - перевіряється на наявність отриманих даних
                    // - якщо властивість сокета Available > 0 - дані прийшли і їх можна отримати
                    do
                    {
                        // отримання даних
                        len = client_socket.Receive(buffer);
                        // Передача отриманого повідомлення (частини) до відповідного текстового поля з попереднім декодуванням(аналогічним застосованому кодуванню на сервері)
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

            // НЕ обов'язково - запуск сервера після запуску форми (для зручності) - проект Server має бути додано до Залежностей
            Process.Start("Server.exe"); 
        }
    }
}