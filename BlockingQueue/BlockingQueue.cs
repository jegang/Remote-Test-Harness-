/****************************************************************************************************
 *  File name       :       BlockingQueue.cs
 *  Function        :       blockingQueue implementation for sender and receiver threads
 *  Application     :       Project # 4 - Software Modeling & Analysis                        
 *  Author          :       Jegan Gopalakrishnan, Reference : Dr.Jim Fawcett Project#4Help16 code
 * **************************************************************************************************/

/*
 *   Module Operations
 *   -----------------
 *   This package implements a generic blocking queue and demonstrates 
 *   communication between two threads using an instance of the queue. 
 *   If the queue is empty when a reader attempts to deQ an item then the
 *   reader will block until the writing thread enQs an item.  Thus waiting
 *   is efficient.
 * 
 *   NOTE:
 *   This blocking queue is implemented using a Monitor and lock, which is
 *   equivalent to using a condition variable with a lock.
 * 
 *   Public Interface
 *   ----------------
 *   BlockingQueue<string> bQ = new BlockingQueue<string>();
 *   bQ.enQ(msg);
 *   string msg = bQ.deQ();
 * 
 */
/*
 *   Build Process
 *   -------------
 *   - Required files:   BlockingQueue.cs
 * 
 *   Maintenance History
 *   -------------------
 *   ver 1.0 : 18 Nov 2016
 *     - first release
 * 
 */

//
using System;
using System.Collections;
using System.Threading;

namespace BlockingQueue
{
    
    public class BlockingQueue<T>
    {
        private Queue blockingQ;        
        object locker_ = new object();

        // <Constructor> - Creates a new queue
        public BlockingQueue()
        {
            blockingQ = new Queue();
        }

        // This method enqueues a string in the queue
        public void enQ(T msg)
        {
            lock (locker_)  // uses Monitor
            {
                blockingQ.Enqueue(msg);
                Monitor.Pulse(locker_);
            }
        }

        //   This method dequeues the message from the queue
        // Note that the entire deQ operation occurs inside lock.

        public T deQ()
        {
            T msg = default(T);
            lock (locker_)
            {
                while (this.size() == 0)
                {
                    Monitor.Wait(locker_);
                }
                msg = (T)blockingQ.Dequeue();
                return msg;
            }
        }
        
        // size() method returns the number of elements in the queue

        public int size()
        {
            int count;
            lock (locker_) { count = blockingQ.Count; }
            return count;
        }

        // clear() method removes all the remaining element in the queue
        public void clear()
        {
            lock (locker_) { blockingQ.Clear(); }
        }
    }

#if(TESTSTUB)
    // To Run the testStub Please follow the steps below
    // 1. Select Properties > Output Type as 'Console Application' save it
    // 2. Select Properties > Build > Conditional Compilation Steps - TEST_STUB
    // 3. Save and build the solution. 
  class Program
  {
    static void Main(string[] args)
    {
      Console.Write("\n  Blocking Queue Test Stub");
      Console.Write("\n ==========================");

      BlockingQueue<string> q = new BlockingQueue<string>();

      // Dequeueing the message in the child thread
      Thread t = new Thread(() =>
      {
        string msg;
        while (true)
        {
          msg = q.deQ(); Console.Write("\n  child thread received {0}", msg);
          if (msg == "quit") break;
        }
      });
      t.Start();
      string sendMsg = "msg #";
      for (int i = 0; i < 20; ++i)
      {
        string temp = sendMsg + i.ToString();
        Console.Write("\n  main thread sending {0}", temp);
        q.enQ(temp);
      }
      q.enQ("quit");
      t.Join();
      Console.Write("\n\n");
    }
  }
#endif
}
