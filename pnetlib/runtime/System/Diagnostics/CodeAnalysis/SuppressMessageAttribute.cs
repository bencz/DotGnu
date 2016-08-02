/*
 * SuppressMessageAttribute.cs - Implementation of the
 *			"System.Diagnostics.CodeAnalysis.SuppressMessageAttribute" class.
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

namespace System.Diagnostics.CodeAnalysis
{

#if CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

[AttributeUsage(AttributeTargets.All, Inherited=false, AllowMultiple=true)]
[Conditional("CODE_ANALYSIS")]
public sealed class SuppressMessageAttribute : Attribute
{
	// Internal state
	private String category;
	private String checkId;
	private String justification;
	private String messageId;
	private String scope;
	private String target;

	// Constructors.
	public SuppressMessageAttribute(String category, String checkId) : base()
	{
		this.category = category;
		this.checkId = checkId;
		this.justification = null;
		this.messageId = null;
		this.scope = null;
		this.target = null;
	}

	public String Category
	{
		get
		{
			return category;
		}
	}

	public String CheckId
	{
		get
		{
			return checkId;
		}
	}

	public String Justification
	{
		get
		{
			return justification;
		}
		set
		{
			justification = value;
		}
	}

	public String MessageId
	{
		get
		{
			return messageId;
		}
		set
		{
			messageId = value;
		}
	}

	public String Scope
	{
		get
		{
			return scope;
		}
		set
		{
			scope = value;
		}
	}

	public String Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}
}; // class SuppressMessageAttribute

#endif // CONFIG_FRAMEWORK_2_0 && !ECMA_COMPAT

}; // namespace System.Diagnostics.CodeAnalysis
