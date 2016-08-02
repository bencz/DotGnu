/*
 * ObjectManager.cs - Implementation of the
 *			"System.Runtime.Serialization.ObjectManager" class.
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

namespace System.Runtime.Serialization
{

#if CONFIG_SERIALIZATION

using System;
using System.Collections;
using System.Reflection;
using System.Security.Permissions;
using System.Runtime.Serialization;

public class ObjectManager
{
	// Common information that is stored for a member fixup.
	private abstract class ObjectFixup
	{
        public ObjectManager manager;
		public ObjectInfo value;
		public ObjectFixup nextFixup;

		// Constructor.
		protected ObjectFixup(ObjectManager manager, ObjectInfo value, ObjectFixup nextFixup)
				{
				    this.manager = manager;
					this.value = value;
					this.nextFixup = nextFixup;
				}

		// Apply this fixup to an object.
		public virtual void Apply(ObjectInfo objI)
				{
					throw new SerializationException
						(_("Serialize_BadFixup"));
				}

	}; // class ObjectFixup

	// Fixup that uses a MemberInfo.
	private class MemberInfoFixup : ObjectFixup
	{
		private MemberInfo member;

		// Constructor.
		public MemberInfoFixup(ObjectManager manager, ObjectInfo value,
		                       MemberInfo member, ObjectFixup nextFixup)
				: base(manager, value, nextFixup)
				{
					this.member = member;
				}

		// Apply this fixup to an object.
		public override void Apply(ObjectInfo objI)
				{
					if(member is FieldInfo)
					{
						((FieldInfo)member).SetValue(objI.obj, value.obj);
					}
					else
					{
						throw new SerializationException
							(_("Serialize_BadFixup"));
					}
				}

	}; // class MemberInfoFixup

	// Fixup that uses a member name.
	private class MemberNameFixup : ObjectFixup
	{
		private String memberName;

		// Constructor.
		public MemberNameFixup(ObjectManager manager, ObjectInfo value,
		                       String memberName, ObjectFixup nextFixup)
				: base(manager, value, nextFixup)
				{
					this.memberName = memberName;
				}

		// Apply this fixup to an object.
		public override void Apply(ObjectInfo objI)
				{				    
				    if(objI.sinfo != null)
				    {
				        objI.sinfo.AddValue(memberName, value.obj);

				        if(nextFixup == null)
				        {
				            BindingFlags flags = BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance;
				            Type[] pTypes = new Type[] {typeof(SerializationInfo), typeof(StreamingContext)};
				            Type type = objI.obj.GetType();
				            ConstructorInfo ctor = type.GetConstructor(flags, null , pTypes, null);
				            if(ctor == null)
				            {
				                throw new SerializationException
        							(_("Serialize_BadFixup"));
				            }
				            Object[] parms = new Object[] {objI.sinfo, manager.context};
				            ctor.InvokeOnEmpty(objI.obj, parms);
				        }
				    }
				    else
				    {
			    		MemberInfo[] member = objI.obj.GetType().GetMember
		    				(memberName);
	    				if(member == null || member.Length != 1)
    					{
					    	throw new SerializationException
				    			(_("Serialize_BadFixup"));
			    		}
		    			if(member[0] is FieldInfo)
	    				{
    						((FieldInfo)(member[0])).SetValue(objI.obj, value.obj);
					    }
				    	else
			    		{
		    				throw new SerializationException
	    						(_("Serialize_BadFixup"));
    					}
				    }
				}

	}; // class MemberNameFixup

	// Fixup that uses an array index.
	private class ArrayIndexFixup : ObjectFixup
	{
		private int[] indices;

		// Constructor.
		public ArrayIndexFixup(ObjectManager manager, ObjectInfo value,
		                       int[] indices, ObjectFixup nextFixup)
				: base(manager, value, nextFixup)
				{
					this.indices = indices;
				}

		// Apply this fixup to an object.
		public override void Apply(ObjectInfo objI)
				{
					((Array)objI.obj).SetValue(value.obj, indices);
				}

	}; // class ArrayIndexFixup

	// Fixup that uses an index into a single-dimensional array.
	private class SingleArrayIndexFixup : ObjectFixup
	{
		private int index;

		// Constructor.
		public SingleArrayIndexFixup(ObjectManager manager, ObjectInfo value,
		                             int index, ObjectFixup nextFixup)
				: base(manager, value, nextFixup)
				{
					this.index = index;
				}

		// Apply this fixup to an object.
		public override void Apply(ObjectInfo objI)
				{
					((Array)objI.obj).SetValue(value.obj, index);
				}

	}; // class SingleArrayIndexFixup

	// Information that is stored for an object identifier.
	private sealed class ObjectInfo
	{
		public Object obj;
		public SerializationInfo sinfo;
		public long idOfContainingObject;
		public MemberInfo member;
		public int[] arrayIndex;
		public ObjectInfo contains;
		public ObjectInfo nextContains;
		public ObjectFixup fixups;
		public bool done;

	}; // class ObjectInfo

	// Internal state.
	private ISurrogateSelector selector;
	protected StreamingContext context;
	private Hashtable objects;
	private ArrayList callbackList;

	// Constructor.
	public ObjectManager(ISurrogateSelector selector,
						 StreamingContext context)
			{
				// Make sure that we have the correct permissions.
				SecurityPermission perm = new SecurityPermission
					(SecurityPermissionFlag.SerializationFormatter);
				perm.Demand();

				// Initialize the object manager.
				this.selector = selector;
				this.context = context;
				this.objects = new Hashtable(16); // avoid expanding of hashtable
				this.callbackList = new ArrayList();
			}

	// Apply a contained member fixup.
	private static void ApplyContained(ObjectInfo oinfo, ObjectInfo contain)
			{
				if(contain.member != null)
				{
					if(contain.member is FieldInfo)
					{
						((FieldInfo)(contain.member)).SetValue
							(oinfo.obj, contain.obj);
					}
					else
					{
						throw new SerializationException
							(_("Serialize_BadFixup"));
					}
				}
				else if(contain.arrayIndex != null)
				{
					((Array)(oinfo.obj)).SetValue
						(contain.obj, contain.arrayIndex);
				}
				else
				{
					throw new SerializationException
						(_("Serialize_BadFixup"));
				}
			}

	// Perform recorded fixups for contained objects.
	private static void DoFixupsForContained(ObjectInfo oinfo)
			{
				ObjectInfo contain = oinfo.contains;
				ObjectFixup fixup;
				do
				{
					if(!(contain.done))
					{
						contain.done = true;
						if(contain.obj == null)
						{
							throw new SerializationException
								(_("Serialize_MissingFixup"));
						}
						if(contain.contains != null)
						{
							DoFixupsForContained(contain);
						}
						fixup = contain.fixups;
						while(fixup != null)
						{
							if(fixup.value.obj == null)
							{
								throw new SerializationException
									(_("Serialize_MissingFixup"));
							}
							fixup.Apply(contain);
							fixup = fixup.nextFixup;
						}
						ApplyContained(oinfo, contain);
					}
					contain = contain.nextContains;
				}
				while(contain != null);
			}

	// Perform recorded fixups.
	public virtual void DoFixups()
			{
				IDictionaryEnumerator e = objects.GetEnumerator();
				ObjectInfo oinfo;
				ObjectFixup fixup;
				while(e.MoveNext())
				{
					oinfo = (ObjectInfo)(e.Value);
					if(oinfo.obj == null)
					{
						throw new SerializationException
							(_("Serialize_MissingFixup"));
					}
					if(oinfo.done || oinfo.idOfContainingObject > 0)
					{
						// We already saw this object or the object is
						// contained within something at a higher level.
						continue;
					}
					oinfo.done = true;
					if(oinfo.contains != null)
					{
						// Handle value type members within this object.
						DoFixupsForContained(oinfo);
					}
					fixup = oinfo.fixups;
					while(fixup != null)
					{
						if(fixup.value.obj == null)
						{
							throw new SerializationException
								(_("Serialize_MissingFixup"));
						}
						fixup.Apply(oinfo);
						fixup = fixup.nextFixup;
					}
				}
				RaiseDeserializationEvent();
			}

	// Return an object with a specific identifier.
	public virtual Object GetObject(long objectID)
			{
				if(objectID <= 0)
				{
					throw new ArgumentOutOfRangeException
						("objectID", _("Serialize_BadObjectID"));
				}
				ObjectInfo info = (ObjectInfo)(objects[objectID]);
				if(info != null)
				{
					return info.obj;
				}
				else
				{
					return null;
				}
			}

	// Raise a deserialization event on all registered objects that want it.
	public virtual void RaiseDeserializationEvent()
			{
				IEnumerator e = callbackList.GetEnumerator();
				while(e.MoveNext())
				{
					IDeserializationCallback cb;
					cb = (e.Current as IDeserializationCallback);
					if(cb != null)
					{
						cb.OnDeserialization(null);
					}
				}
			}

	// Get the object information for an object, or add a new one.
	private ObjectInfo GetObjectInfo(long objectID)
			{
				if(objectID <= 0)
				{
					throw new ArgumentOutOfRangeException
						("objectID", _("Serialize_BadObjectID"));
				}
				ObjectInfo oinfo = (ObjectInfo)(objects[objectID]);
				if(oinfo != null)
				{
					return oinfo;
				}
				else
				{
					oinfo = new ObjectInfo();
					objects[objectID] = oinfo;
					return oinfo;
				}
			}

	// Record an array element fixup to be performed later.
	public virtual void RecordArrayElementFixup
				(long arrayToBeFixed, int index, long objectRequired)
			{
				ObjectInfo oinfo1 = GetObjectInfo(arrayToBeFixed);
				ObjectInfo oinfo2 = GetObjectInfo(objectRequired);
				oinfo1.fixups = new SingleArrayIndexFixup
					(this, oinfo2, index, oinfo1.fixups);
			}
	public virtual void RecordArrayElementFixup
				(long arrayToBeFixed, int[] indices, long objectRequired)
			{
				ObjectInfo oinfo1 = GetObjectInfo(arrayToBeFixed);
				ObjectInfo oinfo2 = GetObjectInfo(objectRequired);
				if(indices == null)
				{
					throw new ArgumentNullException("indices");
				}
				oinfo1.fixups = new ArrayIndexFixup
					(this, oinfo2, indices, oinfo1.fixups);
			}

	// Record an object member fixup to be performed later.
	public virtual void RecordDelayedFixup
				(long objectToBeFixed, String memberName, long objectRequired)
			{
				ObjectInfo oinfo1 = GetObjectInfo(objectToBeFixed);
				ObjectInfo oinfo2 = GetObjectInfo(objectRequired);
				if(memberName == null)
				{
					throw new ArgumentNullException("memberName");
				}
				oinfo1.fixups = new MemberNameFixup
					(this, oinfo2, memberName, oinfo1.fixups);
			}
	public virtual void RecordFixup
				(long objectToBeFixed, MemberInfo member, long objectRequired)
			{
				ObjectInfo oinfo1 = GetObjectInfo(objectToBeFixed);
				ObjectInfo oinfo2 = GetObjectInfo(objectRequired);
				if(member == null)
				{
					throw new ArgumentNullException("member");
				}
				oinfo1.fixups = new MemberInfoFixup
					(this, oinfo2, member, oinfo1.fixups);
			}

	// Register an object with the object manager.
	public virtual void RegisterObject(Object obj, long objectID)
			{
				RegisterObject(obj, objectID, null, 0, null, null);
			}
	public void RegisterObject(Object obj, long objectID,
							   SerializationInfo info)
			{
				RegisterObject(obj, objectID, info, 0, null, null);
			}
	public void RegisterObject(Object obj, long objectID,
							   SerializationInfo info,
							   long idOfContainingObj,
							   MemberInfo member)
			{
				RegisterObject(obj, objectID, info, idOfContainingObj,
							   member, null);
			}
	public void RegisterObject(Object obj, long objectID,
							   SerializationInfo info,
							   long idOfContainingObj,
							   MemberInfo member,
							   int[] arrayIndex)
			{
				if(obj == null)
				{
					throw new ArgumentNullException("objectID");
				}
				if(objectID <= 0)
				{
					throw new ArgumentOutOfRangeException
						("objectID", _("Serialize_BadObjectID"));
				}
				ObjectInfo oinfo = (ObjectInfo)(objects[objectID]);
				if(oinfo != null && oinfo.obj != null && oinfo.obj != obj)
				{
					throw new SerializationException
						(_("Serialize_AlreadyRegistered"));
				}
				else if(oinfo != null)
				{
					// Update the information for an existing reference.
					oinfo.obj = obj;
					if(obj is IDeserializationCallback)
					{
						callbackList.Add(obj);
					}
					if(info != null)
					{
						oinfo.sinfo = info;
					}
					if(member != null)
					{
						oinfo.member = member;
					}
					if(arrayIndex != null)
					{
						oinfo.arrayIndex = arrayIndex;
					}
					if(idOfContainingObj != 0 &&
					   oinfo.idOfContainingObject == 0)
					{
						oinfo.idOfContainingObject = idOfContainingObj;
						RegisterWithContaining(oinfo);
					}
				}
				else
				{
					// Create a new object information block.
					oinfo = new ObjectInfo();
					oinfo.obj = obj;
					oinfo.sinfo = info;
					oinfo.idOfContainingObject = idOfContainingObj;
					oinfo.member = member;
					oinfo.arrayIndex = arrayIndex;
					objects[objectID] = oinfo;
					// Register the object to be called later by
					// "RaiseDeserializationEvent".
					if(obj is IDeserializationCallback)
					{
						callbackList.Add(obj);
					}

					// Register the information block with the container.
					if(idOfContainingObj > 0)
					{
						RegisterWithContaining(oinfo);
					}
				}
			}

	// Register an object with its containing object.
	private void RegisterWithContaining(ObjectInfo oinfo)
			{
				ObjectInfo oinfo2 =
					(ObjectInfo)(objects[oinfo.idOfContainingObject]);
				if(oinfo2 == null)
				{
					oinfo2 = new ObjectInfo();
					objects[oinfo.idOfContainingObject] = oinfo2;
				}
				oinfo.nextContains = oinfo2.contains;
				oinfo2.contains = oinfo;
			}

}; // class ObjectManager

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Serialization
