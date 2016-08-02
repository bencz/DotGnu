/*
 * CompareInfo.cs - Implementation of the
 *		"System.Globalization.CompareInfo" class.
 *
 * Copyright (C) 2001, 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System.Globalization
{

using System.Text;
using System.Reflection;
using System.Runtime.Serialization;

public class CompareInfo : IDeserializationCallback
{
	// Culture that this comparison object is attached to.
	private int culture;

	// Global compare object for the invariant culture.
	private static CompareInfo invariantCompare;

	// Programmers cannot create instances of this class according
	// to the specification, even though it has virtual methods!
	// In fact, we _can_ inherit it through "_I18NCompareInfo",
	// but that is private to this implementation and should not be
	// used by application programmers.
	internal CompareInfo(int culture)
			{
				this.culture = culture;
			}

	// Get the invariant CompareInfo object.
	internal static CompareInfo InvariantCompareInfo
			{
				get
				{
					lock(typeof(CompareInfo))
					{
						if(invariantCompare == null)
						{
							invariantCompare = new CompareInfo(0x007F);
						}
						return invariantCompare;
					}
				}
			}

	// Get the comparison information for a specific culture.
	public static CompareInfo GetCompareInfo(int culture)
			{
				return (new CultureInfo(culture)).CompareInfo;
			}
	public static CompareInfo GetCompareInfo(String culture)
			{
				if(culture == null)
				{
					throw new ArgumentNullException("culture");
				}
			#if CONFIG_REFLECTION
				return (new CultureInfo(culture)).CompareInfo;
			#else
				return InvariantCompareInfo;
			#endif
			}
#if CONFIG_REFLECTION
	public static CompareInfo GetCompareInfo(int culture, Assembly assembly)
			{
				if(assembly == null)
				{
					throw new ArgumentNullException("assembly");
				}
				else if(assembly != typeof(Object).Module.Assembly)
				{
					throw new ArgumentException
						(_("Arg_MustBeCoreLib"), "assembly");
				}
				return GetCompareInfo(culture);
			}
	public static CompareInfo GetCompareInfo(String culture, Assembly assembly)
			{
				if(assembly == null)
				{
					throw new ArgumentNullException("assembly");
				}
				else if(assembly != typeof(Object).Module.Assembly)
				{
					throw new ArgumentException
						(_("Arg_MustBeCoreLib"), "assembly");
				}
				return GetCompareInfo(culture);
			}
#endif // CONFIG_REFLECTION

	// Get the identifier for this comparison object's culture.
	public int LCID
			{
				get
				{
					return culture;
				}
			}

	// Compare two strings.
	public virtual int Compare(String string1, String string2)
			{
				return DefaultCompare(string1, 0,
									  ((string1 != null) ? string1.Length : 0),
									  string2, 0,
									  ((string2 != null) ? string2.Length : 0),
									  CompareOptions.None);
			}
	public virtual int Compare(String string1, String string2,
							   CompareOptions options)
			{
				return DefaultCompare(string1, 0,
									  ((string1 != null) ? string1.Length : 0),
									  string2, 0,
									  ((string2 != null) ? string2.Length : 0),
									  options);
			}
	public virtual int Compare(String string1, int offset1,
							   String string2, int offset2)
			{
				return DefaultCompare(string1, offset1,
									  ((string1 != null) ?
									  		(string1.Length - offset1) : 0),
									  string2, offset2,
									  ((string2 != null) ?
									  		(string2.Length - offset2) : 0),
									  CompareOptions.None);
			}
	public virtual int Compare(String string1, int offset1,
							   String string2, int offset2,
							   CompareOptions options)
			{
				return DefaultCompare(string1, offset1,
									  ((string1 != null) ?
									  		(string1.Length - offset1) : 0),
									  string2, offset2,
									  ((string2 != null) ?
									  		(string2.Length - offset2) : 0),
									  options);
			}
	public virtual int Compare(String string1, int offset1, int length1,
							   String string2, int offset2, int length2)
			{
				return DefaultCompare(string1, offset1, length1,
									  string2, offset2, length2,
									  CompareOptions.None);
			}
	public virtual int Compare(String string1, int offset1, int length1,
							   String string2, int offset2, int length2,
							   CompareOptions options)
			{
				return DefaultCompare(string1, offset1, length1,
									  string2, offset2, length2, options);
			}

	// Default "Compare" implementation that uses I18N to do the work.
	private int DefaultCompare(String string1, int offset1, int length1,
							   String string2, int offset2, int length2,
							   CompareOptions options)
			{
				if(offset1 < 0)
				{
					throw new ArgumentOutOfRangeException
						("offset1", _("ArgRange_StringIndex"));
				}
				if(offset2 < 0)
				{
					throw new ArgumentOutOfRangeException
						("offset2", _("ArgRange_StringIndex"));
				}
				if(length1 < 0)
				{
					throw new ArgumentOutOfRangeException
						("length1", _("ArgRange_StringRange"));
				}
				if(length2 < 0)
				{
					throw new ArgumentOutOfRangeException
						("length2", _("ArgRange_StringRange"));
				}
				if(string1 == null)
				{
					// Index and length must be 0
					if(offset1 != 0)
					{
						throw new ArgumentOutOfRangeException
							("offset1", _("ArgRange_StringIndex"));
					}
					if(length1 != 0)
					{
						throw new ArgumentOutOfRangeException
							("length1", _("ArgRange_StringRange"));
					}
				}
				else
				{
					if(length1 == 0)
					{
						// Offset must not be greater than length
						if(offset1 > string1.Length)
						{
							throw new ArgumentOutOfRangeException
								("offset1", _("ArgRange_StringIndex"));
						}
					}
					else
					{
						if(offset1 >= string1.Length)
						{
							throw new ArgumentOutOfRangeException
								("offset1", _("ArgRange_StringIndex"));
						}
						if(length1 > (string1.Length - offset1))
						{
							throw new ArgumentOutOfRangeException
								("length1", _("ArgRange_StringRange"));
						}
					}
				}
				if(string2 == null)
				{
					// Index and length must be 0
					if(offset2 != 0)
					{
						throw new ArgumentOutOfRangeException
							("offset1", _("ArgRange_StringIndex"));
					}
					if(length2 != 0)
					{
						throw new ArgumentOutOfRangeException
							("length1", _("ArgRange_StringRange"));
					}
				}
				else
				{
					if(length2 == 0)
					{
						// Offset must not be greater than length
						if(offset2 > string2.Length)
						{
							throw new ArgumentOutOfRangeException
								("offset2", _("ArgRange_StringIndex"));
						}
					}
					else
					{
						if(offset2 >= string2.Length)
						{
							throw new ArgumentOutOfRangeException
								("offset2", _("ArgRange_StringIndex"));
						}
						if(length2 > (string2.Length - offset2))
						{
							throw new ArgumentOutOfRangeException
								("length2", _("ArgRange_StringRange"));
						}
					}
				}
			#if CONFIG_REFLECTION
				if(this is _I18NCompareInfo)
				{
					// Use the I18N-supplied comparison routine.
					return ((_I18NCompareInfo)this).CompareImpl
						(string1, offset1, length1,
						 string2, offset2, length2, options);
				}
				else
			#endif
				{
					// Use the invariant comparison method in the engine.
					return String.CompareInternal
						(string1, offset1, length1,
						 string2, offset2, length2,
						 ((options & CompareOptions.IgnoreCase) != 0));
				}
			}

	// Determine if two CompareInfo objects are equal.
	public override bool Equals(Object obj)
			{
				CompareInfo other = (obj as CompareInfo);
				if(other != null)
				{
					return (LCID == other.LCID);
				}
				else
				{
					return false;
				}
			}

	// Get a hash code for this object.
	public override int GetHashCode()
			{
				return LCID;
			}

#if !ECMA_COMPAT

	// Get the sort key for a string.
	public virtual SortKey GetSortKey(String source)
			{
				return GetSortKey(source, CompareOptions.None);
			}
	public virtual SortKey GetSortKey(String source, CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(this is _I18NCompareInfo)
				{
					// Ask the subclass for the raw sort key data.
					return new SortKey(((_I18NCompareInfo)this).GetSortKeyImpl
								(source, options), source);
				}
				else
				{
					// Return the invariant sort key information.
					if((options & CompareOptions.IgnoreCase) != 0)
					{
						return new SortKey
							(Encoding.UTF8.GetBytes
								(CultureInfo.InvariantCulture.TextInfo
									.ToLower(source)), source);
					}
					else
					{
						return new SortKey
							(Encoding.UTF8.GetBytes(source), source);
					}
				}
			}

#endif // !ECMA_COMPAT

	// Search for a specific character in a string.
	public virtual int IndexOf(String source, char value)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, 0, source.Length,
							   CompareOptions.None);
			}
	public virtual int IndexOf(String source, char value,
							   CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, 0, source.Length, options);
			}
	public virtual int IndexOf(String source, char value, int startIndex)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, startIndex,
							   source.Length - startIndex,
							   CompareOptions.None);
			}
	public virtual int IndexOf(String source, char value, int startIndex,
							   CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, startIndex,
							   source.Length - startIndex, options);
			}
	public virtual int IndexOf(String source, char value,
							   int startIndex, int count)
			{
				return IndexOf(source, value, startIndex, count,
							   CompareOptions.None);
			}
	public virtual int IndexOf(String source, char value,
							   int startIndex, int count,
							   CompareOptions options)
			{
				// This may be overridden in subclasses with a more
				// efficient implementation if desired.
				return IndexOf(source, new String(value, 1),
							   startIndex, count, options);
			}

	// Search for a specific substring in a string.
	public virtual int IndexOf(String source, String value)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, 0, source.Length,
							   CompareOptions.None);
			}
	public virtual int IndexOf(String source, String value,
							   CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, 0, source.Length, options);
			}
	public virtual int IndexOf(String source, String value, int startIndex)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, startIndex,
							   source.Length - startIndex,
							   CompareOptions.None);
			}
	public virtual int IndexOf(String source, String value, int startIndex,
							   CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return IndexOf(source, value, startIndex,
							   source.Length - startIndex, options);
			}
	public virtual int IndexOf(String source, String value,
							   int startIndex, int count)
			{
				return IndexOf(source, value, startIndex, count,
							   CompareOptions.None);
			}
	public virtual int IndexOf(String source, String value,
							   int startIndex, int count,
							   CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_StringIndex"));
				}
				if(count < 0 || (source.Length - startIndex) < count)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_StringRange"));
				}
				int vlen = value.Length;
				while(count >= vlen)
				{
					if(Compare(source, startIndex, vlen,
							   value, 0, vlen, options) == 0)
					{
						return startIndex;
					}
					++startIndex;
					--count;
				}
				return -1;
			}

	// Determine if one string is a prefix of another.
	public virtual bool IsPrefix(String source, String prefix)
			{
				return IsPrefix(source, prefix, CompareOptions.None);
			}
	public virtual bool IsPrefix(String source, String prefix,
								 CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(prefix == null)
				{
					throw new ArgumentNullException("prefix");
				}
				if(source.Length < prefix.Length)
				{
					return false;
				}
				else
				{
					return (Compare(source, 0, prefix.Length, prefix, 0,
								    prefix.Length, options) == 0);
				}
			}

	// Determine if one string is a suffix of another.
	public virtual bool IsSuffix(String source, String suffix)
			{
				return IsSuffix(source, suffix, CompareOptions.None);
			}
	public virtual bool IsSuffix(String source, String suffix,
								 CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(suffix == null)
				{
					throw new ArgumentNullException("suffix");
				}
				if(source.Length < suffix.Length)
				{
					return false;
				}
				else
				{
					return (Compare(source, source.Length - suffix.Length,
								    suffix.Length, suffix, 0,
									suffix.Length, options) == 0);
				}
			}

	// Search backwards for a specific character in a string.
	public virtual int LastIndexOf(String source, char value)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, 0, source.Length,
							       CompareOptions.None);
			}
	public virtual int LastIndexOf(String source, char value,
							       CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, 0, source.Length, options);
			}
	public virtual int LastIndexOf(String source, char value, int startIndex)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, startIndex,
							       source.Length - startIndex,
							       CompareOptions.None);
			}
	public virtual int LastIndexOf(String source, char value, int startIndex,
							       CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, startIndex,
							       source.Length - startIndex, options);
			}
	public virtual int LastIndexOf(String source, char value,
							       int startIndex, int count)
			{
				return LastIndexOf(source, value, startIndex, count,
							       CompareOptions.None);
			}
	public virtual int LastIndexOf(String source, char value,
							       int startIndex, int count,
							       CompareOptions options)
			{
				// This may be overridden in subclasses with a more
				// efficient implementation if desired.
				return LastIndexOf(source, new String(value, 1),
							       startIndex, count, options);
			}

	// Search backwards for a specific substring in a string.
	public virtual int LastIndexOf(String source, String value)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, 0, source.Length,
							       CompareOptions.None);
			}
	public virtual int LastIndexOf(String source, String value,
							       CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, 0, source.Length, options);
			}
	public virtual int LastIndexOf(String source, String value, int startIndex)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, startIndex,
							       source.Length - startIndex,
							       CompareOptions.None);
			}
	public virtual int LastIndexOf(String source, String value, int startIndex,
							   CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				return LastIndexOf(source, value, startIndex,
							       source.Length - startIndex, options);
			}
	public virtual int LastIndexOf(String source, String value,
							       int startIndex, int count)
			{
				return LastIndexOf(source, value, startIndex, count,
							       CompareOptions.None);
			}
	public virtual int LastIndexOf(String source, String value,
							       int startIndex, int count,
							       CompareOptions options)
			{
				if(source == null)
				{
					throw new ArgumentNullException("source");
				}
				if(value == null)
				{
					throw new ArgumentNullException("value");
				}
				if(startIndex < 0)
				{
					throw new ArgumentOutOfRangeException
						("startIndex", _("ArgRange_StringIndex"));
				}
				if(count < 0 || (startIndex - count) < -1)
				{
					throw new ArgumentOutOfRangeException
						("count", _("ArgRange_StringRange"));
				}
				int vlen = value.Length;
				if(vlen == 0)
				{
					return 0;
				}
				while(count >= vlen)
				{
					if(Compare(source, startIndex - vlen + 1, vlen,
					           value, 0, vlen, options) == 0)
					{
						return startIndex - vlen + 1;
					}
					--startIndex;
					--count;
				}
				return -1;
			}

	// Implement IDeserializationCallback.
	void IDeserializationCallback.OnDeserialization(Object sender)
			{
				// Nothing to do here.
			}

	// Convert this object into a string.
	public override String ToString()
			{
				return "[CompareInfo:" + culture.ToString("x") + "]";
			}

}; // class CompareInfo

}; // namespace System.Globalization
