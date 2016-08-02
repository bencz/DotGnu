/*
 * GenericAce.cs - Implementation of the
 *			"System.Security.AccessControl.GenericAce" class.
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

public abstract class GenericAce
{
	// Internal state.
	private AceFlags aceFlags;
	private AceType aceType;

	// Constructor.
	internal GenericAce(AceFlags aceFlags, AceType aceType)
			{
				this.aceFlags = aceFlags;
				this.aceType = aceType;
			}

	// Make a copy of this access control element.
	public GenericAce Copy()
			{
				byte[] binaryForm = new byte [BinaryLength];
				GetBinaryForm(binaryForm, 0);
				return CreateFromBinaryForm(binaryForm, 0);
			}

	// Create an access control element from its binary form.
	[TODO]
	public static GenericAce CreateFromBinaryForm
				(byte[] binaryForm, int offset)
			{
				// TODO
				return null;
			}

	// Get the binary form of this access control element.
	public abstract void GetBinaryForm(byte[] binaryForm, int offset);

	// Get the length of the binary form of this element.
	public abstract int BinaryLength { get; }

	// Get or set this object's access control properties.
	public AceFlags AceFlags
			{
				get
				{
					return aceFlags;
				}
				set
				{
					aceFlags = value;
				}
			}
	public AceType AceType
			{
				get
				{
					return aceType;
				}
			}
	public AuditFlags AuditFlags
			{
				get
				{
					AuditFlags flags = AuditFlags.None;
					if((aceFlags & AceFlags.SuccessfulAccess) != 0)
					{
						flags |= AuditFlags.Success;
					}
					if((aceFlags & AceFlags.FailedAccess) != 0)
					{
						flags |= AuditFlags.Failure;
					}
					return flags;
				}
			}
	public InheritanceFlags InheritanceFlags
			{
				get
				{
					InheritanceFlags flags = InheritanceFlags.None;
					if((aceFlags & AceFlags.ContainerInherit) != 0)
					{
						flags |= InheritanceFlags.ContainerInherit;
					}
					if((aceFlags & AceFlags.ObjectInherit) != 0)
					{
						flags |= InheritanceFlags.ObjectInherit;
					}
					return flags;
				}
			}
	public bool IsInherited
			{
				get
				{
					return ((aceFlags & AceFlags.Inherited) != 0);
				}
			}
	public PropagationFlags PropagationFlags
			{
				get
				{
					PropagationFlags flags = PropagationFlags.None;
					if((aceFlags & AceFlags.NoPropagateInherit) != 0)
					{
						flags |= PropagationFlags.NoPropagateInherit;
					}
					if((aceFlags & AceFlags.InheritOnly) != 0)
					{
						flags |= PropagationFlags.InheritOnly;
					}
					return flags;
				}
			}

}; // class GenericAce

#endif // CONFIG_ACCESS_CONTROL

}; // namespace System.Security.AccessControl
