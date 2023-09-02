using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Blue
{
    public partial class Option1Page : Page
    {
        private SerialPort _serialPort;
        private DispatcherTimer _updateTimer;
        private StringBuilder _receivedDataBuffer = new StringBuilder();

        public Option1Page()
        {
            InitializeComponent();

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromMilliseconds(1);
            _updateTimer.Tick += UpdateTimer_Tick;

            ListAvailableComPorts();
            ListAvailableBaudRates();
            MonitorComPortChanges(); // Thêm dòng này để bắt đầu theo dõi sự thay đổi cổng COM

            // Không cần khởi tạo SerialPort tại đây
        }


        private void InitializeSerialPort(string comPort, int baudRate)
        {
            int timeout = 1;

            _serialPort = new SerialPort(comPort, baudRate);
            _serialPort.ReadTimeout = timeout;

            try
            {
                if (!_serialPort.IsOpen)
                {
                    _serialPort.Open();
                    _updateTimer.Start(); // Khởi động timer khi đã mở cổng COM thành công
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening serial port: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            await ReadAndAppendDataAsync();
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CloseSerialPort();
            _updateTimer.Stop();
        }

        private void Reset_Serial(object sender, RoutedEventArgs e)
        {
            CloseSerialPort();
            _receivedDataBuffer.Clear(); // Xóa dữ liệu đã nhận
            TxtData.Text = ""; // Xóa nội dung hiển thị trên TextBox
            ComPortsComboBox.SelectedIndex = -1; // Bỏ chọn cổng COM trên ComboBox
            BaudRateComboBox.SelectedIndex = -1; // Bỏ chọn baudrate trên ComboBox
            _updateTimer.Stop(); // Tạm ngưng timer

        }

        private void Begin_Serial(object sender, RoutedEventArgs e)
        {
            CloseSerialPort();

            // Lấy cổng COM được chọn từ danh sách
            string selectedComPort = ComPortsComboBox.SelectedItem as string;
            int selectedBaudRate = Convert.ToInt32(BaudRateComboBox.SelectedItem);
            if (!string.IsNullOrEmpty(selectedComPort) && selectedBaudRate > 0)
            {
                InitializeSerialPort(selectedComPort, selectedBaudRate);
            }
        }
        private void ListAvailableBaudRates()
        {
            int[] baudRates = { 9600, 19200, 38400, 57600, 115200 }; // Các tốc độ baudrate khả dụng
            foreach (int baudRate in baudRates)
            {
                BaudRateComboBox.Items.Add(baudRate);
            }
        }
        private StringBuilder _sentDataBuffer = new StringBuilder();

        private void Send_Data(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_serialPort != null && _serialPort.IsOpen && !string.IsNullOrEmpty(RxtData.Text))
                {
                    string dataToSend = RxtData.Text;
                    _serialPort.Write(dataToSend);

                    _sentDataBuffer.AppendLine("" + dataToSend);
                    TxtData.Text = _receivedDataBuffer.ToString() + "\n" + _sentDataBuffer.ToString();

                    RxtData.Clear();
                    
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void MonitorComPortChanges()
        {
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 or EventType = 3");
            watcher.EventArrived += (s, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    ListAvailableComPorts();
                });
            };
            watcher.Query = query;
            watcher.Start();
        }
        private List<string> _displayedComPorts = new List<string>(); // Danh sách cổng COM đã hiển thị

        private void ListAvailableComPorts()
        {
            string[] comPorts = SerialPort.GetPortNames();

            // Loại bỏ các cổng không còn sẵn trong danh sách comPorts
            foreach (string port in _displayedComPorts.ToList())
            {
                if (!comPorts.Contains(port))
                {
                    ComPortsComboBox.Items.Remove(port); // Xóa cổng COM khỏi ComboBox
                    _displayedComPorts.Remove(port); // Loại bỏ cổng COM khỏi danh sách hiển thị
                }
            }

            foreach (string port in comPorts)
            {
                if (!_displayedComPorts.Contains(port)) // Kiểm tra cổng COM đã được hiển thị trước đó chưa
                {
                    ComPortsComboBox.Items.Add(port);
                    _displayedComPorts.Add(port); // Thêm cổng COM vào danh sách đã hiển thị
                }
            }
        }
        private void CloseSerialPort()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
                _serialPort.Dispose();

                _receivedDataBuffer.Clear(); // Xóa dữ liệu đã nhận
                _sentDataBuffer.Clear(); // Xóa dữ liệu đã gửi
                TxtData.Clear(); // Xóa nội dung hiển thị trên TextBox
            }
        }
        private async Task ReadAndAppendDataAsync()
        {
            try
            {
                string newData = await Task.Run(() => _serialPort.ReadExisting());

                if (!string.IsNullOrEmpty(newData))
                {
                    _receivedDataBuffer.Append(newData);
                    TxtData.Text = _receivedDataBuffer.ToString();

                    // Tự động cuộn xuống cuối văn bản
                    TxtData.ScrollToEnd();
                }
            }
            catch (TimeoutException)
            {
                // Handle timeout if needed
            }
        }
        private void SaveDataToFile(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                string allData = _receivedDataBuffer.ToString() + "\n" + _sentDataBuffer.ToString();
                File.WriteAllText(filePath, allData);
            }
        }
        
    }
}
