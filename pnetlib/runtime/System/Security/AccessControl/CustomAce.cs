/*
 * CustomAce.cs - Implementation of the
 *			"System.Security.AccessControl.CustomAce" class.
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

public sealed class CustomAce : GenericAce
{
	// Internal state.
	private byte[] opaque;

	// Maximum length of the opaque blob for custom elements.
	public static readonly int MaxOpaqueLength = 0xFFFB;

	// Constructor.
	public CustomAce(AceType aceType, AceFlags aceFlags, byte[] opaque)
			: base(aceFlags, aceType)
			{
				this.opaque = opaque;
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

	// Get the binary form of this access control element.
	public override void GetBinaryForm(byte[] binaryForm, int offset)
			{
				if(opaque != null)
				{
					Array.Copy(opaque, 0, binaryForm, offset, opaque.Length);
				}
			}

	// Get the length of the binary form of this element.
	public override int BinaryLength
			{
				get
				{
					return OpaqueLength;
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

}; // class CustomAce

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
