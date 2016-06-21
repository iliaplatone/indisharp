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
using System.Windows.Forms;
using System.Drawing;

namespace INDI.Forms
{

    public class INDIChooser : Form
    {
        public INDIClient client = null;
        TextBox Address = new TextBox();
        ComboBox Device = new ComboBox();
        public string deviceSelected = "";
        public INDIChooser()
        {
            Int32 y = 15;
            Label l = new Label();
            l.Text = "Connect to address:";
            l.Location = new Point(5, y);
            l.Size = new Size(190, 23);
            y += 33;
            Address.BorderStyle = BorderStyle.FixedSingle;
            Address.Name = "address";
            Address.Text = "127.0.0.1:7624";
            Address.Location = new Point(5, y);
            Address.Size = new Size(190, 23);
            y += 33;
            Controls.Add(Address);
            Controls.Add(l);
            Button b = new Button();
            b.FlatStyle = FlatStyle.Flat;
            b.Text = "Connect";
            b.Location = new Point(5, y);
            b.Size = new Size(190, 23);
            y += 33;
            b.Click += delegate
            {
                Device.Items.Clear();
                client = new INDIClient(Address.Text);
                client.DeviceAdded += Indi_DeviceAdded;
                client.Connect();
                if (client.Connected)
                    client.QueryProperties();
            };
            Controls.Add(b);
            Device.FlatStyle = FlatStyle.Flat;
            Device.Text = "Select device";
            Device.Size = new Size(190, 23);
            Device.Location = new Point(5, y);
            Device.DropDownStyle = ComboBoxStyle.DropDownList;
            Device.SelectedIndexChanged += Device_SelectedIndexChanged;
            y += 33;
            Controls.Add(Device);
            b = new Button();
            b.FlatStyle = FlatStyle.Flat;
            b.Text = "OK";
            b.Location = new Point(5, y);
            b.Size = new Size(190, 23);
            y += 33;
            b.Click += delegate
            {
                if (client == null || !client.Connected)
                    MessageBox.Show("Not Connected to any server!", "ERROR");
                else if (deviceSelected == "")
                    MessageBox.Show("No device selected!", "ERROR");
                else
                    Close();
            };
            Controls.Add(b);
            b = new Button();
            b.FlatStyle = FlatStyle.Flat;
            b.Text = "Cancel";
            b.Location = new Point(5, y);
            b.Size = new Size(190, 23);
            y += 33;
            b.Click += delegate
            {
                if (client != null)
                {
                    client.Disconnect();
                    client.Dispose();
                }
                client = null;
                Close();
            };
            Controls.Add(b);
            this.ClientSize = new Size(200, y);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        void Indi_DeviceAdded(Object sender, DeviceAddedEventArgs e)
        {
            try
            {
                if (IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        Device.Items.Add(e.Device.Name);
                        Device.SelectedIndex = 0;
                    });
                }
            }
            catch
            {
            }
        }

        void Device_SelectedIndexChanged(Object sender, EventArgs e)
        {
            deviceSelected = Device.SelectedItem.ToString();
        }
    }
    public class INDIForm : Form
    {
        public Boolean Connected { get { return server.Connected; } }
        TabControl DevicesConnected = new TabControl();
        string Device;
        INDIClient server;
        public INDIForm(INDIClient host, string device = "")
        {
            this.FormClosing += _FormClosing;
            server = host;
            Device = device;
            Connect();
        }

        void Connect()
        {
            if (server != null)
            {
                server.Connect();
                if (Connected)
                {
                    server.IsNewSwitch += IsNewSwitch;
                    server.IsNewNumber += IsNewNumber;
                    server.IsNewText += IsNewText;
                    server.IsDelProperty += IsDelProperty;
                    server.QueryProperties();
                    DevicesConnected.Size = new Size(495, 395);
                    this.Controls.Clear();
                    this.Controls.Add(DevicesConnected);
                    this.ClientSize = new Size(500, 400);
                    this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                }
            }
        }

        void _FormClosing(Object sender, EventArgs e)
        {
            this.FormClosing -= _FormClosing;
            this.Controls.Clear();
            if (server != null)
                server.Dispose();
        }

        Boolean ChildPresent(Control c, string name)
        {
            try
            {
                if (c.Controls.Count > 0)
                {
                    Control[] ctl = c.Controls.Find(name, false);
                    if (ctl.Length > 0)
                        return true;
                }
                return false;
            }
            finally
            {
            }
        }

        Control GetChild(Control c, string name)
        {
            if (c.Controls.Find(name, false).Length > 0)
                return c.Controls.Find(name, false)[0];
            return null;
        }

        Control GetLastChild(Control c)
        {
            if (c.Controls.Count > 0)
                return c.Controls[c.Controls.Count - 1];
            return null;
        }

        void AddDevice(string device)
        {
            if (device == Device || device == string.Empty)
            {
                TabPage dev = new TabPage();
                dev.Name = device;
                dev.Text = device;
                dev.Size = new Size(490, 367);
                TabControl tab = new TabControl();
                tab.Size = new Size(485, 362);
                tab.Name = device + "_Groups";
                dev.Controls.Add(tab);
                if (!ChildPresent(DevicesConnected, device))
                    DevicesConnected.Controls.Add(dev);
            }
        }

        void AddGroup(string name, string device)
        {
            if (device == Device || device == string.Empty)
            {
                TabControl tab = (TabControl)DevicesConnected.Controls.Find(device, false)[0].Controls.Find(device + "_Groups", false)[0];
                TabPage grp = new TabPage();
                grp.Size = new Size(480, 334);
                grp.Name = name;
                grp.Text = name;
                Panel pan = new Panel();
                pan.Size = new Size(480, 334);
                pan.AutoScroll = true;
                pan.Name = name + "_AutoScrollPanel";
                grp.Controls.Add(pan);
                if (!ChildPresent(tab, name))
                    tab.Controls.Add(grp);
            }
        }

        void IsNewSwitch(Object sender, IsNewSwitchEventArgs e)
        {
            if ((e.Device == Device || e.Device == string.Empty) && e.Vector.Name != String.Empty && e.Vector.Group != String.Empty)
            {
                if (IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        GroupBox vector = new GroupBox();
                        vector.FlatStyle = FlatStyle.Flat;
                        AddDevice(e.Device);
                        AddGroup(e.Vector.Group, e.Device);
                        Control ctl = DevicesConnected.Controls.Find(e.Device, false)[0];
                        ctl = ctl.Controls.Find(e.Device + "_Groups", false)[0];
                        ctl = ctl.Controls.Find(e.Vector.Group, false)[0];
                        ctl = ctl.Controls.Find(e.Vector.Group + "_AutoScrollPanel", false)[0];
                        Panel pan = (Panel)ctl;
                        if (!ChildPresent(pan, e.Vector.Name))
                        {
                            if (GetLastChild(pan) != null)
                                vector.Location = new Point(GetLastChild(pan).Location.X, GetLastChild(pan).Location.Y + GetLastChild(pan).Size.Height + 5);
                            pan.Controls.Add(vector);
                            vector.Name = e.Vector.Name;
                            vector.Text = e.Vector.Label;
                        }
                        vector = (GroupBox)ctl.Controls.Find(e.Vector.Name, false)[0];
                        if (vector.Controls.Count >= e.Vector.Values.Count)
                            goto setSwitches;
                        Int32 y = 20;
                        vector.Size = new Size(455, 20 + e.Vector.Values.Count * 28);
                        for (int k = 1; k < vector.Parent.Controls.Count; k++)
                            vector.Parent.Controls[k].Location = new Point(vector.Parent.Controls[k].Location.X, vector.Parent.Controls[k - 1].Location.Y + vector.Parent.Controls[k - 1].Size.Height + 5);
                        foreach (INDISwitch n in e.Vector.Values)
                        {
                            if (e.Vector.Rule != "OneOfMany" && e.Vector.Rule != "AtMostOne")
                            {
                                var t = new CheckBox();
                                t.FlatStyle = FlatStyle.Flat;
                                t.AutoSize = true;
                                t.Text = n.Label;
                                t.Name = "SWITCH_" + n.Name;
                                t.Enabled = (e.Vector.Permission != "ro");
                                t.Checked = n.value;
                                t.Location = new Point(30, y);
                                t.CheckedChanged += valueChanged;
                                y += 26;
                                vector.Controls.Add(t);
                            }
                            else
                            {
                                var t = new RadioButton();
                                t.FlatStyle = FlatStyle.Flat;
                                t.AutoSize = true;
                                t.Text = n.Label;
                                t.Name = "SELECT_" + n.Name;
                                t.Enabled = (e.Vector.Permission != "ro");
                                t.Checked = n.value;
                                t.Location = new Point(30, y);
                                t.CheckedChanged += valueChanged;
                                y += 26;
                                vector.Controls.Add(t);
                            }
                        }
                        setSwitches:
                        foreach (INDISwitch n in e.Vector.Values)
                        {
                            if (ChildPresent(vector, "SWITCH_" + n.Name))
                            {
                                ((CheckBox)vector.Controls.Find("SWITCH_" + n.Name, false)[0]).CheckedChanged -= valueChanged;
                                ((CheckBox)vector.Controls.Find("SWITCH_" + n.Name, false)[0]).Checked = n.value;
                                ((CheckBox)vector.Controls.Find("SWITCH_" + n.Name, false)[0]).CheckedChanged += valueChanged;
                            }
                            if (ChildPresent(vector, "SELECT_" + n.Name))
                            {
                                ((RadioButton)vector.Controls.Find("SELECT_" + n.Name, false)[0]).CheckedChanged -= valueChanged;
                                ((RadioButton)vector.Controls.Find("SELECT_" + n.Name, false)[0]).Checked = n.value;
                                ((RadioButton)vector.Controls.Find("SELECT_" + n.Name, false)[0]).CheckedChanged += valueChanged;
                            }
                        }
                    });
                }
            }
        }

        void IsNewNumber(Object sender, IsNewNumberEventArgs e)
        {
            if ((e.Device == Device || e.Device == string.Empty) && e.Vector.Name != String.Empty && e.Vector.Group != String.Empty)
            {
                if (IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        GroupBox vector = new GroupBox();
                        vector.FlatStyle = FlatStyle.Flat;
                        AddDevice(e.Device);
                        AddGroup(e.Vector.Group, e.Device);
                        Control ctl = DevicesConnected.Controls.Find(e.Device, false)[0];
                        ctl = ctl.Controls.Find(e.Device + "_Groups", false)[0];
                        ctl = ctl.Controls.Find(e.Vector.Group, false)[0];
                        ctl = ctl.Controls.Find(e.Vector.Group + "_AutoScrollPanel", false)[0];
                        Panel pan = (Panel)ctl;
                        if (!ChildPresent(pan, e.Vector.Name))
                        {
                            if (GetLastChild(pan) != null)
                                vector.Location = new Point(GetLastChild(pan).Location.X, GetLastChild(pan).Location.Y + GetLastChild(pan).Size.Height + 5);
                            pan.Controls.Add(vector);
                            vector.Name = e.Vector.Name;
                            vector.Text = e.Vector.Label;
                        }
                        vector = (GroupBox)ctl.Controls.Find(e.Vector.Name, false)[0];
                        if (vector.Controls.Count >= e.Vector.Values.Count)
                            goto setNumbers;
                        Int32 y = 20;
                        vector.Size = new Size(455, 20 + e.Vector.Values.Count * 28);
                        for (int k = 1; k < vector.Parent.Controls.Count; k++)
                            vector.Parent.Controls[k].Location = new Point(vector.Parent.Controls[k].Location.X, vector.Parent.Controls[k - 1].Location.Y + vector.Parent.Controls[k - 1].Size.Height + 5);
                        foreach (INDINumber n in e.Vector.Values)
                        {
                            Label l = new Label();
                            l.AutoSize = true;
                            l.Text = n.Label;
                            TextBox t = new TextBox();
                            t.BorderStyle = BorderStyle.FixedSingle;
                            t.Text = n.value.ToString();
                            t.Name = "NUMBER_" + n.Name;
                            t.Enabled = (e.Vector.Permission != "ro");
                            l.Location = new Point(30, y);
                            t.Location = new Point(330, y - 5);
                            l.Size = new Size(100, 18);
                            t.Size = new Size(100, 18);
                            t.TextChanged += valueChanged;
                            y += 26;
                            vector.Controls.Add(l);
                            vector.Controls.Add(t);
                        }
                        setNumbers:
                        foreach (INDINumber n in e.Vector.Values)
                        {
                            vector.Controls.Find("NUMBER_" + n.Name, false)[0].TextChanged -= valueChanged;
                            string text = n.value.ToString();
                            if (e.Vector.Name == "TIME_LST" || (e.Vector.Name.Contains("COORD") && n.Name == "RA"))
                                text = (Math.Floor(n.value) % 24).ToString() + ":" + (Math.Floor(n.value * 60) % 60).ToString() + ":" + (Math.Floor(n.value * 3600) % 60).ToString();
                            if (e.Vector.Name.Contains("COORD") && (n.Name == "DEC" || n.Name == "ALT" || n.Name == "AZ"))
                                text = (Math.Floor(n.value) % 360).ToString() + ":" + (Math.Abs(Math.Floor(n.value * 60)) % 60).ToString() + ":" + (Math.Abs(Math.Floor(n.value * 3600)) % 60).ToString();
                            vector.Controls.Find("NUMBER_" + n.Name, false)[0].Text = text;
                            vector.Controls.Find("NUMBER_" + n.Name, false)[0].TextChanged += valueChanged;
                        }
                    });
                }
            }
        }

        void IsNewText(Object sender, IsNewTextEventArgs e)
        {
            if ((e.Device == Device || e.Device == string.Empty) && e.Vector.Name != String.Empty && e.Vector.Group != String.Empty)
            {
                if (IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        GroupBox vector = new GroupBox();
                        vector.FlatStyle = FlatStyle.Flat;
                        AddDevice(e.Device);
                        AddGroup(e.Vector.Group, e.Device);
                        Control ctl = DevicesConnected.Controls.Find(e.Device, false)[0];
                        ctl = ctl.Controls.Find(e.Device + "_Groups", false)[0];
                        ctl = ctl.Controls.Find(e.Vector.Group, false)[0];
                        ctl = ctl.Controls.Find(e.Vector.Group + "_AutoScrollPanel", false)[0];
                        Panel pan = (Panel)ctl;
                        if (!ChildPresent(pan, e.Vector.Name))
                        {
                            if (GetLastChild(pan) != null)
                                vector.Location = new Point(GetLastChild(pan).Location.X, GetLastChild(pan).Location.Y + GetLastChild(pan).Size.Height + 5);
                            pan.Controls.Add(vector);
                            vector.Name = e.Vector.Name;
                            vector.Text = e.Vector.Label;
                        }
                        vector = (GroupBox)ctl.Controls.Find(e.Vector.Name, false)[0];
                        if (vector.Controls.Count >= e.Vector.Values.Count)
                            goto setTexts;
                        Int32 y = 20;
                        vector.Size = new Size(455, 20 + e.Vector.Values.Count * 28);
                        for (int k = 1; k < vector.Parent.Controls.Count; k++)
                            vector.Parent.Controls[k].Location = new Point(vector.Parent.Controls[k].Location.X, vector.Parent.Controls[k - 1].Location.Y + vector.Parent.Controls[k - 1].Size.Height + 5);
                        foreach (INDIText n in e.Vector.Values)
                        {
                            Label l = new Label();
                            l.AutoSize = true;
                            l.Text = n.Label;
                            TextBox t = new TextBox();
                            t.BorderStyle = BorderStyle.FixedSingle;
                            t.Text = n.value;
                            t.Name = "TEXT_" + n.Name;
                            t.Enabled = (e.Vector.Permission != "ro");
                            l.Location = new Point(30, y);
                            t.Location = new Point(330, y - 5);
                            t.TextChanged += valueChanged;
                            y += 26;
                            vector.Controls.Add(l);
                            vector.Controls.Add(t);
                        }
                        setTexts:
                        foreach (INDIText n in e.Vector.Values)
                        {
                            vector.Controls.Find("TEXT_" + n.Name, false)[0].TextChanged -= valueChanged;
                            vector.Controls.Find("TEXT_" + n.Name, false)[0].Text = n.value;
                            vector.Controls.Find("TEXT_" + n.Name, false)[0].TextChanged += valueChanged;
                        }
                    });
                }
            }
        }

        void IsDelProperty(Object sender, IsDelPropertyEventArgs e)
        {
            if (e.Device == Device || e.Device == string.Empty)
            {
                if (IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        Control device, property, group, tab;
                        if (ChildPresent(DevicesConnected, e.Device))
                        {
                            device = DevicesConnected.Controls.Find(e.Device, false)[0];
                            if (device.Controls.Find(e.Vector, true).Length > 0)
                            {
                                property = device.Controls.Find(e.Vector, true)[0];
                                group = property.Parent;
                                tab = group.Parent.Parent;
                                group.Controls.Remove(property);
                                if (group.Controls.Count == 0)
                                    tab.Controls.Remove(group.Parent);
                            }
                        }
                    });
                }
            }
        }

        void valueChanged(Object sender, EventArgs e)
        {
            try
            {
                string device = ((Control)sender).Parent.Parent.Parent.Parent.Parent.Name;
                string vector = ((Control)sender).Parent.Name;
                string type = ((Control)sender).Name;
                if (type.StartsWith("SWITCH_"))
                {
                    server.GetDevice(device).SetSwitch(vector, type.Replace("SWITCH_", ""), ((CheckBox)sender).Checked);
                }
                if (type.StartsWith("SELECT_"))
                {
                    Int32 index = 0;
                    for (index = 0; index < ((Control)sender).Parent.Controls.Count; index++)
                    {
                        if (((RadioButton)((Control)sender).Parent.Controls[index]).Checked)
                            break;
                    }
                    server.GetDevice(device).SetSwitchVector(vector, index);
                }
                if (type.StartsWith("NUMBER_"))
                {
                    Double val = 0;
                    if (type.Contains("TIME_LST") || type.Contains("COORD"))
                    {
                        string[] dt = ((TextBox)sender).Text.Split(":".ToCharArray());
                        if (dt.Length == 3)
                        {
                            double v = 0;
                            if (Double.TryParse(dt[0], out v))
                                val += v;
                            if (Double.TryParse(dt[1], out v))
                                val += v / 60;
                            if (Double.TryParse(dt[2], out v))
                                val += v / 3600;
                        }
                        server.GetDevice(device).SetNumber(vector, type.Replace("NUMBER_", ""), val);
                    }
                    else if (Double.TryParse(((TextBox)sender).Text, out val))
                        server.GetDevice(device).SetNumber(vector, type.Replace("NUMBER_", ""), val);
                }
                if (type.StartsWith("TEXT_"))
                {
                    server.GetDevice(device).SetText(vector, type.Replace("TEXT_", ""), ((TextBox)sender).Text);
                }
                server.QueryProperties();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}
	


