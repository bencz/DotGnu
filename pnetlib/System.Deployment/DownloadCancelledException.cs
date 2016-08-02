/*
 * DownloadCancelledException.cs - Implementation of the
 *		"System.Deployment.DownloadCancelledException" class.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

namespace System.Deployment
{

using System;
using System.Runtime.Serialization;

public class DownloadCancelledException : DeploymentDownloadException
{
	// Constructors.
	public DownloadCancelledException() : base(S._("SD_DownloadCancelled")) {}
	public DownloadCancelledException(String message) : base(message) {}
	public DownloadCancelledException(String message, Exception innerException)
		: base(message, innerException) {}

#if CONFIG_SERIALIZATION

	// De-serialize this object.
	protected DownloadCancelledException(SerializationInfo info,
								  		 StreamingContext context)
		: base(info, context) {}

#endif // CONFIG_SERIALIZATION

}; // class DownloadCancelledException

}; // namespace System.Deployment
