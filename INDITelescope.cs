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
    public enum INDICoordSet
    {
        SLEW = 0,
        TRACK,
        SYNC
    };
    public enum INDIDirection
    {
        NORTH = 0,
        SOUTH,
        WEST,
        EAST,
        NONE
    };
    public enum INDIRate
    {
        GUIDE = 0,
        CENTERING,
        FIND,
        MAX
    };
    public enum INDITrackRate
    {
        SIDEREAL = 0,
        SOLAR,
        LUNAR,
        CUSTOM
    };
    #endregion
    public class INDITelescope : INDIDevice
    {
        #region Constructors / Initialization
        public INDITelescope(string name, INDIClient host)
            : base(name, host)
        {
            Name = name;
            Host = host;
        }
        #endregion

        #region Standard Methods
        public void Move(INDIDirection direction)
        {
            try
            {
                string vector = "TELESCOPE_MOTION_";
                string number = "MOTION_";
                switch (direction)
                {
                    case INDIDirection.NORTH:
                        vector += "NS";
                        number += "N";
                        break;
                    case INDIDirection.SOUTH:
                        vector += "NS";
                        number += "S";
                        break;
                    case INDIDirection.WEST:
                        vector += "WE";
                        number += "W";
                        break;
                    case INDIDirection.EAST:
                        vector += "WE";
                        number += "E";
                        break;
                }
                SetSwitch(vector, number, true);
            }
            catch
            {
            }
        }

        public void Stop(INDIDirection direction)
        {
            try
            {
                string vector = "TELESCOPE_MOTION_";
                string number = "MOTION_";
                switch (direction)
                {
                    case INDIDirection.NORTH:
                        vector += "NS";
                        number += "N";
                        break;
                    case INDIDirection.SOUTH:
                        vector += "NS";
                        number += "S";
                        break;
                    case INDIDirection.WEST:
                        vector += "WE";
                        number += "W";
                        break;
                    case INDIDirection.EAST:
                        vector += "WE";
                        number += "E";
                        break;
                }
                SetSwitch(vector, number, false);
            }
            catch
            {
            }
        }

        public void PulseGuide(INDIDirection direction, Int64 ms)
        {
            string vector = "TELESCOPE_TIMED_GUIDE_";
            string number = "TIMED_GUIDE_";
            switch (direction)
            {
                case INDIDirection.NORTH:
                    vector += "NS";
                    number += "N";
                    break;
                case INDIDirection.SOUTH:
                    vector += "NS";
                    number += "S";
                    break;
                case INDIDirection.WEST:
                    vector += "WE";
                    number += "W";
                    break;
                case INDIDirection.EAST:
                    vector += "WE";
                    number += "E";
                    break;
            }
            try
            {
                SetNumber(vector, number, ms);
            }
            catch
            {
            }
        }

        public void SyncToAltAz(Double Altitude, Double Azimuth)
        {
            CoordSet = INDICoordSet.SYNC;
            Double[] values = { Altitude, Azimuth };
            SetNumberVector("HORIZONTAL_COORD", values);
        }

        public void SlewToAltAz(Double Altitude, Double Azimuth)
        {
            CoordSet = INDICoordSet.SLEW;
            Double[] values = { Altitude, Azimuth };
            SetNumberVector("HORIZONTAL_COORD", values);
        }

        public void SyncToRaDec(Double RightAscension, Double Declination)
        {
            CoordSet = INDICoordSet.SYNC;
            Double[] values = { RightAscension, Declination };
            SetNumberVector("EQUATORIAL_EOD_COORD", values);
        }

        public void SlewToRaDec(Double RightAscension, Double Declination)
        {
            CoordSet = INDICoordSet.SLEW;
            Double[] values = { RightAscension, Declination };
            SetNumberVector("EQUATORIAL_EOD_COORD", values);
        }

        public void Abort()
        {
            try
            {
                SetSwitch("TELESCOPE_ABORT_MOTION", "ABORT_MOTION", true);
            }
            catch
            {
            }
        }
        #endregion

        #region Standard Properties
        public Double RaJ2000
        {
            get
            {
                try
                {
                    return GetNumber("EQUATORIAL_COORD", "RA").value;
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
                    SetNumber("EQUATORIAL_COORD", "RA", value);
                }
                catch
                {
                }
            }
        }

        public Double DecJ2000
        {
            get
            {
                try
                {
                    return GetNumber("EQUATORIAL_COORD", "DEC").value;
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
                    SetNumber("EQUATORIAL_COORD", "DEC", value);
                }
                catch
                {
                }
            }
        }

        public Double Ra
        {
            get
            {
                try
                {
                    return GetNumber("EQUATORIAL_EOD_COORD", "RA").value;
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
                    SetNumber("EQUATORIAL_EOD_COORD", "RA", value);
                }
                catch
                {
                }
            }
        }

        public Double Dec
        {
            get
            {
                try
                {
                    return GetNumber("EQUATORIAL_EOD_COORD", "DEC").value;
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
                    SetNumber("EQUATORIAL_EOD_COORD", "DEC", value);
                }
                catch
                {
                }
            }
        }

        public Double Alt
        {
            get
            {
                try
                {
                    return GetNumber("HORIZONTAL_COORD", "ALT").value;
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
                    SetNumber("HORIZONTAL_COORD", "ALT", value);
                }
                catch
                {
                }
            }
        }

        public Double Az
        {
            get
            {
                try
                {
                    return GetNumber("HORIZONTAL_COORD", "AZ").value;
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
                    SetNumber("HORIZONTAL_COORD", "AZ", value);
                }
                catch
                {
                }
            }
        }

        public Boolean Parked
        {
            get
            {
                try
                {
                    return GetSwitch("TELESCOPE_PARK", "PARK").value;
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
                    SetSwitchVector("TELESCOPE_PARK", value ? 0 : 1);
                }
                catch
                {
                }
            }
        }

        public string Diameter
        {
            get
            {
                try
                {
                    return GetText("TELESCOPE_INFO", "TELESCOPE_APERTURE").value;
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    SetText("TELESCOPE_INFO", "TELESCOPE_APERTURE", value);
                }
                catch
                {
                }
            }
        }

        public string FocalLenght
        {
            get
            {
                try
                {
                    return GetText("TELESCOPE_INFO", "TELESCOPE_FOCAL_LENGTH").value;
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    SetText("TELESCOPE_INFO", "TELESCOPE_FOCAL_LENGTH", value);
                }
                catch
                {
                }
            }
        }

        public string GuiderDiameter
        {
            get
            {
                try
                {
                    return GetText("TELESCOPE_INFO", "GUIDER_APERTURE").value;
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    SetText("TELESCOPE_INFO", "GUIDER_APERTURE", value);
                }
                catch
                {
                }
            }
        }

        public string GuiderFocalLenght
        {
            get
            {
                try
                {
                    return GetText("TELESCOPE_INFO", "GUIDER_FOCAL_LENGTH").value;
                }
                catch
                {
                    return "";
                }
            }
            set
            {
                try
                {
                    SetText("TELESCOPE_INFO", "GUIDER_FOCAL_LENGTH", value);
                }
                catch
                {
                }
            }
        }

        public INDICoordSet CoordSet
        {
            get
            {
                try
                {
                    return (INDICoordSet)GetSwitchVector("ON_COORD_SET").Index;
                }
                catch
                {
                    return INDICoordSet.TRACK;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("ON_COORD_SET", (Int32)value);
                }
                catch
                {
                }
            }
        }

        public INDIRate SlewRate
        {
            get
            {
                try
                {
                    return (INDIRate)GetSwitchVector("TELESCOPE_SLEW_RATE").Index;
                }
                catch
                {
                    return INDIRate.GUIDE;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("TELESCOPE_SLEW_RATE", (Int32)value);
                }
                catch
                {
                }
            }
        }

        public INDITrackRate TrackRate
        {
            get
            {
                try
                {
                    return (INDITrackRate)GetSwitchVector("TELESCOPE_TRACK_RATE").Index;
                }
                catch
                {
                    return INDITrackRate.CUSTOM;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("TELESCOPE_TRACK_RATE", (Int32)value);
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}