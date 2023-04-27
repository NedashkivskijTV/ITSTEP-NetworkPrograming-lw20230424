using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Отримання IP-адреси
            // - введення вручну, отримання через cmd-ipconfig
            //IPAddress address = IPAddress.Parse("192.168.56.1");
            // отримання через IPAddressLocal (працює не завжди)
            //IPAddress address = IPAddress.Loopback;
            // отримання IP автоматично
            IPAddress address = Dns.GetHostAddresses(Dns.GetHostName())[2];

            // Кінцева точка підключення (пасивний сокет прослуховуватиме дані з кінцевої точки - socket буде біндитись до кінцевої точки)
            // - конструктор приймає IP-адресу та порт 
            IPEndPoint endPoint = new IPEndPoint(address, 1024);

            // Пасивний сокет на боці сервера - прослуховує підключення
            // Параметри
            // - AddressFamily.InterNetwork - для IPv4
            // - SocketType.Stream - протокол TCP (за замовчуванням TCP)
            // - ProtocolType.IP - протокол передачі даних
            // Для отримання сокетом точки підключення потрібно передати IP-адресу та порт
            Socket pass_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP); // створення сокета
            
            // привязка сокета до кінцевої точки
            pass_socket.Bind(endPoint); // (пасивний сокет прослуховуватиме дані з кінцевої точки - socket буде біндитись до кінцевої точки)

            // Переведення/запуск сокета в режим прослуховування
            // Може бути задано максимальне значення клієнтів - у даному разі 10
            pass_socket.Listen(10);

            // Інформаційне повідомлення про включення сервера
            Console.WriteLine($"Server was started at port 1024, address {address}"); 

            // Отримання списку IP-адрес microsoft.com
            // їх виведення у консоль на боці сервера та
            // передача їх у повідомленні клієнту
            IPAddress[] addresses = Dns.GetHostAddresses("microsoft.com");
            string str = "";
            foreach (var adr in addresses)
            {
                Console.WriteLine(adr);
                str += adr + "\r\n";
            }

            try
            {
                // Прослуховування - постійне підключення в пасивному режимі
                while (true)
                {
                    // Створення сокету, який буде підключати клієнта та отримувати відправлені ним дані
                    Socket ns = pass_socket.Accept(); // створення сокета винесено за межі блока try з метою винесення коду закриття сокету у блок finally
                    
                    // Інформаційне повідомлення про підключення клієнта
                    Console.WriteLine($"Client #{ns.LocalEndPoint} connected"); // виведення 
                    Console.WriteLine($"Client #{ns.RemoteEndPoint} connected"); // виведення IP та порта клієнта

                    // Відправка даних
                    // - метод Send приймає параметр, переведений у байти
                    // - використовується клас Encoding,
                    // - спосіб кодування Default(за замовчуванням UTF8)
                    // - метод GetBytes
                    ns.Send(Encoding.Default.GetBytes($"Server {ns.LocalEndPoint} send answer {DateTime.Now}\n, address microsoft {str}"));

                    // Закриття сокета - зазвичай розташовується у блоці finally
                    // - закриття комунікації між клієнтом і сервером
                    // - закриття сокета
                    ns.Shutdown(SocketShutdown.Both);
                    ns.Close();
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}