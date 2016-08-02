/*
 * Header.cs - Implementation of the
 *			"System.Runtime.Remoting.Messaging.Header" class.
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

namespace System.Runtime.Remoting.Messaging
{

#if CONFIG_SERIALIZATION

[Serializable]
public class Header
{
	// Accessible internal state.
	public String Name;
	public Object Value;
	public bool MustUnderstand;
	public String HeaderNamespace;

	// Constructor.
	public Header(String _Name, Object _Value)
			{
				this.Name = _Name;
				this.Value = _Value;
			}
	public Header(String _Name, Object _Value, bool _MustUnderstand)
			{
				this.Name = _Name;
				this.Value = _Value;
				this.MustUnderstand = _MustUnderstand;
			}
	public Header(String _Name, Object _Value, bool _MustUnderstand,
				  String _HeaderNamespace)
			{
				this.Name = _Name;
				this.Value = _Value;
				this.MustUnderstand = _MustUnderstand;
				this.HeaderNamespace = _HeaderNamespace;
			}

}; // class Header

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting.Messaging
