/***********************************************************************************************
 *  File name       :       TestHarnessManager.cs
 *  Function        :       Manages Queueing of xml files and running the test requests 
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 *  Reference       :       Prof.Fawcett's Fall 2016 Project 4 Help Code
 * *********************************************************************************************/

/*
*   Module Operations
*   -----------------
*   - Identifies the testRequest xml in the provided location.
*   - downloads all the test drivers mentioned in the xml from the repository
*   - If any test driver/ test code dll is missing in the repository, sends error message to client
*   - If all files are avaialble, it creates child app domain to execute the test cases
*   - After successfull execution of test cases, it sends test result and test logs to repository
*     and test results to client
* 
*   Public Interface
*   ----------------
*   TestHarnessManager.Execute(PathToXMLFile, PostMessageDelegate);
*/
/*
 *   Build Process
 *   -------------
 *   Required Files : TestHarness.cs , TestMoniotor.cs, TestDatabase.cs
 *                    ICommunicator.cs, CommServices.cs, Messages.cs, FileTransfer.cs
 *                    
 *   Maintenance History
 *   -------------------
 *   ver 2.0 : 18 Nov 2016
 *     - updated the public interface docs info, Test Stub and comments
 *     - Added CreateLogs and downloadRequiredFiles method in the class
 *   ver 1.0 : 5 Oct 2016
 *     - first release
 * 
 */

using System;
using System.IO;                    // using Path class to identify the current directory
using System.Security.Policy;       //uses to create evidence for CreateDomain
using System.Runtime.Remoting;      // using ObjectHandle to wraps marshal-by-ref-object
using Logger;                       // Creating using object of class TestDatabase in logger assembly
using System.Xml.Linq;              // XDocument usage
using System.Collections.Generic;   // Creation of linked list
using System.Linq;                  // To query from the xml file
using WCFCommChannel;               // Contains the method for download and upload file
using System.Text;                  // used Path.GetFileName or GetDirectoryName

namespace TestHarness
{
    // TestHarnessManager provides method for all operations mentioned under module operations
    class TestHarnessManager
    {
        // FileTransferutility holds method to download and upload file in Repository
        public FileTransferutility<TestHarness> FileTransfer { get; set; } = new WCFCommChannel.FileTransferutility<TestHarness>();
        // Repository endpoint address - 8083
        public string RepositoryEndPoint { get; } = Comm<TestHarness>.makeEndPoint("http://localhost", 8083);

        // Creating TestResult and TestLogs from the logger object 
        // and sending them to Repository and returns the TestResult string
        public string CreateLogs(string xml, TestDatabase logger)
        {
            StringBuilder Logs = logger.CreateLogs();
            StringBuilder Results = new StringBuilder();
            Results.AppendLine("***********************************************************************");
            Results.AppendLine("AUTHOR      : " + logger.author);
            Results.AppendLine("***********************************************************************");
            Results.AppendLine("Total Number of Test Cases  : " + logger.TotalTestCases);
            Results.AppendLine("Number of Test Cases Passed : " + logger.NumOfTestCasesPassed);
            Results.AppendLine("Number of Test Cases Failed : " + logger.NumOfTestCasesFailed);
            Results.AppendLine("***********************************************************************");
            Results.AppendLine("Press 'Get Logs' button to check the complete logs");
            Results.AppendLine("***********************************************************************");

            // Naming the testlog and testdriver with the author name and timestamp combination
            String xmlFile = Path.GetFileName(xml);
            string TestLog = xmlFile.Split('.')[0] + "_TESTLOG.txt";
            string TestResult = xmlFile.Split('.')[0] + "_TESTRESULT.txt";

            //writing the logs into the temporary directory
            System.IO.File.WriteAllText(Path.GetDirectoryName(xml) + "\\" + TestLog, Logs.ToString());
            System.IO.File.WriteAllText(Path.GetDirectoryName(xml) + "\\" + TestResult, Results.ToString());

            Console.WriteLine("Test Result : {0} and Test Log {1} are stored in Test Harness in {2}", TestLog, TestResult, Path.GetDirectoryName(xml));
            // Sending the TEST LOGS and TEST RESULTS files back to the repository
            Console.WriteLine("Uploading the TestLog : {0} and Test Result : {1} to Repository {2}",TestLog, TestResult, RepositoryEndPoint);
            FileTransfer.ToSendPath = Path.GetDirectoryName(xml) + "\\";
            FileTransfer.uploadFile(TestLog, RepositoryEndPoint);
            FileTransfer.uploadFile(TestResult, RepositoryEndPoint);

            return Results.ToString();
        }

        // Downloads all the test drivers / code mentioned in xmlfile from the repository
        public bool downloadRequiredFiles(string LocalDLLPath, string XMLFile, ref string missingFile)
        {
            XDocument Document;
            // This try catch is to handle the exception due to read permission of the xml file
            try
            {
                Document = XDocument.Load(XMLFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught Exception in the XMLParser class {0}", e.Message);
                return false;
            }

            XElement[] xtests = Document.Descendants("test").ToArray();

            FileTransfer.SavePath = LocalDLLPath;
            // Iterates and finds all the testcode dll's available
            for (int i = 0; i < xtests.Count(); ++i)
            {
                string TestDriver = xtests[i].Element("testDriver").Value;
                // download the testdriver from repository

                Console.WriteLine("\nDownloading.... File {0} from Repository", TestDriver);
                bool FileAvailable = FileTransfer.downloadFile(TestDriver, RepositoryEndPoint);
                if (FileAvailable == false)
                {
                    missingFile = TestDriver;
                    return false;
                }

                IEnumerable<XElement> xtestCode = xtests[i].Elements("library");

                // iterates and finds test code dll names in the xml
                foreach (var xlibrary in xtestCode)
                {
                    Console.WriteLine("\nDownloading.... File {0} from Repository", xlibrary.Value.ToString());
                    //download the testcode from xml file
                    FileAvailable = FileTransfer.downloadFile(xlibrary.Value.ToString(), RepositoryEndPoint);
                    if (FileAvailable == false)
                    {
                        missingFile = xlibrary.Value.ToString();
                        return false;
                    }
                }
            }
            return true;
        }

        //---< CreateAppDomain creates new application domain and injects the TestMonitor class to it-----
        public string RunTestRequestInAppDomain(string path)
        {
            Console.Write("\nCreating Application Domain - THAppDomain");
            Console.Write("\n===========================================");

            AppDomain ChildDomain = null;               // used to store app domain object
            TestDatabase logger = new TestDatabase();   // xml parsed output will be stored here
            string TestResult = "";

            try
            {
                AppDomain main = AppDomain.CurrentDomain;
                Console.WriteLine("\nMain AppDomain is " + main.FriendlyName);

                /* Create application domain setup information for new AppDomain */
                AppDomainSetup DomainInfo = new AppDomainSetup();

                /* Defines search path for assemblies */
                DomainInfo.ApplicationBase = "file:///" + System.Environment.CurrentDirectory;

                /* From Current AppDomain creating evidence for new App Domain */
                Evidence AppDomainEvidence = AppDomain.CurrentDomain.Evidence;

                /* Create Child AppDomain TestHarnessAppDomain */
                ChildDomain = AppDomain.CreateDomain("TestHarnessAppDomain", AppDomainEvidence, DomainInfo);

                ChildDomain.Load("THAppDomain");

                /* Creating instance of the THAppDomain */
                ObjectHandle Obj = ChildDomain.CreateInstance("THAppDomain", "THAppDomain.TestMonitor");
                THAppDomain.TestMonitor TestMonitorObj = (THAppDomain.TestMonitor)Obj.Unwrap();

                // ProcessTestRequest manages parsing and executing the test cases
                logger = TestMonitorObj.ProcessTestRequest(path, logger);    // logger object holds the logs for all test cases

                // console logs for the test case 
                logger.ShowLogs();
                TestResult = CreateLogs(path, logger);
            }
            catch (Exception except)
            {
                Console.Write("\nException caught : {0}  => while executing XML {1}\n", except.Message, path);
                Console.Write("\nEnding prematurely, but shutting down normally\n\n");
            }

            //Unloading the AppDomain
            AppDomain.Unload(ChildDomain);
            return TestResult;
        }

        // Invokes the RunTestRequestInAppDomain to run the testRequest and sends reply back to client
        public static void Execute(string PathToXmlFiles, Action<string, string> PostResult)
        {
            Console.WriteLine("------------------------TEST HARNESS------------------------------ ");

            string[] XMLFiles = Directory.GetFiles(PathToXmlFiles, "*.xml");
            // Hard Coding the path to the Repository 

            if (XMLFiles.Length == 0)
            {
                Console.WriteLine("No XML Files available in the directory {0}", PathToXmlFiles);
                return;
            }
            else
            {
                // Print the number of test requests
                Console.Write("\n {0} XML file is available in the path {1}: ", XMLFiles.Length, PathToXmlFiles);
            }
            TestHarnessManager THM = new TestHarnessManager();

            // Download all the required DLL's from the repository
            string missingFile = "";
            bool status = THM.downloadRequiredFiles(PathToXmlFiles, XMLFiles[0], ref missingFile);
            if (status == false)
            {
                string errMsg = $"DLL File -** {missingFile} ** is not available in repository";
                Console.WriteLine(errMsg);
                Console.WriteLine("Test Harness sending error message (notification) back to client");
                PostResult("Notification", errMsg);
                return;
            }

            // method creates app domain and displays the userlog
            string AppResult = THM.RunTestRequestInAppDomain(XMLFiles[0]);
            if (AppResult != null)
            {
                Console.WriteLine("\n\n**********TEST EXECUTION IS SUCCESS IN CHILD APP DOMAIN***********");
                Console.WriteLine("***************THANKS FOR USING TEST HARNESS**********************");
                //Sending the test Result back to Client
                PostResult("TestResult", AppResult);
            }
        }
#if (TEST_STUB)
        public static void Main(string[] args)
        {
            Console.WriteLine("TestHarness.cs already contains the main method which tests this class functionality");
        }
#endif
    }
}
