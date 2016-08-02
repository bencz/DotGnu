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
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.class nested private sequential sealed serializable ansi 'TestStruct' extends ['.library']'System'.'ValueType'
{
.field public int32 'i'
} // class TestStruct
.method private hidebysig instance void 't1'() cil managed 
{
	.locals init	(char, char *)
	ldloca.s	0
	stloc.1
	ret
	.maxstack 1
} // method t1
.method private hidebysig instance void 't1'(char 'c') cil managed 
{
	.locals init	(char *)
	ldarga.s	1
	stloc.0
	ret
	.maxstack 1
} // method t1
.method private hidebysig instance void 't1'(valuetype 'Test'/'TestStruct' & 't') cil managed 
{
	.locals init	(valuetype 'Test'/'TestStruct' *)
	ldarg.1
	stloc.0
	ret
	.maxstack 1
} // method t1
.method private hidebysig instance void 't2'(valuetype 'Test'/'TestStruct' 't') cil managed 
{
	.locals init	(valuetype 'Test'/'TestStruct' *)
	ldarga.s	1
	stloc.0
	ret
	.maxstack 1
} // method t2
.method private hidebysig instance void 't3'(valuetype 'Test'/'TestStruct' & 't') cil managed 
{
	.locals init	(int32 *)
	ldarg.1
	ldflda	int32 'Test'/'TestStruct'::'i'
	stloc.0
	ret
	.maxstack 1
} // method t3
.method private hidebysig instance void 't4'(valuetype 'Test'/'TestStruct' 't') cil managed 
{
	.locals init	(int32 *)
	ldarga.s	1
	ldflda	int32 'Test'/'TestStruct'::'i'
	stloc.0
	ret
	.maxstack 1
} // method t4
.method private hidebysig instance void 't4'() cil managed 
{
	.locals init	(valuetype 'Test'/'TestStruct', int32 *)
	ldloca.s	0
	ldflda	int32 'Test'/'TestStruct'::'i'
	stloc.1
	ret
	.maxstack 1
} // method t4
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
