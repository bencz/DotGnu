/*
 * BinaryValueReader.cs - Implementation of the
 *	"System.Runtime.Serialization.Formatters.Binary.BinaryValueReader" class.
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

namespace System.Runtime.Serialization.Formatters.Binary
{

#if CONFIG_SERIALIZATION

using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/*
*  unsupported features:
* 	- BinaryElementType.MethodCall: not needed by me :-)
* 	- BinaryElementType.MethodResponse: not needed by me :-)
*   - user defined serialization headers
* 
*  unsupported array types:
*   - Arrays of Enums
*   - BinaryArrayType.Jagged
*   - BinaryArrayType.Multidimensional
*   - BinaryArrayType.SingleWithLowerBounds
*   - BinaryArrayType.JaggedWithLowerBounds
*   - BinaryArrayType.MultidimensionalWithLowerBounds
*/

/*
 * this class contains all context info for a single deserialization.
 */
internal class DeserializationContext 
{
	// the manager used for unresolved references
	public ObjectManager Manager;
	// the reader supplying the data
	public BinaryReader Reader;
	// meta-info for already deserialized types is stored here
	private Hashtable mTypeStore;
	// meta-info for already loaded assemblies is stored here
	private Hashtable mAssemblyStore;
	// the calling BinaryFormatter (not used for now)
	public BinaryFormatter Formatter;
	// info if there are user defined headers (not used for now as 
	// headers are not supported for now)
	public bool IsHeaderPresent;
	// major and minor version used for serialization
	public uint MajorVersion, MinorVersion;

	public DeserializationContext(BinaryFormatter formatter,
		BinaryReader reader)
	{
		Formatter = formatter;
		Reader = reader;
		mTypeStore = new Hashtable();
		mAssemblyStore = new Hashtable();
		Manager = new ObjectManager(formatter.SurrogateSelector, 
										formatter.Context);
	}

	public void SetAssembly(uint id, Assembly val) 
	{
		mAssemblyStore[id] = val;
	}

	public Assembly GetAssembly(uint id) 
	{
		if(!mAssemblyStore.ContainsKey(id)) 
		{
			throw new SerializationException("unknown assembly id:"+id);
		} 
		else 
		{
			return (Assembly) mAssemblyStore[id];
		}
	}

	public void SetTypeInfo(uint id, TypeInfo val) 
	{
		mTypeStore[id] = val;
	}

	public TypeInfo GetTypeInfo(uint id) 
	{
		if(!mTypeStore.ContainsKey(id)) 
		{
			throw new SerializationException("unknown typeinfo id:"+id);
		} 
		else 
		{
			return (TypeInfo) mTypeStore[id];
		}
	}
}

/*
 * stores information about the type of a field.
 */
internal class TypeSpecification 
{
	private BinaryTypeTag mTag;
	private BinaryPrimitiveTypeCode mPrimitiveType;
	private String mClassName;
	private uint mAssembly;

	public TypeSpecification(BinaryPrimitiveTypeCode primitive) 
	{
		mPrimitiveType = primitive;
		mTag = BinaryTypeTag.PrimitiveType;
	}
	public TypeSpecification(String name, uint ass) 
	{
		mClassName = name;
		mAssembly = ass;
		mTag = BinaryTypeTag.GenericType;
	}
	public TypeSpecification(String name) 
	{
		mClassName = name;
		mTag = BinaryTypeTag.RuntimeType;
	}

	public BinaryPrimitiveTypeCode GetPrimitiveType() 
	{
		if(mTag != BinaryTypeTag.PrimitiveType) 
		{
			throw new SerializationException("illegal usage if type-spec");
		}
		else 
		{
			return mPrimitiveType;
		}
	}

	public Type GetObjectType(DeserializationContext context) 
	{
		if(mTag == BinaryTypeTag.PrimitiveType) 
		{
			return BinaryValueReader.GetPrimitiveType(mPrimitiveType);
		}
		else if(mTag == BinaryTypeTag.RuntimeType) 
		{
			return Type.GetType(mClassName, true);
		} 
		else 
		{
			Assembly assembly = context.GetAssembly(mAssembly);
			return assembly.GetType(mClassName, true);
		}
	}
}

/*
 * stores info about an already deserialized Type
 */
internal class TypeInfo 
{
	private String[] mFieldNames;
	private BinaryTypeTag[] mTypeTag;
	private TypeSpecification[] mTypeSpec;
	private Type mObjectType;
	private MemberInfo[] mMembers;
    private bool mIsIserializable;
    private FormatterConverter mConverter;

	public TypeInfo(DeserializationContext context, String name, String[] fieldNames,
	                BinaryTypeTag[] tt, TypeSpecification[] ts, Assembly assembly) 
	{
        mConverter = context.Formatter.converter;
		mFieldNames = fieldNames;
		mTypeTag = tt;
		mTypeSpec = ts;

		// lookup our type in the right assembly
		if(assembly == null) 
		{
			mObjectType = Type.GetType(name, true);
		} 
		else 
		{
			mObjectType = assembly.GetType(name, true);
		}

		if(typeof(ISerializable).IsAssignableFrom(mObjectType))
        {
            mIsIserializable = true;
        }
        else
        {
            mIsIserializable = false;

		    // lookup all members once
	    	mMembers = new MemberInfo[NumMembers];
    		for(int i = 0; i < NumMembers; i++) 
		    {
	    		// ms and mono have their values for boxed primitive types called 'm_value', we need a fix for that
    			if(mObjectType.IsPrimitive && (mFieldNames[i] == "m_value")) 
			    {
		    		mFieldNames[i] = "value_";
	    		}
    			else if (mObjectType == typeof(DateTime) && 
			    			(mFieldNames[i] == "ticks")) 
		    	{
	    			// this is for DateTime
    				mFieldNames[i] = "value_";
			    } 
		    	else if (mObjectType == typeof(TimeSpan) && 
	    					(mFieldNames[i] == "_ticks")) 
    			{
		    		// this is for TimeSpan
	    			mFieldNames[i] = "value_";
    			} 
			    else if (mObjectType == typeof(Decimal)) 
		    	{
	    			switch(mFieldNames[i]) 
    				{
				    	case "hi":
			    		{
		    				mFieldNames[i] = "high";
	    				}
    					break;
					    case "lo":
				    	{
			    			mFieldNames[i] = "low";
		    			}
	    				break;
    					case "mid":
					    {
				    		mFieldNames[i] = "middle";
			    		}
		    			break;
	    			}
    			}

		    	Type memberType;
	    		String memberName;
    			int classSeparator = mFieldNames[i].IndexOf('+');
			    if(classSeparator != -1) 
		    	{
	    			/*
    				*  TODO: check if there are constraints in which assembly 
				    *  the Type may be looked up! for now just look it up 
			    	*  generally
		    		*/
	    			String baseName = mFieldNames[i].Substring(0, classSeparator);
    				memberName = mFieldNames[i].Substring(classSeparator+1, mFieldNames[i].Length-classSeparator-1);
    				memberType = mObjectType;

	    			// MS does NOT store the FullQualifiedTypename if there 
    				// is no collision but only the Typename :-(
				    while(!memberType.FullName.EndsWith(baseName)) 
			    	{
		    			// check if we reached System.Object
	    				if(memberType == memberType.BaseType || memberType == null) 
    					{
						    // TODO : I18n
					    	throw new SerializationException("Can't find member "+mFieldNames[i]);
				    	}
			    		memberType = memberType.BaseType;
		    		}
	    		} 
    			else 
			    {
		    		memberType = mObjectType;
	    			memberName = mFieldNames[i];
    			}

	    		// get member from object
    			MemberInfo[] members = memberType.GetMember(memberName, 
				    							MemberTypes.Field | 
			    								MemberTypes.Property, 
		    									BindingFlags.Instance | 
	    										BindingFlags.Public | 
    											BindingFlags.NonPublic);

		    	if((members == null) || (members.Length < 1)) 
	    		{
    				// TODO: I18n
			    	throw new SerializationException("Can't find member "+mFieldNames[i]);
		    	} 
	    		else 
    			{
			    	mMembers[i] = members[0];
		    	}
	    	}
    	}
	}

	public BinaryTypeTag GetTypeTag(uint index) 
	{
		return mTypeTag[index];
	}

	public TypeSpecification GetTypeSpecification(uint index) 
	{
		return mTypeSpec[index];
	}

	public MemberInfo GetMember(uint index) 
	{
		return mMembers[index];
	}

	public String GetFieldName(uint index) 
	{
		return mFieldNames[index];
	}

	public SerializationInfo GetSerializationInfo() 
	{
		return new SerializationInfo(mObjectType, mConverter);
	}

	public Type ObjectType 
	{
		get {return mObjectType; }
	}

	public int NumMembers 
	{
		get { return mTypeTag.Length; }
	}

	public bool IsISerializable
	{
		get { return mIsIserializable; }
	}
}

/*
 * internal class that is passed arround to indicate that not a
 * 'real' object was read, but only a reference to one.
 */
internal class DelayedReferenceHolder 
{
	private uint mReferenceId;

	public DelayedReferenceHolder(uint refId) 
	{
		mReferenceId = refId;
	}

	public uint ReferenceId 
	{
		get { return mReferenceId; }
	}
}

/*
*  internal class that is passed arround to indicate that not a
*  'real' object was read, but a value indicating that an array
*  should be filled with NULL values.
*/
internal class ArrayNullValueHolder 
{
	private uint mNumNullValues;

	public ArrayNullValueHolder(uint num) 
	{
		mNumNullValues = num;
	}

	public uint NumNullValues 
	{
		get { return mNumNullValues; }
	}
}

/*
 * mother of all reading classes
 */
abstract class BinaryValueReader 
{
	private static BinaryValueReader sHeaderReader = new HeaderReader();
	private static BinaryValueReader sEndReader = new EndReader();
	private static BinaryValueReader sStringReader = new StringReader();
	private static BinaryValueReader sNullReader = new NullReader();
	private static BinaryValueReader sPrimitiveArrayReader = new PrimitiveArrayReader();
	private static BinaryValueReader sStringArrayReader = new StringArrayReader();
	private static BinaryValueReader sObjectArrayReader = new ObjectArrayReader();
	private static BinaryValueReader sAssemblyReader = new AssemblyReader();
	private static BinaryValueReader sExternalObjectReader = new ExternalObjectReader();
	private static BinaryValueReader sRuntimeObjectReader = new RuntimeObjectReader();
	private static BinaryValueReader sRefObjectReader = new RefObjectReader();
	private static BinaryValueReader sObjectReferenceReader = new ObjectReferenceReader();
	private static BinaryValueReader sGenericArrayReader = new GenericArrayReader();
	private static BinaryValueReader sArrayFiller8bReader = new ArrayFiller8bReader();
	private static BinaryValueReader sArrayFiller32bReader = new ArrayFiller32bReader();
	private static BinaryValueReader sBoxedPrimitiveTypeValue = new BoxedPrimitiveTypeValue();

	private static TypeSpecification sBooleanSpec = new TypeSpecification(BinaryPrimitiveTypeCode.Boolean);
	private static TypeSpecification sByteSpec = new TypeSpecification(BinaryPrimitiveTypeCode.Byte);
	private static TypeSpecification sCharSpec = new TypeSpecification(BinaryPrimitiveTypeCode.Char);
	private static TypeSpecification sDecimalSpec = new TypeSpecification(BinaryPrimitiveTypeCode.Decimal);
	private static TypeSpecification sDoubleSpec = new TypeSpecification(BinaryPrimitiveTypeCode.Double);
	private static TypeSpecification sInt16Spec = new TypeSpecification(BinaryPrimitiveTypeCode.Int16);
	private static TypeSpecification sInt32Spec = new TypeSpecification(BinaryPrimitiveTypeCode.Int32);
	private static TypeSpecification sInt64Spec = new TypeSpecification(BinaryPrimitiveTypeCode.Int64);
	private static TypeSpecification sSByteSpec = new TypeSpecification(BinaryPrimitiveTypeCode.SByte);
	private static TypeSpecification sSingleSpec = new TypeSpecification(BinaryPrimitiveTypeCode.Single);
	private static TypeSpecification sTimeSpanSpec = new TypeSpecification(BinaryPrimitiveTypeCode.TimeSpan);
	private static TypeSpecification sDateTimeSpec = new TypeSpecification(BinaryPrimitiveTypeCode.DateTime);
	private static TypeSpecification sUInt16Spec = new TypeSpecification(BinaryPrimitiveTypeCode.UInt16);
	private static TypeSpecification sUInt32Spec = new TypeSpecification(BinaryPrimitiveTypeCode.UInt32);
	private static TypeSpecification sUInt64Spec = new TypeSpecification(BinaryPrimitiveTypeCode.UInt64);
	private static TypeSpecification sStringSpec = new TypeSpecification(BinaryPrimitiveTypeCode.String);

	private static TypeSpecification sStringSpecObject = new TypeSpecification("System.String");
	private static TypeSpecification sObjectSpec = new TypeSpecification("System.Object");
	private static TypeSpecification sStringArraySpec = new TypeSpecification("System.String[]");
	private static TypeSpecification sObjectArraySpec = new TypeSpecification("System.Object[]");

	public static BinaryValueReader GetReader(BinaryElementType type) 
	{
		switch(type) 
		{
			case BinaryElementType.Header:
				return sHeaderReader;
			case BinaryElementType.RefTypeObject:
				return sRefObjectReader;
			case BinaryElementType.RuntimeObject:
				return sRuntimeObjectReader;
			case BinaryElementType.ExternalObject:
				return sExternalObjectReader;
			case BinaryElementType.String:
				return sStringReader;
			case BinaryElementType.GenericArray:
				return sGenericArrayReader;
			case BinaryElementType.BoxedPrimitiveTypeValue:
				return sBoxedPrimitiveTypeValue;
			case BinaryElementType.ObjectReference:
				return sObjectReferenceReader;
			case BinaryElementType.NullValue:
				return sNullReader;
			case BinaryElementType.End:
				return sEndReader;
			case BinaryElementType.Assembly:
				return sAssemblyReader;
			case BinaryElementType.ArrayFiller8b:
				return sArrayFiller8bReader;
			case BinaryElementType.ArrayFiller32b:
				return sArrayFiller32bReader;
			case BinaryElementType.ArrayOfPrimitiveType:
				return sPrimitiveArrayReader;
			case BinaryElementType.ArrayOfObject:
				return sObjectArrayReader;
			case BinaryElementType.ArrayOfString:
				return sStringArrayReader;
			case BinaryElementType.MethodCall:
				throw new SerializationException("NYI element type:"+type);
			case BinaryElementType.MethodResponse:
				throw new SerializationException("NYI element type:"+type);

			default:
				throw new SerializationException("NYI element type:"+type);
		}
	}

	public static Object ReadPrimitiveType(DeserializationContext context, BinaryPrimitiveTypeCode typeCode) 
	{
		switch(typeCode) 
		{
			case BinaryPrimitiveTypeCode.Boolean:
				return context.Reader.ReadBoolean();
			case BinaryPrimitiveTypeCode.Byte:
				return context.Reader.ReadByte();
			case BinaryPrimitiveTypeCode.Char:
				return context.Reader.ReadChar();
			case BinaryPrimitiveTypeCode.Decimal:
				return Decimal.Parse(context.Reader.ReadString());
			case BinaryPrimitiveTypeCode.Double:
				return context.Reader.ReadDouble();
			case BinaryPrimitiveTypeCode.Int16:
				return context.Reader.ReadInt16();
			case BinaryPrimitiveTypeCode.Int32:
				return context.Reader.ReadInt32();
			case BinaryPrimitiveTypeCode.Int64:
				return context.Reader.ReadInt64();
			case BinaryPrimitiveTypeCode.SByte:
				return context.Reader.ReadSByte();
			case BinaryPrimitiveTypeCode.Single:
				return context.Reader.ReadSingle();
			case BinaryPrimitiveTypeCode.TimeSpan:
				return new TimeSpan(context.Reader.ReadInt64());
			case BinaryPrimitiveTypeCode.DateTime:
				return new DateTime(context.Reader.ReadInt64());
			case BinaryPrimitiveTypeCode.UInt16:
				return context.Reader.ReadUInt16();
			case BinaryPrimitiveTypeCode.UInt32:
				return context.Reader.ReadUInt32();
			case BinaryPrimitiveTypeCode.UInt64:
				return context.Reader.ReadUInt64();
			case BinaryPrimitiveTypeCode.String:
				return context.Reader.ReadString();

			default:
				throw new SerializationException("unknown primitive type code:"+typeCode);
		}
	}
		
	internal static Array ReadPrimitiveTypeArray(DeserializationContext context, BinaryPrimitiveTypeCode typeCode, int count ) 
	{
		switch(typeCode) 
		{
			case BinaryPrimitiveTypeCode.Boolean:
				bool[] boolArray = new bool[count];
				for(int i = 0; i < count; i++) {
					boolArray[i] = context.Reader.ReadBoolean();
				}
				return boolArray;
	
			case BinaryPrimitiveTypeCode.Byte:
				byte[] byteArray = context.Reader.ReadBytes(count);
				return byteArray;
	
			case BinaryPrimitiveTypeCode.Char:
				char[] charArray = context.Reader.ReadChars(count);
				return charArray;
	
			case BinaryPrimitiveTypeCode.Decimal:
				decimal[] decimalArray = new decimal[count];
				for(int i = 0; i < count; i++) {
					decimalArray[i] = Decimal.Parse(context.Reader.ReadString());
				}
				return decimalArray;
	
			case BinaryPrimitiveTypeCode.Double:
				double[] doubleArray = new double[count];
				for(int i = 0; i < count; i++) {
					doubleArray[i] = context.Reader.ReadDouble();
				}
				return doubleArray;
	
			case BinaryPrimitiveTypeCode.Int16:
				short[] shortArray = new short[count];
				for(int i = 0; i < count; i++) {
					shortArray[i] = context.Reader.ReadInt16();
				}
				return shortArray;
	
			case BinaryPrimitiveTypeCode.Int32:
				int[] intArray = new int[count];
				for(int i = 0; i < count; i++) {
					intArray[i] = context.Reader.ReadInt32();
				}
				return intArray;
	
			case BinaryPrimitiveTypeCode.Int64:
				long[] longArray = new long[count];
				for(int i = 0; i < count; i++) {
					longArray[i] = context.Reader.ReadInt64();
				}
				return longArray;
	
			case BinaryPrimitiveTypeCode.SByte:
				sbyte[] sbyteArray = new sbyte[count];
				for(int i = 0; i < count; i++) {
					sbyteArray[i] = context.Reader.ReadSByte();
				}
				return sbyteArray;
	
			case BinaryPrimitiveTypeCode.Single:
				float[] singleArray = new float[count];
				for(int i = 0; i < count; i++) {
					singleArray[i] = context.Reader.ReadChar();
				}
				return singleArray;
	
			case BinaryPrimitiveTypeCode.TimeSpan:
				TimeSpan[] tsArray = new TimeSpan[count];
				for(int i = 0; i < count; i++) {
					tsArray[i] = new TimeSpan(context.Reader.ReadInt64());
				}
				return tsArray;
	
			case BinaryPrimitiveTypeCode.DateTime:
				DateTime[] dtArray = new DateTime[count];
				for(int i = 0; i < count; i++) {
					dtArray[i] = new DateTime(context.Reader.ReadInt64());
				}
				return dtArray;
	
			case BinaryPrimitiveTypeCode.UInt16:
				ushort[] ushortArray = new ushort[count];
				for(int i = 0; i < count; i++) {
					ushortArray[i] = context.Reader.ReadUInt16();
	
				}
				return ushortArray;
	
			case BinaryPrimitiveTypeCode.UInt32:
				uint[] uintArray = new uint[count];
				for(int i = 0; i < count; i++) {
					uintArray[i] = context.Reader.ReadUInt32();
				}
				return uintArray;
	
			case BinaryPrimitiveTypeCode.UInt64:
				ulong[] ulongArray = new ulong[count];
				for(int i = 0; i < count; i++) {
					ulongArray[i] = context.Reader.ReadUInt64();
				}
				return ulongArray; 
	
			case BinaryPrimitiveTypeCode.String:
				string[] stringArray = new string[count];
				for(int i = 0; i < count; i++) {
					stringArray[i] = context.Reader.ReadString();
				}
				return stringArray;
	
			default:
				throw new SerializationException("unknown primitive type code:"+typeCode);
		}
	}

	public static Type GetPrimitiveType(BinaryPrimitiveTypeCode typeCode)
	{
		switch(typeCode) 
		{
			case BinaryPrimitiveTypeCode.Boolean:
				return typeof(bool);
			case BinaryPrimitiveTypeCode.Byte:
				return typeof(byte);
			case BinaryPrimitiveTypeCode.Char:
				return typeof(char);
			case BinaryPrimitiveTypeCode.Decimal:
				return typeof(decimal);
			case BinaryPrimitiveTypeCode.Double:
				return typeof(double);
			case BinaryPrimitiveTypeCode.Int16:
				return typeof(Int16);
			case BinaryPrimitiveTypeCode.Int32:
				return typeof(Int32);
			case BinaryPrimitiveTypeCode.Int64:
				return typeof(Int64);
			case BinaryPrimitiveTypeCode.SByte:
				return typeof(sbyte);
			case BinaryPrimitiveTypeCode.Single:
				return typeof(Single);
			case BinaryPrimitiveTypeCode.TimeSpan:
				return typeof(TimeSpan);
			case BinaryPrimitiveTypeCode.DateTime:
				return typeof(DateTime);
			case BinaryPrimitiveTypeCode.UInt16:
				return typeof(UInt16);
			case BinaryPrimitiveTypeCode.UInt32:
				return typeof(UInt32);
			case BinaryPrimitiveTypeCode.UInt64:
				return typeof(UInt64);
			case BinaryPrimitiveTypeCode.String:
				return typeof(String);

			default:
				throw new SerializationException("unknown primitive type code:"+typeCode);
		}
	}

	public static Object Deserialize(DeserializationContext context) 
	{
		bool keepGoing = true;
		Object tree = null;

		// TODO: find better solution for finding the root of the object tree
		do 
		{
			Object other;
			keepGoing = ReadValue(context, out other);
			if(tree == null && other != null) 
			{
				tree = other;
			}
		} while(keepGoing);

		return tree;
	}

	public static bool ReadValue(DeserializationContext context, out Object outVal) 
	{
		BinaryElementType element = (BinaryElementType) context.Reader.ReadByte();
		BinaryValueReader reader = GetReader(element);
		return reader.Read(context, out outVal);
	}

	public static TypeSpecification ReadTypeSpec(DeserializationContext context, BinaryTypeTag typeTag) 
	{
		switch(typeTag) 
		{
			case BinaryTypeTag.PrimitiveType:
			case BinaryTypeTag.ArrayOfPrimitiveType:
				BinaryPrimitiveTypeCode typeCode = (BinaryPrimitiveTypeCode) context.Reader.ReadByte();
				switch(typeCode) 
				{
					case BinaryPrimitiveTypeCode.Boolean:
						return sBooleanSpec;
					case BinaryPrimitiveTypeCode.Byte:
						return sByteSpec;
					case BinaryPrimitiveTypeCode.Char:
						return sCharSpec;
					case BinaryPrimitiveTypeCode.Decimal:
						return sDecimalSpec;
					case BinaryPrimitiveTypeCode.Double:
						return sDoubleSpec;
					case BinaryPrimitiveTypeCode.Int16:
						return sInt16Spec;
					case BinaryPrimitiveTypeCode.Int32:
						return sInt32Spec;
					case BinaryPrimitiveTypeCode.Int64:
						return sInt64Spec;
					case BinaryPrimitiveTypeCode.SByte:
						return sSByteSpec;
					case BinaryPrimitiveTypeCode.Single:
						return sSingleSpec;
					case BinaryPrimitiveTypeCode.TimeSpan:
						return sTimeSpanSpec;
					case BinaryPrimitiveTypeCode.DateTime:
						return sDateTimeSpec;
					case BinaryPrimitiveTypeCode.UInt16:
						return sUInt16Spec;
					case BinaryPrimitiveTypeCode.UInt32:
						return sUInt32Spec;
					case BinaryPrimitiveTypeCode.UInt64:
						return sUInt64Spec;
					case BinaryPrimitiveTypeCode.String:
						return sStringArraySpec;
		
					default:
						throw new SerializationException("unknown primitive type code:"+typeCode);
				}
				throw new SerializationException("unknown primitive type code:"+typeCode);

			case BinaryTypeTag.RuntimeType:
				return new TypeSpecification(context.Reader.ReadString());

			case BinaryTypeTag.GenericType:
				String typeName = context.Reader.ReadString();
				uint assId = context.Reader.ReadUInt32();
				return new TypeSpecification(typeName, assId);

            case BinaryTypeTag.String:
                return sStringSpecObject;

            case BinaryTypeTag.ObjectType:
                return sObjectSpec;

            case BinaryTypeTag.ArrayOfString:
                return sStringArraySpec;

            case BinaryTypeTag.ArrayOfObject:
                return sObjectArraySpec;

			default:
				return null;
		}
	}

	public abstract bool Read(DeserializationContext context, out Object outVal);
}

class NullReader : BinaryValueReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		outVal = null;
		return true;
	}
}

class EndReader : BinaryValueReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		context.Manager.DoFixups();
		outVal = null;
		return false;
	}
}

class HeaderReader : BinaryValueReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint id = context.Reader.ReadUInt32();
		
		// check if there is a serialization header
		int hasHeader = context.Reader.ReadInt32();
		if(hasHeader == 2) 
		{
			context.IsHeaderPresent = true;
		} 
		else if(hasHeader == -1) 
		{
			context.IsHeaderPresent = false;
		}
		else
		{
			throw new SerializationException("unknown header specification:"+hasHeader);
		}

		// read major/minor version
		context.MajorVersion = context.Reader.ReadUInt32();
		context.MinorVersion = context.Reader.ReadUInt32();

		outVal = null;
		return true;
	}
}

class StringReader : BinaryValueReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint id = context.Reader.ReadUInt32();
		String str = context.Reader.ReadString();

		context.Manager.RegisterObject(str, id);
		outVal = str;
		return true;
	}
}

abstract class ArrayReader : BinaryValueReader
{
	public bool Read(DeserializationContext context, uint id, uint count, Object type, out Object outVal)
	{
		bool ret = true;

		Array array;

		if(type is BinaryPrimitiveTypeCode) 
		{
			// this is a primitive array
			array = ReadPrimitiveTypeArray(context, (BinaryPrimitiveTypeCode) type, (int) count);
			
		}
		else if(type is Type)
		{
			// this is an object array
			Type convertedType = (Type) type;
			array = Array.CreateInstance(convertedType, (int) count);
			for(int i = 0; i < count; i++) 
			{
				Object val;
				ret &= ReadValue(context, out val);
					
				if(val is DelayedReferenceHolder) 
				{
					// record this index for fixup
					DelayedReferenceHolder holder = (DelayedReferenceHolder) val;
					context.Manager.RecordArrayElementFixup(id, i, holder.ReferenceId);
				} 
				else if(val is ArrayNullValueHolder) 
				{
					ArrayNullValueHolder holder = (ArrayNullValueHolder) val;
					for(int j = 0; j < holder.NumNullValues; j++) 
					{
						array.SetValue(null, i);
						i++;
					}
				}
				else 
				{
					// set this value
					array.SetValue(val, i);
				}
			}
		} 
		else 
		{
			throw new SerializationException("illegal call with:"+type);
		}
			
		context.Manager.RegisterObject(array, id);
		outVal = array;

		return ret;
	}
}

class StringArrayReader : ArrayReader 
{
	public override bool Read(DeserializationContext context, out Object outVal) 
	{
		uint id = context.Reader.ReadUInt32();
		uint count = context.Reader.ReadUInt32();

		return Read(context, id, count, typeof(String), out outVal);
	}
}

class ObjectArrayReader : ArrayReader 
{
	public override bool Read(DeserializationContext context, out Object outVal) 
	{
		uint id = context.Reader.ReadUInt32();
		uint count = context.Reader.ReadUInt32();

		return Read(context, id, count, typeof(Object), out outVal);
	}
}

class PrimitiveArrayReader : ArrayReader 
{
	public override bool Read(DeserializationContext context, out Object outVal) 
	{
		uint id = context.Reader.ReadUInt32();
		uint count = context.Reader.ReadUInt32();

		BinaryPrimitiveTypeCode typeCode = (BinaryPrimitiveTypeCode) context.Reader.ReadByte();
		return Read(context, id, count, typeCode, out outVal);
	}
}

class GenericArrayReader : ArrayReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint id = context.Reader.ReadUInt32();
		uint arrayType = context.Reader.ReadByte();
		uint dimensions = context.Reader.ReadUInt32();

		uint[] dimSizes = new uint[dimensions];
		for(int i = 0; i < dimensions; i++) 
		{
			dimSizes[i] = context.Reader.ReadUInt32();
		}

		// TODO: up to now we only support single dimension arrays
		if(dimensions > 1 || ((BinaryArrayType) arrayType != BinaryArrayType.Single)) 
		{
			throw new SerializationException("array dimmensions > 1 || jagged arrays NYI!");
		}
			
		BinaryTypeTag typeTag = (BinaryTypeTag) context.Reader.ReadByte();
		TypeSpecification typeSpec = ReadTypeSpec(context, typeTag);

		if(typeTag == BinaryTypeTag.PrimitiveType) 
		{
			return Read(context, id, dimSizes[0], typeSpec.GetPrimitiveType(), out outVal);
		} 
		else 
		{
			return Read(context, id, dimSizes[0], typeSpec.GetObjectType(context), out outVal);
		}
	}
}

class AssemblyReader : BinaryValueReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint id = context.Reader.ReadUInt32();
		String name = context.Reader.ReadString();

		// load specified assembly
		Assembly assembly = Assembly.Load(name);
		context.SetAssembly(id, assembly);

		/*
		*  registering an assembly leads to instanciating at fixup which will not work
		*  context.mObjManager.RegisterObject(assembly, id);
		* 
		*  returning the assembly would break the (much to simple!) alg. in Deserialize()
		*  which chooses the first object
		*  outVal = assembly;
		*/
		outVal = null;
		return true;
	}
}

abstract class ObjectReader : BinaryValueReader
{
	public bool Read(DeserializationContext context, out Object outVal, uint id, TypeInfo typeInfo) 
	{
		bool ret = true;

    	// create instance
   		Object obj = null;
        SerializationInfo info = null;
	    
        if(typeInfo.IsISerializable)
        {
            info = typeInfo.GetSerializationInfo();

	    	// create instance
    		obj = FormatterServices.GetUninitializedObject(typeInfo.ObjectType);

	    	// read and set values
    		for(uint i = 0; i < typeInfo.NumMembers; i++) 
		    {
	    		// first get inlined data
    			Object memberValue;
			    if(typeInfo.GetTypeTag(i) == BinaryTypeTag.PrimitiveType) 
		    	{
	    			memberValue = ReadPrimitiveType(context, typeInfo.GetTypeSpecification(i).GetPrimitiveType());
    			} 
			    else 
		    	{
	    			ret &= ReadValue(context, out memberValue);
    			}

    			// set value
			    String field = typeInfo.GetFieldName(i);
		    	if(memberValue is DelayedReferenceHolder) 
	    		{
    				// this is a reference
				    DelayedReferenceHolder holder = (DelayedReferenceHolder) memberValue;
			    	context.Manager.RecordDelayedFixup(id, field, holder.ReferenceId);
		    	}
	    		else 
    			{
    		    	// this is a real value
	    	        info.AddValue(field, memberValue, typeInfo.GetTypeSpecification(i).GetObjectType(context));
	    		}
    		}
    		context.Manager.RegisterObject(obj, id, info);
        }
        else
        {
	    	// create instance
    		obj = FormatterServices.GetUninitializedObject(typeInfo.ObjectType);

	    	// read and set values
    		for(uint i = 0; i < typeInfo.NumMembers; i++) 
		    {
	    		// first get inlined data
    			Object memberValue;
			    if(typeInfo.GetTypeTag(i) == BinaryTypeTag.PrimitiveType) 
		    	{
	    			memberValue = ReadPrimitiveType(context, typeInfo.GetTypeSpecification(i).GetPrimitiveType());
    			} 
			    else 
		    	{
	    			ret &= ReadValue(context, out memberValue);
    			}

    			// set value
			    MemberInfo field = typeInfo.GetMember(i);
		    	if(memberValue is DelayedReferenceHolder) 
	    		{
    				// this is a reference
				    DelayedReferenceHolder holder = (DelayedReferenceHolder) memberValue;
			    	context.Manager.RecordFixup(id, field, holder.ReferenceId);
		    	} 
	    		else 
    			{
		    		// this is a real value
	    			if(field is FieldInfo) 
    				{
			    		FieldInfo fi = (FieldInfo) field;
		    			fi.SetValue(obj, memberValue);
	    			}
    					// TODO: i'm not sure if I have to cover that case, too!
					    // I just noticed that Mono does this
				    	//				else if(field is PropertyInfo)
			    		//				{
		    			//					PropertyInfo pi = (PropertyInfo) field;
	    				//					pi.SetValue();
    				else 
				    {
			    		throw new SerializationException("unknown memeber type:"+field.GetType());
		    		}
	    		}
    		}
    		context.Manager.RegisterObject(obj, id);
        }
		outVal = obj;
		return ret;
	}
}

abstract class FullObjectReader : ObjectReader
{
	public bool Read(DeserializationContext context, out Object outVal, bool external)
	{
		uint id = context.Reader.ReadUInt32();
		String name = context.Reader.ReadString();

		uint fieldCount = context.Reader.ReadUInt32();

		// collect the names of the fields
		String[] fieldNames = new String[fieldCount];
		for(int i = 0; i < fieldCount; i++) 
		{
			fieldNames[i] = context.Reader.ReadString();
		}

		// collect the type-tags of the fields
		BinaryTypeTag[] typeTags = new BinaryTypeTag[fieldCount];
		for(int i = 0; i < fieldCount; i++) 
		{
			typeTags[i] = (BinaryTypeTag) context.Reader.ReadByte();
		}

		// collect the type-specifications of the fields if necessary
		TypeSpecification[] typeSpecs = new TypeSpecification[fieldCount];
		for(int i = 0; i < fieldCount; i++) 
		{
			typeSpecs[i] = ReadTypeSpec(context, typeTags[i]);
		}

		// read assembly-id if this is no runtime object
		Assembly assembly = null;
		if(external) 
		{
			assembly = context.GetAssembly(context.Reader.ReadUInt32());
		}

		// store type-information for later usage
		TypeInfo typeInfo = new TypeInfo(context, name, fieldNames, typeTags, typeSpecs, assembly);
		context.SetTypeInfo(id, typeInfo);

		// let our parent read the inlined values
		return Read(context, out outVal, id, typeInfo);
	}
}

class ExternalObjectReader : FullObjectReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		return Read(context, out outVal, true);
	}
}

class RuntimeObjectReader : FullObjectReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		return Read(context, out outVal, false);
	}
}

class RefObjectReader : ObjectReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint id = context.Reader.ReadUInt32();
		uint refId = context.Reader.ReadUInt32();

		// get type-information from context
		TypeInfo typeInfo = context.GetTypeInfo(refId);

		return Read(context, out outVal, id, typeInfo);
	}
}

class ObjectReferenceReader : BinaryValueReader
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint refId = context.Reader.ReadUInt32();

		// this 'special' object indicates that we haven't read a real object, but will read it later on
		outVal = new DelayedReferenceHolder(refId);
		return true;
	}
}

class ArrayFiller8bReader : BinaryValueReader 
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint numNulls = context.Reader.ReadByte();

		// this 'special' object indicates that we haven't read a real object,
		// but should insert a number of NULL values.
		outVal = new ArrayNullValueHolder(numNulls);
		return true;
	}
}

class ArrayFiller32bReader : BinaryValueReader 
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		uint numNulls = context.Reader.ReadUInt32();

		// this 'special' object indicates that we haven't read a real object,
		// but should insert a number of NULL values.
		outVal = new ArrayNullValueHolder(numNulls);
		return true;
	}
}

class BoxedPrimitiveTypeValue : BinaryValueReader 
{
	public override bool Read(DeserializationContext context, out Object outVal)
	{
		TypeSpecification typeSpec = ReadTypeSpec(context, BinaryTypeTag.PrimitiveType);
		outVal = ReadPrimitiveType(context, typeSpec.GetPrimitiveType());
		return true;
	}
}

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization.Formatters.Binary
