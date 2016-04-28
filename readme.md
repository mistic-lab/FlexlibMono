![alt tag](https://travis-ci.org/krippendorf/FlexlibMono.svg?branch=master)

#Mono/Linux compatible version of Flexlib
A few experiments with Flexlib on mono.
Currently only short experiments and snippets.

# Lincence
* For Flexradio assemblies see the Flexlib licence.
* All code in Namespace HB9FXQ is MIT licenced.


There is sample code from the original flexlib package in the repo that isn't part of the Mono solution. To start, use the solution file in the src folder. 

## List of utils that exist so far: 

### HB9FXQ.DaxIqCat
Use this to subscribe to a DAX IQ Channel and push raw data to an UDP socket.... a rename will follow :) 
The util can be used while SmartSDR running on any other - or - the same machine.

Samples:

For both samples, start the util in a shell: 

```
mono HB9FXQ.DaxIqCat.exe 192000 1 "127.0.0.1" 5566
```

 arguments
* SampleRate for the IQ channel
* DAX channel
* UDP socket address
* UDP socket port

#### Gnu Radio

Then use a UDP source like: 

![alt tag](https://raw.githubusercontent.com/krippendorf/FlexlibMono/master/doc/img/grc_iq.png)

To use with other software like Baudline, nc is your friend:

```
nc -u -l 5566 | /opt/devApps/baudline/baudline -samplerate 96000 -reset -channels 2 -quadrature -format le32f -scaleby 32767. -flipcomplex  -stdin
```

![alt tag](https://raw.githubusercontent.com/krippendorf/FlexlibMono/master/doc/img/baudline_iq.png)
