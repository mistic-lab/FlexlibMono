/*
 * 
 * For FlexLib assemblies see separate copyright. All Sources in namespace HB9FXQ are MIT
 * 
 * 

The MIT License (MIT)
Copyright (c) 2016 HB9FXQ, Frank Werner-Krippendorf mail@hb9fxq

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

*/



namespace HB9FXQ.DaxIqCat
{
    using System;
    using System.Linq;
    using System.Threading;

    class Program
	{

	    private const string NullPanaString = "0: 0.000000 USB [0,0]";


        public static void Main (string[] args)
		{

			if (4 != args.Length) {
				Console.WriteLine ("Util to subscribe to a Flexradio SmartSDR DAX IQ cannel and push raw (float) data to a udp socket.");
				Console.WriteLine ("");
				Console.WriteLine ("usage: HB9FXQ.DaxIqCat");
				Console.WriteLine("\t[samplerate(int) daxChannel(ushort) udpSocketAdr (string) socketPort (ushort)]");
				Console.WriteLine("eg. HB9FXQ.DaxIqCat 192000 1 \"127.0.0.1\" 5566");
				Environment.Exit (-1);
			}

			var wrapper = new DaxIqCat (
				Convert.ToInt32(args[0]), 
				Convert.ToUInt16(args[1]), 
				args[2],
				Convert.ToUInt16(args[3]));  // connects to the first radio detected !

		    var msg = "-1";

			while (true) {
             
				if (1>wrapper.ConnectedRadio.SliceList.Count()) continue;
                    
                    var currentInfo = wrapper.ConnectedRadio.SliceList.Aggregate(string.Empty,
                        (current, slice) => current + slice.ToString());

                    if (currentInfo != msg && !currentInfo.Contains(NullPanaString))
                    {
                        Console.WriteLine(Environment.NewLine + "* SLICES LIST       ---------------------");
                        Console.WriteLine(DateTime.UtcNow.ToLongDateString() + " " +
                                          DateTime.UtcNow.ToLongTimeString());
                        Console.WriteLine(currentInfo);
                        Console.WriteLine("*-----------------------------------------" + Environment.NewLine);
                        msg = currentInfo;
                        wrapper.PackageWriteNotifyDone = false;
                    }

                    Thread.Sleep(1000);
                
			}
		}
	}
}

