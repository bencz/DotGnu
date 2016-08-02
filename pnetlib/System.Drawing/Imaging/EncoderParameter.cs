/*
 * EncoderParameter.cs - Implementation of the
 *			"System.Drawing.Imaging.EncoderParameter" class.
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

namespace System.Drawing.Imaging
{

#if !ECMA_COMPAT

public sealed class EncoderParameter : IDisposable
{
	// Storage for a tuple of values.
	private sealed class Tuple
	{
		// Accessible state.
		public Object value1;
		public Object value2;
		public Object value3;
		public Object value4;

		// Constructor.
		public Tuple(Object value1, Object value2)
				{
					this.value1 = value1;
					this.value2 = value2;
				}
		public Tuple(Object value1, Object value2,
					 Object value3, Object value4)
				{
					this.value1 = value1;
					this.value2 = value2;
					this.value3 = value3;
					this.value4 = value4;
				}

	}; // class Tuple

	// Internal state.
	private Guid encoder;
	private int numberOfValues;
	private EncoderParameterValueType type;
	private Object value;

	// Constructors.
	public EncoderParameter(Encoder encoder, byte value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = 1;
				this.type = EncoderParameterValueType.ValueTypeByte;
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, byte value, bool undefined)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = 1;
				this.type = (undefined ?
							  EncoderParameterValueType.ValueTypeUndefined :
							  EncoderParameterValueType.ValueTypeByte);
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, byte[] value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = value.Length;
				this.type = EncoderParameterValueType.ValueTypeByte;
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, byte[] value, bool undefined)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = value.Length;
				this.type = (undefined ?
							  EncoderParameterValueType.ValueTypeUndefined :
							  EncoderParameterValueType.ValueTypeByte);
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, short value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = 1;
				this.type = EncoderParameterValueType.ValueTypeShort;
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, short[] value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = value.Length;
				this.type = EncoderParameterValueType.ValueTypeShort;
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, long value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = 1;
				this.type = EncoderParameterValueType.ValueTypeLong;
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, long[] value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = value.Length;
				this.type = EncoderParameterValueType.ValueTypeLong;
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, String value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = value.Length + 1;
				this.type = EncoderParameterValueType.ValueTypeAscii;
				this.value = value;
			}
	public EncoderParameter(Encoder encoder, int numerator, int denominator)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = 1;
				this.type = EncoderParameterValueType.ValueTypeRational;
				this.value = new Tuple(numerator, denominator);
			}
	public EncoderParameter(Encoder encoder, int[] numerator,
							int[] denominator)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = numerator.Length;
				this.type = EncoderParameterValueType.ValueTypeRational;
				this.value = new Tuple(numerator, denominator);
			}
	public EncoderParameter(Encoder encoder, long rangebegin, long rangeend)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = 1;
				this.type = EncoderParameterValueType.ValueTypeLongRange;
				this.value = new Tuple(rangebegin, rangeend);
			}
	public EncoderParameter(Encoder encoder, long[] rangebegin,
							long[] rangeend)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = rangebegin.Length;
				this.type = EncoderParameterValueType.ValueTypeLongRange;
				this.value = new Tuple(rangebegin, rangeend);
			}
	public EncoderParameter(Encoder encoder, int NumberOfValues,
							int Type, int Value)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = NumberOfValues;
				this.type = (EncoderParameterValueType)Type;
				this.value = Value;
			}
	public EncoderParameter(Encoder encoder, int numerator1, int denominator1,
							int numerator2, int denominator2)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = 1;
				this.type = EncoderParameterValueType.ValueTypeRationalRange;
				this.value = new Tuple(numerator1, denominator1,
									   numerator2, denominator2);
			}
	public EncoderParameter(Encoder encoder, int[] numerator1,
							int[] denominator1, int[] numerator2,
							int[] denominator2)
			{
				this.encoder = encoder.Guid;
				this.numberOfValues = numerator1.Length;
				this.type = EncoderParameterValueType.ValueTypeRationalRange;
				this.value = new Tuple(numerator1, denominator1,
									   numerator2, denominator2);
			}

	// Get or set this object's properties.
	public Encoder Encoder
			{
				get
				{
					return new Encoder(encoder);
				}
				set
				{
					encoder = value.Guid;
				}
			}
	public int NumberOfValues
			{
				get
				{
					return numberOfValues;
				}
			}
	public EncoderParameterValueType Type
			{
				get
				{
					return type;
				}
			}
	public EncoderParameterValueType ValueType
			{
				get
				{
					// For some reason, the API defines two ways
					// of obtaining the same value.
					return type;
				}
			}

	// Dispose this object.
	public void Dispose()
			{
				// Nothing to do here in this implementation.
				GC.SuppressFinalize(this);
			}

}; // class EncoderParameter

#endif // !ECMA_COMPAT

}; // namespace System.Drawing.Imaging
