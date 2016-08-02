/*
 * DcopReply.cs - Definition of the DCOP reply - ICE user structure.
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

namespace Xsharp.Dcop
{

using System;

// [StructLayout(LayoutKind.Sequential)]
// This is our user structure, we can do whatever with it! Neat, is not it?
internal class DcopReply
{

	internal enum ReplyStatus
	{
		// Just to make them look nice
		Pending = 0,
		Ok = 1,
		Failed = -1
	}
	public ReplyStatus status;
	public string replyType;
	public int replyId;
	public int transactionId;
	public string calledApp;
	public Object replyObject;
	public string replySlot;

	public DcopReply(DcopFunction fun, int id)
	{
		status = ReplyStatus.Pending;
		replyId = id;
		replyType = fun.ReturnValue;
	}

} // class DcopReply

} // namespace Xsharp.Dcop

