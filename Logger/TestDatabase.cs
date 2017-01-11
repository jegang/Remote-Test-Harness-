/***********************************************************************************************
 *  File name       :       TestDatabase.cs
 *  Function        :       Stores the content of test request and it's logs
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 * *********************************************************************************************/
/*
*   Module Operations
*   -----------------
*   Provides the class structure to store the contents of the xml file and test results
*   It also provides functionality to show the logs too.
* 
*   Public Interface
*   ----------------
*   TestDatabase T = new TestDatabase();
*   T.ShowLogs();
*   string LogContents = T.CreateLogs();
*/
/*
 *   Build Process
 *   -------------
 *   - Required files:   TestDatabase.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 2.0 : 19 Nov 2016
 *     - updated the public interface docs info, Test Stub and comments
 *     - Added CreateLogs() method to return the log content as string
 *   ver 1.0 : 5 Oct 2016
 *     - first release
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Logger
{
    //---< class TestCase contains the data for each test case in a test Request and it's logs
    public class TestCase : MarshalByRefObject
    {
        public string testName { get; set; }            // Name of the test case
        public string testDriver { get; set; }          // Name of test driver 
        public List<string> testCode { get; set; }      // Names of all the test code to be tested by test driver
        public bool isDLLPresent { get; set; }          // check all DLLS for test case present or not
        public bool status { get; set; }                // PASS / FAIL status of the test case
        public string testLog { get; set; }             // Logs returned by the testDriver
        public DateTime timeStamp { get; set; }         // stores timeStamp of completion of test case
    }
    //----< class TestDatabase act as the DB for the TestRequest
    public class TestDatabase : MarshalByRefObject
    {
        public string xmlName { get; set; }             // Name of the XML file or testRequest
        public string author { get; set; }              // author of the XML file
        
        public List<TestCase> testResults { get; set; } // data of each test case in the xml file

        public int TotalTestCases { get; set; }         // counts the number of test case   
        public int NumOfTestCasesPassed { get; set; }   // stores the count of passed test cases
        public int NumOfTestCasesFailed { get; set; }   // stores the count of failed test cases

        //----< Create Log returns the log content as strings
        public StringBuilder CreateLogs()
        {
            StringBuilder LOG = new StringBuilder();
            TotalTestCases = testResults.Count;
            NumOfTestCasesPassed = 0;
            NumOfTestCasesFailed = 0;
            LOG.Append("**********************************************************************");
            LOG.AppendLine();
            LOG.Append("TEST NAME       : " + xmlName);
            LOG.AppendLine();
            LOG.Append("AUTHOR          : "+ author);
            LOG.AppendLine();
            LOG.Append("**********************************************************************");
            LOG.AppendLine();
            foreach (TestCase TC in testResults)
            {
                LOG.Append("TEST CASE       : " + TC.testName);
                LOG.AppendLine();
                LOG.Append("TEST DRIVER     : " +TC.testDriver);
                LOG.AppendLine();
                foreach (string testCode in TC.testCode)
                {
                    LOG.Append("TEST CODE       : " + testCode);
                    LOG.AppendLine();
                }
                LOG.Append("EXECUTED TIME   : " + TC.timeStamp);
                LOG.AppendLine();
                if (TC.status == true)
                    NumOfTestCasesPassed += 1;
                else
                    NumOfTestCasesFailed += 1;
                LOG.Append("TEST RESULT     : " + ((TC.status == true) ? "PASS" : "FAIL"));
                LOG.AppendLine();
                LOG.Append("TEST LOGS       : "+TC.testLog);
                LOG.AppendLine();
                LOG.Append("**********************************************************************");
                LOG.AppendLine();
            }
            return LOG;
        }
        
        //---< ShowLogs() displays the content of TestCase and TestLogs structure
        public void ShowLogs()
        {
            StringBuilder Log = CreateLogs();
            Console.WriteLine(Log);
        }
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
            TestDatabase TL = new TestDatabase();
            TL.author = "Jegan Gopalakrishnan";
            TL.xmlName = "VendingMachine";
            TestCase TC = new TestCase();
            TC.status = true;
            string str = "VendingMachine.dll";
            TC.testCode = new List<string>();
            TC.testCode.Add(str);
            TC.testName = "Appy Drinks";
            TC.timeStamp = DateTime.Now;
            TC.testLog = "Test case passed successfully for APPY input";
            TC.testDriver = "TestDriverVendingMachine.dll";
            TL.testResults = new List<TestCase>();
            TL.testResults.Add(TC);
            TL.ShowLogs();
            Console.ReadKey();
        }
    }
#endif
}
