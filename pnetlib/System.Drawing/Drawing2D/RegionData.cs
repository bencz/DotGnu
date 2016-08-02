/*
 * RegionData.cs - Implementation of the
 *			"System.Drawing.Drawing2D.RegionData" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2005  Free Software Foundation
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

using System.Collections;

namespace System.Drawing.Drawing2D
{

public sealed class RegionData
{

	// Public interface
	public byte[] Data
	{
		get { return data; }
		set	{ data = value; }
	}

	public RegionData Clone() 
	{
		RegionData rd = new RegionData();
		if ( data != null )
		{
			rd.data = (byte[]) data.Clone();
		}
		return rd ;
	}

	// Internal state.
	private byte[] data;

	/*
	 * Flags & Constants
	 */
	// Basic regions
	private const Int32 REG_RECT        = 0x10000000;
	private const Int32 REG_PATH        = 0x10000001;
	private const Int32 REG_EMPTY       = 0x10000002;
	private const Int32 REG_INF         = 0x10000003;
	// Operation codes
	private const Int32 OP_INTERSECT    = 0x00000001;
	private const Int32 OP_UNION        = 0x00000002;
	private const Int32 OP_XOR          = 0x00000003;
	private const Int32 OP_EXCLUDE      = 0x00000004;
	private const Int32 OP_COMPLEMENT   = 0x00000005;
	// Magic number
	private const Int32  _MAGIC_        = unchecked( (Int32) 0xDBC01001 );
	// Format qualifiers for path data
	private const Int32 FMT_SHORT       = 0x00004000;
	private const Int32 FMT_SINGLE      = 0x00000000;
	// Array handling
	private const Int32 HEADER_SIZE     = 0x00000010;
	private const Int32 CAP_MIN         = 0x00000080;
	private const Int32 CAP_MAX         = 0x7FFFFFFF;
	private const Int32 CAP_INC         = 0x00000100;

	static private byte[] dataRectangleF;
	static private byte[] dataEmpty;
	static private byte[] dataInfinite;
	
	// Static constructor to initialize a default dataRectangleF to enhance performance
	static RegionData() {
		dataRectangleF = CreateStaticData( 36, REG_RECT  );
		dataEmpty      = CreateStaticData( 20, REG_EMPTY );
		dataInfinite   = CreateStaticData( 20, REG_INF   );
	}
	

	static byte[] CreateStaticData( int size, int type ) {
		
		byte [] data = new byte[size];
		
		int index = HEADER_SIZE;
		
		int value = type;

		if( BitConverter.IsLittleEndian ) {
			data[index+0] = (byte)(value    );
			data[index+1] = (byte)(value>> 8);
			data[index+2] = (byte)(value>>16);
			data[index+3] = (byte)(value>>24);
		}
		else {
			data[index+3] = (byte)(value    );
			data[index+2] = (byte)(value>> 8);
			data[index+1] = (byte)(value>>16);
			data[index+0] = (byte)(value>>24);
		}
		
		value = size-8;
		
		// WriteHeader( size - 8 , 0 );
		
		if( BitConverter.IsLittleEndian ) {
			data[0] = (byte)(value    );
			data[1] = (byte)(value>> 8);
			data[2] = (byte)(value>>16);
			data[3] = (byte)(value>>24);
		}
		else {
			data[3] = (byte)(value    );
			data[2] = (byte)(value>> 8);
			data[1] = (byte)(value>>16);
			data[0] = (byte)(value>>24);
		}
		
		// TODO Checksum
		data[4] = 0xFF;
		data[5] = 0xFF;
		data[6] = 0xFF;
		data[7] = 0xFF;
		
		// PutInt32( _MAGIC_     ,  8 ) ;
		value = _MAGIC_;
		
		if( BitConverter.IsLittleEndian ) {
			data[ 8] = (byte)(value    );
			data[ 9] = (byte)(value>> 8);
			data[10] = (byte)(value>>16);
			data[11] = (byte)(value>>24);
		}
		else {
			data[11] = (byte)(value    );
			data[10] = (byte)(value>> 8);
			data[ 9] = (byte)(value>>16);
			data[ 8] = (byte)(value>>24);
		}
		
		// PutInt32( _2nrOps     , 12 ) ;
		// PutInt32( 0     , 12 ) ;
		// not needed zero is zero !
		return data;
	}
	/*
	 * Constructors, for internal use ( used by System.Drawing.Region )
	 */
	// Default constructor
	internal RegionData() { }
	// Construct region data for a given rectangle 
	internal RegionData( RectangleF rect ) 
	{
		InitializeRegionData( rect );
	}
	// Construct region data for a given graphic path 
	internal RegionData( GraphicsPath path ) 
	{
		InitializeRegionData( path );
	}

	/*
	   Region Constructor from given RegionData:
	    The region data is parsed 
	    and operations are evaluated
	    on the returned region .
	 */
	internal Region ConstructRegion ( RegionData other ) 
	{
		return Evaluate( other.data ); // other.Data );
	}

	/*
	 * Tests
	 */
	[TODO]
	private void ValidateCheckSum()
	{
		// TODO: validate check sum
		return ;
	}
	// check if we are the infinite region
	internal bool IsInfinite()
	{
		return (( GetInt32( data, 12 ) == 0 ) && 
			    ( GetInt32( data, HEADER_SIZE ) == REG_INF ));
	}
	// check if we are the empty region
	internal bool IsEmpty()
	{
		return (( GetInt32( data, 12 ) == 0 ) && 
				( GetInt32( data, HEADER_SIZE ) == REG_EMPTY ));
	}

	/*
	 * Creational methods
	 */
	private void InitializeRegionData( RectangleF rect ) 
	{
		/*
		int size =  AddRegionRectangle( rect , StartRegionData(36) ) ;
		WriteHeader( size - 8 , 0 );
		Resize( size ) ;
		*/
		// optimize performance by cloning template
		data = (byte[])dataRectangleF.Clone();	
		
		int index = HEADER_SIZE;
		PutSingle( rect.X      , index+=4 );
		PutSingle( rect.Y      , index+=4 );
		PutSingle( rect.Width  , index+=4 );
		PutSingle( rect.Height , index+=4 );
	}
	private void InitializeRegionData( GraphicsPath path ) 
	{
		int size =  AddRegionPath( path , StartRegionData() ) ;
		WriteHeader( size - 8 , 0 );
		Resize( size ) ;
	}
	private void InitializeEmptyRegionData()
	{
		/*
		int size =  AddRegionEmpty( StartRegionData(20) ) ;
		WriteHeader( size - 8 , 0 );
		Resize( size ) ;
		*/
		
		// optimize performance by cloning template
		data = (byte[])dataEmpty.Clone();	
	}
	private void InitializeInfiniteRegionData()
	{
		/*
		int size =  AddRegionInfinite( StartRegionData(20) ) ;
		WriteHeader( size - 8 , 0 );
		Resize( size ) ;
		*/
		
		// optimize performance by cloning template
		data = (byte[])dataInfinite.Clone();	
	}

	/*
	 * Unary Region operations 
	 */
	internal void MakeEmpty ( )
	{
		InitializeEmptyRegionData() ;
	}
	internal void MakeInfinite ( )
	{
		InitializeInfiniteRegionData() ;
	}
	internal void Translate ( float dX, float dY ) 
	{
		if ( ( dX == 0.0f) && ( dY == 0.0f ) )
			return ; // nothing to do
		int size = 8 + GetInt32( data, 0 ) ;
		int nrOps2 = GetInt32( data, 12 ) ;
		int readPointer = HEADER_SIZE ;
		int writePointer = readPointer ;
		while ( readPointer < size ) 
		{
			Token T = NextToken( data, ref readPointer, false );
			Int32 tokenType = GetInt32( T.rawData , 0 );
			switch (tokenType) 
			{
				case REG_RECT:
				{
					RectangleF rect = GetRectangle( T.rawData );
					rect.X += dX ; 
					rect.Y += dY ; 
					writePointer = AddRegionRectangle( rect , writePointer ) ;
				}
				break;
				
				case REG_PATH:
				{
					GraphicsPath path = GetPath( T.rawData );
					for ( int n=0; n<path.PointCount; n++ ) {
						path.PathPoints[n].X += dX ;
						path.PathPoints[n].Y += dY ;
					}
					writePointer = AddRegionPath( path , writePointer ) ;
				}
				break;
				
				default:
				{
					// other tokens are not concerned
					writePointer += T.rawData.Length ;
				}
				break;
			}
		}
		WriteHeader( size - 8 , nrOps2 );
		Resize( size ) ;
	}
	internal void Transform ( Matrix matrix )
	{
		if ( matrix.IsIdentity )
		{
			return ; // bail out
		}
		byte[] oldData = (byte[]) this.data.Clone(); // this.Clone().Data ;
		int oldSize = 8 + GetInt32( oldData, 0 ) ;
		int nrOps2 = GetInt32( oldData, 12 ) ;
		int readPointer =  StartRegionData() ;
		int writePointer = readPointer ;
		float m11 = matrix.Elements[0] ; 
		float m12 = matrix.Elements[1] ; 
		float m21 = matrix.Elements[2] ; 
		float m22 = matrix.Elements[3] ; 
		float  dX = matrix.Elements[4] ; 
		float  dY = matrix.Elements[5] ; 
		while ( readPointer < oldSize ) 
		{
			Token T = NextToken( oldData, ref readPointer, false );
			Int32 tokenType = GetInt32( T.rawData , 0 );
			switch (tokenType) 
			{
				case REG_RECT:
				{
					RectangleF rect = GetRectangle( T.rawData );
					if ( ( m12 == 0.0f) && ( m21 == 0.0f ) ) 
					{
						// in this case, rect transforms to another rectangle
						rect.X = ( m11 < 0 ) ? m11 * ( rect.X + rect.Width ) 
											 : m11 * rect.X ;
						rect.X += dX ; 
						
						/*	there is actually a BUG in Microsoft's .NET 
						 	1.1 implementation:
							for (m22<0) they evaluate rect.Y to be 
							m22*(rect.Y+rect.Height), which cannot be possible 
							by simple geometric reasoning... */

						rect.Y = ( m22 < 0 ) ? m22 * ( rect.Y - rect.Height ) 
											 : m22 * rect.Y ;
						
						rect.Y += dY ; 
						rect.Width = ( m11 < 0 ) ? -m11 * rect.Width 
												 : m11 * rect.Width ;
						
						rect.Height = ( m22 < 0 ) ? -m22 * rect.Height 
												  : m22 * rect.Height ;
						
						writePointer = AddRegionRectangle( rect , writePointer ) ;
					} 
					else 
					{
						// rectangular shape is not preserved -> use a graphics path
						GraphicsPath path = new GraphicsPath() ;
						path.AddRectangle( rect ) ;
						path.Transform( matrix ) ;
						writePointer = AddRegionPath( path , writePointer ) ;
					}
				}
				break;
				
				case REG_PATH:
				{
					GraphicsPath path = GetPath( T.rawData );
					path.Transform( matrix ) ;
					writePointer = AddRegionPath( path , writePointer ) ;
				}
				break;
				
				default:
				{
					// other tokens are not concerned
					writePointer = AddRawData( T.rawData , writePointer ) ;
				}
				break;
			}
		}
		WriteHeader( writePointer - 8 , nrOps2 );
		Resize( writePointer ) ;
	}

	/*
	 * Binary Region operations 
	 */
	internal void Intersect ( RectangleF rect ) 
	{
		if ( IsEmpty() )		    // bail out 
		{
			return;
		}
		else if ( IsInfinite() )	    // return the other region
		{
			InitializeRegionData( rect ) ; 
		}
		else
		{
			RegionOperation( OP_INTERSECT, REG_RECT, rect );
		}
	}
	
	internal void Union ( RectangleF rect ) 
	{
		if ( IsEmpty() )		    // return the other region 
		{
			InitializeRegionData( rect ) ; 
		}
		else if ( IsInfinite() )	    // bail out 
		{
			return;
		}
		else
		{
			RegionOperation( OP_UNION, REG_RECT, rect );
		}
	}
	
	internal void Xor ( RectangleF rect ) 
	{
		if ( IsEmpty() )		    // return the other region
		{
			InitializeRegionData( rect ) ; 
		}
		else
		{
			RegionOperation( OP_XOR, REG_RECT, rect );
		}
	}
	
	internal void Exclude ( RectangleF rect ) 
	{
		if ( IsEmpty() )		    // bail out
		{
			return ; 
		}
		else
		{
			RegionOperation( OP_EXCLUDE, REG_RECT, rect );
		}
	}
	
	internal void Complement ( RectangleF rect ) 
	{
		if ( IsEmpty() )		    // return the other region
		{
			InitializeRegionData( rect ) ; 
		}
		else if ( IsInfinite() )	    // return the empty region 
		{
			InitializeEmptyRegionData() ;
		}
		else 
		{
			RegionOperation( OP_COMPLEMENT, REG_RECT, rect );
		}
	}

	internal void Intersect ( GraphicsPath path ) 
	{
		if ( IsEmpty() )		    // bail out 
		{
			return;
		}
		else if ( IsInfinite() )	    // return the other region 
		{
			InitializeRegionData( path ) ; 
		}
		else
		{
			RegionOperation( OP_INTERSECT, REG_PATH, path );
		}
	}
	
	internal void Union ( GraphicsPath path ) 
	{
		if ( IsEmpty() )		    // return the other region 
		{
			InitializeRegionData( path ) ; 
		}
		else if ( IsInfinite() )	    // bail out 
		{
			return;
		}
		else
		{
			RegionOperation( OP_UNION, REG_PATH, path );
		}
	}
	internal void Xor ( GraphicsPath path ) {
		if ( IsEmpty() )		    // return the other region
			InitializeRegionData( path ) ; 
		else
			RegionOperation( OP_XOR, REG_PATH, path );
	}
	
	internal void Exclude ( GraphicsPath path ) 
	{
		if ( IsEmpty() )		    // bail out
		{
			return ; 
		}
		else
		{
			RegionOperation( OP_EXCLUDE, REG_PATH, path );
		}
	}
	
	internal void Complement ( GraphicsPath path ) 
	{
		if ( IsEmpty() )		    // return the other region
		{
			InitializeRegionData( path ) ; 
		}
		else if ( IsInfinite() )	    // return the empty region 
		{
			InitializeEmptyRegionData() ;
		}
		else 
		{
			RegionOperation( OP_COMPLEMENT, REG_PATH, path );
		}
	}
	
	internal void Intersect ( Region other ) 
	{
		if ( IsEmpty() )			       // bail out 
		{
			return;
		}
		
		RegionData otherdata = other.GetRegionData();
		
		if ( otherdata.IsEmpty() )    // return the empty region 
		{
			InitializeEmptyRegionData() ;
		}
		else if ( IsInfinite() )		       // return the other region 
		{
			data = otherdata.data;
		}
		else if ( otherdata.IsInfinite() ) // bail out
		{
			return;
		}
		else
		{
			RegionOperation( OP_INTERSECT, -1, other );
		}
	}
	
	internal void Union ( Region other ) 
	{
		if ( IsEmpty() )			       // return the other region
		{
			data = other.GetRegionData().Data ;
		}
		else if ( other.GetRegionData().IsEmpty() )    // bail out
		{
			return ;
		}
		else if ( IsInfinite() )		       // bail out
		{
			return ;
		}
		else if ( other.GetRegionData().IsInfinite() ) // return the infinite region
		{
			InitializeInfiniteRegionData() ;
		}
		else
		{
			RegionOperation( OP_UNION, -1, other );
		}
	}
	
	internal void Xor ( Region other ) 
	{
		if ( IsEmpty() )			       // return the other region
		{
			data = other.GetRegionData().Data ;
		}
		else if ( other.GetRegionData().IsEmpty() )    // bail out
		{
			return ;
		}
		else
		{
			RegionOperation( OP_XOR, -1, other );
		}
	}
	internal void Exclude ( Region other ) {
		if ( IsEmpty() )				// bail out
		{
			return ; 
		}
		
		if ( other.GetRegionData().IsEmpty() )     // bail out
		{
			return;
		}
		else if ( other.GetRegionData().IsInfinite() )  // return the empty region
		{
			InitializeEmptyRegionData() ;
		}
		else
		{
			RegionOperation( OP_EXCLUDE, -1, other );
		}
	}
	internal void Complement ( Region other ) {
		
		if ( IsEmpty() )			      // return the other region
		{
			data = other.GetRegionData().Data ;
		}
		else if ( other.GetRegionData().IsEmpty() )   // return the empty region
		{
			InitializeEmptyRegionData() ;
		}
		else if ( IsInfinite() )		      // return the empty region 
		{
			InitializeEmptyRegionData() ;
		}
		else 
		{
			RegionOperation( OP_COMPLEMENT, -1, other );
		}
	}
	private void RegionOperation( int opcode, int regtype, Object regionObject )
	{
		// get current twice number of operations 
		int nrOps2 =  GetInt32( data, 12 ) ;
		// skip header from current data
		int size = data.Length - HEADER_SIZE;
		byte[] bytes = new byte[size];
		Array.Copy( data , HEADER_SIZE, bytes , 0, size );
		// create a fresh region data
		int index = StartRegionData(size+40); // need size+40 bytes most time
		// insert the new op code
		PutInt32( opcode, index );    
		index+=4;
		// add old data
		EnsureCapacity( index + size ) ;
		Array.Copy( bytes, 0, data, index, size );
		index+=size;
		// insert new data, update twice number of operations 
		switch (regtype) 
		{
			case REG_RECT:
			{
				index = AddRegionRectangle( (RectangleF) regionObject , index );
				nrOps2 += 2 ;
			}
			break;
			case REG_PATH:
			{
				index = AddRegionPath( (GraphicsPath) regionObject , index );
				nrOps2 += 2 ;
			}
			break;
			default:
			{
				index = AddRegion( (Region) regionObject ,index );
				nrOps2 += ( 2 + GetInt32( ((Region) regionObject).GetRegionData().data , 12 ) );
			}
			break;
		}
		// finish
		WriteHeader( index - 8 , nrOps2 ) ;
		Resize ( index ) ;
	}

	/*
	 * IO for primitive data types
	 */
	// Put a byte at specified index
	private void PutByte( Byte value, int index )
	{
		data[index] = value ;
	}
	// Read a byte from specified index
	private static Byte GetByte( byte[] bts, int index )
	{
		return bts[index] ;
	}
	// Put an unsigned short at specified index
	private void PutInt16( Int16 value, int index )
	{
		if( BitConverter.IsLittleEndian ) {
			data[index+0] = (byte)(value    );
			data[index+1] = (byte)(value>> 8);
		}
		else {
			data[index+1] = (byte)(value    );
			data[index+0] = (byte)(value>> 8);
		}
		/*
		not using BitConverter enhances performance
		byte[] bytes = BitConverter.GetBytes( value ) ;
		for ( ushort s = 0; s<2 ; s++ )
		{
			data[index+s] = bytes[s] ;
		}
		*/
	}
	// Read an unsigned short from specified index
	private static Int16 GetInt16( byte[] bts, int index )
	{
		if(BitConverter.IsLittleEndian)
		{
			return (short)(((int)(bts[index])) |
							(((int)(bts[index + 1])) << 8));
		}
		else
		{
			return (short)(((int)(bts[index + 1])) |
							(((int)(bts[index])) << 8));
		}
		
		// not using BitConverter enhances performance
		// return BitConverter.ToInt16( bts , index ) ;
	}
	// Put an unsigned integer at specified index
	private void PutInt32( Int32 value, int index )
	{
		if( BitConverter.IsLittleEndian ) {
			data[index+0] = (byte)(value    );
			data[index+1] = (byte)(value>> 8);
			data[index+2] = (byte)(value>>16);
			data[index+3] = (byte)(value>>24);
		}
		else {
			data[index+3] = (byte)(value    );
			data[index+2] = (byte)(value>> 8);
			data[index+1] = (byte)(value>>16);
			data[index+0] = (byte)(value>>24);
		}
		/*
		not using BitConverter enhances performance
		byte[] bytes = BitConverter.GetBytes( value ) ;
		for ( ushort s = 0; s<4 ; s++ )
		{
			data[index+s] = bytes[s] ;
		}
		*/
	}
	// Read an unsigned integer from specified index
	private static Int32 GetInt32( byte[] bts, int index )
	{
		if(BitConverter.IsLittleEndian)
		{
			return (((int)(bts[index])) |
					(((int)(bts[index + 1])) << 8) |
					(((int)(bts[index + 2])) << 16) |
					(((int)(bts[index + 3])) << 24));
		}
		else
		{
			return (((int)(bts[index + 3])) |
					(((int)(bts[index + 2])) << 8) |
					(((int)(bts[index + 1])) << 16) |
					(((int)(bts[index])) << 24));
		}
		// not using BitConverter enhances performance
		// return BitConverter.ToInt32( bts , index ) ;
	}
	
	// Put a single precision floating point at specified index using
	//  IEEE floating point standard representation
	private void PutSingle( Single value, int index )
	{
		byte[] bytes = BitConverter.GetBytes( value ) ;
		for ( ushort s = 0; s<4 ; s++ )
		{
			data[index+s] = bytes[s] ;
		}
	}
	
	// Read a single precision floating point from specified index 
	private static Single GetSingle( byte[] bts, int index )
	{
		return BitConverter.ToSingle( bts , index ) ;
	}


	/*
	 * Private methods for updating the internal state
	 */
	[TODO]
		private Int32 CheckSum() 
		{
			// TODO: compute check sum
			return unchecked( (Int32) 0xFFFFFFFF );
		}
	/*
	 * Tha data header format:
	 *   index    description		     format
	 *   -----------------------------------------------------
	 *   00-03    size after checksum	     Int32
	 *   04-07    checksum ??		     ??      TODO
	 *   08-11    magic number		    Int32
	 *   12-15    2 * (nr. of bin. operations)    Int32
	 */
	private void WriteHeader( int size, Int32 _2nrOps )
	{
		PutInt32( size	,  0 ) ;
		PutInt32( CheckSum()  ,  4 ) ;
		PutInt32( _MAGIC_     ,  8 ) ;
		PutInt32( _2nrOps     , 12 ) ;
	}
	private void Resize( int size ) 
	{
		if ( size != data.Length ) {
			byte[] bytes = new byte[ size ];
			Array.Copy( data , bytes , size );
			data = bytes ;
		}
	}
	private void EnsureCapacity( int newsize )
	{
		uint oldcap = (uint) data.Length ;
		if( newsize <= oldcap ) { // check before calculating new cap
			return; 
		}
		uint newcap = (uint) newsize + (uint) CAP_INC - (uint) ( newsize % CAP_INC ) ;
		if ( newcap <= oldcap )
		{
			return;
		}
		if ( newcap > CAP_MAX )
		{
			throw new OverflowException( "array too large" );
		}
		byte[] bytes = new byte[ newcap ];
		Array.Copy( data , bytes , oldcap );
		data = bytes ;
	}
	// Start a new region data.
	// Returns next free write index.
	private int StartRegionData()
	{
		data = new byte[CAP_MIN];
		return HEADER_SIZE ;
	}
	// Start a new region data with specifix size.
	// Returns next free write index.
	private int StartRegionData(int size)
	{
		data = new byte[size];
		return HEADER_SIZE ;
	}
	// Add a rectangle region at the given index.
	// Returns next free write index.
	private int AddRegionRectangle( RectangleF rect , int index ) 
	{ 
		EnsureCapacity( index + 20 );
		PutInt32 ( REG_RECT    , index    );
		PutSingle( rect.X      , index+=4 );
		PutSingle( rect.Y      , index+=4 );
		PutSingle( rect.Width  , index+=4 );
		PutSingle( rect.Height , index+=4 );
		return index+=4 ;
	}
	// Add a path region at s given index.
	// Returns next free write index.
	private int AddRegionPath( GraphicsPath path , int index ) 
	{ 
		bool fmtShort = HasSimpleFormat( path );
		int nrPoints = path.PointCount ;
		int pathSize = nrPoints * ( fmtShort ? 5 : 9 ) + 12 ;
		EnsureCapacity( index + 8 + pathSize );
		PutInt32( REG_PATH , index    );
		PutInt32( pathSize , index+=4 );
		PutInt32( _MAGIC_  , index+=4 );
		PutInt32( nrPoints , index+=4 );
		PutInt32( fmtShort ? FMT_SHORT : FMT_SINGLE , index+=4 );
		if ( fmtShort ) 
		{
			index+=2;
			for ( int i=0; i<nrPoints; i++ ) 
			{
				PutInt16( (Int16) path.PathPoints[i].X , index+=2 );
				PutInt16( (Int16) path.PathPoints[i].Y , index+=2 );
			}
			index+=1;
		} 
		else 
		{
			for ( int i=0; i<nrPoints; i++ ) 
			{
				PutSingle( path.PathPoints[i].X , index+=4 );
				PutSingle( path.PathPoints[i].Y , index+=4 );
			}
			index+=3;
		}
		for ( int i=0; i<nrPoints; i++ ) 
		{
			PutByte( path.PathTypes[i] , ++index ) ;
		}
		return index+=1;
	}
	// Add the empty region at a given index.
	// Returns next free write index.
	private int AddRegionEmpty( int index ) 
	{ 
		EnsureCapacity( index + 4 );
		PutInt32( REG_EMPTY , index );
		return index + 4 ;
	}
	// Add the infinite region at a given index.
	// Returns next free write index.
	private int AddRegionInfinite( int index ) 
	{ 
		EnsureCapacity( index + 4 );
		PutInt32( REG_INF , index );
		return index + 4 ;
	}
	// Add another region at a given index.
	// Returns next free write index.
	private int AddRegion( Region region , int index ) 
	{ 
		byte[] other = region.GetRegionData().data ; // use member direct, region.GetRegionData().Data ;
		int size = other.Length - HEADER_SIZE ;
		EnsureCapacity( index + size );
		Array.Copy( other, HEADER_SIZE, data, index, size );
		return index + size ;
	}
	// Add a chunk of raw data at a given index.
	// Returns next free write index.
	private int AddRawData( byte[] raw , int index ) 
	{ 
		int size = raw.Length ;
		EnsureCapacity( index + size );
		Array.Copy( raw, 0, data, index, size ) ;
		return index + size ;
	}
	// If we can cast all coordinates to short without loosing in precision, 
	//  then we store points in short format
	private bool HasSimpleFormat( GraphicsPath path )
	{
		foreach ( PointF p in path.PathPoints ) 
		{
			if ( ( p.X != (Int16) p.X ) || ( p.Y != (Int16) p.Y ) )
			{
				return false ;
			}
		}
		return true ;
	}
	// Returns a basic Region from a given raw byte representation
	private Region GetRegion( byte[] raw )
	{
		Int32 regtype = GetInt32( raw , 0 );
		Region region = null;
		switch (regtype) {
			case REG_RECT:
			{
				region = new Region( GetRectangle( raw ) );
			}
			break;
			case REG_PATH:
			{
				region = new Region( GetPath( raw ) );
			}
			break;
			case REG_EMPTY:
			{
				region = new Region();
				region.MakeEmpty() ;
			}
			break;
			case REG_INF:
			{
				region = new Region();
				region.MakeInfinite() ;
			}
			break;
			default:
				// is not a region
				break;
		}
		return region ;
	}
	// Returns a rectangle structure from a given raw byte representation
	private RectangleF GetRectangle( byte[] raw ) 
	{ 
		int index = 0;
		Single X = GetSingle( raw, index+=4 );
		Single Y = GetSingle( raw, index+=4 );
		Single W = GetSingle( raw, index+=4 );
		Single H = GetSingle( raw, index+=4 );
		return new RectangleF( X, Y, W, H );
	}
	// Returns a graphics path from a given raw byte representation
	private GraphicsPath GetPath( byte[] raw ) 
	{ 
		int nrPoints = GetInt32( raw , 12 ) ;
		PointF[] points = new PointF[ nrPoints ] ;
		byte[] types = new byte[ nrPoints ] ;
		int format = GetInt32( raw , 16 ) ;
		int index = 16 ;
		if ( format == FMT_SHORT ) 
		{
			Int16 X , Y ;
			index+=2 ;
			for ( int n=0; n<nrPoints; n++ ) 
			{
				X = GetInt16( raw , index+=2 ) ;
				Y = GetInt16( raw , index+=2 ) ;
				points[n] = new PointF( (Single) X , (Single) Y );
			}
			index+=1 ;
		} 
		else 
		{
			Single X , Y ;
			for ( int n=0; n<nrPoints; n++ ) 
			{
				X = GetSingle( raw , index+=4 ) ;
				Y = GetSingle( raw , index+=4 ) ;
				points[n] = new PointF( X , Y ) ;
			}
			index+=3 ;
		}
		for ( int n=0; n<nrPoints; n++ )
		{
			types[n] = GetByte( raw, index+=1 ) ;
		}
		return new GraphicsPath( points , types ) ;
	}

	/*
	 * Parses a region data and evaluates all binary operations. 
	 * Returns a region with all operations applied.
	 */
	private Region Evaluate ( byte[] regData )
	{
		int dataSize = GetInt32( regData, 0 ) + 8 ;
		int nrOps = GetInt32( regData, 12 ) / 2 ;
		int pointer =  HEADER_SIZE ;
		Token T = new Token();
		Stack stack = new Stack();
		while ( pointer < dataSize ) 
		{
			T = NextToken( regData, ref pointer, true );
			if ( T.Type == Token.OP )
			{
				stack.Push( T );
			}
			else 
			{
				if ( ( stack.Count == 0 ) || ( ((Token) stack.Peek()).Type == Token.OP ) )
				{
					stack.Push( T );
				}
				else 
				{
					while ( ( nrOps > 0 ) &&
							( ((Token) stack.Peek()).Type == Token.DATA ) ) 
					{
						Token S = (Token) stack.Pop() ;
						Token O = (Token) stack.Pop() ;
						T = O.Eval( S, T );
						nrOps-- ;
					}
					stack.Push( T );
				}
			}
		}
		return T.region ;
	}
	private class Token 
	{
		public const int OP	  = 1;
		public const int DATA	= 2;

		// Type is OP or DATA
		public int Type;

		// is an op code for Type=OP;
		// is a region data WITHOUT header for Type=DATA
		public byte[] rawData;

		// associated region, for DATA Token only
		public Region region = new Region();

		private Int32 GetOpCode()
		{
			if ( this.Type != Token.OP ) 
				throw new InvalidOperationException();
			return GetInt32( this.rawData, 0 );
		}

		public Token Eval( Token A, Token B ) 
		{
			if ( ! ( ( this.Type == Token.OP ) && 
					( A.Type == Token.DATA ) && ( B.Type == Token.DATA ) ) )
			{
				throw new InvalidOperationException();
			}
			switch ( GetOpCode() ) 
			{
				case OP_INTERSECT   :
				{
					A.region.Intersect( B.region ) ;
				}
				break;
				case OP_UNION       :
				{
					A.region.Union( B.region ) ;
				}
				break;
				case OP_XOR	 :
				{
					A.region.Xor( B.region ) ;
				}
				break;
				case OP_EXCLUDE     :
				{
					A.region.Exclude( B.region ) ;
				}
				break;
				case OP_COMPLEMENT  :
				{
					A.region.Complement( B.region ) ;
				}
				break;
				default:
				{
					throw new InvalidOperationException();
				}
				break;
			}
			Token c = new Token();
			c.Type = Token.DATA;
			c.region = A.region;
			
			RegionData regdata = A.region.GetRegionData();
			int size = GetInt32( regdata.data, 0 ) - 8 ;
			c.rawData = new byte[size] ;
			Array.Copy( regdata.data, 
						HEADER_SIZE, c.rawData, 0, size ) ;
			return c;
		}
	}
	// Returns the next token from a raw data stream.
	// Adjusts the read pointer "ref int index".
	private Token NextToken( byte[] regdat, ref int index, bool withRegion ) 
	{
		Token tok = new Token();
		int size = 4;
		Int32 lookahead = GetInt32( regdat, index );

		switch( lookahead ) {
		
			case OP_INTERSECT :
			case OP_UNION :
			case OP_XOR :
			case OP_EXCLUDE :
			case OP_COMPLEMENT :
				tok.Type = Token.OP;
				break;
				
			case REG_RECT :
				tok.Type = Token.DATA;
				size = 20 ;
				break;
		
			case REG_PATH :
				tok.Type = Token.DATA;
				size = 8 + GetInt32( regdat, index+4 );
				break;
		
			case REG_EMPTY :
			case REG_INF :
				tok.Type = Token.DATA;
				break;
		
			default :
				throw new ArgumentException( "invalid token" );
		}
		
		tok.rawData = new byte[ size ];
		Array.Copy( regdat, index, tok.rawData, 0, size );
		if ( withRegion )
		{
			tok.region = GetRegion( tok.rawData ) ;
		}
		index+=size ;
		return tok;
	}

}; // class RegionData

}; // namespace System.Drawing.Drawing2D
