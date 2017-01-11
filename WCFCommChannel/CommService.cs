/***********************************************************************************************
 *  File name       :       CommService.cs
 *  Function        :       contains the utility class sender, receiver and comm that can
 *                          be used in the WCF commnication 
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 *  Reference       :       Prof.Jim Fawcett, Project 4 Fall 16 Help code
 * *********************************************************************************************/
/*
 * Package Operations:
 * -------------------
 * This package defindes a Sender class and Receiver class that
 * manage all of the details to set up a WCF channel.
 * 
 * Public Interface
 * ----------------
 * Comm<ClassName> comm { get; set; } = new Comm<ClassName>();
 * 
 * Required Files:
 * ---------------
 * CommService.cs, ICommunicator, BlockingQueue.cs, Messages.cs
 *   
 * Maintenance History:
 * --------------------
 * ver 1.0 : 18 Nov 2016
 * - first release
 */
using System;
using System.ServiceModel;
using System.Threading;
using BlockingQueue;
using System.IO;

namespace WCFCommChannel
{
    // Receiver hosts Communication service used by other Peer
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class Receiver<T> : ICommunicator
    {
        // File Service parameters
        string filename;
        string savePath = "..\\..\\RepositoryStorageLocation";      // storage location in the repository
        string ToSendPath = "..\\..\\RepositoryStorageLocation";    // sending location in the repository
        int BlockSize = 1024;
        byte[] block;

        static BlockingQueue<Message> rcvBlockingQ = null;          // Receiver blocking queue
        ServiceHost service = null;                                 // service point for the host

        public string name { get; set; }            // name of the invoking class

        // Constructor - defines the block size for file transfer and blocking queue object creation
        public Receiver()
        {
            block = new byte[BlockSize];
            if (rcvBlockingQ == null)
                rcvBlockingQ = new BlockingQueue<Message>();
        }

        // Returns the Save path repository
        public string GetRepositoryPath ()
        {
            return savePath;
        } 

        // Service method uploadFile implemented from interface ICommunicator
        public void upLoadFile(FileTransferMessage msg)
        {
            int totalBytes = 0;
            filename = msg.filename;
            string rfilename = Path.Combine(savePath, filename);
            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            // Writes content of the file from the given stream
            using (var outputStream = new FileStream(rfilename, FileMode.Create))
            {
                while (true)
                {
                    int bytesRead = msg.transferStream.Read(block, 0, BlockSize);
                    totalBytes += bytesRead;
                    if (bytesRead > 0)
                        outputStream.Write(block, 0, bytesRead);
                    else
                        break;
                }
            }
            Console.Write("\nReceived file \"{0}\" of {1} bytes",filename, totalBytes);
        }

        // Service method downLoadFile implemented form interface ICommunicator
        public Stream downLoadFile(string filename)
        {
            string sfilename = Path.Combine(ToSendPath, filename);
            FileStream outStream = null;
            // creates the stream for the given name and returns the same to the caller of the function
            if (File.Exists(sfilename))
            {
                outStream = new FileStream(sfilename, FileMode.Open);
            }
            else
            {
                
                throw new Exception("");
            }
            Console.Write("\n Sent File \"{0}\"", filename);
            return outStream;
        }

        // start() method executes the method passed as a argument
        public Thread start(ThreadStart rcvThreadProc)
        {
            Thread rcvThread = new Thread(rcvThreadProc);
            rcvThread.Start();
            return rcvThread;
        }

        // closes the WCF service opened by the end point
        public void Close()
        {
            service.Close();
        }

        //  Create ServiceHost for Communication service
        public void CreateRecvChannel(string address)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 50000000;
            Uri baseAddress = new Uri(address);
            service = new ServiceHost(typeof(Receiver<T>), baseAddress);
            service.AddServiceEndpoint(typeof(ICommunicator), binding, baseAddress);
            service.Open();
            Console.Write("\nService is open and listening on {0}", address);
        }

        // Implement service method to receive messages from other Peers
        public void PostMessage(Message msg)
        {
            rcvBlockingQ.enQ(msg);
        }

        // Implement service method to extract messages from other Peers.
        // This will often block on empty queue, so user should provide
        // read thread.
        public Message GetMessage()
        {
            Message msg = rcvBlockingQ.deQ();
            Console.Write("\n{0} dequeuing message posted by {1}", name, msg.from);
            return msg;
        }
    }
    
    // Sender is client of another Peer's Communication service
    public class Sender
    {
        public string name { get; set; }

        ICommunicator channel;
        BlockingQueue<Message> sndBlockingQ = null;
        Thread sndThrd = null;
        int tryCount = 0, MaxCount = 10;
        string currEndpoint = "";

        // Processing the sender thread
        void ThreadProc()
        {
            tryCount = 0;
            while (true)
            {
                Message msg = sndBlockingQ.deQ();
                Console.WriteLine("Sending the Below Msg to {0}",msg.to);
                msg.showMsg();
                if (msg.to != currEndpoint)
                {
                    currEndpoint = msg.to;
                    CreateSendChannel(currEndpoint);
                }
                while (true)
                {
                    try
                    {
                        channel.PostMessage(msg);
                        Console.Write("\nPosted message from {0} to {1}", name, msg.to);
                        tryCount = 0;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.Write("\n  connection failed" + ex.Message);
                        if (++tryCount < MaxCount)
                            Thread.Sleep(100);
                        else
                        {
                            Console.Write("\n  {0}", "can't connect\n");
                            currEndpoint = "";
                            tryCount = 0;
                            break;
                        }
                    }
                }
                if (msg.body == "quit")
                    break;
            }
        }

        // Initialize Sender - creates blocking queue and invokes ThreadProc method in new thread
        public Sender()
        {
            sndBlockingQ = new BlockingQueue<Message>();
            sndThrd = new Thread(ThreadProc);
            sndThrd.IsBackground = true;
            sndThrd.Start();
        }


        // Create proxy to another Peer's Communicator 
        public void CreateSendChannel(string address)
        {
            EndpointAddress baseAddress = new EndpointAddress(address);
            BasicHttpBinding binding = new BasicHttpBinding();
            ChannelFactory<ICommunicator> factory
              = new ChannelFactory<ICommunicator>(binding, address);
            channel = factory.CreateChannel();
            Console.Write("\nService proxy created for {0}", address);
        }

        //----< posts message to another Peer's queue >------------------
        /*
         *  This is a non-service method that passes message to
         *  send thread for posting to service.
         */
        public void PostMessage(Message msg)
        {
            sndBlockingQ.enQ(msg);
        }

        // Closes the send channel
        public void Close()
        {
            ChannelFactory<ICommunicator> temp = (ChannelFactory<ICommunicator>)channel;
            temp.Close();
        }
    }

    // Comm class simply aggregates a Sender and a Receiver
    public class Comm<T>
    {
        public string name { get; set; } = typeof(T).Name;
        public Receiver<T> rcvr { get; set; } = new Receiver<T>();
        public Sender sndr { get; set; } = new Sender();

        // Constructor - assigns the name of sender and receiver
        public Comm()
        {
            rcvr.name = name;
            sndr.name = name;
        }

        // Making the endpoint by appending the url and port number passed
        public static string makeEndPoint(string url, int port)
        {
            string endPoint = url + ":" + port.ToString() + "/ICommunicator";
            return endPoint;
        }

        // thrdProc() used only for testing purpose
        public void thrdProc()
        {
            while (true)
            {
                Message msg = rcvr.GetMessage();
                msg.showMsg();
                if (msg.body == "quit")
                {
                    break;
                }
            }
        }
    }
#if (TEST_STUB)

  class Cat { }
  class TestComm
  {
    [STAThread]
    static void Main(string[] args)
    {
      Comm<Cat> comm = new Comm<Cat>();
      string endPoint = Comm<Cat>.makeEndPoint("http://localhost", 8080);
      comm.rcvr.CreateRecvChannel(endPoint);
      comm.rcvr.start(comm.thrdProc);
      comm.sndr = new Sender();
      comm.sndr.CreateSendChannel(endPoint);
      Message msg1 = new Message();
      msg1.body = "Message #1";
      comm.sndr.PostMessage(msg1);
      Message msg2 = new Message();
      msg2.body = "quit";
      comm.sndr.PostMessage(msg2);
      Console.Write("\n  Comm Service Running:");
      Console.Write("\n  Press key to quit");
      Console.ReadKey();
      Console.Write("\n\n");
    }
#endif
}

