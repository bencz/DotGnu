/*
 * History.cs - Implementation of the "DotGNU.Terminal.History" class.
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

namespace DotGNU.Terminal
{

using System;
using System.Collections;

public sealed class History
{
	// Internal state.
	private static int maxHistorySize = 0;
	private static ArrayList history = new ArrayList();

	// Cannot instantiate this class.
	private History() {}

	// Add a line of input to the scroll-back history.
	public static void AddHistory(String line)
			{
				if(line == null)
				{
					line = String.Empty;
				}
				if(maxHistorySize != 0 && history.Count == maxHistorySize)
				{
					// Remove the oldest entry, to preserve the maximum size.
					history.RemoveAt(0);
				}
				history.Add(line);
			}

	// Add a line of input to the scroll-back history, if it is
	// different from the most recent line that is present.
	public static void AddHistoryUnique(String line)
			{
				if(line == null)
				{
					line = String.Empty;
				}
				if(history.Count == 0 ||
				   ((String)(history[history.Count - 1])) != line)
				{
					AddHistory(line);
				}
			}

	// Clear the scroll-back history.
	public static void ClearHistory()
			{
				history.Clear();
			}

	// Get or set the maximum history list size.  Zero if unlimited.
	public static int MaximumHistorySize
			{
				get
				{
					return maxHistorySize;
				}
				set
				{
					if(value < 0)
					{
						throw new ArgumentOutOfRangeException("value", "Argument must not be negative");
					}
					maxHistorySize = value;
				}
			}

	// Get the number of items currently in the history.
	public static int Count
			{
				get
				{
					return history.Count;
				}
			}

	// Get a particular history item.  Zero is the most recent.
	public static String GetHistory(int index)
			{
				if(index >= 0 && index < history.Count)
				{
					return (String)(history[history.Count - index - 1]);
				}
				else
				{
					return String.Empty;
				}
			}

	// Set a particular history item.  Zero is the most recent.
	public static void SetHistory(int index, String line)
			{
				if(line == null)
				{
					line = String.Empty;
				}
				if(index >= 0 && index < history.Count)
				{
					history[history.Count - index - 1] = line;
				}
			}

}; // class History

}; // namespace DotGNU.Terminal
