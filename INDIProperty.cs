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

namespace INDI
{
    public class IVector
    {
        public string Device;
        public string Name;
        public string Label;
        public string Group;
        public string Permission;
        public string Rule;
        public IVector(string device, string name, string label, string group, string permission, string rule)
        {
            Device = device != null ? device : "";
            Name = name != null ? name : "";
            Label = label != null ? label : "";
            Group = group != null ? group : "";
            Permission = permission != null ? permission : "";
            Rule = rule != null ? rule : "";
        }
        public void Change(IVector v)
        {
            Device = v.Device != null ? v.Device : Device;
            Name = v.Name != null ? v.Name : Name;
            Label = v.Label != null ? v.Label : Label;
            Group = v.Group != null ? v.Group : Group;
            Permission = v.Permission != null ? v.Permission : Permission;
            Rule = v.Rule != null ? v.Rule : Rule;
        }
    }
	public class ISwitchVector : IVector {
		public List<INDISwitch> Values;
        public Int32 Index;
		public ISwitchVector( string device, string name, string label, string group, string permission, string rule, List<INDISwitch> v)
            :base(device, name, label, group, permission, rule)
		{
            Values = v;
            if(rule.ToLower() == "oneofmany")
            {
                for (Index = 0; Index < v.Count; Index++)
                    if (v[Index].value)
                        break;
            }
		}
        public void Change(ISwitchVector v)
        {
            base.Change(v);
            Values = v.Values;
        }
	}
	public class ITextVector : IVector
    {
        public List<INDIText> Values;
		public ITextVector( string device, string name, string label, string group, string permission, string rule, List<INDIText> v)
            : base(device, name, label, group, permission, rule)
        {
            Values = v;
		}
        public void Change(ITextVector v)
        {
            base.Change(v);
            Values = v.Values;
        }
	}
	public class INumberVector : IVector
    {
        public List<INDINumber> Values;
		public INumberVector( string device, string name, string label, string group, string permission, string rule, List<INDINumber> v)
            : base(device, name, label, group, permission, rule)
        {
            Values = v;
		}
        public void Change(INumberVector v)
        {
            base.Change(v);
            Values = v.Values;
        }
	}
	public class IBlobVector : IVector
    {
        public List<INDIBlob> Values;
        public IBlobVector(string device, string name, string label, string group, string permission, string rule, List<INDIBlob> v)
            : base(device, name, label, group, permission, rule)
        {
            Values = v;
		}
        public void Change(IBlobVector v)
        {
            base.Change(v);
            Values = v.Values;
        }
	}

	public class INDISwitch {
		public string Name;
		public string Label;
		public Boolean value;
        public INDISwitch(string name, string label, Boolean val)
        {
            Label = label == null ? "" : label;
            Name = name == null ? "" : name;
            value = val;
        }
	}

    public class INDIText
    {
		public string Name;
		public string Label;
        public string value;
        public INDIText(string name, string label, string val)
        {
            Label = label == null ? "" : label;
            Name = name == null ? "" : name;
            value = val;
        }
	}

    public class INDINumber
    {
		public string Name;
		public string Label;
		public string format;
		public Double min;
		public Double max;
		public Double step;
		public Double value;
        public INDINumber(string name, string label, string frmt, Double minimum, Double maximum, Double stepp, Double val)
        {
            Label = label == null ? "" : label;
            Name = name == null ? "" : name;
            format = frmt == null ? "" : frmt;
            value = val;
            min = minimum;
            max = maximum;
            step = stepp;
        }
	}

    public class INDIBlob
    {
		public string format;
		public string Name;
		public string Label;
		public byte[] value;
		public Int32 size;
        public INDIBlob(string name, string label, string frmt, byte[] val, Int32 length)
        {
            Label = label == null ? "" : label;
            Name = name == null ? "" : name;
            format = frmt == null ? "" : frmt;
            value = val;
            size = length;
        }
	}
}

