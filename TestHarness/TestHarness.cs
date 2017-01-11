/********************************************************************************************************
 *  File name       :       TestHarness.cs
 *  Function        :       Creates a end point and process all testrequest and sends back the testresult
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan, Reference : Prof.Jim Fawcett Project4Fall16HelpCode
 * ******************************************************************************************************/

/*
 * Module Operations:
 * -------------------
 * The TestHarness package defines one class, TestHarness, that uses the Comm<TestHarness>
 * class to receive and send messages from a remote endpoint.
 * 
 * Required Files:
 * ---------------
 * - TestHarness.cs , TestMoniotor.cs, TestDatabase.cs
 * - ICommunicator.cs, CommServices.cs, Messages.cs, FileTransfer.cs
 * 
 *  Public Interface
 *  ----------------
 *  TestHarness TH = new TestHarness();
 *  
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 21 Nov 2016
 * - first release 
 */
using System;
using System.Threading.Tasks;
using System.Threading;
using WCFCommChannel;
using System.Xml;
using System.IO;

namespace TestHarness
{
    // Class creates 2 threads sender & receiver for WCF communication
    class TestHarness
    {
        public Comm<TestHarness> comm { get; set; } = new Comm<TestHarness>();
        // test harness end point
        public string endPoint { get; } = Comm<TestHarness>.makeEndPoint("http://localhost", 8080);
        private Thread rcvThread = null;

        // contructor creates receiver thread and executes rcvThreadProc
        public TestHarness()
        {
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }

        // waits for the receive thread created in constructor
        public void wait()
        {
            rcvThread.Join();
        }

        //  sends the msg reply back to the sender with the mentioned msgbody and msgtype
        void SendReply(string msgtype, string msgBody, Message msg)
        {
            string temp = msg.from;
            msg.from = msg.to;
            msg.to = temp;
            msg.type = msgtype;
            msg.body = msgBody;
            comm.sndr.PostMessage(msg);
        }

        // method dequeues the test request and invokes test harness manager to process them
        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                Console.Write("\n{0} received message of type {1} from {2}:\n", comm.name,msg.type, msg.from);
                msg.showMsg();

                if (msg.type == "TestRequest")
                {
                    //Convert msgBody string to XML file
                    string xml = msg.body;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xml);
                    Console.WriteLine();
                    // Saving the xml in the name with the combination of author and time stamp
                    string AuthorTimeStamp = msg.author + msg.time.ToString("MM-dd-yy-H-mm-ss");
                    string PathToXmlFile = Path.Combine(Directory.GetCurrentDirectory(), $"..\\..\\TemporaryFiles\\{AuthorTimeStamp}");
                    if (!Directory.Exists(PathToXmlFile))
                        Directory.CreateDirectory(PathToXmlFile);


                    string XMLFileName = $"{PathToXmlFile}\\{AuthorTimeStamp}.xml";
                    doc.Save(XMLFileName);
                    Console.WriteLine("The receiver msg xml body is stored as xml file : {0}", XMLFileName);

                    // Creating the action delegate replyMsg and passing the same to testExecutor
                    Action<string, string> replyMsg = (x, y) => SendReply(x, y, msg);

                    Console.WriteLine("Creating new task(thread) to Execute the test Request {0} from {1}", Path.GetFileName(XMLFileName), msg.from);

                    Task Thread = Task.Run(() => TestHarnessManager.Execute(PathToXmlFile, replyMsg));
                }
                else
                {
                    Console.WriteLine("Irrelavent message , dropping it");
                }
            }
        }

        // start point of test harness server
        static void Main(string[] args)
        {
            Console.Title = "TEST HARNESS SERVER - http://localhost:8080/ICommunicator";
            Console.WriteLine("\t\t\t\tTest Harness Server");
            Console.WriteLine("\t\t\t\t===================");
            TestHarness TH = new TestHarness();
            TH.wait();
            Console.Write("\n\n");
        }
    }
}
