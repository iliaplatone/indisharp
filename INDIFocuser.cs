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
    enum INDIFocuserMotion
    {
        INWARD = 0,
        OUTWARD,
    };
    #endregion
    class INDIFocuser : INDIDevice
    {
        #region Constructors / Initialization
        public INDIFocuser(string name, INDIClient host)
            : base(name, host)
        {
            Name = name;
            Host = host;
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
