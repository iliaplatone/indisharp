/* This file is part of INDISharp, Copyright © 2014-2015 Ilia Platone <info@iliaplatone.com>.
*
*  INDISharp is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*  
*  INDISharp is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with INDISharp.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace INDI
{
    public class INDIServer : IDisposable
    {
        Int32 _BufferSize = 32768;
        public Int32 BufferSize
        {
            get
            {
                return _BufferSize;
            }
            set
            {
                _BufferSize = value;
            }
        }
        public List<INDIClient> Drivers = new List<INDIClient>();
        TcpListener server;
        Thread listenThread;
        Boolean ThreadsRunning = true;
        string Address = "127.0.0.1:7624";
        Boolean active;
        public Boolean Active
        {
            get
            {
                return active;
            }
        }

        public INDIServer(string address = "127.0.0.1:7624")
        {
            Address = address;
        }
        public INDIServer(string[] clients, string address = "localhost:7624")
        {
            Address = address;
            foreach (string s in clients)
            {
                INDIClient c = new INDIClient(s);
                AddDriver(c);
            }
        }

        public INDIServer(List<INDIClient> clients, string address = "127.0.0.1:7624")
        {
            Address = address;
            foreach (INDIClient c in clients)
            {
                AddDriver(c);
            }
        }

        public void AddDriver(INDIClient c)
        {
            if(!Drivers.Contains(c))
                Drivers.Add(c);
        }

        public void RemoveDriver(INDIClient c)
        {
            if (Drivers.Contains(c))
                Drivers.Remove(c);
        }

        public void Start()
        {
            try
            {
                Uri uri = new Uri("http://" + Address);
                Int32 port = 7624;
                if(Address.Contains(":"))
                   port = uri.Port; 
                server = new TcpListener(IPAddress.Parse(uri.Host), port);
                server.Start();
                listenThread = new Thread(Listen);
                listenThread.Name = "INDISharp Server main thread";
                listenThread.Start();
                active = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public string DefineProperties()
        {
            string ret = "";
            foreach (INDIClient c in Drivers)
            {
                ret += c.DefineProperties();
            }
            return ret;
        }

        public void Stop()
        {
            ThreadsRunning = false;
            try
            {
                foreach (INDIClient d in Drivers)
                {
                    d.Disconnect();
                }
                server.Stop();
            }
            catch
            {
            }
        }

        public void Dispose()
        {
            Stop();
            server = null;
            Drivers.Clear();
        }

        public INDIClient GetDriver(string name)
        {
            foreach (INDIClient c in Drivers)
            {
                if (c.Name == name)
                    return c;
            }
            return null;
        }

        private void Listen()
        {

            while (ThreadsRunning && active)
            {
                try
                {
                    TcpClient client = server.AcceptTcpClient();
                    if (client.Connected)
                    {
                        Thread ReadThread = new Thread(new ParameterizedThreadStart(_readThread));
                        ReadThread.IsBackground = true;
                        ReadThread.Name = "INDISharp Server read thread";
                        ReadThread.Start(client);
                        Thread WriteThread = new Thread(new ParameterizedThreadStart(_writeThread));
                        WriteThread.IsBackground = true;
                        WriteThread.Name = "INDISharp Server write thread";
                        WriteThread.Start(client);
                    }
                }
                catch
                {
                    active = false;
                    break;
                }
            }

        }

        private void _writeThread(Object client)
        {
            byte[] m = new byte[1];
            string message = "";
            TcpClient c = (TcpClient)client;
            NetworkStream s = c.GetStream();
            while (ThreadsRunning && c.Connected)
            {
                try
                {
                    foreach (INDIClient d in Drivers)
                        message += d.OutputString;
                    if (message.Length > 0)
                    {
                        m = Encoding.UTF8.GetBytes(message);
                        s.Write(m, 0, m.Length);
                        message = string.Empty;
                    }
                }
                catch
                {
                    break;
                }
                Thread.Sleep(100);
            }
        }
        private void _readThread(Object client)
        {
            string message = "";
            TcpClient c = (TcpClient)client;
            NetworkStream s = c.GetStream();
            while (ThreadsRunning && c.Connected)
            {
                try
                {
                    byte[] m = new byte[BufferSize];
                    Int32 bytesRead = s.Read(m, 0, BufferSize);
                    if (bytesRead > 0)
                    {
                        message = Encoding.UTF8.GetString(m, 0, bytesRead);
                        foreach (INDIClient d in Drivers)
                            d.InputString = message;
                    }
                }
                catch
                {
                    break;
                }
                Thread.Sleep(100);
            }
            c.Close();
        }
    }
}
