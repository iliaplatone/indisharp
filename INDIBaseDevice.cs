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
using System.Globalization;
using System.Threading;
using System.Xml.Linq;

namespace INDI
{
    public class INDIBaseDevice
    {

        public string Name;
        List<ITextVector> Texts;
        List<INumberVector> Numbers;
        List<ISwitchVector> Switches;
        List<IBlobVector> Blobs;
       	public INDIClient Host = null;
        public INDIBaseDevice(string n, INDIClient host, bool client = true)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            if (host == null)
                throw new ArgumentException("host cannot be null");
            IsClient = client;
            Host = host;
            Name = n;
            Texts = new List<ITextVector>();
            Numbers = new List<INumberVector>();
            Switches = new List<ISwitchVector>();
            Blobs = new List<IBlobVector>();
            host.IsDelProperty += isDelProperty;
            host.IsNewText += isNewText;
            host.IsNewNumber += isNewNumber;
            host.IsNewSwitch += isNewSwitch;
            host.IsNewBlob += isNewBlob;
        }

        public virtual void isDelProperty(Object sender, IsDelPropertyEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            if (e.Device == Name)
            {
                Switches.Remove(GetSwitchVector(e.Vector));
                Blobs.Remove(GetBlobVector(e.Vector));
                Numbers.Remove(GetNumberVector(e.Vector));
                Texts.Remove(GetTextVector(e.Vector));
            }
        }

        public virtual void isNewText(Object sender, IsNewTextEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            if (e.Device == Name)
            {
                ITextVector v = GetTextVector(e.Vector.Name);
                if (v == null)
                    AddTextVector(e.Vector);
            }
        }

        public virtual void isNewNumber(Object sender, IsNewNumberEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            if (e.Device == Name)
            {
                INumberVector v = GetNumberVector(e.Vector.Name);
                if (v == null)
                    AddNumberVector(e.Vector);
            }
        }

        public virtual void isNewSwitch(Object sender, IsNewSwitchEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            if (e.Device == Name)
            {
                ISwitchVector v = GetSwitchVector(e.Vector.Name);
                if (v == null)
                    AddSwitchVector(e.Vector);
            }
        }

        public virtual void isNewBlob(Object sender, IsNewBlobEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            if (e.Device == Name)
            {
                IBlobVector v = GetBlobVector(e.Vector.Name);
                if (v == null)
                    AddBlobVector(e.Vector);
            }
        }

        public string EnableBLOB(Boolean enable)
        {
            string ret =
                new XElement("enableBLOB", new XAttribute("device", Name), (enable ? "Also" : "Never")).ToString();
            Host.OutputString = ret;
            return ret;
        }

        public void AddTextVector(ITextVector v)
        {
            ITextVector vector = GetTextVector(v.Name);
            if (vector != null)
            {
                GetTextVector(v.Name).Change(v);
                return;
            }
            Texts.Add(v);
        }

        public void AddSwitchVector(ISwitchVector v)
        {
            ISwitchVector vector = GetSwitchVector(v.Name);
            if (vector != null)
            {
                GetSwitchVector(v.Name).Change(v);
                return;
            }
            Switches.Add(v);
        }

        public void AddNumberVector(INumberVector v)
        {
            INumberVector vector = GetNumberVector(v.Name);
            if (vector != null)
            {
                GetNumberVector(v.Name).Change(v);
                return;
            }
            Numbers.Add(v);
        }

        public void AddBlobVector(IBlobVector v)
        {
            IBlobVector vector = GetBlobVector(v.Name);
            if (vector != null)
            {
                GetBlobVector(v.Name).Change(v);
                return;
            }
            Blobs.Add(v);
        }

        public List<string> GetGroups()
        {
            List<string> ret = new List<string>();
            foreach (ISwitchVector d in Host.GetDevice(Name).Switches)
            {
                if (!ret.Contains(d.Group))
                    ret.Add(d.Group);
            }
            foreach (INumberVector d in Host.GetDevice(Name).Numbers)
            {
                if (!ret.Contains(d.Group))
                    ret.Add(d.Group);
            }
            foreach (ITextVector d in Host.GetDevice(Name).Texts)
            {
                if (!ret.Contains(d.Group))
                    ret.Add(d.Group);
            }
            foreach (IBlobVector d in Host.GetDevice(Name).Blobs)
            {
                if (!ret.Contains(d.Group))
                    ret.Add(d.Group);
            }
            return ret;
        }

        public ISwitchVector GetSwitchVector(string name)
        {
			INDIDevice d = Host.GetDevice (Name);
            foreach (ISwitchVector s in d.Switches)
                if (s.Name == name && name != "")
                    return s;
            return null;
        }

        public INumberVector GetNumberVector(string name)
		{
			INDIDevice d = Host.GetDevice (Name);
			foreach (INumberVector s in d.Numbers)
                if (s.Name == name && name != "")
                    return s;
            return null;
        }

        public ITextVector GetTextVector(string name)
        {
            foreach (ITextVector s in Host.GetDevice(Name).Texts)
                if (s.Name == name && name != "")
                    return s;
            return null;
        }

        public IBlobVector GetBlobVector(string name)
        {
            foreach (IBlobVector s in Host.GetDevice(Name).Blobs)
                if (s.Name == name && name != "")
                    return s;
            return null;
        }

        public INDINumber GetNumber(string vector, string name)
        {
            INumberVector v = GetNumberVector(vector);
            if (v != null)
            {
                for (Int32 i = 0; i < v.Values.Count; i++)
                {
                    if (v.Values[i].Name == name)
                        return v.Values[i];
                }
            }
            return null;
        }

        public INDIText GetText(string vector, string name)
        {
            ITextVector v = GetTextVector(vector);
            if (v != null)
            {
                for (Int32 i = 0; i < v.Values.Count; i++)
                {
                    if (v.Values[i].Name == name)
                        return v.Values[i];
                }
            }
            return null;
        }

        public INDISwitch GetSwitch(string vector, string name)
        {
            ISwitchVector v = GetSwitchVector(vector);
            if (v != null)
            {
                for (Int32 i = 0; i < v.Values.Count; i++)
                {
                    if (v.Values[i].Name == name)
                        return v.Values[i];
                }
            }
            return null;
        }

        public INDIBlob GetBlob(string vector, string name)
        {
            IBlobVector v = GetBlobVector(vector);
            if (v != null)
            {
                for (Int32 i = 0; i < v.Values.Count; i++)
                {
                    if (v.Values[i].Name == name)
                        return v.Values[i];
                }
            }
            return null;
        }

        public string SetVector(string name, byte[][] blobs, Boolean[] switches, string[] texts, Double[] numbers)
        {
            if (blobs != null)
                return SetBlobVector(name, blobs);
            if (switches != null)
                return SetSwitchVector(name, switches);
            if (texts != null)
                return SetTextVector(name, texts);
            if (numbers != null)
                return SetNumberVector(name, numbers);
            return "";
        }

        public string SetNumber(string vector, string name, Double value)
        {
            INumberVector v = GetNumberVector(vector);
            if (v == null)
                throw new ArgumentException();
            Double[] values = new Double[v.Values.Count];
            for (Int32 i = 0; i < values.Length; i++)
            {
                if (v.Values[i].Name == name)
                    values[i] = value;
                else
                    values[i] = v.Values[i].value;
            }
            return SetNumberVector(vector, values);
        }

        public string SetText(string vector, string name, string value)
        {
            ITextVector v = GetTextVector(vector);
            if (v == null)
                throw new ArgumentException();
            string[] values = new string[v.Values.Count];
            for (Int32 i = 0; i < values.Length; i++)
            {
                if (v.Values[i].Name == name)
                    values[i] = value;
                else
                    values[i] = v.Values[i].value;
            }
            return SetTextVector(vector, values);
        }

        public string SetSwitch(string vector, string name, Boolean value)
        {
            ISwitchVector v = GetSwitchVector(vector);
            if (v == null)
                throw new ArgumentException();
            Boolean[] values = new Boolean[v.Values.Count];
            for (Int32 i = 0; i < values.Length; i++)
            {
                if (v.Values[i].Name == name)
                {
                    values[i] = value;
                }
                else
                {
                    if (v.Rule == "OneOfMany")
                    {
                        values[i] = !value;
                    }
                    else
                    {
                        values[i] = v.Values[i].value;
                    }
                }
            }
            return SetSwitchVector(vector, values);
        }

        public string SetBlob(string vector, string name, byte[] value)
        {
            IBlobVector v = GetBlobVector(vector);
            if (v == null)
                throw new ArgumentException();
            byte[][] values = new byte[v.Values.Count][];
            for (Int32 i = 0; i < values.Length; i++)
            {
                if (v.Values[i].Name == name)
                    values[i] = value;
                else
                    values[i] = v.Values[i].value;
            }
            return SetBlobVector(vector, values);
        }

        public string SetNumberVector(string vector, Double[] values)
        {
            if (GetNumberVector(vector) == null)
                throw new ArgumentException();
            XElement[] items = new XElement[values.Length];
            for (Int32 i = 0; i < GetNumberVector(vector).Values.Count; i++)
            {
                items[i] = new XElement("oneNumber", new XAttribute("name", GetNumberVector(vector).Values[i].Name), values[i].ToString().Replace(",", "."));
                GetNumberVector(vector).Values[i].value = values[i];
            }
            string ret =
                new XElement("newNumberVector",
                    new XAttribute("device", Name),
                    new XAttribute("name", vector),
                    items).ToString();
            Host.OutputString = ret;
            if (IsClient)
                DefineNumbers(vector);
            return ret;
        }

        public string SetTextVector(string vector, string[] values)
        {
            if (GetTextVector(vector) == null)
                throw new ArgumentException();
            XElement[] items = new XElement[values.Length];
            for (Int32 i = 0; i < GetTextVector(vector).Values.Count; i++)
            {
                items[i] = new XElement("oneText", new XAttribute("name", GetTextVector(vector).Values[i].Name), values[i].ToString());
                GetTextVector(vector).Values[i].value = values[i].ToString();
            }
            string ret =
                new XElement("newTextVector",
                    new XAttribute("device", Name),
                    new XAttribute("name", vector),
                    items).ToString();
            Host.OutputString = ret;
            if (IsClient)
                DefineTexts(vector);
            return ret;
        }

        public string SetSwitchVector(string vector, Int32 index)
        {
            if (GetSwitchVector(vector) == null)
                throw new ArgumentException();
            XElement[] items = new XElement[GetSwitchVector(vector).Values.Count];
            for (Int32 i = 0; i < GetSwitchVector(vector).Values.Count; i++)
            {
                items[i] = new XElement("oneSwitch", new XAttribute("name", GetSwitchVector(vector).Values[i].Name), (i == index ? "On" : "Off"));
                GetSwitchVector(vector).Values[i].value = (i == index);
            }
            string ret =
                new XElement("newSwitchVector",
                    new XAttribute("device", Name),
                    new XAttribute("name", vector),
                    items).ToString();
            Host.OutputString = ret;
            if (IsClient)
                DefineSwitches(vector);
            return ret;
        }

        public string SetSwitchVector(string vector, Boolean[] values)
        {
            if (GetSwitchVector(vector) == null)
                throw new ArgumentException();
            XElement[] items = new XElement[values.Length];
            for (Int32 i = 0; i < GetSwitchVector(vector).Values.Count; i++)
            {
                items[i] = new XElement("oneSwitch", new XAttribute("name", GetSwitchVector(vector).Values[i].Name), (values[i] ? "On" : "Off"));
                GetSwitchVector(vector).Values[i].value = values[i];
            }
            string ret =
                new XElement("newSwitchVector",
                    new XAttribute("device", Name),
                    new XAttribute("name", vector),
                    items).ToString();
            Host.OutputString = ret;
            return ret;
        }

        public string SetBlobVector(string vector, byte[][] values)
        {
            if (GetBlobVector(vector) == null)
                throw new ArgumentException();
            XElement[] items = new XElement[values.Length];
            for (Int32 i = 0; i < GetBlobVector(vector).Values.Count; i++)
            {
                string data = Convert.ToBase64String(values[i]);
                items[i] = new XElement("oneBlob", new XAttribute("name", GetBlobVector(vector).Values[i].Name), new XAttribute("format", GetBlobVector(vector).Values[i].format), new XAttribute("size", data.Length), data);
                GetBlobVector(vector).Values[i].value = values[i];
            }
            string ret =
                new XElement("newBlobVector",
                    new XAttribute("device", Name),
                    new XAttribute("name", vector),
                    items).ToString();
            Host.OutputString = ret;
            return ret;
        }

        public string QueryProperties(string vector = "")
        {
            string ret =
                new XElement("getProperties",
                    new XAttribute("device", Name),
                    new XAttribute("name", vector),
                    new XAttribute("version", "1.7")).ToString();
            Host.OutputString = ret;
            return ret;
        }

        public string DefineProperties(string vector = "")
        {
            string ret = "";
            ret += DefineNumbers(vector);
            ret += DefineSwitches(vector);
            ret += DefineTexts(vector);
            ret += DefineBlobs(vector);
            return ret;
        }

        public string DefineNumbers(string name = "")
        {
            string ret = "";
            foreach (INumberVector vector in Numbers)
            {
                if ((name != "" && vector.Name == name) || name == "")
                {
                    XElement[] items = new XElement[vector.Values.Count];
                    for (Int32 i = 0; i < vector.Values.Count; i++)
                    {
                        items[i] = new XElement("defNumber",
                            new XAttribute("label", vector.Values[i].Label),
                            new XAttribute("name", vector.Values[i].Name),
                            new XAttribute("format", vector.Values[i].format),
                            new XAttribute("min", vector.Values[i].min),
                            new XAttribute("max", vector.Values[i].max),
                            new XAttribute("step", vector.Values[i].step),
                            vector.Values[i].value.ToString());
                    }
                    ret +=
                        new XElement("defNumberVector",
                            new XAttribute("device", vector.Device),
                            new XAttribute("name", vector.Name),
                            new XAttribute("label", vector.Label),
                            new XAttribute("group", vector.Group),
                            new XAttribute("perm", vector.Permission),
                            items).ToString();
                }
            }
            Host.OutputString = ret;
            return ret;
        }

        public string DefineTexts(string name = "")
        {
            string ret = "";
            foreach (ITextVector vector in Texts)
            {
                if ((name != "" && vector.Name == name) || name == "")
                {
                    XElement[] items = new XElement[vector.Values.Count];
                    for (Int32 i = 0; i < vector.Values.Count; i++)
                    {
                        items[i] = new XElement("defText",
                            new XAttribute("label", vector.Values[i].Label),
                            new XAttribute("name", vector.Values[i].Name),
                            vector.Values[i].value.ToString());
                    }
                    ret +=
                        new XElement("defTextVector",
                            new XAttribute("device", vector.Device),
                            new XAttribute("name", vector.Name),
                            new XAttribute("label", vector.Label),
                            new XAttribute("group", vector.Group),
                            new XAttribute("perm", vector.Permission),
                            items).ToString();
                }
            }
            Host.OutputString = ret;
            return ret;
        }

        public string DefineSwitches(string name = "")
        {
            string ret = "";
            foreach (ISwitchVector vector in Switches)
            {
                if ((name != "" && vector.Name == name) || name == "")
                {
                    XElement[] items = new XElement[vector.Values.Count];
                    for (Int32 i = 0; i < vector.Values.Count; i++)
                    {
                        items[i] = new XElement("defSwitch",
                            new XAttribute("label", vector.Values[i].Label),
                            new XAttribute("name", vector.Values[i].Name),
                            (vector.Values[i].value ? "On" : "Off"));
                    }
                    ret +=
                        new XElement("defSwitchVector",
                            new XAttribute("device", vector.Device),
                            new XAttribute("name", vector.Name),
                            new XAttribute("label", vector.Label),
                            new XAttribute("group", vector.Group),
                            new XAttribute("rule", vector.Rule),
                            new XAttribute("perm", vector.Permission),
                            items).ToString();
                }
            }
            Host.OutputString = ret;
            return ret;
        }

        public string DefineBlobs(string name = "")
        {
            string ret = "";
            foreach (IBlobVector vector in Blobs)
            {
                if ((name != "" && vector.Name == name) || name == "")
                {
                    XElement[] items = new XElement[vector.Values.Count];
                    for (Int32 i = 0; i < vector.Values.Count; i++)
                    {
                        items[i] = new XElement("defBlob",
                            new XAttribute("label", vector.Values[i].Label),
                            new XAttribute("name", vector.Values[i].Name),
                            new XAttribute("format", vector.Values[i].format),
                            Convert.ToBase64String(vector.Values[i].value));
                    }
                    ret =
                        new XElement("defBlobVector",
                            new XAttribute("device", vector.Device),
                            new XAttribute("name", vector.Name),
                            new XAttribute("label", vector.Label),
                            new XAttribute("group", vector.Group),
                            new XAttribute("perm", vector.Permission),
                            items).ToString();
                }
            }
            Host.OutputString = ret;
            return ret;
        }
        public bool IsClient { get; internal set; }
    }
}

