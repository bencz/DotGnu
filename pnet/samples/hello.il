.assembly extern mscorlib
{
	.ver 1:0:2411:0
}

.assembly hello
{
}

.class Hello extends [mscorlib]System.Object
{
	.method private static void Main() il managed
  	{
  		.maxstack 1
		.entrypoint
		ldstr	"Hello World!"
		call	void [mscorlib]System.Console::WriteLine(class [mscorlib]System.String)
		ret
	}
}
