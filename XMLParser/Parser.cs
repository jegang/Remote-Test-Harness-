/***********************************************************************************************
 *  File name       :       Parser.cs
 *  Function        :       parses the given xml file and stores the log the the logger object
 *  Application     :       Project # 2 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 *  Date            :       Oct 5 2016
 * *********************************************************************************************/
using System;
using System.IO;        // using Path in the directory
using System.Linq;      // class is using IEnumerables
using System.Xml.Linq;  // using XDocument in the class

namespace XMLParser
{
    using Logger;           // Logger object is used to store the contents of xml file
    using System.Collections.Generic;   // creating queue

    //------< Parses the given xml file -----------------------------------------
    public class Parser : MarshalByRefObject
    {
        string XmlFile = null;          // Stores the file of xml
        TestDatabase TestRequest = null;// Parsed output is stored in the object
        string RepositoryPath = null;   // Path of the repository where DLLs and xmls are stored

        //----< constructor stores XmlFile, created TestRequest and gets repository--------------
        public Parser(string Xml)
        {
            XmlFile = Xml;
            TestRequest = new TestDatabase();
            RepositoryPath = Path.GetDirectoryName(XmlFile);
        }
        //-----< DoParse() parses and stores the xml data in logger object
        public TestDatabase DoParse()
        {
            XDocument Document;

            // This try catch is to handle the exception due to read permission of the xml file
            try
            {
                Document = XDocument.Load(XmlFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught Exception in the XMLParser class {0}", e.Message);
                return TestRequest;
            }
            //List<TestCase> TestCases = new List<TestCase>();
            // stores author, xmlName and testResults from xml file
            TestRequest.author = Document.Descendants("author").First().Value;
            TestRequest.xmlName = Path.GetFileName(XmlFile);
            TestRequest.testResults = new List<TestCase>();

            XElement[] xtests = Document.Descendants("test").ToArray();
            int numTests = xtests.Count();

            // Iterates and finds all the testcode dll's available
            for (int i = 0; i < numTests; ++i)
            {
                TestCase TC = new TestCase();
                TC.isDLLPresent = true;             // Initializing the DLLpresent as true

                TC.testCode = new List<string>();
                TC.testName = xtests[i].Attribute("name").Value;
                TC.testDriver = xtests[i].Element("testDriver").Value;

                // check whether driver exists in repository
                string DriverPath = RepositoryPath + @"\" + TC.testDriver;

                // If TestDriver dll is missing then update isDLLPresent as false
                if (!(System.IO.File.Exists(DriverPath)))
                {
                    Console.WriteLine("Driver File does not exist" + DriverPath);
                    TC.isDLLPresent = false;
                }

                IEnumerable<XElement> xtestCode = xtests[i].Elements("library");

                // iterates and finds test code dll names in the logger object
                foreach (var xlibrary in xtestCode)
                {
                    string testCodePath = RepositoryPath + @"\" + (xlibrary.Value); // Forming the complete file path
                    // If Testcode dll is missing then update isDLLPresent as false
                    if (!(System.IO.File.Exists(testCodePath)))
                    {
                        Console.WriteLine("Code file doesnot exist" + testCodePath);
                        TC.isDLLPresent = false;
                    }

                    TC.testCode.Add(xlibrary.Value);
                }
                // Appending on the testResults list
                TestRequest.testResults.Add(TC);
            }
            return TestRequest;
        }
    }

#if (TEST_STUB)
    // To Run the testStub Please follow the steps below
    // 1. Select Properties > Output Type as 'Console Application' save it
    // 2. Select Properties > Build > Conditional Compilation Steps - TEST_STUB
    // 3. Save and build the solution. 
    class testStub
    {
        //---< Main function act as test stub for the class LoadXMLtoQueue and XMLQueue >---
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

                Console.WriteLine(" ************************************************************ ");
                Console.Write("\n  {0,12} : {1}", "TEST NAME", TR.xmlName);
                Console.Write("\n  {0,12} : {1}", "AUTHOR", TR.author);
                Console.WriteLine(" \n*********************************************************** ");
                foreach (TestCase TC in TR.testResults)
                {
                    Console.Write("\n {0,12} : {1}", "TEST CASE", TC.testName);
                    Console.Write("\n {0,12} : {1}", "TEST DRIVER", TC.testDriver);
                    foreach (string testCode in TC.testCode)
                        Console.Write("\n {0,12} : {1}", "TEST CODE", testCode);
                    Console.WriteLine(" \n=========================================================== ");
                }
            }
            Console.ReadKey();
        }
    }
#endif
}
