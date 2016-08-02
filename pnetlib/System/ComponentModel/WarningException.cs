/*
 * WarningException.cs - Implementation of the
 *		"System.ComponentModel.WarningException" class.
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

namespace System.ComponentModel
{

#if CONFIG_COMPONENT_MODEL

using System;

public class WarningException : SystemException
{
	// Internal state.
	private String helpUrl;
	private String helpTopic;

	// Constructors.
	public WarningException(String message)
			: base(message)
			{
				HResult = unchecked((int)0x80131501);
			}
	public WarningException(String message, String helpUrl)
			: base(message)
			{
				HResult = unchecked((int)0x80131501);
				this.helpUrl = helpUrl;
			}
	public WarningException(String message, String helpUrl, String helpTopic)
			: base(message)
			{
				HResult = unchecked((int)0x80131501);
				this.helpUrl = helpUrl;
				this.helpTopic = helpTopic;
				this.helpUrl = helpUrl;
			}

	// Get the exception properties.
	public String HelpTopic
			{
				get
				{
					return helpTopic;
				}
			}
	public String HelpUrl
			{
				get
				{
					return helpUrl;
				}
			}

}; // class WarningException

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
