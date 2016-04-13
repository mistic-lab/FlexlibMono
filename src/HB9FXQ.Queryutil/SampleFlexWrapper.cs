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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flex.Smoothlake.FlexLib;
using Flex.Util;
using ZeroMQ;
using System.Net;
using System.Net.Sockets;

namespace HB9FXQ.DaxIqCat
{
	public class SampleFlexWrapper
	{     		
		private int samplerate;
		private string socketAdr;
		private ushort socketPrt;
		private ushort daxChannel;
	
		public SampleFlexWrapper(int samplerate_, ushort daxChannel_, string socketAdr_, ushort socketPrt_){


			samplerate = samplerate_;
			daxChannel = daxChannel_;
			socketAdr = socketAdr_;
			socketPrt = socketPrt_;

			// INIT Radio
			API.RadioAdded += API_RadioAdded;
			API.ProgramName = "daxncat";
			API.Init();

			// INIT Socket endpoint
			sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			endPoint = new IPEndPoint(IPAddress.Parse(socketAdr), socketPrt);

			Console.WriteLine ("Waiting for the radio");

			while (ConnectedRadio == null) {
			}

			Console.WriteLine ("Connected to " + ConnectedRadio);

		}


		private Socket sock;
		private IPEndPoint endPoint;
		public Radio ConnectedRadio { get; set;}

	

		private void API_RadioAdded(Radio radio)
		{
			Console.WriteLine("API_RadioAdded Fired:  " + radio.ToString());
			Console.WriteLine("Connection to radio....");
			radio.Connect();
			ConnectedRadio = radio;

			Console.WriteLine ("Waiting for panatapter list... ");

			while (!ConnectedRadio.PanadapterList.Any ()) {
				// wait
			}

			var firstPan = ConnectedRadio.PanadapterList.FirstOrDefault ();
			firstPan.DAXIQChannel = 0;
			firstPan.DAXIQChannel = daxChannel;

			var IQ = ConnectedRadio.CreateIQStream (1);
			IQ.DataReady += IQ_DataReady;
			IQ.RequestIQStreamFromRadio ();
			IQ.SampleRate = samplerate;
		}

		byte[] outBuffer = new byte[1024*4];

		bool pkgSeen = false;

		void IQ_DataReady (IQStream iq_stream, float[] data)
		{			
			ushort i = 0; 

			foreach(float rx in data){	
				
				foreach (var bt in BitConverter.GetBytes (rx)) {
					outBuffer [i++] = bt;
				}
			}

			sock.SendTo(outBuffer , endPoint);

			if(!pkgSeen){
				Console.WriteLine ("OK, writing IQ data...");
				pkgSeen = true;
			}

	     }
	}
}
