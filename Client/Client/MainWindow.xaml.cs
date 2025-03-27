using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Data;
using System.Globalization;
using System.Threading;
using System.Windows.Documents;
using System.Security.Cryptography;

namespace Client
{
    public partial class MainWindow : Window, INotifyPropertyChanged 
    {
        private Socket _clientSocket;
        private bool _isConnected;
   
        private string _connectionStatus = "Отключено";

        public event PropertyChangedEventHandler PropertyChanged;

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConnectionStatus)));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            
            if (_isConnected) return;

            try
            {
                _clientSocket = new Socket(AddressFamily.InterNetwork,
                                        SocketType.Stream,
                                        ProtocolType.Tcp);

                var ipAddress = IPAddress.Parse(ServerIpTextBox.Text);
                var port = int.Parse(ServerPortTextBox.Text);
                var remoteEP = new IPEndPoint(ipAddress, port);

                _clientSocket.Connect(remoteEP);
                _isConnected = true;
                ConnectionStatus = "Подключено";
                UpdateUI();
                AddSystemMessage($"Подключено к {remoteEP}");

                new Thread(ReceiveMessages) { IsBackground = true }.Start(); //Creating a new pool otherwise Recive  stops all programm
            }
            catch (Exception ex)
            {
                AddSystemMessage($"Ошибка подключения: {ex.Message}");
                ConnectionStatus = "Ошибка";
                UpdateUI();
                _clientSocket?.Close();
            }
        }

        private void ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (_isConnected)
                {
                    int bytesReceived = _clientSocket.Receive(buffer);
                    if (bytesReceived == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                    Dispatcher.Invoke(() => ProcessMessage(message)); // put a ui refresh in line to ui-pool
          
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    AddSystemMessage($"Ошибка получения: {ex.Message}");
                    Disconnect();
                });
            }
        }

        private void ProcessMessage(string message)
        {
            if (message.Contains("<TheEnd>"))
            {
                AddSystemMessage("Сервер закрыл соединение");
                Disconnect();
                return;
            }
             
            
            AddMessage("Сервер", message);
        }

      
        private void Disconnect()
        {
            try
            {
                if (_clientSocket != null && _clientSocket.Connected)
                {
                    _clientSocket.Shutdown(SocketShutdown.Both);
                    _clientSocket.Close();
                }
            }
            catch { }
            finally
            {
                _isConnected = false;
                ConnectionStatus = "Отключено";
                UpdateUI();
                _clientSocket = null;
            }
        }

        private void UpdateUI() // Ui refrsh nothing interesting
        {
            Dispatcher.Invoke(() =>
            {
                ConnectionIndicator.Fill = _isConnected ? Brushes.Green : Brushes.Red;
                SendButton.IsEnabled = _isConnected;
                DisconnectButton.IsEnabled = _isConnected;
                ConnectButton.IsEnabled = !_isConnected;
            });
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage(MessageTextBox.Text);
        }

        private void SendMessage(string message)
        {
            if (!_isConnected || string.IsNullOrWhiteSpace(message)) return;

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                _clientSocket.Send(bytes);
                AddMessage("Вы", message, true);
                MessageTextBox.Clear();
            }
            catch (Exception ex)
            {
                AddSystemMessage($"Ошибка отправки: {ex.Message}");
                Disconnect();
            }
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)//Sending a message from enter key
            {
                SendMessage(MessageTextBox.Text);
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectToServer();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void AddMessage(string sender, string text, bool isOwn = false)
        {
            var message = new ChatMessage
            {
                Sender = sender,
                Text = text,
                Timestamp = DateTime.Now,
                IsOwn = isOwn,
                IsSystem = false
            };

            ChatMessagesListView.Items.Add(message); //adding message on screen 
            ChatMessagesListView.ScrollIntoView(message);// scroll mesagger to the last message
        }

        private void AddSystemMessage(string text)
        {
            var message = new ChatMessage
            {
                Sender = "Система",
                Text = text,
                Timestamp = DateTime.Now,
                IsSystem = true
            };

            ChatMessagesListView.Items.Add(message);
            ChatMessagesListView.ScrollIntoView(message);
        }

        protected override void OnClosed(EventArgs e) // переопределение этого говна чтобы рвал соединение 
        {
            Disconnect();
            base.OnClosed(e);
        }
    }

    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsOwn { get; set; }
        public bool IsSystem { get; set; }
    }
     // дальше управление стилями 
    public class SystemMessageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Blue : Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.LightBlue : Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}