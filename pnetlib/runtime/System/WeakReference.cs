/*
 * WeakReference.cs - Implementation of the "System.WeakReference" class.
 *
 * Copyright (C) 2002, 2003  Southern Storm Software, Pty Ltd.
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

namespace System
{

#if !ECMA_COMPAT

using System.Runtime.InteropServices;
using System.Runtime.Serialization;

public class WeakReference
#if CONFIG_SERIALIZATION
	: ISerializable
#endif
{

	// Internal state.
	private GCHandle handle;

	// Constructors.
	public WeakReference(Object obj) : this(obj, false) {}
	public WeakReference(Object obj, bool trackResurrection)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if(trackResurrection)
				{
					this.handle = GCHandle.Alloc
						(obj, GCHandleType.WeakTrackResurrection);
				}
				else
				{
					this.handle = GCHandle.Alloc(obj, GCHandleType.Weak);
				}
			}
#if CONFIG_SERIALIZATION
	protected WeakReference(SerializationInfo info, StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				Object obj = info.GetValue("TrackedObject", typeof(Object));
				bool track = info.GetBoolean("TrackResurrection");
				if(obj == null)
				{
					throw new ArgumentNullException("obj");
				}
				if(track)
				{
					this.handle = GCHandle.Alloc
						(obj, GCHandleType.WeakTrackResurrection);
				}
				else
				{
					this.handle = GCHandle.Alloc(obj, GCHandleType.Weak);
				}
			}
#endif // CONFIG_SERIALIZATION

	// Destructor.
	~WeakReference()
			{
				handle.Free();
			}

	// Determine if the referenced object is still alive.
	public virtual bool IsAlive
			{
				get
				{
					return (handle.Target != null);
				}
			}

	// Get or set the target of this weak reference.
	public virtual Object Target
			{
				get
				{
					return handle.Target;
				}
				set
				{
					handle.Target = value;
				}
			}

	// Determine if the object is tracked after finalization.
	public virtual bool TrackResurrection
			{
				get
				{
					return (handle.GetHandleType() ==
								GCHandleType.WeakTrackResurrection);
				}
			}

#if CONFIG_SERIALIZATION

	// Get the serialization data for this object.
	public virtual void GetObjectData(SerializationInfo info,
									  StreamingContext context)
			{
				if(info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("TrackedObject", Target, typeof(Object));
				info.AddValue("TrackResurrection", TrackResurrection);
			}

#endif // CONFIG_SERIALIZATION

}; // class WeakReference

#endif // !ECMA_COMPAT

}; // namespace System
