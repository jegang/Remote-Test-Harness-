/********************************************************************************************************
 *  File name       :       Repository.cs
 *  Function        :       Act as Repository for storing DLL's and Logs using WCF
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan, Reference : Prof.Jim Fawcett Project4Fall16HelpCode
 * ******************************************************************************************************/
/*
*   Module Operations
*   -----------------
*   Repository uses the WCFCommChannel and creates two threads, 
*   sender thread - running the sender blocking queue, sends the message to the destined end point
*   receiver thread - process the receiver thread messages for handling client queries
* 
*   Public Interface
*   ----------------
*   Repository R = new Repository(); // All the recevier, sender WCF thread will be created as part of this
*/
/*
 *   Build Process
 *   -------------
 *   - Required files:   CommService.cs, FileTransfer.cs, ICommunicator.cs, Messages.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 20 Nov 2016
 *     - first release
 */
using System;
using System.IO;
using System.Threading;     
using WCFCommChannel;

namespace Repository
{
    // contains methods for sender and receiver process threads - to store and send files
    class Repository
    {
        // FileTransferutility holds the method for download and upload file
        public FileTransferutility<Repository> FileTransfer { get; set; } = new FileTransferutility<Repository>();
        // Sender and Receiver thread for Repository
        public Comm<Repository> comm { get; set; } = new Comm<Repository>();
        public string endPoint { get; } = Comm<Repository>.makeEndPoint("http://localhost", 8083);
        private Thread rcvThread = null;

        // Constructor - initializes the receiver 
        public Repository()
        {
            comm.rcvr.CreateRecvChannel(endPoint);
            rcvThread = comm.rcvr.start(rcvThreadProc);
        }
        // It joins for the receive thread
        public void wait()
        {
            rcvThread.Join();
        }

        // Sends Reply message back to the sender with the modified test body and msg type
        void SendReplyMsg(string msgtype, string msgBody, Message msg)
        {
            string temp = msg.from;
            msg.from = msg.to;
            msg.to = temp;
            msg.type = msgtype;
            msg.body = msgBody;
            comm.sndr.PostMessage(msg);
        }

        // Executed in seperate thread infintely which process all the incoming messages
        void rcvThreadProc()
        {
            while (true)
            {
                Message msg = comm.rcvr.GetMessage();
                string replyMsgContent;
                Console.WriteLine("Received the message of type : {0} from : {1} and the message is ",msg.type,msg.from);
                msg.showMsg();
                if (msg.type == "ClientQuery")
                {
                    string FileName = comm.rcvr.GetRepositoryPath() + "\\" + msg.body;
                    if(File.Exists(FileName))
                    {
                        Console.WriteLine("Log file {0} available in the Repository", FileName);
                        replyMsgContent = File.ReadAllText(FileName);
                    }
                    else
                    {
                        replyMsgContent = $"Log File {msg.body} not available in the repository";
                        Console.WriteLine(replyMsgContent);
                    }
                    Console.WriteLine("Sending the Logs embedded in msg body back to client");
                    SendReplyMsg("ClientQuery", replyMsgContent, msg);
                }
                if (msg.body == "quit")
                    break;
            }
        }
        // Main method where Repository creates end point and waits for the messages
        static void Main()
        {
            Console.Title = "Repository - http://localhost:8083/ICommunicator";
            Console.Write("\n\t\t\t\tREPOSITORY SERVER ");
            Console.Write("\n\t\t\t\t====================\n");
            Repository Rep = new Repository();
            Console.WriteLine("\n"+@"Repository Storage Location : Project 4\RemoteTestHarness\Repository\RepositoryStorageLocation");
            Rep.wait();
            Console.Write("\n\n");
        }
    }
}

