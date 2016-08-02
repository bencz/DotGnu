/*
 * XmlNodeChangedEventArgs.cs - Implementation of the
 *		"System.Xml.XmlNodeChangedEventArgs" class.
 *
 * Copyright (C) 2002 Southern Storm Software, Pty Ltd.
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

namespace System.Xml
{

using System;

#if ECMA_COMPAT
internal
#else
public
#endif
class XmlNodeChangedEventArgs
{
	// Internal state.
	private XmlNodeChangedAction action;
	private XmlNode node;
	private XmlNode oldParent;
	private XmlNode newParent;

	// Constructor.
	internal XmlNodeChangedEventArgs
				(XmlNodeChangedAction action, XmlNode node,
				 XmlNode oldParent, XmlNode newParent)
			{
				this.action = action;
				this.node = node;
				this.oldParent = oldParent;
				this.newParent = newParent;
			}

	// Properties.
	public XmlNodeChangedAction Action
			{
				get
				{
					return action;
				}
			}
	public XmlNode Node
			{
				get
				{
					return node;
				}
			}
	public XmlNode OldParent
			{
				get
				{
					return oldParent;
				}
			}
	public XmlNode NewParent
			{
				get
				{
					return newParent;
				}
			}

}; // class XmlNodeChangedEventArgs

}; // namespace System.Xml
