// This sample prints information about the code pages that are present
// in the system library.  Usage as follows:
//
//    ilrun codepage.exe
//       -- List all code pages using a short one line per page form.
//
//    ilrun codepage.exe -v
//       -- List all code pages using a verbose form.
//
//    ilrun codepage.exe nnn
//       -- Print verbose information about code page "nnn".
//
//    ilrun codepage.exe name
//       -- Print verbose information about web encoding "name".
//
//    ilrun codepage.exe -u nnn
//       -- Dump mappings from Unicode to code page "nnn".
//
//    ilrun codepage.exe -u name
//       -- Dump mappings from Unicode to web encoding "name".
//
//    ilrun codepage.exe -c nnn
//       -- Dump mappings from code page "nnn" to Unicode.
//
//    ilrun codepage.exe -c name
//       -- Dump mappings from web encoding "name" to Unicode.

using System;
using System.Text;

class CodePage
{
#if !ECMA_COMPAT
	// Print information about an encoding.
	private static void PrintEncoding(Encoding enc, bool verbose, bool isdef)
	{
		if(verbose)
		{
			Console.Write("{0}: ", enc.CodePage);
			Console.Write(enc.EncodingName);
			if(isdef)
			{
				Console.Write(" [default encoding]");
			}
			Console.WriteLine();
			Console.Write("\tBodyName=");
			Console.WriteLine(enc.BodyName);
			Console.Write("\tHeaderName=");
			Console.WriteLine(enc.HeaderName);
			Console.Write("\tWebName=");
			Console.WriteLine(enc.WebName);
			Console.Write("\tIsBrowserDisplay=");
			Console.WriteLine(enc.IsBrowserDisplay);
			Console.Write("\tIsBrowserSave=");
			Console.WriteLine(enc.IsBrowserSave);
			Console.Write("\tIsMailNewsDisplay=");
			Console.WriteLine(enc.IsMailNewsDisplay);
			Console.Write("\tIsMailNewsSave=");
			Console.WriteLine(enc.IsMailNewsSave);
			Console.Write("\tWindowsCodePage=");
			Console.WriteLine(enc.WindowsCodePage);
		}
		else
		{
			Console.Write("{0,5}: ", enc.CodePage);
			Console.Write(enc.EncodingName);
			if(isdef)
			{
				Console.Write(" [default encoding]");
			}
			Console.WriteLine();
		}
	}

	// Print all code pages in the system.
	private static void PrintAll(bool verbose)
	{
		int page;
		int defaultPage;
		Encoding enc;
		enc = Encoding.Default;
		defaultPage = enc.CodePage;
		for(page = 1; page < 65536; ++page)
		{
			try
			{
				enc = Encoding.GetEncoding(page);
			}
			catch(NotSupportedException)
			{
				enc = null;
			}
			catch(ArgumentException)
			{
				enc = null;
			}
			if(enc != null)
			{
				PrintEncoding(enc, verbose, (page == defaultPage));
				if(verbose)
				{
					Console.WriteLine();
				}
			}
		}
	}

	private static String hexcharsLower = "0123456789abcdef";
	private static String hexcharsUpper = "0123456789ABCDEF";

	// Dump a value in hexadecimal.
	private static void DumpHex(int value, int numDigits)
	{
		String hexchars;
		if(numDigits >= 0)
		{
			hexchars = hexcharsLower;
		}
		else
		{
			hexchars = hexcharsUpper;
			numDigits = -numDigits;
		}
		int shift = (numDigits * 4) - 4;
		while(shift >= 0)
		{
			Console.Write(hexchars[(value >> shift) & 0x0F]);
			shift -= 4;
		}
	}

	// Names for the first 33 characters of the ASCII character set.
	private static String[] ctrlNames =
		{"NUL ", "SOH ", "STX ", "ETX ", "EOT ", "ENQ ",
		 "ACK ", "BEL ", "BS  ", "HT  ", "LF  ", "VT  ",
		 "FF  ", "CR  ", "SO  ", "SI  ", "DLE ", "DC1 ",
		 "DC2 ", "DC3 ", "DC4 ", "NAK ", "SYN ", "ETB ",
		 "CAN ", "EM  ", "SUB ", "ESC ", "FS  ", "GS  ",
		 "RS  ", "US  ", "SP  "};

	// Dump the 8-bit byte to Unicode character mappings
	// for an encoding.
	private static void Dump8BitMappings(Encoding enc)
	{
		byte[] buf = new byte [1];
		char[] chars = new char [enc.GetMaxCharCount(1)];
		int value, numChars, ch;
		for(value = 0; value < 256; ++value)
		{
			if((value % 8) == 0)
			{
				Console.WriteLine();
				DumpHex(value, 2);
				Console.Write(':');
			}
			buf[0] = (byte)value;
			try
			{
				numChars = enc.GetChars(buf, 0, 1, chars, 0);
			}
			catch(ArgumentException)
			{
				numChars = 0;
			}
			Console.Write(' ');
			if(numChars == 1)
			{
				ch = chars[0];
				if(ch <= 0x20)
				{
					Console.Write(ctrlNames[ch]);
				}
				else if(ch < 0x7F)
				{
					if(ch != '\'')
					{
						Console.Write('\'');
						Console.Write((char)ch);
						Console.Write('\'');
						Console.Write(' ');
					}
					else
					{
						Console.Write("\"'\" ");
					}
				}
				else if(ch == 0x7F)
				{
					Console.Write("DEL ");
				}
				else
				{
					DumpHex(ch, 4);
				}
			}
			else
			{
				Console.Write("????");
			}
			if((value % 4) == 3)
			{
				Console.Write(' ');
			}
		}
		Console.WriteLine();
	}

	// Dump the Unicode character mappings for an encoding.
	private static void DumpEncoding(Encoding enc, bool fromCodePage)
	{
		byte[] buf = new byte [enc.GetMaxByteCount(1)];
		char[] chars = new char [1];
		ushort[] usage = new ushort [65535];
		int value, numBytes, index, codeValue;
		Console.WriteLine("# Code page {0} - {1}",
						  enc.CodePage, enc.EncodingName);
		Console.WriteLine();
		for(value = 0; value < 65536; ++value)
		{
			chars[0] = (char)value;
			try
			{
				numBytes = enc.GetBytes(chars, 0, 1, buf, 0);
			}
			catch(ArgumentException)
			{
				numBytes = 0;
			}
			catch(NotSupportedException)
			{
				numBytes = 0;
			}
			if(numBytes > 0 &&
			   (numBytes != 1 || buf[0] != (byte)'?' || value == (int)'?'))
			{
				if(fromCodePage)
				{
					Console.Write("0x");
					for(index = 0; index < numBytes; ++index)
					{
						DumpHex(buf[index], -2);
					}
					Console.Write(" 0x");
					DumpHex(value, -4);
					Console.WriteLine();
				}
				else
				{
					Console.Write("<U");
					DumpHex(value, -4);
					Console.Write("> \\x");
					codeValue = 0;
					for(index = 0; index < numBytes; ++index)
					{
						codeValue = (codeValue << 8) + buf[index];
						DumpHex(buf[index], -2);
					}
					if(codeValue < 0x10000)
					{
						Console.WriteLine(" |{0}", usage[codeValue]);
						++(usage[codeValue]);
					}
					else
					{
						Console.WriteLine();
					}
				}
			}
		}
	}

	// Print information about a specific code page.
	private static void PrintPage(int page)
	{
		Encoding enc;
		try
		{
			enc = Encoding.GetEncoding(page);
		}
		catch(NotSupportedException)
		{
			enc = null;
		}
		catch(ArgumentException)
		{
			enc = null;
		}
		if(enc != null)
		{
			PrintEncoding(enc, true, false);
			Dump8BitMappings(enc);
		}
		else
		{
			Console.Write(page);
			Console.WriteLine(": unknown code page");
		}
	}

	// Print information about a specific web encoding.
	private static void PrintWebEncoding(String name)
	{
		Encoding enc;
		try
		{
			enc = Encoding.GetEncoding(name);
		}
		catch(NotSupportedException)
		{
			enc = null;
		}
		catch(ArgumentException)
		{
			enc = null;
		}
		if(enc != null)
		{
			PrintEncoding(enc, true, false);
			Dump8BitMappings(enc);
		}
		else
		{
			Console.Write(name);
			Console.WriteLine(": unknown encoding name");
		}
	}

	// Dump Unicodoe mappings for a specific code page.
	private static void DumpPage(int page, bool fromCodePage)
	{
		Encoding enc;
		try
		{
			enc = Encoding.GetEncoding(page);
		}
		catch(NotSupportedException)
		{
			enc = null;
		}
		catch(ArgumentException)
		{
			enc = null;
		}
		if(enc != null)
		{
			DumpEncoding(enc, fromCodePage);
		}
		else
		{
			Console.Write(page);
			Console.WriteLine(": unknown code page");
		}
	}

	// Dump Unicode mappings for a specific web encoding.
	private static void DumpWebEncoding(String name, bool fromCodePage)
	{
		Encoding enc;
		try
		{
			enc = Encoding.GetEncoding(name);
		}
		catch(NotSupportedException)
		{
			enc = null;
		}
		catch(ArgumentException)
		{
			enc = null;
		}
		if(enc != null)
		{
			DumpEncoding(enc, fromCodePage);
		}
		else
		{
			Console.Write(name);
			Console.WriteLine(": unknown encoding name");
		}
	}

	public static void Main(String[] args)
	{
		if(args.Length > 0)
		{
			if(args[0] == "-v")
			{
				PrintAll(true);
			}
			else if(args[0] == "-u" && args.Length > 1)
			{
				if(args[1][0] >= '0' && args[1][0] <= '9')
				{
					DumpPage(Int32.Parse(args[1]), false);
				}
				else
				{
					DumpWebEncoding(args[1], false);
				}
			}
			else if(args[0] == "-c" && args.Length > 1)
			{
				if(args[1][0] >= '0' && args[1][0] <= '9')
				{
					DumpPage(Int32.Parse(args[1]), true);
				}
				else
				{
					DumpWebEncoding(args[1], true);
				}
			}
			else if(args[0][0] >= '0' && args[0][0] <= '9')
			{
				PrintPage(Int32.Parse(args[0]));
			}
			else
			{
				PrintWebEncoding(args[0]);
			}
		}
		else
		{
			PrintAll(false);
		}
	}
#else
	public static void Main(String[] args)
	{
		Console.WriteLine("codepage.exe doesn't work in ECMA-compatible modes");
	}
#endif
}
