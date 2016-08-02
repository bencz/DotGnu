/*
 * RegexSyntax.cs - Implementation of the "System.Private.RegexSyntax" class.
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

namespace System.Private
{

using System;

[Flags]
internal enum RegexSyntax
{
	// Basic syntax flags.
	None                        = 0x00000000,
	BackslashEscapeInLists		= 0x00000001,
	BkPlusQm					= 0x00000002,
	CharClasses					= 0x00000004,
	ContextIndepAnchors			= 0x00000008,
	ContextIndepOps				= 0x00000010,
	ContextInvalidOps			= 0x00000020,
	DotNewline					= 0x00000040,
	DotNotNull					= 0x00000080,
	HatListsNotNewline			= 0x00000100,
	Intervals					= 0x00000200,
	LimitedOps					= 0x00000400,
	NewlineAlt					= 0x00000800,
	NoBkBraces					= 0x00001000,
	NoBkParens					= 0x00002000,
	NoBkRefs					= 0x00004000,
	NoBkVbar					= 0x00008000,
	NoEmptyRanges				= 0x00010000,
	UnmatchedRightParenOrd		= 0x00020000,
	NoPosixBacktracking			= 0x00040000,
	NoGnuOps					= 0x00080000,
	Debug						= 0x00100000,
	InvalidIntervalOrd			= 0x00200000,
	IgnoreCase					= 0x00400000,
	Wildcard					= 0x00800000,
	All							= 0x00FFFFFF,

	// Useful syntax groups.
	PosixCommon					= CharClasses | DotNewline | DotNotNull |
	              				  Intervals | NoEmptyRanges,
	PosixBasic					= PosixCommon | BkPlusQm,
	PosixMinimalBasic			= PosixCommon | LimitedOps,
	PosixExtended				= PosixCommon | ContextIndepAnchors |
								  ContextIndepOps | NoBkBraces | NoBkParens |
								  NoBkVbar | ContextInvalidOps |
								  UnmatchedRightParenOrd,
	PosixMinimalExtended		= PosixCommon | ContextIndepAnchors |
						   		  ContextInvalidOps | NoBkBraces | NoBkParens |
						   		  NoBkRefs | NoBkVbar | UnmatchedRightParenOrd,
	Emacs						= None,
	Awk							= BackslashEscapeInLists | DotNotNull |
								  NoBkParens | NoBkRefs | NoBkVbar |
								  NoEmptyRanges | DotNewline |
		  						  ContextIndepAnchors |
								  UnmatchedRightParenOrd | NoGnuOps,
	GnuAwk						= (PosixExtended | BackslashEscapeInLists |
								   Debug) & ~(DotNotNull | Intervals |
								   ContextIndepOps),
	PosixAwk					= PosixExtended | BackslashEscapeInLists |
								  Intervals | NoGnuOps,
	Grep						= BkPlusQm | CharClasses | HatListsNotNewline |
	       						  Intervals | NewlineAlt,
	Egrep						= CharClasses | ContextIndepAnchors |
								  ContextIndepOps | HatListsNotNewline |
								  NewlineAlt | NoBkParens | NoBkVbar,
	PosixEgrep					= Egrep | Intervals | NoBkBraces |
								  InvalidIntervalOrd,
	Ed							= PosixBasic,
	Sed							= PosixBasic

}; // enum RegexSyntax

}; // namespace System.Private
