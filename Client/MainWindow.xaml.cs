/***********************************************************************************************
* File name       :       MainWindow.xaml.cs
* Function        :       Gives the GUI for the user and defines the funcion call for
*                         each GUI input from the user
* Application     :       Project # 4 - Software Modeling & Analysis                        
* Author          :       Jegan Gopalakrishnan
* Reference       :       Prof.Jim Fawcett, Project 4 Fall 16 Help code
 **********************************************************************************************/

/*
 * Package Operations:
 * -------------------
 * Contains action method for all the GUI application and runs the demonstrates the
 * default test executive constructor
 * 
 * Required Files:
 * ---------------
 * - ChannelDemo.cs, ICommunicator.cs, CommServices.cs, Messages.cs
 *
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 19 Nov 2016
 * - first release 
 *  
 */

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Threading;
using System.IO;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace WPFClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32")]
        public static extern void AllocConsole();
        [DllImport("kernel32")]
        public static extern void FreeConsole();

        public Client client { get; set; }

        // Constructor - Initializes the WPF window and Runs the default test executive
        public MainWindow()
        {
            InitializeComponent();
            //Creating console to display the runtime output
            AllocConsole();
            Console.Title = "CLIENT-1 http://localhost:8081/ICommunicator";
            Console.WriteLine("\t\t\t\tClient-1 - Successfull Scenario Demonstration");
            Console.WriteLine("\t\t\t\t==============================================");
            client = new WPFClient.Client();
            try
            {
                // Hard Coding the path to the Repository for Test Executive
                string PathToXmlFiles = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\ClientStorageLocation\\");
                Console.WriteLine("\n\nTest Executive : DLL and XML files are placed in : {0} \n",PathToXmlFiles);
                Action<string> TextBox = (x) => updateTextBox(x);
                Action<string> TextBlock = (x) => updateTextBlock(x);
                Task Th = Task.Run(() => client.Run(PathToXmlFiles, TextBox, TextBlock));
                Console.WriteLine("Client querying for logs to Repository - requesting logs from the repository");
                Th.ContinueWith(antecedent =>
                {
                    Thread.Sleep(2000); // to allow the test harness to execute and store the logs
                    client.GetLogsFromRepository();
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in WPFClient.ClientUtility.Run Method & Exception is " + ex.Message);
            }
        }

        // Functionality for the exit button
        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Exit button is pressed and application is terminationg");
            this.Close();
            FreeConsole();
        }

        // Functionality for the browse button
        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog d = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult xmlPath = d.ShowDialog();

            textBox.Text = d.SelectedPath;
            Console.WriteLine("Browse button is pressed and the path chosen is {0}", d.SelectedPath);
        }

        // Functionality for the GetLogs button
        private void buttonLogs_Click(object sender, RoutedEventArgs e)
        {
            if (client.TestExecutedAtleastOnce)
            {
                try
                {
                    Console.WriteLine("GetLogs button is pressed - requesting logs from the repository");
                    Thread th = new Thread(() => client.GetLogsFromRepository());
                    th.Start();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in WPFClient.ClientUtility.GetLogsFromRepository Method & Exception is " + ex.Message);
                }
            }
            else
            {
                textBlockResult.Text = "Please choose a directory and click 'Run Test' To execute the test case";
                Console.WriteLine(textBlockResult.Text);
            }
        }

        // Functionality for RunTest button
        private void buttonRun_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string PathToXmlFiles = textBox.Text;
                Action<string> TextBox = (x) => updateTextBox(x);
                Action<string> TextBlock = (x) => updateTextBlock(x);
                Thread th = new Thread(() => client.Run(PathToXmlFiles, TextBox, TextBlock));
                th.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in WPFClient.ClientUtility.Run Method & Exception is " + ex.Message);
            }
        }

        // This method is not used
        private void buttonStatus_Click(object sender, RoutedEventArgs e)
        {
            // not used
        }

        // Dispatcher.Invoke() functionality to update the textBox from child thread 
        public void updateTextBox(string x)
        {
            Application.Current.Dispatcher.BeginInvoke(
                   DispatcherPriority.Background,
                   new Action(() => textBox.Text = x));
        }

        // Dispatcher.Invoke() functionality to update the textBlock from child thread
        public void updateTextBlock(string x)
        {
            Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => textBlockResult.Text = x));
        }
    }
}
