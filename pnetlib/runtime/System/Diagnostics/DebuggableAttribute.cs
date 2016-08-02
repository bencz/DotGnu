/*
 * DebuggableAttribute.cs - Implementation of the
 *			"System.Diagnostics.DebuggableAttribute" class.
 *
 * Copyright (C) 2001  Southern Storm Software, Pty Ltd.
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

namespace System.Diagnostics
{

#if !ECMA_COMPAT

#if CONFIG_FRAMEWORK_2_0
using System.Runtime.InteropServices;
#endif

[AttributeUsage(AttributeTargets.Assembly |
				AttributeTargets.Module,
				AllowMultiple=false)]
public sealed class DebuggableAttribute : Attribute
{
#if CONFIG_FRAMEWORK_2_0
	[Flags]
	[ComVisible(true)]
	public enum DebuggingModes
	{
		None							= 0,
		Default							= 1,
		IgnoreSymbolStoreSequencePoints = 2,
		EnableEditAndContinue			= 4,
		DisableOptimizations			= 256
	}

	DebuggingModes debuggingFlags;
#else
	// Internal state.
	private bool jitTracking;
	private bool disableOpt;
#endif

#if CONFIG_FRAMEWORK_2_0
	// Constructors.
	public DebuggableAttribute(DebuggingModes modes)
			{
				debuggingFlags = modes;
			}

	public DebuggableAttribute(bool enableJITTracking,
							   bool disableJITOptimizer)
			{
				debuggingFlags = DebuggingModes.None;
				if(enableJITTracking)
				{
					debuggingFlags |= DebuggingModes.Default;
				}
				if(disableJITOptimizer)
				{
					debuggingFlags |= DebuggingModes.DisableOptimizations;
				}
			}

	// Properties.
	public bool IsJITTrackingEnabled
			{
				get
				{
					return (debuggingFlags & DebuggingModes.Default) != DebuggingModes.None;
				}
			}
	public bool IsJITOptimizerDisabled
			{
				get
				{
					return (debuggingFlags & DebuggingModes.DisableOptimizations) != DebuggingModes.None;
				}
			}
	public DebuggingModes DebuggingFlags
			{
				get
				{
					return debuggingFlags;
				}
			}
#else
	// Constructors.
	public DebuggableAttribute(bool enableJITTracking,
							   bool disableJITOptimizer)
			{
				jitTracking = enableJITTracking;
				disableOpt = disableJITOptimizer;
			}

	// Properties.
	public bool IsJITTrackingEnabled
			{
				get
				{
					return jitTracking;
				}
			}
	public bool IsJITOptimizerDisabled
			{
				get
				{
					return disableOpt;
				}
			}
#endif
}; // class DebuggableAttribute

#endif // !ECMA_COMPAT

}; // namespace System.Diagnostics
