/***********************************************************************************************
 *  File name       :       FileTransfer.cs
 *  Function        :       provides the utility function for the uploading and downloading
 *                          files
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 *  Reference       :       Prof.Jim Fawcett, Project 4 Fall 16 Help code
 * *********************************************************************************************/
/*
* Package Operations:
* -------------------
* This package defines the utility method uploadFile and downloadFile to upload and 
* download files as blocks via WCF communication
* 
* Public Interface:
* ----------------
* FileTransferutility<ClassName> FileTransfer { get; set; } = new FileTransferutility<ClassName>();
* bool result = FileTransfer.uploadFile(filename,destinationEndPointURL);
* bool result = FileTransfer.downloadFile(filename, string url)
* 
* Build History:
* ---------------
* Required Files : FileTransfer.cs
*   
* Maintenance History:
* --------------------
* ver 1.0 : 18 Nov 2016
* - first release
*/
/*
 * Note:
 * - Uses Programmatic configuration, no app.config file used.
 * - Uses ChannelFactory to create proxy programmatically. 
 * - Expects to find ToSend directory under application with files
 *   to send.
 * - Will create SavedFiles directory if it does not already exist.
 */

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WCFCommChannel
{
    // Class provides utility for uploading and downloading using WCF communication
    public class FileTransferutility<T>
    {
        public string name { get; set; } = typeof(T).Name;  // Name of the caller class
        ICommunicator channel;                              // Channel creation for sending files
        public string ToSendPath = "..\\..\\ToSend";        // Path from which files will be uploaded
        public string SavePath = "..\\..\\SavedFiles";      // Path from which files will be downlloaded
        int BlockSize = 1024;                               // defining BlockSize 
        byte[] block;

        // Constructor creating file block of size 1024
        public FileTransferutility()
        {
            block = new byte[BlockSize];
        }

        // Creating send channel to the mentioned url
        ICommunicator CreateServiceChannel(string url)
        {
            BasicHttpSecurityMode securityMode = BasicHttpSecurityMode.None;
            BasicHttpBinding binding = new BasicHttpBinding(securityMode);
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = 500000000;
            EndpointAddress address = new EndpointAddress(url);

            ChannelFactory<ICommunicator> factory
              = new ChannelFactory<ICommunicator>(binding, address);
            return factory.CreateChannel();
        }

        // utility method creates file stream and invokes the uploadFile method of Peer
        public bool uploadFile(string filename, string url)
        {
            string fqname = Path.Combine(ToSendPath, filename);
            try
            {
                // Creates input stream for the provided file and sends the stream data to Peer
                using (var inputStream = new FileStream(fqname, FileMode.Open))
                {
                    FileTransferMessage msg = new FileTransferMessage();
                    msg.filename = filename;
                    msg.transferStream = inputStream;
                    channel = CreateServiceChannel(url);
                    channel.upLoadFile(msg);
                }
                
                Console.Write("\n{0} Successfully Uploaded file \"{1}\" to {2}", name, filename, url);
                ((System.ServiceModel.Channels.IChannel)channel).Close();
                return true;
            }
            catch
            {
                Console.Write("\nCan't locate the file \"{0}\"", fqname);
                ((System.ServiceModel.Channels.IChannel)channel).Close();
                return false;
            }
        }

        // utility method - gets the stream from peer and downloads the entire file content
        public bool downloadFile(string filename, string url)
        {
            int totalBytes = 0;
            try
            {
                // Creates service channel and gets the stream from the Peer'd download file
                channel = CreateServiceChannel(url);
                Stream strm;
                try
                {
                    strm = channel.downLoadFile(filename);
                }
                catch(Exception)
                {
                    Console.WriteLine("file {0} not present in repository", filename);
                    ((System.ServiceModel.Channels.IChannel)channel).Close();
                    return false;
                }
               
                // Download the file block by block using the Peer's stream
                string rfilename = Path.Combine(SavePath, filename);
                if (!Directory.Exists(SavePath))
                    Directory.CreateDirectory(SavePath);
                using (var outputStream = new FileStream(rfilename, FileMode.Create))
                { 
                    while (true)
                    {
                        int bytesRead = strm.Read(block, 0, BlockSize);
                        totalBytes += bytesRead;
                        if (bytesRead > 0)
                            outputStream.Write(block, 0, bytesRead);
                        else
                            break;
                    }
                }
                Console.Write("\n  Received file \"{0}\" of {1} bytes ", filename, totalBytes );
            }
            catch (Exception ex)
            {
                Console.Write("\n Exception caught in method : downloadFile and message is : {0}", ex.Message);
                ((System.ServiceModel.Channels.IChannel)channel).Close();
                return false;
            }
            ((System.ServiceModel.Channels.IChannel)channel).Close();
            return true;
        }

#if (TEST_STUB)
        static void Main()
        {
            Console.Write("\n  Client of SelfHosted File Stream Service");
            Console.Write("\n ==========================================\n");

            FileTransferutility<string> clnt = new FileTransferutility<string>();
            clnt.channel = CreateServiceChannel("http://localhost:8000/StreamService");
            
            clnt.uploadFile("TestDriverVendingMachine1");
       
            
            clnt.downloadFile("test.txt");
         
            Console.Write("\n\n  Press key to terminate client");
            Console.ReadKey();
            
            ((IChannel)clnt.channel).Close();
        }
#endif
    }
}
