/*
 * UnicodeCategory.cs - Implement "System.Globalization.UnicodeCategory".
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Globalization
{

public enum UnicodeCategory
{
	UppercaseLetter				= 0,
	LowercaseLetter				= 1,
	TitlecaseLetter				= 2,
	ModifierLetter				= 3,
	OtherLetter					= 4,
	NonSpacingMark				= 5,
	SpacingCombiningMark		= 6,
	EnclosingMark				= 7,
	DecimalDigitNumber			= 8,
	LetterNumber				= 9,
	OtherNumber					= 10,
	SpaceSeparator				= 11,
	LineSeparator				= 12,
	ParagraphSeparator			= 13,
	Control						= 14,
	Format						= 15,
	Surrogate					= 16,
	PrivateUse					= 17,
	ConnectorPunctuation		= 18,
	DashPunctuation				= 19,
	OpenPunctuation				= 20,
	ClosePunctuation			= 21,
	InitialQuotePunctuation		= 22,
	FinalQuotePunctuation		= 23,
	OtherPunctuation			= 24,
	MathSymbol					= 25,
	CurrencySymbol				= 26,
	ModifierSymbol				= 27,
	OtherSymbol					= 28,
	OtherNotAssigned			= 29

}; // enum UnicodeCategory

}; // namespace System.Globalization
