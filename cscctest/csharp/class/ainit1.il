.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto ansi beforefieldinit 'Test' extends ['.library']'System'.'Object'
{
.field public static int32[] 'x'
.field public static int64[] 'y'
.method public static hidebysig void 'test'() cil managed 
{
	.locals init	(int32[])
	ldc.i4.4
	newarr	['.library']'System'.'Int32'
	dup
.data D1 = {int32(0), int32(1), int32(2), int32(3)}
	ldtoken	field valuetype 'Test'/'<T1>' 'Test'/'<D1>'::'data'
	call	void [.library]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [.library]System.Array, valuetype [.library]System.RuntimeFieldHandle)
	stloc.0
	ret
	.maxstack 3
} // method test
.class nested private explicit sealed '<T1>' extends [.library]System.ValueType
{
	.pack 1
	.size 16
}
.class nested assembly '<D1>' extends [.library]System.Object
{
	.field assembly static valuetype 'Test'/'<T1>' 'data' at D1
}
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.4
	newarr	['.library']'System'.'Int32'
	dup
.data D2 = {int32(0), int32(1), int32(2), int32(3)}
	ldtoken	field valuetype 'Test'/'<T2>' 'Test'/'<D2>'::'data'
	call	void [.library]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [.library]System.Array, valuetype [.library]System.RuntimeFieldHandle)
	stsfld	int32[] 'Test'::'x'
	ldc.i4.4
	newarr	['.library']'System'.'Int64'
	dup
.data D3 = {int64(0x0000000000000000), int64(0x0000000000000001), int64(0x0000000000000002), int64(0x3000000000000000)}
	ldtoken	field valuetype 'Test'/'<T3>' 'Test'/'<D3>'::'data'
	call	void [.library]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [.library]System.Array, valuetype [.library]System.RuntimeFieldHandle)
	stsfld	int64[] 'Test'::'y'
	ret
	.maxstack 3
} // method .cctor
.class nested private explicit sealed '<T3>' extends [.library]System.ValueType
{
	.pack 1
	.size 32
}
.class nested assembly '<D3>' extends [.library]System.Object
{
	.field assembly static valuetype 'Test'/'<T3>' 'data' at D3
}
.class nested private explicit sealed '<T2>' extends [.library]System.ValueType
{
	.pack 1
	.size 16
}
.class nested assembly '<D2>' extends [.library]System.Object
{
	.field assembly static valuetype 'Test'/'<T2>' 'data' at D2
}
} // class Test
