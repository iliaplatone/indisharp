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
    #region Custom Event Argument classes
    public class ImageReceivedEventArgs : EventArgs
	{
		public byte[] ImageData;
		public string Format;
		public string Name;
        public string Vector;

		public ImageReceivedEventArgs(byte[] imagedata, string name, string vector, string format)
		{
            Vector = vector;
			Name = name;
			Format = format;
			ImageData = imagedata;
		}
	}
    #endregion
    #region Enums
    public enum INDIFrameType
    {
        LIGHT = 0,
        BIAS,
        DARK,
        FLAT
    };
    #endregion
    public class INDICamera : INDIDevice
    {
        public event EventHandler<ImageReceivedEventArgs> ImageReceived = null;
        #region Constructors / Initialization
        public INDICamera(string name, INDIClient host)
            : base(name, host)
        {
            EnableBLOB(true);
            IsNewBlob += imageReceived;
        }
        #endregion

        #region Standard Methods
        public void StartExposure(Double duration, INDIFrameType type)
        {
            try
            {
                SetSwitchVector("CCD_FRAME_TYPE", (Int32)type);
                SetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE", duration);
            }
            catch
            {
            }
        }

        public void ResetFrame()
        {
            try
            {
                SetSwitch("CCD_FRAME_RESET", "RESET", true);
            }
            catch { }
        }

        public void Abort()
        {
            try
            {
                SetSwitch("CCD_ABORT_EXPOSURE", "ABORT", true);
            }
            catch { }
        }

        void imageReceived(Object sender, IsNewBlobEventArgs e)
        {
            try
            {
                if (e.Vector.Device == Name)
                {
                    INDIClient caller = (INDIClient)sender;
                    for (int i = 0; i < e.Vector.Values.Count; i++)
                    {
                        Console.WriteLine("Received BLOB " + e.Vector.Values[i].Name + " of size " + e.Vector.Values[i].size + " from device " + e.Device + "@" + caller.Address + ":" + caller.Port);
                        if (ImageReceived != null)
                        {
                            ImageReceived(this, new ImageReceivedEventArgs(e.Vector.Values[i].value, e.Vector.Values[i].Name, e.Vector.Name, e.Vector.Values[0].format));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region Standard Properties
        public Boolean CompessionEnabled
        {
            set
            {
                try
                {
                    SetSwitch("CCD_COMPRESSION", "CCD_COMPRESSION_ON", value);
                }
                catch
                {
                }
            }
            get
            {
                try
                {
                    return GetSwitch("CCD_COMPRESSION", "CCD_COMPRESSION_ON").value;
                }
                catch
                {
                }
                return false;
            }
        }

        public Int16 BayerX
        {
            get
            {
                try
                {
                    Int16.Parse(GetText("CCD_CFA", "CFA_OFFSET_X").value);
                }
                catch
                {
                }
                return 0;
            }
        }

        public Int16 BayerY
        {
            get
            {
                try
                {
                    Int16.Parse(GetText("CCD_CFA", "CFA_OFFSET_Y").value);
                }
                catch
                {
                }
                return 0;
            }
        }

        public Double ExposureTime
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").value;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public Boolean CoolerStarted
        {
            get
            {
                try
                {
                    return GetSwitch("CCD_COOLER", "COOLER_ON").value;
                }
                catch { }
                return false;
            }
            set
            {
                try
                {
                    SetSwitchVector("CCD_COOLER", value ? 0 : 1);
                }
                catch { }
            }
        }

        public Double CoolerPower
        {
            get
            {
                try
                {
                    return GetNumber("CCD_COOLER_POWER", "CCD_COOLER_VALUE").value;
                }
                catch { }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_COOLER_POWER", "CCD_COOLER_VALUE", value);
                }
                catch { }
            }
        }

        public Boolean CanSetCoolerPower
        {
            get
            {
                try
                {
                    return GetNumberVector("CCD_COOLER_POWER").Permission == "rw";
                }
                catch { }
                return false;
            }
        }

        public Int16 BinX
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_BINNING", "HOR_BIN").value;
                }
                catch
                {
                }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_BINNING", "HOR_BIN", value);
                }
                catch
                {
                }
            }
        }

        public Int16 BinY
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_BINNING", "VER_BIN").value;
                }
                catch
                {
                }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_BINNING", "VER_BIN", value);
                }
                catch
                {
                }
            }
        }

        public Int16 MaxBinX
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_BINNING", "HOR_BIN").max;
                }
                catch
                {
                }
                return 0;
            }
        }

        public Int16 MaxBinY
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_BINNING", "VER_BIN").max;
                }
                catch
                {
                }
                return 0;
            }
        }

        public Int32 StartX
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_FRAME", "X").value;
                }
                catch
                {
                }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "X", value);
                }
                catch
                {
                }
            }
        }

        public Int32 StartY
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_FRAME", "Y").value;
                }
                catch
                {
                }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "Y", value);
                }
                catch
                {
                }
            }
        }

        public Int32 Width
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_FRAME", "WIDTH").value;
                }
                catch
                {
                }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "WIDTH", value);
                }
                catch
                {
                }
            }
        }

        public Int32 Height
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_FRAME", "HEIGHT").value;
                }
                catch
                {
                }
                return 0;
            }
            set
            {
                try
                {
                    SetNumber("CCD_FRAME", "HEIGHT", value);
                }
                catch
                {
                }
            }
        }

        public Int32 FullWidth
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_INFO", "CCD_MAX_X").value;
                }
                catch
                {
                }
                return 0;
            }
        }

        public Int32 FullHeight
        {
            get
            {
                try
                {
                    return (Int16)GetNumber("CCD_INFO", "CCD_MAX_Y").value;
                }
                catch
                {
                }
                return 0;
            }
        }

        public Double MinExposure
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").min;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public Double MaxExposure
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").max;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public Double ExposureStep
        {
            get
            {
                try
                {
                    return GetNumber("CCD_EXPOSURE", "CCD_EXPOSURE_VALUE").step;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public Double MaxADU
        {
            get
            {
                try
                {
                    return 65536;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public Double PixelSizeX
        {
            get
            {
                try
                {
                    return GetNumber("CCD_INFO", "CCD_PIXEL_SIZE_X").value;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public Double PixelSizeY
        {
            get
            {
                try
                {
                    return GetNumber("CCD_INFO", "CCD_PIXEL_SIZE_Y").value;
                }
                catch
                {
                }
                return 0.0;
            }
        }

        public Double CCDTemperature
        {
            get
            {
                try
                {
                    return GetNumber("CCD_TEMPERATURE", "CCD_TEMPERATURE_VALUE").value;
                }
                catch
                {
                }
                return 0.0;
            }
            set
            {
                try
                {
                    if (CanSetCCDTemperature)
                    {
                        SetNumber("CCD_TEMPERATURE", "CCD_TEMPERATURE_VALUE", value);
                    }
                }
                catch
                {
                }
            }
        }

        public Boolean CanSetCCDTemperature
        {
            get
            {
                try
                {
                    return (GetNumberVector("CCD_TEMPERATURE").Permission == "rw");
                }
                catch
                {
                }
                return false;
            }
        }
        #endregion
    }
}

