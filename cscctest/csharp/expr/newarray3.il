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
.method private hidebysig instance void 'm1'() cil managed 
{
	.locals init	(int32[], valuetype ['.library']'System'.'Decimal'[], int32[,], int32[,])
	ldc.i4.4
	newarr	['.library']'System'.'Int32'
	dup
.data D1 = {int32(3), int32(4), int32(5), int32(6)}
	ldtoken	field valuetype 'Test'/'<T1>' 'Test'/'<D1>'::'data'
	call	void [.library]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [.library]System.Array, valuetype [.library]System.RuntimeFieldHandle)
	stloc.0
	ldc.i4.5
	newarr	['.library']'System'.'Decimal'
	dup
	ldc.i4.0
	ldelema	['.library']'System'.'Decimal'
	ldc.i4.3
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stobj	['.library']'System'.'Decimal'
	dup
	ldc.i4.1
	ldelema	['.library']'System'.'Decimal'
	ldc.i4.s	45
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.1
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stobj	['.library']'System'.'Decimal'
	dup
	ldc.i4.2
	ldelema	['.library']'System'.'Decimal'
	ldc.i4.5
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stobj	['.library']'System'.'Decimal'
	dup
	ldc.i4.3
	ldelema	['.library']'System'.'Decimal'
	ldc.i4.s	65
	ldc.i4.0
	ldc.i4.0
	ldc.i4.0
	ldc.i4.1
	newobj	instance void [.library]System.Decimal::.ctor(int32, int32, int32, bool, unsigned int8)
	stobj	['.library']'System'.'Decimal'
	dup
	ldc.i4.4
	ldelema	['.library']'System'.'Decimal'
	ldc.i4.7
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stobj	['.library']'System'.'Decimal'
	stloc.1
	ldc.i4.2
	ldc.i4.2
	newobj	instance void int32[,]::.ctor(int32, int32)
	dup
.data D2 = {int32(3), int32(4), int32(5), int32(6)}
	ldtoken	field valuetype 'Test'/'<T2>' 'Test'/'<D2>'::'data'
	call	void [.library]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [.library]System.Array, valuetype [.library]System.RuntimeFieldHandle)
	stloc.2
	ldc.i4.3
	ldc.i4.2
	newobj	instance void int32[,]::.ctor(int32, int32)
	dup
.data D3 = {int32(3), int32(4), int32(5), int32(6),
int32(7), int32(8)}
	ldtoken	field valuetype 'Test'/'<T3>' 'Test'/'<D3>'::'data'
	call	void [.library]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [.library]System.Array, valuetype [.library]System.RuntimeFieldHandle)
	stloc.3
	ret
	.maxstack 7
} // method m1
.class nested private explicit sealed '<T3>' extends [.library]System.ValueType
{
	.pack 1
	.size 24
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
} // class Test
