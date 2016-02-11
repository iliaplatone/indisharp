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
    class INDIFilterWheel : INDIDevice
    {
        #region Constructors / Initialization
        public INDIFilterWheel(string name, INDIClient host)
            : base(name, host)
        {
            Name = name;
            Host = host;
        }
        #endregion

        #region Standard Properties
        public Int32 FilterSlot
        {
            get
            {
                try
                {
                    return (Int32)GetNumber("FILTER_SLOT", "FILTER_SLOT_VALUE").value;
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
                    Int32 v = value;
                    if (v < 1)
                        v = 1;
                    SetNumber("FILTER_SLOT", "FILTER_SLOT_VALUE", v);
                }
                catch
                {
                }
            }
        }

        public string FilterName
        {
            get
            {
                try
                {
                    return GetText("FILTER_NAME", "FILTER_NAME_VALUE").value;
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
                    SetText("FILTER_NAME", "FILTER_NAME_VALUE", value);
                }
                catch
                {
                }
            }
        }
        #endregion
    }
}
