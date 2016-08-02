/*
 * QualifiedAce.cs - Implementation of the
 *			"System.Security.AccessControl.QualifiedAce" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Security.AccessControl
{

#if CONFIG_ACCESS_CONTROL

using System.Security.Principal;

public abstract class QualifiedAce : KnownAce
{
	// Internal state.
	private byte[] opaque;
	private AceQualifier aceQualifier;
	private bool isCallback;

	// Constructor.
	internal QualifiedAce(AceFlags aceFlags, AceType aceType, int accessMask,
						  SecurityIdentifier securityIdentifier,
						  byte[] opaque, AceQualifier aceQualifier,
						  bool isCallback)
			: base(aceFlags, aceType, accessMask, securityIdentifier)
			{
				this.opaque = opaque;
				this.aceQualifier = aceQualifier;
				this.isCallback = isCallback;
			}

	// Get the opaque blob.
	public byte[] GetOpaque()
			{
				return opaque;
			}

	// Set the opaque blob.
	public void SetOpaque(byte[] opaque)
			{
				this.opaque = opaque;
			}

	// Get this object's properties.
	public AceQualifier AceQualifier
			{
				get
				{
					return aceQualifier;
				}
			}
	public bool IsCallback
			{
				get
				{
					return isCallback;
				}
			}
	public int OpaqueLength
			{
				get
				{
					if(opaque == null)
					{
						return 0;
					}
					else
					{
						return opaque.Length;
					}
				}
			}

}; // class QualifiedAce

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
