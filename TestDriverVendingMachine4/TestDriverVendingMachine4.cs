/***********************************************************************************************
 *  File name       :       TestDriverVendingMachine4.cs
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
 *   - Required files:   TestDriverVendingMachine4.cs, ITest.cs, VendingMachine.cs
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

namespace TestDriverVendingMachine4
{
    using VendingMachine;   // test code is in the namespace VendingMachine
    using ITest;            // interface ITest available in the namespace VendingMachine

    public class TestDriverVendingMachine4 : ITest
    {
        string Log;         // stores the pass or failure logs
        VendingMachine vm;  // object for Test Code Class vending machine

        //---------------< Constructor - creates object for the test code >-------------------------------
        public TestDriverVendingMachine4()
        {
            Log = "TestDriverVendingMachine4 invoked";
            vm = new VendingMachine();
        }

        //---------< test() extends from interface ITest, tests the functionality of method fetch>------
        public bool test()
        {
            string drinks;
            drinks = vm.fetch('C');
            if (drinks.Equals("Coke"))
            {
                Log += "\n FETCH method passed for input C";
                return true;
            }

            Log += "\n FETCH method failed for input C Expected Output : Coke || Actual Output " + drinks;
            return false;
        }

        //------------< getLog() implements from interface ITest returns the stored log>----------------
        public string getLog()
        {
            return Log;
        }
    }

    class TestStub
    {
        //------------< Main function act as test stub for the class TestDriverVendingMachine4 >---------------
        public static void Main()
        {
            TestDriverVendingMachine4 td = new TestDriverVendingMachine4();
            bool testResult = td.test();
            Console.WriteLine(td.getLog());
            Console.ReadKey();
        }
    }
}