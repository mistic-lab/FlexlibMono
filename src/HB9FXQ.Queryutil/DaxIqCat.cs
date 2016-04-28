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
    using System.Net;
    using System.Net.Sockets;
    using System;
    using System.Linq;
    using Flex.Smoothlake.FlexLib;

    public class DaxIqCat
	{     		
		private readonly int _samplerate;       // dax channel sample rate
        private readonly ushort _daxChannel;    // dax channel number
        private readonly Socket _sock;          // socket, for outgoing data
        private readonly IPEndPoint _endPoint;  // IPEndPoint outgoing
        readonly byte[] _outBuffer = new byte[1024 * 4];    // buffor for outgoing IQ Data
        public bool PackageWriteNotifyDone { get; set; }

        public Radio ConnectedRadio { get; private set; }
        
        /// <summary>
        /// Retrieves IQ samples from the radio and pushes the raw (float) IQ data to a UDP endpint 
        /// </summary>
        /// <param name="samplerate">DAX channel sample rate</param>
        /// <param name="daxChannel">dax channel number</param>
        /// <param name="socketAdr">socket IP address</param>
        /// <param name="socketPrt">socket port number</param>
        public DaxIqCat(int samplerate, ushort daxChannel, string socketAdr, ushort socketPrt){

            _samplerate = samplerate;
			_daxChannel = daxChannel;

            // INIT Radio
			API.RadioAdded += API_RadioAdded;
			API.ProgramName = "daxncat";
			API.Init();

			// INIT Socket endpoint
			_sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_endPoint = new IPEndPoint(IPAddress.Parse(socketAdr), socketPrt);

			Console.WriteLine ("Waiting for the radio");

			while (ConnectedRadio == null) {
			}

			Console.WriteLine ("Connected to " + ConnectedRadio);
		}

        /// <summary>
        /// fired when the radio was added
        /// </summary>
        /// <param name="radio">reference to the added radio</param>
		private void API_RadioAdded(Radio radio)
		{
			Console.WriteLine("API_RadioAdded Fired:  {0}", radio);
			Console.WriteLine("Connecting to radio....");
			radio.Connect();
			ConnectedRadio = radio;

			Console.WriteLine ("Waiting for panatapter list... ");
		   
		    while (1>ConnectedRadio.PanadapterList.Count())
		    {
		        // wait for a panadapter
		    }
		  

		    var firstPan = ConnectedRadio.PanadapterList.FirstOrDefault ();

		    if (null == firstPan)
		    {
		        throw new Exception("Could not retrieve panadapter");
		    }

            firstPan.DAXIQChannel = 0;
			firstPan.DAXIQChannel = _daxChannel;

			var iq = ConnectedRadio.CreateIQStream (_daxChannel);

			iq.DataReady += IQ_DataReady;
			iq.RequestIQStreamFromRadio ();
			iq.SampleRate = _samplerate;
		}

        
        /// <summary>
        /// Called when IQ data for the subscriped IQ channel is ready
        /// </summary>
        /// <param name="iqStream">reference to the IQ stream</param>
        /// <param name="data"></param>
		void IQ_DataReady (IQStream iqStream, float[] data)
		{			
			ushort i = 0; 

			foreach (var bt in data.SelectMany(BitConverter.GetBytes))
			{
			    _outBuffer [i++] = bt;
			}

			_sock.SendTo(_outBuffer , _endPoint);
		    StdOutPackageNotify(iqStream);
		}

        private double prevCenterFreq = 0d; 

        private void StdOutPackageNotify(IQStream iqStream)
        {
            if (PackageWriteNotifyDone && prevCenterFreq.Equals(iqStream.Pan.CenterFreq)) return;

            prevCenterFreq = iqStream.Pan.CenterFreq; 
            Console.WriteLine("> Writing Packages to <{3}:{4}> DAX channel [{0} @{1} kHz] Center Freq: {2} MHz >", iqStream.DAXIQChannel, iqStream.SampleRate, iqStream.Pan.CenterFreq, _endPoint.Address, _endPoint.Port);
            PackageWriteNotifyDone = true;
        }
        
       
	}
}
