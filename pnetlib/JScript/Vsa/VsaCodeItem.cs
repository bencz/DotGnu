/*
 * VsaCodeItem.cs - script code item for an engine.
 *
 * Copyright (C) 2003 Southern Storm Software, Pty Ltd.
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
 
namespace Microsoft.JScript
{

using System;
using System.CodeDom;
using Microsoft.JScript.Vsa;
using Microsoft.Vsa;

internal sealed class VsaCodeItem : VsaItem, IVsaCodeItem
{
	// Internal state.
	private String sourceText;
	private JNode parsed;

	// Constructor.
	internal VsaCodeItem(VsaEngine engine, String name, VsaItemFlag flag)
			: base(engine, name, VsaItemType.Code, flag)
			{
				sourceText = null;
				parsed = null;
			}

	// Implement the "IVsaCodeItem" interface.
#if CONFIG_CODEDOM
	public CodeObject CodeDOM
			{
				get
				{
					lock(this)
					{
						CheckForClosed();
						throw new VsaException(VsaError.CodeDOMNotAvailable);
					}
				}
			}
#endif
	public String SourceText
			{
				get
				{
					lock(this)
					{
						CheckForClosed();
						return sourceText;
					}
				}
				set
				{
					lock(this)
					{
						CheckForClosed();
						sourceText = value;
					}
				}
			}
	public void AddEventSource(String eventSourceName, String eventSourceType)
			{
				lock(this)
				{
					CheckForClosed();
					throw new NotSupportedException();
				}
			}
	public void AppendSourceText(String text)
			{
				lock(this)
				{
					CheckForClosed();
					sourceText += text;
				}
			}
	public void RemoveEventSource(String eventSourceName)
			{
				lock(this)
				{
					CheckForClosed();
					throw new NotSupportedException();
				}
			}

	// Reset the compiled state of this item.
	internal override void Reset()
			{
				parsed = null;
			}

	// Compile this item.
	internal override bool Compile()
			{
				if(parsed == null && sourceText != null)
				{
					Context context = new Context(sourceText);
					context.codebase = new CodeBase(codebaseOption, this);
					context.codebase.site = engine.Site;
					JSParser parser = new JSParser(context);
					parser.printSupported = engine.printSupported;
					try
					{
						parsed = parser.ParseSource(false);
					}
					catch(JScriptException e)
					{
#if !CONFIG_SMALL_CONSOLE
						ScriptStream.Error.WriteLine(e.Message);
#else
						ScriptStream.WriteLine(e.Message);
#endif
						return false;
					}
					if(parser.numErrors > 0)
					{
						// There were errors that were partially recovered.
						parsed = null;
						return false;
					}
				}
				return true;
			}

	// Run this item.
	internal override void Run()
			{
				if(parsed != null)
				{
					int size = engine.ScriptObjectStackSize();
					try
					{
						parsed.Eval(engine);
					}
					finally
					{
						// Make sure we return to the global level, even if
						// some aberrant exception caused us to miss any
						// of the scope pops.  This is just paranoia.
						engine.ScriptObjectStackSetSize(size);
					}
				}
			}

}; // class VsaCodeItem

}; // namespace Microsoft.JScript
