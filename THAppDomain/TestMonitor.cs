/***********************************************************************************************
 *  File name       :       THAppDomain.cs
 *  Function        :       monitors parsing, loading and running the testRequest
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 * *********************************************************************************************/
/*
*   Module Operations
*   -----------------
*   - Calls the parser class to parse the Test XML and then invokes the loader class to load and run
*     the xml files. It returns the logger object back to the main domain
* 
*   Public Interface
*   ----------------
*   TestMonitor TM = new TestMonitor();
*   TestDatabase Tb = TM.ProcessTestRequest();
*/
/*
 *   Build Process
 *   -------------
 *   - Required files:   TestExecutor.cs, TestDatabase.cs, Parser.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 2.0 : 18 Nov 2016
 *     - updated the public interface docs info, Test Stub and comments
 *   ver 1.0 : 5 Oct 2016
 *     - first release
 * 
 */
using System;

// THAppDomain is injected in  to the child App Domain
namespace THAppDomain
{
    using Logger;       // Using TestDatabase Class for storing logs
    using System.IO;    // Using Path class for retrieving directory of repository
    using ITest;
    using XMLParser;    // Parser class is used to parse the xml file and stores the result in the logger class
    using Loader;       // Loader loads the test drivers and executes the test cases

    //  Class TestMonitor uses Parser , loader to parse the xmls and execute the test cases
    public class TestMonitor : MarshalByRefObject
    {
        AppDomain childDomain = null;    // stores the name of the App Domain where TestMonitor runs

        //---< Constructor initiates the current domain and displays the same
        public TestMonitor()
        {
            childDomain = AppDomain.CurrentDomain;
            Console.Write("\nApplication Domain created {0}\n", childDomain.FriendlyName);
        }

        // This method calls the Parser class and then the Loader to execute the test cases
        public TestDatabase ProcessTestRequest(string XmlFile, TestDatabase logger)
        {
            // Parser parser the xmlfile content and stores the data in the logger object

            Parser ParserObj = new Parser(XmlFile);  // creating object for parser class
            logger = ParserObj.DoParse();            // returns parser object with all the xml contents stored

            Console.WriteLine("Test Requests are processed in the child App Domain {0}", childDomain);
            string Repository = Path.GetDirectoryName(XmlFile);         // Getting Repository path where TestDrivers and Test Code are stored
            TestExecutor Test = new TestExecutor(Repository, logger);   // Passing Repository path and logger object

            Test.ProcessTest();         // Executes all the test cases in the logger object and updates the same with results

            return logger;
        }
    }

#if (TEST_STUB)
    // To Run the testStub Please follow the steps below
    // 1. Select Properties > Output Type as 'Console Application' save it
    // 2. Select Properties > Build > Conditional Compilation Steps - TEST_STUB
    // 3. Save and build the solution. 
    class testStub
    {
        //---< Main function act as test stub for the class TestMonitor >---
        public static void Main()
        {
            // Hard Coding the path to the Repository 
            string PathToXmlFiles = Path.Combine(Directory.GetCurrentDirectory(), @"../../../../Repository/");
            Console.WriteLine("All DLL and XML files are placed in below Repository \n" + PathToXmlFiles);
            
            // Gettin the path of the xml files
            string[] XMLFiles = Directory.GetFiles(PathToXmlFiles, "*.xml", SearchOption.AllDirectories);

            Console.Write("\nCreating Application Domain - THAppDomain");
            Console.Write("\n===========================================");

            AppDomain ChildDomain = null;               // used to store app domain object
            TestDatabase logger = new TestDatabase();   // xml parsed output will be stored here

            try
            {
                AppDomain main = AppDomain.CurrentDomain;
                Console.WriteLine("\nMain AppDomain is " + main.FriendlyName);

                /* Create application domain setup information for new AppDomain */
                AppDomainSetup DomainInfo = new AppDomainSetup();

                /* Defines search path for assemblies */
                DomainInfo.ApplicationBase = "file:///" + System.Environment.CurrentDirectory;

                /* Create Child AppDomain TestHarnessAppDomain */
                ChildDomain = AppDomain.CreateDomain("TestHarnessAppDomain", null, DomainInfo);

                ChildDomain.Load("THAppDomain");

                /* Creating instance of the THAppDomain */
                ObjectHandle Obj = ChildDomain.CreateInstance("THAppDomain", "THAppDomain.TestMonitor");
                THAppDomain.TestMonitor TestMonitorObj = (THAppDomain.TestMonitor)Obj.Unwrap();

                foreach (string path in XMLFiles)
                {
                    // ProcessTestRequest manages parsing and executing the test cases
                    logger = TestMonitorObj.ProcessTestRequest(path, logger);    // logger object holds the logs for all test cases

                    // console logs for the test case 
                    Console.WriteLine("Client Queries can be handled here. As this project is a single threaded process displaying the logs without client request");
                    logger.ShowLogs();
                }
            }
            catch (Exception except)
            {
                Console.Write("\nException caught : {0}\n", except.Message);
                Console.Write("\nEnding prematurely, but shutting down normally\n\n");
            }

            //Unloading the AppDomain
            AppDomain.Unload(ChildDomain);
            Console.ReadKey();
        }
    }
#endif
}
