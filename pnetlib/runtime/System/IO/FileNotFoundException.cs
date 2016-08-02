/*
 * FileNotFoundException.cs - Implementation of the
 *		"System.IO.FileNotFoundException" class.
 *
 * Copyright (C) 2001, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.IO
{

using System;
using Platform;
using System.Runtime.Serialization;

public class FileNotFoundException : IOException
{
	// Internal state.
	private String fileName;
#if !ECMA_COMPAT
	private String fusionLog;
#endif

	// Constructors.
	public FileNotFoundException()
			: base(Errno.ENOENT, _("Exception_FileNotFound"))
			{
				fileName = null;
			}
	public FileNotFoundException(String msg)
			: base(Errno.ENOENT, msg)
			{
				fileName = null;
			}
	public FileNotFoundException(String msg, String fileName)
			: base(Errno.ENOENT, msg)
			{
				this.fileName = fileName;
			}
	public FileNotFoundException(String msg, Exception inner)
			: base(Errno.ENOENT, msg, inner)
			{
				fileName = null;
			}
	public FileNotFoundException(String msg, String fileName, Exception inner)
			: base(Errno.ENOENT, msg, inner)
			{
				this.fileName = fileName;
			}
#if CONFIG_SERIALIZATION
	protected FileNotFoundException(SerializationInfo info,
									StreamingContext context)
			: base(info, context)
			{
				fileName = info.GetString("FileNotFound_FileName");
				fusionLog = info.GetString("FileNotFound_FusionLog");
			}
#endif

	// Get the message for this exception.  Because of "MessageDefault",
	// we don't actually need this.  However, we include it because
	// the ECMA standard expects this to be present.
	public override String Message
			{
				get
				{
					return base.Message;
				}
			}

	// Convert this exception into a string.  Because of "MessageExtra",
	// we don't actually need this.  However, we include it because the
	// ECMA standard expects this to be present.
	public override String ToString()
			{
				return base.ToString();
			}

	// Get the filename that was associated with this exception.
	public String FileName
			{
				get
				{
					return fileName;
				}
			}

	// Get the extra data to insert into "Exception::ToString()"'s result.
	internal override String MessageExtra
			{
				get
				{
					if(fileName != null)
					{
						return String.Format
							   		(_("Exception_Filename"), fileName);
					}
					else
					{
						return null;
					}
				}
			}

	// Get the default message for this exception type.
	internal override String MessageDefault
			{
				get
				{
					if(fileName != null)
					{
						return String.Format
							(_("IO_FileNotFound"), fileName);
					}
					else
					{
						return _("Exception_FileNotFound");
					}
				}
			}

#if !ECMA_COMPAT
	// Get the fusion log for this exception.
	public String FusionLog
			{
				get
				{
					return fusionLog;
				}
			}
#endif

	// Get the default HResult value for this type of exception.
	internal override uint HResultDefault
			{
				get
				{
					return 0x80070002;
				}
			}

#if CONFIG_SERIALIZATION
	// Get ther serialization data for this object.
	public override void GetObjectData(SerializationInfo info,
									   StreamingContext context)
			{
				base.GetObjectData(info, context);
				info.AddValue("FileNotFound_FileName", fileName);
				info.AddValue("FileNotFound_FusionLog", fusionLog);
			}
#endif

}; // class FileNotFoundException

}; // namespace System.IO
