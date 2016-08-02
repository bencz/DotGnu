/*
 * FtpStatusCode.cs - Implementation of the "System.Net.FtpStatusCode" class.
 *
 * Copyright (C) 2007  Southern Storm Software, Pty Ltd.
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

namespace System.Net
{

#if CONFIG_FRAMEWORK_2_0 && !CONFIG_COMPACT_FRAMEWORK

public enum FtpStatusCode
{
	Undefined							= 0,
	RestartMarker						= 110,
	ServiceTemporarilyNotAvailable		= 120,
	DataAlreadyOpen						= 125,
	OpeningData							= 150,
	CommandOk							= 200,
	CommandExtraneous					= 202,
	DirectoryStatus						= 212,
	FileStatus							= 213,
	SystemType							= 215,
	SendUserCommand						= 220,
	ClosingControl						= 221,
	ClosingData							= 226,
	EnteringPassive						= 227,
	LoggedInProceed						= 230,
	ServerWantsSecureSession			= 234,
	FileActionOk						= 250,
	PathnameCreated						= 257,
	SendPasswordCommand					= 331,
	NeedLoginAccount					= 332,
	FileCommandPending					= 350,
	ServiceNotAvailable					= 421,
	CantOpenData						= 425,
	ConnectionClosed					= 426,
	ActionNotTakenFileUnavailableOrBusy	= 450,
	ActionAbortedLocalProcessingError	= 451,
	ActionNotTakenInsufficientSpace		= 452,
	CommandSyntaxError					= 500,
	ArgumentSyntaxError					= 501,
	CommandNotImplemented				= 502,
	BadCommandSequence					= 503,
	NotLoggedIn							= 530,
	AccountNeeded						= 532,
	ActionNotTakenFileUnavailable		= 550,
	ActionAbortedUnknownPageType		= 551,
	FileActionAborted					= 552,
	ActionNotTakenFilenameNotAllowed	= 553
}; // enum FtpStatusCode

#endif // CONFIG_FRAMEWORK_2_0  && !CONFIG_COMPACT_FRAMEWORK

}; // namespace System.Net
