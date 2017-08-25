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

namespace INDI
{
    #region Custom Event Argument classes
    public class INDIFocuserNumberEventArgs : IsNewNumberEventArgs
    {
        public INDIFocuserNumberType Type;
        public List<INDINumber> Values;
        public INDIFocuserNumberEventArgs(INumberVector vector, string dev) : base(vector, dev)
        {
            Values = vector.Values;
            switch (vector.Name)
            {
                case "FOCUS_SPEED":
                    Type = INDIFocuserNumberType.Speed;
                    break;
                case "FOCUS_TIMER":
                    Type = INDIFocuserNumberType.Timer;
                    break;
                case "REL_FOCUS_POSITION":
                    Type = INDIFocuserNumberType.RelPosition;
                    break;
                case "ABS_FOCUS_POSITION":
                    Type = INDIFocuserNumberType.AbsPosition;
                    break;

                case "TIME_LST":
                    Type = INDIFocuserNumberType.TimeLst;
                    break;
                case "GEOGRAPHIC_COORD":
                    Type = INDIFocuserNumberType.GeographicCoord;
                    break;
                case "ATMOSPHERE":
                    Type = INDIFocuserNumberType.Atmosphere;
                    break;
                default:
                    Type = INDIFocuserNumberType.Other;
                    break;
            }
        }
    }
    public class INDIFocuserSwitchEventArgs : IsNewSwitchEventArgs
    {
        public INDIFocuserSwitchType Type;
        public List<INDISwitch> Values;
        public INDIFocuserSwitchEventArgs(ISwitchVector vector, string dev) : base(vector, dev)
        {
            Values = vector.Values;
            switch (vector.Name)
            {
                case "FOCUS_MOTION":
                    Type = INDIFocuserSwitchType.Motion;
                    break;
                case "FOCUS_ABORT_MOTION":
                    Type = INDIFocuserSwitchType.AbortMotion;
                    break;

                case "CONNECTION":
                    Type = INDIFocuserSwitchType.Connection;
                    break;
                case "UPLOAD_MODE":
                    Type = INDIFocuserSwitchType.UploadMode;
                    break;
                default:
                    Type = INDIFocuserSwitchType.Other;
                    break;
            }
        }
    }
    public class INDIFocuserTextEventArgs : IsNewTextEventArgs
    {
        public INDIFocuserTextType Type;
        public List<INDIText> Values;
        public INDIFocuserTextEventArgs(ITextVector vector, string dev) : base(vector, dev)
        {
            Values = vector.Values;
            switch (vector.Name)
            {
                case "DEVICE_PORT":
                    Type = INDIFocuserTextType.DevicePort;
                    break;
                case "TIME_UTC":
                    Type = INDIFocuserTextType.TimeUtc;
                    break;
                case "UPLOAD_SETTINGS":
                    Type = INDIFocuserTextType.UploadSettings;
                    break;
                case "ACTIVE_DEVICES":
                    Type = INDIFocuserTextType.ActiveDevices;
                    break;
                default:
                    Type = INDIFocuserTextType.Other;
                    break;
            }
        }
    }
    #endregion
    #region Enums
    public enum INDIFocuserNumberType
    {
        TimeLst,
        GeographicCoord,
        Atmosphere,
        Other,

        Speed,
        Timer,
        RelPosition,
        AbsPosition,
    }
    public enum INDIFocuserSwitchType
    {
        Connection,
        UploadMode,
        Other,
        
        Motion,
        AbortMotion,
    }
    public enum INDIFocuserTextType
    {
        DevicePort,
        TimeUtc,
        UploadSettings,
        ActiveDevices,
        Other,
    }
    public enum INDIFocuserMotion
    {
        INWARD = 0,
        OUTWARD,
    };
    #endregion
	public class INDIFocuser : INDIDevice
    {
        public event EventHandler<INDIFocuserNumberEventArgs> IsNewNumber = null;
        public event EventHandler<INDIFocuserSwitchEventArgs> IsNewSwitch = null;
        public event EventHandler<INDIFocuserTextEventArgs> IsNewText = null;
        #region Constructors / Initialization
        public INDIFocuser(string name, INDIClient host, bool client = true)
            : base(name, host, client)
        {
            if (!client)
            {
                AddNumberVector(new INumberVector(Name, "FOCUS_SPEED", "Select focus speed", "Main Control", "rw", "", new List<INDINumber>
            {
                new INDINumber("FOCUS_SPEED_VALUE", "Set focuser speed", "%3.1f", 0, 1000.0, 0.1, 0.0)
            }));
                AddSwitchVector(new ISwitchVector(Name, "FOCUS_MOTION", "Move focuser", "Motion Control", "rw", "OneOfMany", new List<INDISwitch>
            {
                new INDISwitch("FOCUS_INWARD", "Focus inward", true),
                new INDISwitch("FOCUS_OUTWARD", "Focus outward", false)
            }));
                AddNumberVector(new INumberVector(Name, "FOCUS_TIMER", "Focuser speed", "Motion Control", "rw", "", new List<INDINumber>
            {
                new INDINumber("FOCUS_TIMER_VALUE", "Focus in the direction, speed and time selected", "%4.0f", 0.00, 1000.0, 1.0, 0.0)
            }));
                AddNumberVector(new INumberVector(Name, "REL_FOCUS_POSITION", "Focuser Relative position", "Motion Control", "rw", "", new List<INDINumber>
            {
                new INDINumber("FOCUS_RELATIVE_POSITION", "Focuser Relative position", "%5.0f", 0.00, 50000.0, 1, 0.0)
            }));
                AddNumberVector(new INumberVector(Name, "ABS_FOCUS_POSITION", "Focuser Absolute position", "Motion Control", "rw", "", new List<INDINumber>
            {
                new INDINumber("FOCUS_ABSOLUTE_POSITION", "Focuser Absolute position", "%5.0f", 0.00, 50000.0, 1, 0.0)
            }));
                AddSwitchVector(new ISwitchVector(Name, "FOCUS_ABORT_MOTION", "Abort focuser motion", "Motion Control", "rw", "AtMostOne", new List<INDISwitch>
            {
                new INDISwitch("ABORT", "Abort focuser motion", false)
            }));
            }
        }
        #endregion

        #region Standard Methods
        public void Move(INDIFocuserMotion dir, Double milliseconds)
        {
            Direction = dir;
            try
            {
                SetNumber("FOCUS_TIMER", "FOCUS_TIMER_VALUE", milliseconds);
            }
            catch { }
        }
        public void Abort()
        {
            SetSwitch("FOCUS_ABORT_MOTION", "ABORT", true);
        }

        public override void isNewNumber(Object sender, IsNewNumberEventArgs e)
        {
            base.isNewNumber(sender, e);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            try
            {
                if (e.Vector.Device == Name)
                {
                    IsNewNumber?.Invoke(this, new INDIFocuserNumberEventArgs(e.Vector, e.Device));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void isNewSwitch(Object sender, IsNewSwitchEventArgs e)
        {
            base.isNewSwitch(sender, e);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            try
            {
                if (e.Vector.Device == Name)
                {
                    IsNewSwitch?.Invoke(this, new INDIFocuserSwitchEventArgs(e.Vector, e.Device));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void isNewText(Object sender, IsNewTextEventArgs e)
        {
            base.isNewText(sender, e);
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            try
            {
                if (e.Vector.Device == Name)
                {
                    IsNewText?.Invoke(this, new INDIFocuserTextEventArgs(e.Vector, e.Device));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Standard Properties
        public Double Speed
        {
            get
            {
                try
                {
                    return GetNumber("FOCUS_SPEED", "FOCUS_SPEED_VALUE").value;
                }
                catch
                {
                    return 0;
                }
            }
            set
            {
                try
                {
                    SetNumber("FOCUS_SPEED", "FOCUS_SPEED_VALUE", value);
                }
                catch
                {
                }
            }
        }

        public Double RelativePosition
        {
            get
            {
                try
                {
                    return GetNumber("REL_FOCUS_POSITION", "FOCUS_RELATIVE_POSITION").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("REL_FOCUS_POSITION", "FOCUS_RELATIVE_POSITION", value);
                }
                catch { }
            }
        }

        public Double AbsolutePosition
        {
            get
            {
                try
                {
                    return GetNumber("ABS_FOCUS_POSITION", "FOCUS_ABSOLUTE_POSITION").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("ABS_FOCUS_POSITION", "FOCUS_ABSOLUTE_POSITION", value);
                }
                catch { }
            }
        }

        public INDIFocuserMotion Direction
        {
            get
            {
                try
                {
                    return (INDIFocuserMotion)GetSwitchVector("FOCUS_MOTION").Index;
                }
                catch
                {
                    return INDIFocuserMotion.INWARD;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("FOCUS_MOTION", (Int32)value);
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}
