/*
 * Constants.cs - Implementation of the
 *			"Microsoft.VisualBasic.Constants" class.
 *
 * Copyright (C) 2003, 2004  Southern Storm Software, Pty Ltd.
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

namespace Microsoft.VisualBasic
{

using System;
using Microsoft.VisualBasic.CompilerServices;

[StandardModule]
public sealed class Constants
{
	// This class cannot be instantiated.
	private Constants() {}

	// AppWinStyle values.
	public const AppWinStyle vbHide = AppWinStyle.Hide;
	public const AppWinStyle vbNormalFocus = AppWinStyle.NormalFocus;
	public const AppWinStyle vbMinimizedFocus = AppWinStyle.MinimizedFocus;
	public const AppWinStyle vbMaximizedFocus = AppWinStyle.MaximizedFocus;
	public const AppWinStyle vbNormalNoFocus = AppWinStyle.NormalNoFocus;
	public const AppWinStyle vbMinimizedNoFocus = AppWinStyle.MinimizedNoFocus;

	// CallType values.
	public const CallType vbMethod = CallType.Method;
	public const CallType vbGet = CallType.Get;
	public const CallType vbLet = CallType.Let;
	public const CallType vbSet = CallType.Set;

	// CompareMethod values.
	public const CompareMethod vbBinaryCompare = CompareMethod.Binary;
	public const CompareMethod vbTextCompare = CompareMethod.Binary;

	// ControlChar values.
	public const String vbBack = "\u0008";
	public const String vbCr = "\r";
	public const String vbCrLf = "\r\n";
	public const String vbFormFeed = "\u000C";
	public const String vbLf = "\n";
	public const String vbNewLine = "\r\n";
	public const String vbNullChar = "\0";
	public const String vbNullString = null;
	public const String vbTab = "\t";
	public const String vbVerticalTab = "\u000B";

	// DateFormat values.
	public const DateFormat vbGeneralDate = DateFormat.GeneralDate;
	public const DateFormat vbLongDate = DateFormat.LongDate;
	public const DateFormat vbShortDate = DateFormat.ShortDate;
	public const DateFormat vbLongTime = DateFormat.LongTime;
	public const DateFormat vbShortTime = DateFormat.ShortTime;

	// FileAttribute values.
	public const FileAttribute vbNormal = FileAttribute.Normal;
	public const FileAttribute vbReadOnly = FileAttribute.ReadOnly;
	public const FileAttribute vbHidden = FileAttribute.Hidden;
	public const FileAttribute vbSystem = FileAttribute.System;
	public const FileAttribute vbVolume = FileAttribute.Volume;
	public const FileAttribute vbDirectory = FileAttribute.Directory;
	public const FileAttribute vbArchive = FileAttribute.Archive;

	// FirstDayOfWeek values.
	public const FirstDayOfWeek vbUseSystemDayOfWeek = FirstDayOfWeek.System;
	public const FirstDayOfWeek vbSunday = FirstDayOfWeek.Sunday;
	public const FirstDayOfWeek vbMonday = FirstDayOfWeek.Monday;
	public const FirstDayOfWeek vbTuesday = FirstDayOfWeek.Tuesday;
	public const FirstDayOfWeek vbWednesday = FirstDayOfWeek.Wednesday;
	public const FirstDayOfWeek vbThursday = FirstDayOfWeek.Thursday;
	public const FirstDayOfWeek vbFriday = FirstDayOfWeek.Friday;
	public const FirstDayOfWeek vbSaturday = FirstDayOfWeek.Saturday;

	// FirstWeekOfYear values.
	public const FirstWeekOfYear vbUseSystem = FirstWeekOfYear.System;
	public const FirstWeekOfYear vbFirstJan1 = FirstWeekOfYear.Jan1;
	public const FirstWeekOfYear vbFirstFourDays
			= FirstWeekOfYear.FirstFourDays;
	public const FirstWeekOfYear vbFirstFullWeek
			= FirstWeekOfYear.FirstFullWeek;

	// MsgBoxResult values.
	public const MsgBoxResult vbOK = MsgBoxResult.OK;
	public const MsgBoxResult vbCancel = MsgBoxResult.Cancel;
	public const MsgBoxResult vbAbort = MsgBoxResult.Abort;
	public const MsgBoxResult vbRetry = MsgBoxResult.Retry;
	public const MsgBoxResult vbIgnore = MsgBoxResult.Ignore;
	public const MsgBoxResult vbYes = MsgBoxResult.Yes;
	public const MsgBoxResult vbNo = MsgBoxResult.No;

	// MsgBoxStyle values.
	public const MsgBoxStyle vbApplicationModal = MsgBoxStyle.ApplicationModal;
	public const MsgBoxStyle vbDefaultButton1 = MsgBoxStyle.DefaultButton1;
	public const MsgBoxStyle vbOKOnly = MsgBoxStyle.OKOnly;
	public const MsgBoxStyle vbOKCancel = MsgBoxStyle.OKCancel;
	public const MsgBoxStyle vbAbortRetryIgnore = MsgBoxStyle.AbortRetryIgnore;
	public const MsgBoxStyle vbYesNoCancel = MsgBoxStyle.YesNoCancel;
	public const MsgBoxStyle vbYesNo = MsgBoxStyle.YesNo;
	public const MsgBoxStyle vbRetryCancel = MsgBoxStyle.RetryCancel;
	public const MsgBoxStyle vbCritical = MsgBoxStyle.Critical;
	public const MsgBoxStyle vbQuestion = MsgBoxStyle.Question;
	public const MsgBoxStyle vbExclamation = MsgBoxStyle.Exclamation;
	public const MsgBoxStyle vbInformation = MsgBoxStyle.Information;
	public const MsgBoxStyle vbDefaultButton2 = MsgBoxStyle.DefaultButton2;
	public const MsgBoxStyle vbDefaultButton3 = MsgBoxStyle.DefaultButton3;
	public const MsgBoxStyle vbSystemModal = MsgBoxStyle.SystemModal;
	public const MsgBoxStyle vbMsgBoxHelp = MsgBoxStyle.MsgBoxHelp;
	public const MsgBoxStyle vbMsgBoxSetForeground
			= MsgBoxStyle.MsgBoxSetForeground;
	public const MsgBoxStyle vbMsgBoxRight = MsgBoxStyle.MsgBoxRight;
	public const MsgBoxStyle vbMsgBoxRtlReading = MsgBoxStyle.MsgBoxRtlReading;

	// TriState values.
	public const TriState vbFalse = TriState.False;
	public const TriState vbTrue = TriState.True;
	public const TriState vbUseDefault = TriState.UseDefault;

	// VariantType values.
	public const VariantType vbEmpty = VariantType.Empty;
	public const VariantType vbNull = VariantType.Null;
	public const VariantType vbInteger = VariantType.Integer;
	public const VariantType vbSingle = VariantType.Single;
	public const VariantType vbDouble = VariantType.Double;
	public const VariantType vbCurrency = VariantType.Currency;
	public const VariantType vbDate = VariantType.Date;
	public const VariantType vbString = VariantType.String;
	public const VariantType vbObject = VariantType.Object;
	public const VariantType vbBoolean = VariantType.Boolean;
	public const VariantType vbVariant = VariantType.Variant;
	public const VariantType vbDecimal = VariantType.Decimal;
	public const VariantType vbByte = VariantType.Byte;
	public const VariantType vbLong = VariantType.Long;
	public const VariantType vbUserDefinedType = VariantType.UserDefinedType;
	public const VariantType vbArray = VariantType.Array;

	// VbStrConv values.
	public const VbStrConv vbUpperCase = VbStrConv.UpperCase;
	public const VbStrConv vbLowerCase = VbStrConv.LowerCase;
	public const VbStrConv vbProperCase = VbStrConv.ProperCase;
	public const VbStrConv vbWide = VbStrConv.Wide;
	public const VbStrConv vbNarrow = VbStrConv.Narrow;
	public const VbStrConv vbKatakana = VbStrConv.Katakana;
	public const VbStrConv vbHiragana = VbStrConv.Hiragana;
	public const VbStrConv vbSimplifiedChinese = VbStrConv.SimplifiedChinese;
	public const VbStrConv vbTraditionalChinese = VbStrConv.TraditionalChinese;
	public const VbStrConv vbLinguisticCasing = VbStrConv.LinguisticCasing;

	// Other constants.
	public const int vbObjectError = unchecked((int)0x80040000);

}; // class Constants

}; // namespace Microsoft.VisualBasic
