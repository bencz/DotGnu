/*
 * BinaryValueWriter.cs - Implementation of the
 *	"System.Runtime.Serialization.Formatters.Binary.BinaryValueWriter" class.
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

using System.IO;
using System.Reflection;
using System.Collections;
using System.Runtime.Serialization;

internal abstract class BinaryValueWriter
{
	// Builtin writers.
	private static BinaryValueWriter booleanWriter = new BooleanWriter();
	private static BinaryValueWriter byteWriter = new ByteWriter();
	private static BinaryValueWriter sbyteWriter = new SByteWriter();
	private static BinaryValueWriter charWriter = new CharWriter();
	private static BinaryValueWriter int16Writer = new Int16Writer();
	private static BinaryValueWriter uint16Writer = new UInt16Writer();
	private static BinaryValueWriter int32Writer = new Int32Writer();
	private static BinaryValueWriter uint32Writer = new UInt32Writer();
	private static BinaryValueWriter int64Writer = new Int64Writer();
	private static BinaryValueWriter uint64Writer = new UInt64Writer();
	private static BinaryValueWriter singleWriter = new SingleWriter();
	private static BinaryValueWriter doubleWriter = new DoubleWriter();
	private static BinaryValueWriter decimalWriter = new DecimalWriter();
	private static BinaryValueWriter dateTimeWriter = new DateTimeWriter();
	private static BinaryValueWriter timeSpanWriter = new TimeSpanWriter();
	private static BinaryValueWriter stringWriter = new StringWriter();
	private static BinaryValueWriter objectWriter = new ObjectWriter();
	private static BinaryValueWriter infoWriter = new SurrogateWriter(null);
	private static BinaryValueWriter serializableWriter = new ISerializableWriter();
	private static BinaryValueWriter arrayWriter = new ArrayWriter();

	// Context information for writing binary values.
	public class BinaryValueContext
	{
		public BinaryFormatter formatter;
		public BinaryWriter writer;
		public ObjectIDGenerator gen;
		public Queue queue;
		public Queue assemblyQueue;

		// Constructor.
		public BinaryValueContext(BinaryFormatter formatter,
								  BinaryWriter writer)
				{
					this.formatter = formatter;
					this.writer = writer;
					this.gen = new ObjectIDGenerator();
					this.queue = new Queue();
					this.assemblyQueue = new Queue();
				}

		// Process queued objects.
		public void ProcessQueue()
				{
					Assembly assembly;
					bool firstTime;
					long objectID;
					for(;;)
					{
						if(assemblyQueue.Count > 0)
						{
							// Output a pending assembly reference.
							assembly = (assemblyQueue.Dequeue() as Assembly);
							objectID = gen.GetId(assembly, out firstTime);
							writer.Write((byte)(BinaryElementType.Assembly));
							writer.Write((int)objectID);
							WriteAssemblyName(this, assembly);
						}
						else if(queue.Count > 0)
						{
							Object obj = queue.Peek();
							if(OutputAssembly(obj)) 
							{
								formatter.WriteObject(this, queue.Dequeue());
							}
						}
						else
						{
							break;
						}
					}
				}

		public bool OutputAssembly(Object obj)
		{
			bool firstTime;

			Type tp = obj.GetType();
			while(tp.IsArray)
			{
				tp = tp.GetElementType();
			}
			gen.GetId(tp.Assembly, out firstTime);
			if(firstTime) 
			{
				assemblyQueue.Enqueue(tp.Assembly);
			}
			return !firstTime;
		}

	}; // class BinaryValueContext

	// Constructor.
	protected BinaryValueWriter() {}

	// Write the type tag for a type.
	public abstract void WriteTypeTag(BinaryValueContext context, Type type);

	// Write the type specification for a type.
	public abstract void WriteTypeSpec(BinaryValueContext context, Type type);

	// Write the inline form of values for a type.
	public abstract void WriteInline(BinaryValueContext context,
									 Object value, Type type,
									 Type fieldType);

	// Write the object header information for a type.
	public abstract void WriteObjectHeader(BinaryValueContext context,
										   Object value, Type type,
										   long objectID, long prevObject);

	// Write the object form of values for a type.
	public abstract void WriteObject(BinaryValueContext context,
									 Object value, Type type);

	// Get the primitive type code for a type.
	internal static BinaryPrimitiveTypeCode GetPrimitiveTypeCode(Type type)
			{
				if(type == typeof(bool))
				{
					return BinaryPrimitiveTypeCode.Boolean;
				}
				else if(type == typeof(byte))
				{
					return BinaryPrimitiveTypeCode.Byte;
				}
				else if(type == typeof(sbyte))
				{
					return BinaryPrimitiveTypeCode.SByte;
				}
				else if(type == typeof(char))
				{
					return BinaryPrimitiveTypeCode.Char;
				}
				else if(type == typeof(short))
				{
					return BinaryPrimitiveTypeCode.Int16;
				}
				else if(type == typeof(ushort))
				{
					return BinaryPrimitiveTypeCode.UInt16;
				}
				else if(type == typeof(int))
				{
					return BinaryPrimitiveTypeCode.Int32;
				}
				else if(type == typeof(uint))
				{
					return BinaryPrimitiveTypeCode.UInt32;
				}
				else if(type == typeof(long))
				{
					return BinaryPrimitiveTypeCode.Int64;
				}
				else if(type == typeof(ulong))
				{
					return BinaryPrimitiveTypeCode.UInt64;
				}
				else if(type == typeof(float))
				{
					return BinaryPrimitiveTypeCode.Single;
				}
				else if(type == typeof(double))
				{
					return BinaryPrimitiveTypeCode.Double;
				}
				else if(type == typeof(Decimal))
				{
					return BinaryPrimitiveTypeCode.Decimal;
				}
				else if(type == typeof(DateTime))
				{
					return BinaryPrimitiveTypeCode.DateTime;
				}
				else if(type == typeof(TimeSpan))
				{
					return BinaryPrimitiveTypeCode.TimeSpan;
				}
				else if(type == typeof(String))
				{
					return BinaryPrimitiveTypeCode.String;
				}
				else
				{
					return (BinaryPrimitiveTypeCode)0;
				}
			}

	// Get the value writer for a particular type.
	public static BinaryValueWriter GetWriter
				(BinaryValueContext context, Type type)
			{
				BinaryPrimitiveTypeCode code;

				// Handle the primitive types first.
				code = GetPrimitiveTypeCode(type);

				switch(code)
				{
					case BinaryPrimitiveTypeCode.Boolean:
						return booleanWriter;
					case BinaryPrimitiveTypeCode.Byte:
						return byteWriter;
					case BinaryPrimitiveTypeCode.Char:
						return charWriter;
					case BinaryPrimitiveTypeCode.Decimal:
						return decimalWriter;
					case BinaryPrimitiveTypeCode.Double:
						return doubleWriter;
					case BinaryPrimitiveTypeCode.Int16:
						return int16Writer;
					case BinaryPrimitiveTypeCode.Int32:
						return int32Writer;
					case BinaryPrimitiveTypeCode.Int64:
						return int64Writer;
					case BinaryPrimitiveTypeCode.SByte:
						return sbyteWriter;
					case BinaryPrimitiveTypeCode.Single:
						return singleWriter;
					case BinaryPrimitiveTypeCode.TimeSpan:
						return timeSpanWriter;
					case BinaryPrimitiveTypeCode.DateTime:
						return dateTimeWriter;
					case BinaryPrimitiveTypeCode.UInt16:
						return uint16Writer;
					case BinaryPrimitiveTypeCode.UInt32:
						return uint32Writer;
					case BinaryPrimitiveTypeCode.UInt64:
						return uint64Writer;
					case BinaryPrimitiveTypeCode.String:
						return stringWriter;
				}

				// Check for types that implement ISerializable.
				if(typeof(ISerializable).IsAssignableFrom(type))
				{
					return serializableWriter;
				}

				// Handle special types that we recognize.
				if(type == typeof(Object))
				{
					return objectWriter;
				}
				else if(type.IsArray)
				{
					return arrayWriter;
				}

				// Check for surrogates.
				ISurrogateSelector selector;
				ISerializationSurrogate surrogate;
				selector = context.formatter.SurrogateSelector;
				if(selector != null)
				{
					surrogate = selector.GetSurrogate
						(type, context.formatter.Context, out selector);
					if(surrogate != null)
					{
						return new SurrogateWriter(surrogate);
					}
				}

				// Bail out if the type is not marked with the
				// "serializable" flag.
				if(!type.IsSerializable && !type.IsInterface)
				{
					throw new SerializationException
						(String.Format
							(_("Serialize_CannotSerialize"), type));
				}

				// Everything else is handled as an object.
				return objectWriter;
			}

	// Write an assembly name to an output stream.
	private static void WriteAssemblyName(BinaryValueContext context,
										  Assembly assembly)
			{
				String name = assembly.FullName;
				
				if(context.formatter.AssemblyFormat ==
						FormatterAssemblyStyle.Full)
				{
					context.writer.Write(name);
				}
				else
				{
					int index = name.IndexOf(',');
					if(index != -1)
					{
						context.writer.Write(name.Substring(0, index));
					}
					else
					{
						context.writer.Write(name);
					}
				}
			}

	// if a member-name is used in the current class and in a base class, then the name has to be prefixed by <class>+
	private static String GetMemberName(MemberInfo[] allMembers, MemberInfo member)
	{
		bool prefix = false;
		foreach(MemberInfo mi in allMembers)
		{
			if(mi.Name == member.Name && mi.DeclaringType != member.DeclaringType)
			{
				prefix = true;
				break;
			}
		}

		if(prefix)
		{
			return member.DeclaringType.FullName+"+"+member.Name;
		}
		else
		{
			return member.Name;
		}
	}

	// Write object values.
	private class ObjectWriter : BinaryValueWriter
	{
		// Constructor.
		public ObjectWriter() : base() {}

		// Write the type tag for a type.
		public override void WriteTypeTag
					(BinaryValueContext context, Type type)
				{
					if(type == typeof(Object))
					{
						context.writer.Write((byte)(BinaryTypeTag.ObjectType));
					}
					else if(type.Assembly == Assembly.GetExecutingAssembly())
					{
						context.writer.Write((byte)(BinaryTypeTag.RuntimeType));
					}
					else
					{
						context.writer.Write((byte)(BinaryTypeTag.GenericType));
					}
				}

		// Write the type specification for a type.
		public override void WriteTypeSpec
					(BinaryValueContext context, Type type)
				{
					if(type == typeof(Object))
					{
						// Nothing to do here.
					}
					else if(type.Assembly == Assembly.GetExecutingAssembly())
					{
						context.writer.Write(type.FullName);
					}
					else
					{
						bool firstTime;
						long assemblyId;
						assemblyId = context.gen.GetId
							(type.Assembly, out firstTime);
						context.writer.Write(type.FullName);
						context.writer.Write((int)assemblyId);
						if(firstTime)
						{
							// We need to output the assembly later.
							context.assemblyQueue.Enqueue(type.Assembly);
						}
					}
				}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					BinaryPrimitiveTypeCode code;
					BinaryValueWriter vw;
					bool firstTime;
					long objectID;
					long typeID;

					if(value == null)
					{
						// Write a null value.
						context.writer.Write
							((byte)(BinaryElementType.NullValue));
						return;
					}
					else if(type == typeof(String)) {
						stringWriter.WriteInline(context, value, type, fieldType);
						return;
					}
					else if(type.IsValueType)
					{
						if(fieldType.IsValueType)
						{
							// Expand the value instance inline.
							vw = GetWriter(context, type);
							typeID = context.gen.GetIDForType(type);
							objectID = context.gen.GetId(value, out firstTime);
							if(typeID == -1)
							{
								context.gen.RegisterType(type, objectID);
							}
							vw.WriteObjectHeader(context, value, type,
												 objectID, typeID);
							vw.WriteObject(context, value, type);
							return;
						}
						else if((code = GetPrimitiveTypeCode(type)) != 0)
						{
							// This is a boxed primitive value.
							context.writer.Write
								((byte)(BinaryElementType.
											BoxedPrimitiveTypeValue));
							vw = GetWriter(context, type);
							vw.WriteTypeSpec(context, type);
							vw.WriteInline(context, value, type, type);
							return;
						}
					}

					// Queue the object to be expanded later.
					objectID = context.gen.GetId(value, out firstTime);
					context.writer.Write
						((byte)(BinaryElementType.ObjectReference));
					context.writer.Write((int)objectID);
					if(firstTime)
					{
						context.queue.Enqueue(value);
					}
				}

		// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
				{
					if(prevObject == -1)
					{
						// Write the full type information.
						long assemblyID;
						if(type.Assembly == Assembly.GetExecutingAssembly())
						{
							context.writer.Write
								((byte)(BinaryElementType.RuntimeObject));
							assemblyID = -1;
						}
						else
						{
							bool firstTime;
							assemblyID = context.gen.GetId
								(type.Assembly, out firstTime);
							if(firstTime)
							{
								context.writer.Write
									((byte)(BinaryElementType.Assembly));
								context.writer.Write((int)assemblyID);
								WriteAssemblyName(context, type.Assembly);
							}
							context.writer.Write
								((byte)(BinaryElementType.ExternalObject));
						}
						context.writer.Write((int)objectID);
						context.writer.Write(type.FullName);
						MemberInfo[] members =
							FormatterServices.GetSerializableMembers
								(type, context.formatter.Context);
						// write out number of members
						context.writer.Write((int)members.Length);

						int index;
						Type fieldType;
						for(index = 0; index < members.Length; ++index)
						{
							context.writer.Write(GetMemberName(members, members[index]));
						}
						for(index = 0; index < members.Length; ++index)
						{
							if(members[index] is FieldInfo)
							{
								fieldType = ((FieldInfo)(members[index]))
												.FieldType;
							}
							else
							{
								fieldType = ((PropertyInfo)(members[index]))
												.PropertyType;
							}
							GetWriter(context, fieldType).WriteTypeTag
								(context, fieldType);
						}
						for(index = 0; index < members.Length; ++index)
						{
							if(members[index] is FieldInfo)
							{
								fieldType = ((FieldInfo)(members[index]))
												.FieldType;
							}
							else
							{
								fieldType = ((PropertyInfo)(members[index]))
												.PropertyType;
							}
							GetWriter(context, fieldType).WriteTypeSpec
								(context, fieldType);
						}
						if(assemblyID != -1)
						{
							context.writer.Write((int)assemblyID);
						}
					}
					else
					{
						// Write a short header, referring to a previous
						// object's type information.
						context.writer.Write
							((byte)(BinaryElementType.RefTypeObject));
						context.writer.Write((int)objectID);
						context.writer.Write((int)prevObject);
					}
				}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
										 Object value, Type type)
				{
					MemberInfo[] members =
						FormatterServices.GetSerializableMembers
							(type, context.formatter.Context);
					Object[] values =
						FormatterServices.GetObjectData(value, members);
					int index;
					Type fieldType;
					Type valueType;
					for(index = 0; index < members.Length; ++index)
					{
						if(members[index] is FieldInfo)
						{
							fieldType = ((FieldInfo)(members[index]))
											.FieldType;
						}
						else
						{
							fieldType = ((PropertyInfo)(members[index]))
											.PropertyType;
						}
						if(values[index] != null)
						{
							valueType = values[index].GetType();
						}
						else
						{
							valueType = fieldType;
						}
						GetWriter(context, fieldType).WriteInline
							(context, values[index], valueType, fieldType);
					}
				}

	}; // class ObjectWriter

	private class ISerializableWriter : ObjectWriter 
	{
		// Write the object header information for a type.
		// IS11n objects are written like normal objects, but the
		// members are replaced by the key->value pairs from the
		// SerInfo. There is an exception if the Object is serialized
		// using another type (SetType()), then this type is serialized instead
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
				{
					// get members
					StreamingContext streamContext = context.formatter.Context;
					SerializationInfo info = new SerializationInfo(type, context.formatter.converter);
					((ISerializable) value).GetObjectData(info, streamContext);
					Assembly iserAssembly = Assembly.Load(info.AssemblyName);

					if(prevObject == -1)
					{
						// Write the full type information.
						long assemblyID;
						if(iserAssembly == Assembly.GetExecutingAssembly())
						{
							context.writer.Write
								((byte)(BinaryElementType.RuntimeObject));
							assemblyID = -1;
						}
						else
						{
							bool firstTime;
							assemblyID = context.gen.GetId
								(iserAssembly, out firstTime);
							if(firstTime)
							{
								context.writer.Write
									((byte)(BinaryElementType.Assembly));
								context.writer.Write((int)assemblyID);
								WriteAssemblyName(context, iserAssembly);
							}
							context.writer.Write
								((byte)(BinaryElementType.ExternalObject));
						}

						context.writer.Write((int)objectID);
						context.writer.Write(info.FullTypeName);

						// write out number of members
						context.writer.Write((int)info.MemberCount);

						foreach(SerializationEntry entry in info)
						{
							context.writer.Write(entry.Name);
						}

						foreach(SerializationEntry entry in info)
						{
							GetWriter(context, entry.ObjectType).WriteTypeTag
								(context, entry.ObjectType);
						}

						foreach(SerializationEntry entry in info)
						{
							GetWriter(context, entry.ObjectType).WriteTypeSpec
								(context, entry.ObjectType);
						}
						if(assemblyID != -1)
						{
							context.writer.Write((int)assemblyID);
						}
					}
					else
					{
						// Write a short header, referring to a previous
						// object's type information.
						context.writer.Write
							((byte)(BinaryElementType.RefTypeObject));
						context.writer.Write((int)objectID);
						context.writer.Write((int)prevObject);
					}
				}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
			Object value, Type type)
		{
			StreamingContext streamContext = context.formatter.Context;
			SerializationInfo info = new SerializationInfo(type, context.formatter.converter);
			
			((ISerializable) value).GetObjectData(info, streamContext);

			// the entries are written using the type-spec supplied when
			// they were put into the SerInfo, but the writer is determined
			// by the real type!
			foreach(SerializationEntry entry in info)
			{
				Object val = entry.Value;
				if(val != null)
				{
					GetWriter(context, val.GetType()).WriteInline
						(context, val, val.GetType(), entry.ObjectType);
				}
				else
				{
					// NULL is always written as object
					GetWriter(context, typeof(Object)).WriteInline
						(context, val, typeof(Object), entry.ObjectType);
				}
			}
		}
	}

	// Write object values using serialization surrogates.
	private class SurrogateWriter : ObjectWriter
	{
		// Internal state.
		private ISerializationSurrogate surrogate;

		// Constructor.
		public SurrogateWriter(ISerializationSurrogate surrogate)
				{
					this.surrogate = surrogate;
				}

		// Get object data using the prevailing surrogate.
		private SerializationInfo GetObjectData
					(BinaryValueContext context, Object value, Type type)
				{
					SerializationInfo info = new SerializationInfo
						(type, context.formatter.converter);
					if(surrogate == null)
					{
						((ISerializable)value).GetObjectData
							(info, context.formatter.Context);
					}
					else
					{
						surrogate.GetObjectData
							(value, info, context.formatter.Context);
					}
					return info;
				}

		// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
				{
					if(prevObject == -1)
					{
						// Write the full type information.
						long assemblyID;
						if(type.Assembly == Assembly.GetExecutingAssembly())
						{
							context.writer.Write
								((byte)(BinaryElementType.RuntimeObject));
							assemblyID = -1;
						}
						else
						{
							bool firstTime;
							assemblyID = context.gen.GetId
								(type.Assembly, out firstTime);
							if(firstTime)
							{
								context.writer.Write
									((byte)(BinaryElementType.Assembly));
								context.writer.Write((int)assemblyID);
								WriteAssemblyName(context, type.Assembly);
							}
							context.writer.Write
								((byte)(BinaryElementType.ExternalObject));
						}
						context.writer.Write((int)objectID);
						context.writer.Write(type.FullName);
						SerializationInfo info = GetObjectData
							(context, value, type);
						SerializationInfoEnumerator e = info.GetEnumerator();
						Type objectType;
						while(e.MoveNext())
						{
							context.writer.Write(e.Name);
						}
						e.Reset();
						while(e.MoveNext())
						{
							objectType = e.ObjectType;
							GetWriter(context, objectType)
								.WriteTypeTag(context, objectType);
						}
						e.Reset();
						while(e.MoveNext())
						{
							objectType = e.ObjectType;
							GetWriter(context, objectType)
								.WriteTypeSpec(context, objectType);
						}
						if(assemblyID != -1)
						{
							context.writer.Write((int)assemblyID);
						}
					}
					else
					{
						// Write a short header, referring to a previous
						// object's type information.
						context.writer.Write
							((byte)(BinaryElementType.RefTypeObject));
						context.writer.Write((int)objectID);
						context.writer.Write((int)prevObject);
					}
				}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
										 Object value, Type type)
				{
					SerializationInfo info = GetObjectData
						(context, value, type);
					SerializationInfoEnumerator e = info.GetEnumerator();
					Type objectType;
					Type valueType;
					Object fieldValue;
					while(e.MoveNext())
					{
						objectType = e.ObjectType;
						fieldValue = e.Value;
						if(value == null)
						{
							valueType = objectType;
						}
						else
						{
							valueType = fieldValue.GetType();
						}
						GetWriter(context, objectType).WriteInline
							(context, fieldValue, valueType, objectType);
					}
				}

	}; // class SurrogateWriter

	// Write primitive values.
	private abstract class PrimitiveWriter : BinaryValueWriter
	{
		// Internal state.
		private BinaryPrimitiveTypeCode code;
		protected String fieldName;

		// Constructor.
		public PrimitiveWriter(BinaryPrimitiveTypeCode code)
				{
					this.code = code;
					this.fieldName = "m_value";
				}
		public PrimitiveWriter(BinaryPrimitiveTypeCode code, String fieldName)
				{
					this.code = code;
					this.fieldName = fieldName;
				}

		// Write the type tag for a type.
		public override void WriteTypeTag
					(BinaryValueContext context, Type type)
				{
					context.writer.Write((byte)(BinaryTypeTag.PrimitiveType));
				}

		// Write the type specification for a type.
		public override void WriteTypeSpec
					(BinaryValueContext context, Type type)
				{
					context.writer.Write((byte)code);
				}

		// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
				{
					if(prevObject == -1)
					{
						// Write the full type information.
						context.writer.Write
							((byte)(BinaryElementType.RuntimeObject));
						context.writer.Write((int)objectID);
						context.writer.Write(type.FullName);
						context.writer.Write((int)1);
						context.writer.Write(fieldName);
						WriteTypeTag(context, type);
						WriteTypeSpec(context, type);
					}
					else
					{
						// Write a short header, referring to a previous
						// object's type information.
						context.writer.Write
							((byte)(BinaryElementType.RefTypeObject));
						context.writer.Write((int)objectID);
						context.writer.Write((int)prevObject);
					}
				}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
										 Object value, Type type)
				{
					// The object field is just the primitive value itself.
					WriteInline(context, value, type, type);
				}

	}; // class PrimitiveWriter

	// Write boolean values.
	private class BooleanWriter : PrimitiveWriter
	{
		// Constructor.
		public BooleanWriter() : base(BinaryPrimitiveTypeCode.Boolean) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((bool)value);
				}

	}; // class BooleanWriter

	// Write byte values.
	private class ByteWriter : PrimitiveWriter
	{
		// Constructor.
		public ByteWriter() : base(BinaryPrimitiveTypeCode.Byte) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((byte)value);
				}

	}; // class ByteWriter

	// Write sbyte values.
	private class SByteWriter : PrimitiveWriter
	{
		// Constructor.
		public SByteWriter() : base(BinaryPrimitiveTypeCode.SByte) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((sbyte)value);
				}

	}; // class SByteWriter

	// Write char values.
	private class CharWriter : PrimitiveWriter
	{
		// Constructor.
		public CharWriter() : base(BinaryPrimitiveTypeCode.Char) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((char)value);
				}

	}; // class CharWriter

	// Write short values.
	private class Int16Writer : PrimitiveWriter
	{
		// Constructor.
		public Int16Writer() : base(BinaryPrimitiveTypeCode.Int16) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((short)value);
				}

	}; // class Int16Writer

	// Write ushort values.
	private class UInt16Writer : PrimitiveWriter
	{
		// Constructor.
		public UInt16Writer() : base(BinaryPrimitiveTypeCode.UInt16) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((ushort)value);
				}

	}; // class UInt16Writer

	// Write int values.
	private class Int32Writer : PrimitiveWriter
	{
		// Constructor.
		public Int32Writer() : base(BinaryPrimitiveTypeCode.Int32) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((int)value);
				}

	}; // class Int32Writer

	// Write uint values.
	private class UInt32Writer : PrimitiveWriter
	{
		// Constructor.
		public UInt32Writer() : base(BinaryPrimitiveTypeCode.UInt32) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((uint)value);
				}

	}; // class UInt32Writer

	// Write long values.
	private class Int64Writer : PrimitiveWriter
	{
		// Constructor.
		public Int64Writer() : base(BinaryPrimitiveTypeCode.Int64) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((long)value);
				}

	}; // class Int64Writer

	// Write ulong values.
	private class UInt64Writer : PrimitiveWriter
	{
		// Constructor.
		public UInt64Writer() : base(BinaryPrimitiveTypeCode.UInt64) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((ulong)value);
				}

	}; // class UInt64Writer

	// Write float values.
	private class SingleWriter : PrimitiveWriter
	{
		// Constructor.
		public SingleWriter() : base(BinaryPrimitiveTypeCode.Single) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((float)value);
				}

	}; // class SingleWriter

	// Write double values.
	private class DoubleWriter : PrimitiveWriter
	{
		// Constructor.
		public DoubleWriter() : base(BinaryPrimitiveTypeCode.Double) {}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					context.writer.Write((double)value);
				}

	}; // class DoubleWriter

	// Write Decimal values.
	private class DecimalWriter : PrimitiveWriter
	{
		// Constructor.
		public DecimalWriter() : base(BinaryPrimitiveTypeCode.Decimal) {}

		// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
				{
					if(prevObject == -1)
					{
						// Write the full type information.
						context.writer.Write
							((byte)(BinaryElementType.RuntimeObject));
						context.writer.Write((int)objectID);
						context.writer.Write(type.FullName);
						context.writer.Write((int)4);
						context.writer.Write("flags");
						context.writer.Write("hi");
						context.writer.Write("lo");
						context.writer.Write("mid");
						int32Writer.WriteTypeTag(context, null);
						int32Writer.WriteTypeTag(context, null);
						int32Writer.WriteTypeTag(context, null);
						int32Writer.WriteTypeTag(context, null);
						int32Writer.WriteTypeSpec(context, null);
						int32Writer.WriteTypeSpec(context, null);
						int32Writer.WriteTypeSpec(context, null);
						int32Writer.WriteTypeSpec(context, null);
					}
					else
					{
						// Write a short header, referring to a previous
						// object's type information.
						context.writer.Write
							((byte)(BinaryElementType.RefTypeObject));
						context.writer.Write((int)objectID);
						context.writer.Write((int)prevObject);
					}
				}
				// Write the object form of values for a type.
				public override void WriteObject(BinaryValueContext context,
												 Object value, Type type)
				{
					// NOTE: the stupid swapping of the int's is intentional
					Decimal dec = (Decimal) value;
					int[] bits = Decimal.GetBits(dec);
					context.writer.Write(bits[3]);
					context.writer.Write(bits[2]);
					context.writer.Write(bits[0]);
					context.writer.Write(bits[1]);
				}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					// NOTE: it's stupid ... but Bug for bug compatibility
					// was much simpler to write the bits out and read it back
					context.writer.Write(value.ToString());
				}

	}; // class DecimalWriter

	// Write DateTime values.
	private class DateTimeWriter : PrimitiveWriter
	{
		// Constructor.
		public DateTimeWriter()
			: base(BinaryPrimitiveTypeCode.DateTime, "ticks") {}

				// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
												Object value, Type type,
												long objectID, long prevObject)
				{
					if(prevObject == -1)
					{
						// Write the full type information.
						context.writer.Write((byte)(BinaryElementType.RuntimeObject));
						context.writer.Write((int)objectID);
						context.writer.Write(type.FullName);
						context.writer.Write((int)1);
						context.writer.Write(fieldName);
						int64Writer.WriteTypeTag(context, null);
						int64Writer.WriteTypeSpec(context, null);
					}
					else
					{
						// Write a short header, referring to a previous
						// object's type information.
						context.writer.Write((byte)(BinaryElementType.RefTypeObject));
						context.writer.Write((int)objectID);
						context.writer.Write((int)prevObject);
					}
				}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
										 Object value, Type type)
		{
			// this has to be saved as long
			context.writer.Write((long) ((DateTime)value).Ticks);
		}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
		{
			context.writer.Write(((DateTime)value).Ticks);
		}

	}; // class DateTimeWriter

	// Write TimeSpan values.
	private class TimeSpanWriter : PrimitiveWriter
	{
		// Constructor.
		public TimeSpanWriter()
			: base(BinaryPrimitiveTypeCode.TimeSpan, "_ticks") {}

				// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
		{
			if(prevObject == -1)
			{
				// Write the full type information.
				context.writer.Write((byte)(BinaryElementType.RuntimeObject));
				context.writer.Write((int)objectID);
				context.writer.Write(type.FullName);
				context.writer.Write((int)1);
				context.writer.Write(fieldName);
				int64Writer.WriteTypeTag(context, null);
				int64Writer.WriteTypeSpec(context, null);
			}
			else
			{
				// Write a short header, referring to a previous
				// object's type information.
				context.writer.Write((byte)(BinaryElementType.RefTypeObject));
				context.writer.Write((int)objectID);
				context.writer.Write((int)prevObject);
			}
		}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
										 Object value, Type type)
		{
			// this has to be saved as long
			context.writer.Write((long) ((TimeSpan)value).Ticks);
		}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
		{
			context.writer.Write(((TimeSpan)value).Ticks);
		}

	}; // class TimeSpanWriter

	// Write String values.
	private class StringWriter : BinaryValueWriter
	{
		// Constructor.
		public StringWriter() : base() {}

		// Write the type tag for a type.
		public override void WriteTypeTag
					(BinaryValueContext context, Type type)
		{
			context.writer.Write((byte)(BinaryTypeTag.String));
		}

		// Write the type specification for a type.
		public override void WriteTypeSpec
					(BinaryValueContext context, Type type)
		{
			// Nothing to do here.
		}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
		{
			bool firstTime;
			
			if(value == null)
			{
				// Write a null value.
				context.writer.Write((byte)(BinaryElementType.NullValue));
				return;
			} 
			else 
			{
				long objectID = context.gen.GetId(value, out firstTime);
				if(firstTime) 
				{
					context.writer.Write((byte)(BinaryElementType.String));
					context.writer.Write((int)objectID);
					context.writer.Write((String)value);
				} 
				else 
				{
					context.writer.Write
							((byte)(BinaryElementType.ObjectReference));
					context.writer.Write((int)objectID);
				}
			}
		}

		// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
				{
					context.writer.Write((byte)(BinaryElementType.String));
					context.writer.Write((int) objectID);
				}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
										 Object value, Type type)
				{
					if(value == null)
					{
						// Write a null value.
						context.writer.Write
								((byte)(BinaryElementType.NullValue));
					} 
					else 
					{
						context.writer.Write((String)value);
					}
				}

	}; // class StringWriter

	// Write array values.
	private class ArrayWriter : BinaryValueWriter
	{
		// Constructor.
		public ArrayWriter() : base() {}

		// Get the type of an array.
		private BinaryArrayType GetArrayType(Array value, Type type)
				{
					int rank = type.GetArrayRank();
					bool hasLowerBounds = false;
					int dim;
					for(dim = 0; dim < rank; ++dim)
					{
						if(value.GetLowerBound(dim) != 0)
						{
							hasLowerBounds = true;
							break;
						}
					}
					if(type.GetElementType().IsArray)
					{
						if(rank == 1)
						{
							if(hasLowerBounds)
							{
								return BinaryArrayType.JaggedWithLowerBounds;
							}
							else
							{
								return BinaryArrayType.Jagged;
							}
						}
						else
						{
							if(hasLowerBounds)
							{
								return BinaryArrayType.
									MultidimensionalWithLowerBounds;
							}
							else
							{
								return BinaryArrayType.Multidimensional;
							}
						}
					}
					else if(rank == 1)
					{
						if(hasLowerBounds)
						{
							return BinaryArrayType.SingleWithLowerBounds;
						}
						else
						{
							return BinaryArrayType.Single;
						}
					}
					else
					{
						if(hasLowerBounds)
						{
							return BinaryArrayType.
								MultidimensionalWithLowerBounds;
						}
						else
						{
							return BinaryArrayType.Multidimensional;
						}
					}
				}

		// Write the type tag for a type.
		public override void WriteTypeTag
					(BinaryValueContext context, Type type)
				{
					if(type == typeof(Object[]))
					{
						context.writer.Write
							((byte)(BinaryTypeTag.ArrayOfObject));
					}
					else if(type == typeof(String[]))
					{
						context.writer.Write
							((byte)(BinaryTypeTag.ArrayOfString));
					}
					else
					{
						BinaryPrimitiveTypeCode prim;
						prim = GetPrimitiveTypeCode(type.GetElementType());
						if(prim != (BinaryPrimitiveTypeCode)0 &&
						   type.GetArrayRank() == 1)
						{
							context.writer.Write
								((byte)(BinaryTypeTag.ArrayOfPrimitiveType));
						}
						else
						{
							context.writer.Write
								((byte)(BinaryTypeTag.GenericType));
						}
					}
				}

		// Write the type specification for a type.
		public override void WriteTypeSpec
					(BinaryValueContext context, Type type)
				{
					if(type == typeof(Object[]) || type == typeof(String[]))
					{
						return;
					}
					else
					{
						BinaryPrimitiveTypeCode prim;
						prim = GetPrimitiveTypeCode(type.GetElementType());
						if(prim != (BinaryPrimitiveTypeCode)0 &&
						   type.GetArrayRank() == 1)
						{
							context.writer.Write((byte)prim);
						}
						else
						{
							bool firstTime;
							long assemblyId;
							assemblyId = context.gen.GetId
							(type.GetElementType().Assembly, out firstTime);
							context.writer.Write(type.FullName);
							context.writer.Write((int)assemblyId);
							if(firstTime)
							{
								// We need to output the assembly later.
								context.assemblyQueue.Enqueue(type.GetElementType().Assembly);
							}
						}
					}
				}

		// Write the inline form of values for a type.
		public override void WriteInline(BinaryValueContext context,
										 Object value, Type type,
										 Type fieldType)
				{
					bool firstTime;
					long objectID;
					if(value == null)
					{
						// Write a null value.
						context.writer.Write
							((byte)(BinaryElementType.NullValue));
					}
					else
					{
						// Queue the object to be expanded later.
						objectID = context.gen.GetId(value, out firstTime);
						context.writer.Write
							((byte)(BinaryElementType.ObjectReference));
						context.writer.Write((int)objectID);
						if(firstTime)
						{
							context.queue.Enqueue(value);
						}
					}
				}

		// Write the object header information for a type.
		public override void WriteObjectHeader(BinaryValueContext context,
											   Object value, Type type,
											   long objectID, long prevObject)
				{
					BinaryPrimitiveTypeCode prim;
					BinaryArrayType atype;
					prim = GetPrimitiveTypeCode(type.GetElementType());
					atype = GetArrayType((Array)value, type);
					if(type == typeof(Object[]))
					{
						context.writer.Write
							((byte)(BinaryElementType.ArrayOfObject));
						context.writer.Write((int)objectID);
						context.writer.Write(((Array)value).GetLength(0));
					}
					else if(type == typeof(String[]))
					{
						context.writer.Write
							((byte)(BinaryElementType.ArrayOfString));
						context.writer.Write((int)objectID);
						context.writer.Write(((Array)value).GetLength(0));
					}
					else if(prim != (BinaryPrimitiveTypeCode)0 &&
							atype == BinaryArrayType.Single)
					{
						context.writer.Write
							((byte)(BinaryElementType.ArrayOfPrimitiveType));
						context.writer.Write((int)objectID);
						context.writer.Write(((Array)value).GetLength(0));
						context.writer.Write((byte)prim);
					}
					else
					{
						int rank = type.GetArrayRank();
						int dim;
						context.writer.Write
							((byte)(BinaryElementType.GenericArray));
						context.writer.Write((int)objectID);
						context.writer.Write((byte)atype);
						context.writer.Write(rank);
						for(dim = 0; dim < rank; ++dim)
						{
							context.writer.Write
								(((Array)value).GetLength(dim));
						}
						if(((int)atype) >= 3)
						{
							for(dim = 0; dim < rank; ++dim)
							{
								context.writer.Write
									(((Array)value).GetLowerBound(dim));
							}
						}
						BinaryValueWriter vw;
						type = type.GetElementType();
						vw = GetWriter(context, type);
						vw.WriteTypeTag(context, type);
						vw.WriteTypeSpec(context, type);
					}
				}

		// Write the object form of values for a type.
		public override void WriteObject(BinaryValueContext context,
										 Object value, Type type)
				{
					Type elementType = type.GetElementType();

					if(elementType == typeof(Boolean)) 
					{
						foreach(bool elem in (bool[])value)
						{
							context.writer.Write(elem);
						}
					} 
					else if(elementType == typeof(Byte)) 
					{
						context.writer.Write((byte[]) value);
					}
					else if(elementType == typeof(SByte)) 
					{
						/* wish we had macros */
						foreach(sbyte elem in (sbyte[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(Char)) 
					{
						foreach(char elem in (char[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(Int16)) 
					{
						foreach(short elem in (short[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(UInt16))
					{
						foreach(ushort elem in (ushort[])value)
						{
							context.writer.Write(elem);
						}
					} 
					else if(elementType == typeof(Int32)) 
					{
						foreach(int elem in (int[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(UInt32)) 
					{
						foreach(uint elem in (uint[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(Int64)) 
					{
						foreach(long elem in (long[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(UInt64))
					{
						foreach(ulong elem in (ulong[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(Single))
					{
						foreach(float elem in (float[])value)
						{
							context.writer.Write(elem);
						}
					}
					else if(elementType == typeof(Double)) 
					{
						foreach(double elem in (double[])value)
						{
							context.writer.Write(elem);
						}
					}
					else 
					{
						// other arrays are treated with more respect
						Array ar = (Array) value;
						BinaryValueWriter writer = GetWriter(context, elementType);
						if(writer == null)
						{
							throw new SerializationException
								(String.Format
									(_("Serialize_CannotSerialize"), type));
						}
						for(int i = 0; i < ar.GetLength(0); i++) 
						{
							object o = ar.GetValue(i);
							if(o == null)
							{
								// Write a null value.
								context.writer.Write
									((byte)(BinaryElementType.NullValue));
							}
							else
							{
								writer.WriteInline(context, o, o.GetType(), type);
							}
						}
					}
				}
	}; // class ArrayWriter

}; // class BinaryValueWriter

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization.Formatters.Binary
