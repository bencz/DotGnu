/*
 * StringPrototype.cs - Prototype for JScript string objects.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.Text;
using Microsoft.JScript.Vsa;

public class StringPrototype : StringObject
{
	// Constructor.
	internal StringPrototype(ScriptObject prototype)
			: base(prototype, String.Empty)
			{
				// Add the builtin "String" properties to the prototype.
				EngineInstance inst = EngineInstance.GetEngineInstance(engine);
				Put("constructor", inst.GetStringConstructor());
				AddBuiltin(inst, "anchor");
				AddBuiltin(inst, "big");
				AddBuiltin(inst, "blink");
				AddBuiltin(inst, "bold");
				AddBuiltin(inst, "fixed");
				AddBuiltin(inst, "fontcolor");
				AddBuiltin(inst, "fontsize");
				AddBuiltin(inst, "charAt");
				AddBuiltin(inst, "charCodeAt");
				AddBuiltin(inst, "concat");
			}

	// Get the "String" class constructor.  Don't use this.
	public static StringConstructor constructor
			{
				get
				{
					return EngineInstance.Default.GetStringConstructor();
				}
			}

	// Get the character at a particular position in a string.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_charAt)]
	public static String charAt(Object thisob, double pos)
			{
				return new String(Convert.ToString(thisob)[(int)pos],1);
			}

	// Get the character code at a particular position in a string.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_charCodeAt)]
	public static Object charCodeAt(Object thisob, double pos)
			{
				return (int)(Convert.ToString(thisob))[(int)pos];
			}

	// Concatenate strings.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject |
				JSFunctionAttributeEnum.HasVarArgs,
				JSBuiltin.String_concat)]
	public static String concat(Object thisob, params Object[] args)
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(Convert.ToString(thisob));
				foreach(Object obj in args)
				{
					builder.Append(Convert.ToString(obj));
				}
				return builder.ToString();
			}

	// Build a HTML anchor tag.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_anchor)]
	public static String anchor(Object thisob, Object anchorName)
			{
				return "<A NAME=\"" + Convert.ToString(anchorName) +
					   "\">" + Convert.ToString(thisob) + "</A>";
			}

	// Build a HTML "big" tag.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_big)]
	public static String big(Object thisob)
			{
				return "<BIG>" + Convert.ToString(thisob) + "</BIG>";
			}

	// Build a HTML "blink" tag.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_blink)]
	public static String blink(Object thisob)
			{
				return "<BLINK>" + Convert.ToString(thisob) + "</BLINK>";
			}

	// Build a HTML "bold" tag.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_bold)]
	public static String bold(Object thisob)
			{
				return "<B>" + Convert.ToString(thisob) + "</B>";
			}

	// Build a HTML "tt" tag.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_fixed)]
	public static String @fixed(Object thisob)
			{
				return "<TT>" + Convert.ToString(thisob) + "</TT>";
			}

	// Build a HTML font color tag.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_fontcolor)]
	public static String fontcolor(Object thisob, Object colorName)
			{
				return "<FONT COLOR=\"" + Convert.ToString(colorName) +
					   "\">" + Convert.ToString(thisob) + "</FONT>";
			}

	// Build a HTML font size tag.
	[JSFunction(JSFunctionAttributeEnum.HasThisObject,
				JSBuiltin.String_fontsize)]
	public static String fontsize(Object thisob, Object fontSize)
			{
				return "<FONT SIZE=\"" + Convert.ToString(fontSize) +
					   "\">" + Convert.ToString(thisob) + "</FONT>";
			}

}; // class StringPrototype

// "Lenient" version of the above class which exports all of the
// prototype's properties to the user level.
public class LenientStringPrototype : StringPrototype
{
	// Accessible properties.
	public new Object constructor;
	public new Object charAt;
	public new Object charCodeAt;
	public new Object concat;
	public new Object anchor;
	public new Object big;
	public new Object blink;
	public new Object bold;
	public new Object @fixed;
	public new Object fontcolor;
	public new Object fontsize;

	// Constructor.
	internal LenientStringPrototype(ScriptObject prototype)
			: base(prototype)
			{
				constructor = Get("constructor");
				charAt = Get("charAt");
				charCodeAt = Get("charCodeAt");
				concat = Get("concat");
				anchor = Get("anchor");
				big = Get("big");
				blink = Get("blink");
				bold = Get("bold");
				@fixed = Get("fixed");
				fontcolor = Get("fontcolor");
				fontsize = Get("fontsize");
			}

}; // class LenientStringPrototype

}; // namespace Microsoft.JScript
