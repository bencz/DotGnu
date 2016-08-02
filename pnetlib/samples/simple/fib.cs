
using System;

public class Fib
{
	// Get the n'th Fibonacci number by iteration,
	// where fib(0) = 0, fib(1) = 1.
	public static uint fib(uint n)
	{
		uint a = 0;
		uint b = 1;
		uint temp;
		if(n == 0)
		{
			return 0;
		}
		while(n > 0)
		{
			temp = a + b;
			a = b;
			b = temp;
			--n;
		}
		return b;
	}

	// Program entry point.
	public static void Main()
	{
		uint n;
		for(n = 1; n <= 10; ++n)
		{
			Console.Write(fib(n));
			Console.Write(" ");
		}
		Console.WriteLine();
	}
}
