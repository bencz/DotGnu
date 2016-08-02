/*
 * JSScannerTest.cs - Test support routines for JSScanner.
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

#if TEST

// This class is provided for the benefit of the test suite, which
// needs to access the internal details of "JSScanner".  This class
// MUST NOT be used in application code as it is unique to this
// implementation and will not exist elsewhere.  It is also subject
// to change without notice.  You have been warned!

public sealed class JSScannerTest
{

	// Create a scanner from a string.
	public static JSScanner TestCreateScanner(String source)
			{
				return new JSScanner(new Context(source));
			}

	// Get the last-parsed identifier name from a scanner.
	public static String TestGetIdentifierName(JSScanner scanner)
			{
				return scanner.GetIdentifierName();
			}

	// Extract token information from a scanner.
	public static Context TestGetTokenContext(JSScanner scanner)
			{
				return scanner.GetTokenContext();
			}

	// Parse a regular expression from a scanner.
	public static bool TestParseRegex(JSScanner scanner, out String regex)
			{
				return scanner.ParseRegex(out regex);
			}

	// Extract an error code from a "ScannerFailure" exception.
	public static JSError TestExtractError(Exception e)
			{
				if(e is JSScanner.ScannerFailure)
				{
					return ((JSScanner.ScannerFailure)e).error;
				}
				else
				{
					return JSError.NoError;
				}
			}

}; // class JSScannerTest

#endif // TEST

}; // namespace Microsoft.JScript
