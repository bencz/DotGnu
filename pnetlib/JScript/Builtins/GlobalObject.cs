/*
 * GlobalObject.cs - Implementation of the global JScript object.
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
using System.Reflection;
using System.Globalization;
using Microsoft.JScript.Vsa;

public class GlobalObject
{
	// Actual storage for the global object.
	internal VsaEngine engine;
	internal JSObject globalObject;

	// Constructor.
	internal GlobalObject(VsaEngine engine)
			{
				// Record the engine for later.
				this.engine = engine;

				// Create the actual storage value, with no prototype.
				globalObject = new JSObject(null, engine);

				// Get the instance object for the engine.
				EngineInstance inst = EngineInstance.GetEngineInstance(engine);

				// Add all of the properties to the global object.
				AddBuiltin(inst, "CollectGarbage");
				AddBuiltin(inst, "decodeURI");
				AddBuiltin(inst, "decodeURIComponent");
				AddBuiltin(inst, "encodeURI");
				AddBuiltin(inst, "encodeURIComponent");
				AddBuiltin(inst, "escape");
				AddBuiltin(inst, "eval");
				AddBuiltin(inst, "GetObject");
				AddBuiltin(inst, "isFinite");
				AddBuiltin(inst, "isNaN");
				AddBuiltin(inst, "parseFloat");
				AddBuiltin(inst, "parseInt");
				AddBuiltin(inst, "ScriptEngine");
				AddBuiltin(inst, "ScriptEngineBuildVersion");
				AddBuiltin(inst, "ScriptEngineMajorVersion");
				AddBuiltin(inst, "ScriptEngineMinorVersion");
				AddBuiltin(inst, "unescape");
				globalObject.Put("Infinity", Double.PositiveInfinity,
								 PropertyAttributes.DontEnum |
								 PropertyAttributes.DontDelete);
				globalObject.Put("NaN", Double.NaN,
								 PropertyAttributes.DontEnum |
								 PropertyAttributes.DontDelete);
				globalObject.Put("undefined", null,
								 PropertyAttributes.DontEnum |
								 PropertyAttributes.DontDelete);
#if false
				AddProperty("ActiveXObject", ActiveXObject);
#endif
				AddProperty("Array", Array);
#if false
				AddProperty("Boolean", Boolean);
				AddProperty("Date", Date);
				AddProperty("Enumerator", Enumerator);
				AddProperty("Error", Error);
				AddProperty("EvalError", EvalError);
#endif
				AddProperty("Function", Function);
				AddProperty("Math", Math);
				AddProperty("Number", Number);
				AddProperty("Object", Object);
#if false
				AddProperty("RangeError", RangeError);
				AddProperty("ReferenceError", ReferenceError);
				AddProperty("RegExp", RegExp);
#endif
				AddProperty("String", String);
#if false
				AddProperty("SyntaxError", SyntaxError);
				AddProperty("TypeError", TypeError);
				AddProperty("URIError", URIError);
				AddProperty("VBArray", VBArray);
#endif
			}

	// Add a builtin function to the global object.
	private void AddBuiltin(EngineInstance inst, string name)
			{
				MethodInfo method = typeof(GlobalObject).GetMethod(name);
				globalObject.Put(name, new BuiltinFunction
					(inst.GetFunctionPrototype(), name, method),
					PropertyAttributes.None);
			}

	// Add a property to the global object.
	private void AddProperty(string name, object value)
			{
				globalObject.Put(name, value, PropertyAttributes.None);
			}

#if false
	// Return various type constructor objects.  Do not use these,
	// as they aren't reentrant-safe.
	public static ActiveXObjectConstructor ActiveXObject
			{
				get
				{
					return ActiveXObjectConstructor.constructor;
				}
			}
#endif
	public static ArrayConstructor Array
			{
				get
				{
					return EngineInstance.Default.GetArrayConstructor();
				}
			}
#if false
	public static BooleanConstructor Boolean
			{
				get
				{
					return BooleanConstructor.constructor;
				}
			}
	public static DateConstructor Date
			{
				get
				{
					return DateConstructor.constructor;
				}
			}
	public static EnumeratorConstructor Enumerator
			{
				get
				{
					return EnumeratorConstructor.constructor;
				}
			}
	public static ErrorConstructor Error
			{
				get
				{
					return ErrorConstructor.constructor;
				}
			}
	public static ErrorConstructor EvalError
			{
				get
				{
					return ErrorConstructor.evalError;
				}
			}
#endif
	public static FunctionConstructor Function
			{
				get
				{
					return EngineInstance.Default.GetFunctionConstructor();
				}
			}
	public static MathObject Math
			{
				get
				{
					return new MathObject(EngineInstance.Default.GetObjectPrototype());
				}
			}
	public static NumberConstructor Number
			{
				get
				{
					return EngineInstance.Default.GetNumberConstructor();
				}
			}
	public static ObjectConstructor Object
			{
				get
				{
					return EngineInstance.Default.GetObjectConstructor();
				}
			}
#if false
	public static ErrorConstructor RangeError
			{
				get
				{
					return ErrorConstructor.rangeError;
				}
			}
	public static ErrorConstructor ReferenceError
			{
				get
				{
					return ErrorConstructor.referenceError;
				}
			}
	public static RegExpConstructor RegExp
			{
				get
				{
					return RegExpConstructor.constructor;
				}
			}
#endif
	public static StringConstructor String
			{
				get
				{
					return EngineInstance.Default.GetStringConstructor();
				}
			}
#if false
	public static ErrorConstructor SyntaxError
			{
				get
				{
					return ErrorConstructor.syntaxError;
				}
			}
	public static ErrorConstructor TypeError
			{
				get
				{
					return ErrorConstructor.typeError;
				}
			}
	public static ErrorConstructor URIError
			{
				get
				{
					return ErrorConstructor.uriError;
				}
			}
	public static VBArrayConstructor VBArray
			{
				get
				{
					return VBArrayConstructor.constructor;
				}
			}
#endif

	// Return various system types.
	public static Type boolean
			{
				get
				{
					return typeof(bool);
				}
			}
	public static Type @byte
			{
				get
				{
					return typeof(Byte);
				}
			}
	public static Type @sbyte
			{
				get
				{
					return typeof(SByte);
				}
			}
	public static Type @char
			{
				get
				{
					return typeof(Char);
				}
			}
	public static Type @short
			{
				get
				{
					return typeof(Int16);
				}
			}
	public static Type @ushort
			{
				get
				{
					return typeof(UInt16);
				}
			}
	public static Type @int
			{
				get
				{
					return typeof(Int32);
				}
			}
	public static Type @uint
			{
				get
				{
					return typeof(UInt32);
				}
			}
	public static Type @long
			{
				get
				{
					return typeof(Int64);
				}
			}
	public static Type @ulong
			{
				get
				{
					return typeof(UInt64);
				}
			}
	public static Type @float
			{
				get
				{
					return typeof(Single);
				}
			}
	public static Type @double
			{
				get
				{
					return typeof(Double);
				}
			}
	public static Type @decimal
			{
				get
				{
					return typeof(Decimal);
				}
			}
	public static Type @void
			{
				get
				{
					return typeof(void);
				}
			}

	// Perform garbage collection.
	[JSFunction(0, JSBuiltin.Global_CollectGarbage)]
	public static void CollectGarbage()
			{
			#if !ECMA_COMPAT
				GC.Collect();
			#endif
			}

	// Determine if a character is reserved within URI's.
	private static bool IsURIReserved(char ch)
			{
				switch (ch)
				{
					case ';': case '/': case '?': case ':': case '@': case '&':
					case '=': case '+': case '$': case ',': case '#':
						return true;
				}
				return false;
			}

	// Parse the rest of a UTF-8 sequence within an encoded URI.
	private static int ParseRestOfUTF8(int utf8, string uri, ref int _index)
			{
				int index = _index;
				int size;
				int utf8Char;
				if((utf8 & 0xE0) == 0xC0)
				{
					size = 1;
					utf8 &= 0x1F;
				}
				else if((utf8 & 0xF0) == 0xE0)
				{
					size = 2;
					utf8 &= 0x0F;
				}
				else if((utf8 & 0xF8) == 0xF0)
				{
					size = 3;
					utf8 &= 0x07;
				}
				else if((utf8 & 0xFC) == 0xF8)
				{
					size = 4;
					utf8 &= 0x03;
				}
				else
				{
					// Invalid UTF-8 start character.
					throw new JScriptException(JSError.URIDecodeError);
				}
				while(size > 0)
				{
					if((index + 2) >= uri.Length || uri[index] != '%' ||
					   !JSScanner.IsHexDigit(uri[index + 1]) ||
					   !JSScanner.IsHexDigit(uri[index + 2]))
					{
						throw new JScriptException(JSError.URIDecodeError);
					}
					utf8Char = (JSScanner.FromHex(uri[index + 1]) << 4) |
							    JSScanner.FromHex(uri[index + 2]);
					if((utf8Char & 0xC0) != 0x80)
					{
						// Invalid UTF-8 component character.
						throw new JScriptException(JSError.URIDecodeError);
					}
					index += 3;
					utf8 = (utf8 << 6) | (utf8Char & 0x3F);
					--size;
				}
				_index = index;
				if(utf8 < 0)
				{
					// Invalid UTF-8 character.
					throw new JScriptException(JSError.URIDecodeError);
				}
				return utf8;
			}

	// Internal URI decoding logic.
	private static string InternalDecode(string uri, bool reserved)
			{
				StringBuilder builder = new StringBuilder(uri.Length);
				int index = 0;
				char ch;
				int utf8Char;
				int start;
				while(index < uri.Length)
				{
					ch = uri[index++];
					if(ch != '%')
					{
						builder.Append(ch);
					}
					else if((index + 1) >= uri.Length ||
					        !JSScanner.IsHexDigit(uri[index]) ||
							!JSScanner.IsHexDigit(uri[index + 1]))
					{
						// Invalid hexadecimal sequence.
						throw new JScriptException(JSError.URIDecodeError);
					}
					else
					{
						start = index - 1;
						utf8Char = (JSScanner.FromHex(uri[index]) << 4) |
								    JSScanner.FromHex(uri[index + 1]);
						index += 2;
						if(utf8Char >= 0x80)
						{
							// Parse the rest of the UTF-8 sequence.
							utf8Char = ParseRestOfUTF8
								(utf8Char, uri, ref index);
						}
						if(utf8Char < 0x10000)
						{
							// Single-character.
							if(reserved && IsURIReserved((char)utf8Char))
							{
								builder.Append(uri, start, index - start);
							}
							else
							{
								builder.Append((char)utf8Char);
							}
						}
						else if(utf8Char < 0x110000)
						{
							// Surrogate pair.
							utf8Char -= 0x10000;
							builder.Append((char)((utf8Char >> 10) + 0xD800));
							builder.Append((char)((utf8Char & 0x3FF) + 0xDC00));
						}
						else
						{
							// UTF-8 character is out of range.
							throw new JScriptException(JSError.URIDecodeError);
						}
					}
				}
				return builder.ToString();
			}

	// Decode a URI.
	[JSFunction(0, JSBuiltin.Global_decodeURI)]
	public static string decodeURI(object encodedURI)
			{
				return InternalDecode(Convert.ToString(encodedURI), true);
			}

	// Decode a URI component.
	[JSFunction(0, JSBuiltin.Global_decodeURIComponent)]
	public static string decodeURIComponent(object encodedURI)
			{
				return InternalDecode(Convert.ToString(encodedURI), false);
			}

	// Hexadecimal characters for URI encoding.
	private const string hex = "0123456789ABCDEF";

	// Append a hex sequence to a string builder.
	private static void AppendHex(StringBuilder builder, int value)
			{
				builder.Append('%');
				builder.Append(hex[(value >> 4) & 0x0F]);
				builder.Append(hex[value & 0x0F]);
			}

	// Internal URI encoding logic.
	private static string InternalEncode(string uri, bool reserved)
			{
				StringBuilder builder = new StringBuilder(uri.Length);
				int index = 0;
				char ch;
				int value;
				while(index < uri.Length)
				{
					ch = uri[index++];
					if((ch >= 'A' && ch <= 'Z') ||
					   (ch >= 'a' && ch <= 'z') ||
					   (ch >= '0' && ch <= '9') ||
					   ch == '-' || ch == '_' || ch == '.' ||
					   ch == '!' || ch == '~' || ch == '*' ||
					   ch == '\'' || ch == '(' || ch == ')')
					{
						builder.Append(ch);
					}
					else if(reserved &&
							(ch == ';' || ch == '/' || ch == '?' ||
							 ch == ':' || ch == '@' || ch == '&' ||
							 ch == '=' || ch == '+' || ch == '$' ||
							 ch == ',' || ch == '#'))
					{
						builder.Append(ch);
					}
					else if(ch < 0x80)
					{
						AppendHex(builder, ch);
					}
					else if(ch < (1 << 11))
					{
						AppendHex(builder, (ch >> 6) | 0xC0);
						AppendHex(builder, (ch & 0x3F) | 0x80);
					}
					else if(ch >= 0xD800 && ch <= 0xDBFF)
					{
						if(index >= uri.Length)
						{
							throw new JScriptException(JSError.URIEncodeError);
						}
						value = (ch - 0xD800) << 10;
						ch = uri[index++];
						if(ch < 0xDC00 || ch > 0xDFFF)
						{
							throw new JScriptException(JSError.URIEncodeError);
						}
						value += (ch - 0xDC00) + 0x10000;
						AppendHex(builder, (ch >> 18) | 0xF0);
						AppendHex(builder, ((ch >> 12) & 0x3F) | 0x80);
						AppendHex(builder, ((ch >> 6) & 0x3F) | 0x80);
						AppendHex(builder, (ch & 0x3F) | 0x80);
					}
					else if(ch >= 0xDC00 && ch <= 0xDFFF)
					{
						throw new JScriptException(JSError.URIEncodeError);
					}
					else
					{
						AppendHex(builder, (ch >> 12) | 0xE0);
						AppendHex(builder, ((ch >> 6) & 0x3F) | 0x80);
						AppendHex(builder, (ch & 0x3F) | 0x80);
					}
				}
				return builder.ToString();
			}

	// Encode a URI.
	[JSFunction(0, JSBuiltin.Global_encodeURI)]
	public static string encodeURI(object uri)
			{
				return InternalEncode(Convert.ToString(uri), true);
			}

	// Encode a URI component.
	[JSFunction(0, JSBuiltin.Global_encodeURIComponent)]
	public static string encodeURIComponent(object uri)
			{
				return InternalEncode(Convert.ToString(uri), false);
			}

	// Escape a string.
	[JSFunction(0, JSBuiltin.Global_escape)]
	[NotRecommended("escape")]
	public static string escape(object str)
			{
				string s = Convert.ToString(str);
				StringBuilder builder = new StringBuilder(s.Length);
				foreach(char ch in s)
				{
					if((ch >= 'A' && ch <= 'Z') ||
					   (ch >= 'a' && ch <= 'z') ||
					   (ch >= '0' && ch <= '9') ||
					   ch == '@' || ch == '*' || ch == '_' ||
					   ch == '+' || ch == '-' || ch == '.' ||
					   ch == '/')
					{
						builder.Append(ch);
					}
					else if(ch < 0x0100)
					{
						builder.Append('%');
						builder.Append(hex[(ch >> 4) & 0x0F]);
						builder.Append(hex[ch & 0x0F]);
					}
					else
					{
						builder.Append('%');
						builder.Append('u');
						builder.Append(hex[(ch >> 12) & 0x0F]);
						builder.Append(hex[(ch >> 8) & 0x0F]);
						builder.Append(hex[(ch >> 4) & 0x0F]);
						builder.Append(hex[ch & 0x0F]);
					}
				}
				return builder.ToString();
			}

	// Evaluate a string.
	[JSFunction(0, JSBuiltin.Global_eval)]
	public static object eval(object str)
			{
				// The parser recognizes "eval" as a special node type,
				// so this method should never be called.
				throw new JScriptException(JSError.IllegalEval);
			}
			
	// Gets an object
	[JSFunction(0, JSBuiltin.Global_GetObject)]
	public static object GetObject(object moniker, object progId)
			{
				throw new JScriptException(JSError.InvalidCall);
			}

	// Determine if a number is finite.
	[JSFunction(0, JSBuiltin.Global_isFinite)]
	public static bool isFinite(double number)
			{
				return !(Double.IsNaN(number) || Double.IsInfinity(number));
			}

	// Determine if a number is NaN.
	[JSFunction(0, JSBuiltin.Global_isNaN)]
	public static bool isNaN(object num)
			{
				return Double.IsNaN(Convert.ToNumber(num));
			}

	// Parse a floating-point value.
	[JSFunction(0, JSBuiltin.Global_parseFloat)]
	public static double parseFloat(object str)
			{
				string s = Convert.ToString(str);
				int index = 0;
				int start;
				char ch;

				// Skip leading white space.
				while(index < s.Length && Char.IsWhiteSpace(s[index]))
				{
					++index;
				}

				// Check for infinities.
				if((s.Length - index) >= 8 &&
				   string.Compare(s, index, "Infinity", 0, 8) == 0)
				{
					return Double.PositiveInfinity;
				}
				if((s.Length - index) >= 9 &&
				   string.Compare(s, index, "-Infinity", 0, 9) == 0)
				{
					return Double.NegativeInfinity;
				}
				if((s.Length - index) >= 9 &&
				   string.Compare(s, index, "+Infinity", 0, 9) == 0)
				{
					return Double.PositiveInfinity;
				}

				// Find the longest prefix that looks like a float.
				start = index;
				if(index < s.Length && (s[index] == '-' || s[index] == '+'))
				{
					++index;
				}
				if(index >= s.Length)
				{
					return Double.NaN;
				}
				ch = s[index];
				if((ch < '0' || ch > '9') && ch != '.')
				{
					return Double.NaN;
				}
				while(ch >= '0' && ch <= '9')
				{
					++index;
					if(index >= s.Length)
					{
						break;
					}
					ch = s[index];
				}
				if(index < s.Length && s[index] == '.')
				{
					++index;
					while(index < s.Length)
					{
						ch = s[index];
						if(ch < '0' || ch > '9')
						{
							break;
						}
						++index;
					}
				}
				if(index < s.Length && (s[index] == 'e' || s[index] == 'E'))
				{
					++index;
					if(index < s.Length && (s[index] == '-' || s[index] == '+'))
					{
						++index;
					}
					if(index >= s.Length)
					{
						return Double.NaN;
					}
					ch = s[index];
					if(ch < '0' || ch > '9')
					{
						return Double.NaN;
					}
					++index;
					while(index < s.Length)
					{
						ch = s[index];
						if(ch < '0' || ch > '9')
						{
							break;
						}
						++index;
					}
				}
				if(start == index)
				{
					return Double.NaN;
				}

				// Convert the string into a floating-point value.
				return Double.Parse(s.Substring(start, index - start),
									NumberFormatInfo.InvariantInfo);
			}

	// Parse an integer value.
	[JSFunction(0, JSBuiltin.Global_parseInt)]
	public static double parseInt(object str, object radix)
			{
				string s = Convert.ToString(str);
				int r = Convert.ToInt32(radix);
				int index = 0;
				double value = 0.0;
				double sign = 1.0;
				int numDigits = 0;
				int digit;
				char ch;

				// Skip leading white space.
				while(index < s.Length && Char.IsWhiteSpace(s[index]))
				{
					++index;
				}

				// Handle the sign.
				if(index < s.Length)
				{
					if(s[index] == '-')
					{
						++index;
						sign = -1.0;
					}
					else if(s[index] == '+')
					{
						++index;
					}
				}

				// If the string is empty, or the radix is invalid
				// then return NaN.
				if(index >= s.Length)
				{
					return Double.NaN;
				}
				if(r == 0)
				{
					r = 10;
					if(s[index] == '0')
					{
						if((index + 1) < s.Length &&
						   (s[index + 1] == 'x' || s[index + 1] == 'X'))
						{
							r = 16;
							index += 2;
						}
						else
						{
							r = 8;
						}
					}
				}
				else if(r < 2 || r > 36)
				{
					return Double.NaN;
				}

				// Process the digits until we hit something else.
				while(index < s.Length)
				{
					ch = s[index++];
					if(ch >= '0' && ch <= '9')
					{
						digit = ch - '0';
					}
					else if(ch >= 'A' && ch <= 'Z')
					{
						digit = ch - 'A' + 10;
					}
					else if(ch >= 'a' && ch <= 'z')
					{
						digit = ch - 'a' + 10;
					}
					else
					{
						digit = 36;
					}
					if(digit >= r)
					{
						break;
					}
					value = value * (double)r + (double)digit;
					++numDigits;
				}
				if(numDigits == 0)
				{
					return Double.NaN;
				}

				// Return the final value.
				return value * sign;
			}

	// Get the name and version information for the script engine.
	//
	// Note: we return the version of the runtime that we are running
	// against, instead of the version that we were compiled against.
	// This is because compile versions make no sense in our environment.
	//
	// Because the JScript library will normally be installed at the
	// same time as the corresponding runtime, using the runtime version
	// will normally give what we expect anyway.
	//
	[JSFunction(0, JSBuiltin.Global_ScriptEngine)]
	public static string ScriptEngine()
			{
				return "JScript";
			}
	[JSFunction(0, JSBuiltin.Global_ScriptEngineBuildVersion)]
	public static int ScriptEngineBuildVersion()
			{
				return Environment.Version.Build;
			}
	[JSFunction(0, JSBuiltin.Global_ScriptEngineMajorVersion)]
	public static int ScriptEngineMajorVersion()
			{
				return Environment.Version.Major;
			}
	[JSFunction(0, JSBuiltin.Global_ScriptEngineMinorVersion)]
	public static int ScriptEngineMinorVersion()
			{
				return Environment.Version.Minor;
			}

	// Unescape a string.
	[JSFunction(0, JSBuiltin.Global_unescape)]
	[NotRecommended("unescape")]
	public static string unescape(object str)
			{
				string s = Convert.ToString(str);
				StringBuilder builder = new StringBuilder(s.Length);
				int index = 0;
				char ch;
				while(index < s.Length)
				{
					ch = s[index++];
					if(ch != '%')
					{
						builder.Append(ch);
					}
					else if((index + 1) < s.Length &&
						    JSScanner.IsHexDigit(s[index]) &&
						    JSScanner.IsHexDigit(s[index + 1]))
					{
						ch = (char)((JSScanner.FromHex(s[index]) << 4) |
									JSScanner.FromHex(s[index + 1]));
						builder.Append(ch);
						index += 2;
					}
					else if((index + 4) < s.Length &&
						    s[index] == 'u' &&
						    JSScanner.IsHexDigit(s[index + 1]) &&
						    JSScanner.IsHexDigit(s[index + 2]) &&
						    JSScanner.IsHexDigit(s[index + 3]) &&
						    JSScanner.IsHexDigit(s[index + 4]))
					{
						ch = (char)((JSScanner.FromHex(s[index + 1]) << 12) |
									(JSScanner.FromHex(s[index + 2]) << 8) |
									(JSScanner.FromHex(s[index + 3]) << 4) |
									JSScanner.FromHex(s[index + 4]));
						builder.Append(ch);
						index += 5;
					}
					else
					{
						builder.Append(ch);
					}
				}
				return builder.ToString();
			}

}; // class GlobalObject

// "Lenient" version of the above class which exports all of the
// object's properties to the user level.
public sealed class LenientGlobalObject : GlobalObject
{
	// Accessible properties.
	public new object boolean;
	public new object @byte;
	public new object @sbyte;
	public new object @char;
	public new object @short;
	public new object @ushort;
	public new object @int;
	public new object @uint;
	public new object @long;
	public new object @ulong;
	public new object @float;
	public new object @double;
	public new object @decimal;
	public new object @void;
	public new object CollectGarbage;
	public new object decodeURI;
	public new object decodeURIComponent;
	public new object encodeURI;
	public new object encodeURIComponent;
	[NotRecommended("escape")] public new object escape;
	public new object eval;
	public new object GetObject;
	public new object isFinite;
	public new object isNaN;
	public new object parseFloat;
	public new object parseInt;
	public new object ScriptEngine;
	public new object ScriptEngineBuildVersion;
	public new object ScriptEngineMajorVersion;
	public new object ScriptEngineMinorVersion;
	[NotRecommended("unescape")] public new object unescape;
	public object Infinity;
	public object NaN;
	public object undefined;

	// Constructor.
	internal LenientGlobalObject(VsaEngine engine)
			: base(engine)
			{
				boolean = GlobalObject.boolean;
				@byte = GlobalObject.@byte;
				@sbyte = GlobalObject.@sbyte;
				@char = GlobalObject.@char;
				@short = GlobalObject.@short;
				@ushort = GlobalObject.@ushort;
				@int = GlobalObject.@int;
				@uint = GlobalObject.@uint;
				@long = GlobalObject.@long;
				@ulong = GlobalObject.@ulong;
				@float = GlobalObject.@float;
				@double = GlobalObject.@double;
				@decimal = GlobalObject.@decimal;
				@void = GlobalObject.@void;
				CollectGarbage = globalObject.Get("CollectGarbage");
				decodeURI = globalObject.Get("decodeURI");
				decodeURIComponent =
					globalObject.Get("decodeURIComponent");
				encodeURI = globalObject.Get("encodeURI");
				encodeURIComponent =
					globalObject.Get("encodeURIComponent");
				escape = globalObject.Get("escape");
				eval = globalObject.Get("eval");
				GetObject = globalObject.Get("GetObject");
				isFinite = globalObject.Get("isFinite");
				isNaN = globalObject.Get("isNaN");
				parseFloat = globalObject.Get("parseFloat");
				parseInt = globalObject.Get("parseInt");
				ScriptEngine = globalObject.Get("ScriptEngine");
				ScriptEngineBuildVersion =
					globalObject.Get("ScriptEngineBuildVersion");
				ScriptEngineMajorVersion =
					globalObject.Get("ScriptEngineMajorVersion");
				ScriptEngineMinorVersion =
					globalObject.Get("ScriptEngineMinorVersion");
				unescape = globalObject.Get("unescape");
				Infinity = Double.PositiveInfinity;
				NaN = Double.NaN;
				undefined = null;
			}

	// Get or set the class constructors.
#if false
	private object activeXObject;
	public new object ActiveXObject
			{
				get
				{
					lock(this)
					{
						if(activeXObject == null ||
						   activeXObject is Missing)
						{
							activeXObject = GlobalObject.ActiveXObject;
						}
						return activeXObject;
					}
				}
				set
				{
					lock(this)
					{
						activeXObject = value;
					}
				}
			}
#endif
	private object array;
	public new object Array
			{
				get
				{
					lock(this)
					{
						if(array == null ||
						   array is Missing)
						{
							array = EngineInstance.GetEngineInstance
								(engine).GetArrayConstructor();
						}
						return array;
					}
				}
				set
				{
					lock(this)
					{
						array = value;
					}
				}
			}
#if false
	private object booleanConstructor;
	public new object Boolean
			{
				get
				{
					lock(this)
					{
						if(booleanConstructor == null ||
						   booleanConstructor is Missing)
						{
							booleanConstructor = GlobalObject.Boolean;
						}
						return booleanConstructor;
					}
				}
				set
				{
					lock(this)
					{
						booleanConstructor = value;
					}
				}
			}
	private object date;
	public new object Date
			{
				get
				{
					lock(this)
					{
						if(date == null ||
						   date is Missing)
						{
							date = GlobalObject.Date;
						}
						return date;
					}
				}
				set
				{
					lock(this)
					{
						date = value;
					}
				}
			}
	private object enumerator;
	public new object Enumerator
			{
				get
				{
					lock(this)
					{
						if(enumerator == null ||
						   enumerator is Missing)
						{
							enumerator = GlobalObject.Enumerator;
						}
						return enumerator;
					}
				}
				set
				{
					lock(this)
					{
						enumerator = value;
					}
				}
			}
	private object error;
	public new object Error
			{
				get
				{
					lock(this)
					{
						if(error == null ||
						   error is Missing)
						{
							error = GlobalObject.Error;
						}
						return error;
					}
				}
				set
				{
					lock(this)
					{
						error = value;
					}
				}
			}
	private object evalError;
	public new object EvalError
			{
				get
				{
					lock(this)
					{
						if(evalError == null ||
						   evalError is Missing)
						{
							evalError = GlobalObject.EvalError;
						}
						return evalError;
					}
				}
				set
				{
					lock(this)
					{
						evalError = value;
					}
				}
			}
#endif
	private object function;
	public new object Function
			{
				get
				{
					lock(this)
					{
						if(function == null ||
						   function is Missing)
						{
							function = EngineInstance.GetEngineInstance
									(engine).GetFunctionConstructor();
						}
						return function;
					}
				}
				set
				{
					lock(this)
					{
						function = value;
					}
				}
			}
	private object math;
	public new object Math
			{
				get
				{
					lock(this)
					{
						if(math == null ||
						   math is Missing)
						{
							// should be thread safe to do this ?
							return new LenientMathObject(
								EngineInstance.Default.GetObjectPrototype(), 
								EngineInstance.Default.GetFunctionPrototype());
						}
						return math;
					}
				}
				set
				{
					lock(this)
					{
						math = value;
					}
				}
			}
	private object number;
	public new object Number
			{
				get
				{
					lock(this)
					{
						if(number == null ||
						   number is Missing)
						{
							number = GlobalObject.Number;
						}
						return number;
					}
				}
				set
				{
					lock(this)
					{
						number = value;
					}
				}
			}
	private object objectConstructor;
	public new object Object
			{
				get
				{
					lock(this)
					{
						if(objectConstructor == null ||
						   objectConstructor is Missing)
						{
							objectConstructor =
								EngineInstance.GetEngineInstance
									(engine).GetObjectConstructor();
						}
						return objectConstructor;
					}
				}
				set
				{
					lock(this)
					{
						objectConstructor = value;
					}
				}
			}
#if false
	private object rangeError;
	public new object RangeError
			{
				get
				{
					lock(this)
					{
						if(rangeError == null ||
						   rangeError is Missing)
						{
							rangeError = GlobalObject.RangeError;
						}
						return rangeError;
					}
				}
				set
				{
					lock(this)
					{
						rangeError = value;
					}
				}
			}
	private object referenceError;
	public new object ReferenceError
			{
				get
				{
					lock(this)
					{
						if(referenceError == null ||
						   referenceError is Missing)
						{
							referenceError = GlobalObject.ReferenceError;
						}
						return referenceError;
					}
				}
				set
				{
					lock(this)
					{
						referenceError = value;
					}
				}
			}
	private object regexp;
	public new object RegExp
			{
				get
				{
					lock(this)
					{
						if(regexp == null ||
						   regexp is Missing)
						{
							regexp = GlobalObject.RegExp;
						}
						return regexp;
					}
				}
				set
				{
					lock(this)
					{
						regexp = value;
					}
				}
			}
#endif
	private object stringConstructor;
	public new object String
			{
				get
				{
					lock(this)
					{
						if(stringConstructor == null ||
						   stringConstructor is Missing)
						{
							stringConstructor =
								EngineInstance.GetEngineInstance
									(engine).GetStringConstructor();
						}
						return stringConstructor;
					}
				}
				set
				{
					lock(this)
					{
						stringConstructor = value;
					}
				}
			}
#if false
	private object syntaxError;
	public new object SyntaxError
			{
				get
				{
					lock(this)
					{
						if(syntaxError == null ||
						   syntaxError is Missing)
						{
							syntaxError = GlobalObject.SyntaxError;
						}
						return syntaxError;
					}
				}
				set
				{
					lock(this)
					{
						syntaxError = value;
					}
				}
			}
	private object typeError;
	public new object TypeError
			{
				get
				{
					lock(this)
					{
						if(typeError == null ||
						   typeError is Missing)
						{
							typeError = GlobalObject.TypeError;
						}
						return typeError;
					}
				}
				set
				{
					lock(this)
					{
						syntaxError = value;
					}
				}
			}
	private object uriError;
	public new object URIError
			{
				get
				{
					lock(this)
					{
						if(uriError == null ||
						   uriError is Missing)
						{
							uriError = GlobalObject.URIError;
						}
						return uriError;
					}
				}
				set
				{
					lock(this)
					{
						uriError = value;
					}
				}
			}
	private object vbArray;
	public new object VBArray
			{
				get
				{
					lock(this)
					{
						if(vbArray == null ||
						   vbArray is Missing)
						{
							vbArray = GlobalObject.VBArray;
						}
						return vbArray;
					}
				}
				set
				{
					lock(this)
					{
						vbArray = value;
					}
				}
			}
#endif

}; // class LenientGlobalObject

}; // namespace Microsoft.JScript
