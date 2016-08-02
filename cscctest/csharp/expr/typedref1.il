.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private sequential sealed serializable ansi 'W' extends ['.library']'System'.'ValueType'
{
.field private int32 'w'
} // class W
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(typedref 'tref') cil managed 
{
	.locals init	(class ['.library']'System'.'Type', int32, class 'Test', class 'Test'[], valuetype 'W')
	ldarg.1
	refanytype
	call	class [.library]System.Type [.library]System.Type::GetTypeFromHandle(valuetype [.library]System.RuntimeTypeHandle)
	stloc.0
	ldarg.1
	refanyval	int32
	ldind.i4
	stloc.1
	ldarg.1
	refanyval	'Test'
	ldind.ref
	stloc.2
	ldarg.1
	refanyval	class 'Test'[]
	ldind.ref
	stloc.3
	ldarg.1
	refanyval	'W'
	ldobj	'W'
	stloc.s	4
	ret
	.maxstack 1
} // method m1
.field private int32 'fld'
.field private valuetype 'W' 'fld2'
.field private static class 'Test' 'fld3'
.method private hidebysig instance void 'm2'() cil managed 
{
	.locals init	(int32)
	ldc.i4.0
	stloc.0
	ldarg.0
	ldloca.s	0
	mkrefany	int32
	callvirt	instance void 'Test'::'m1'(typedref)
	ldarg.0
	ldarg.0
	ldflda	int32 'Test'::'fld'
	mkrefany	int32
	callvirt	instance void 'Test'::'m1'(typedref)
	ldarg.0
	ldarg.0
	ldflda	valuetype 'W' 'Test'::'fld2'
	mkrefany	'W'
	callvirt	instance void 'Test'::'m1'(typedref)
	ldarg.0
	ldsflda	class 'Test' 'Test'::'fld3'
	mkrefany	'Test'
	callvirt	instance void 'Test'::'m1'(typedref)
	ret
	.maxstack 2
} // method m2
.method private hidebysig instance void 'm3'(int32[] 'x', int32 'y') cil managed 
{
	ldarg.0
	ldarg.1
	ldarg.2
	ldelema	int32
	mkrefany	int32
	callvirt	instance void 'Test'::'m1'(typedref)
	ret
	.maxstack 3
} // method m3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
