/***********************************************************************************************
 *  File name       :       LoadXMLToQ.cs
 *  Function        :       Tests the functionality of fetch method in class VendingMachine
 *  Application     :       Project # 2 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan
 *  Date            :       Oct 5 2016
 * *********************************************************************************************/


using System;
using System.Collections;  /* uses the Queue from this collections */
using System.IO;


namespace LoadXMLToQ
{
    //-------------<Wrapper class for in built queue>--------------------
    public class XMLQueue<T>
    {
        private Queue XmlQ;     // stores xml file names

        //-------------<constructor creates object for XmlQ>--------------------
        public XMLQueue()
        {
            XmlQ = new Queue();
        }

        //-------------<XMLEnQ() adds the xml file into the queue>----------------
        public void XMLEnQ(T XmlFile)
        {
            if (XmlFile != null)
            {
                Console.WriteLine("\nAdded xml file to Queue : {0}", XmlFile);
                XmlQ.Enqueue(XmlFile);
            }
        }

        //-------------<XMLDeQ() dequeues the file from the queue>-------------------------------
        public T XMLDeQ()
        {
            T XmlFile = (T)XmlQ.Dequeue();
            Console.WriteLine("\nDequeued xml file from Queue : {0}", XmlFile);
            return XmlFile;
        }

        //-------------<XMLQSize() returns the number of strings in the queue>--------------------
        public int XMLQSize()
        {
            return (XmlQ.Count);
        }
    }

    //--< class contains methods to invoke XMLQueue class to add xml files in specific directory>---
    public class LoadXMLtoQueue
    {
        private XMLQueue<string> Q;     // object for wrapper class XMLQueue

        //-------------< constructor stores queue value in the object>--------------------
        public LoadXMLtoQueue(XMLQueue<string> queue)
        {
            Q = queue;
        }

        //--------------< LoadXmlFiles() finds xml files in the given path and stores their complete name
        //--------------< in XMLQueue
        public int LoadXmlFiles(string PathToXmlFiles)
        {
            //try -catch identifies if repository directory is missing
            try
            {
                // Gets all .xml files in the given directory 
                string[] XMLFiles = Directory.GetFiles(PathToXmlFiles, "*.xml", SearchOption.AllDirectories);

                foreach (string file in XMLFiles)
                {
                    Q.XMLEnQ(file);     // adds complete filepath in the queue
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught in fetching the xml files, Exception message is  : " + e.Message);
            }
            int QueueSize = Q.XMLQSize();
            return QueueSize;       // returns the number of element added to the queue

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

            // Calling XMLQueuing to add all the xml files into the queue 
            XMLQueue<string> xmlQ = new XMLQueue<string>();

            LoadXMLtoQueue load = new LoadXMLtoQueue(xmlQ);
            int QueueSize = load.LoadXmlFiles(PathToXmlFiles);

            Console.WriteLine("Number of elements added in the queue is "+ QueueSize);
            Console.WriteLine(xmlQ.XMLQSize());
            
            Console.ReadKey();
        }
    }
#endif
}
