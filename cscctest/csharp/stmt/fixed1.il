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
.method private hidebysig instance void 't1'(int32[] 'array') cil managed 
{
	.locals init	(int32 *, int32 & pinned, void *, int32 & pinned)
	ldarg.1
	ldc.i4.0
	ldelema	int32
	dup
	stloc.1
	stloc.0
	ldarg.1
	ldc.i4.0
	ldelema	int32
	dup
	stloc.3
	stloc.2
	ret
	.maxstack 2
} // method t1
.method private hidebysig instance void 't2'(int32[] 'array') cil managed 
{
	.locals init	(int32 *, int32[] pinned, void *, int32[] pinned)
	ldarg.1
	dup
	stloc.1
	dup
	brfalse	?L1
	dup
	ldlen
	brfalse	?L2
	ldc.i4.0
	ldelema	int32
	conv.i
	br	?L3
?L2:
	pop
	ldnull
?L1:
	conv.i
?L3:
	stloc.0
	ldarg.1
	dup
	stloc.3
	dup
	brfalse	?L4
	dup
	ldlen
	brfalse	?L5
	ldc.i4.0
	ldelema	int32
	conv.i
	br	?L6
?L5:
	pop
	ldnull
?L4:
	conv.i
?L6:
	stloc.2
	ret
	.maxstack 3
} // method t2
.method private hidebysig instance void 't3'(class ['.library']'System'.'String' 'str') cil managed 
{
	.locals init	(char *, class ['.library']'System'.'String' pinned, void *, class ['.library']'System'.'String' pinned)
	ldarg.1
	dup
	stloc.1
	conv.i
	dup
	brfalse	?L7
	call	int32 [.library]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
	conv.i
	add
?L7:
	stloc.0
	ldarg.1
	dup
	stloc.3
	conv.i
	dup
	brfalse	?L8
	call	int32 [.library]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
	conv.i
	add
?L8:
	stloc.2
	ret
	.maxstack 2
} // method t3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
