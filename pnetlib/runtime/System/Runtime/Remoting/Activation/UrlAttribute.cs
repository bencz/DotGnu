/*
 * UrlAttribute.cs - Implementation of the
 *			"System.Runtime.Remoting.Activation.UrlAttribute" class.
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

namespace System.Runtime.Remoting.Activation
{

#if CONFIG_REMOTING

using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Contexts;

public sealed class UrlAttribute : ContextAttribute
{
	// Internal state.
	private String callSiteUrl;

	// Constructor.
	public UrlAttribute(String callSiteURL)
			: base("callSiteURL")
			{
				if(callSiteURL == null)
				{
					throw new ArgumentNullException("callSiteURL");
				}
				this.callSiteUrl = callSiteURL;
			}

	// Get the URL value.
	public String UrlValue
			{
				get
				{
					return callSiteUrl;
				}
			}

	// Determine if two attributes are equal.
	public override bool Equals(Object obj)
			{
				UrlAttribute other = (obj as UrlAttribute);
				if(other != null)
				{
					if(callSiteUrl == other.callSiteUrl)
					{
						return base.Equals(obj);
					}
					else
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return callSiteUrl.GetHashCode();
			}

	// Get the properties for a new construction context.
	public override void GetPropertiesForNewContext
				(IConstructionCallMessage ctorMsg)
			{
				// Nothing to do here.
			}

	// Determine if a context is OK with respect to this attribute.
	public override bool IsContextOK
				(Context ctx, IConstructionCallMessage msg)
			{
				// Nothing to do here except say "no".
				return false;
			}

}; // class UrlAttribute

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Activation
