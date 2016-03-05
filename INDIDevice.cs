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
    public enum INDIUploadMode
    {
        CLIENT = 0,
        LOCAL,
        BOTH
    };
    #endregion
    public class INDIDevice : INDIBaseDevice, IDisposable
    {
        #region Constructors / Initialization
        public INDIDevice(string name, INDIClient host)
            : base(name, host)
        {
        }
        public void Dispose()
        {
            Connected = false;
            GC.Collect();
        }
        #endregion

        #region Standard Properties
        public Boolean Connected
        {
            get
            {
                try
                {
                    QueryProperties("CONNECTION");
                    return GetSwitch("CONNECTION", "CONNECT").value;
                }
                catch
                {
                }
                return false;
            }
            set
            {
                try
                {
                    if (value == true)
                        SetSwitchVector("CONNECTION", 0);
                    else
                        SetSwitchVector("CONNECTION", 1);
                }
                catch
                {
                }
            }
        }

        public string DevicePort
        {
            get
            {
                try
                {
                    return GetText("DEVICE_PORT", "PORT").value;
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
                    SetText("DEVICE_PORT", "PORT", value);
                }
                catch { }
            }
        }

        public Double SiderealTime
        {
            get
            {
                try
                {
                    return GetNumber("TIME_LST", "LST").value;
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
                    SetNumber("TIME_LST", "LST", value);
                }
                catch { }
            }
        }

        public DateTime UtcTime
        {
            get
            {
                try
                {
                    return DateTime.Parse(GetText("TIME_UTC", "UTC").value);
                }
                catch
                {
                    return DateTime.UtcNow;
                }
            }
            set
            {
                try
                {
                    SetText("TIME_UTC", "UTC", value.ToShortDateString());
                }
                catch { }
            }
        }

        public DateTime UtcOffset
        {
            get
            {
                try
                {
                    return DateTime.Parse(GetText("TIME_UTC", "OFFSET").value);
                }
                catch
                {
                    return new DateTime(0);
                }
            }
            set
            {
                try
                {
                    SetText("TIME_UTC", "OFFSET", value.ToShortDateString());
                }
                catch { }
            }
        }

        public Double Latitude
        {
            get
            {
                try
                {
                    return GetNumber("GEOGRAPHIC_COORD", "LAT").value;
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
                    SetNumber("GEOGRAPHIC_COORD", "LAT", value);
                }
                catch { }
            }
        }

        public Double Longitude
        {
            get
            {
                try
                {
                    return GetNumber("GEOGRAPHIC_COORD", "Int64").value;
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
                    SetNumber("GEOGRAPHIC_COORD", "Int64", value);
                }
                catch { }
            }
        }

        public Double Elevation
        {
            get
            {
                try
                {
                    return GetNumber("GEOGRAPHIC_COORD", "ELEV").value;
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
                    SetNumber("GEOGRAPHIC_COORD", "ELEV", value);
                }
                catch { }
            }
        }

        public Double AmbientTemperature
        {
            get
            {
                try
                {
                    return GetNumber("ATMOSPHERE", "TEMPERATURE").value;
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
                    SetNumber("ATMOSPHERE", "TEMPERATURE", value);
                }
                catch { }
            }
        }

        public Double AmbientPressure
        {
            get
            {
                try
                {
                    return GetNumber("ATMOSPHERE", "PRESSURE").value;
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
                    SetNumber("ATMOSPHERE", "PRESSURE", value);
                }
                catch { }
            }
        }

        public Double AmbientHumidity
        {
            get
            {
                try
                {
                    return GetNumber("ATMOSPHERE", "HUMIDITY").value;
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
                    SetNumber("ATMOSPHERE", "HUMIDITY", value);
                }
                catch { }
            }
        }
        public INDIUploadMode UploadMode
        {
            get
            {
                try
                {
                    return (INDIUploadMode)GetSwitchVector("UPLOAD_MODE").Index;
                }
                catch
                {
                    return INDIUploadMode.CLIENT;
                }
            }
            set
            {
                try
                {
                    SetSwitchVector("UPLOAD_MODE", (Int32)value);
                }
                catch
                {
                }
            }
        }
        public string UploadDirectory
        {
            get
            {
                try
                {
                    return GetText("UPLOAD_SETTINGS", "UPLOAD_DIR").value;
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
                    SetText("UPLOAD_SETTINGS", "UPLOAD_DIR", value);
                }
                catch { }
            }
        }

        public string UploadPrefix
        {
            get
            {
                try
                {
                    return GetText("UPLOAD_SETTINGS", "UPLOAD_PREFIX").value;
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
                    SetText("UPLOAD_SETTINGS", "UPLOAD_PREFIX", value);
                }
                catch { }
            }
        }

        public string ActiveTelescope
        {
            get
            {
                try
                {
                    return GetText("ACTIVE_DEVICES", "ACTIVE_TELESCOPE").value;
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
                    SetText("ACTIVE_DEVICES", "ACTIVE_TELESCOPE", value);
                }
                catch { }
            }
        }

        public string ActiveCCD
        {
            get
            {
                try
                {
                    return GetText("ACTIVE_DEVICES", "ACTIVE_CCD").value;
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
                    SetText("ACTIVE_DEVICES", "ACTIVE_CCD", value);
                }
                catch { }
            }
        }

        public string ActiveFilterWheel
        {
            get
            {
                try
                {
                    return GetText("ACTIVE_DEVICES", "ACTIVE_FILTER").value;
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
                    SetText("ACTIVE_DEVICES", "ACTIVE_FILTER", value);
                }
                catch { }
            }
        }

        public string ActiveFocuser
        {
            get
            {
                try
                {
                    return GetText("ACTIVE_DEVICES", "ACTIVE_FOCUSER").value;
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
                    SetText("ACTIVE_DEVICES", "ACTIVE_FOCUSER", value);
                }
                catch { }
            }
        }

        public string ActiveDome
        {
            get
            {
                try
                {
                    return GetText("ACTIVE_DEVICES", "ACTIVE_DOME").value;
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
                    SetText("ACTIVE_DEVICES", "ACTIVE_DOME", value);
                }
                catch { }
            }
        }

        public string ActiveGPS
        {
            get
            {
                try
                {
                    return GetText("ACTIVE_DEVICES", "ACTIVE_GPS").value;
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
                    SetText("ACTIVE_DEVICES", "ACTIVE_GPS", value);
                }
                catch { }
            }
        }
        #endregion
    }
}