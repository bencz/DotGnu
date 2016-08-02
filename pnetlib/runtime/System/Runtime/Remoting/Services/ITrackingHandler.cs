/*
 * ITrackingHandler.cs - Implementation of the
 *			"System.Runtime.Remoting.Services.ITrackingHandler" class.
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

namespace System.Runtime.Remoting.Services
{

#if CONFIG_REMOTING

public interface ITrackingHandler
{
	// Track a disconnected object.
	void DisconnectedObject(Object obj);

	// Track a marshalled object.
	void MarshaledObject(Object obj, ObjRef or);

	// Track an unmarshalled object.
	void UnmarshaledObject(Object obj, ObjRef or);

}; // interface ITrackingHandler

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Services
