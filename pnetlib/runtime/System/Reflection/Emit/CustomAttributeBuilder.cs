/*
 * CustomAttributeBuilder.cs - Implementation of the
 *		"System.Reflection.Emit.CustomAttributeBuilder" class.
 *
 * Copyright (C) 2002  Southern Storm Software, Pty Ltd.
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

namespace System.Reflection.Emit
{

#if CONFIG_REFLECTION_EMIT

using System;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;


public class CustomAttributeBuilder
{
	internal ConstructorInfo con;
	private Object[] constructorArgs;
	private PropertyInfo[] namedProperties;
	private Object[] propertyValues;
	private FieldInfo[] namedFields;
	private Object[] fieldValues;


	// A subset of the members from Partition II 22.1.15
	// that are valid types for custom attributes. Note:
	// these values are different from NATIVE_TYPE_* and
	// TypeCode
	private enum SerializationType: byte
	{
		End = 0x00,
		Void = 0x01,
		Boolean = 0x02,
		Char = 0x03,
		SByte = 0x04,
		Byte = 0x05,
		Int16 = 0x06,
		UInt16 = 0x07,
		Int32 = 0x08,
		UInt32 = 0x09,
		Int64 = 0x0a,
		UInt64 = 0x0b,
		Single = 0x0c,
		Double = 0x0d,
		String = 0x0e,
		Type = 0x50,
		TaggedObject = 0x51,
		Field = 0x53,
		Property = 0x54,
		Array = 0x1d };

	public CustomAttributeBuilder(ConstructorInfo con,
										Object[] constructorArgs):
			this(con, constructorArgs, new PropertyInfo[0],
					new Object[0], new FieldInfo[0], new Object[0])
		{
		}

	public CustomAttributeBuilder(ConstructorInfo con,
										Object[] constructorArgs,
										FieldInfo[] namedFields,
										Object[] fieldValues):
			this(con, constructorArgs, new PropertyInfo[0],
					new Object[0], namedFields, fieldValues)
		{
		}

	public CustomAttributeBuilder(ConstructorInfo con,
										Object[] constructorArgs,
										PropertyInfo[] namedProperties,
										Object[] propertyValues):
			this(con, constructorArgs, namedProperties, propertyValues,
					new FieldInfo[0], new Object[0])
		{
		}

	public CustomAttributeBuilder(ConstructorInfo con,
										Object[] constructorArgs,
										PropertyInfo[] namedProperties,
										Object[] propertyValues,
										FieldInfo[] namedFields,
										Object[] fieldValues)
		{
			// Proceed through the documented exception list mostly
			// in order
			if (con == null)
				throw new ArgumentNullException("con");
			if (constructorArgs == null)
				throw new ArgumentNullException("constructorArgs");
			if (namedProperties == null)
				throw new ArgumentNullException("namedProperties");
			if (propertyValues == null)
				throw new ArgumentNullException("propertyValues");
			if (namedFields == null)
				throw new ArgumentNullException("namedFields");
			if (fieldValues == null)
				throw new ArgumentNullException("fieldValues");

			// TODO: check whether any elements of the array
			// parameters is null

			if (namedProperties.Length != propertyValues.Length)
				throw new ArgumentException(_("Emit_LengthsPropertiesDifferent"));
			if (namedFields.Length != fieldValues.Length)
				throw new ArgumentException(_("Emit_LengthsFieldsDifferent"));

			if (con.IsStatic || con.IsPrivate)
				throw new ArgumentException(_("Emit_ConstructorPrivateOrStatic"));

			// Validate length and type of ctor parameters
			ParameterInfo[] parameters = con.GetParameters();
			if (parameters.Length != constructorArgs.Length)
			{
				throw new ArgumentException(_("Emit_ConstructorArgsNumber"));
			}
			for (int i = 0; i <parameters.Length; i++)
			{
				if (! IsValidSerializationType(parameters[i].ParameterType))
				{
					throw new ArgumentException(_("Emit_IllegalCustomAttributeType"));
				}
				if ( ! IsSameOrSubclassOf(constructorArgs[i].GetType(), parameters[i].ParameterType))
				{
					throw new ArgumentException(_("Emit_ConstructorArgsType"),
							parameters[i].Name);
				}
			}

			// Validate property types, whether the property has a setter
			for (int i = 0; i < namedProperties.Length; i++)
			{
				if (! IsValidSerializationType(namedProperties[i].PropertyType))
				{
					throw new ArgumentException(_("Emit_IllegalCustomAttributeType"));
				}
				if (! IsSameOrSubclassOf(propertyValues[i].GetType(), namedProperties[i].PropertyType))
				{
					throw new ArgumentException(_("Emit_PropertyType"),
							namedProperties[i].Name);
				}
				if (!namedProperties[i].CanWrite)
				{
					throw new ArgumentException(_("Emit_CannotWrite"),
						namedProperties[i].Name);
				}
			}

			// Validate fields and whether each field belongs to the
			// same class or base class as the constructor
			for (int i = 0; i < namedFields.Length; i++)
			{
				if (! IsValidSerializationType(namedFields[i].FieldType))
				{
					throw new ArgumentException(_("Emit_IllegalCustomAttributeType"));
				}
				if (! IsSameOrSubclassOf(fieldValues[i].GetType(), namedFields[i].FieldType))
				{
					throw new ArgumentException(_("Emit_FieldType"),
						namedFields[i].Name);
				}
				if (! IsSameOrSubclassOf(namedFields[i].DeclaringType, con.DeclaringType))
				{
					throw new ArgumentException(_("Emit_FieldNotAssignable"),
							namedFields[i].Name);
				}
			}

			this.con = con;
			this.constructorArgs = constructorArgs;
			this.namedProperties = namedProperties;
			this.propertyValues = propertyValues;
			this.namedFields = namedFields;
			this.fieldValues = fieldValues;
		} // end CustomAttributeBuilder()

	// Verify whether Type type is a valid type for a formal
	// parameter, field, or property, in a custom attribute.
	// (This should be used on components of the attribute
	// class itself -- not on the parameters passed to the
	// attribute.)
	private static bool IsValidSerializationType(Type type)
		{
			// Permitted types are String, Type, Object, any primitive
			// type, an enum with an underlying integral type, or a
			// one-dimensional array of any of the above.

			if (type == typeof(string) || type == typeof(Type) ||
					type == typeof(Object) || type.IsPrimitive)
			{
				return true;
			}
			else if (type.IsEnum)
			{
				// The underlying type of an enum must be integral
				// (i.e. no Single, Double, or Char)
				//
				// Note: GetTypeCode() gets the typecode for the underlying
				// type when the type is an enum.
				switch (Type.GetTypeCode(type))
				{
					case TypeCode.SByte:
					case TypeCode.Byte:
					case TypeCode.Int16:
					case TypeCode.UInt16:
					case TypeCode.Int32:
					case TypeCode.UInt32:
					case TypeCode.Int64:
					case TypeCode.UInt64:
						return true;
					default:
						return false;
				}
			}
			else if (type.IsArray && type.GetArrayRank() == 1)
			{
				return IsValidSerializationType(type.GetElementType());
			}
			else
			{
				return false;
			}
		} // IsValidSerializatoinType()

	// Returns true is typeA and typeB are the same, or if typeA is
	// a subclass of typeB.
	private static bool IsSameOrSubclassOf(Type typeA, Type typeB)
		{
			if (typeA == typeB || typeA.IsSubclassOf(typeB))
				return true;
			else
				return false;
		}

	// Write a SerString to the writer.
	private static void WriteSerString(String name, BinaryWriter writer)
		{
			byte[] encodedString = Encoding.UTF8.GetBytes(name);
			byte[] encodedLength = ClrHelpers.ToPackedLen(encodedString.Length);
			writer.Write(encodedLength, 0, encodedLength.Length);
			writer.Write(encodedString, 0, encodedString.Length);
		}

	// Write a field or property type to the writer.
	private static void WriteFieldOrPropType(Type type, BinaryWriter writer)
		{
			if (IsSameOrSubclassOf(type, typeof(Type)))
			{
				writer.Write((byte) SerializationType.Type);
			}
			else if (type == typeof(Object))
			{
				writer.Write((byte) SerializationType.TaggedObject);
			}
			else
			{
				switch (Type.GetTypeCode(type))
				{
					case TypeCode.Boolean:
						writer.Write((byte) SerializationType.Boolean);
						break;
					case TypeCode.Char:
						writer.Write((byte) SerializationType.Char);
						break;
					case TypeCode.SByte:
						writer.Write((byte) SerializationType.SByte);
						break;
					case TypeCode.Byte:
						writer.Write((byte) SerializationType.Byte);
						break;
					case TypeCode.Int16:
						writer.Write((byte) SerializationType.Int16);
						break;
					case TypeCode.UInt16:
						writer.Write((byte) SerializationType.UInt16);
						break;
					case TypeCode.Int32:
						writer.Write((byte) SerializationType.Int32);
						break;
					case TypeCode.UInt32:
						writer.Write((byte) SerializationType.UInt32);
						break;
					case TypeCode.Int64:
						writer.Write((byte) SerializationType.Int64);
						break;
					case TypeCode.UInt64:
						writer.Write((byte) SerializationType.UInt64);
						break;
					case TypeCode.Single:
						writer.Write((byte) SerializationType.Single);
						break;
					case TypeCode.Double:
						writer.Write((byte) SerializationType.Double);
						break;
					case TypeCode.String:
						writer.Write((byte) SerializationType.String);
						break;
				}
			}
		} // WriteFieldOrPropType()

	// Write an element into the given memory stream
	private static void WriteElem(Object elem, Type paramType, BinaryWriter writer)
	{
			// The tricky part is that we must differentiate
			// boxed and unboxed valuetypes. Boxed valuetypes
			// will have ctor parameter type "Object", whereas
			// unboxed valuetypes will be declared as valuetypes

			Type elemType = elem.GetType();

			if (elem is System.String)
			{
				WriteSerString((String) elem, writer);
			}
			else if (elem is System.Type)
			{
				WriteSerString(((Type) elem).AssemblyQualifiedName, writer);
			}
			else if (elemType.IsPrimitive || elemType.IsEnum)
			{
				// Boxed valuetypes are preceeded by their
				// SerializationType
				if (paramType == typeof(Object))
				{
					WriteFieldOrPropType(elemType, writer);
				}

				// Note: When GetTypeCode() is invoked on enums, it
				// retrieves the TypeCode for the underlying integral type.

				switch (Type.GetTypeCode(elemType))
				{
					case TypeCode.Boolean:
						writer.Write((Boolean) elem);
						break;
					case TypeCode.Char:
						writer.Write((Char) elem);
						break;
					case TypeCode.SByte:
						writer.Write((SByte) elem);
						break;
					case TypeCode.Byte:
						writer.Write((Byte) elem);
						break;
					case TypeCode.Int16:
						writer.Write((Int16) elem);
						break;
					case TypeCode.UInt16:
						writer.Write((UInt16) elem);
						break;
					case TypeCode.Int32:
						writer.Write((Int32) elem);
						break;
					case TypeCode.UInt32:
						writer.Write((UInt32) elem);
						break;
					case TypeCode.Int64:
						writer.Write((Int64) elem);
						break;
					case TypeCode.UInt64:
						writer.Write((UInt64) elem);
						break;
					case TypeCode.Single:
						writer.Write((Single) elem);
						break;
					case TypeCode.Double:
						writer.Write((Double) elem);
						break;
				}
			}
			// TODO: Should we throw an exception if none of
			// these is applicatable? (The ctor really should check, though.)
			// If so, what exception do we throw?
		} // WriteElem

	// Write a FixedArg into the memory stream
	private static void WriteFixedArg(object elem, Type paramType, BinaryWriter writer)
		{
			if (! paramType.IsArray )
			{
				WriteElem(elem, paramType, writer);
			}
			else
			{
				Array arrayArg = (Array) elem;
				writer.Write((UInt32) arrayArg.Length);
				for (int j = 0; j < arrayArg.Length; j++)
				{
					WriteElem(arrayArg.GetValue(j), paramType, writer);
				}
			}
		}

	// Write a NamedArg into the memory stream. (SerializationType.Field
	// or .Property should have already been written.)
	private static void WriteNamedArg(Object elem, Type paramType,
			string fieldName, BinaryWriter writer)
		{
			// elementType will either be the paramType or, if paramType
			// is an array, the element type of paramType
			Type elementType;

			if (paramType.IsArray)
			{
				// Arrays are prefixed with SerializationType.Array
				// (0x1d), which is not documented.
				writer.Write((byte) SerializationType.Array);
				elementType = paramType.GetElementType();
			}
			else
			{
				elementType = paramType;
			}

			WriteFieldOrPropType(elementType, writer);

			// FieldOrPropName
			WriteSerString(fieldName, writer);

			// The rest is treated as a FixedArg
			WriteFixedArg(elem, paramType, writer);
		}

	// Create a blob representation of the custom attribute
	internal byte[] ToBytes()
		{
			MemoryStream stream = new MemoryStream();

			// Note: BinaryWriter uses little-endian on all platforms,
			// which is needed for non-compressed binary values
			BinaryWriter writer = new BinaryWriter(stream);

			ParameterInfo[] paramInfo = con.GetParameters();

			// Prolog for custom attribute is an unsigned int16
			writer.Write((UInt16) 0x0001);

			// Zero or not FixedArgs (stored in constructorArgs)
			for (int i = 0; i < constructorArgs.Length; i++)
			{
				WriteFixedArg(constructorArgs[i], paramInfo[i].ParameterType, writer);
			}

			// NumNamedArgs
			writer.Write((UInt16) (namedProperties.Length + namedFields.Length));

			//NamedArgs -- fields
			for (int i = 0; i < namedFields.Length; i++)
			{
				writer.Write((byte) SerializationType.Field);
				WriteNamedArg(fieldValues[i], namedFields[i].FieldType,
						namedFields[i].Name, writer);
			}

			// NamedArgs -- properties
			for (int i = 0; i < namedProperties.Length; i++)
			{
				writer.Write((byte) SerializationType.Property);
				WriteNamedArg(propertyValues[i], namedProperties[i].PropertyType,
						namedProperties[i].Name, writer);
			}

			return stream.ToArray();
		} // ToBytes()

}; // class CustomAttributeBuilder

#endif // CONFIG_REFLECTION_EMIT

}; // namespace System.Reflection.Emit
