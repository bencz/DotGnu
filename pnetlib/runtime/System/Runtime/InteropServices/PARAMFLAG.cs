/*
 * PARAMFLAG.cs - Implementation of the
 *			"System.Runtime.InteropServices.PARAMFLAG" class.
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

[ComVisible(false)]
[Serializable]
[Flags]
#if CONFIG_FRAMEWORK_1_2
[Obsolete("Use the class in System.Runtime.InteropServices.ComTypes instead")]
#endif
public enum PARAMFLAG : short
{
	PARAMFLAG_NONE         = 0x0000,
	PARAMFLAG_FIN          = 0x0001,
	PARAMFLAG_FOUT         = 0x0002,
	PARAMFLAG_FLCID        = 0x0004,
	PARAMFLAG_FRETVAL      = 0x0008,
	PARAMFLAG_FOPT         = 0x0010,
	PARAMFLAG_FHASDEFAULT  = 0x0020,
	PARAMFLAG_FHASCUSTDATA = 0x0040

}; // enum PARAMFLAG

#endif // CONFIG_COM_INTEROP

}; // namespace System.Runtime.InteropServices
