/***********************************************************************************************
 *  File name       :       ClientUtilties.cs
 *  Function        :       Contains the client utilities to send XML's and receiver testResults
 *                          and logs
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 *  Reference       :       Prof.Jim Fawcett, Project 4 Fall 16 Help code
 * *********************************************************************************************/

/*
 * Package Operations:
 * -------------------
 * The Client package defines one class, Client, that uses the Comm<Client>
 * class to pass messages to a remote endpoint.
 * 
 * Build Process: 
 * --------------- 
 * Required Files:  ClientUtilities.cs, ICommunicator.cs, CommServices.cs, FileTransfer.cs, Messages.cs
 * 
 * Public Interface:
 * -----------------
 * Client client = new Client();
 * client.Run(PathToDefaultXmlFiles, TextBox, AddToTextBlock);
 * client.GetLogsFromRepository();
 *
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 18 Nov 2016
 * - first release  
 */

using System;
using System.Threading;
using WCFCommChannel;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace WPFClient2
{
    // Client class demonstrates how an application uses Comm
    public class Client
    {
        public Action<string> TextBlock { get; set; }
        public FileTransferutility<Client> FileTransfer { get; set; } = new WCFCommChannel.FileTransferutility<Client>();
        public Comm<Client> comm { get; set; } = new Comm<Client>();
        public string ClientEndPoint { get; } = Comm<Client>.makeEndPoint("http://localhost", 8082);
        public Message msg { get; set; }        // created message for the client
        public bool TestExecutedAtleastOnce = false;

        private Thread rcvThread = null;

        public HiResTimer hrt { get; set; }     // To calculate the computation time of the execution of test case

        // Constructor to initialize receiver and HiResTimer()
        public Client()
        {
            hrt = new HiResTimer();
            comm.rcvr.CreateRecvChannel(ClientEndPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
            msg = new Message();
        }

        // join receive thread 
        public void wait()
        {
            rcvThread.Join();
        }

        // construct a basic message with the given contents
        public Message makeMessage(string author, string fromEndPoint, string toEndPoint, string msgBody, string msgType, DateTime msgTime)
        {
            msg.type = msgType;
            msg.time = msgTime;
            msg.author = author;
            msg.from = fromEndPoint;
            msg.to = toEndPoint;
            msg.body = msgBody;
            return msg;
        }

        // Use private service method to receive a message 
        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                msg.time = DateTime.Now;
                Console.Write("\n{0} Received message from {1} of message type {2}  and the message is :\n", comm.name, msg.from, msg.type);
                if (msg.type == "TestResult" || msg.type == "Notification")
                {
                    hrt.Stop();     // stops the timer
                    String msgContent = msg.body + "\n" + $"Time Taken for execution and communication latency {hrt.ElapsedMicroseconds / 1000} milliseconds  Approximately {(float)hrt.ElapsedMicroseconds / (float)1000000} seconds";
                    Console.WriteLine(msgContent);
                    TextBlock(msgContent);
                }
                else if (msg.type == "ClientQuery")
                {
                    Console.WriteLine(msg.body);
                    string LogFile = FileTransfer.SavePath + "\\" + msg.author + msg.time.ToString("MM-dd-yy-H-mm-ss") + "_Log.txt";
                    System.IO.File.WriteAllText(LogFile, msg.body);
                    System.Diagnostics.Process.Start(LogFile);
                }
                if (msg.body == "quit")
                    break;
            }
        }

        // Requests the logfile in combination of author name and timestamp to the repository
        public void GetLogsFromRepository()
        {
            string RepositoryEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8083);
            string logFile = msg.author + msg.time.ToString("MM-dd-yy-H-mm-ss") + "_TESTLOG.txt";
            Message repMsg = makeMessage(msg.author, msg.from, RepositoryEndPoint, logFile, "ClientQuery", msg.time);
            Console.WriteLine("Requesting for log file {0} from Repository", logFile);
            comm.sndr.PostMessage(repMsg);
        }


        // Run() form the TestRequest Message with the msgBody of xml content and post it to TestHarness
        public void Run(string PathToDefaultXmlFiles, Action<string> TextBox, Action<string> AddToTextBlock)
        {
            TestExecutedAtleastOnce = true;
            Thread.Sleep(1000);
            TextBlock = AddToTextBlock;
            TextBlock("");
            hrt.Start();    // starting timer of execution
            string RepositoryEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8083);
            string TestHarnessEndPoint = Comm<Client>.makeEndPoint("http://localhost", 8080);
            // Getting the xml files in the given directory
            string[] XMLFiles = Directory.GetFiles(PathToDefaultXmlFiles, "*.xml", SearchOption.AllDirectories);
            if (XMLFiles.Length != 1)
            {
                string errMsg = (XMLFiles.Length == 0) ? "ERROR : No XML Files available in the location" : "ERROR: Only one xml should be available in the directory";
                Console.WriteLine(errMsg);
                TextBlock(errMsg);
                return;
            }
            // uploading all the dll files available to the repository
            string[] DLLFiles = Directory.GetFiles(PathToDefaultXmlFiles, "*.dll", SearchOption.AllDirectories);
            foreach (string DLL in DLLFiles)
            {
                FileTransfer.ToSendPath = PathToDefaultXmlFiles;
                bool returnValue = FileTransfer.uploadFile(Path.GetFileName(DLL), RepositoryEndPoint);
                if (returnValue == false)
                {
                    Console.WriteLine("Failed to transfer {0} to repository", DLL);
                    return;
                }
            }
            Console.WriteLine("\n\nTestRequest XMl file name : {0}", Path.GetFileName(XMLFiles[0]));
            Console.WriteLine("\nCreating TestRequest Message with message body as XML content of file {0}", Path.GetFileName(XMLFiles[0]));
            TextBox(Path.GetDirectoryName(XMLFiles[0]));
            // Logic to convert XML to string
            string xmlPath = XMLFiles[0];
            string xmlString = System.IO.File.ReadAllText(xmlPath);
            XDocument doc = XDocument.Load(xmlPath);
            string Author = doc.Descendants("author").First().Value;
            // making testrequest message and sending it to the test harness server
            msg = makeMessage(Author, ClientEndPoint, TestHarnessEndPoint, xmlString, "TestRequest", DateTime.Now);
            comm.sndr.PostMessage(msg);
            wait();
            Console.Write("\n\n");
        }
    }
}
