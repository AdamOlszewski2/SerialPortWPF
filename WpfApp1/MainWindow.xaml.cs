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
using System.Collections;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort _serialPort = new SerialPort();
        static ManagementObjectSearcher searcher;
        public MainWindow()
        {
            InitializeComponent();
            foreach (string s in SerialPort.GetPortNames())
            {
                nrPortu.Items.Add(s);
            }
        }

        public class InfoToSend
        {
            public string Title { get; set; }
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

            try
            {
                _serialPort.PortName = nrPortu.Text;
                _serialPort.BaudRate = Int32.Parse(baudRate.Text);
                _serialPort.Parity = Parity.None;
                _serialPort.DataBits = 8;
                _serialPort.StopBits = StopBits.One;
                _serialPort.Handshake = Handshake.None;
                _serialPort.Open();
                if (_serialPort.IsOpen)
                {
                    txt_Port.Text = "Port " + nrPortu.Text + " otwarty";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Nie wybrano portu, lub port jest otwarty!\n" + ex.ToString());
            }
        }
        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            _serialPort.Close();
            txt_Port.Text = "Port " + _serialPort.PortName + " zamknięty";
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort.IsOpen)
                GetInfo();
        }

        public void GetInfo()
        {
            searcher = new ManagementObjectSearcher("Select * from Win32_Processor");
            string x; float w;
            foreach (ManagementObject cdrom in searcher.Get())
            {
                _serialPort.WriteLine(cdrom.GetPropertyValue("Name").ToString());
                x = cdrom.GetPropertyValue("CurrentClockSpeed").ToString();
                w = float.Parse(x);
                w /= 1000;
                _serialPort.WriteLine("CPU Clock " + w + " GHz");
                _serialPort.WriteLine("Cores " + cdrom.GetPropertyValue("NumberOfCores").ToString());
            }

            searcher = new ManagementObjectSearcher("Select Name from Win32_VideoController");
            foreach (ManagementObject cdrom in searcher.Get())
            {
                _serialPort.WriteLine(cdrom.GetPropertyValue("Name").ToString());
            }

            searcher = new ManagementObjectSearcher("Select Model from Win32_DiskDrive");
            foreach (ManagementObject cdrom in searcher.Get())
            {
                _serialPort.WriteLine(cdrom.GetPropertyValue("Model").ToString());
            }
            UInt64 Capacity = 0;
            searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
            foreach (ManagementObject cdrom in searcher.Get())
            {
                Capacity += (UInt64)cdrom["Capacity"];
            }
            _serialPort.WriteLine(String.Format("RAM: {0}GB", Capacity / (1024 * 1024 * 1024)));

            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
            foreach (ManagementObject cdrom in searcher.Get())
            {
                _serialPort.WriteLine(cdrom.GetPropertyValue("DeviceID").ToString() + " Size: " + cdrom.GetPropertyValue("Size").ToString() + "B" + " FileSystem: " + cdrom.GetPropertyValue("FileSystem").ToString());
            }
            //searcher = new ManagementObjectSearcher("Select * from Win32_DiskDrive");
            //foreach (ManagementObject cdrom in searcher.Get())
            //{
            //    PropertyDataCollection props = cdrom.Properties;
            //    foreach (PropertyData prop in props)
            //    {
            //        Console.WriteLine("Property name: {0}", prop.Name);
            //        Console.WriteLine("Property value: {0}", prop.Value);
            //    }
            //}
        }
    }
}
