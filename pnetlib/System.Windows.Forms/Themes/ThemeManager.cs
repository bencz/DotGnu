/*
 * ThemeManager.cs - Implementation of the
 *			"System.Windows.Forms.Themes.ThemeManager" class.
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

using System;
using System.Reflection;

namespace System.Windows.Forms.Themes
{
	internal sealed class ThemeManager
	{
		// Internal state.
		private static IThemePainter mainPainter;
		private static IThemePainter systemPainter;

		// Initialize the theme painters.
		static ThemeManager()
		{
			mainPainter = CreateDefaultThemePainter();
			systemPainter = mainPainter;
		}

		// Get the main .NET theme painter.
		public static IThemePainter MainPainter
		{
			get
			{
				return mainPainter;
			}
		}

		// Get the system theme painter, which will usually be the same
		// as the main painter, but may differ in some themes.
		public static IThemePainter SystemPainter
		{
			get
			{
				return systemPainter;
			}
		}

		// Get the painter for a particular flat style.
		public static IThemePainter PainterForStyle(FlatStyle style)
		{
			if(style == FlatStyle.System)
			{
				return SystemPainter;
			}
			else
			{
				return MainPainter;
			}
		}

		// Determine if this platform appears to be Unix-ish.
		private static bool IsUnix()
		{
			#if !ECMA_COMPAT
			if(Environment.OSVersion.Platform != PlatformID.Unix)
				#else
					if(Path.DirectorySeparatorChar == '\\' ||
					Path.AltDirectorySeparatorChar == '\\')
				#endif
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		// Get the override ThemePainter name.
		private static String GetThemeOverride()
		{
			String name = null;

			// Search for "--theme" in the command-line options.		
			String[] args = Environment.GetCommandLineArgs();
			int index;
			name = null;
			for(index = 1; index < args.Length; ++index)
			{			
				if(args[index].StartsWith("--theme="))
				{
					name = args[index].Substring(8);
				}
			}
		
			// Check the environment next.
			if(name == null)
			{
				name = Environment.GetEnvironmentVariable(
									"PNET_WINFORMS_THEME");
			}

			// Bail out if no ThemePainter name specified.
			if(name == null || name == String.Empty)
			{
				return null;
			}

			// Prepend "System.Windows.Forms.Themes." if necessary.
			if(name.IndexOf('.') == -1)
			{
				name = "System.Windows.Forms.Themes." + name;
			}
			return name;
		}

		private static IThemePainter CreateThemePainter(string name)
		{
			try
			{
				// Load the ThemePainter's assembly.
				Assembly assembly = Assembly.Load(name);

				Type type = assembly.GetType("System.Windows.Forms.Themes.ThemePainter");
				if(type == null)
				{
					throw new NotSupportedException();
				}
				// Instantiate ThemePainter and return it.
				ConstructorInfo ctor = type.GetConstructor(new Type [0]);
				return (IThemePainter)(ctor.Invoke(new Object [0]));
			}
			catch
			{
				// some how the new ThemePainter failed
				return new DefaultThemePainter();
			}
		}

		// Create the default ThemePainter.
		private static IThemePainter CreateDefaultThemePainter()
		{
			#if CONFIG_REFLECTION
				// Determine the name of the theme we wish to use.
				String name = GetThemeOverride();
				if(name == null)
				{
					return new DefaultThemePainter();
				}
				else
				{
					// Load the DLL with the provided assembly name and use it to theme.
					return CreateThemePainter(name);
				}
			#else
				// We can't tell what platform were on so use DefaultThemePainter
				return new DefaultThemePainter();
			#endif
		}
	}; // class ThemeManager

}; // namespace System.Windows.Forms.Themes
