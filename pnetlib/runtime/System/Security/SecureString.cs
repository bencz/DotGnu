/*
 * SecureString.cs - Implementation of the
 *					"System.Security.SecureString" class.
 *
 * Copyright (C) 2010  Southern Storm Software, Pty Ltd.
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

namespace System.Security
{

#if CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

using System.Runtime.ConstrainedExecution;

public sealed class SecureString : CriticalFinalizerObject, IDisposable
{
	// Some constants used internally
	private const int maxLength = 65536;

	// Internal state.
	private String value;
	private bool isReadOnly;
	private bool isDisposed;

	// Constructors.
	private SecureString(String value)
	{
		this.value = value;
	}

	public SecureString()
	{
		value = String.Empty;
	}

	public unsafe SecureString(char *value, int length)
	{
		if(value != null)
		{
			if(length == 0)
			{
				this.value = String.Empty;
			}
			else if((length > 0) && (length <= maxLength))
			{
				this.value = new String(value, 0, length);
			}
			else
			{
				throw new ArgumentOutOfRangeException("length");
			}
		}
		else
		{
			throw new ArgumentNullException("value");
		}
	}

	// Get the length of the secure string.
	public int Length
	{
		get
		{
			return value.Length;
		}
	}

	// Append a character to the secure string.
	public void AppendChar(char c)
	{
		if(!isDisposed)
		{
			if(!isReadOnly)
			{
				if(value.Length < maxLength)
				{
					value += c;
				}
				else
				{
					throw new ArgumentOutOfRangeException();
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

	// Clear the value of the secure string.
	public void Clear()
	{
		if(!isDisposed)
		{
			if(!isReadOnly)
			{
				value = String.Empty;
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

	// Create a copy of this secure string.
	public SecureString Copy()
	{
		if(!isDisposed)
		{
			return new SecureString(this.value);
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

	// Dispose this secure string
	public void Dispose()
	{
		this.isDisposed = true;
	}

	public void InsertAt(int index, char c)
	{
		if(!isDisposed)
		{
			if(!isReadOnly)
			{
				if(value.Length < maxLength)
				{
					if(index == 0)
					{
						value = new String(c, 1) + this.value;
					}
					else if((index > 0) && (index < this.value.Length))
					{
						this.value = value.Insert(index, new String(c, 1));
					}
					else if(index == this.value.Length)
					{
						this.value += c;
					}
				}
				else
				{
					throw new ArgumentOutOfRangeException("index");
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

	public bool IsReadOnly()
	{
		if(!isDisposed)
		{
			return isReadOnly;
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

	public void MakeReadOnly()
	{
		if(!isDisposed)
		{
			this.isReadOnly = true;
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

	public void RemoveAt(int index)
	{
		if(!isDisposed)
		{
			if(!isReadOnly)
			{
				if((index >= 0) && (index < this.value.Length))
				{
					this.value = value.Remove(index, 1);
				}
				else
				{
					throw new ArgumentOutOfRangeException("index");
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

	public void SetAt(int index, char c)
	{
		if(!isDisposed)
		{
			if(!isReadOnly)
			{
				if((index >= 0) && (index < this.value.Length))
				{
					this.value.SetChar(index, c);
				}
				else
				{
					throw new ArgumentOutOfRangeException("index");
				}
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		else
		{
			throw new ObjectDisposedException("SecureString");
		}
	}

}; // class SecureString

#endif // CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

}; // namespace System.Security
