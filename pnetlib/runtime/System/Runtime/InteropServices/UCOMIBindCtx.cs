/*
 * UCOMIBindCtx.cs - Implementation of the
 *			"System.Runtime.InteropServices.UCOMIBindCtx" class.
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

namespace System.Runtime.InteropServices
{

#if CONFIG_COM_INTEROP

[Guid("0000000e-0000-0000-C000-000000000046")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[ComImport]
#if CONFIG_FRAMEWORK_1_2
[Obsolete("Use the class in System.Runtime.InteropServices.ComTypes instead")]
#endif
public interface UCOMIBindCtx
{
	void EnumObjectParam(out UCOMIEnumString ppenum);
	void GetBindOptions(ref BIND_OPTS pbindopts);
	void GetObjectParam(String pszKey, out Object ppunk);
	void GetRunningObjectTable(out UCOMIRunningObjectTable pprot);
	void RegisterObjectBound(Object punk);
	void RegisterObjectParam(String pszKey, Object punk);
	void ReleaseBoundObjects();
	void RevokeObjectBound(Object punk);
	void RevokeObjectParam(String pszKey);
	void SetBindOptions(ref BIND_OPTS pbindopts);

}; // class UCOMIBindCtx

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
