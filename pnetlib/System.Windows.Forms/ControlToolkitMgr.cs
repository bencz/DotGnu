/*
 * ControlToolkitManager.cs - Implementation ControlToolkitManager class.
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

namespace System.Windows.Forms
{

using System;
using System.Threading;
using System.Drawing;
using System.Drawing.Toolkit;

/*
	This class is for thread safe creating toolkit windows.
*/
internal class ControlToolkitManager : Control
{
	static ControlToolkitManager Instance; // The only Instance
	
	static ControlToolkitManager() {
		Instance =  new ControlToolkitManager();
	}
	
	
	Thread mCreateThread; // the thread, with the first control was created
	
	private ControlToolkitManager() {
		toolkitWindow = ToolkitManager.Toolkit.CreateChildWindow(null, 0, 0, 10, 10, this);
		mCreateThread  = Thread.CurrentThread;	// remember the thread which created the window
	}
	
	#region create normal child window
	
	delegate IToolkitWindow DelegateCreateChildWindow( Control control, IToolkitWindow parent, int x, int y, int w, int h );
	
	private IToolkitWindow InternalCreateChildWindow( Control control, IToolkitWindow parent, int x, int y, int w, int h ) 
	{
		// be ThreadSafe
		if( IsInvokeRequired ) {
			return (IToolkitWindow) Invoke( 
					new DelegateCreateChildWindow( this.InternalCreateChildWindow), 
					new object[] { control, parent, x, y, w, h } 
				);
		}
		
		// Check parent Toolkit too, might be null in some difficult cases. 
		if(parent != null && parent.Toolkit != null )
		{
			return parent.Toolkit.CreateChildWindow
					(parent, x, y, w, h, new ControlWeakRef(control) );
		}
		else
		{
			// Use the default toolkit to create.
			return ToolkitManager.Toolkit.CreateChildWindow
					(null, x, y, w, h, new ControlWeakRef(control) );
		}
	}
	
	#endregion 
	
	#region create top level window
	
	delegate IToolkitTopLevelWindow DelegateCreateTopLevelWindow( Control control, int w, int h );
	
	private IToolkitTopLevelWindow InternalCreateTopLevelWindow(Control control, int w, int h)
	{
		// be ThreadSafe
		if( IsInvokeRequired ) {
			return (IToolkitTopLevelWindow) Invoke( 
					new DelegateCreateTopLevelWindow( this.InternalCreateTopLevelWindow), 
			new object[] { control, w, h } );
		}
		
		return ToolkitManager.Toolkit.CreateTopLevelWindow( w, h, new ControlWeakRef(control) );
	}
	
	#endregion 
	
	#region create mdi child window as TopLevelWindow
	
	delegate IToolkitTopLevelWindow DelegateCreateMdiChildWindow( Control control, IToolkitMdiClient mdiClient, int x, int y, int w, int h );
	
	IToolkitTopLevelWindow InternalCreateMdiChildWindow( Control control, IToolkitMdiClient mdiClient, int x, int y, int w, int h ) 
	{
		// be ThreadSafe
		if( IsInvokeRequired ) {
			return (IToolkitTopLevelWindow) Invoke( 
					new DelegateCreateMdiChildWindow( this.InternalCreateMdiChildWindow), 
					new object[] { control, mdiClient, x, y, w, h } 
				);
		}
		
		return mdiClient.CreateChildWindow(x,y,w,h, new ControlWeakRef(control) );
	}
	
	#endregion
	
	#region create mdi child window as cient window
	
	delegate IToolkitWindow DelegateCreateMdiClient( Control control, IToolkitWindow parent, int x, int y, int w, int h );
	
	IToolkitWindow InternalCreateMdiClient( Control control, IToolkitWindow parent, int x, int y, int w, int h )
	{
		// be ThreadSafe
		if( IsInvokeRequired ) {
			return (IToolkitTopLevelWindow) Invoke( 
					new DelegateCreateMdiClient( this.InternalCreateMdiClient), 
					new object[] { control, parent, x, y, w, h } 
				);
		}
		
		if( null != parent ) {
			return parent.Toolkit.CreateMdiClient( parent, x, y, w, h, new ControlWeakRef(control) );
		}
		else {
			return ToolkitManager.Toolkit.CreateMdiClient(null, x, y, w, h, new ControlWeakRef(control) );
		}
	}
	
	#endregion
	
	#region create popup window
	
	delegate IToolkitWindow DelegateCreatePopupWindow(Control control, int x, int y, int w, int h );
	
	private IToolkitWindow InternalCreatePopupWindow(Control control, int x, int y, int w, int h )
	{
		// be ThreadSafe
		if( IsInvokeRequired ) {
			return (IToolkitWindow) Invoke( 
					new DelegateCreatePopupWindow( this.InternalCreatePopupWindow ), 
					new object[] { control, x, y, w, h } 
				);
		}
		
		return ToolkitManager.Toolkit.CreatePopupWindow( x,y,w,h, new ControlWeakRef(control) );
	}

	#endregion
	
	#region public methods
	
	internal static IToolkitWindow CreatePopupWindow( Control control, int x, int y, int w, int h )
	{
		return Instance.InternalCreatePopupWindow( control, x, y, w, h );
	}
	
	internal static IToolkitWindow CreateMdiClient( Control control, IToolkitWindow parent, int x, int y, int w, int h )
	{
		return Instance.InternalCreateMdiClient( control, parent, x, y, w, h );
	}
	
	internal static IToolkitTopLevelWindow CreateMdiChildWindow(Control control, IToolkitMdiClient mdiClient, int x, int y, int w, int h )
	{
		return Instance.InternalCreateMdiChildWindow( control, mdiClient, x,y,w,h );
	}
	
	internal static IToolkitTopLevelWindow CreateTopLevelWindow(Control control, int w, int h ) 
	{
		return Instance.InternalCreateTopLevelWindow(control, w, h);
	}
	
	internal static IToolkitWindow CreateChildWindow(Control control, IToolkitWindow parent, int x, int y, int w, int h ) 
	{
		return Instance.InternalCreateChildWindow(control, parent, x, y, w, h );
	}
	
	internal static bool IsInvokeRequired 
	{
		get {
			return ( Instance.mCreateThread != Thread.CurrentThread );
		}
	}
	
	#endregion

}

// Helper class to keep WeakReferences to Control
// this is needed, because we must not keep a real reference in ToolkitWindow, 
// else Control never would get disposed.
internal class ControlWeakRef : IToolkitEventSink
{
	private WeakReference mControlWeakRef;

	public ControlWeakRef( IToolkitEventSink control ) {
		this.mControlWeakRef = new WeakReference( control, false );
	}

	public void ToolkitExpose(Graphics graphics) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitExpose(graphics);
		}
	}

	public void ToolkitMouseEnter() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMouseEnter();
		}
	}

	public void ToolkitMouseLeave() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMouseLeave();
		}
	}

	public void ToolkitFocusEnter() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitFocusEnter();
		}
	}

	public void ToolkitFocusLeave() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitFocusLeave();
		}
	}

	public void ToolkitPrimaryFocusEnter() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitPrimaryFocusEnter();
		}
	}

	public void ToolkitPrimaryFocusLeave() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitPrimaryFocusLeave();
		}
	}

	public bool ToolkitKeyDown(ToolkitKeys key) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			return co.ToolkitKeyDown(key);
		}
		return false;
	}

	public bool ToolkitKeyUp(ToolkitKeys key) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			return co.ToolkitKeyUp(key);
		}
		return false;
	}

	public bool ToolkitKeyChar(char charCode) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			return co.ToolkitKeyChar(charCode);
		}
		return false;
	}

	public void ToolkitMouseDown(ToolkitMouseButtons buttons, ToolkitKeys modifiers, int clicks, int x, int y, int delta) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMouseDown(buttons, modifiers, clicks, x, y, delta);
		}
	}

	public void ToolkitMouseUp(ToolkitMouseButtons buttons, ToolkitKeys modifiers, int clicks, int x, int y, int delta) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMouseUp(buttons, modifiers, clicks, x, y, delta);
		}
	}

	public void ToolkitMouseHover(ToolkitMouseButtons buttons, ToolkitKeys modifiers, int clicks, int x, int y, int delta) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMouseHover(buttons, modifiers, clicks, x, y, delta);
		}
	}

	public void ToolkitMouseMove(ToolkitMouseButtons buttons, ToolkitKeys modifiers, int clicks, int x, int y, int delta) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMouseMove(buttons, modifiers, clicks, x, y, delta);
		}
	}

	public void ToolkitMouseWheel(ToolkitMouseButtons buttons, ToolkitKeys modifiers, int clicks, int x, int y, int delta) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMouseWheel(buttons, modifiers, clicks, x, y, delta);
		}
	}

	public void ToolkitExternalMove(int x, int y) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitExternalMove(x,y);
		}
	}

	public void ToolkitExternalResize(int width, int height) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitExternalResize(width,height);
		}
	}

	public void ToolkitClose() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitClose();
		}
	}

	public void ToolkitHelp() {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitHelp();
		}
	}

	public void ToolkitStateChanged(int state) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitStateChanged(state);
		}
	}

	public void ToolkitMdiActivate(IToolkitWindow child) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitMdiActivate(child);
		}
	}

	public void ToolkitBeginInvoke(IntPtr i_gch) {
		IToolkitEventSink co = this.mControlWeakRef.Target as IToolkitEventSink;
		if( null != co ) {
			co.ToolkitBeginInvoke(i_gch);
		}
	}

}

}