/*
 * ServicePointManager.cs - Implementation of the
 *			"System.Net.ServicePointManager" class.
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

public class ServicePointManager
{
	// Default values.
	public const int DefaultNonPersistentConnectionLimit = 4;
	public const int DefaultPersistentConnectionLimit	 = 2;

	// Internal state.
#if CONFIG_X509_CERTIFICATES
	private static ICertificatePolicy certificatePolicy;
#endif
	private static bool checkCertificateRevocationList;
	private static int defaultConnectionLimit =
				DefaultPersistentConnectionLimit;
	private static int maxServicePointIdleTime = 900000;	// 900 seconds.
	private static int maxServicePoints;
	private static SecurityProtocolType securityProtocol =
				SecurityProtocolType.Ssl3;

	// Cannot instantiate this class.
	private ServicePointManager() {}

#if CONFIG_X509_CERTIFICATES

	// Get or set the certificate policy.
	public static ICertificatePolicy CertificatePolicy
			{
				get
				{
					return certificatePolicy;
				}
				set
				{
					certificatePolicy = value;
				}
			}

	// Get or set the revocation list check flag.
	public static bool CheckCertificateRevocationList
			{
				get
				{
					return checkCertificateRevocationList;
				}
				set
				{
					checkCertificateRevocationList = value;
				}
			}

#endif // CONFIG_X509_CERTIFICATES

	// Get or set the default connection limit.
	public static int DefaultConnectionLimit
			{
				get
				{
					return defaultConnectionLimit;
				}
				set
				{
					if(value <= 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_PositiveNonZero"));
					}
					defaultConnectionLimit = value;
				}
			}

	// Get or set the maximum service point idle time.
	public static int MaxServicePointIdleTime
			{
				get
				{
					return maxServicePointIdleTime;
				}
				set
				{
					if(value < -1)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegOrNegOne"));
					}
					maxServicePointIdleTime = value;
				}
			}

	// Get or set the maximum number of service points.
	public static int MaxServicePoints
			{
				get
				{
					return maxServicePoints;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException
							("value", S._("ArgRange_NonNegative"));
					}
					maxServicePoints = value;
				}
			}

	// Get or set the security protocol to use.
	public static SecurityProtocolType SecurityProtocol
			{
				get
				{
					return securityProtocol;
				}
				set
				{
					securityProtocol = value;
				}
			}

	// Find an existing service point.
	public static ServicePoint FindServicePoint(Uri address)
			{
				return FindServicePoint
					(address, GlobalProxySelection.GetEmptyWebProxy());
			}
	public static ServicePoint FindServicePoint
				(String uriString, IWebProxy proxy)
			{
				return FindServicePoint(new Uri(uriString), proxy);
			}
	[TODO]
	public static ServicePoint FindServicePoint(Uri address, IWebProxy proxy)
			{
				if(address == null)
				{
					throw new ArgumentNullException("address");
				}
				// TODO
				return null;
			}

}; // class ServicePointManager

}; // namespace System.Net
