/*
 * WindowsIdentity.cs - Implementation of the
 *		"System.Security.Principal.WindowsIdentity" class.
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

namespace System.Security.Principal
{

#if CONFIG_POLICY_OBJECTS

using System.Runtime.Serialization;

// We don't use Windows identities in this implementation, but we
// still need to provide this class for API-compatibility.

[Serializable]
public class WindowsIdentity
	: IIdentity
#if CONFIG_SERIALIZATION
	, ISerializable, IDeserializationCallback
#endif
{
	// Internal state.
	private IntPtr userToken;
	private String name;
	private String type;
	private WindowsAccountType acctType;
	private bool isAuthenticated;

	// Constructor.
	public WindowsIdentity(IntPtr userToken)
			{
				this.userToken = userToken;
				this.name = String.Empty;
				this.type = "NTLM";
				this.acctType = WindowsAccountType.Normal;
				this.isAuthenticated = false;
			}
	public WindowsIdentity(IntPtr userToken, String type)
			{
				this.userToken = userToken;
				this.name = String.Empty;
				this.type = type;
				this.acctType = WindowsAccountType.Normal;
				this.isAuthenticated = false;
			}
	public WindowsIdentity(IntPtr userToken, String type,
						   WindowsAccountType acctType)
			{
				this.userToken = userToken;
				this.name = String.Empty;
				this.type = type;
				this.acctType = acctType;
				this.isAuthenticated = false;
			}
	public WindowsIdentity(IntPtr userToken, String type,
						   WindowsAccountType acctType,
						   bool isAuthenticated)
			{
				this.userToken = userToken;
				this.name = String.Empty;
				this.type = type;
				this.acctType = acctType;
				this.isAuthenticated = isAuthenticated;
			}
	public WindowsIdentity(String sUserPrincipalName)
			{
				this.userToken = IntPtr.Zero;
				this.name = sUserPrincipalName;
				this.type = "NTLM";
				this.acctType = WindowsAccountType.Normal;
				this.isAuthenticated = false;
			}
	public WindowsIdentity(String sUserPrincipalName, String type)
			{
				this.userToken = IntPtr.Zero;
				this.name = sUserPrincipalName;
				this.type = type;
				this.acctType = WindowsAccountType.Normal;
				this.isAuthenticated = false;
			}
#if CONFIG_SERIALIZATION
	public WindowsIdentity(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				userToken = (IntPtr)(info.GetValue
					("m_userToken", typeof(IntPtr)));
				name = info.GetString("m_name");
				type = info.GetString("m_type");
				acctType = (WindowsAccountType)(info.GetValue
					("m_acctType", typeof(WindowsAccountType)));
				isAuthenticated = info.GetBoolean("m_isAuthenticated");
			}
#endif

	// Destructor.
	~WindowsIdentity()
			{
				// Nothing to do here in this implementation.
			}

	// Get the type of authentication used.
	public virtual String AuthenticationType
			{
				get
				{
					return type;
				}
			}

	// Determine if this account is anonymous.
	public virtual bool IsAnonymous
			{
				get
				{
					return (acctType == WindowsAccountType.Anonymous);
				}
			}

	// Determine if we have been authenticated.
	public virtual bool IsAuthenticated
			{
				get
				{
					return isAuthenticated;
				}
			}

	// Determine if this account is guest.
	public virtual bool IsGuest
			{
				get
				{
					return (acctType == WindowsAccountType.Guest);
				}
			}

	// Determine if this account is system.
	public virtual bool IsSystem
			{
				get
				{
					return (acctType == WindowsAccountType.System);
				}
			}

	// Get the name associated with this identity.
	public virtual String Name
			{
				get
				{
					return name;
				}
			}

	// Get the Windows user account token.
	public virtual IntPtr Token
			{
				get
				{
					return userToken;
				}
			}

#if CONFIG_SERIALIZATION

	// Implement the ISerializable interface.
	void ISerializable.GetObjectData(SerializationInfo info,
									 StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("m_userToken", userToken, typeof(IntPtr));
				info.AddValue("m_name", name);
				info.AddValue("m_type", type);
				info.AddValue("m_acctType", acctType,
							  typeof(WindowsAccountType));
				info.AddValue("m_isAuthenticated", isAuthenticated);
			}

	// Implement the IDeserializationCallback interface.
	void IDeserializationCallback.OnDeserialization(Object sender)
			{
				// Nothing to do here in this implementation.
			}

#endif // CONFIG_SERIALIZATION

	// Get the anonymous Windows identity object.
	public static WindowsIdentity GetAnonymous()
			{
				return new WindowsIdentity
					(IntPtr.Zero, String.Empty, WindowsAccountType.Anonymous);
			}

	// Get the current Windows user identity.
	public static WindowsIdentity GetCurrent()
			{
				WindowsIdentity identity = new WindowsIdentity
					(IntPtr.Zero, String.Empty, WindowsAccountType.Normal);
				identity.name = Environment.UserName;
				return identity;
			}

	// Create an impersonation context.
	public virtual WindowsImpersonationContext Impersonate()
			{
				return new WindowsImpersonationContext();
			}
	public static WindowsImpersonationContext Impersonate(IntPtr userToken)
			{
				return (new WindowsIdentity(userToken)).Impersonate();
			}

}; // class WindowsIdentity

#endif // CONFIG_POLICY_OBJECTS

}; // namespace System.Security.Principal
