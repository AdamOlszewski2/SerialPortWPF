using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Management;
using System.Management.Instrumentation;
using System.IO.Ports;
using System.Threading;
namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort = new SerialPort();
        public MainWindow()
        {
            InitializeComponent();
            foreach(string s in SerialPort.GetPortNames())
            {
                nrPortu.Items.Add(s);
            }
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            nrPortu.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
            {
                nrPortu.Items.Add(s);
            }
        }

        private void openPort_Click(object sender, RoutedEventArgs e)
        {
            if(nrPortu.Text == ""  || _serialPort.IsOpen)
            {
                MessageBox.Show("Nie wybrano portu, lub port jest otwarty!");
            }
            else
            {
                _serialPort.PortName = nrPortu.Text;
                _serialPort.BaudRate = Int32.Parse(baudRate.Text);
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;
                _serialPort.Open();
                for(int i = 0; i < 10; i++)
                {
                    _serialPort.WriteLine("AAA");
                    Thread.Sleep(500);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }
    }
}
