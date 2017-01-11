/***********************************************************************************************
* File name       :       HiResolutionTimer.cs
* Function        :       Contains the utility function to calculate the time taken
* Application     :       Project # 4 - Software Modeling & Analysis                        
* Author          :       Jegan Gopalakrishnan
* Reference       :       Prof.Jim Fawcett, Project 4 Fall 16 Help code
 **********************************************************************************************/

/*
 * Package Operations:
 * -------------------
 * Create object for the class and use Start() and Stop() method - the execution time taken
 * between these two function can be displayed in obj.ElapsedMicroseconds
 * 
 * Build Process: 
 * --------------- 
 * Required Files:  HiResolutionTimer.cs
 * 
 * Public Interface:
 * -----------------
 * HiResTimer hrt = new HiResTimer();
 * hrt.Start();
 * hrt.Stop();
 *
 * Maintenance History:
 * --------------------
 * Ver 1.0 : 18 Nov 2016
 * - first release  
 */

using System;
using System.Runtime.InteropServices; // for DllImport attribute
using System.ComponentModel; // for Win32Exception class
using System.Threading; // for Thread.Sleep method

namespace WPFClient
{
    public class HiResTimer
    {
        protected ulong a, b, f;

        // constructor initializes the local variable
        public HiResTimer()
        {
            a = b = 0UL;
            if (QueryPerformanceFrequency(out f) == 0)
                throw new Win32Exception();
        }

        // Gives the ElapsedTicks between start and stop method invocation
        public ulong ElapsedTicks
        {
            get
            { return (b - a); }
        }

        // Gives the ElapsedTicks between start and stop method invocation in microseconds
        public ulong ElapsedMicroseconds
        {
            get
            {
                ulong d = (b - a);
                if (d < 0x10c6f7a0b5edUL) // 2^64 / 1e6
                    return (d * 1000000UL) / f;
                else
                    return (d / f) * 1000000UL;
            }
        }

        //Gives the ElapsedTimeSpan between start and stop method invocation
        public TimeSpan ElapsedTimeSpan
        {
            get
            {
                ulong t = 10UL * ElapsedMicroseconds;
                if ((t & 0x8000000000000000UL) == 0UL)
                    return new TimeSpan((long)t);
                else
                    return TimeSpan.MaxValue;
            }
        }

        // Gives the frequency
        public ulong Frequency
        {
            get
            { return f; }
        }
        
        // Start method starts the timer
        public void Start()
        {
            Thread.Sleep(0);
            QueryPerformanceCounter(out a);
        }

        // Stop method stops the timer
        public ulong Stop()
        {
            QueryPerformanceCounter(out b);
            return ElapsedTicks;
        }

        // Here, C# makes calls into C language functions in Win32 API
        // through the magic of .Net Interop

        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern
           int QueryPerformanceFrequency(out ulong x);

        [DllImport("kernel32.dll")]
        protected static extern
           int QueryPerformanceCounter(out ulong x);
    }
}
#if (TEST_STUB)
public static void Main()
{
    HiResTimer hrt = new HiResTimer();
    hrt.Start();
    Thread.Sleep(100);
    hrt.Stop();
    String msgContent = msg.body + "\n" + $"Time Taken for execution {hrt.ElapsedMicroseconds/1000} milliseconds  Approximately {(float)hrt.ElapsedMicroseconds/(float)1000000} seconds";
    global::System.Console.WriteLine(msgContent);
}
#endif