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
.method private hidebysig instance void 'm1_int'(int32[] 'x', int32 'y') cil managed 
{
	.locals init	(int32)
	ldarg.1
	ldarg.2
	ldelem.i4
	stloc.0
	ldarg.1
	ldarg.2
	ldloc.0
	stelem.i4
	.locals init	(int32[])
	.locals init	(int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.i4
	ldloc.0
	add
	stelem.i4
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.i4
	ldc.i4.1
	add
	dup
	stloc.2
	stelem.i4
	ldloc.2
	stloc.0
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.i4
	ldc.i4.1
	sub
	stelem.i4
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	.locals init	(int32)
	ldelem.i4
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	ldc.i4.1
	sub
	stelem.i4
	stloc.0
	ret
	.maxstack 5
} // method m1_int
.method private hidebysig instance void 'm1_byte'(unsigned int8[] 'x', int32 'y') cil managed 
{
	.locals init	(unsigned int8)
	ldarg.1
	ldarg.2
	ldelem.u1
	stloc.0
	ldarg.1
	ldarg.2
	ldloc.0
	stelem.i1
	.locals init	(unsigned int8[])
	.locals init	(int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.u1
	ldc.i4.1
	add
	conv.u1
	.locals init	(unsigned int8)
	dup
	stloc.3
	stelem.i1
	ldloc.3
	stloc.0
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.u1
	ldc.i4.1
	sub
	conv.u1
	stelem.i1
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldelem.u1
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	ldc.i4.1
	sub
	conv.u1
	stelem.i1
	stloc.0
	ret
	.maxstack 5
} // method m1_byte
.method private hidebysig instance void 'm1_long'(int64[] 'x', int32 'y') cil managed 
{
	.locals init	(int64)
	ldarg.1
	ldarg.2
	ldelem.i8
	stloc.0
	ldarg.1
	ldarg.2
	ldloc.0
	stelem.i8
	.locals init	(int64[])
	.locals init	(int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.i8
	ldloc.0
	add
	stelem.i8
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.i8
	ldc.i4.1
	conv.i8
	add
	.locals init	(int64)
	dup
	stloc.3
	stelem.i8
	ldloc.3
	stloc.0
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldloc.1
	ldloc.2
	ldelem.i8
	ldc.i4.1
	conv.i8
	sub
	stelem.i8
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldelem.i8
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	ldc.i4.1
	conv.i8
	sub
	stelem.i8
	stloc.0
	ret
	.maxstack 5
} // method m1_long
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm1_decimal'(valuetype ['.library']'System'.'Decimal'[] 'x', int32 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldelema	['.library']'System'.'Decimal'
	ldc.i4.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	stobj	['.library']'System'.'Decimal'
	.locals init	(valuetype ['.library']'System'.'Decimal'[])
	.locals init	(int32)
	ldarg.1
	dup
	stloc.0
	ldarg.2
	dup
	stloc.1
	ldelema	['.library']'System'.'Decimal'
	ldloc.0
	ldloc.1
	ldelema	['.library']'System'.'Decimal'
	ldobj	['.library']'System'.'Decimal'
	ldc.i4.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	stobj	['.library']'System'.'Decimal'
	ldarg.1
	ldarg.2
	ldelema	['.library']'System'.'Decimal'
	ldobj	['.library']'System'.'Decimal'
	ret
	.maxstack 3
} // method m1_decimal
.method private hidebysig instance class ['.library']'System'.'String' 'm1_string'(class ['.library']'System'.'String'[] 'x', int32 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldstr	"Hello World"
	stelem.ref
	ldarg.1
	ldarg.2
	ldelem.ref
	ret
	.maxstack 3
} // method m1_string
.method private hidebysig instance void 'm2_int'(int32[,] 'x', int32 'y', int32 'z') cil managed 
{
	.locals init	(int32)
	ldarg.1
	ldarg.2
	ldarg.3
	call	instance int32 int32[,]::'Get'(int32, int32)
	stloc.0
	ldarg.1
	ldarg.2
	ldarg.3
	ldloc.0
	call	instance void int32[,]::'Set'(int32, int32, int32)
	.locals init	(int32[,])
	.locals init	(int32)
	.locals init	(int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance int32 int32[,]::'Get'(int32, int32)
	ldloc.0
	add
	call	instance void int32[,]::'Set'(int32, int32, int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance int32 int32[,]::'Get'(int32, int32)
	ldc.i4.1
	add
	dup
	stloc.2
	call	instance void int32[,]::'Set'(int32, int32, int32)
	ldloc.2
	stloc.0
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance int32 int32[,]::'Get'(int32, int32)
	ldc.i4.1
	sub
	call	instance void int32[,]::'Set'(int32, int32, int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	.locals init	(int32)
	call	instance int32 int32[,]::'Get'(int32, int32)
	dup
	stloc.s	4
	ldloc.1
	ldloc.2
	ldloc.3
	ldloc.s	4
	ldc.i4.1
	sub
	call	instance void int32[,]::'Set'(int32, int32, int32)
	stloc.0
	ret
	.maxstack 6
} // method m2_int
.method private hidebysig instance void 'm2_byte'(unsigned int8[,] 'x', int32 'y', int32 'z') cil managed 
{
	.locals init	(unsigned int8)
	ldarg.1
	ldarg.2
	ldarg.3
	call	instance unsigned int8 unsigned int8[,]::'Get'(int32, int32)
	stloc.0
	ldarg.1
	ldarg.2
	ldarg.3
	ldloc.0
	call	instance void unsigned int8[,]::'Set'(int32, int32, unsigned int8)
	.locals init	(unsigned int8[,])
	.locals init	(int32)
	.locals init	(int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance unsigned int8 unsigned int8[,]::'Get'(int32, int32)
	ldc.i4.1
	add
	conv.u1
	.locals init	(unsigned int8)
	dup
	stloc.s	4
	call	instance void unsigned int8[,]::'Set'(int32, int32, unsigned int8)
	ldloc.s	4
	stloc.0
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance unsigned int8 unsigned int8[,]::'Get'(int32, int32)
	ldc.i4.1
	sub
	conv.u1
	call	instance void unsigned int8[,]::'Set'(int32, int32, unsigned int8)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	call	instance unsigned int8 unsigned int8[,]::'Get'(int32, int32)
	dup
	stloc.s	4
	ldloc.1
	ldloc.2
	ldloc.3
	ldloc.s	4
	ldc.i4.1
	sub
	conv.u1
	call	instance void unsigned int8[,]::'Set'(int32, int32, unsigned int8)
	stloc.0
	ret
	.maxstack 6
} // method m2_byte
.method private hidebysig instance void 'm2_long'(int64[,] 'x', int32 'y', int32 'z') cil managed 
{
	.locals init	(int64)
	ldarg.1
	ldarg.2
	ldarg.3
	call	instance int64 int64[,]::'Get'(int32, int32)
	stloc.0
	ldarg.1
	ldarg.2
	ldarg.3
	ldloc.0
	call	instance void int64[,]::'Set'(int32, int32, int64)
	.locals init	(int64[,])
	.locals init	(int32)
	.locals init	(int32)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance int64 int64[,]::'Get'(int32, int32)
	ldloc.0
	add
	call	instance void int64[,]::'Set'(int32, int32, int64)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance int64 int64[,]::'Get'(int32, int32)
	ldc.i4.1
	conv.i8
	add
	.locals init	(int64)
	dup
	stloc.s	4
	call	instance void int64[,]::'Set'(int32, int32, int64)
	ldloc.s	4
	stloc.0
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	ldloc.1
	ldloc.2
	ldloc.3
	call	instance int64 int64[,]::'Get'(int32, int32)
	ldc.i4.1
	conv.i8
	sub
	call	instance void int64[,]::'Set'(int32, int32, int64)
	ldarg.1
	dup
	stloc.1
	ldarg.2
	dup
	stloc.2
	ldarg.3
	dup
	stloc.3
	call	instance int64 int64[,]::'Get'(int32, int32)
	dup
	stloc.s	4
	ldloc.1
	ldloc.2
	ldloc.3
	ldloc.s	4
	ldc.i4.1
	conv.i8
	sub
	call	instance void int64[,]::'Set'(int32, int32, int64)
	stloc.0
	ret
	.maxstack 6
} // method m2_long
.method private hidebysig instance valuetype ['.library']'System'.'Decimal' 'm2_decimal'(valuetype ['.library']'System'.'Decimal'[,] 'x', int32 'y', int32 'z') cil managed 
{
	ldarg.1
	ldarg.2
	ldarg.3
	ldc.i4.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	instance void valuetype ['.library']'System'.'Decimal'[,]::'Set'(int32, int32, valuetype ['.library']'System'.'Decimal')
	.locals init	(valuetype ['.library']'System'.'Decimal'[,])
	.locals init	(int32)
	.locals init	(int32)
	ldarg.1
	dup
	stloc.0
	ldarg.2
	dup
	stloc.1
	ldarg.3
	dup
	stloc.2
	ldloc.0
	ldloc.1
	ldloc.2
	call	instance valuetype ['.library']'System'.'Decimal' valuetype ['.library']'System'.'Decimal'[,]::'Get'(int32, int32)
	ldc.i4.2
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Implicit(int32)
	call	valuetype [.library]System.Decimal [.library]System.Decimal::op_Addition(valuetype [.library]System.Decimal, valuetype [.library]System.Decimal)
	call	instance void valuetype ['.library']'System'.'Decimal'[,]::'Set'(int32, int32, valuetype ['.library']'System'.'Decimal')
	ldarg.1
	ldarg.2
	ldarg.3
	call	instance valuetype ['.library']'System'.'Decimal' valuetype ['.library']'System'.'Decimal'[,]::'Get'(int32, int32)
	ret
	.maxstack 6
} // method m2_decimal
.method private hidebysig instance class ['.library']'System'.'String' 'm2_string'(class ['.library']'System'.'String'[,] 'x', int32 'y', int32 'z') cil managed 
{
	ldarg.1
	ldarg.2
	ldarg.3
	ldstr	"Hello World"
	call	instance void class ['.library']'System'.'String'[,]::'Set'(int32, int32, class ['.library']'System'.'String')
	ldarg.1
	ldarg.2
	ldarg.3
	call	instance class ['.library']'System'.'String' class ['.library']'System'.'String'[,]::'Get'(int32, int32)
	ret
	.maxstack 4
} // method m2_string
.method private hidebysig instance int32 'm3_uint'(int32[] 'x', unsigned int32 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldc.i4.1
	add
	conv.ovf.i4.un
	ldelem.i4
	ret
	.maxstack 3
} // method m3_uint
.method private hidebysig instance int32 'm3_long'(int32[] 'x', int64 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldc.i4.1
	conv.i8
	add
	conv.ovf.i4
	ldelem.i4
	ret
	.maxstack 3
} // method m3_long
.method private hidebysig instance int32 'm3_ulong'(int32[] 'x', unsigned int64 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldc.i4.1
	conv.u8
	add
	conv.ovf.i4.un
	ldelem.i4
	ret
	.maxstack 3
} // method m3_ulong
.method private hidebysig instance int32 'm3_uint'(int32[] 'x', unsigned int16 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldelem.i4
	ret
	.maxstack 2
} // method m3_uint
.method private hidebysig instance int32 'm3_c_uint'(int32[] 'x', unsigned int32 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldc.i4.1
	add.ovf.un
	conv.ovf.i4.un
	ldelem.i4
	ret
	.maxstack 3
} // method m3_c_uint
.method private hidebysig instance int32 'm3_c_long'(int32[] 'x', int64 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldc.i4.1
	conv.i8
	add.ovf
	conv.ovf.i4
	ldelem.i4
	ret
	.maxstack 3
} // method m3_c_long
.method private hidebysig instance int32 'm3_c_ulong'(int32[] 'x', unsigned int64 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldc.i4.1
	conv.u8
	add.ovf.un
	conv.ovf.i4.un
	ldelem.i4
	ret
	.maxstack 3
} // method m3_c_ulong
.method private hidebysig instance int32 'm3_c_uint'(int32[] 'x', unsigned int16 'y') cil managed 
{
	ldarg.1
	ldarg.2
	ldelem.i4
	ret
	.maxstack 2
} // method m3_c_uint
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
