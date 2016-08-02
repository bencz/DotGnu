/*
 * IrDADeviceInfo.cs - Implementation of the
 *			"System.Net.IrDADeviceInfo" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System.Net.Sockets
{

using System.Text;

public class IrDADeviceInfo
{
	// Internal state.
	internal IrDACharacterSet characterSet;
	internal byte[] deviceID;
	internal String deviceName;
	internal IrDAHints hints;

	// Constructor.
	internal IrDADeviceInfo(byte[] data, int posn)
			{
				deviceID = new byte [4];
				deviceID[0] = data[posn];
				deviceID[1] = data[posn + 1];
				deviceID[2] = data[posn + 2];
				deviceID[3] = data[posn + 3];
				int offset = posn + 4;
				StringBuilder builder = new StringBuilder();
				while(offset < (posn + 26) && data[offset] != 0)
				{
					builder.Append((char)(data[offset]));
					++offset;
				}
				deviceName = builder.ToString();
				hints = (IrDAHints)(data[posn + 26] | (data[posn + 27] << 8));
				characterSet = (IrDACharacterSet)(data[posn + 28]);
			}

	// Get the device's properties.
	public IrDACharacterSet CharacterSet
			{
				get
				{
					return characterSet;
				}
			}
	public byte[] DeviceID
			{
				get
				{
					return deviceID;
				}
			}
	public String DeviceName
			{
				get
				{
					return deviceName;
				}
			}
	public IrDAHints Hints
			{
				get
				{
					return hints;
				}
			}

}; // class IrDADeviceInfo

}; // namespace System.Net.Sockets
