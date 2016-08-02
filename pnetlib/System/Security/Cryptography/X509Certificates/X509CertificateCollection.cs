/*
 * X509CertificateCollection.cs - Implementation of
 *	"System.Security.Cryptography.X509Certificates.X509CertificateCollection".
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

namespace System.Security.Cryptography.X509Certificates
{

#if CONFIG_X509_CERTIFICATES

using System.Collections;

[Serializable]
public class X509CertificateCollection : CollectionBase
{
	// Constructors.
	public X509CertificateCollection() {}
	public X509CertificateCollection(X509Certificate[] value)
			{
				AddRange(value);
			}
	public X509CertificateCollection(X509CertificateCollection value)
			{
				AddRange(value);
			}

	// Get or set a collection element.
	public X509Certificate this[int index]
			{
				get
				{
					return (X509Certificate)(((IList)this)[index]);
				}
				set
				{
					((IList)this)[index] = value;
				}
			}

	// Add a certificate to this collection.
	public int Add(X509Certificate value)
			{
				return ((IList)this).Add(value);
			}

	// Add a range of certificates to this collection.
	public void AddRange(X509Certificate[] value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(X509Certificate cert in value)
				{
					Add(cert);
				}
			}
	public void AddRange(X509CertificateCollection value)
			{
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				foreach(X509Certificate cert in value)
				{
					Add(cert);
				}
			}

	// Determine if this collection contains a particular certificate.
	public bool Contains(X509Certificate value)
			{
				return ((IList)this).Contains(value);
			}

	// Copy the contents of this collection to an array.
	public void CopyTo(X509Certificate[] array, int index)
			{
				((IList)this).CopyTo(array, index);
			}

	// Get a certificate enumerator for this collection.
	public new X509CertificateEnumerator GetEnumerator()
			{
				return new X509CertificateEnumerator(this);
			}

	// Get a hash code for this entire collection.
	public override int GetHashCode()
			{
				int hash = 0;
				IEnumerator e = ((IList)this).GetEnumerator();
				while(e.MoveNext())
				{
					hash ^= e.Current.GetHashCode();
				}
				return hash;
			}

	// Get the index of a specific certificate.
	public int IndexOf(X509Certificate value)
			{
				return ((IList)this).IndexOf(value);
			}

	// Insert a certificate into this collection
	public void Insert(int index, X509Certificate value)
			{
				((IList)this).Insert(index, value);
			}

	// Remove a certificate from this collection.
	public void Remove(X509Certificate value)
			{
				((IList)this).Remove(value);
			}

	// Certificate enumerator class.
	public class X509CertificateEnumerator : IEnumerator
	{
		// Internal state.
		private IEnumerator e;

		// Constructor.
		public X509CertificateEnumerator(X509CertificateCollection mappings)
				{
					e = ((IList)mappings).GetEnumerator();
				}

		// Get the current certificate.
		public X509Certificate Current
				{
					get
					{
						return (X509Certificate)(e.Current);
					}
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					return e.MoveNext();
				}
		public void Reset()
				{
					e.Reset();
				}
		Object IEnumerator.Current
				{
					get
					{
						return e.Current;
					}
				}

	}; // class X509CertificateEnumerator

}; // class X509CertificateCollection

#endif // CONFIG_X509_CERTIFICATES

}; // namespace System.Security.Cryptography.X509Certificates
