/***********************************************************************************************
 *  File name       :       TestDriverVendingMachine3.cs
 *  Function        :       Tests the functionality of fetch method in class VendingMachine
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 * *********************************************************************************************/
/*
*   Module Operations
*   -----------------
*   - Tests the functionality of fetch method in class VendingMachine
* 
*   Public Interface
*   ----------------
*   ITest IT = new ITest();
*   bool retValue = IT.test();
*   string logs = IT.getLogs();
*/
/*
 *   Build Process
 *   -------------
 *   - Required files:   TestDriverVendingMachine3.cs, ITest.cs, VendingMachine.cs
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

namespace TestDriverVendingMachine3
{
    using VendingMachine;   // test code is in the namespace VendingMachine
    using ITest;            // interface ITest available in the namespace VendingMachine

    public class TestDriverVendingMachine3 : ITest
    {
        string Log;         // stores the pass or failure logs
        VendingMachine vm;  // object for Test Code Class vending machine

        //---------------< Constructor - creates object for the test code >-------------------------------
        public TestDriverVendingMachine3()
        {
            Log = "TestDriverVendingMachine3 invoked";
            vm = new VendingMachine();
        }

        //---------< test() extends from interface ITest, tests the functionality of method fetch>------
        public bool test()
        {
            string drinks;
            drinks = vm.fetch('B');
            if (drinks.Equals("Beer"))
            {
                Log += "\n FETCH method passed for input B";
                return true;
            }

            Log += "\n FETCH method failed for input B Expected Output : Beer || Actual Output " + drinks;
            return false;
        }

        //------------< getLog() implements from interface ITest returns the stored log>----------------
        public string getLog()
        {
            return Log;
        }
    }

#if (TEST_STUB)
    class TestStub
    {
        //------------< Main function act as test stub for the class TestDriverVendingMachine3 >---------------
        public static void Main()
        {
            TestDriverVendingMachine3 td = new TestDriverVendingMachine3();
            bool testResult = td.test();
            Console.WriteLine(td.getLog());
            Console.ReadKey();
        }
    }
#endif
}