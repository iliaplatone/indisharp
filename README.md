# indisharp
INDI library porting to a C# DLL: automation &amp; control of astronomical instruments

INDISharp does not include any native code and can also be compiled for AnyCPU so is totally portable on many devices or processors. It needs only a C# JIT to run and some essential library usually present into .Net frameworks already.

This library is a port of the INDI astronomical instrumentation applicative library (see https://indilib.org)
You can find the original C port of INDI on GitHub here https://github.com/indilib/indi

This library can be also be found on NuGet when using a C# IDE. Just search for INDISharp on the NuGet repository search to install it.

There's not any documentation yet. Next thing to do is an explicative documentation. I ask you sorry if there's not one yet after this long time.

## Usage and library structure

INDISharp can be used both as a client and as a server, or repeater.
INDISharp can connect to an external INDI server and drive all the devices linked to it.
There are various driver interfaces compatible with INDI drivers.

- INDICamera
- INDITelescope
- INDIDome
- INDIFilterWheel
- INDIFocuser
- INDISpectrograph
- INDIDetector

For generic drivers or interfaces not included into this list you can use the INDIDevice interface or inherit this class into your driver.

If you want to create a client and connect to a server keep in mind that INDISharp is event-driven so I'd advice to not to use sleeps or blocking code but to use asynchronous code itself.

A simple WinForm class is present into INDISharp and includes client code and some rudimental graphical interface to the code.

## Getting started

```
using System;
using System.Windows.Forms;
using INDI;
using INDI.Forms;

namespace indisharpexample
{ 
    public class Program
    {
        public static void Main()
        {
            INDIChooser chooser = new INDIChooser();
            chooser.ShowDialog();
            INDIForm form = new INDIForm(chooser.client, chooser.deviceSelected);
            Application.Run(form);
        }
    }
}
```
