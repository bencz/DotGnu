.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
	.custom instance void [.library]System.Security.Permissions.SecurityPermissionAttribute::.ctor(valuetype [.library]System.Security.Permissions.SecurityAction) =
		(01 00 08 00 00 00 01 00 54 02 10 53 6B 69 70 56
		 65 72 69 66 69 63 61 74 69 6F 6E 01)
}
.module '<Module>'
.custom instance void [.library]System.Security.UnverifiableCodeAttribute::.ctor() = (01 00 00 00)
.class private sequential sealed serializable ansi 'S' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
.field private int32 'y'
} // class S
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(int32)
	ldc.i4.4
	stloc.0
	sizeof	['.library']'System'.'Decimal'
	stloc.0
	sizeof	'S'
	stloc.0
	ret
	.maxstack 1
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
