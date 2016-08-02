/*
 * DcopRef.cs - Base class for remote dcop objects with callable methods.
 * Is is fairly low-level, don't use it directly.
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

namespace Xsharp.Dcop
{

using System;
using Xsharp;

/// <summary>
/// <para>The <see cref="T:Xsharp.Dcop.DcopRef"/> class represents remote object
/// with callable methods. Base class for all other DCOP object (see DcopBuilder
/// exapmle).
///
/// Remote object is identified by application-object-(optional)type triad.
/// Latter is not used by this implementation and have limited use in KDE (inheritance
/// mostly).
///
/// Remote object have a set of functions implemented.</para>
/// </summary>
// TODO: Split this into factory and simplier DcopRef
public class DcopRef : ICloneable
{

	private string app = null;
	private string obj = null;

	private DcopClient client;

	/// <summary>
	/// <para>Tells if this DcopRef was initialised or still under construction.</para>
	/// </summary>
	protected bool initialised = false;

	// They should not change in runtime, o cache them
	protected string[] inter = null;

	/// <summary>
	/// <para>Constructs an empty DCOP ref. Useful if you planning to fill in and initialise
	/// it later.</para>
	/// </summary>
	public DcopRef()
	{
		// TODO: Add some code there?
	}

	/// <summary>
	/// <para>This meant to be called by QDataStream. You should use DcopRef() &amp; Discover*()</para>
	/// </summary>
	///
	/// <param name="app">
	/// <para>'Application' identifier of this DcopRef.</para>
	/// </param>
	///
	/// <param name="obj">
	/// <para>'Object' identifier of this DcopRef.</para>
	/// </param>
	public DcopRef(string app, string obj)
	{
		this.app = app;
		this.obj = obj;
	}

	/// <summary>
	/// <para>This is used to cast DcopRefs to subclasses in marshal code. You should use DcopRef() &amp;
	/// Discover*()</para>
	/// </summary>
	///
	/// <param name="parent">
	/// <para>DcopRef to copy identifiers from.</para>
	/// </param>
	public DcopRef(DcopRef parent)
	{
		if(parent == null)
		{
			throw new ArgumentNullException("parent", "Argument cannot be null");
		}

		this.app = parent.App;
		this.obj = parent.Obj;
	}

	/// <summary>
	/// <para>Tries to find application on server and bind reference to application.</para>
	/// </summary>
	///
	/// <param name="app">
	/// <para>'Application' to lookup on server.</para>
	/// </param>
	///
	/// <param name="sessioned">
	/// <para>Some applications are sessioned ('app-pid'), some are not (just 'app').</para>
	/// </param>
	///
	/// <param name="createIfNotExists">
	/// <para>If this application is absent, try to start it or not.</para>
	/// </param>
	///
	/// <exception cref="T:Xsharp.Dcop.DcopException">
	/// <para>Raised if connection to application has failed.</para>
	/// </exception>
	public void DiscoverApplication(string app, bool sessioned, bool createIfNotExists)
	{
		// Discovers object
		string[] apps;
		string[] bits;
		if(client == null)
		{
			DiscoverClient();
		}
		apps = client.registeredApplications();
		for(int i = 0; i < apps.Length; i++)
		{
			if(sessioned)
			{
				bits = apps[i].Split(new char[]{'-'});
				if( (String.Join("-", bits, 0, bits.Length - 1) == app) &&
					(Int32.Parse(bits[bits.Length - 1]) > 0) )
				{
					this.app = apps[i];
					return; // Will discover first found
				}
			}
			else
			{
				if(apps[i] == app)
				{
					this.app = apps[i];
					return;
				}
			}
		}
		if(createIfNotExists)
		{
			ServiceResult res = (ServiceResult)client.Call("klauncher", "klauncher", "serviceResult start_service_by_desktop_name(QString,QStringList)", app, null);
			if(res.Result > 0) // Yes, >0, it's not error
			{
				throw new DcopNamingException(String.Format("Failed to discover, klauncher returned: {0}", res.ErrorMessage));
			}
			this.app = res.DcopName;
			return;
		}
		throw new DcopNamingException("Failed to discover application");
	}

	/// <summary>
	/// <para>Initialises DcopRef, checking if object exists on server.
	/// You cannot change internals after calling Initialise()</para>
	/// </summary>
	///
	/// <exception cref="T:Xsharp.Dcop.DcopException">
	/// <para>Raised if connection to object has failed.</para>
	/// </exception>
	public void Initialise()
	{
		if(client == null)
		{
			DiscoverClient();
		}
		if(app == null)
		{
			throw new DcopConnectionException("`app' was not initialised");
		}
		if(obj == null)
		{
			this.obj = ""; // Just use default one
		}
		initialised = true;
		string[] inter = interfaces();
		for(int i = 0; i < inter.Length; i++)
		{
			if(inter[i] == "DCOPObject")
			{
				return;
			}
		}
		initialised = false;
		throw new DcopNamingException("Not an object or naming fault");
	}


	/// <summary>
	/// <para>Calls function on this DCOP object.
	/// This should be called in subclasses' stubs.</para>
	/// </summary>
	///
	/// <param name="fun">
	/// <para>Remote function to call. See DcopFunction class for syntax.</para>
	/// </param>
	///
	/// <param name="parameters">
	/// <para>Parameters to pass to function. Keep them in sync with function definition.</para>
	/// </param>
	///
	/// <value>
	/// <para>Object, received from remote app. Its type depends on remote app not function name.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.DcopException">
	/// <para>Exception raised if there are problems either with connection or naming.</para>
	/// </exception>
	public Object Call(string fun, params Object[] args)
	{
		if(!initialised)
		{
			throw new DcopConnectionException("Attempt to do Call()s on non-initialised Dcop reference");
		}
		return client.Call(app, obj, fun, args);
	}

	public Object Clone()
	{
		DcopRef res = new DcopRef();
		res.Obj = obj;
		res.Client = client;
		if(!initialised)
		{
			res.App = app;
		}
		else
		{
			res.DiscoverApplication(app, false, false);
			res.Initialise();
		}
		return res;
	}

	protected void DiscoverClient()
	{
		if(DcopClient.MainClient == null)
		{
			if(Application.Primary != null)
			DcopClient.MainClient = new DcopClient(Application.Primary.Display, null);
		}
		this.client = DcopClient.MainClient;
	}

	/// <summary>
	/// <para>Lists objects, contained in this DcopRef's application.</para>
	/// </summary>
	///
	/// <value>
	/// <para>List of object.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.DcopException">
	/// <para>Exception raised if there are problems either with connection or naming.</para>
	/// </exception>
	[DcopCall("QCStringList objects()")]
	public string[] objects()
	{
		if(app == null)
		{
			throw new DcopConnectionException("`app' is null");
		}
		if(client == null)
		{
			DiscoverClient();
		}
		return (string[])client.Call(app, "DCOPClient", "QCStringList objects()");
	}

	/// <summary>
	/// <para>Lists functions, avaliable in this DcopRef's object.</para>
	/// </summary>
	///
	/// <value>
	/// <para>List of functions.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.DcopException">
	/// <para>Exception raised if there are problems either with connection or naming.</para>
	/// </exception>
	[DcopCall("QCStringList functions()")]
	public string[] functions()
	{
		return (string[])Call("QCStringList functions()");
	}

	/// <summary>
	/// <para>Lists interfaces, implemented in this DcopRef's object.</para>
	/// </summary>
	///
	/// <value>
	/// <para>List of interfaces.</para>
	/// </value>
	///
	/// <exception cref="T:Xsharp.DcopException">
	/// <para>Exception raised if there are problems either with connection or naming.</para>
	/// </exception>
	[DcopCall("QCStringList interfaces()")]
	public string[] interfaces()
	{
		if(inter == null)
		{
			inter = (string[])Call("QCStringList interfaces()");
		}
		return inter;
	}

	/// <summary>
	/// <para>Application identifier. Can not be set after DcopRef is Initialised().</para>
	/// </summary>
	public string App
	{
		get
		{
			return app;
		}
		set
		{
			if(initialised)
			{
				throw new DcopConnectionException("Attempt to change internal state while already initialised");
			}
			app = value;
		}
	}

	/// <summary>
	/// <para>Object identifier. Can not be set after DcopRef is Initialised().</para>
	/// </summary>
	public string Obj
	{
		get
		{
			return obj;
		}
		set
		{
			if(initialised)
			{
				throw new DcopConnectionException("Attempt to change internal state while already initialised");
			}
			obj = value;
		}
	}

/*	public string Type
	{
		get
		{
			return Type;
		}
	}*/


	/// <summary>
	/// <para>DCOP client used to do calls. Can not be set after DcopRef is Initialised().</para>
	/// </summary>
	public DcopClient Client
	{
		get
		{
			return client;
		}
		set
		{
			if(initialised)
			{
				throw new DcopConnectionException("Attempt to change internal state while already initialised");
			}
			client = value;
		}
	}

}; // Class DcopRef

} // Namespace Xsharp.Dcop

