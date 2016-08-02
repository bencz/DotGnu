
using System;

public class Hello
{
	public static void Main(String[] args)
	{
		if(args.Length == 0)
		{
			Console.WriteLine("Hello World!");
		}
		else
		{
			Console.Write("Hello");
			foreach(String value in args)
			{
				Console.Write(' ');
				Console.Write(value);
			}
			Console.WriteLine("!");
		}
	}
}
