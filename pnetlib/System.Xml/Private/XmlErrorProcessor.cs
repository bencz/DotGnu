/*
 * XmlErrorProcessor.cs - Implementation of the
 *			"System.Xml.Private.XmlErrorProcessor" class.
 *
 * Copyright (C) 2004  Free Software Foundation, Inc.
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

namespace System.Xml.Private
{

using System;

internal class XmlErrorProcessor
{
	// Internal state.
	private ErrorHandler errorHandler;


	// Constructor.
	protected XmlErrorProcessor(ErrorHandler errorHandler)
			{
				this.errorHandler = errorHandler;
			}


	// Get or set the error handler.
	public virtual ErrorHandler ErrorHandler
			{
				get { return errorHandler; }
				set { errorHandler = value; }
			}


	// Handle the error state.
	protected virtual void Error()
			{
				Error("Xml_ReaderError");
			}
	protected virtual void Error(String messageTag, params Object[] args)
			{
				if(errorHandler != null)
				{
					errorHandler(messageTag, args);
				}
			}

}; // class XmlErrorProcessor

}; // namespace System.Xml.Private
