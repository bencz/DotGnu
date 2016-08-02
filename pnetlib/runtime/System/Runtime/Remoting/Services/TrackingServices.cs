/*
 * TrackingServices.cs - Implementation of the
 *			"System.Runtime.Remoting.Services.TrackingServices" class.
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

using System.Collections;

public class TrackingServices
{
	// Internal state.
	private static ArrayList handlerList;

	// Register a tracking handler.
	public static void RegisterTrackingHandler(ITrackingHandler handler)
			{
				if(handler == null)
				{
					throw new ArgumentNullException("handler");
				}
				lock(typeof(TrackingServices))
				{
					if(handlerList == null)
					{
						handlerList = new ArrayList();
						handlerList.Add(handler);
					}
					else if(!handlerList.Contains(handler))
					{
						handlerList.Add(handler);
					}
				}
			}

	// Unregister a tracking handler.
	public static void UnregisterTrackingHandler(ITrackingHandler handler)
			{
				if(handler == null)
				{
					throw new ArgumentNullException("handler");
				}
				lock(typeof(TrackingServices))
				{
					if(handlerList != null)
					{
						handlerList.Remove(handler);
					}
				}
			}

	// Get a list of all registered tracking handlers.
	public static ITrackingHandler[] RegisteredHandlers
			{
				get
				{
					lock(typeof(TrackingServices))
					{
						if(handlerList == null)
						{
							return new ITrackingHandler [0];
						}
						else
						{
							ITrackingHandler[] array;
							array = new ITrackingHandler [handlerList.Count];
							handlerList.CopyTo(array, 0);
							return array;
						}
					}
				}
			}

}; // class TrackingServices

#endif // CONFIG_REMOTING

}; // namespace System.Runtime.Remoting.Services
