/*
 * LicenseManager.cs - Implementation of the
 *		"System.ComponentModel.ComponentModel.LicenseManager" class.
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
using System.Collections;
using System.Reflection;

public sealed class LicenseManager
{
	// Internal state.
	private static LicenseContext currentContext;
	private static Object contextLockedBy;
	private static Object ourLock;
	private static Hashtable providers;
	private static DefaultLicenseProvider defaultProvider;

	// Cannot instantiate this class.
	private LicenseManager() {}

	// Get or set the current license context.
	public static LicenseContext CurrentContext
			{
				get
				{
					lock(typeof(LicenseManager))
					{
						if(currentContext == null)
						{
							currentContext = new LicenseContext();
						}
						return currentContext;
					}
				}
				set
				{
					lock(typeof(LicenseManager))
					{
						if(currentContext != null)
						{
							throw new InvalidOperationException
								(S._("Invalid_LicenseContextChange"));
						}
						currentContext = value;
					}
				}
			}

	// Get the usage mode for the current license context.
	public static LicenseUsageMode UsageMode
			{
				get
				{
					return CurrentContext.UsageMode;
				}
			}

	// Create an instance of an object, within a particular license context.
	public static Object CreateWithContext
				(Type type, LicenseContext creationContext)
			{
				return CreateWithContext(type, creationContext, new Object [0]);
			}
	public static Object CreateWithContext
				(Type type, LicenseContext creationContext, Object[] args)
			{
				lock(typeof(LicenseManager))
				{
					// Temporarily switch to the new context during creation.
					LicenseContext savedContext = currentContext;
					currentContext = creationContext;
					try
					{
						// Make sure that we are the only context user.
						if(ourLock == null)
						{
							ourLock = new Object();
						}
						LockContext(ourLock);
						try
						{
							try
							{
								return Activator.CreateInstance(type, args);
							}
							catch(TargetInvocationException e)
							{
								// Re-throw the inner exception, if present.
								if(e.InnerException != null)
								{
									throw e.InnerException;
								}
								else
								{
									throw;
								}
							}
						}
						finally
						{
							UnlockContext(ourLock);
						}
					}
					finally
					{
						currentContext = savedContext;
					}
				}
			}

	// Determine if a type has a valid license.
	public static bool IsLicensed(Type type)
			{
				return IsValid(type);
			}

	// Get the license provider for a specific type.
	private static LicenseProvider GetProvider(Type type)
			{
				Type providerType;
				LicenseProvider provider;
				Object[] attrs;
				lock(typeof(LicenseManager))
				{
					// Get the cached license provider.
					if(providers == null)
					{
						providers = new Hashtable();
					}
					provider = (providers[type] as LicenseProvider);
					if(provider != null)
					{
						return provider;
					}

					// Check the type's "LicenseProvider" attribute.
					attrs = type.GetCustomAttributes
						(typeof(LicenseProviderAttribute), true);
					if(attrs != null && attrs.Length > 0)
					{
						providerType = ((LicenseProviderAttribute)(attrs[0]))
								.LicenseProvider;
						if(providerType != null)
						{
							provider = (LicenseProvider)
								(Activator.CreateInstance(providerType));
							providers[type] = provider;
							return provider;
						}
					}

					// No declared provider, so use the default provider.
					if(defaultProvider == null)
					{
						defaultProvider = new DefaultLicenseProvider();
					}
					providers[type] = defaultProvider;
					return defaultProvider;
				}
			}

	// Perform license validation for a type.
	private static License PerformValidation(Type type, Object instance)
			{
				LicenseProvider provider = GetProvider(type);
				return provider.GetLicense
					(CurrentContext, type, instance, false);
			}

	// Determine if a valid license can be granted for a type.
	public static bool IsValid(Type type)
			{
				License license = PerformValidation(type, null);
				if(license != null)
				{
					license.Dispose();
					return true;
				}
				else
				{
					return false;
				}
			}
	public static bool IsValid(Type type, Object instance, out License license)
			{
				license = PerformValidation(type, instance);
				return (license != null);
			}

	// Lock the license context associated with an object.
	public static void LockContext(Object contextUser)
			{
				lock(typeof(LicenseManager))
				{
					if(contextLockedBy == null)
					{
						contextLockedBy = contextUser;
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_LicenseContextLocked"));
					}
				}
			}

	// Unlock the license context associated with an object.
	public static void UnlockContext(Object contextUser)
			{
				lock(typeof(LicenseManager))
				{
					if(contextLockedBy == contextUser)
					{
						contextLockedBy = null;
					}
					else
					{
						throw new InvalidOperationException
							(S._("Invalid_LicenseContextNotLocked"));
					}
				}
			}

	// Validate a license for a type.
	public static void Validate(Type type)
			{
				if(!IsValid(type))
				{
					throw new LicenseException(type);
				}
			}
	public static License Validate(Type type, Object instance)
			{
				License license;
				if(!IsValid(type, instance, out license))
				{
					throw new LicenseException(type, instance);
				}
				return license;
			}

	// Default license provider for types that don't have their own.
	private sealed class DefaultLicenseProvider : LicenseProvider
	{
		// Get the license for a type.
		public override License GetLicense
					(LicenseContext context, Type type, Object instance,
			 		 bool allowExceptions)
				{
					return new DefaultLicense(type.FullName);
				}

	}; // class DefaultLicenseProvider

	// The default license class.
	private sealed class DefaultLicense : License
	{
		// Internal state.
		private String key;

		// Constructor.
		public DefaultLicense(String key)
				{
					this.key = key;
				}

		// Get the license key.
		public override String LicenseKey
				{
					get
					{
						return key;
					}
				}

		// Dispose of this license.
		public override void Dispose()
				{
					// Nothing to do here.
				}

	}; // class DefaultLicense

}; // class LicenseManager

#endif // CONFIG_COMPONENT_MODEL

}; // namespace System.ComponentModel
