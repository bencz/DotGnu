/*
 * SecurityPermissionAttribute.cs - Implementation of the
 *			"System.Security.Permissions.SecurityPermissionAttribute" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Security.Permissions
{

// Note: this class sometimes inherits from "Attribute" because otherwise it
// confuses the Microsoft C# compiler when building with "/nostdlib".

[AttributeUsage(AttributeTargets.Assembly |
			 	AttributeTargets.Class |
			 	AttributeTargets.Struct |
			 	AttributeTargets.Constructor |
			 	AttributeTargets.Method,
			 	AllowMultiple=true, Inherited=false)]
#if __CSCC__ && CONFIG_PERMISSIONS
public sealed class SecurityPermissionAttribute : CodeAccessSecurityAttribute
#else
public sealed class SecurityPermissionAttribute : Attribute
#endif
{
	// Internal state.
	private SecurityPermissionFlag flags;

	// Constructor.
	public SecurityPermissionAttribute(SecurityAction action)
			//: base(action)
			{
				// Nothing to do here.
			}

#if __CSCC__ && CONFIG_PERMISSIONS
	// Create a permission object that corresponds to this attribute.
	public override IPermission CreatePermission()
			{
				if(Unrestricted)
				{
					return new SecurityPermission
						(PermissionState.Unrestricted);
				}
				else
				{
					return new SecurityPermission(flags);
				}
			}
#endif

	// Get or set the security permission flags.
	public SecurityPermissionFlag Flags
			{
				get
				{
					return flags;
				}
				set
				{
					flags = value;
				}
			}

	// This property is not specified by ECMA, but it must be present
	// or Microsoft's C# compiler throws an internal error.
	public bool SkipVerification
			{
				get
				{
					return ((flags & SecurityPermissionFlag.SkipVerification)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.SkipVerification;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.SkipVerification;
					}
				}
			}

#if __CSCC__ && !ECMA_COMPAT && CONFIG_PERMISSIONS

	// Non-ECMA properties.
	public bool Assertion
			{
				get
				{
					return ((flags & SecurityPermissionFlag.Assertion)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.Assertion;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.Assertion;
					}
				}
			}
	public bool ControlAppDomain
			{
				get
				{
					return ((flags & SecurityPermissionFlag.ControlAppDomain)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.ControlAppDomain;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.ControlAppDomain;
					}
				}
			}
	public bool ControlDomainPolicy
			{
				get
				{
					return ((flags & SecurityPermissionFlag.ControlDomainPolicy)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.ControlDomainPolicy;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.ControlDomainPolicy;
					}
				}
			}
	public bool ControlEvidence
			{
				get
				{
					return ((flags & SecurityPermissionFlag.ControlEvidence)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.ControlEvidence;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.ControlEvidence;
					}
				}
			}
	public bool ControlPolicy
			{
				get
				{
					return ((flags & SecurityPermissionFlag.ControlPolicy)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.ControlPolicy;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.ControlPolicy;
					}
				}
			}
	public bool ControlPrincipal
			{
				get
				{
					return ((flags & SecurityPermissionFlag.ControlPrincipal)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.ControlPrincipal;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.ControlPrincipal;
					}
				}
			}
	public bool ControlThread
			{
				get
				{
					return ((flags & SecurityPermissionFlag.ControlThread)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.ControlThread;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.ControlThread;
					}
				}
			}
	public bool Execution
			{
				get
				{
					return ((flags & SecurityPermissionFlag.Execution)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.Execution;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.Execution;
					}
				}
			}
	public bool Infrastructure
			{
				get
				{
					return ((flags & SecurityPermissionFlag.Infrastructure)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.Infrastructure;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.Infrastructure;
					}
				}
			}
	public bool RemotingConfiguration
			{
				get
				{
					return
						((flags & SecurityPermissionFlag.RemotingConfiguration)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.RemotingConfiguration;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.RemotingConfiguration;
					}
				}
			}
	public bool SerializationFormatter
			{
				get
				{
					return
						((flags & SecurityPermissionFlag.SerializationFormatter)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.SerializationFormatter;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.SerializationFormatter;
					}
				}
			}
	public bool UnmanagedCode
			{
				get
				{
					return ((flags & SecurityPermissionFlag.UnmanagedCode)
								!= 0);
				}
				set
				{
					if(value)
					{
						flags |= SecurityPermissionFlag.UnmanagedCode;
					}
					else
					{
						flags &= ~SecurityPermissionFlag.UnmanagedCode;
					}
				}
			}

#endif // !ECMA_COMPAT && CONFIG_PERMISSIONS

}; // class SecurityPermissionAttribute

}; // namespace System.Security.Permissions
