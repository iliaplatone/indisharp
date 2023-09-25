using System;
using INDI;

namespace indiclient
{
    public class indiclient
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 7624;
            string device = "Telescope Simulator";
            string property = "EQUATORIAL_EOD_COORD";

            if (args.Length > 0)
                address = args[0];
            if (args.Length > 1)
            {
                if (Int32.TryParse(args[1], out port)) { };
            }
            if (args.Length > 2)
            {
                device = args[2];
            }
            if (args.Length > 3)
            {
                property = args[3];
            }
            //declare and instantiate the getter object
            TelescopePropertyGetter getter = new TelescopePropertyGetter(address, port, device, property);
            //append a callback function delegate to the CoordinatesUpdated event
            getter.CoordinatesUpdated += (sender, e) =>
            {
                //Just print the current coordinates
                Console.WriteLine(" RA: " + e.RightAscension + " Dec: " + e.Declination + Environment.NewLine);
            };

            getter.Connect();
            while(true) { System.Threading.Thread.Sleep(50);  }

        }
    }

    public class TelescopeAddedEventArgs : EventArgs
    {
        /// <summary>
        /// The telescope driver INDISharp object.
        /// </summary>
        public INDITelescope telescope;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:indiclient.TelescopeAddedEventArgs"/> class.
        /// </summary>
        /// <param name="t">T.</param>
        public TelescopeAddedEventArgs(INDITelescope t)
        {
            telescope = t;
        }
    }

    public class CoordinatesUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// The right ascension.
        /// </summary>
        public double RightAscension;
        /// <summary>
        /// The declination.
        /// </summary>
        public double Declination;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:indiclient.CoordinatesUpdatedEventArgs"/> class.
        /// </summary>
        /// <param name="ra">Ra.</param>
        /// <param name="dec">Dec.</param>
        public CoordinatesUpdatedEventArgs(double ra, double dec)
        {
            RightAscension = ra;
            Declination = dec;
        }
    }


    public class TelescopePropertyGetter
    {
        INDIClient client;
        INDITelescope telescope;
        /// <summary>
        /// Gets the Right Ascension.
        /// </summary>
        /// <value>The ra.</value>
        public double RA { get; internal set; }
        /// <summary>
        /// Gets the Declination.
        /// </summary>
        /// <value>The dec.</value>
        public double Dec { get; internal set; }
        /// <summary>
        /// Raised when the telescope driver will be connected
        /// </summary>
        public event EventHandler<TelescopeAddedEventArgs> TelescopeAdded;
        /// <summary>
        /// Occurs when celestial coordinates updated.
        /// </summary>
        public event EventHandler<CoordinatesUpdatedEventArgs> CoordinatesUpdated;

        string device = "EqMod Mount";
        private string property = "EQUATORIAL_EOD_COORD";
        private string address = "127.0.0.1";
        private int port = 7624;

        /// <summary>
        /// TelescopePropertyGetter constructor. This method instantiates a new object of this class.
        /// </summary>
        /// <param name="addr">Host address where the INDI server is running.</param>
        /// <param name="p">TCP Port number where to communicate with the INDI drivers</param>
        /// <param name="dev">Device name to connect to</param>
        /// <param name="prop">Number property name (not label) of the coordinates required (EQUATORIAL_EOD_COORD or EQUATORIAL_COORD)</param>
        public TelescopePropertyGetter(string addr, int p, string dev, string prop)
        {
            property = prop;
            device = dev;
            address = addr;
            port = p;
        }

        /// <summary>
        /// Connect to the server and the device driver at once, Telescope drivers usually poll coordinates periodically so you'd get
        /// coordinates updated periodically (500ms default) If you configure the driver from another client you can adjust the polling
        /// frequency.
        /// </summary>
        public void Connect()
        {
            client = new INDIClient(address, port);
            client.DeviceAdded += (sender, e) =>
            {
                if (e.Device.Name == device)
                {
                    INDIDevice dev = e.Device;
                    telescope = new INDITelescope(e.Device.Name, client);
                    dev.ConnectedChanged += (s, ev) =>
                    {
                        if (ev.Vector.Index == 0)
                        {
                            if(TelescopeAdded != null)
                                TelescopeAdded(this, new TelescopeAddedEventArgs(new INDITelescope(dev.Name, client)));
                        };
                        client.QueryProperties();
                    };
                    telescope.IsNewNumber += (t, n) =>
                    {
                        if (n.Vector.Name == property)
                        {
                            RA = telescope.Ra;
                            Dec = telescope.Dec;
                            if(CoordinatesUpdated != null)
                                CoordinatesUpdated(this, new CoordinatesUpdatedEventArgs(RA, Dec));
                        }
                    };
                    telescope.Connected = true;
                }
            };
            client.Connect();
            if(client.Connected)
            {
                client.QueryProperties();
            }
        }
    }
}
