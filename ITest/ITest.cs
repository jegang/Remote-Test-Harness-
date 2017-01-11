/*********************************************************************************************
 *  File name       :       ITest.cs
 *  Function        :       Contains interface ITest which every test driver implements
 *  Application     :       Project # 2 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 * ******************************************************************************************/
/*
*   Module Operations
*   -----------------
*   - ITest interface should be implemented by all the test drivers and two methods
*   - test() must returns true or false based on the pass or failure of test cases
*   - getLog() should return the logs of the test cases
*
*   Public Interface
*   ----------------
*       ITest T = new ITest();
*       bool result = T.test();
*       string logs = T.getLog();
*/      
/*
 *   Build Process
 *   -------------
 *   - Required files:   ITest.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 2.0 : 18 Nov 2016
 *     - updated the public interface docs info, Test Stub and comments
 *   ver 1.0 : 5 Oct 2016
 *     - first release
 * 
 */

namespace ITest
{
    //-------<< ITest interface implemented in every test driver >>--------------------------
    public interface ITest
    {
        bool test();        // Returns true - when test case passes , false - when test case fails
        string getLog();    // Returns the log of the test driver
    }
}


#if(TEST_STUB)
    class testStub
    {
        //---< Empty test stub interface is provided beloe, if in future any other class is added 
        //      to the package
        public static void Main()
        {
            // Nothing to test here
            Console.ReadKey();
        }
    }
#endif
