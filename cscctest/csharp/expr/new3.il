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
.class private sequential sealed serializable ansi 'X' extends ['.library']'System'.'ValueType'
{
.field public int32 'i'
.method public hidebysig specialname rtspecialname instance void '.ctor'(int32 'i') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	int32 'X'::'i'
	ret
	.maxstack 2
} // method .ctor
} // class X
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 't1'(valuetype 'X' 'x') cil managed 
{
	.locals init	(valuetype 'X' *)
	ldarga.s	1
	stloc.0
	ldloc.0
	initobj	'X'
	ldloc.0
	ldc.i4.1
	call	instance void 'X'::'.ctor'(int32)
	ret
	.maxstack 2
} // method t1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
