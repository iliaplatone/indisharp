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

namespace INDI
{
    #region Enums
	public enum INDIDomeMotion
    {
        CW = 0,
        CCW,
    };

	public enum INDIDomeShutterState
    {
        OPEN = 0,
        CLOSE,
    };

	public enum INDIDomeGoTo
    {
        HOME = 0,
        PARK,
    };
    #endregion
	public class INDIDome : INDIDevice
    {
        #region Constructors / Initialization
        public INDIDome(string name, INDIClient host)
            : base(name, host)
        {
            Name = name;
            Host = host;
        }
        #endregion

        #region Standard Methods
        public void Move(INDIDomeMotion dir, Double milliseconds)
        {
            Direction = dir;
            try
            {
                SetNumber("DOME_TIMER", "DOME_TIMER_VALUE", milliseconds);
            }
            catch { }
        }
        public void Abort()
        {
            try
            {
                SetSwitch("DOME_ABORT_MOTION", "ABORT", true);
            }
            catch { }
        }
        public void GoHome()
        {
            try
            {
                SetSwitchVector("DOME_GOTO", (Int32)INDIDomeGoTo.HOME);
            }
            catch { }
        }
        public void Park()
        {
            try
            {
                SetSwitchVector("DOME_GOTO", (Int32)INDIDomeGoTo.PARK);
            }
            catch { }
        }
        #endregion

        #region Standard Properties
        public Double Speed
        {
            get
            {
                try
                {
                    return GetNumber("DOME_SPEED", "DOME_SPEED_VALUE").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_SPEED", "DOME_SPEED_VALUE", value);
                }
                catch { }
            }
        }

        public Double RelativePosition
        {
            get
            {
                try
                {
                    return GetNumber("REL_DOME_POSITION", "DOME_RELATIVE_POSITION").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("REL_DOME_POSITION", "DOME_RELATIVE_POSITION", value);
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
                    return GetNumber("ABS_DOME_POSITION", "DOME_ABSOLUTE_POSITION").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("ABS_DOME_POSITION", "DOME_ABSOLUTE_POSITION", value);
                }
                catch { }
            }
        }

        public Double HomePosition
        {
            get
            {
                try
                {
                    return GetNumber("DOME_PARAMS", "HOME_POSITION").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_PARAMS", "HOME_POSITION", value);
                }
                catch { }
            }
        }

        public Double ParkPosition
        {
            get
            {
                try
                {
                    return GetNumber("DOME_PARAMS", "PARK_POSITION").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_PARAMS", "PARK_POSITION", value);
                }
                catch { }
            }
        }

        public Double AutoSyncThreshold
        {
            get
            {
                try
                {
                    return GetNumber("DOME_PARAMS", "AUTOSYNC_THRESHOLD").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_PARAMS", "HOME_POSITION", value);
                }
                catch { }
            }
        }

        Boolean Slaving
        {
            get
            {
                try
                {
                    return GetSwitch("DOME_AUTOSYNC", "DOME_AUTOSYNC_ENABLE").value;
                }
                catch
                {
                    return false;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("DOME_AUTOSYNC", value ? 0 : 1);
                }
                catch
                {
                }
            }
        }

        public Double Radius
        {
            get
            {
                try
                {
                    return GetNumber("DOME_MEASUREMENTS", "DM_DOME_RADIUS").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_MEASUREMENTS", "DM_DOME_RADIUS", value);
                }
                catch { }
            }
        }

        public Double ShutterWidth
        {
            get
            {
                try
                {
                    return GetNumber("DOME_MEASUREMENTS", "DOME_SHUTTER_WIDTH").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_MEASUREMENTS", "DOME_SHUTTER_WIDTH", value);
                }
                catch { }
            }
        }

        public Double NorthDisplacement
        {
            get
            {
                try
                {
                    return GetNumber("DOME_MEASUREMENTS", "DM_NORTH_DISPLACEMENT").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_MEASUREMENTS", "DM_NORTH_DISPLACEMENT", value);
                }
                catch { }
            }
        }

        public Double EastDisplacement
        {
            get
            {
                try
                {
                    return GetNumber("DOME_MEASUREMENTS", "DM_EAST_DISPLACEMENT").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_MEASUREMENTS", "DM_EAST_DISPLACEMENT", value);
                }
                catch { }
            }
        }

        public Double UpDisplacement
        {
            get
            {
                try
                {
                    return GetNumber("DOME_MEASUREMENTS", "DM_UP_DISPLACEMENT").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_MEASUREMENTS", "DM_UP_DISPLACEMENT", value);
                }
                catch { }
            }
        }

        public Double OpticsDistance
        {
            get
            {
                try
                {
                    return GetNumber("DOME_MEASUREMENTS", "DM_OTA_OFFSET").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("DOME_MEASUREMENTS", "DM_OTA_OFFSET", value);
                }
                catch { }
            }
        }

        public INDIDomeShutterState ShutterState
        {
            get
            {
                try
                {
                    return (INDIDomeShutterState)GetSwitchVector("DOME_SHUTTER").Index;
                }
                catch
                {
                    return INDIDomeShutterState.CLOSE;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("DOME_SHUTTER", (Int32)value);
                }
                catch
                {
                }
            }
        }

        public INDIDomeMotion Direction
        {
            get
            {
                try
                {
                    return (INDIDomeMotion)GetSwitchVector("DOME_MOTION").Index;
                }
                catch
                {
                    return INDIDomeMotion.CW;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("DOME_MOTION", (Int32)value);
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}