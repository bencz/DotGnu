/*
 * IDataObject.cs - Implementation of the
 *			"System.Windows.Forms.IDataObject" class.
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

namespace System.Windows.Forms
{

using System.Runtime.InteropServices;

#if !ECMA_COMPAT
[ComVisible(true)]
#endif
public interface IDataObject
{
	// Get the data associated with the specified format.
	Object GetData(String format);
	Object GetData(Type format);
	Object GetData(String format, bool autoConvert);

	// Determine if there is data present with the specified format.
	bool GetDataPresent(String format);
	bool GetDataPresent(Type format);
	bool GetDataPresent(String format, bool autoConvert);

	// Get a list of all formats that are supported by this data object.
	String[] GetFormats();
	String[] GetFormats(bool autoConvert);

	// Set data on this object.
	void SetData(Object data);
	void SetData(String format, Object data);
	void SetData(Type format, Object data);
	void SetData(String format, bool autoConvert, Object data);

}; // interface IDataObject

}; // namespace System.Windows.Forms
