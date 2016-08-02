/* dummy main for compiling samples in profiles with insufficient features */

#if !CONFIG_REFLECTION

public class Dummy
{
	public static void Main(String[] args)
	{
		Console.WriteLine
			("This program won't work in the current pnetlib configuration.");
		Console.WriteLine
			("Recompile pnetlib with the correct profile settings.");
	}
}

#endif
