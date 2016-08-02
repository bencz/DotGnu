/*
 * SymMethod.cs - Implementation of the
 *			"System.Diagnostics.SymbolStore.SymMethod" class.
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

namespace System.Diagnostics.SymbolStore
{

#if CONFIG_EXTENDED_DIAGNOSTICS

using System.Collections;

public class SymMethod : ISymbolMethod
{
	// Internal state.
	private unsafe ISymUnmanagedMethod *pMethod;
	private SymReader reader;
	private int token;
	private SymScope rootScope;
	private ArrayList sequencePoints;

	// Information about a sequence point.
	private class SequencePoint
	{
		// Accessible internal state.
		public int offset;
		public ISymbolDocument document;
		public int line;
		public int column;

		// Constructor.
		public SequencePoint(int offset, ISymbolDocument document,
							 int line, int column)
				{
					this.offset = offset;
					this.document = document;
					this.line = line;
					this.column = column;
				}

	}; // class SequencePoint

	// Comparison class for sorting on ascending offset.
	private class SequencePointComparer : IComparer
	{
		// Compare two sequence points.
		public int Compare(Object x, Object y)
				{
					return ((SequencePoint)x).offset -
						   ((SequencePoint)y).offset;
				}

	}; // class SequencePointComparer

	// Constructors.
	public unsafe SymMethod(ISymUnmanagedMethod *pMethod)
			{
				this.pMethod = pMethod;
			}
	internal SymMethod(SymReader reader, int token)
			{
				this.reader = reader;
				this.token = token;
			}

	// Destructor (C++ style).
	~SymMethod() {}
	public void __dtor()
			{
				GC.SuppressFinalize(this);
				Finalize();
			}

	// Implement the ISymbolMethod interface.
	public virtual ISymbolNamespace GetNamespace()
			{
				throw new NotSupportedException();
			}
	public virtual int GetOffset
				(ISymbolDocument document, int line, int column)
			{
				// Load the sequence point information.
				LoadSequencePoints();

				// Search for an applicable block, based on the line number.
				int index;
				SequencePoint seq1;
				SequencePoint seq2;
				for(index = 0; index < sequencePoints.Count; ++index)
				{
					seq1 = (SequencePoint)(sequencePoints[index]);
					if(line >= seq1.line)
					{
						if(index < sequencePoints.Count - 1)
						{
							seq2 = (SequencePoint)(sequencePoints[index + 1]);
							if(line < seq2.line)
							{
								if(document == null || seq1.document == null ||
								   document.URL == seq1.document.URL)
								{
									return seq1.offset;
								}
							}
						}
						else
						{
							if(document == null || seq1.document == null ||
							   document.URL == seq1.document.URL)
							{
								return seq1.offset;
							}
						}
					}
				}

				// Could not find the offset, so assume offset zero.
				return 0;
			}
	public virtual ISymbolVariable[] GetParameters()
			{
				// We don't store debug symbol information for
				// parameters in this implementation, because
				// parameter data can be retrieved from metadata.
				return new ISymbolVariable [0];
			}
	public virtual int[] GetRanges
				(ISymbolDocument document, int line, int column)
			{
				// Not used in this implementation.
				return new int [0];
			}
	public virtual ISymbolScope GetScope(int offset)
			{
				SymScope scope = (SymScope)RootScope;
				if(scope != null)
				{
					return scope.FindOffset(offset);
				}
				else
				{
					return null;
				}
			}
	public virtual void GetSequencePoints
					(int[] offsets, ISymbolDocument[] documents, 
					 int[] lines, int[] columns, int[] endLines, 
					 int[] endColumns)
			{
				int index;
				LoadSequencePoints();
				for(index = 0; index < sequencePoints.Count; ++index)
				{
					if(offsets != null && index < offsets.Length)
					{
						offsets[index] =
							((SequencePoint)(sequencePoints[index])).offset;
					}
					if(documents != null && index < documents.Length)
					{
						documents[index] =
							((SequencePoint)(sequencePoints[index])).document;
					}
					if(lines != null && index < lines.Length)
					{
						lines[index] = 
							((SequencePoint)(sequencePoints[index])).line;
					}
					if(columns != null && index < columns.Length)
					{
						columns[index] =
							((SequencePoint)(sequencePoints[index])).column;
					}
					if(endLines != null && index < endLines.Length)
					{
						// This information is not available.
						endLines[index] = 0;
					}
					if(endColumns != null && index < endColumns.Length)
					{
						// This information is not available.
						endColumns[index] = 0;
					}
				}
			}
	public virtual bool GetSourceStartEnd
					(ISymbolDocument[] docs, int[] lines, int[] columns)
			{
				throw new NotSupportedException();
			}
	public virtual ISymbolScope RootScope 
			{
 				get
				{
					if(rootScope == null)
					{
						rootScope = new SymScope(this);
						LoadVariables();
					}
					return rootScope;
				}
 			}
	public virtual int SequencePointCount 
			{
 				get
				{
					LoadSequencePoints();
					return sequencePoints.Count;
				}
 			}
	public virtual SymbolToken Token 
			{
 				get
				{
					return new SymbolToken(token);
				}
 			}

	// Load the local variable information.
	private void LoadVariables()
			{
				SymInfoEnumerator e;
				int nameIndex, index;
				int start, end;
				SymScope childScope;
				e = new SymInfoEnumerator(reader, token);
				while(e.MoveNext())
				{
					if(e.Type == SymReader.DataType_LocalVariables)
					{
						// Local variables within the root scope.
						while((nameIndex = e.GetNextInt()) != -1)
						{
							index = e.GetNextInt();
							rootScope.AddLocal
								(reader.ReadString(nameIndex), index);
						}
					}
					else if(e.Type == SymReader.DataType_LocalVariablesOffsets)
					{
						// Local variables within a child scope.
						start = e.GetNextInt();
						end = e.GetNextInt();
						childScope = rootScope.FindScope(end, start);
						while((nameIndex = e.GetNextInt()) != -1)
						{
							index = e.GetNextInt();
							childScope.AddLocal
								(reader.ReadString(nameIndex), index);
						}
					}
				}
			}

	// Load the sequence point information.
	private void LoadSequencePoints()
			{
				SymInfoEnumerator e;
				String filename;
				ISymbolDocument document;
				int line, column, offset;

				// Bail out if we have already loaded the sequence points.
				if(sequencePoints != null)
				{
					return;
				}

				// Create the sequence point list.
				sequencePoints = new ArrayList();

				// Load the sequence point information.
				e = new SymInfoEnumerator(reader, token);
				while(e.MoveNext())
				{
					if(e.Type == SymReader.DataType_LineColumn)
					{
						// Block contains line and column values only.
						filename = reader.ReadString(e.GetNextInt());
						document = reader.GetDocument(filename);
						while((line = e.GetNextInt()) != -1)
						{
							column = e.GetNextInt();
							sequencePoints.Add
								(new SequencePoint(0, document, line, column));
						}
					}
					else if(e.Type == SymReader.DataType_LineOffsets)
					{
						// Block contains line and offset values only.
						filename = reader.ReadString(e.GetNextInt());
						document = reader.GetDocument(filename);
						while((line = e.GetNextInt()) != -1)
						{
							offset = e.GetNextInt();
							sequencePoints.Add
								(new SequencePoint(offset, document, line, 0));
						}
					}
					else if(e.Type == SymReader.DataType_LineColumnOffsets)
					{
						// Block contains line, column, and offset values.
						filename = reader.ReadString(e.GetNextInt());
						document = reader.GetDocument(filename);
						while((line = e.GetNextInt()) != -1)
						{
							column = e.GetNextInt();
							offset = e.GetNextInt();
							sequencePoints.Add
								(new SequencePoint
									(offset, document, line, column));
						}
					}
				}

				// Sort the sequence points on ascending offset.
				sequencePoints.Sort(new SequencePointComparer());
			}

}; // class SymMethod

#endif // CONFIG_EXTENDED_DIAGNOSTICS

}; // namespace System.Diagnostics.SymbolStore
