/***********************************************************************************************
 *  File name       :       Loader.cs
 *  Function        :       Reads the logger object and loads individual test cases
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 * *********************************************************************************************/
/*
*   Module Operations
*   -----------------
*    Looader module loads the TestDriver.dll file passed to it and checks whether it implements ITest.
*    Then it executes the bool test() method and collects the logs of the executed test case using getLog()
* 
*   Public Interface
*   ----------------
*  TestExecutor T = new TestExecutor(ReposiotryPath, TestRequest);
*  bool result = T.ProcessTest();
* 
*/
/*
 *   Build Process
 *   -------------
 *   - Required files:   Parser.cs, ITest.cs, TestDatabase.cs, TestExecutor.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 2.0 : 18 Nov 2016
 *     - updated the public interface docs info, Test Stub and comments
 *   ver 1.0 : 5 Oct 2016
 *     - first release
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loader
{
    using XMLParser;            // used in test stub
    using ITest;                // Checks whether test drivers are implemented from the interface ITest
    using Logger;               // Using logger object to store the logs
    using System.IO;            // used in test stub
    using System.Reflection;    // Using the Assembly class to load the test driver

    // TestExecutor loads all the test driver in the provided object and executes it in the order
    public class TestExecutor : MarshalByRefObject
    {
        string Repository = null;
        TestDatabase TestInfo = null;

        //---< constructor collects repository directory and the Object of TestDatabase
        public TestExecutor(string RepositoryPath, TestDatabase TestRequest)
        {
            Repository = RepositoryPath;    // Gets the Repository path where DLL files are stored
            TestInfo = TestRequest;         // Gets the object of TestDatabase 
        }

        // Loads all the test cases in the TestInfo object and updates the logs in them
        public bool ProcessTest()
        {
            foreach (TestCase Test in TestInfo.testResults)
            {
                // Test Execution will not happen if any of the DLL's are missing
                if (Test.isDLLPresent == false)
                {
                    // Update the test status is failed and log as DLL files are missed
                    Test.status = false;
                    Test.testLog = "Test Execution failed due to missing of DLL's related to Test case - " + Test.testName;
                }
                else
                {
                    // Execute the test case 
                    string TestDriver = Repository + @"\" + Test.testDriver;
                    Console.WriteLine("Executing the test driver " + Test.testDriver);
                    Assembly TestDriverAssembly = Assembly.LoadFrom(TestDriver);
                    Type[] TypesInDriver = TestDriverAssembly.GetExportedTypes();

                    foreach (Type TestType in TypesInDriver)
                    {
                        // Identifies the class that is extended from the ITest interface
                        if (TestType.IsClass && typeof(ITest).IsAssignableFrom(TestType))
                        {
                            ITest DriverObject = (ITest)Activator.CreateInstance(TestType);    // Create instance of test driver
                            Test.status = DriverObject.test();
                            Test.testLog = DriverObject.getLog();
                            Console.WriteLine("Class {0} implements the interface ITest", TestType.Name);
                            if (Test.status)
                            {
                                Console.WriteLine("Execution of Test Case {0} is PASSED", Test.testName);
                            }
                            else
                            {
                                Console.WriteLine("Execution of Test Case {0} is FAILED", Test.testName);

                            }
                            Test.timeStamp = DateTime.Now;  // updating the time stamp of execution of test cases
                            Console.WriteLine("Logs are stored for Test Name : {0} Author : {1} TimeStamp : {2}", Test.testName, TestInfo.author, Test.timeStamp);
                            Console.WriteLine("Logs from getLog() method of test driver are as follows");
                            Console.WriteLine("---------LOG STARTS HERE FOR TEST DRIVER  {0} ------------------", Test.testName);
                            Console.WriteLine(Test.testLog);
                            Console.WriteLine("---------LOG ENDS HERE FOR TEST DRIVER  {0} ------------------", Test.testName);
                        }
                    }

                }
            }
            return true;
        }

#if (TEST_STUB)
    // To Run the testStub Please follow the steps below
    // 1. Select Properties > Output Type as 'Console Application' save it
    // 2. Select Properties > Build > Conditional Compilation Steps - TEST_STUB
    // 3. Save and build the solution. 
    class testStub
    {
        //------------< Main function act as test stub for the class Test Logs >---------------
        public static void Main()
        {
            string PresentDirectory = Directory.GetCurrentDirectory(); // Current directory is TestHarness/bin/debug 

            // Hard Coding the path to the Repository 
            string PathToXmlFiles = Path.Combine(Directory.GetCurrentDirectory(), @"../../../../Repository/");
            string[] XMLFiles = Directory.GetFiles(PathToXmlFiles, "*.xml", SearchOption.AllDirectories);

            TestDatabase TR;
            foreach (string file in XMLFiles)
            {
                Parser ParsingObject = new Parser(file);
                TR = ParsingObject.DoParse();
                string Repository = Path.GetDirectoryName(file);         // Getting Repository path where TestDrivers and Test Code are stored
                TestExecutor Test = new TestExecutor(Repository, TR);   // Passing Repository path and logger object
                TR.ShowLogs();
            }
            Console.ReadKey();
        }
    }
#endif

    }
}