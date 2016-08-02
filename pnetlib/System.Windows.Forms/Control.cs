/*
 * Control.cs - Implementation of the
 *			"System.Windows.Forms.Control" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * Copyright (C) 2003  Neil Cawse.
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

namespace System.Windows.Forms
{

using System;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Drawing.Text;
using System.ComponentModel;
using System.Drawing.Toolkit;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.ComponentModel.Design.Serialization;

#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
[Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
[DefaultProperty("Text")]
[DefaultEvent("Click")]
#endif
#if CONFIG_COMPONENT_MODEL_DESIGN
[DesignerSerializer("System.Windows.Forms.Design.ControlCodeDomSerializer, System.Design",
			 "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
#endif
#if CONFIG_COMPONENT_MODEL
[ToolboxItemFilter("System.Windows.Forms")]
public class Control : Component, ISynchronizeInvoke, IWin32Window
#else
public class Control : IWin32Window, IDisposable
#endif
		, IToolkitEventSink
{
	// Internal state.
	internal IToolkitWindow toolkitWindow;
	private Control parent;
	// Outside bounds of the control including windows decorations
	// in the case of forms and non client areas
	private int left, top, width, height;
	// Distances for anchors
	private bool updateDistances;
	private int distLeft, distRight, distTop, distBottom;
	internal String text;
	private String name;
	private HookedEvent hookedEvents;
	private Control[] children;
	private int numChildren;
	private BindingContext bindingContext;
	private ControlFlags flags;
	internal bool visible;
	internal Color backColor;
	internal Color foreColor;
	private Font font;
	private Image backgroundImage;
	private byte anchorStyles;
	private byte dockStyle;
	private byte imeMode;
	private byte rightToLeft;
	private int tabIndex;
	private int layoutSuspended;
	private Object tag;
	private CreateParams currentParams;
	private ContextMenu contextMenu;
	private static Font defaultFont;
#if !CONFIG_COMPACT_FORMS
	private AccessibleObject accessibilityObject;
#endif
	private static Keys currentModifiers;
	private BorderStyle borderStyle;
	private static Point mousePosition;
	private static MouseButtons mouseButtons;
	private int controlStyle;
	// The thread that was used to create the control.
	// private Thread createThread; not needed anymore
	private Cursor cursor;
	private IToolkitWindowBuffer buffer;
	private ControlBindingsCollection controlBindingsCollection;
	private ControlCollection controlCollection;
	static private Timer hoverTimer;
	static private Control hoverControl;



	// Miscellaneous flags for controls.
	[Flags]
	private enum ControlFlags
	{
		None                = 0x0000,
		Enabled             = 0x0001,
		TabStop             = 0x0002,
		AllowDrop           = 0x0004,
		CausesValidation    = 0x0008,
		Disposed            = 0x0010,
		Disposing           = 0x0020,
		LayoutInitSuspended = 0x0040,
		PerformingLayout    = 0x0080,
		NotifyClick         = 0x0100,
		NotifyDoubleClick   = 0x0200,
		ValidationCancelled = 0x0400,
		NeedReparent        = 0x0800,
		Default             = (Enabled | CausesValidation | TabStop)

	}; // enum ControlFlags

	//
	// Implentation of classes and variables for Invoke/BeginInvoke/EndInvoke
	//
	//
	// This is the IAsyncResult class associated with a BeginInvoke
	//
	internal class InvokeAsyncResult: IAsyncResult
	{
		private bool bComplete;
		private ManualResetEvent waitHandle;
		public Object retObject;
		public Object asyncStateObject;	// The AsyncState object

		public InvokeAsyncResult()
		{
			bComplete = false;			// This event hasn't completed
			waitHandle = new ManualResetEvent(false);
		}
		
		~InvokeAsyncResult() {
			if( null != waitHandle ) {
				waitHandle.Close();
			}
		}

		public void WaitToComplete()
		{
			lock(this)				// Lock ourselve for thread safety
			{
				if(bComplete)		// If we have already completed
				{
					return;			// just blow out of here
				}
			}
			waitHandle.WaitOne();	// wait for the vent to be fired 
		}

		public void SetComplete()
		{
			lock(this)
			{
				bComplete = true;
			}
			
			waitHandle.Set(); // fire the event. this would not work with mutex. but works with events.
		}

		public Object AsyncState
		{
			get
			{
				return asyncStateObject;
			}
			set
			{
				asyncStateObject = value;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return waitHandle;
			}
		}
		
		public bool CompletedSynchronously
		{
			get
			{
				return false;
			}
		}

		public bool IsCompleted
		{
			get
			{
				lock(this)
				{
					return bComplete;
				}
			}
		}
	}

	//
	// This is the class that keeps the delegate, the parameters,
	// and a weak reference to the IAsyncResult (InvokeAsyncResult)
	// InvokeAsyncresult ar = wr.Target
	//
	internal class InvokeParameters
	{
		public Delegate method;
		public Object[] args;
		public InvokeAsyncResult wr;
	}

	// Constructors.
	public Control()
			{
				this.name = String.Empty;
				this.flags = ControlFlags.Default;
				this.visible = true;
				this.anchorStyles =
					(byte)(AnchorStyles.Top | AnchorStyles.Left);
				this.imeMode = (byte)DefaultImeMode;
				this.rightToLeft = (byte)(RightToLeft.Inherit);
				this.tabIndex = -1;
				borderStyle = BorderStyle.None;
				// Create the currentParams
				currentParams = new CreateParams();
				currentParams = CreateParams;
				SetStyle( ControlStyles.UserPaint |
					ControlStyles.StandardClick |
					ControlStyles.Selectable |
					ControlStyles.StandardDoubleClick |
					ControlStyles.AllPaintingInWmPaint, true);
				Size initialSize = DefaultSize;
				width = initialSize.Width;
				height = initialSize.Height;

				controlBindingsCollection = new ControlBindingsCollection(this);
				if( null == hoverTimer ) 
				{
					hoverTimer = new Timer();
					hoverTimer.Interval = 1000;
					hoverTimer.Enabled = false;
					hoverTimer.Tick += new EventHandler(ProcessHoverTimerEvent);
				}
				
				updateDistances = true;
			}
	public Control(String text) : this()
			{
				this.Text = text;
			}
	public Control(Control parent, String text) : this()
			{
				this.Text = text;
				if(parent != null)
				{
					this.Parent = parent;
				}
			}
	public Control(String text, int left, int top, int width, int height)
		: this()
			{
				this.Text = text;
				this.Bounds = new Rectangle(left, top, width, height);
			}
	public Control(Control parent, String text,
		int left, int top, int width, int height)
		: this()
			{
				this.Text = text;
				if(parent != null)
				{
					this.Parent = parent;
				}
				this.Bounds = new Rectangle(left, top, width, height);
			}

#if !CONFIG_COMPONENT_MODEL

	// Destructor.
	~Control()
			{
				Dispose(false);
			}

#endif

	// Create the toolkit window underlying this control.
	internal virtual IToolkitWindow CreateToolkitWindow(IToolkitWindow parent)
			{
				// Because we use owner-draw in this implementation
				// for all widgets, we are only interested in the
				// position and size information from "CreateParams".
				CreateParams cp = CreateParams;
				// Convert to toolkit coordinates
				int x = cp.X + ToolkitDrawOrigin.X;
				int y = cp.Y + ToolkitDrawOrigin.Y;
				int width = cp.Width - ToolkitDrawSize.Width;
				int height = cp.Height - ToolkitDrawSize.Height;
					
				if(parent != null)
				{
					// Offset the co-ordinates to the client origin.
					// the toolkit co-ordinates must be relative to the draw origin.
					x += Parent.ClientOrigin.X - Parent.ToolkitDrawOrigin.X;
					y += Parent.ClientOrigin.Y - Parent.ToolkitDrawOrigin.Y; 
				}
				// use ControlToolkitManager to create the window thread safe
				return ControlToolkitManager.CreateChildWindow( this, parent, x, y, width, height );
			}


	// Process a hoverTimer event
	static private void ProcessHoverTimerEvent(object sender, EventArgs e)
	{
		if( hoverControl != null ) 
		{
			hoverControl.OnMouseHover(e);
		}
	}

#if CONFIG_COMPONENT_MODEL

	private Queue invokeEventQueue = new Queue();

	// Implement the ISynchronizeInvoke interface.
	private void ProcessInvokeEvent(IntPtr i_gch)	
	{
		while( true ) {
			InvokeParameters iParm = null;
			
			lock( this.invokeEventQueue ) {
				
				if( this.invokeEventQueue.Count > 0 ) {
					iParm = (InvokeParameters) invokeEventQueue.Dequeue();
				}
				else {
					iParm = null;
				}
			}
			
			if( null == iParm ) break;	// no more items
		
			Delegate dg = iParm.method;
			
			Object ro = null;
			
			try {
				ro = dg.DynamicInvoke(iParm.args);
			}
			finally {
				InvokeAsyncResult ar = iParm.wr;
				
				if( ar != null )
				{
					ar.retObject = ro;
					ar.SetComplete();
				}
			}
		}
	}

	[TODO]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IAsyncResult BeginInvoke(Delegate method, Object[] args)
			{
				lock( this.invokeEventQueue ) {

					InvokeParameters iParm = new InvokeParameters();
					InvokeAsyncResult ar = new InvokeAsyncResult();
					if( args != null )
					{
						ar.AsyncState = args[args.Length - 1];
					}
					
					iParm.method = method;
					iParm.args   = args;
					iParm.wr = ar;
	
					if(toolkitWindow == null)
					{
						CreateControlInner();
					}
					
					this.invokeEventQueue.Enqueue( iParm );
	
					toolkitWindow.SendBeginInvoke(IntPtr.Zero);
	
					return ar;
				}
			}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public IAsyncResult BeginInvoke(Delegate method)
			{
				return BeginInvoke(method,null);
			}

	[TODO]
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public Object EndInvoke(IAsyncResult result)
			{
				InvokeAsyncResult ar = (result as InvokeAsyncResult);
				if( !ar.IsCompleted ) {	// it could be that we processed it already, so it might be completed
					if( !InvokeRequired ) {
						// we do not need to wait for the callback from toolkit.
						// all waiting invokes will be processed.
						this.ProcessInvokeEvent( IntPtr.Zero );	
					}
					else {
						ar.WaitToComplete();
					}
				}
				return ar.retObject;
			}

	public Object Invoke(Delegate method, Object[] args)
			{
				if( !InvokeRequired ) {		// no need to use toolkit, do the invoke directly
					return method.DynamicInvoke( args );
				}
				InvokeAsyncResult ar = this.BeginInvoke(method,args) as InvokeAsyncResult;
				ar.WaitToComplete();
				return ar.retObject;
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool InvokeRequired
			{
				get
				{
					// return (createThread != null && createThread != Thread.CurrentThread); not needed any more
					return ControlToolkitManager.IsInvokeRequired;
				}
			}

#endif // CONFIG_COMPONENT_MODEL

	// Get or set the control's properties.
#if !CONFIG_COMPACT_FORMS
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public AccessibleObject AccessibilityObject
			{
				get
				{
					if(accessibilityObject == null)
					{
						accessibilityObject = CreateAccessibilityInstance();
					}
					return accessibilityObject;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public String AccessibleDefaultActionDescription
			{
				get
				{
					return AccessibilityObject.defaultAction;
				}
				set
				{
					AccessibilityObject.defaultAction = value;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue("")]
#endif
	public String AccessibleDescription
			{
				get
				{
					return AccessibilityObject.description;
				}
				set
				{
					AccessibilityObject.description = value;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue("")]
#endif
	public String AccessibleName
			{
				get
				{
					return AccessibilityObject.name;
				}
				set
				{
					AccessibilityObject.name = value;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(AccessibleRole.Default)]
#endif
	public AccessibleRole AccessibleRole
			{
				get
				{
					return AccessibilityObject.role;
				}
				set
				{
					AccessibilityObject.role = value;
				}
			}
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(false)]
#endif
	public virtual bool AllowDrop
			{
				get
				{
					return GetControlFlag(ControlFlags.AllowDrop);
				}
				set
				{
					SetControlFlag(ControlFlags.AllowDrop, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(AnchorStyles.Top | AnchorStyles.Left)]
#endif
	public virtual AnchorStyles Anchor
			{
				get
				{
					return (AnchorStyles)anchorStyles;
				}
				set
				{
					anchorStyles = (byte)value;
				}
			}
#if !ECMA_COMPAT
	[DispId(-501)]
#endif
	[TODO]
	public virtual Color BackColor
			{
				get
				{
					if(!(backColor.IsEmpty))
					{
						return backColor;
					}
					else if(parent != null)
					{
						return parent.BackColor;
					}
					else
					{
						return DefaultBackColor;
					}
				}
				set
				{
					if(value != backColor)
					{
						if(value.A < 255 &&
						   !GetStyle(ControlStyles.SupportsTransparentBackColor))
						{
							throw new ArgumentException("value"); // Fill in with appropriate message
						}
						backColor = value;
						OnBackColorChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(null)]
#endif
	public virtual Image BackgroundImage
			{
				get
				{
					return backgroundImage;
				}
				set
				{
					if(value != backgroundImage)
					{
						backgroundImage = value;
						OnBackgroundImageChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public virtual BindingContext BindingContext
			{
				get
				{
					if(bindingContext != null)
					{
						return bindingContext;
					}
					else if(parent != null)
					{
						return parent.BindingContext;
					}
					else
					{
						return null;
					}
				}
				set
				{
					if(bindingContext != value)
					{
						bindingContext = value;
						OnBindingContextChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public int Bottom
			{
				get
				{
					return top + height;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public Rectangle Bounds
			{
				get
				{
					return new Rectangle(left, top, width, height);
				}
				set
				{
					if (value != Bounds)
						SetBoundsCore(value.Left, value.Top, value.Width, 
							value.Height, BoundsSpecified.All);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool CanFocus
			{
				get
				{
					return (Visible && Enabled && GetStyle(ControlStyles.Selectable));
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool CanSelect
			{
				get
				{
					if (!GetStyle(ControlStyles.Selectable))
						return false;
					for (Control control = this; control != null; control = control.parent)
					{
						if (!control.Visible || !control.Enabled)
							return false;
					}
					return true;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(true)]
#endif
	public bool CausesValidation
			{
				get
				{
					return GetControlFlag(ControlFlags.CausesValidation);
				}
				set
				{
					if(GetControlFlag(ControlFlags.CausesValidation) != value)
					{
						SetControlFlag(ControlFlags.CausesValidation, value);
						OnCausesValidationChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool Capture
			{
				get
				{
					if(toolkitWindow != null)
					{
						return toolkitWindow.Capture;
					}
					else
					{
						return false;
					}
				}
				set
				{
					if(toolkitWindow != null)
					{
							toolkitWindow.Capture = value;
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public Rectangle ClientRectangle
			{
				get
				{
					Size size = ClientSize;
					return new Rectangle(0, 0, size.Width, size.Height);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public Size ClientSize
			{
				get
				{
					Size offset = ClientToBounds(Size.Empty);
					return new Size(width - offset.Width, height - offset.Height);
				}
				set
				{
					SetClientSizeCore(value.Width, value.Height);
				}
			}
#if !ECMA_COMPAT
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public String CompanyName
			{
				get
				{
					Assembly assembly = GetType().Module.Assembly;
					Object[] attrs = assembly.GetCustomAttributes
						(typeof(AssemblyCompanyAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						return ((AssemblyCompanyAttribute)(attrs[0])).Company;
					}
					return assembly.GetName().Name;
				}
			}
#endif // !ECMA_COMPAT
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Description("ControlCompanyNameDescr")]
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool ContainsFocus
			{
				get
				{
					if(toolkitWindow != null && toolkitWindow.Focused)
					{
						return true;
					}
					for(int i = (numChildren - 1); i >= 0; --i)
					{
						if(children[i].ContainsFocus)
						{
							return true;
						}
					}
					return false;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(null)]
#endif
	public virtual ContextMenu ContextMenu
			{
				get
				{
					return contextMenu;
				}
				set
				{
					if(contextMenu != null)
					{
						contextMenu.RemoveFromControl();
					}
					contextMenu = value;
					if(contextMenu != null)
					{
						Control control = contextMenu.SourceControl;
						if(control != null)
							control.ContextMenu = null;
						contextMenu.AddToControl(this);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
	public ControlCollection Controls
			{
				get
				{
					if(controlCollection == null)
					{
						controlCollection = CreateControlsInstance();
					}
					return controlCollection;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool Created
			{
				get
				{
					return (toolkitWindow != null);
				}
			}
	protected virtual CreateParams CreateParams
			{
				get
				{
					CreateParams cp = new CreateParams();
					cp.Caption = text;
					cp.X = left;
					cp.Y = top;
					cp.Width = width;
					cp.Height = height;
					cp.ClassStyle = Win32Constants.CS_DBLCLKS;
					cp.Style = Win32Constants.WS_CLIPCHILDREN;
					if(GetStyle(ControlStyles.ContainerControl))
					{
						cp.ExStyle = Win32Constants.WS_EX_CONTROLPARENT;
					}
					if(!IsTopLevel)
					{
						cp.Style |= Win32Constants.WS_CHILD |
							Win32Constants.WS_CLIPSIBLINGS;
					}
					if(GetControlFlag(ControlFlags.TabStop))
					{
						cp.Style |= Win32Constants.WS_TABSTOP;
					}
					if(visible)
					{
						cp.Style |= Win32Constants.WS_VISIBLE;
					}
					if(!Enabled)
					{
						cp.Style |= Win32Constants.WS_DISABLED;
					}
					if(RightToLeft == RightToLeft.Yes)
					{
						cp.ExStyle |= Win32Constants.WS_EX_LEFTSCROLLBAR |
							Win32Constants.WS_EX_RTLREADING |
							Win32Constants.WS_EX_RIGHT;
					}
					return cp;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(null)]
#endif
	public virtual Cursor Cursor
			{
				get
				{
					if(((Object)cursor) != null)
					{
						return cursor;
					}
					else if(parent != null)
					{
						return parent.Cursor;
					}
					else
					{
						return Cursors.Default;
					}
				}
				set
				{
					if(cursor != value)
					{
						cursor = value;
						if(toolkitWindow != null)
						{
							if(value != null)
							{
								value.SetCursorOnWindow(toolkitWindow);
							}
							else
							{
								toolkitWindow.SetCursor
									(ToolkitCursorType.InheritParent, null);
							}
						}
						OnCursorChanged(EventArgs.Empty);
					}
				}
			}
	[TODO]
#if CONFIG_COMPONENT_MODEL 
	[ParenthesizePropertyName(true)]
	[RefreshProperties(RefreshProperties.All)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#endif
	public ControlBindingsCollection DataBindings
			{
				get
				{
					if( null == controlBindingsCollection ) {
						controlBindingsCollection = new ControlBindingsCollection(this);
					}
					return controlBindingsCollection;
				}
			}
	public static Color DefaultBackColor
			{
				get
				{
					return SystemColors.Control;
				}
			}
	public static Font DefaultFont
			{
				get
				{
					lock(typeof(Control))
					{
						if(defaultFont == null)
						{
							defaultFont =
								ToolkitManager.Toolkit.CreateDefaultFont();
						}
						return defaultFont;
					}
				}
			}
	public static Color DefaultForeColor
			{
				get
				{
					return SystemColors.ControlText;
				}
			}
	protected virtual ImeMode DefaultImeMode
			{
				get
				{
					return ImeMode.Inherit;
				}
			}
	protected virtual Size DefaultSize
			{
				get
				{
					return new Size(0, 0);
				}
			}
	protected int FontHeight
			{
				get
				{
					return Font.Height;
				}
				set
				{
					// The spec says that we can only set this to Font.Height,
					// or to -1 to clear the cached value.  Since we don't
					// cache font height values, we have nothing to do here.
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public virtual Rectangle DisplayRectangle
			{
				get
				{
					return ClientRectangle;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool Disposing
			{
				get
				{
					return GetControlFlag(ControlFlags.Disposing);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
	[RefreshProperties(RefreshProperties.Repaint)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(DockStyle.None)]
#endif
	public virtual DockStyle Dock
			{
				get
				{
					return (DockStyle)dockStyle;
				}
				set
				{
					if(dockStyle != (byte)value)
					{
						dockStyle = (byte)value;
						OnDockChanged(EventArgs.Empty);
						// Rethink our layout
						PerformLayout(this,"Dock");
						if(parent != null) {
							parent.PerformLayout(this, "Dock");
							if( value != DockStyle.None ) parent.PerformActualLayout();
						}
					}
				}
			}
#if !ECMA_COMPAT
	[DispId(-514)]
#endif
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
	public bool Enabled
			{
				get
				{
					if(!GetControlFlag(ControlFlags.Enabled))
					{
						// There is no point going further up the tree.
						return false;
					}
					else if(parent != null)
					{
						return parent.Enabled;
					}
					else
					{
						return true;
					}
				}
				set
				{
					if(GetControlFlag(ControlFlags.Enabled) != value)
					{
						SetControlFlag(ControlFlags.Enabled, value);
						OnEnabledChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public virtual bool Focused
			{
				get
				{
					if(toolkitWindow != null)
					{
						return toolkitWindow.Focused;
					}
					else
					{
						return false;
					}
				}
			}
#if !ECMA_COMPAT
	[DispId(-512)]
#endif
#if CONFIG_COMPONENT_MODEL
	[AmbientValue(null)]
	[Localizable(true)]
#endif
	public virtual Font Font
			{
				get
				{
					// Inherit our parent's font if necessary.
					Control control = parent;
					Font font = this.font;
					while(font == null && control != null)
					{
						font = control.font;
						control = control.parent;
					}
					if(font != null)
					{
						return font;
					}
					else
					{
						return DefaultFont;
					}
				}
				set
				{
					if(font != value)
					{
						font = value;
						OnFontChanged(EventArgs.Empty);
					}
				}
			}
#if !ECMA_COMPAT
	[DispId(-513)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public virtual Color ForeColor
			{
				get
				{
					if(!(foreColor.IsEmpty))
					{
						return foreColor;
					}
					else if(parent != null)
					{
						return parent.ForeColor;
					}
					else
					{
						return DefaultForeColor;
					}
				}
				set
				{
					if(value != foreColor)
					{
						foreColor = value;
						OnForeColorChanged(EventArgs.Empty);
					}
				}
			}
#if !ECMA_COMPAT
	[DispId(-515)]
#endif
	public IntPtr Handle
			{
				get
				{
					if(toolkitWindow != null)
					{
						return toolkitWindow.GetHwnd();
					}
					else
					{
						return IntPtr.Zero;
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool HasChildren
			{
				get
				{
					return (numChildren > 0);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Always)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public int Height
			{
				get
				{
					return height;
				}
				set
				{
					if (value != height)
					{
						SetBoundsCore(left, top, width, value,
							BoundsSpecified.Height);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[AmbientValue(ImeMode.Inherit)]
	[Localizable(true)]
#endif
	public ImeMode ImeMode
			{
				get
				{
					if ((ImeMode)imeMode == ImeMode.Inherit)
					{
						if (parent != null)
						{
							return parent.ImeMode;
						}
						else
						{
							// return disabled = default
							return ImeMode.Disable;
						}
					}
					else
					{
						return (ImeMode)imeMode;
					}
				}
				set
				{
					if(imeMode != (byte)value)
					{
						imeMode = (byte)value;
						OnImeModeChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool IsAccessible
			{
				get
				{
					// By default, we assume that everything is accessible.
					return true;
				}
				set
				{
					// Not used in this implementation.
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool IsDisposed
			{
				get
				{
					return GetControlFlag(ControlFlags.Disposed);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool IsHandleCreated
			{
				get
				{
					return (toolkitWindow != null);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Always)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public int Left
			{
				get
				{
					return left;
				}
				set
				{
					if(value != left)
					{
						SetBoundsCore(value, top, width, height,
							BoundsSpecified.X);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
	public Point Location
			{
				get
				{
					return new Point(left, top);
				}
				set
				{
					if(value.X != left || value.Y != top)
					{
						SetBoundsCore(value.X, value.Y, width, height,
							BoundsSpecified.Location);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public String Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public Control Parent
			{
				get
				{
					return parent;
				}
				set
				{
					// Bail out if setting the parent to the same value.
					if(value == parent)
					{
						return;
					}

					// Cannot set the parent of top-level controls.
					if(value != null && IsTopLevel)
					{
						throw new ArgumentException
							(S._("SWF_SettingTopLevelParent"));
					}

					// Check for circularities.
					if(value != null && OccursIn(value))
					{
						throw new ArgumentException
							(S._("SWF_CircularityDetected"));
					}

					// Remove the control from its current parent.
					int posn;
					if(parent != null)
					{
						posn = 0;
						while(posn < parent.numChildren)
						{
							if(parent.children[posn] == this)
							{
								--(parent.numChildren);
								while(posn < parent.numChildren)
								{
									parent.children[posn] =
										parent.children[posn + 1];
									posn++;
								}
								/* 
								Don't forget to set to null, 
								Or a reference of the would be kept !!!
								*/
								parent.children[posn] = null; 
								break;
							}
							posn++;
						}
					}

					// Add the control to its new parent.
					parent = value;
					if(value != null)
					{
						if(value.children == null)
						{
							value.children = new Control [4];
						}
						else if(value.numChildren >= value.children.Length)
						{
							Control[] newChildren;
							newChildren = new Control [value.numChildren * 2];
							Array.Copy(value.children, 0, newChildren, 0,
								value.numChildren);
							value.children = newChildren;
						}
						value.children[(value.numChildren)++] = this;
						value.OnControlAdded(new ControlEventArgs(this));
					}

					// Reparent the control within the windowing system.
					Reparent(value);

					// Notify event handlers that the parent changed.
					OnParentChanged(EventArgs.Empty);

					// If we were using our parent's binding context,
					// then that has changed as well.
					if(bindingContext == null)
					{
						if( Created ) {
							OnBindingContextChanged(EventArgs.Empty);
						}
					}

					// Initialize layout for calculating the anchor.
					InitLayout();
				}
			}
#if !ECMA_COMPAT
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public String ProductName
			{
				get
				{
					Assembly assembly = GetType().Module.Assembly;
					Object[] attrs = assembly.GetCustomAttributes
						(typeof(AssemblyProductAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						return ((AssemblyProductAttribute)(attrs[0])).Product;
					}
					return assembly.GetName().Name;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public String ProductVersion
			{
				get
				{
					Assembly assembly = GetType().Module.Assembly;
					Object[] attrs = assembly.GetCustomAttributes
						(typeof(AssemblyInformationalVersionAttribute), false);
					if(attrs != null && attrs.Length > 0)
					{
						return ((AssemblyInformationalVersionAttribute)
							(attrs[0])).InformationalVersion;
					}
					return assembly.GetName().Version.ToString();
				}
			}
#endif
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public bool RecreatingHandle
			{
				get
				{
					// We create it once and dispose it once.
					return false;
				}
			}
	[TODO]
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public Region Region
			{
				get
				{
					// Fill in here
					return null;
				}
				set
				{
					// Fill in here
				}
			}
	protected bool ResizeRedraw
			{
				get
				{
					return GetStyle(ControlStyles.ResizeRedraw);
				}
				set
				{
					SetStyle(ControlStyles.ResizeRedraw, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public int Right
			{
				get
				{
					return left + width;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[AmbientValue(RightToLeft.Inherit)]
	[Localizable(true)]
#endif
	public virtual RightToLeft RightToLeft
			{
				get
				{
					if(rightToLeft != (byte)(RightToLeft.Inherit))
					{
						return (RightToLeft)rightToLeft;
					}
					else if(parent != null)
					{
						return parent.RightToLeft;
					}
					else
					{
						return RightToLeft.No;
					}
				}
				set
				{
					if(rightToLeft != (byte)value)
					{
						rightToLeft = (byte)value;
						OnRightToLeftChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	protected virtual bool ShowFocusCues
			{
				get
				{
					if(parent != null)
					{
						return parent.ShowFocusCues;
					}
					else
					{
						return true;
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	protected virtual bool ShowKeyboardCues
			{
				get
				{
					if(parent != null)
					{
						return parent.ShowKeyboardCues;
					}
					else
					{
						return true;
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	public override ISite Site
			{
				get
				{
					return base.Site;
				}
				set
				{
					base.Site = value;
				}
			}
#endif
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
	public Size Size
			{
				get
				{
					return new Size(width, height);
				}
				set
				{
					if(value.Width != width || value.Height != height)
					{
						SetBoundsCore(left, top, value.Width, value.Height,
							BoundsSpecified.Size);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
	[MergableProperty(false)]
#endif
	public int TabIndex
			{
				get
				{
					if (tabIndex == -1)
						return 0;
					return tabIndex;
				}
				set
				{
					if(tabIndex != value)
					{
						tabIndex = value;
						OnTabIndexChanged(EventArgs.Empty);
					}
				}
			}
#if !ECMA_COMPAT
	[DispId(-516)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[DefaultValue(true)]
#endif
	public bool TabStop
			{
				get
				{
					return GetControlFlag(ControlFlags.TabStop);
				}
				set
				{
					if(GetControlFlag(ControlFlags.TabStop) != value)
					{
						SetControlFlag(ControlFlags.TabStop, value);
						OnTabStopChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(false)]
	[Bindable(true)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[TypeConverter(typeof(StringConverter))]
	[DefaultValue(null)]
#endif
	public Object Tag
			{
				get
				{
					return tag;
				}
				set
				{
					tag = value;
				}
			}
#if !ECMA_COMPAT
	[DispId(-517)]
#endif
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
	public virtual String Text
			{
				get
				{
					if( null == text ) return String.Empty;
					return text;
				}
				set
				{
					if(text != value)
					{
						text = value;
						OnTextChanged(EventArgs.Empty);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public int Top
			{
				get
				{
					return top;
				}
				set
				{
					if(value != top)
					{
						SetBoundsCore(left, value, width, height,
							BoundsSpecified.Y);
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public Control TopLevelControl
			{
				get
				{
					Control ctrl = this;
					Control parent;
					while((parent = ctrl.Parent) != null)
					{
						ctrl = ctrl.Parent;
					}
					return ctrl;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[Localizable(true)]
#endif
	public bool Visible
			{
				get
				{
					if(!visible)
					{
						// There is no point going further up the tree.
						return false;
					}
					else if(parent != null)
					{
						return parent.Visible;
					}
					else
					{
						return true;
					}
				}
				set
				{
					if( value != visible )	// check last state here, even is checked in SetVisibleCore to get more performance
						SetVisibleCore(value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Always)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
#endif
	public int Width
			{
				get
				{
					return width;
				}
				set
				{
					if(value != width)
					{
						SetBoundsCore(left, top, value, height,
							BoundsSpecified.Width);
					}
				}
			}
	internal virtual bool IsTopLevel
			{
				get
				{
					// "IsTopLevel" indicates if the control class
					// cannot support parents (e.g. app windows).
					// Most controls can support parents.
					return false;
				}
			}

	// Get the global state of the modifier keys.
	public static Keys ModifierKeys
			{
				get
				{
					return (currentModifiers & Keys.Modifiers);
				}
			}

	[TODO]
	// Get the global state of the mouse buttons.
	// TODO: This only works when the mouse is within the bounds of a form
	public static MouseButtons MouseButtons
			{
				get
				{
					return mouseButtons;
				}
			}

	[TODO]
	// Get the current screen position of the mouse.
	// TODO: This only works when the mouse is within the bounds of a form
	public static Point MousePosition
			{
				get
				{
					return mousePosition;
				}
			}

	// Determine if a control occurs in a tree of controls.
	// This is used to detect circularities.
	private bool OccursIn(Control control)
			{
				if(control == this)
				{
					return true;
				}
				int posn;
				for(posn = 0; posn < numChildren; ++posn)
				{
					if(children[posn].OccursIn(control))
					{
						return true;
					}
				}
				return false;
			}

#if !CONFIG_COMPACT_FORMS

	// Notify client applications of accessibility events.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void AccessibilityNotifyClients
		(AccessibleEvents accEvent, int childID)
			{
				// Not used in this implementation.
			}

	// Create the accessibility object for this control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual AccessibleObject CreateAccessibilityInstance()
			{
				return new AccessibleObject(this);
			}

#endif

	// Bring the control to the front of its sibling stack.
	public void BringToFront()
			{
				if(parent != null)
				{
					if(parent.children[0] != this)
					{
						Control sibling = parent.children[0];
						int posn = 0;
						while(parent.children[posn] != this)
						{
							++posn;
						}
						while(posn > 0)
						{
							parent.children[posn] = parent.children[posn - 1];
							--posn;
						}
						parent.children[0] = this;
					}
				}
				if(toolkitWindow != null)
				{
					toolkitWindow.Raise();
				}
			}

	// Determine if this control contains a particular child.
	public bool Contains(Control ctl)
			{
				while (ctl != null)
				{
					ctl = ctl.parent;
					if (ctl == this)
						return true; 
				}
				return false;
			}

	// Force the control to be created.
	public void CreateControl()
			{
				// Only create if needed
				if(toolkitWindow != null) { return; }

				// Cannot create the control if it has been disposed.
				if(GetControlFlag(ControlFlags.Disposed))
				{
					throw new ObjectDisposedException
						("control", S._("SWF_ControlDisposed"));
				}

				this.CreateControlInner();

				// If one of the parents of this control is not visible then the control
				// will not be created. We must ensure that the control is created, even if
				// its parent isnt.
				if(toolkitWindow == null)
				{
					CreateHandle();
				}
			}
	
	private void CreateControlInner()
			{
				// Create the handle for this control only if needed.
				if(toolkitWindow == null) 
				{
					CreateHandle();
				}

				// Create the child controls.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					// Get the current child.
					Control child = children[posn];

					// We only need to create a control if its visible.
					if(child.visible)
					{
						// Update the windowing system z-order.
						if(child.toolkitWindow != null)
						{
							child.toolkitWindow.Raise();
						}

						// Create the child control.
						child.CreateControlInner();
					}
				}

				// Map the control to the screen if it is visible.
				if(visible && toolkitWindow != null)
				{
					toolkitWindow.IsMapped = true;
				}

				// Notify subclasses that the create has occurred.
				OnCreateControl();

				// Record a change in binding context if necessary.
				if(bindingContext == null && parent != null)
				{
					OnBindingContextChanged(EventArgs.Empty);
				}
			}

	// Create a new control collection for this instance.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual ControlCollection CreateControlsInstance()
			{
				return new ControlCollection(this);
			}

	// Create the handle for this control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void CreateHandle()
			{
				if( toolkitWindow != null ) return; // already created
				
				// Cannot create the control if it has been disposed.
				if(GetControlFlag(ControlFlags.Disposed))
				{
					throw new ObjectDisposedException
						("control", S._("SWF_ControlDisposed"));
				}

				// Create the handle using the toolkit.
				if(parent != null)
				{
					toolkitWindow = CreateToolkitWindow(parent.toolkitWindow);

					// Mark this control as requiring a reparent, if needed.
					if(parent.toolkitWindow == null)
					{
						SetControlFlag(ControlFlags.NeedReparent, true);
					}
				}
				else
				{
					toolkitWindow = CreateToolkitWindow(null);
				}

				// Dont think we need this - Neil
				//toolkitWindow.Lower();

				// Copy color information into the toolkit window.
				toolkitWindow.SetForeground(ForeColor);
				toolkitWindow.SetBackground(BackColor);
				// TODO: background images

				// Set the initial cursor if we aren't inheriting our parent.
				if(cursor != null)
				{
					cursor.SetCursorOnWindow(toolkitWindow);
				}

				// createThread = Thread.CurrentThread; not needed anymore

				// Reparent the children which require it.
				for(int i = 0; i < numChildren; ++i)
				{
					if(children[i].GetControlFlag(ControlFlags.NeedReparent))
					{
						children[i].Reparent(this);
					}
				}

				// Notify subclasses that the handle has been created.
				OnHandleCreated(EventArgs.Empty);
			}

	// Create a graphics drawing object for the control.
	public Graphics CreateGraphics()
			{
				CreateControl();
				if(toolkitWindow != null)
				{
					return ToolkitManager.CreateGraphics(
						toolkitWindow.GetGraphics(),
						new Rectangle( ClientOrigin - new Size(ToolkitDrawOrigin), ClientSize));
				}
				else
				{
					return null;
				}
			}

	public Graphics CreateNonClientGraphics()
			{
				CreateControl();
				if(toolkitWindow != null)
				{
					return ToolkitManager.CreateGraphics
						(toolkitWindow.GetGraphics());
				}
				else
				{
					return null;
				}
			}

	// Destroy the handle associated with the control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void DestroyHandle()
			{
				// Bail out if we don't have a handle.
				if(toolkitWindow == null)
				{
					return;
				}
				
				// process all pending InvokeEvents 
				// it could be that a BeginInvoke is waiting for EndInvoke.
				this.ProcessInvokeEvent( IntPtr.Zero );
				
				// Destroy all of the child controls.
				int child;
				for(child = 0; child < numChildren; ++child)
				{
					children[child].DestroyHandle();
				}

				// Destroy the toolkit window.
				if(toolkitWindow != null)
				{
					toolkitWindow.Destroy();
					toolkitWindow = null;
				}
				
				// Dispose DoubleBuffer too here
				if( buffer != null ) {
					buffer.Dispose();
					buffer = null;
				}
				
				// Notify event handlers that the handle is destroyed.
				OnHandleDestroyed(EventArgs.Empty);
			}

#if !CONFIG_COMPACT_FORMS

	// Begin a drag and drop operation on this control.
	[TODO]
	public DragDropEffects DoDragDrop
		(Object data, DragDropEffects allowedEffects)
			{
				// Fill in
				return allowedEffects;
			}

#endif // !CONFIG_COMPACT_FORMS

	private bool IsDisposedOrDisposing
	{
		get 
		{
			return GetControlFlag(ControlFlags.Disposed) || GetControlFlag(ControlFlags.Disposing);
		}
	}

	// Dispose of this control.
#if CONFIG_COMPACT_FORMS
	public new void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
#elif !CONFIG_COMPONENT_MODEL
	public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
#endif
	[TODO]
#if CONFIG_COMPONENT_MODEL
	protected override void Dispose(bool disposing)
#else
	protected virtual void Dispose(bool disposing)
#endif
			{
				if( this.IsDisposedOrDisposing ) {
					return;// do nothing if already disposing or disposed
				}
				
				try
				{
					SetControlFlag(ControlFlags.Disposing, true);
					try
					{
						if( toolkitWindow != null ) { // prevent one method call for performance
							DestroyHandle();
						}
						
						// Remove this control from Parent
						if( null != this.parent ) {
							this.parent.Controls.Remove(this);
						}
						
						// Dispose all childs
						if( null != controlCollection ) {
							try {
								Control o;
								int iCount = controlCollection.Count;
								for( int i = 0; i < iCount; i++ ) {
									o = controlCollection[i];
									o.parent = null;
									o.Dispose();
									o = null;
								}
							}
							catch( Exception ) {
								// ignore exceptions
							}
							controlCollection = null;
						}
						
						if( null != children ) {
							for(int i = 0; i < children.Length; i++ )
							{
								children[i] = null;
							}
						}
						numChildren = 0;
						children = null;

						StopHover();
					}
					finally
					{
						SetControlFlag(ControlFlags.Disposed, true);
					}
				}
				finally
				{
					SetControlFlag(ControlFlags.Disposing, false);
				}
#if CONFIG_COMPONENT_MODEL
				base.Dispose(disposing);
#endif
			}

	// Find the form that this control is a member of.
	public Form FindForm()
			{
				Control current = this;
				while(current != null && !(current is Form))
				{
					current = current.Parent;
				}
				return (Form)current;
			}

	// Set the input focus to this control.
	public bool Focus()
			{
				if (CanFocus && toolkitWindow != null)
					toolkitWindow.Focus();

				// Set the active control in the parent container.
				if (Focused && Parent != null)
				{
					ContainerControl container = Parent.GetContainerControl() as ContainerControl;
					if (container != null)
						container.ActiveControl = this;
				}
				return Focused;
			}

	// Convert a child HWND into the corresponding Control object.
	[TODO]
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	public static Control FromChildHandle(IntPtr handle)
			{
				// Fill in
				return FromHandle(handle);
			}

	// Convert a HWND into the corresponding Control object.
	[TODO]
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	public static Control FromHandle(IntPtr handle)
		{
			// Fill in
			return null;
		}

	// Get the child at a specific location within this control.
	public Control ChildAtPoint(Point pt)
			{
				int posn, x, y;
				Control child;
				x = pt.X;
				y = pt.Y;
				for(posn = 0; posn < numChildren; ++posn)
				{
					child = children[posn];
					if(!(child.visible))
					{
						continue;
					}
					if(x >= child.left && x < (child.left + child.width) &&
						y >= child.top && y < (child.top + child.height))
					{
						return child;
					}
				}
				return null;
			}

	// Get the container control for this control.
	public IContainerControl GetContainerControl()
			{
				Control current = this;
				while(current != null && !(current is IContainerControl && (current.controlStyle & (int)ControlStyles.ContainerControl) != 0))
				{
					current = current.Parent;
				}
				return (IContainerControl)current;
			}

	// Get a control flag.
	private bool GetControlFlag(ControlFlags mask)
			{
				return ((flags & mask) == mask);
			}

	// Get the next or previous control in the tab order.
	public Control GetNextControl(Control ctl, bool forward)
			{
				if (!Contains(ctl))
				{
					ctl = this;
				}

				if (forward && ctl.children != null && ctl.numChildren > 0 && (ctl == this || !(ctl is IContainerControl) || !ctl.GetStyle(ControlStyles.ContainerControl)))
				{
					// Find the first control in the children.
					Control found = ctl.children[0];
					for (int i = 1; i < ctl.numChildren; i++)
					{
						if (found.tabIndex > ctl.children[i].tabIndex)
						{
							found = ctl.children[i];
						}
					}
					return found;
				}
				
				// Search through the childs hierarchy for the next control, until we've search "this" control.
				while (ctl != this)
				{
					Control found = null;
					if (ctl.parent.numChildren > 0)
					{
						bool passedStart = false;
						if (forward)
						{
							for (int i = 0; i < ctl.parent.numChildren; i++)
							{
								Control child = ctl.parent.children[i];
								if (child == ctl)
								{
									passedStart = true;
								}
								else if (child.tabIndex >= ctl.tabIndex && (child.tabIndex != ctl.tabIndex || passedStart))
								{
									if (found == null || found.tabIndex > child.tabIndex)
									{
										found = ctl.parent.children[i];
									}
								}
							}
							if (found != null)
							{
								return found;
							}
						}
						else // backwards
						{
							// Search up through the childs hierarchy for the previous control, until we've search in this control.
							for (int i = ctl.parent.numChildren - 1; i >= 0 ; i--)
							{
								Control child = ctl.parent.children[i];
								if (child == ctl)
								{
									passedStart = true;
								}
								else if (child.tabIndex <= ctl.tabIndex && (child.tabIndex != ctl.tabIndex || passedStart))
								{
									if (found == null || found.tabIndex < child.tabIndex)
									{
										found = ctl.parent.children[i];
									}
								}
							}
							if (found == null)
							{
								if (ctl.parent == this)
								{
									return null;
								}
								return ctl.parent;
							}
							else
							{
								ctl = found;
								break;
							}
						}

					}
					ctl = ctl.parent;
				}

				if (!forward)
				{
					// Find the container because there was no control found.
		#if CONFIG_COMPONENT_MODEL
					while (ctl.numChildren > 0 && (ctl ==this || !(ctl is IContainer) || !ctl.GetStyle(ControlStyles.ContainerControl)))
		#else
					while (ctl.numChildren > 0 && (ctl ==this || !ctl.GetStyle(ControlStyles.ContainerControl)))
		#endif
					{
						Control found = ctl.children[ctl.numChildren - 1];
						for (int i = ctl.numChildren - 2; i >= 0; i--)
						{
							Control c = ctl.children[i];
							if (found.tabIndex < c.tabIndex)
							{
								found = c;
							}
						}
						ctl = found;
					}
				}
			
				if (ctl != this)
				{
					return ctl;
				}
				return null;
			}

	// Get a particular style flag.
	protected bool GetStyle(ControlStyles flag)
			{
				return (((ControlStyles)controlStyle & flag) == flag);
			}

	// Determine if this is a top-level control.
	protected bool GetTopLevel()
			{
				return IsTopLevel;
			}

	// Hide the control.
	public void Hide()
			{
				Visible = false;
			}

	// Initialize layout as this control has just been added to a container.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void InitLayout()
			{
				if(parent != null && Dock == DockStyle.None)
				{
					UpdateDistances();
				}
			}

	// Invalidate a region of the control and queue up a repaint request.
	public void Invalidate()
			{
				Invalidate(ClientRectangle, false);
			}

	public void Invalidate(bool invalidateChildren)
			{
				Invalidate(ClientRectangle, invalidateChildren);
			}

	public void Invalidate(Rectangle rc)
			{
				Invalidate(rc, false);
			}

	public void Invalidate(Rectangle rc, bool invalidateChildren)
			{
				using (Region region = new Region(rc))
				{
					Invalidate(region, invalidateChildren);
				}
			}

	public void Invalidate(Region region)
			{
				Invalidate(region, false);
			}

	public void Invalidate(Region region, bool invalidateChildren)
			{
				if (!Visible)
				{
					return; // nothing to do 
				}

				if (toolkitWindow == null)
				{
					if ((parent == null) || (!parent.IsHandleCreated))
					{
						return;
					}

					CreateControl ();
				}

				using (Region region1 = region.Clone())
				{
					InvalidateInternal(region1, invalidateChildren);
					using (Graphics g = CreateGraphics())
					{
						Rectangle bounds = Rectangle.Truncate(region.GetBounds(g));
						OnInvalidated(new InvalidateEventArgs(bounds));
					}
				}
			}

	private void InvalidateInternal(Region region, bool invalidateChildren)
			{
				if(invalidateChildren)
				{
					for(int i = (numChildren - 1); i >= 0; --i)
					{
						Control child = children[i];
						if (child.visible)
						{
							Region region1 = (Region)region.Clone();
							region1.Intersect(child.Bounds);
							region1.Translate(-child.Left, - child.Top);
							child.InvalidateInternal(region1, true);
						}
					}
				}

				// Exclude the children from the invalidate
				for(int i = (numChildren - 1); i >= 0; --i)
				{
					Control child = children[i];
					if (child.visible)
					{
						region.Exclude(children[i].Bounds);
					}
				}

				// TODO Inefficient
				RectangleF[] rs = region.GetRegionScans(new Drawing.Drawing2D.Matrix());
				if( rs.Length > 0 ) {
					// TODO Inefficient
					Point p = ClientOrigin;
					Size  s = ClientSize;
					int xOrigin = p.X;
					int yOrigin = p.Y;
				// The rectangle relative to the toolkit that is the bounds for this control.
					Rectangle parentInvalidateBounds = new Rectangle(xOrigin, yOrigin, s.Width, s.Height);
					
					for(int i = 0; i < rs.Length; i++)
					{
						Rectangle b = Rectangle.Truncate(rs[i]);
						// Get in local coordinates.
						b.Offset(xOrigin, yOrigin);
						b.Intersect(parentInvalidateBounds);
						if(!b.IsEmpty)
						{
							if(toolkitWindow == null)
							{
								CreateControl();
							}
							toolkitWindow.Invalidate(b.X, b.Y, b.Width, b.Height);
						}
					}
				}
			}

	// Invoke a delegate on the thread that owns the low-level control.
	public Object Invoke(Delegate method)
			{
				return this.Invoke(method, null);
			}

	// Invoke the "GotFocus" event on a particular control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void InvokeGotFocus(Control toInvoke, EventArgs e)
			{
				toInvoke.OnGotFocus(e);
			}

	// Invoke the "LostFocus" event on a particular control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void InvokeLostFocus(Control toInvoke, EventArgs e)
			{
				toInvoke.OnLostFocus(e);
			}

	// Invoke the "Click" event on a particular control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void InvokeOnClick(Control toInvoke, EventArgs e)
			{
				toInvoke.OnClick(e);
			}

	// Invoke the "Paint" event on a particular control.
	protected void InvokePaint(Control c, PaintEventArgs e)
			{
				c.OnPaint(e);
			}

	// Invoke the "PaintBackground" event on a particular control.
	protected void InvokePaintBackground(Control c, PaintEventArgs e)
			{
				c.OnPaintBackground(e);
			}

	// Determine if a character is recognized by a control as an input char.
	protected virtual bool IsInputChar(char c)
			{
				// By default, pass the request up to our parent.
				if(parent != null)
				{
					return parent.IsInputChar(c);
				}
				else
				{
					return true;
				}
			}

	// Determine if a key is recognized by a control as an input key.
	protected virtual bool IsInputKey(Keys keyData)
			{
				return false; 
			}

	// Determine if a character is mnemonic in a string.
	public static bool IsMnemonic(char charCode, String text)
			{
				if(charCode == '&' || text == null)
				{
					return false;
				}
				int index = text.IndexOf('&') + 1;
				if(index < text.Length)
				{
					char ch = text[index];
					return (Char.ToUpper(ch) == Char.ToUpper(charCode));
				}
				return false;
			}

	// Force the child to perform layout.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	public void PerformLayout()
			{
				PerformLayout(null, null);
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	public void PerformLayout
		(Control affectedControl, String affectedProperty)
			{
				// Bail out if layout was suspended.
				if(layoutSuspended > 0)
				{
					return;
				}

				if( !this.visible ) {	// attention, don't use Visible, do layout, if this visible even if Parent is not visible.
					return; // no need to layout, increases performance 
				}
				
				// Mark this control as currently being laid out.
				SetControlFlag(ControlFlags.PerformingLayout, true);
				++layoutSuspended;

				// Lay out this control.  We use a try block to make
				// sure that the layout control variables are reset
				// if "OnLayout" throws an exception for some reason.
				try
				{
					OnLayout(new LayoutEventArgs(affectedControl,
						affectedProperty));
				}
				finally
				{
					// We are finished laying out this control.
					--layoutSuspended;
					SetControlFlag(ControlFlags.PerformingLayout, false);
				}
			}

	// Perform anchoring layouting.
	//
	// Some comments about this: we save the distance to the left, right, top and bottom
	// edge on each call to SetBoundsCore (except when we disabled this by setting the
	// updateDistances bool to false, which we only do in this function).
	//
	// This function then computes the positions/sizes of a control using these distances.
	// That's easier and more reliable way than trying to keep track of size changes and
	// then applying deltas.
	//
	// When no Left or Right AnchorStyle is given we need to center horizontally, and when
	// no Top or Bottom AnchorStyle is given we need to center vertically. This centering
	// seems to work like this: first we assume that the control plus its distances form a
	// a rectangle. Then we center /this/ rectangle and add distLeft or distTop to get the
	// location of the control.
	private void PerformAnchorLayout (Control child, Rectangle rect)
			{
				int x, y, w, h;
				AnchorStyles anchor = child.Anchor;
				
				x = child.left;
				y = child.top;
				w = child.width;
				h = child.height;
			
				if ((anchor & AnchorStyles.Right) != 0)
				{
					if ((anchor & AnchorStyles.Left) != 0)
					{
						x = child.distLeft;
						w = rect.Width - x - child.distRight;
						if (w < 0)
						{
							w = 0;
						}
					}
					else
					{
						x = rect.Width - w - child.distRight;
					}
				}
				else if ((anchor & AnchorStyles.Left) != 0)
				{
					x = child.distLeft;
				}
				else
				{
					x = ((rect.Width - (w + child.distLeft + child.distRight)) / 2) + child.distLeft;
				}
				
				if ((anchor & AnchorStyles.Bottom) != 0)
				{
					if ((anchor & AnchorStyles.Top) != 0)
					{
						y = child.distTop;
						h = rect.Height - y - child.distBottom;
						if (h < 0)
						{
							h = 0;
						}
					}
					else
					{
						y = rect.Height - h - child.distBottom;
					}
				}
				else if ((anchor & AnchorStyles.Top) != 0)
				{
					y = child.distTop;
				}
				else
				{
					y = ((rect.Height - (h + child.distTop + child.distBottom)) / 2) + child.distTop;
				}
				
				child.updateDistances = false;		
				child.SetBounds (x, y, w, h);
				child.updateDistances = true;
			}
	

	// Perform actual layout on the control.  Called from "OnLayout".
	private void PerformActualLayout()
			{
				Rectangle rect;
				int left, right, top, bottom;
				int posn, temp;
				Control child;

				// If our height is less than the height of an empty control, then we have probably been minimized and we must not layout.
				
				Size offset = ClientToBounds(Size.Empty);
				if(height < offset.Height)
				{
					return;
				}

				// Start with the display rectangle.
				rect = DisplayRectangle;
				left = rect.Left;
				right = rect.Right;
				top = rect.Top;
				bottom = rect.Bottom;

				// Lay out the controls, from first to last
				for(posn = numChildren - 1; posn >= 0; --posn)
				{
					child = children[posn];
					if(child.visible)
					{
						switch(child.Dock)
						{
							case DockStyle.Top:
							{
								child.SetBounds
									(left, top, right - left, child.Height);
								top += child.Height;
							}
							break;
	
							case DockStyle.Bottom:
							{
								temp = child.Height;
								child.SetBounds
									(left, bottom - temp, right - left, temp);
								bottom -= child.Height;
							}
							break;
	
							case DockStyle.Left:
							{
								child.SetBounds
									(left, top, child.Width, bottom - top);
								left += child.Width;
							}
							break;
	
							case DockStyle.Right:
							{
								temp = child.Width;
								child.SetBounds
									(right - temp, top, temp, bottom - top);
								right -= child.Width;
							}
							break;
	
							case DockStyle.Fill:
							{
								child.SetBounds
									(left, top, right - left, bottom - top);
								right = left;
								bottom = top;
							}
							break;
						}
					}
				}
				
				for(posn = numChildren - 1; posn >= 0; --posn)
				{
					child = children[posn];
					if(child.visible)
					{
						if (child.Dock == DockStyle.None)
						{
							// rect is still the DisplayRectangle
							PerformAnchorLayout (child, rect);
						}
					}
				}
			} 

	// Convert a screen point into client co-ordinates.
	public Point PointToClient(Point p)
			{
				Point client = ClientOrigin;
				if(parent != null)
				{
					p = parent.PointToClient(p);
				}
				return new Point(p.X - left - client.X, p.Y - top - client.Y);
			}

	// Convert a client point into screen co-ordinates.
	public Point PointToScreen(Point p)
			{
				Point client = ClientOrigin;
				if(parent != null)
				{
					p = parent.PointToScreen(p);
				}
				return new Point(p.X + left + client.X, p.Y + top + client.Y);
			}

	// Process a command key.
	protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
			{
				if (contextMenu != null && contextMenu.ProcessCmdKey(ref msg, keyData))
				{
					return true; 
				}
				if (parent != null)
				{
					return parent.ProcessCmdKey(ref msg, keyData);
				}
				return false;
			}

	// Process a dialog character.
	protected virtual bool ProcessDialogChar(char charCode)
			{
				// By default, pass the message up to our parent.
				if(parent != null)
				{
					return parent.ProcessDialogChar(charCode);
				}
				else
				{
					return false;
				}
			}

	// Process a dialog key.
	protected virtual bool ProcessDialogKey(Keys keyData)
			{
				// By default, pass the message up to our parent.
				if(parent != null)
				{
					return parent.ProcessDialogKey(keyData);
				}
				else
				{
					return false;
				}
			}

	// Process a key event by turning it into its EventArgs form.
	protected virtual bool ProcessKeyEventArgs(ref Message msg)
			{
				int msgNum = msg.Msg;
				KeyEventArgs args1;
				KeyPressEventArgs args2;
				if(msgNum == Win32Constants.WM_KEYDOWN)
				{
					args1 = new KeyEventArgs(msg.key);
					OnKeyDown(args1);
					return args1.Handled;
				}
				else if(msgNum == Win32Constants.WM_KEYUP)
				{
					args1 = new KeyEventArgs(msg.key);
					OnKeyUp(args1);
					return args1.Handled;
				}
				else if(msgNum == Win32Constants.WM_CHAR)
				{
					args2 = new KeyPressEventArgs((char)(msg.key));
					OnKeyPress(args2);
					return args2.Handled;
				}
				else
				{
					return false;
				}
			}

	// Process a keyboard message.
	protected internal virtual bool ProcessKeyMessage(ref Message msg)
			{
				// If we have a parent, then let it preview the event first.
				if(parent != null)
				{
					if(parent.ProcessKeyPreview(ref msg))
					{
						return true;
					}
				}

				// Turn the event into its EventArgs form and dispatch it.
				return ProcessKeyEventArgs(ref msg);
			}

	// Preview a keyboard message.
	protected virtual bool ProcessKeyPreview(ref Message msg)
			{
				// By default, pass the message up to our parent.
				if(parent != null)
				{
					return parent.ProcessKeyPreview(ref msg);
				}
				else
				{
					return false;
				}
			}

	// Process a key mnemonic.
	protected virtual bool ProcessMnemonic(char charCode)
			{
				// By default, controls don't have mnemonics.
				// Overridden by subclasses that need mnemonics.
				return false;
			}

	// Used by ContainerControl to process the mnemonic.
	internal virtual bool ProcessMnemonicInternal(char charCode)
			{
				return ProcessMnemonic(charCode);
			}

	// Force the handle to be recreated.
	[TODO]
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void RecreateHandle()
			{
				// Finish
				if (toolkitWindow == null)
				{
					return;
				}
				toolkitWindow.Invalidate(0, 0, width, height);
			}

	// Convert a screen rectangle into client co-ordinates.
	public Rectangle RectangleToClient(Rectangle r)
			{
				return new Rectangle(PointToClient(r.Location), r.Size);
			}

	// Convert a client rectangle into screen co-ordinates.
	public Rectangle RectangleToScreen(Rectangle r)
			{
				return new Rectangle(PointToScreen(r.Location), r.Size);
			}

	// Reflect a message to the correct control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected static bool ReflectMessage(IntPtr hWnd, ref Message m)
			{
				// We don't use this method in this implementation.
				return false;
			}

	// Force an immediate refresh on the control.
	public virtual void Refresh()
			{
				if (toolkitWindow != null)
				{
					Invalidate(true);
					toolkitWindow.Update();
				}
			}

	// Reset the background color to its default value.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public virtual void ResetBackColor()
			{
				BackColor = Color.Empty;
			}

	// Reset the data bindings to its default value.
	[TODO]
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public void ResetBindings()
			{
				return;
			}

	// Reset the cursor to its default value.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public virtual void ResetCursor()
			{
				Cursor = null;
			}

	// Reset the foreground color to its default value.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public virtual void ResetForeColor()
			{
				ForeColor = Color.Empty;
			}

	// Reset the font to its default value.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public virtual void ResetFont()
			{
				Font = null;
			}

	// Reset the input method mode to its default value.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public void ResetImeMode()
			{
				ImeMode = DefaultImeMode;
			}

	// Reset the right to left property to its default value.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Never)]
#endif
	public virtual void ResetRightToLeft()
			{
				RightToLeft = RightToLeft.Inherit;
			}
	
	protected virtual void ResetMouseEventArgs()
			{
				// Nothing to do here ?.
			}

	// Reset the text property to its default value.
	public virtual void ResetText()
			{
				Text = String.Empty;
			}

	// Resume layout operations.
	public void ResumeLayout()
			{
				ResumeLayout(true);
			}
	public void ResumeLayout(bool performLayout)
			{
				if(layoutSuspended <= 0 || (--layoutSuspended) == 0)
				{
					if(performLayout &&
					   !GetControlFlag(ControlFlags.PerformingLayout))
					{
						PerformLayout();
					}
					if( !performLayout) {
						for( int i = (numChildren - 1); i >= 0; --i ) {
							children[i].InitLayout();
						}
					}
				}
			}

	// Translate an alignment value for right-to-left text.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected ContentAlignment RtlTranslateAlignment(ContentAlignment align)
			{
				if(RightToLeft == RightToLeft.No)
				{
					return align;
				}
				else
				{
					switch(align)
					{
						case ContentAlignment.TopLeft:
							return ContentAlignment.TopRight;
						case ContentAlignment.TopRight:
							return ContentAlignment.TopLeft;
						case ContentAlignment.MiddleLeft:
							return ContentAlignment.MiddleRight;
						case ContentAlignment.MiddleRight:
							return ContentAlignment.MiddleLeft;
						case ContentAlignment.BottomLeft:
							return ContentAlignment.BottomRight;
						case ContentAlignment.BottomRight:
							return ContentAlignment.BottomLeft;
						default:
							return align;
					}
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected HorizontalAlignment RtlTranslateAlignment
		(HorizontalAlignment align)
			{
				if(RightToLeft == RightToLeft.No)
				{
					return align;
				}
				else if(align == HorizontalAlignment.Left)
				{
					return HorizontalAlignment.Right;
				}
				else if(align == HorizontalAlignment.Right)
				{
					return HorizontalAlignment.Left;
				}
				else
				{
					return align;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected LeftRightAlignment RtlTranslateAlignment
		(LeftRightAlignment align)
			{
				if(RightToLeft == RightToLeft.No)
				{
					return align;
				}
				else if(align == LeftRightAlignment.Left)
				{
					return LeftRightAlignment.Right;
				}
				else
				{
					return LeftRightAlignment.Left;
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected ContentAlignment RtlTranslateContent(ContentAlignment align)
			{
				return RtlTranslateAlignment(align);
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected HorizontalAlignment RtlTranslateHorizontal
		(HorizontalAlignment align)
			{
				return RtlTranslateAlignment(align);
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected LeftRightAlignment RtlTranslateLeftRight
		(LeftRightAlignment align)
			{
				return RtlTranslateAlignment(align);
			}

	// Scale this control and its children.
	public void Scale(float ratio)
			{
				ScaleCore(ratio, ratio);
			}
	public void Scale(float dx, float dy)
			{
				ScaleCore(dx, dy);
			}

	// Inner core of "Scale".
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void ScaleCore(float dx, float dy)
			{
				layoutSuspended++;
				int newLeft = (int)Math.Floor(dx * left);
				int newTop = (int)Math.Floor(dy * top);
				int newWidth = width;
				if (!GetStyle(ControlStyles.FixedWidth))
					newWidth = (int) (dx *  (left + width) + 0.5) - newLeft;
				int newHeight = height;
				if (!GetStyle(ControlStyles.FixedHeight))
					newHeight = (int) (dy *  (top + height) + 0.5) - newTop;
				SetBoundsCore(newLeft, newTop, newWidth, newHeight, BoundsSpecified.All);
				// Scale the children.
				if(children != null)
				{
					for(int i = (numChildren - 1); i >= 0; --i)
					{
						children[i].Scale(dx, dy);
					}
				}
				layoutSuspended--;

			}

	// Select this control.
	public void Select()
			{
				Select(false, false);
			}
	
	protected virtual void Select(bool directed, bool forward)
			{
				IContainerControl container = this.GetContainerControl();
				if (container != null)
					container.ActiveControl = this;
			}

	// Select the next control.
	public bool SelectNextControl
		(Control ctl, bool forward, bool tabStopOnly,
		bool nested, bool wrap)
			{
				if (!Contains(ctl) || !nested && ctl.parent != this)
					ctl = null;

				Control control = ctl;
				// Look for the next control we can select.
				do
				{
					ctl = GetNextControl(ctl, forward);
					if (ctl == null)
					{
						if (!wrap)
							break;
						continue;
					}
					if (ctl.CanSelect && (ctl.TabStop || !tabStopOnly) && (nested || ctl.parent == this))
					{
						// Found a control.
						ctl.Select(true, forward);
						return true;
					}
				}
				while (ctl != control);
				// Did not find a control.
				return false;
			}

	// Send this control to the back of its sibling stack.
	public void SendToBack()
			{
				if(parent != null)
				{
					if(parent.children[parent.numChildren - 1] != this)
					{
						Control sibling;
						sibling = parent.children[parent.numChildren - 1];
						int posn = parent.numChildren - 1;
						while(parent.children[posn] != this)
						{
							--posn;
						}
						while(posn < (parent.numChildren - 1))
						{
							parent.children[posn] = parent.children[posn + 1];
							++posn;
						}
						parent.children[parent.numChildren - 1] = this;
					}
				}
				if(toolkitWindow != null)
				{
					toolkitWindow.Lower();
				}
			}

	// Set the bounds of the control.
	public void SetBounds(int x, int y, int width, int height)
			{
				SetBoundsCore(x, y, width, height, BoundsSpecified.All);
			}
	public void SetBounds(int x, int y, int width, int height,
		BoundsSpecified specified)
			{
				SetBoundsCore(x, y, width, height, specified);
			}

	// Inner core of "SetBounds".
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void SetBoundsCore
		(int x, int y, int width, int height,
		BoundsSpecified specified)
			{
				bool modified = (x != this.left || y != this.top || width != this.width || height != this.height);
				
				if( !modified ) return; // no need to do anything
				
				// Set unspecified components to the right values.
				if((specified & BoundsSpecified.X) == 0)
				{
					x = this.left;
				}
				if((specified & BoundsSpecified.Y) == 0)
				{
					y = this.top;
				}
				if((specified & BoundsSpecified.Width) == 0)
				{
					width = this.width;
				}
				if((specified & BoundsSpecified.Height) == 0)
				{
					height = this.height;
				}

				// Move and resize the toolkit version of the control.
				if(toolkitWindow != null && modified)
				{
					SetBoundsToolkit(x, y, width, height);
				}

				// Update the bounds and emit the necessary events.
				UpdateBounds(x, y, width, height);
			}

	// Adjust the actual position of the control depending on windows decorations (Draw Origin) or non client areas (client origin) like menus.
	private void SetBoundsToolkit(int x, int y, int width, int height)
			{
				// Convert from outside to toolkit coordinates
				int xT = x + ToolkitDrawOrigin.X;
				int yT = y + ToolkitDrawOrigin.Y;
				int widthT = width - ToolkitDrawSize.Width;
				int heightT = height - ToolkitDrawSize.Height;
				if (Parent != null)
				{
					xT += Parent.ClientOrigin.X - Parent.ToolkitDrawOrigin.X;
					yT += Parent.ClientOrigin.Y - Parent.ToolkitDrawOrigin.Y;
				}
				// Controls are located in the client area
				toolkitWindow.MoveResize( xT, yT, widthT, heightT);
			}

	// Inner core of setting the client size.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void SetClientSizeCore(int x, int y)
			{
				Size client = ClientToBounds(new Size(x, y));
				SetBoundsCore(left, top, client.Width, client.Height, BoundsSpecified.Size);
			}

	// Set a control flag.
	private void SetControlFlag(ControlFlags mask, bool value)
			{
				if(value)
				{
					flags |= mask;
				}
				else
				{
					flags &= ~mask;
				}
			}

	// Set a style bit.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void SetStyle(ControlStyles flag, bool value)
			{
				if(value)
				{
					controlStyle |= (int)flag;
				}
				else
				{
					controlStyle &= ~(int)flag;
				}
			}

	// Set the top-level property of this control.
	[TODO]
	protected void SetTopLevel(bool value)
			{
				return;
			}

	// Inner core of setting the visibility state.
	protected virtual void SetVisibleCore(bool value)
			{
				if(visible != value)
				{
					if (!value)
					{
						if (ContainsFocus && Parent != null)
						{
							Control container = Parent.GetContainerControl() as Control;
							if (container != null)
							{
									container.SelectNextControl(this, true, true, true, true);
							}
						}
					}
					// Update the visible state.
					visible = value;
					OnVisibleChanged(EventArgs.Empty);

					// Perform layout on the parent or self.
					if(parent != null) {
						parent.PerformLayout(this, "Visible");
						if( this.Dock != DockStyle.None ) parent.PerformActualLayout();
					}
					else
						PerformLayout(this, "Visible");
				}
			}

	// Show the control on-screen.
	public void Show()
			{
				Visible = true;
			}

	// Suspend layout for this control.
	public void SuspendLayout()
			{
				++layoutSuspended;
			}

	// Update the invalidated regions in this control.
	public void Update()
			{
				if(toolkitWindow == null || !visible)
				{
					return;
				}
				for(int i = (numChildren - 1); i >= 0; --i)
				{
					Control child = children[i];
					if(child.visible)
					{
						child.Update();
					}
				}
				toolkitWindow.Update();
			}

	// Update the "distances" from the edges for anchor layouting.
	private void UpdateDistances ()
			{
				// We temporarily disable this function from PerformAnchorLayout
				// to not unnecessarily waste some cycles.
				if (!updateDistances)
					return;
				
				if (parent != null)
				{
					distLeft = this.left;
					distRight = parent.ClientSize.Width - (this.left + this.width);
					distTop = this.top;
					distBottom = parent.ClientSize.Height - (this.top + this.height);
					
					if (distRight < 0)
						distRight = 0;
					if (distBottom < 0)
						distBottom = 0;
				}
				else
				{
					distLeft = 0;
					distRight = 0;
					distTop = 0;
					distBottom = 0;
				}
			}

	// Update the bounds of the control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void UpdateBounds()
			{
				UpdateBounds(left, top, width, height);
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void UpdateBounds(int x, int y, int width, int height)
			{
				bool moved;
				bool resized;
				moved = (x != this.left || y != this.top);
				resized = (width != this.width || height != this.height);
				
				this.left = x;
				this.top = y;
				this.width = width;
				this.height = height;

				UpdateDistances ();

				if(moved)
				{
					OnLocationChanged(EventArgs.Empty);
				}
				if(resized)
				{
					OnSizeChanged(EventArgs.Empty);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void UpdateBounds(int x, int y, int width, int height,
		int clientWidth, int clientHeight)
			{
				// Ignore the client size information: we assume that
				// the client area remains fixed relative to the bounds.
				UpdateBounds(x, y, width, height);
			}

	// Apply the changed styles to the control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void UpdateStyles()
			{
				currentParams = CreateParams;
				Invalidate(true);
			}

	// Update the Z-order of a control.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected void UpdateZOrder()
			{
				// We don't use this - the child collection updates
				// the Z-order of child controls automatically.
			}

	// Pre-process a message before it is dispatched by the event loop.
	public virtual bool PreProcessMessage(ref Message msg)
			{
				// Handle dialog and command keys.
				int msgNum = msg.Msg;
				if(msgNum == Win32Constants.WM_KEYDOWN)
				{
					if(ProcessCmdKey(ref msg, msg.key))
					{
						return true;
					}
					if(!IsInputKey(msg.key))
					{
						return ProcessDialogKey(msg.key);
					}
				}
				else if(msgNum == Win32Constants.WM_CHAR)
				{
					bool altKeyDown = ((currentModifiers & Keys.Alt) != 0);
					if(altKeyDown || !IsInputChar((char)(msg.key)))
					{
						return ProcessDialogChar((char)(msg.key));
					}
				}
				return false;
			}

#if !CONFIG_COMPACT_FORMS

	// Default window procedure for this control class.
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void DefWndProc(ref Message msg)
			{
				// Window procedures are not used in this implementation.
			}

	// Process a message.
	protected virtual void WndProc(ref Message m)
			{
				// Window procedures are not used in this implementation.
			}

#endif // !CONFIG_COMPACT_FORMS

	// Hooked event information.
	private sealed class HookedEvent
			{
				public EventId eventId;
				public Delegate handler;
				public HookedEvent next;

				public HookedEvent(EventId eventId, Delegate handler,
					HookedEvent next)
				{
					this.eventId = eventId;
					this.handler = handler;
					this.next = next;
				}

			}; // class HookedEvent

	// Add a handler for a specific event.
	internal void AddHandler(EventId eventId, Delegate handler)
			{
				lock(this)
				{
					HookedEvent current = hookedEvents;
					while(current != null)
					{
						if(current.eventId == eventId)
						{
							current.handler =
								Delegate.Combine(current.handler, handler);
							return;
						}
						current = current.next;
					}
					hookedEvents = new HookedEvent
						(eventId, handler, hookedEvents);
				}
			}

	// Remove a handler from a specific event.
	internal void RemoveHandler(EventId eventId, Delegate handler)
			{
				lock(this)
				{
					HookedEvent current = hookedEvents;
					HookedEvent prev = null;
					while(current != null)
					{
						if(current.eventId == eventId)
						{
							current.handler =
								Delegate.Remove(current.handler, handler);
							if(current.handler == null)
							{
								if(prev != null)
								{
									prev.next = current.next;
								}
								else
								{
									hookedEvents = current.next;
								}
							}
							return;
						}
						prev = current;
						current = current.next;
					}
				}
			}

	// Get the handler for a specific event.
	internal Delegate GetHandler(EventId eventId)
			{
				lock(this)
				{
					HookedEvent current = hookedEvents;
					while(current != null)
					{
						if(current.eventId == eventId)
						{
							return current.handler;
						}
						current = current.next;
					}
					return null;
				}
			}

	// Events that may be emitted by this control.
	public event EventHandler BackColorChanged
			{
				add
				{
					AddHandler(EventId.BackColorChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.BackColorChanged, value);
				}
			}
	public event EventHandler BackgroundImageChanged
			{
				add
				{
					AddHandler(EventId.BackgroundImageChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.BackgroundImageChanged, value);
				}
			}
	public event EventHandler BindingContextChanged
			{
				add
				{
					AddHandler(EventId.BindingContextChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.BindingContextChanged, value);
				}
			}
	public event EventHandler CausesValidationChanged
			{
				add
				{
					AddHandler(EventId.CausesValidationChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.CausesValidationChanged, value);
				}
			}
	public event UICuesEventHandler ChangeUICues
			{
				add
				{
					AddHandler(EventId.ChangeUICues, value);
				}
				remove
				{
					RemoveHandler(EventId.ChangeUICues, value);
				}
			}
	public event EventHandler Click
			{
				add
				{
					AddHandler(EventId.Click, value);
				}
				remove
				{
					RemoveHandler(EventId.Click, value);
				}
			}
	public event EventHandler ContextMenuChanged
			{
				add
				{
					AddHandler(EventId.ContextMenuChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.ContextMenuChanged, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event ControlEventHandler ControlAdded
			{
				add
				{
					AddHandler(EventId.ControlAdded, value);
				}
				remove
				{
					RemoveHandler(EventId.ControlAdded, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event ControlEventHandler ControlRemoved
			{
				add
				{
					AddHandler(EventId.ControlRemoved, value);
				}
				remove
				{
					RemoveHandler(EventId.ControlRemoved, value);
				}
			}
	public event EventHandler CursorChanged
			{
				add
				{
					AddHandler(EventId.CursorChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.CursorChanged, value);
				}
			}
	public event EventHandler DockChanged
			{
				add
				{
					AddHandler(EventId.DockChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.DockChanged, value);
				}
			}
	public event EventHandler DoubleClick
			{
				add
				{
					AddHandler(EventId.DoubleClick, value);
				}
				remove
				{
					RemoveHandler(EventId.DoubleClick, value);
				}
			}
#if !CONFIG_COMPACT_FORMS
	public event DragEventHandler DragDrop
			{
				add
				{
					AddHandler(EventId.DragDrop, value);
				}
				remove
				{
					RemoveHandler(EventId.DragDrop, value);
				}
			}
	public event DragEventHandler DragEnter
			{
				add
				{
					AddHandler(EventId.DragEnter, value);
				}
				remove
				{
					RemoveHandler(EventId.DragEnter, value);
				}
			}
	public event EventHandler DragLeave
			{
				add
				{
					AddHandler(EventId.DragLeave, value);
				}
				remove
				{
					RemoveHandler(EventId.DragLeave, value);
				}
			}
	public event DragEventHandler DragOver
			{
				add
				{
					AddHandler(EventId.DragOver, value);
				}
				remove
				{
					RemoveHandler(EventId.DragOver, value);
				}
			}
#endif
	public event EventHandler EnabledChanged
			{
				add
				{
					AddHandler(EventId.EnabledChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.EnabledChanged, value);
				}
			}
	public event EventHandler Enter
			{
				add
				{
					AddHandler(EventId.Enter, value);
				}
				remove
				{
					RemoveHandler(EventId.Enter, value);
				}
			}
	public event EventHandler FontChanged
			{
				add
				{
					AddHandler(EventId.FontChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.FontChanged, value);
				}
			}
	public event EventHandler ForeColorChanged
			{
				add
				{
					AddHandler(EventId.ForeColorChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.ForeColorChanged, value);
				}
			}
#if !CONFIG_COMPACT_FORMS
	public event GiveFeedbackEventHandler GiveFeedback
			{
				add
				{
					AddHandler(EventId.GiveFeedback, value);
				}
				remove
				{
					RemoveHandler(EventId.GiveFeedback, value);
				}
			}
#endif
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event EventHandler GotFocus
			{
				add
				{
					AddHandler(EventId.GotFocus, value);
				}
				remove
				{
					RemoveHandler(EventId.GotFocus, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event EventHandler HandleCreated
			{
				add
				{
					AddHandler(EventId.HandleCreated, value);
				}
				remove
				{
					RemoveHandler(EventId.HandleCreated, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event EventHandler HandleDestroyed
			{
				add
				{
					AddHandler(EventId.HandleDestroyed, value);
				}
				remove
				{
					RemoveHandler(EventId.HandleDestroyed, value);
				}
			}
	public event HelpEventHandler HelpRequested
			{
				add
				{
					AddHandler(EventId.HelpRequested, value);
				}
				remove
				{
					RemoveHandler(EventId.HelpRequested, value);
				}
			}
	public event EventHandler ImeModeChanged
			{
				add
				{
					AddHandler(EventId.ImeModeChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.ImeModeChanged, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event InvalidateEventHandler Invalidated
			{
				add
				{
					AddHandler(EventId.Invalidated, value);
				}
				remove
				{
					RemoveHandler(EventId.Invalidated, value);
				}
			}
	public event KeyEventHandler KeyDown
			{
				add
				{
					AddHandler(EventId.KeyDown, value);
				}
				remove
				{
					RemoveHandler(EventId.KeyDown, value);
				}
			}
	public event KeyPressEventHandler KeyPress
			{
				add
				{
					AddHandler(EventId.KeyPress, value);
				}
				remove
				{
					RemoveHandler(EventId.KeyPress, value);
				}
			}
	public event KeyEventHandler KeyUp
			{
				add
				{
					AddHandler(EventId.KeyUp, value);
				}
				remove
				{
					RemoveHandler(EventId.KeyUp, value);
				}
			}
	public event LayoutEventHandler Layout
			{
				add
				{
					AddHandler(EventId.Layout, value);
				}
				remove
				{
					RemoveHandler(EventId.Layout, value);
				}
			}
	public event EventHandler Leave
			{
				add
				{
					AddHandler(EventId.Leave, value);
				}
				remove
				{
					RemoveHandler(EventId.Leave, value);
				}
			}
	public event EventHandler LocationChanged
			{
				add
				{
					AddHandler(EventId.LocationChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.LocationChanged, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event EventHandler LostFocus
			{
				add
				{
					AddHandler(EventId.LostFocus, value);
				}
				remove
				{
					RemoveHandler(EventId.LostFocus, value);
				}
			}
	public event MouseEventHandler MouseDown
			{
				add
				{
					AddHandler(EventId.MouseDown, value);
				}
				remove
				{
					RemoveHandler(EventId.MouseDown, value);
				}
			}
	public event EventHandler MouseEnter
			{
				add
				{
					AddHandler(EventId.MouseEnter, value);
				}
				remove
				{
					RemoveHandler(EventId.MouseEnter, value);
				}
			}
	public event EventHandler MouseHover
			{
				add
				{
					AddHandler(EventId.MouseHover, value);
				}
				remove
				{
					RemoveHandler(EventId.MouseHover, value);
				}
			}
	public event EventHandler MouseLeave
			{
				add
				{
					AddHandler(EventId.MouseLeave, value);
				}
				remove
				{
					RemoveHandler(EventId.MouseLeave, value);
				}
			}
	public event MouseEventHandler MouseMove
			{
				add
				{
					AddHandler(EventId.MouseMove, value);
				}
				remove
				{
					RemoveHandler(EventId.MouseMove, value);
				}
			}
	public event MouseEventHandler MouseUp
			{
				add
				{
					AddHandler(EventId.MouseUp, value);
				}
				remove
				{
					RemoveHandler(EventId.MouseUp, value);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
#if CONFIG_COMPONENT_MODEL || CONFIG_EXTENDED_DIAGNOSTICS
	[Browsable(false)]
#endif
	public event MouseEventHandler MouseWheel
			{
				add
				{
					AddHandler(EventId.MouseWheel, value);
				}
				remove
				{
					RemoveHandler(EventId.MouseWheel, value);
				}
			}
	public event EventHandler Move
			{
				add
				{
					AddHandler(EventId.Move, value);
				}
				remove
				{
					RemoveHandler(EventId.Move, value);
				}
			}
	public event PaintEventHandler Paint
			{
				add
				{
					AddHandler(EventId.Paint, value);
				}
				remove
				{
					RemoveHandler(EventId.Paint, value);
				}
			}
	public event EventHandler ParentChanged
			{
				add
				{
					AddHandler(EventId.ParentChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.ParentChanged, value);
				}
			}
#if !CONFIG_COMPACT_FORMS
	public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
			{
				add
				{
					AddHandler(EventId.QueryAccessibilityHelp, value);
				}
				remove
				{
					RemoveHandler(EventId.QueryAccessibilityHelp, value);
				}
			}
	public event QueryContinueDragEventHandler QueryContinueDrag
			{
				add
				{
					AddHandler(EventId.QueryContinueDrag, value);
				}
				remove
				{
					RemoveHandler(EventId.QueryContinueDrag, value);
				}
			}
#endif
	public event EventHandler Resize
			{
				add
				{
					AddHandler(EventId.Resize, value);
				}
				remove
				{
					RemoveHandler(EventId.Resize, value);
				}
			}
	public event EventHandler RightToLeftChanged
			{
				add
				{
					AddHandler(EventId.RightToLeftChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.RightToLeftChanged, value);
				}
			}
	public event EventHandler SizeChanged
			{
				add
				{
					AddHandler(EventId.SizeChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.SizeChanged, value);
				}
			}
	public event EventHandler StyleChanged
			{
				add
				{
					AddHandler(EventId.StyleChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.StyleChanged, value);
				}
			}
	public event EventHandler SystemColorsChanged
			{
				add
				{
					AddHandler(EventId.SystemColorsChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.SystemColorsChanged, value);
				}
			}
	public event EventHandler TabIndexChanged
			{
				add
				{
					AddHandler(EventId.TabIndexChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.TabIndexChanged, value);
				}
			}
	public event EventHandler TabStopChanged
			{
				add
				{
					AddHandler(EventId.TabStopChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.TabStopChanged, value);
				}
			}
	public event EventHandler TextChanged
			{
				add
				{
					AddHandler(EventId.TextChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.TextChanged, value);
				}
			}
	public event EventHandler Validated
			{
				add
				{
					AddHandler(EventId.Validated, value);
				}
				remove
				{
					RemoveHandler(EventId.Validated, value);
				}
			}
	public event CancelEventHandler Validating
			{
				add
				{
					AddHandler(EventId.Validating, value);
				}
				remove
				{
					RemoveHandler(EventId.Validating, value);
				}
			}
	public event EventHandler VisibleChanged
			{
				add
				{
					AddHandler(EventId.VisibleChanged, value);
				}
				remove
				{
					RemoveHandler(EventId.VisibleChanged, value);
				}
			}

	// Virtual methods that deliver events in response to control behaviour.
	protected virtual void OnBackColorChanged(EventArgs e)
			{
				// Set the background color on the toolkit window.
				if(toolkitWindow != null)
				{
					toolkitWindow.SetBackground(backColor);
					// Invalidate this control to repaint it.
					Invalidate();
				}

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.BackColorChanged));
				if(handler != null)
				{
					handler(this, e);
				}

				// Pass the change notification to the children.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].OnParentBackColorChanged(e);
				}
			}
	[TODO]
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnBackgroundImageChanged(EventArgs e)
			{
				// TODO: set the background image on the toolkit window.

				// Invalidate this control to repaint it.
				Invalidate();

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.BackgroundImageChanged));
				if(handler != null)
				{
					handler(this, e);
				}

				// Pass the change notification to the children.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].OnParentBackgroundImageChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnBindingContextChanged(EventArgs e)
			{
				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.BindingContextChanged));
				if(handler != null)
				{
					handler(this, e);
				}

				// Pass the change notification to the children.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].OnParentBindingContextChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnCausesValidationChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.CausesValidationChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnChangeUICues(UICuesEventArgs e)
			{
				UICuesEventHandler handler;
				handler = (UICuesEventHandler)
					(GetHandler(EventId.ChangeUICues));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnClick(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Click));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnContextMenuChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.ContextMenuChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnControlAdded(ControlEventArgs e)
			{
				ControlEventHandler handler;
				handler = (ControlEventHandler)
					(GetHandler(EventId.ControlAdded));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnControlRemoved(ControlEventArgs e)
			{
				ControlEventHandler handler;
				handler = (ControlEventHandler)
					(GetHandler(EventId.ControlRemoved));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnCreateControl()
			{
				// Nothing to do in the base class.
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnCursorChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.CursorChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnDockChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.DockChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnDoubleClick(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.DoubleClick));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if !CONFIG_COMPACT_FORMS
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnDragDrop(DragEventArgs e)
			{
				DragEventHandler handler;
				handler = (DragEventHandler)(GetHandler(EventId.DragDrop));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnDragEnter(DragEventArgs e)
			{
				DragEventHandler handler;
				handler = (DragEventHandler)(GetHandler(EventId.DragEnter));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnDragLeave(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.DragLeave));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnDragOver(DragEventArgs e)
			{
				DragEventHandler handler;
				handler = (DragEventHandler)(GetHandler(EventId.DragOver));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#endif
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnEnabledChanged(EventArgs e)
			{
				// Repaint the control.
				if( toolkitWindow != null ) {
					Invalidate();
					Update();
				}

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.EnabledChanged));
				if(handler != null)
				{
					handler(this, e);
				}

				// Pass the change notification to the children.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].OnParentEnabledChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnEnter(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Enter));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnFontChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.FontChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnForeColorChanged(EventArgs e)
			{
				// Set the foreground color on the toolkit window.
				if(toolkitWindow != null)
				{
					toolkitWindow.SetForeground(foreColor);
					// Invalidate this control to repaint it.
					Invalidate();
				}

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.ForeColorChanged));
				if(handler != null)
				{
					handler(this, e);
				}

				// Pass the change notification to the children.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].OnParentForeColorChanged(e);
				}
			}
#if !CONFIG_COMPACT_FORMS
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnGiveFeedback(GiveFeedbackEventArgs e)
			{
				GiveFeedbackEventHandler handler;
				handler = (GiveFeedbackEventHandler)
					(GetHandler(EventId.GiveFeedback));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#endif
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnGotFocus(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.GotFocus));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnHandleCreated(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.HandleCreated));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnHandleDestroyed(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.HandleDestroyed));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnHelpRequested(HelpEventArgs e)
			{
				HelpEventHandler handler;
				handler = (HelpEventHandler)
					(GetHandler(EventId.HelpRequested));
				if(handler != null)
				{
					handler(this, e);
				}
			}
	protected virtual void OnImeModeChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.ImeModeChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnInvalidated(InvalidateEventArgs e)
			{
				InvalidateEventHandler handler;
				handler = (InvalidateEventHandler)
					(GetHandler(EventId.Invalidated));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnKeyDown(KeyEventArgs e)
			{
				KeyEventHandler handler;
				handler = (KeyEventHandler)(GetHandler(EventId.KeyDown));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnKeyPress(KeyPressEventArgs e)
			{
				KeyPressEventHandler handler;
				handler = (KeyPressEventHandler)(GetHandler(EventId.KeyPress));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnKeyUp(KeyEventArgs e)
			{
				KeyEventHandler handler;
				handler = (KeyEventHandler)(GetHandler(EventId.KeyUp));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnLayout(LayoutEventArgs e)
			{
				// Invoke the event handler.
				LayoutEventHandler handler;
				handler = (LayoutEventHandler)(GetHandler(EventId.Layout));
				if(handler != null)
				{
					handler(this, e);
				}

				// Perform layout on this control's contents.
				PerformActualLayout();
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnLeave(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Leave));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnLocationChanged(EventArgs e)
			{
				// Raise the "Move" event first.
				OnMove(e);

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.LocationChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnLostFocus(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.LostFocus));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMouseDown(MouseEventArgs e)
			{
				if( this.IsDisposedOrDisposing ) {
					return;// do nothing if already disposing or disposed
				}

				StopHover();

				MouseEventHandler handler;
				handler = (MouseEventHandler)(GetHandler(EventId.MouseDown));
				if(handler != null)
				{
					handler(this, e);
				}
			}
	internal void OnMouseDownInternal(MouseEventArgs e)
			{
				OnMouseDown(e);
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMouseEnter(EventArgs e)
			{
				if( this.IsDisposedOrDisposing ) {
					return;// do nothing if already disposing or disposed
				}

				StartHover();

				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.MouseEnter));
				if(handler != null)
				{
					handler(this, e);
				}
			}

	private void StartHover() 
	{
		hoverTimer.Stop();
		hoverControl = this;
		hoverTimer.Start();
	}

	private void StopHover() 
	{
		if( hoverControl == this ) 
		{
			hoverControl = null;
			hoverTimer.Stop();
		}
	}

#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMouseHover(EventArgs e)
			{
				StopHover();

				if( this.IsDisposedOrDisposing ) {
					return;// do nothing if already disposing or disposed
				}
				
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.MouseHover));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMouseLeave(EventArgs e)
			{
				if( this.IsDisposedOrDisposing ) {
					return;// do nothing if already disposing or disposed
				}
				StopHover();

				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.MouseLeave));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMouseMove(MouseEventArgs e)
			{
				MouseEventHandler handler;
				handler = (MouseEventHandler)(GetHandler(EventId.MouseMove));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMouseUp(MouseEventArgs e)
			{
				if( this.IsDisposedOrDisposing ) {
					return;// do nothing if already disposing or disposed
				}
				StopHover();

				MouseEventHandler handler;
				handler = (MouseEventHandler)(GetHandler(EventId.MouseUp));
				if(handler != null)
				{
					handler(this, e);
				}
			}
	internal void OnMouseUpInternal(MouseEventArgs e)
	{
		OnMouseUp(e);
	}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMouseWheel(MouseEventArgs e)
			{
				if( this.IsDisposedOrDisposing ) {
					return;// do nothing if already disposing or disposed
				}
				StopHover();

				MouseEventHandler handler;
				handler = (MouseEventHandler)(GetHandler(EventId.MouseWheel));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnMove(EventArgs e)
			{
				// Raise the "Move" event.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Move));
				if(handler != null)
				{
					handler(this, e);
				}

				// If the window is transparent, then invalidate.
				if(GetStyle( ControlStyles.SupportsTransparentBackColor))
				{
					Invalidate();
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnPaint(PaintEventArgs e)
			{
				PaintEventHandler handler;
				handler = (PaintEventHandler)(GetHandler(EventId.Paint));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnPaintBackground(PaintEventArgs e)
			{
				using(Brush b = CreateBackgroundBrush())
				{
					e.Graphics.FillRectangle(b, DisplayRectangle);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentBackColorChanged(EventArgs e)
			{
				if((backColor.A < 255) && (backgroundImage == null))
				{
					OnBackColorChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentBackgroundImageChanged(EventArgs e)
			{
				if((backColor.A < 255) && (backgroundImage == null))
				{
					OnBackgroundImageChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentBindingContextChanged(EventArgs e)
			{
				if(bindingContext == null)
				{
					OnBindingContextChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.ParentChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentEnabledChanged(EventArgs e)
			{
				if( GetControlFlag(ControlFlags.Enabled) ) {
					OnEnabledChanged(e);
				}
				/*
				the above code does the same
				bool parentEnabled = parent.Enabled;
				bool enabled = GetControlFlag(ControlFlags.Enabled);
				if((!parentEnabled && enabled) != (parentEnabled && enabled))
				{	
					OnEnabledChanged(e);
				}
				*/
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentFontChanged(EventArgs e)
			{
				if(font == null)
				{
					OnFontChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentForeColorChanged(EventArgs e)
			{
				if(foreColor.IsEmpty)
				{
					OnForeColorChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentRightToLeftChanged(EventArgs e)
			{
				if(rightToLeft == (byte)(RightToLeft.Inherit))
				{
					OnRightToLeftChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnParentVisibleChanged(EventArgs e)
			{
				if( visible ) OnVisibleChanged(e);
			}
	internal virtual void OnPrimaryEnter(EventArgs e)
			{
				// Nothing to do here: overridden in "Form".
			}
	internal virtual void OnPrimaryLeave(EventArgs e)
			{
				// Nothing to do here: overridden in "Form".
			}
#if !CONFIG_COMPACT_FORMS
	protected virtual void OnQueryAccessibilityHelp
		(QueryAccessibilityHelpEventArgs e)
			{
				QueryAccessibilityHelpEventHandler handler;
				handler = (QueryAccessibilityHelpEventHandler)
					(GetHandler(EventId.QueryAccessibilityHelp));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnQueryContinueDrag
		(QueryContinueDragEventArgs e)
			{
				QueryContinueDragEventHandler handler;
				handler = (QueryContinueDragEventHandler)
					(GetHandler(EventId.QueryContinueDrag));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#endif
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnResize(EventArgs e)
			{
				// Force a repaint if "ResizeRedraw" is set.
				if(GetStyle(ControlStyles.ResizeRedraw))
				{
					Invalidate();
				}

				// Perform layout on this control.
				PerformLayout(this, "Bounds");

				// Notify parent to perform layout because child has changed size.
				if (Parent != null)
				{
					Parent.PerformLayout();
				}

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Resize));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnRightToLeftChanged(EventArgs e)
			{
				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.RightToLeftChanged));
				if(handler != null)
				{
					handler(this, e);
				}

				// Pass the change notification to the children.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].OnParentRightToLeftChanged(e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnSizeChanged(EventArgs e)
			{
				if(toolkitWindow != null)
					toolkitWindow.IsMapped = visible && width > 0 && height > 0;

				// Raise the "Resize" event first.
				OnResize(e);

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.SizeChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnStyleChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.StyleChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnSystemColorsChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.SystemColorsChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnTabIndexChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.TabIndexChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnTabStopChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)
					(GetHandler(EventId.TabStopChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnTextChanged(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.TextChanged));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnValidated(EventArgs e)
			{
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.Validated));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnValidating(CancelEventArgs e)
			{
				CancelEventHandler handler;
				handler = (CancelEventHandler)(GetHandler(EventId.Validating));
				if(handler != null)
				{
					handler(this, e);
				}
			}
#if CONFIG_COMPONENT_MODEL
	[EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
	protected virtual void OnVisibleChanged(EventArgs e)
			{
				// Map or unmap the toolkit window.
				if(toolkitWindow != null)
				{
					toolkitWindow.IsMapped = visible && height > 0 && width > 0;

					// May as well release the double buffer resource if its being used.
					if(!visible && buffer != null)
					{
						buffer.Dispose();
						buffer = null;
					}
				}
				else if(visible && !GetControlFlag(ControlFlags.Disposed) &&
				        (parent == null || parent.IsHandleCreated))
				{
					// Create the toolkit window for the first time.
					// This will also map the toolkit window to the screen.
					CreateControl();

					// Force PerformLayout to be called on all the children in the heirarchy.
					ForceLayout();
				}

				// Invoke the event handler.
				EventHandler handler;
				handler = (EventHandler)(GetHandler(EventId.VisibleChanged));
				if(handler != null)
				{
					handler(this, e);
				}

				// Pass the change notification to the children.
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].OnParentVisibleChanged(e);
				}
			}

	// Make sure that the control lays itself out and all its children.
	private void ForceLayout()
			{
				PerformLayout(this,null);
				for(int posn = (numChildren - 1); posn >= 0; --posn)
				{
					children[posn].ForceLayout();
				}
			}

	// Move a child to below another.  Does not update "children".
	private static void MoveToBelow(Control after, Control child)
			{
				if(after.toolkitWindow != null && child.toolkitWindow != null)
				{
					child.toolkitWindow.MoveToBelow(after.toolkitWindow);
				}
			}

	// Move a child to above another.  Does not update "children".
	private static void MoveToAbove(Control before, Control child)
			{
				if(before.toolkitWindow != null && child.toolkitWindow != null)
				{
					child.toolkitWindow.MoveToAbove(before.toolkitWindow);
				}
			}

	// Reparent this control within the windowing system.
	// If "newParent" is null, then remove it from all parents.
	// The control will end up at the bottom of the sibling stack.
	private void Reparent(Control newParent)
			{
				if(toolkitWindow == null)
				{
					return;
				}

				if(newParent == null)
				{
					/* 
					 * Don't reparent the window to null, because the window would be 
					 * reparent to a placeholder window.
					 * Then a reference would be still kept, and the control never gets disposed.
					 * So Destroy the handle. If the control is reused, the handle gets 
					 * created again.
					 *
					 * we can do this again, since we can create handles thread  safe with ControlToolkitManager.
					 */
					DestroyHandle();
				}
				else if(newParent.toolkitWindow != null)
				{
					int xOffset = parent.ClientOrigin.X - parent.ToolkitDrawOrigin.X
						+ ToolkitDrawOrigin.X;
					int yOffset = parent.ClientOrigin.Y - parent.ToolkitDrawOrigin.Y
						+ ToolkitDrawOrigin.Y;
					toolkitWindow.Reparent
						(newParent.toolkitWindow, left + xOffset,
						top + yOffset);
					toolkitWindow.Lower();
				}
				else
				{
					SetControlFlag(ControlFlags.NeedReparent, true);
				}
			}

	// Collection of child controls.
	public class ControlCollection
		: IList
#if !CONFIG_COMPACT_FORMS
		, ICloneable
#endif
	{
		// Internal state.
		private Control owner;
		bool bDisposeRemove = false;

		// Constructor.
		public ControlCollection(Control owner)
				{
					this.owner = owner;
				}
				

		// Get the control at a specific index.
		public virtual Control this[int index]
				{
					get
					{
						if(index < 0 || index >= owner.numChildren)
						{
							throw new ArgumentOutOfRangeException
								("index", S._("SWF_InvalidControlIndex"));
						}
						return owner.children[index];
					}
				}

		// Implement the ICollection interface.
		public void CopyTo(Array array, int index)
				{
					if(owner.numChildren > 0)
					{
						Array.Copy(owner.children, 0, array, index,
							owner.numChildren);
					}
				}
		public virtual int Count
				{
					get
					{
						return owner.numChildren;
					}
				}
		bool ICollection.IsSynchronized
				{
					get
					{
						return false;
					}
				}
		Object ICollection.SyncRoot
				{
					get
					{
						return this;
					}
				}

		// Implement the IList interface.
		int IList.Add(Object value)
				{
					if(value is Control)
					{
						int count = Count;
						Add((Control)value);
						return count;
					}
					else
					{
						throw new ArgumentException(S._("SWF_NotAControl"));
					}
				}
		public virtual void Clear()
				{
					owner.SuspendLayout();
					try
					{
						int count = Count;
						while(count > 0)
						{
							--count;
							Remove(this[count]);
						}
					}
					finally
					{
						owner.ResumeLayout();
					}
				}
				
		bool IList.Contains(Object value)
				{
					if(value is Control)
					{
						return Contains((Control)value);
					}
					else
					{
						return false;
					}
				}
		int IList.IndexOf(Object value)
				{
					if(value is Control)
					{
						return IndexOf((Control)value);
					}
					else
					{
						return -1;
					}
				}
		void IList.Insert(int index, Object value)
				{
					throw new NotSupportedException();
				}
		void IList.Remove(Object value)
				{
					if(value is Control)
					{
						Remove((Control)value);
					}
				}
		public virtual void RemoveAt(int index)
				{
					Remove(this[index]);
				}
		bool IList.IsFixedSize
				{
					get
					{
						return false;
					}
				}
		public bool IsReadOnly
				{ 
					get
					{
						return false;
					}
				}
		Object IList.this[int index]
				{
					get
					{
						return this[index];
					}
					set
					{
						throw new NotSupportedException();
					}
				}

		// Implement the IEnumerable interface.
		public virtual IEnumerator GetEnumerator()
				{
					return new ControlCollectionEnumerator(owner);
				}

		// Add a control to this collection.
		public virtual void Add(Control value)
				{
					if(value != null)
					{	
						if(value.Parent == owner)
						{
							// We are already under this owner, so merely
							// send it to the back of its sibling stack.
							value.SendToBack();
						}
						else
						{
							// Suspend layout on the parent while we do this.
							owner.SuspendLayout();
							try
							{
								// Change the parent to the new owner.
								value.Parent = owner;

								// Assign the next tab order if the control doesnt have one.
								if (value.tabIndex == -1)
								{
									int lastIndex = 0;
									for (int i = 0; i < owner.numChildren; i++)
									{
										int index = owner.children[i].TabIndex;
										if (lastIndex <= index)
											lastIndex = index + 1;
									}
									value.tabIndex = lastIndex;
								}
							}
							finally
							{
								// Resume layout, but don't perform it yet.
								owner.ResumeLayout(false);
							}

							// Now perform layout on the control if necessary.
							if (owner.IsHandleCreated && value.Visible)
							{
								// Make sure the control exists.
								value.CreateControl();
								owner.PerformLayout(value, "Parent");
							}
						}
					}
				}

		// Determine whether a specific control is in this collection
		public bool Contains(Control control)
				{
					return (control != null && control.Parent == owner);
				}

		// Get the index of a specific child in this collection.
		public int GetChildIndex(Control child)
				{
					int index = IndexOf(child);
					if(index == -1)
					{
						throw new ArgumentException
							(S._("SWF_ControlNotAChild"));
					}
					return index;
				}

		// Get the index of a specific child in the collection.
		public int IndexOf(Control control)
				{
					int index;
					for(index = 0; index < owner.numChildren; ++index)
					{
						if(owner.children[index] == control)
						{
							return index;
						}
					}
					return -1;
				}

		// Remove a specific control from the collection.
		public virtual void Remove(Control value)
				{
					if( this.bDisposeRemove ) return;
					
					if(value != null && value.Parent == owner)
					{
						// Update the parent.
						value.Parent = null;

						// Perform layout on the owner.
						owner.PerformLayout(value, "Parent");

						// Notify the owner that the control has been removed.
						owner.OnControlRemoved(new ControlEventArgs(value));
						
						/* 
						 * Get the ContainerControl and notify that the control was removed.
						 * the ContainerControl should activate the next control
						*/
						ContainerControl container = this.owner.GetContainerControl() as ContainerControl;
						if( null != container ) {
							container.AfterControlRemoved(value);
						}
					}
				}

		// Change the index of a child in the collection.
		public void SetChildIndex(Control child, int newIndex)
				{
					// Validate the parameters.
					if(child == null)
					{
						throw new ArgumentNullException("child");
					}
					else if(child.Parent != owner)
					{
						throw new ArgumentException
							(S._("SWF_ControlNotAChild"));
					}
					if(newIndex < 0)
					{
						newIndex = 0;
					}
					else if(newIndex >= owner.numChildren)
					{
						newIndex = owner.numChildren - 1;
					}

					// Find the previous index of the control.
					int index = IndexOf(child);

					// Move the control.
					int posn;
					if(index < newIndex)
					{
						MoveToBelow(owner.children[newIndex], child);
						for(posn = index; posn < newIndex; ++posn)
						{
							owner.children[posn] = owner.children[posn + 1];
						}
						owner.children[newIndex] = child;
					}
					else if(index > newIndex)
					{
						MoveToAbove(owner.children[newIndex], child);
						for(posn = index; posn > newIndex; --posn)
						{
							owner.children[posn] = owner.children[posn - 1];
						}
						owner.children[newIndex] = child;
					}
				}

#if !CONFIG_COMPACT_FORMS

		// Implement the ICloneable interface.
		Object ICloneable.Clone()
				{
					return this;
				}

		// Add a range of controls to this collection.
		public virtual void AddRange(Control[] controls)
				{
					if(controls == null)
					{
						throw new ArgumentNullException("controls");
					}
					owner.SuspendLayout();
					try
					{
						for(int posn = 0; posn < controls.Length; posn++)
						{
							Add(controls[posn]);
						}
					}
					finally
					{
						owner.ResumeLayout();
					}
				}

		// Determine whether two control collections are equal.
		public override bool Equals(Object obj)
				{
					if(obj is ControlCollection)
					{
						return (((ControlCollection)obj).owner == owner);
					}
					else
					{
						return false;
					}
				}

		// Get the hash code for this collection.
		public override int GetHashCode()
				{
					return owner.GetHashCode();
				}

		// Get the index of a specific child in this collection.
		public int GetChildIndex(Control child, bool throwException)
				{
					int index = IndexOf(child);
					if(index == -1 && throwException)
					{
						throw new ArgumentException
							(S._("SWF_ControlNotAChild"));
					}
					return index;
				}

#endif // !CONFIG_COMPACT_FORMS

	}; // class ControlCollection

	// Enumerator class for control collections.
	private sealed class ControlCollectionEnumerator : IEnumerator
	{
		// Internal state.
		private Control owner;
		private int index;

		// Constructor.
		public ControlCollectionEnumerator(Control owner)
				{
					this.owner = owner;
					this.index = -1;
				}

		// Implement the IEnumerator interface.
		public bool MoveNext()
				{
					++index;
					return (index < owner.numChildren);
				}
		public void Reset()
				{
					index = -1;
				}
		public Object Current
				{
					get
					{
						if(index < 0 || index >= owner.numChildren)
						{
							throw new InvalidOperationException
								(S._("SWF_InvalidControlIndex"));
						}
						return owner.children[index];
					}
				}

	}; // class ControlCollectionEnumerator

	// Toolkit event that is emitted for an expose on this window.
	void IToolkitEventSink.ToolkitExpose(Graphics graphics)
			{
				// Must we double buffer?
				bool doubleBuffer = GetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint);

				// The paint only allows drawing in the client area.
				Rectangle clientRectangle = new Rectangle(ClientOrigin - new Size(ToolkitDrawOrigin), ClientSize);
				Rectangle clipBounds = Rectangle.Truncate(graphics.ClipBounds);
				clipBounds.Offset(clientRectangle.Location);

				// Create or destroy the buffer as needed.
				if (doubleBuffer)
				{
					if (buffer == null)
					{
						buffer = ToolkitManager.Toolkit.CreateWindowBuffer(toolkitWindow);
					}
				}
				else if (buffer != null)
				{
					buffer.Dispose();
					buffer = null;
				}

				// If we are double buffering, we need to create a Graphics of a bitmap, do all the drawing on that and then write that to the screen in one go.
				if (doubleBuffer && buffer != null)
				{
					Graphics gFull = null;
					Graphics g = null;
					IToolkitGraphics toolkitGraphics = buffer.BeginDoubleBuffer();
					try
					{
						if (borderStyle != BorderStyle.None)
						{
							gFull = ToolkitManager.CreateGraphics(toolkitGraphics);
							DrawBorders(gFull);
						}
					
						g = ToolkitManager.CreateGraphics(toolkitGraphics, clientRectangle);

						PaintEventArgs e = new PaintEventArgs(g, clipBounds);
						if (GetStyle(ControlStyles.AllPaintingInWmPaint))
						{
							GraphicsState state = graphics.Save();
							OnPaintBackground(e);
							g.Restore(state);
						}
						OnPaint(e);
						buffer.EndDoubleBuffer();
					}
					finally
					{
						if (gFull != null)
						{
							gFull.Dispose();
						}
						if (g != null)
						{
							g.Dispose();
						}
					}

				}
				else //!doubleBuffer
				{
					if (!GetStyle(ControlStyles.Opaque))
					{
						// We must erase the background.
						using (Brush brush = CreateBackgroundBrush())
						{
							graphics.FillRectangle(brush, 0, 0, width, height);
						}
					}

					DrawBorders(graphics);
					// Create the graphics of the client area.
					using (Graphics g = ToolkitManager.CreateGraphics(graphics, clientRectangle))
					{
				
						PaintEventArgs e = new PaintEventArgs(g, clipBounds);
						if (GetStyle(ControlStyles.AllPaintingInWmPaint))
						{
							GraphicsState state = g.Save();
							OnPaintBackground(e);
							g.Restore(state);
						}
						OnPaint(e);
					}
				}
			}

	private void DrawBorders(Graphics graphics)
			{
				// Draw border if necessary
				switch (borderStyle)
				{
					case(BorderStyle.Fixed3D):
						ControlPaint.DrawBorder3D( graphics, new Rectangle(0,0,width, height), Border3DStyle.Sunken);
						break;
					case (BorderStyle.FixedSingle):
						using (Pen p = new Pen(ForeColor))
						{
							graphics.DrawRectangle(p, 0, 0, width - 1, height - 1);
						}
						break;
				}
			}

	// Toolkit event that is emitted when a "BeginInvoke" generated message is
	// send to this window
	// i_gch is a pointer to an InvokeParms class defined elsewhere in here
	void IToolkitEventSink.ToolkitBeginInvoke(IntPtr i_gch)
			{
				ProcessInvokeEvent(i_gch);
			}

	// Toolkit event that is emitted when the mouse enters this window.
	void IToolkitEventSink.ToolkitMouseEnter()
			{
				if(Enabled) // Check Parent Enabled too, not just this Control. GetControlFlag(ControlFlags.Enabled))
					OnMouseEnter(EventArgs.Empty);
			}

	// Toolkit event that is emitted when the mouse leaves this window.
	void IToolkitEventSink.ToolkitMouseLeave()
			{
				SetControlFlag(ControlFlags.NotifyDoubleClick, false);
				SetControlFlag(ControlFlags.NotifyClick, false);
				if(Enabled) // Check Parent Enabled too, not just this Control. GetControlFlag(ControlFlags.Enabled))
					OnMouseLeave(EventArgs.Empty);
			}

	// Toolkit event that is emitted when the focus enters this window.
	void IToolkitEventSink.ToolkitFocusEnter()
			{
				OnGotFocus(EventArgs.Empty);
			}

	// Toolkit event that is emitted when the focus leaves this window.
	void IToolkitEventSink.ToolkitFocusLeave()
			{
				OnLostFocus(EventArgs.Empty);
			}

	// Event that is emitted when the primary focus enters this window.
	// This is only called on top-level windows.
	void IToolkitEventSink.ToolkitPrimaryFocusEnter()
			{
				OnPrimaryEnter(EventArgs.Empty);
			}

	// Event that is emitted when the primary focus leaves this window.
	// This is only called on top-level windows.
	void IToolkitEventSink.ToolkitPrimaryFocusLeave()
			{
				OnPrimaryLeave(EventArgs.Empty);
			}

	// Toolkit event that is emitted for a key down event.
	bool IToolkitEventSink.ToolkitKeyDown(ToolkitKeys key)
			{
				// Create a fake key message and dispatch it.
				currentModifiers = (Keys)key;
				Message m = Message.CreateKeyMessage
					(Win32Constants.WM_KEYDOWN, (Keys)key);
				if(PreProcessMessage(ref m))
				{
					// The key was dispatched as a dialog or command key.
					return true;
				}
				return ProcessKeyMessage(ref m);
			}

	// Toolkit event that is emitted for a key up event.
	bool IToolkitEventSink.ToolkitKeyUp(ToolkitKeys key)
			{
				// Create a fake key message and dispatch it.
				currentModifiers = (Keys)key;
				Message m = Message.CreateKeyMessage
					(Win32Constants.WM_KEYUP, (Keys)key);
				if(PreProcessMessage(ref m))
				{
					// The key was dispatched as a dialog or command key.
					return true;
				}
				return ProcessKeyMessage(ref m);
			}

	// Toolkit event that is emitted for a key character event.
	bool IToolkitEventSink.ToolkitKeyChar(char charCode)
			{			
				// Create a fake key character message and dispatch it.
				Message m = Message.CreateKeyMessage
					(Win32Constants.WM_CHAR, (Keys)(int)charCode);
				if(PreProcessMessage(ref m))
				{
					// The key was dispatched as a dialog or command key.
					return true;
				}
				return ProcessKeyMessage(ref m);
			}

	// Toolkit event that is emitted for a mouse down event.
	void IToolkitEventSink.ToolkitMouseDown
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		int clicks, int x, int y, int delta)
			{
				if( !Enabled) //Check if this !Enabled OR Parent !Enabled, not just this control.  //if(!GetControlFlag(ControlFlags.Enabled))
				{
					if (parent != null)
						((IToolkitEventSink)parent).ToolkitMouseDown(buttons, modifiers, clicks, x + left, y + top, delta);
					return;
				}
				
				// Convert to client coordinates
				x += ToolkitDrawOrigin.X - ClientOrigin.X;
				y += ToolkitDrawOrigin.Y - ClientOrigin.Y;
				mouseButtons = (MouseButtons)buttons;
									
				currentModifiers = (Keys)modifiers;

				ToolkitMouseDown(mouseButtons, currentModifiers, clicks, x, y, delta);
			}

	internal protected virtual void ToolkitMouseDown
		(MouseButtons buttons, Keys modifiers,
		int clicks, int x, int y, int delta)
			{
				// Convert to client coordinates
				if( x >= 0 ) x += ToolkitDrawOrigin.X - ClientOrigin.X;
				if( y >= 0 ) y += ToolkitDrawOrigin.Y - ClientOrigin.Y;
				mousePosition = PointToScreen(new Point(x, y));
				
				if(GetStyle(ControlStyles.Selectable) && buttons == MouseButtons.Left)
				{
					Focus();
				}

				// Walk up the hierarchy and see if we must focus the control
				//if(Enabled) checked above
				{
					OnMouseDown(new MouseEventArgs
						(buttons, clicks, x, y, delta));
				}

				// We fire the OnDoubleClick and OnClick events when the mouse button is up/
				if(GetStyle(ControlStyles.StandardClick))
				{
					if(clicks == 2 && GetStyle(ControlStyles.StandardDoubleClick))
					{
						SetControlFlag(ControlFlags.NotifyDoubleClick, true);
					}
					else
					{
						SetControlFlag(ControlFlags.NotifyClick, true);
					}
				}
			}

	// Toolkit event that is emitted for a mouse up event.
	void IToolkitEventSink.ToolkitMouseUp
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		int clicks, int x, int y, int delta)
			{
				if(!Enabled) // Check Parent Enabled too, not just this Control. !GetControlFlag(ControlFlags.Enabled))
				{
					if (parent != null)
						((IToolkitEventSink)parent).ToolkitMouseUp(buttons, modifiers, clicks, x + left, y + top, delta);
					return;
				}
				
				// Convert to client coordinates
				x += ToolkitDrawOrigin.X - ClientOrigin.X;
				y += ToolkitDrawOrigin.Y - ClientOrigin.Y;
				mouseButtons = (MouseButtons)buttons;
				currentModifiers = (Keys)modifiers;
				if(GetControlFlag(ControlFlags.NotifyDoubleClick))
				{
					OnDoubleClick(EventArgs.Empty);
					SetControlFlag(ControlFlags.NotifyDoubleClick, false);
				}
				else if(GetControlFlag(ControlFlags.NotifyClick))
				{
					OnClick(EventArgs.Empty);
					SetControlFlag(ControlFlags.NotifyClick, false);
				}
				OnMouseUp(new MouseEventArgs
					((MouseButtons)buttons, clicks, x, y, delta));
				// See if we need to display the context menu.
				if(mouseButtons == MouseButtons.Right && contextMenu != null)
				{
					contextMenu.Show(this, new Point(x, y));
				}
			}

	// Toolkit event that is emitted for a mouse hover event.
	void IToolkitEventSink.ToolkitMouseHover
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		int clicks, int x, int y, int delta)
			{
				if(!Enabled) // Check Parent Enabled too, not just this Control. !GetControlFlag(ControlFlags.Enabled))
				{
					if (parent != null)
						((IToolkitEventSink)parent).ToolkitMouseHover(buttons, modifiers, clicks, x + left, y + top, delta);
					return;
				}
				
				// Convert to client coordinates
				x += ToolkitDrawOrigin.X - ClientOrigin.X;
				y += ToolkitDrawOrigin.Y - ClientOrigin.Y;
					
				currentModifiers = (Keys)modifiers;
				OnMouseHover(new MouseEventArgs
					((MouseButtons)buttons, clicks, x, y, delta));
			}

	// Toolkit event that is emitted for a mouse move event.
	void IToolkitEventSink.ToolkitMouseMove
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		int clicks, int x, int y, int delta)
			{
				if(!Enabled) //Check Parent Enabled too, not just this Control. !GetControlFlag(ControlFlags.Enabled))
				{
					if (parent != null)
						((IToolkitEventSink)parent).ToolkitMouseMove(buttons, modifiers, clicks, x + left, y + top, delta);
					return;
				}
				
				// Convert to client coordinates
				x += ToolkitDrawOrigin.X - ClientOrigin.X;
				y += ToolkitDrawOrigin.Y - ClientOrigin.Y;
				mousePosition = PointToScreen(new Point(x, y));
				
				currentModifiers = (Keys)modifiers;
				OnMouseMove(new MouseEventArgs
					((MouseButtons)buttons, clicks, x, y, delta));
			}

	// Toolkit event that is emitted for a mouse wheel event.
	void IToolkitEventSink.ToolkitMouseWheel
		(ToolkitMouseButtons buttons, ToolkitKeys modifiers,
		int clicks, int x, int y, int delta)
			{
				if( !Enabled) { // Check Parent Enabled too, not just this Control.
					if (parent != null)
						((IToolkitEventSink)parent).ToolkitMouseWheel(buttons, modifiers, clicks, x + left, y + top, delta);
					return;
				}
				// Convert to client coordinates
				x += ToolkitDrawOrigin.X - ClientOrigin.X;
				y += ToolkitDrawOrigin.Y - ClientOrigin.Y;
				currentModifiers = (Keys)modifiers;
				DoOnMouseWheel(new MouseEventArgs
					((MouseButtons)buttons, clicks, x, y, delta));
			}

	// Helper to emulate the behaviour of mouse wheel events on MS .NET:
	// The event is first given to the outer form and then up to the
	// control with the input focus. Since we send the ToolkitMouseWheel
	// call to the control with the input focus we must first traverse
	// the control hierarchy up to the outer form, which is what this
	// helper does.
	void DoOnMouseWheel (MouseEventArgs e)
			{
				if (parent != null)
					parent.DoOnMouseWheel (e);

				OnMouseWheel (e);
			}

	// Toolkit event that is emitted when the window is moved by
	// external means (e.g. the user dragging the window).
	void IToolkitEventSink.ToolkitExternalMove(int x, int y)
			{
				// Convert to outside top left
				x -= ToolkitDrawOrigin.X;
				y -= ToolkitDrawOrigin.Y;
				if (parent != null)
				{
					x -= parent.ClientOrigin.X - parent.ToolkitDrawOrigin.X;
					y -= parent.ClientOrigin.Y - parent.ToolkitDrawOrigin.Y;
				}
				if(x != left || y != top)
				{
					Location = new Point(x , y);
				}
			}

	// Toolkit event that is emitted when the window is resized by
	// external means (e.g. the user resizing the window).
	void IToolkitEventSink.ToolkitExternalResize(int width, int height)
			{
				// Convert to outside width
				int w = width + ToolkitDrawSize.Width;
				int h = height + ToolkitDrawSize.Height;
				if(w != this.width || h != this.height)
				{
					Size = new Size(w, h);
				}
			}

	// Event that is emitted when the close button on a window
	// is selected by the user.
	void IToolkitEventSink.ToolkitClose()
			{
				CloseRequest();
			}

	// Event that is emitted when the help button on a window
	// is selected by the user.
	void IToolkitEventSink.ToolkitHelp()
			{
				OnHelpRequested(new HelpEventArgs(new Point(0, 0)));
			}

	// Event that is emitted when the window state changes.
	// The argument is the "int" version of a "FormWindowState" value.
	void IToolkitEventSink.ToolkitStateChanged(int state)
			{
				WindowStateChanged((FormWindowState)state);
			}

	// Event that is emitted when the active MDI child window changes.
	// The "child" parameter is null if a window has been deactivated.
	void IToolkitEventSink.ToolkitMdiActivate(IToolkitWindow child)
			{
				MdiActivate(child);
			}

	// Messages that are processed by the "Form" class.
	internal virtual void CloseRequest() {}
	internal virtual void WindowStateChanged(FormWindowState state) {}
	internal virtual void MdiActivate(IToolkitWindow child) {}

	// Create a brush that can be used to fill with the background color/image.
	internal Brush CreateBackgroundBrush()
			{
				if(backgroundImage != null)
				{
					return new TextureBrush(backgroundImage);
				}
				return new SolidBrush(BackColor);
			}

	// Override this function if the clientRectangle is smaller than the full control.
	internal virtual Size ClientToBounds(Size size)
			{
				switch (borderStyle)
				{
					case (BorderStyle.Fixed3D):
						return new Size(size.Width + 2 * 2, size.Height + 2 * 2);
					case (BorderStyle.FixedSingle):
						return new Size(size.Width + 2, size.Height + 2);
					default: //BorderStyle.None
						return size; // new Size(size.Width, size.Height);
				}
			}

	// This is the Client Origin relative to the top left outside of the window.
	// The Client area is the area in a control excluding main menus
	// borders or window titlebars.
	// Override this if the ClientRectangle is different to the bounds of the control.
	public virtual Point ClientOrigin
			{
				get
				{
					switch (borderStyle)
					{
						case (BorderStyle.Fixed3D):
							return new Point(2, 2);
						case (BorderStyle.FixedSingle):
							return new Point(1, 1);
						default: //BorderStyle.None
							return Point.Empty;
					}
				}
			}

	// This is the offset from the top left of the actual window to
	// where the toolkit allows us to start drawing. In between are windows
	// decorations like titlebars but not menus
	// This is overridden in form
	protected virtual Point ToolkitDrawOrigin
			{
				get
				{
					return Point.Empty;
				}
			}

	// This is how much the toolkit increases a Windows size because of decorations
	// This is overridden in Form.
	protected virtual Size ToolkitDrawSize
			{
				get
				{
					return Size.Empty;
				}
			}

	protected internal void DoEnter()
			{
				OnEnter(EventArgs.Empty);
			}

	protected internal void DoLeave()
	{
		OnLeave(EventArgs.Empty);
	}

	protected internal bool DoValidating()
	{
		CancelEventArgs args = new CancelEventArgs();
		OnValidating(args);
		return args.Cancel;
	}

	protected internal void DoValidated()
	{
		OnValidated(EventArgs.Empty);
	}

	internal virtual void DoValidationCancel(bool cancelled)
	{
			SetControlFlag(ControlFlags.ValidationCancelled, cancelled);
	}


#if !CONFIG_COMPACT_FORMS

	// Methods that support "AccessibleObject".

	// Get the accessibility help information.
	internal String GetAccessibilityHelp()
			{
				QueryAccessibilityHelpEventArgs e;
				e = new QueryAccessibilityHelpEventArgs();
				OnQueryAccessibilityHelp(e);
				return e.HelpString;
			}

	// Get the number of children underneath this control.
	internal int GetNumChildren()
			{
				return numChildren;
			}

	// Get a specific child by index.
	internal Control GetChildByIndex(int index)
			{
				if(index >= 0 && index < numChildren)
				{
					return children[index];
				}
				else
				{
					return null;
				}
			}

	// Get the focused child, or this control if it has focus.
	internal Control GetFocusedChild()
			{
				if(toolkitWindow != null && toolkitWindow.Focused)
				{
					return this;
				}

				for(int i = (numChildren - 1); i >= 0; --i)
				{
					if(children[i].ContainsFocus)
					{
						return children[i];
					}
				}
				return null;
			}

	// Get the selected child, or this control if it is selected.
	[TODO]
	internal Control GetSelectedChild()
			{
				return null;
			}

	// Get the keyboard shortcut.
	internal virtual String GetKeyboardShortcut()
			{
				return null;
			}

	// Perform the default accessibility action for this control.
	internal virtual void DoDefaultAction()
			{
				// Nothing to do here.
			}

	// Get the help topic for this accessible object.
	internal virtual int GetHelpTopic(out String fileName)
			{
				fileName = null;
				return -1;
			}

	internal BorderStyle BorderStyleInternal
			{
				get
				{
					return borderStyle;
				}
				set
				{
					borderStyle = value;
					RecreateHandle();
				}
			}
	[TODO]
	public class ControlAccessibleObject : AccessibleObject
	{
		private Control ownerControl;

		public override string DefaultAction
				{
					get
					{
						return base.DefaultAction;
					}
				}

		public override string Description
				{
					get
					{
						return base.Description;
					}
				}

		public IntPtr Handle
				{
					get
					{
						return IntPtr.Zero;
					}

					set
					{
					}
				}

		public override string Help
				{
					get
					{
						return base.Help;
					}
				}

		public override string KeyboardShortcut
				{
					get
					{
						return base.KeyboardShortcut;
					}
				}

		public override string Name
				{
					get
					{
						return base.Name;
					}

					set
					{
						ownerControl.AccessibleName = value;
					}
				}

		public Control Owner
				{
					get
					{
						return ownerControl;
					}
				}

		private Label PreviousLabel
				{
					get
					{
						return null;
					}
				}

		public override AccessibleRole Role
				{
					get
					{
						return base.Role;
					}
				}

		public ControlAccessibleObject(Control ownerControl)
				{
					this.ownerControl = ownerControl;
				}

		public override int GetHelpTopic(out string fileName)
				{
					fileName = "";
					return 0;
				}

		public void NotifyClients(AccessibleEvents accEvent)
				{
				}

		public void NotifyClients(AccessibleEvents accEvent, int childID)
				{
				}

		public override string ToString()
				{
					return "ControlAccessibleObject: Owner = " + Owner == null ? "null" : Owner.ToString();
				}

		static ControlAccessibleObject()
				{
				}
	}


#endif // !CONFIG_COMPACT_FORMS

}; // class Control

}; // namespace System.Windows.Forms
