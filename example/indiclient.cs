using System;
using INDI;

namespace indiclient
{
    public class indiclient
    {
        public static void Main(string[] args)
        {
            string address = "127.0.0.1";
            int port = 7624;
            string device = "Telescope Simulator";
            string property = "EQUATORIAL_EOD_COORD";

            if (args.Length > 0)
                address = args[0];
            if(args.Length > 1)
            {
                if (Int32.TryParse(args[1], out port)) { };
            }
            if (args.Length > 2)
            {
                device = args[2];
            }
            if(args.Length > 3)
            {
                property = args[3];
            }
            TelescopePropertyGetter getter = new TelescopePropertyGetter(address, port, device, property);
            getter.CoordinatesUpdated += (sender, e) =>
            {
                Console.WriteLine(" RA: " + e.RightAscension + " Dec: " + e.Declination + Environment.NewLine);
            };

            getter.Connect();
            while(true) { System.Threading.Thread.Sleep(50);  }

        }
    }

    public class TelescopeAddedEventArgs : EventArgs
    {
        public INDITelescope telescope;
        public TelescopeAddedEventArgs(INDITelescope t)
        {
            telescope = t;
        }
    }

    public class CoordinatesUpdatedEventArgs : EventArgs
    {
        public double RightAscension;
        public double Declination;
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
        public double RA { get; internal set; }
        public double Dec { get; internal set; }
        public event EventHandler<TelescopeAddedEventArgs> TelescopeAdded;
        public event EventHandler<CoordinatesUpdatedEventArgs> CoordinatesUpdated;
        private string device = "EqMod Mount";
        private string property = "EQUATORIAL_EOD_COORD";
        private string address = "127.0.0.1";
        private int port = 7624;

        public TelescopePropertyGetter(string addr, int p, string dev, string prop)
        {
            property = prop;
            device = dev;
            address = addr;
            port = p;
        }

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
