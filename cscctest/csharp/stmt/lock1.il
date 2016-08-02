.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(class 'Test' 't') cil managed 
{
	.locals init	(class ['.library']'System'.'Object')
	ldarg.1
	dup
	stloc.0
	call	void [.library]System.Threading.Monitor::Enter(class [.library]System.Object)
	.try {
	ldarg.0
	ldarg.1
	callvirt	instance void 'Test'::'m2'(class ['.library']'System'.'Object')
	leave	?L1
	}
	finally {
	ldloc.0
	call	void [.library]System.Threading.Monitor::Exit(class [.library]System.Object)
	endfinally
	}
?L1:
	ret
	.maxstack 2
} // method m1
.method private hidebysig instance void 'm2'(class ['.library']'System'.'Object' 't') cil managed 
{
	ret
	.maxstack 0
} // method m2
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
