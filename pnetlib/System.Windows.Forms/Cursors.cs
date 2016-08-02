/*
 * Cursors.cs - Implementation of the
 *			"System.Windows.Forms.Cursors" class.
 *
 * Copyright (C) 2003 Neil Cawse.
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
namespace System.Windows.Forms
{
	using System.Drawing.Toolkit;

	public sealed class Cursors 
	{
		private static Cursor appStarting;
		private static Cursor arrow;
		private static Cursor cross;
		private static Cursor defaultCursor;
		private static Cursor iBeam;
		private static Cursor no;
		private static Cursor sizeAll;
		private static Cursor sizeNESW;
		private static Cursor sizeNS;
		private static Cursor sizeNWSE;
		private static Cursor sizeWE;
		private static Cursor upArrow;
		private static Cursor waitCursor;
		private static Cursor help;
		private static Cursor hSplit;
		private static Cursor vSplit;
		private static Cursor noMove2D;
		private static Cursor noMoveHoriz;
		private static Cursor noMoveVert;
		private static Cursor panEast;
		private static Cursor panNE;
		private static Cursor panNorth;
		private static Cursor panNW;
		private static Cursor panSE;
		private static Cursor panSouth;
		private static Cursor panSW;
		private static Cursor panWest;
		private static Cursor hand;

		// Cannot instantiate this class.
		private Cursors() {}

		public static Cursor AppStarting
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(appStarting == null)
					{
						appStarting = new Cursor
							(ToolkitCursorType.AppStarting,
							 typeof(Cursors), "appstarting.cur");
					}
					return appStarting;
				}
			}
		}
		public static Cursor Arrow
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(arrow == null)
					{
						arrow = new Cursor
							(ToolkitCursorType.Arrow,
							 typeof(Cursors), "normal.cur");
					}
					return arrow;
				}
			}
		}
		public static Cursor Cross
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(cross == null)
					{
						cross = new Cursor
							(ToolkitCursorType.Cross,
							 typeof(Cursors), "cross.cur");
					}
					return cross;
				}
			}
		}
		public static Cursor Default
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(defaultCursor == null)
					{
						defaultCursor = new Cursor
							(ToolkitCursorType.Default,
							 typeof(Cursors), "normal.cur");
					}
					return defaultCursor;
				}
			}
		}
		public static Cursor IBeam
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(iBeam == null)
					{
						iBeam = new Cursor
							(ToolkitCursorType.IBeam,
							 typeof(Cursors), "ibeam.cur");
					}
					return iBeam;
				}
			}
		}
		public static Cursor No
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(no == null)
					{
						no = new Cursor
							(ToolkitCursorType.No,
							 typeof(Cursors), "no.cur");
					}
					return no;
				}
			}
		}
		public static Cursor SizeAll
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(sizeAll == null)
					{
						sizeAll = new Cursor
							(ToolkitCursorType.SizeAll,
							 typeof(Cursors), "sizeall.cur");
					}
					return sizeAll;
				}
			}
		}
		public static Cursor SizeNESW
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(sizeNESW == null)
					{
						sizeNESW = new Cursor
							(ToolkitCursorType.SizeNESW,
							 typeof(Cursors), "sizenesw.cur");
					}
					return sizeNESW;
				}
			}
		}
		public static Cursor SizeNS
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(sizeNS == null)
					{
						sizeNS = new Cursor
							(ToolkitCursorType.SizeNS,
							 typeof(Cursors), "sizens.cur");
					}
					return sizeNS;
				}
			}
		}
		public static Cursor SizeNWSE
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(sizeNWSE == null)
					{
						sizeNWSE = new Cursor
							(ToolkitCursorType.SizeNWSE,
							 typeof(Cursors), "sizenwse.cur");
					}
					return sizeNWSE;
				}
			}
		}	
		public static Cursor SizeWE
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(sizeWE == null)
					{
						sizeWE = new Cursor
							(ToolkitCursorType.SizeWE,
							 typeof(Cursors), "sizewe.cur");
					}
					return sizeWE;
				}
			}
		}
		public static Cursor UpArrow
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(upArrow == null)
					{
						upArrow = new Cursor
							(ToolkitCursorType.UpArrow,
							 typeof(Cursors), "up.cur");
					}
					return upArrow;
				}
			}
		}
		public static Cursor WaitCursor
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(waitCursor == null)
					{
						waitCursor = new Cursor
							(ToolkitCursorType.WaitCursor,
							 typeof(Cursors), "wait.cur");
					}
					return waitCursor;
				}
			}
		}
		public static Cursor Help
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(help == null)
					{
						help = new Cursor
							(ToolkitCursorType.Help,
							 typeof(Cursors), "help.cur");
					}
					return help;
				}
			}
		}
		public static Cursor HSplit
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(hSplit == null)
					{
						hSplit = new Cursor
							(ToolkitCursorType.HSplit,
							 typeof(Cursors), "hsplit.cur");
					}
					return hSplit;
				}
			}
		}
		public static Cursor VSplit
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(vSplit == null)
					{
						vSplit = new Cursor
							(ToolkitCursorType.VSplit,
							 typeof(Cursors), "vsplit.cur");
					}
					return vSplit;
				}
			}
		}
		public static Cursor NoMove2D
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(noMove2D == null)
					{
						noMove2D = new Cursor
							(ToolkitCursorType.NoMove2D,
							 typeof(Cursors), "nomove2d.cur");
					}
					return noMove2D;
				}
			}
		}
		public static Cursor NoMoveHoriz
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(noMoveHoriz == null)
					{
						noMoveHoriz = new Cursor
							(ToolkitCursorType.NoMoveHoriz,
							 typeof(Cursors), "nomovehoriz.cur");
					}
					return noMoveHoriz;
				}
			}
		}
		public static Cursor NoMoveVert
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(noMoveVert == null)
					{
						noMoveVert = new Cursor
							(ToolkitCursorType.NoMoveVert,
							 typeof(Cursors), "nomovevert.cur");
					}
					return noMoveVert;
				}
			}
		}
		public static Cursor PanEast
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panEast == null)
					{
						panEast = new Cursor
							(ToolkitCursorType.PanEast,
							 typeof(Cursors), "paneast.cur");
					}
					return panEast;
				}
			}
		}
		public static Cursor PanNE
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panNE == null)
					{
						panNE = new Cursor
							(ToolkitCursorType.PanNE,
							 typeof(Cursors), "panne.cur");
					}
					return panNE;
				}
			}
		}
		public static Cursor PanNorth
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panNorth == null)
					{
						panNorth = new Cursor
							(ToolkitCursorType.PanNorth,
							 typeof(Cursors), "pannorth.cur");
					}
					return panNorth;
				}
			}
		}
		public static Cursor PanNW
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panNW == null)
					{
						panNW = new Cursor
							(ToolkitCursorType.PanNW,
							 typeof(Cursors), "pannw.cur");
					}
					return panNW;
				}
			}
		}
		public static Cursor PanSE
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panSE == null)
					{
						panSE = new Cursor
							(ToolkitCursorType.PanSE,
							 typeof(Cursors), "panse.cur");
					}
					return panSE;
				}
			}
		}
		public static Cursor PanSouth
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panSouth == null)
					{
						panSouth = new Cursor
							(ToolkitCursorType.PanSouth,
							 typeof(Cursors), "pansouth.cur");
					}
					return panSouth;
				}
			}
		}
		public static Cursor PanSW
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panSW == null)
					{
						panSW = new Cursor
							(ToolkitCursorType.PanSW,
							 typeof(Cursors), "pansw.cur");
					}
					return panSW;
				}
			}
		}
		public static Cursor PanWest
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(panWest == null)
					{
						panWest = new Cursor
							(ToolkitCursorType.PanWest,
							 typeof(Cursors), "panwest.cur");
					}
					return panWest;
				}
			}
		}
		public static Cursor Hand
		{
			get
			{
				lock(typeof(Cursors))
				{
					if(hand == null)
					{
						hand = new Cursor
							(ToolkitCursorType.Hand,
							 typeof(Cursors), "hand.cur");
					}
					return hand;
				}
			}
		}
	}

}
