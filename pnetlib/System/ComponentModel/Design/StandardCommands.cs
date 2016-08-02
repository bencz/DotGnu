/*
 * StandardCommands.cs - Implementation of the
 *		"System.ComponentModel.Design.StandardCommands" class.
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

namespace System.ComponentModel.Design
{

#if CONFIG_COMPONENT_MODEL_DESIGN

public class StandardCommands
{
	// Standard command identifiers.
	public static readonly CommandID AlignBottom;
	public static readonly CommandID AlignHorizontalCenters;
	public static readonly CommandID AlignLeft;
	public static readonly CommandID AlignRight;
	public static readonly CommandID AlignToGrid;
	public static readonly CommandID AlignTop;
	public static readonly CommandID AlignVerticalCenters;
	public static readonly CommandID ArrangeBottom;
	public static readonly CommandID ArrangeRight;
	public static readonly CommandID BringForward;
	public static readonly CommandID BringToFront;
	public static readonly CommandID CenterHorizontally;
	public static readonly CommandID CenterVertically;
	public static readonly CommandID Copy;
	public static readonly CommandID Cut;
	public static readonly CommandID Delete;
	public static readonly CommandID Group;
	public static readonly CommandID HorizSpaceConcatenate;
	public static readonly CommandID HorizSpaceDecrease;
	public static readonly CommandID HorizSpaceIncrease;
	public static readonly CommandID HorizSpaceMakeEqual;
	public static readonly CommandID Paste;
	public static readonly CommandID Properties;
	public static readonly CommandID Redo;
	public static readonly CommandID MultiLevelRedo;
	public static readonly CommandID SelectAll;
	public static readonly CommandID SendBackward;
	public static readonly CommandID SendToBack;
	public static readonly CommandID SizeToControl;
	public static readonly CommandID SizeToControlHeight;
	public static readonly CommandID SizeToControlWidth;
	public static readonly CommandID SizeToFit;
	public static readonly CommandID SizeToGrid;
	public static readonly CommandID SnapToGrid;
	public static readonly CommandID TabOrder;
	public static readonly CommandID Undo;
	public static readonly CommandID MultiLevelUndo;
	public static readonly CommandID Ungroup;
	public static readonly CommandID VertSpaceConcatenate;
	public static readonly CommandID VertSpaceDecrease;
	public static readonly CommandID VertSpaceIncrease;
	public static readonly CommandID VertSpaceMakeEqual;
	public static readonly CommandID ShowGrid;
	public static readonly CommandID ViewGrid;
	public static readonly CommandID Replace;
	public static readonly CommandID PropertiesWindow;
	public static readonly CommandID LockControls;
	public static readonly CommandID F1Help;
	public static readonly CommandID VerbFirst;
	public static readonly CommandID VerbLast;
	public static readonly CommandID ArrangeIcons;
	public static readonly CommandID LineupIcons;
	public static readonly CommandID ShowLargeIcons;

	// Initialize the command identifiers.
	static StandardCommands()
			{
				Guid guid1;
				Guid guid2;

				guid1 = new Guid("{5efc7975-14bc-11cf-9b2b-00aa00573819}");
				guid2 = new Guid("{74d21313-2aee-11d1-8bfb-00a0c90f26f7}");

				AlignBottom					= new CommandID(guid1, 1);
				AlignHorizontalCenters		= new CommandID(guid1, 2);
				AlignLeft					= new CommandID(guid1, 3);
				AlignRight					= new CommandID(guid1, 4);
				AlignToGrid					= new CommandID(guid1, 5);
				AlignTop					= new CommandID(guid1, 6);
				AlignVerticalCenters		= new CommandID(guid1, 7);
				ArrangeBottom				= new CommandID(guid1, 8);
				ArrangeRight				= new CommandID(guid1, 9);
				BringForward				= new CommandID(guid1, 10);
				BringToFront				= new CommandID(guid1, 11);
				CenterHorizontally			= new CommandID(guid1, 12);
				CenterVertically			= new CommandID(guid1, 13);
				Copy						= new CommandID(guid1, 15);
				Cut							= new CommandID(guid1, 16);
				Delete						= new CommandID(guid1, 17);
				Group						= new CommandID(guid1, 20);
				HorizSpaceConcatenate		= new CommandID(guid1, 21);
				HorizSpaceDecrease			= new CommandID(guid1, 22);
				HorizSpaceIncrease			= new CommandID(guid1, 23);
				HorizSpaceMakeEqual			= new CommandID(guid1, 24);
				Paste						= new CommandID(guid1, 26);
				Properties					= new CommandID(guid1, 28);
				Redo						= new CommandID(guid1, 29);
				MultiLevelRedo				= new CommandID(guid1, 30);
				SelectAll					= new CommandID(guid1, 31);
				SendBackward				= new CommandID(guid1, 32);
				SendToBack					= new CommandID(guid1, 33);
				SizeToControl				= new CommandID(guid1, 35);
				SizeToControlHeight			= new CommandID(guid1, 36);
				SizeToControlWidth			= new CommandID(guid1, 37);
				SizeToFit					= new CommandID(guid1, 38);
				SizeToGrid					= new CommandID(guid1, 39);
				SnapToGrid					= new CommandID(guid1, 40);
				TabOrder					= new CommandID(guid1, 41);
				Undo						= new CommandID(guid1, 43);
				MultiLevelUndo				= new CommandID(guid1, 44);
				Ungroup						= new CommandID(guid1, 45);
				VertSpaceConcatenate		= new CommandID(guid1, 46);
				VertSpaceDecrease			= new CommandID(guid1, 47);
				VertSpaceIncrease			= new CommandID(guid1, 48);
				VertSpaceMakeEqual			= new CommandID(guid1, 49);
				ShowGrid					= new CommandID(guid1, 103);
				ViewGrid					= new CommandID(guid1, 125);
				Replace						= new CommandID(guid1, 230);
				PropertiesWindow			= new CommandID(guid1, 235);
				LockControls				= new CommandID(guid1, 369);
				F1Help						= new CommandID(guid1, 377);
				VerbFirst					= new CommandID(guid2, 0x2000);
				VerbLast					= new CommandID(guid2, 0x2100);
				ArrangeIcons				= new CommandID(guid2, 0x300A);
				LineupIcons					= new CommandID(guid2, 0x300B);
				ShowLargeIcons				= new CommandID(guid2, 0x300C);
			}

}; // class StandardCommands

#endif // CONFIG_COMPONENT_MODEL_DESIGN

}; // namespace System.ComponentModel.Design
