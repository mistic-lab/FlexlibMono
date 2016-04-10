//=================================================================
// hiperftimer.cs
//=================================================================
// Taken directly from a Code Projects article written by
// Daniel Strigl.
// http://www.codeproject.com/csharp/highperformancetimercshar.asp
//=================================================================

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Flex.Util
{
    public class HiPerfTimer
    {
        
		private long startTime, stopTime;
 

        // Constructor
        public HiPerfTimer()
        {
            startTime = 0;
            stopTime = 0;
		

           
        }

        // Start the timer
        public void Start()
        {
            // let the waiting threads do their work - start on fresh timeslice
            Thread.Sleep(0);
			startTime = DateTime.Now.Millisecond;
          
        }

        // Stop the timer
        public void Stop()
        {
            stopTime = DateTime.Now.Millisecond;
        }

        // Returns the duration of the timer (in seconds)
        public double Duration
        {
            get
            {
                return (double)(stopTime - startTime) / 1000;
            }
        }

        public double DurationMsec
        {
            get
            {
				return (double)((stopTime - startTime)); 
				}
        }        
    }
}