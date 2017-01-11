# Remote-Test-Harness- description
In this project I have developed an implementation for accessing and using a remote Test Harness server from multiple concurrent clients. 
The Test Harness will retrieve test code from a Repository server1.  One or more client(s) will concurrently supply the Test Harness with 
Test Requests. The Test Harness will request test drivers and code to test, as cited in a Test Request, from the Repository. 
One or more client(s) will concurrently extract Test Results and logs by enqueuing requests and displaying Test Harness and/or Repository replies2
when they arrive. 

The TestHarness, Repository, and Clients are all distinct projects that compile to separate executables. All communication between these 
processes will be based on message-passing Windows Communication Foundation (WCF) channels. Client activities will be defined by user 
actions in a Windows Presentation Foundation (WPF) user interface. 

On startup, Client, Repository, and TestHarness instances will demonstrate that all functional requirements are met with no input from 
the user. For that you may use a separate TestExecutive project if that seems appropriate.

/***********************************************************************************************
 *  File name       :       ReadMe.txt
 *  Function        :       Explains how to use the Test Harness Application                       
 *  Author          :       Jegan Gopalakrishnan
 *  Date            :       Nov 24 2016
 * *********************************************************************************************/
 
 Application of Project
 =======================
 *	Users places the test driver and test code dll in a folder and provides the directory location to the client GUI using browse button. 
 
 Steps to Run the Application:
 ===============================
 **Make sure you read the section below to create a XML and test driver**
 1. Place your XML file, TestDriver and TestCode Dlls in the any directory 
 2. Run the compile.bat file in Project4\RemoteTestHarness folder
 3. Run the run.bat file as administrator in Project4\RemoteTestHarness folder
 4. Select any one of client WPF window, click Browse button and choose the directory where you placed DLL and XML files
 5. Click "Run Test" button , the test result will be displayed in the GUI
 6. Click "Get Logs" button to view the complete logs of test execution
 7. Click "Exit" button to exit the client application
 
 Format of XML file should be as below
 =====================================
 <?xml version="1.0" encoding="utf-8" ?>				# header of the xml file
<testRequest>											# testRequest tag includes all the test cases
<author>Jegan Gopalakrishnan</author>					# Name of the author
<test name="TestDriverVendingMachine1">   				# Name of the test case
<testDriver>TestDriverVendingMachine1.dll</testDriver>	# Name of the test driver. Make sure you place test driver in the Project2\Repository\ folder
<library>VendingMachine.dll</library>					# Name of the test code.
</test>
<test name="TestDriverVendingMachine2">					
<testDriver>TestDriverVendingMachine2.dll</testDriver>
<library>VendingMachine.dll</library>
</test>
<test name="TestDriverVendingMachine3">
<testDriver>TestDriverVendingMachine3.dll</testDriver>
<library>VendingMachine.dll</library>
</test>
<test name="TestDriverVendingMachine3">
<testDriver>TestDriverVendingMachine4.dll</testDriver>
<library>VendingMachine.dll</library>
</test>
</testRequest>

How to create a TEST DRIVER
===========================

TestDriver should be implemented from a Interface ITest 
  //-------<< ITest interface implemented in every test driver >>--------------------------
    public interface ITest
    {
        bool test();        // Returns true - when test case passes , false - when test case fails
        string getLog();    // Returns the log of the test driver
    }
Make sure you have implented the test() and getLog() function in your Dlls


***********************************************************************************************
		THANKS FOR USING THE TEST HARNESS PROGRAM . HAVE A NICE DAY !!!!!!!!!!!!!
***********************************************************************************************

