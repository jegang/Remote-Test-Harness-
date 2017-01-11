/***********************************************************************************************
 *  File name       :       VendingMachine.cs
 *  Function        :       Simple VendingMachine functionality has 2 success and failure cases
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 * *********************************************************************************************/
/*
*   Module Operations
*   -----------------
*   - Simple functionality of VendingMachine containing two successfull and two failure scenarios
* 
*   Public Interface
*   ----------------
*   VendingMachine VM = new VendingMachine();
*   VM.fetch('A');
*/
/*
 *   Build Process
 *   -------------
 *   - Required files:   VendingMachine.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 2.0 : 18 Nov 2016
 *     - updated the public interface docs info, Test Stub and comments
 *   ver 1.0 : 5 Oct 2016
 *     - first release
 * 
 */
namespace VendingMachine
{
    public class VendingMachine
    {
        string[] drinks;            // Stores the some vending drinks name

        //------------< constructor initializes the drinks value  >----------
        public VendingMachine()
        {
            drinks = new string[] { "Appy", "Beer", "Coke", "Dew" };
        }

        //------------< returns the drinks based on the first letter of the drink  >-------------------
        public string fetch(char input)
        {
            switch (input)
            {
                case 'A':
                    return drinks[0];   // Success Scenario - returns Appy
                case 'B':
                    return drinks[0];   // Failure Scenario - returns Appy
                case 'C':
                    return drinks[2];   // Success Scenario - returns Coke
                case 'D':
                    return drinks[2];   // Success Scenario - returns Coke
                default:
                    return "Invalid";
            }
        }
    }

#if (TEST_STUB)
    class TestStub
    {
        //------------< Main function act as test stub for the class VendingMachine >---------------
        public static void Main()
        {
            VendingMachine vm = new VendingMachine();
            System.Console.WriteLine(vm.fetch('A'));
            System.Console.WriteLine(vm.fetch('C'));
        }
    }
#endif
}
