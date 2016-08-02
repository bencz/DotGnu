/*
 * TestUri.cs - Test class for "System.Uri" 
 *
 * Copyright (C) 2002  Free Software Foundation, Inc.
 * 
 * Contributed by Stephen Compall <rushing@sigecom.net>
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

using CSUnit;
using System;

/*
my sage (cough) advice to anyone writing unit tests...have a little
fun! Leave some little jokes to make future generations of hackers
chortle as they wait for their 2 second long build process to
complete.
*/

public class TestUri : TestCase
{

	Uri rmsUri, pathEnding, noPathEnding;
	const String rmsString
				="ftp://rms@ftp.gnu.org:2538/pub/gnu/?freesoftware=good";
	const String guileDir= "http://www.gnu.org/software/../software/guile/";
	const String guileFile= "http://www.gnu.org/software/../software/guile";

	// Constructor.
	public TestUri(String name)	: base(name)
	{
		// Nothing to do here.
	}

	// Set up for the tests.
	protected override void Setup()
	{
		this.rmsUri = new Uri(rmsString);
		this.pathEnding = new Uri(guileDir);
		this.noPathEnding = new Uri(guileFile);
	}

	// Clean up after the tests.
	protected override void Cleanup()
	{
		// Nothing to do here.
	}

	public void TestUriConstructor()
	{
		String lasturi = null;
		try // good constructors
		{
			new Uri(lasturi = rmsString);
			new Uri(lasturi = guileDir);
			new Uri(lasturi = guileFile);
			new Uri(lasturi = "mailto:gnu@gnu.org");
			new Uri(lasturi = "jabber:gnu@gnu.org/MyPresence");
		}
		catch (Exception)
		{
			Fail(lasturi.ToString()+" threw an exception it shouldn't have!");
		}
	}

	public void TestUriCanonicalize()
	{
		AssertEquals("Should keep the ending slash when there is one",
				     "/software/guile/", pathEnding.AbsolutePath);
		
		AssertEquals("Shouldn't have an ending slash when there isn't one",
			     	"/software/guile",
				     noPathEnding.AbsolutePath);
	}

	public void TestUriCheckHostName()
	{
		AssertEquals("www.gnu.org is a DNS name",
			     UriHostNameType.Dns,
			     Uri.CheckHostName("www.gnu.org"));
		AssertEquals("www.southern.-storm.com.au is not a DNS name",
			     UriHostNameType.Unknown,
			     Uri.CheckHostName("www.southern.-storm.com.au"));
		AssertEquals("www.southern-storm.com.au is a DNS name",
			     UriHostNameType.Dns,
			     Uri.CheckHostName("www.southern-storm.com.au"));
		AssertEquals("127.0.0.1 is an IPv4 address",
			     UriHostNameType.IPv4,
			     Uri.CheckHostName("127.0.0.1"));
		AssertEquals(".63.64.201.1 is not an IPv4 address",
			     UriHostNameType.Unknown,
			     Uri.CheckHostName(".63.64.201.1"));
		AssertEquals("207..211.18.4 is not an IPv4 address",
			     UriHostNameType.Unknown,
			     Uri.CheckHostName("207..211.18.4"));

		// checking IPng

		AssertEquals(":F0F0::0 should have bad IPng zerocompress at beginning",
			     UriHostNameType.Unknown,
			     Uri.CheckHostName(":F0F0::0"));

		AssertEquals("::F0F0:0 allows fake zerocompress at beginning",
			     UriHostNameType.IPv6,
			     Uri.CheckHostName("::F0F0:0"));
		AssertEquals("0:1:2:3:4:5:6:127.0.0.1 has too many elements",
			     UriHostNameType.Unknown,
			     Uri.CheckHostName("0:1:2:3:4:5:6:127.0.0.1"));
		AssertEquals("0:1:2:3:4:5:127.0.0.1 has the right number of elements",
			     UriHostNameType.IPv6,
			     Uri.CheckHostName("0:1:2:3:4:5:127.0.0.1"));
	}

	public void TestUriCheckSchemeName()
	{
		Assert("Anr.7 is a scheme name",
		       Uri.CheckSchemeName("Anr.7"));
		Assert("6thsense is not a scheme name",
		       !Uri.CheckSchemeName("6thsense"));
		Assert("gnu+freedom-limits is a scheme name",
		       Uri.CheckSchemeName("gnu+freedom-limits"));
		// that's GNU plus Freedom minus Limits
	}

	// TestUriCheckSecurity() is not necessary

	// TODO
	public void TestUriEquals()
	{
	}

	// TODO
	public void TestUriEscape()
	{
	}

	// TODO
	public void TestUriEscapeString()
	{
	}

	// TODO
	public void TestUriFromHex()
	{
	}

	// TODO
	public void TestUriGetHashCode()
	{
	}

	// TODO
	public void TestUriGetLeftPart()
	{
	}

	// TODO
	public void TestUriHexEscape()
	{
	}

	// TODO
	public void TestUriHexUnescape()
	{
	}

	// TODO
	public void TestUriIsBadFileSystemCharacter()
	{
	}

	// TODO
	public void TestUriIsExcludedCharacter()
	{
	}

	public void TestUriIsHexDigit()
	{
		// gee, this is a hard one
		Assert("C is a hex digit", Uri.IsHexDigit('C'));
		Assert("9 is a hex digit", Uri.IsHexDigit('9'));
		// incidentally, 0xC9 in binary is...11001001
		Assert("f is a hex digit", Uri.IsHexDigit('f'));
		Assert("G is not a hex digit", !Uri.IsHexDigit('G'));
		Assert("\x00C9 is not a hex digit",
		       !Uri.IsHexDigit('\x00C9')); // I am one funny guy
		Assert("\x20AC is not a hex digit (then again, neither is $)",
		       !Uri.IsHexDigit('\x20AC'));
		// ok, classes like this don't really need all this testing ;)
	}

	public void TestUriIsHexEncoding()
	{
		Assert("\"%c9\", position 0, is hex encoding", Uri.IsHexEncoding("%c9", 0));
		Assert("\"%c9\", position -1, is not hex encoding", !Uri.IsHexEncoding("%c9", -1));
		Assert("\"0x%c9\", position 3, is not hex encoding", !Uri.IsHexEncoding("0x%c9", 3));
		Assert("\"0x%c9\", position 2, is hex encoding", Uri.IsHexEncoding("0x%c9", 2));
		Assert("\"%at\", position 0, is not hex encoding", !Uri.IsHexEncoding("%at", 0));
		Assert("\"%af\", position 100, is not hex encoding", !Uri.IsHexEncoding("%af", 100));
	}

	// TODO
	public void TestUriIsReservedCharacter()
	{
	}

	public void TestUriMakeRelative()
	{
		Uri gnuphil = new Uri("http://www.gnu.org/philosophy/why-free.html");
		Uri gnuoreilly = new Uri("http://www.gnu.org/gnu/thegnuproject.html");
		Uri mozillaftp = new Uri("ftp://ftp.mozilla.org/pub/mozilla/latest/mozilla-i686-pc-linux-gnu-sea.tar.gz");
		Uri mozillahttp = new Uri("http://ftp.mozilla.org/pub/mozilla/latest/mozilla-i686-pc-linux-gnu-sea.tar.gz");
		Uri mandrake = new Uri("ftp://distro.ibiblio.org/pub/Linux/distributions/mandrake/Mandrake/iso/");
		Uri debian = new Uri("ftp://distro.ibiblio.org/pub/Linux/distributions/debian/main/");
		Uri debianrelease = new Uri("ftp://distro.ibiblio.org/pub/Linux/distributions/debian/main/source/Release");

		AssertEquals(
				"Code figures out simple relative Uri correctly (with files)",
			     gnuphil.MakeRelative(gnuoreilly),
			     "../gnu/thegnuproject.html");
		AssertEquals("notices different schemes when comparing Uris",
			     mozillaftp.MakeRelative(mozillahttp),
			     mozillahttp.AbsoluteUri);
		AssertEquals("figures out more complex, directory-based relative Uri",
			     mandrake.MakeRelative(debian),
			     "../../../debian/main/");
		AssertEquals("tells difference between files and directorys, by looking for ending slash",
			debianrelease.MakeRelative(debian), "../");
		AssertEquals("correctly goes further into subdirectories",
			debian.MakeRelative(debianrelease), "source/Release");
	}

	// Parse N/A

	public void TestUriToString()
	{
		Uri uri=new Uri("http://dotgnu.org:80");	
		AssertEquals("Removing default ports from uris",
					"http://dotgnu.org/",
					uri.ToString());
		uri = new Uri("mailto:developers:secret@dotgnu.org");
		AssertEquals("Passwords in the uris",
					"mailto:developers:secret@dotgnu.org",
					uri.ToString());
	}

	// TODO
	public void TestUriUnescape()
	{
	}

	// TODO
	public void TestUriAbsolutePath()
	{
	}

	// TODO
	public void TestUriAbsoluteUri()
	{
	}

	public void TestUriAuthority()
	{
		AssertEquals("rmsUri: Authority built correctly", rmsUri.Authority, "rms@ftp.gnu.org:2538");
	}

	// TODO
	public void TestUriFragment()
	{
	}

	public void TestUriHost()
	{
		AssertEquals("rmsUri: Host parsed", rmsUri.Host, "ftp.gnu.org");
	}
	public void TestUriHostNameType()
	{
		AssertEquals("rmsUri: Correct HostNameType detected", rmsUri.HostNameType, UriHostNameType.Dns);
	}
	public void TestUriIsDefaultPort()
	{
		Assert("rmsUri: 2538 is not default for ftp", !rmsUri.IsDefaultPort);
	}

	// TODO
	public void TestUriIsFile()
	{
	}

	// TODO
	public void TestUriIsLoopback()
	{
	}

	// TODO
	public void TestUriLocalPath()
	{
	}

	// TODO
	public void TestUriPathAndQuery()
	{

	}
	public void TestUriPort()
	{
		AssertEquals("rmsUri: Port parsed", rmsUri.Port, 2538);
	}
	public void TestUriQuery()
	{
		AssertEquals("rmsUri: Query parsed", rmsUri.Query, "?freesoftware=good");
	}
	public void TestUriScheme()
	{
		AssertEquals("rmsUri: Scheme parsed", rmsUri.Scheme, "ftp");
	}

	// TODO
	public void TestUriUserEscaped()
	{
	}

	public void TestUriUserInfo()
	{
		AssertEquals("rmsUri: User info parsed", rmsUri.UserInfo, "rms");
	}
}
