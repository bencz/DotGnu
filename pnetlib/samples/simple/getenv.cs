// An example that demonstrates access to environment variables.
//
// "getenv" with no command-line arguments will print the values
// of all environment variables.
//
// "getenv" with an argument will print the value of just that
// environment variable.
//
// "getenv" with two arguments, where the first is "-d", will
// access environment variables by way of a dictionary indexer.

using System;
using System.Collections;

public class getenv
{
	public static void Main(String[] args)
	{
		String value;
		IDictionary vars;

		if(args.Length == 2 && args[0] == "-d")
		{
			// Access the value through a dictionary indexer.
			vars = Environment.GetEnvironmentVariables();
			value = (String)(vars[args[1]]);
			if(value != null)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.Write(args[1]);
				Console.WriteLine(" does not exist in the environment");
			}
		}
		else if(args.Length == 1)
		{
			// Access the value through "GetEnvironmentVariable".
			value = Environment.GetEnvironmentVariable(args[0]);
			if(value != null)
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.Write(args[0]);
				Console.WriteLine(" does not exist in the environment");
			}
		}
		else
		{
			// Dump the contents of all environment variables.
			vars = Environment.GetEnvironmentVariables();
			IDictionaryEnumerator e = vars.GetEnumerator();
			while(e.MoveNext())
			{
				Console.Write((String)(e.Key));
				Console.Write("=");
				Console.WriteLine((String)(e.Value));
			}
		}
	}
}
