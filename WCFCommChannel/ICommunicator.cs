/***********************************************************************************************
 *  File name       :       ICommunicator.cs
 *  Function        :       Interface created and operator / service contract defined for 
 *                          WCF communication
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 *  Reference       :       Prof.Jim Fawcett, Project 4 Fall 16 Help code
 * *********************************************************************************************/

/*
 *   Module Operations
 *   -----------------
 *   ICommunicator class defines the operation contract for file transfer and message transfer
 *   across various WCF end point
 * 
 *   Public Interface
 *   ----------------
 *   ICommunicator IC = new ICommunicator();
 *   IC.PostMessage(msg);
 *   IC.upLoadFile(msg);
 *   Stream s1 = IC.downLoadFile(filename);
 *   Message m1 = IC.GetMessage();
 */
/*
 *   Build Process
 *   -------------
 *   - Required files: ICommunicator.cs  
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 18 Nov 2016
 *     - first release
 * 
 */

using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WCFCommChannel
{
    [ServiceContract]
    public interface ICommunicator
    {
        [OperationContract(IsOneWay = true)]
        void PostMessage(Message msg);
        // Method to upload and download files using WCF
        [OperationContract(IsOneWay = true)]
        void upLoadFile(FileTransferMessage msg);
        [OperationContract]
        Stream downLoadFile(string filename);
        // used only locally so not exposed as service method

        Message GetMessage();
    }
    // This class is implemented by Receiver<T> class in CommService.cs
}
