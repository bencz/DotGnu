/*
 * ServicePoint.cs - Implementation of the "System.Net.ServicePoint" class.
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

using System.Security.Cryptography.X509Certificates;

public class ServicePoint
{
	// Internal state.
	internal Uri address;
#if CONFIG_X509_CERTIFICATES
	internal X509Certificate certificate;
	internal X509Certificate clientCertificate;
#endif
	internal int connectionLimit;
	internal String connectionName;
	internal int currentConnections;
	internal DateTime idleSince;
	internal int maxIdleTime;
	internal Version version;
	internal bool supportsPipelining;
	internal bool useNagleAlgorithm;
	internal bool expect100Continue;

	// Constructor.
	internal ServicePoint()
			{
				maxIdleTime = -1;
			}

	// Get this object's properties.
	public Uri Address
			{
				get
				{
					return address;
				}
			}
#if CONFIG_X509_CERTIFICATES
	public X509Certificate Certificate
			{
				get
				{
					return certificate;
				}
			}
	public X509Certificate ClientCertificate
			{
				get
				{
					return clientCertificate;
				}
			}
#endif
	public int ConnectionLimit
			{
				get
				{
					return connectionLimit;
				}
				set
				{
					if(value <= 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_PositiveNonZero"));
					}
					connectionLimit = value;
				}
			}
	public String ConnectionName
			{
				get
				{
					return connectionName;
				}
			}
	public int CurrentConnections
			{
				get
				{
					return currentConnections;
				}
			}
	public bool Expect100Continue
			{
				get
				{
					return expect100Continue;
				}
				set
				{
					expect100Continue = value;
				}
			}
	public DateTime IdleSince
			{
				get
				{
					return idleSince;
				}
			}
	public int MaxIdleTime
			{
				get
				{
					return maxIdleTime;
				}
				set
				{
					if(value < -1)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegOrNegOne"));
					}
					maxIdleTime = value;
				}
			}
	public virtual Version ProtocolVersion
			{
				get
				{
					return version;
				}
			}
	public bool SupportsPipelining
			{
				get
				{
					return supportsPipelining;
				}
			}
	public bool UseNagleAlgorithm
			{
				get
				{
					return useNagleAlgorithm;
				}
			}

	// Get the hash code for this object.
	public override int GetHashCode()
			{
				return base.GetHashCode();
			}
	
}; // class ServicePoint

}; // namespace System.Net
