/*
 * Authorization.cs - Implementation of the "System.Net.Authorization" class.
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

namespace System.Net
{

public class Authorization
{
	// Internal state.
	private String token;
	private bool finished;
	private String connectionGroupId;
	private String[] protectionRealm;

	// Constructors.
	public Authorization(String token) : this(token, true, null) {}
	public Authorization(String token, bool finished)
			: this(token, finished, null) {}
	public Authorization(String token, bool finished,
						 String connectionGroupId)
			{
				this.token = (token == String.Empty ? null : token);
				this.finished = finished;
				this.connectionGroupId =
					(connectionGroupId == String.Empty
						? null : connectionGroupId);
			}

	// Get this object's properties.
	public bool Complete
			{
				get
				{
					return finished;
				}
			}
	public String ConnectionGroupId
			{
				get
				{
					return connectionGroupId;
				}
			}
	public String Message
			{
				get
				{
					return token;
				}
			}
	public String[] ProtectionRealm
			{
				get
				{
					return protectionRealm;
				}
				set
				{
					if(value != null && value.Length == 0)
					{
						protectionRealm = null;
					}
					else
					{
						protectionRealm = value;
					}
				}
			}

}; // class Authorization

}; // namespace System.Net
