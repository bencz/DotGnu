.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.custom instance void OpenSystem.C.MemoryModelAttribute::.ctor(int32) = (01 00 40 00 00 00 00 00)
.method public static void 'm1'(int32 'x') cil managed 
{
	.locals init	(int32)
	ldarg.0
	.locals init	(int32)
	stloc.1
	ldloc.1
	ldc.i4.0
	beq	?L1
	ldloc.1
	ldc.i4.1
	beq	?L2
	ldloc.1
	ldc.i4.2
	beq	?L3
	ldloc.1
	ldc.i4.3
	beq	?L4
	br	?L5
?L1:
	ldc.i4.0
	stloc.0
	br	?L6
?L2:
	ldc.i4.1
	stloc.0
	br	?L6
?L3:
	ldc.i4.2
	stloc.0
	br	?L6
?L4:
	ldc.i4.3
	stloc.0
	br	?L6
?L5:
?L6:
	ldarg.0
	switch (
		?L7,
		?L8,
		?L9,
		?L10,
		?L11,
		?L12,
		?L13,
		?L14
	)
	br	?L15
?L7:
	ldc.i4.0
	stloc.0
	br	?L16
?L8:
	ldc.i4.1
	stloc.0
	br	?L16
?L9:
	ldc.i4.2
	stloc.0
	br	?L16
?L10:
	ldc.i4.3
	stloc.0
	br	?L16
?L11:
	ldc.i4.4
	stloc.0
	br	?L16
?L12:
	ldc.i4.5
	stloc.0
	br	?L16
?L13:
	ldc.i4.6
	stloc.0
	br	?L16
?L14:
	ldc.i4.7
	stloc.0
	br	?L16
?L15:
?L16:
	ldarg.0
	ldc.i4.1
	sub
	switch (
		?L17,
		?L18,
		?L19,
		?L20,
		?L21,
		?L22,
		?L23,
		?L24
	)
	br	?L25
?L17:
	ldc.i4.1
	stloc.0
	br	?L26
?L18:
	ldc.i4.2
	stloc.0
	br	?L26
?L19:
	ldc.i4.3
	stloc.0
	br	?L26
?L20:
	ldc.i4.4
	stloc.0
	br	?L26
?L21:
	ldc.i4.5
	stloc.0
	br	?L26
?L22:
	ldc.i4.6
	stloc.0
	br	?L26
?L23:
	ldc.i4.7
	stloc.0
	br	?L26
?L24:
	ldc.i4.8
	stloc.0
	br	?L26
?L25:
?L26:
	ldarg.0
	stloc.1
	ldloc.1
	ldc.i4	900
	bge	?L27
	ldloc.1
	ldc.i4	500
	bge	?L28
	ldloc.1
	ldc.i4.s	100
	beq	?L29
	ldloc.1
	ldc.i4	200
	beq	?L30
	ldloc.1
	ldc.i4	300
	beq	?L31
	ldloc.1
	ldc.i4	400
	beq	?L32
	br	?L33
?L28:
	ldloc.1
	ldc.i4	500
	beq	?L34
	ldloc.1
	ldc.i4	600
	beq	?L35
	ldloc.1
	ldc.i4	700
	beq	?L36
	ldloc.1
	ldc.i4	800
	beq	?L37
	br	?L33
?L27:
	ldloc.1
	ldc.i4	1300
	bge	?L38
	ldloc.1
	ldc.i4	900
	beq	?L39
	ldloc.1
	ldc.i4	1000
	beq	?L40
	ldloc.1
	ldc.i4	1100
	beq	?L41
	ldloc.1
	ldc.i4	1200
	beq	?L42
	br	?L33
?L38:
	ldloc.1
	ldc.i4	1300
	beq	?L43
	ldloc.1
	ldc.i4	1400
	beq	?L44
	ldloc.1
	ldc.i4	1500
	beq	?L45
	ldloc.1
	ldc.i4	1600
	beq	?L46
	br	?L33
?L46:
	ldc.i4	1600
	stloc.0
	br	?L47
?L45:
	ldc.i4	1500
	stloc.0
	br	?L47
?L44:
	ldc.i4	1400
	stloc.0
	br	?L47
?L43:
	ldc.i4	1300
	stloc.0
	br	?L47
?L42:
	ldc.i4	1200
	stloc.0
	br	?L47
?L41:
	ldc.i4	1100
	stloc.0
	br	?L47
?L40:
	ldc.i4	1000
	stloc.0
	br	?L47
?L39:
	ldc.i4	900
	stloc.0
	br	?L47
?L37:
	ldc.i4	800
	stloc.0
	br	?L47
?L36:
	ldc.i4	700
	stloc.0
	br	?L47
?L35:
	ldc.i4	600
	stloc.0
	br	?L47
?L34:
	ldc.i4	500
	stloc.0
	br	?L47
?L32:
	ldc.i4	400
	stloc.0
	br	?L47
?L31:
	ldc.i4	300
	stloc.0
	br	?L47
?L30:
	ldc.i4	200
	stloc.0
	br	?L47
?L29:
	ldc.i4.s	100
	stloc.0
	br	?L47
?L33:
?L47:
	ret
	.maxstack 2
} // method m1
.method public static void 'm2'(int64 'x') cil managed 
{
	.locals init	(int32)
	ldarg.0
	.locals init	(int64)
	stloc.1
	ldloc.1
	ldc.i4.0
	conv.i8
	beq	?L48
	ldloc.1
	ldc.i4.1
	conv.i8
	beq	?L49
	ldloc.1
	ldc.i4.2
	conv.i8
	beq	?L50
	ldloc.1
	ldc.i4.3
	conv.i8
	beq	?L51
	br	?L52
?L48:
	ldc.i4.0
	stloc.0
	br	?L53
?L49:
	ldc.i4.1
	stloc.0
	br	?L53
?L50:
	ldc.i4.2
	stloc.0
	br	?L53
?L51:
	ldc.i4.3
	stloc.0
	br	?L53
?L52:
?L53:
	ldarg.0
	stloc.1
	ldloc.1
	ldc.i4.4
	conv.i8
	bge	?L54
	ldloc.1
	ldc.i4.0
	conv.i8
	beq	?L55
	ldloc.1
	ldc.i4.1
	conv.i8
	beq	?L56
	ldloc.1
	ldc.i4.2
	conv.i8
	beq	?L57
	ldloc.1
	ldc.i4.3
	conv.i8
	beq	?L58
	br	?L59
?L54:
	ldloc.1
	ldc.i4.4
	conv.i8
	beq	?L60
	ldloc.1
	ldc.i4.5
	conv.i8
	beq	?L61
	ldloc.1
	ldc.i4.6
	conv.i8
	beq	?L62
	ldloc.1
	ldc.i4.7
	conv.i8
	beq	?L63
	br	?L59
?L55:
	ldc.i4.0
	stloc.0
	br	?L64
?L56:
	ldc.i4.1
	stloc.0
	br	?L64
?L57:
	ldc.i4.2
	stloc.0
	br	?L64
?L58:
	ldc.i4.3
	stloc.0
	br	?L64
?L60:
	ldc.i4.4
	stloc.0
	br	?L64
?L61:
	ldc.i4.5
	stloc.0
	br	?L64
?L62:
	ldc.i4.6
	stloc.0
	br	?L64
?L63:
	ldc.i4.7
	stloc.0
	br	?L64
?L59:
?L64:
	ret
	.maxstack 2
} // method m2
