// This example demonstrates catching exceptions and then
// reporting the complete stack trace of the throw point.

using System;

class Hello
{
	public static void ThrowUp(int x)
	{
		throw new Exception();
	}

	public static int ThrowUpDivZero(int x)
	{
		return 3 / x;
	}

	public static void Main()
	{
		Console.WriteLine("Testing user-created exception:");
		Console.WriteLine();
		try
		{
			ThrowUp(0);
		}
		catch(Exception e)
		{
			Console.Write("Caught: ");
			Console.WriteLine(e.ToString());
		}
		Console.WriteLine("Testing system-created exception:");
		Console.WriteLine();
		try
		{
			ThrowUpDivZero(0);
		}
		catch(Exception e)
		{
			Console.Write("Caught: ");
			Console.WriteLine(e.ToString());
		}
	}
}
