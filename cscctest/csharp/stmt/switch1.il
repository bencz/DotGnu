.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public static literal valuetype 'Color' 'Yellow' = int32(0x00000003)
.field public static literal valuetype 'Color' 'Cyan' = int32(0x00000004)
.field public static literal valuetype 'Color' 'Magenta' = int32(0x00000005)
.field public static literal valuetype 'Color' 'Black' = int32(0x00000006)
.field public static literal valuetype 'Color' 'White' = int32(0x00000007)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(int32 'x') cil managed 
{
	.locals init	(int32)
	ldarg.1
	switch (
		?L1,
		?L2,
		?L3,
		?L4
	)
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
	ldarg.1
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
	ldarg.1
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
	ldarg.1
	dup
	ldc.i4	900
	bge	?L27
	dup
	ldc.i4	500
	bge	?L28
	dup
	ldc.i4.s	100
	beq	?L29
	dup
	ldc.i4	200
	beq	?L30
	dup
	ldc.i4	300
	beq	?L31
	dup
	ldc.i4	400
	beq	?L32
	br	?L33
?L28:
	dup
	ldc.i4	500
	beq	?L34
	dup
	ldc.i4	600
	beq	?L35
	dup
	ldc.i4	700
	beq	?L36
	dup
	ldc.i4	800
	beq	?L37
	br	?L33
?L27:
	dup
	ldc.i4	1300
	bge	?L38
	dup
	ldc.i4	900
	beq	?L39
	dup
	ldc.i4	1000
	beq	?L40
	dup
	ldc.i4	1100
	beq	?L41
	dup
	ldc.i4	1200
	beq	?L42
	br	?L33
?L38:
	dup
	ldc.i4	1300
	beq	?L43
	dup
	ldc.i4	1400
	beq	?L44
	dup
	ldc.i4	1500
	beq	?L45
	dup
	ldc.i4	1600
	beq	?L46
	br	?L33
?L46:
	pop
	ldc.i4	1600
	stloc.0
	br	?L47
?L45:
	pop
	ldc.i4	1500
	stloc.0
	br	?L47
?L44:
	pop
	ldc.i4	1400
	stloc.0
	br	?L47
?L43:
	pop
	ldc.i4	1300
	stloc.0
	br	?L47
?L42:
	pop
	ldc.i4	1200
	stloc.0
	br	?L47
?L41:
	pop
	ldc.i4	1100
	stloc.0
	br	?L47
?L40:
	pop
	ldc.i4	1000
	stloc.0
	br	?L47
?L39:
	pop
	ldc.i4	900
	stloc.0
	br	?L47
?L37:
	pop
	ldc.i4	800
	stloc.0
	br	?L47
?L36:
	pop
	ldc.i4	700
	stloc.0
	br	?L47
?L35:
	pop
	ldc.i4	600
	stloc.0
	br	?L47
?L34:
	pop
	ldc.i4	500
	stloc.0
	br	?L47
?L32:
	pop
	ldc.i4	400
	stloc.0
	br	?L47
?L31:
	pop
	ldc.i4	300
	stloc.0
	br	?L47
?L30:
	pop
	ldc.i4	200
	stloc.0
	br	?L47
?L29:
	pop
	ldc.i4.s	100
	stloc.0
	br	?L47
?L33:
	pop
?L47:
	ret
	.maxstack 3
} // method m1
.method private hidebysig instance void 'm2'(int64 'x') cil managed 
{
	.locals init	(int32)
	ldarg.1
	dup
	ldc.i4.0
	conv.i8
	beq	?L48
	dup
	ldc.i4.1
	conv.i8
	beq	?L49
	dup
	ldc.i4.2
	conv.i8
	beq	?L50
	dup
	ldc.i4.3
	conv.i8
	beq	?L51
	br	?L52
?L48:
	pop
	ldc.i4.0
	stloc.0
	br	?L53
?L49:
	pop
	ldc.i4.1
	stloc.0
	br	?L53
?L50:
	pop
	ldc.i4.2
	stloc.0
	br	?L53
?L51:
	pop
	ldc.i4.3
	stloc.0
	br	?L53
?L52:
	pop
?L53:
	ldarg.1
	dup
	ldc.i4.4
	conv.i8
	bge	?L54
	dup
	ldc.i4.0
	conv.i8
	beq	?L55
	dup
	ldc.i4.1
	conv.i8
	beq	?L56
	dup
	ldc.i4.2
	conv.i8
	beq	?L57
	dup
	ldc.i4.3
	conv.i8
	beq	?L58
	br	?L59
?L54:
	dup
	ldc.i4.4
	conv.i8
	beq	?L60
	dup
	ldc.i4.5
	conv.i8
	beq	?L61
	dup
	ldc.i4.6
	conv.i8
	beq	?L62
	dup
	ldc.i4.7
	conv.i8
	beq	?L63
	br	?L59
?L55:
	pop
	ldc.i4.0
	stloc.0
	br	?L64
?L56:
	pop
	ldc.i4.1
	stloc.0
	br	?L64
?L57:
	pop
	ldc.i4.2
	stloc.0
	br	?L64
?L58:
	pop
	ldc.i4.3
	stloc.0
	br	?L64
?L60:
	pop
	ldc.i4.4
	stloc.0
	br	?L64
?L61:
	pop
	ldc.i4.5
	stloc.0
	br	?L64
?L62:
	pop
	ldc.i4.6
	stloc.0
	br	?L64
?L63:
	pop
	ldc.i4.7
	stloc.0
	br	?L64
?L59:
	pop
?L64:
	ret
	.maxstack 3
} // method m2
.method private hidebysig instance void 'm3'(class ['.library']'System'.'String' 'x') cil managed 
{
	.locals init	(int32)
	ldarg.1
	dup
	brfalse	?L65
	dup
	ldstr	"1"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L66
	dup
	ldstr	"2"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L67
	dup
	ldstr	"3"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L68
	br	?L69
?L65:
	pop
	ldc.i4.0
	stloc.0
	br	?L70
?L66:
	pop
	ldc.i4.1
	stloc.0
	br	?L70
?L67:
	pop
	ldc.i4.2
	stloc.0
	br	?L70
?L68:
	pop
	ldc.i4.3
	stloc.0
	br	?L70
?L69:
	pop
?L70:
	ldarg.1
	dup
	ldstr	"4"
	call	int32 [.library]System.String::CompareOrdinal(class [.library]System.String, class [.library]System.String)
	ldc.i4.0
	bge	?L71
	dup
	brfalse	?L72
	dup
	ldstr	"1"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L73
	dup
	ldstr	"2"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L74
	dup
	ldstr	"3"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L75
	br	?L76
?L71:
	dup
	ldstr	"4"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L77
	dup
	ldstr	"5"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L78
	dup
	ldstr	"6"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L79
	dup
	ldstr	"7"
	call	bool [.library]System.String::op_Equality(class [.library]System.String, class [.library]System.String)
	brtrue	?L80
	br	?L76
?L80:
	pop
	ldc.i4.7
	stloc.0
	br	?L81
?L79:
	pop
	ldc.i4.6
	stloc.0
	br	?L81
?L78:
	pop
	ldc.i4.5
	stloc.0
	br	?L81
?L77:
	pop
	ldc.i4.4
	stloc.0
	br	?L81
?L75:
	pop
	ldc.i4.3
	stloc.0
	br	?L81
?L74:
	pop
	ldc.i4.2
	stloc.0
	br	?L81
?L73:
	pop
	ldc.i4.1
	stloc.0
	br	?L81
?L72:
	pop
	ldc.i4.0
	stloc.0
	br	?L81
?L76:
	pop
?L81:
	ret
	.maxstack 3
} // method m3
.method private hidebysig instance void 'm4'(valuetype 'Color' 'x') cil managed 
{
	.locals init	(int32)
	ldarg.1
	switch (
		?L82,
		?L83,
		?L84,
		?L85
	)
	br	?L86
?L82:
	ldc.i4.0
	stloc.0
	br	?L87
?L83:
	ldc.i4.1
	stloc.0
	br	?L87
?L84:
	ldc.i4.2
	stloc.0
	br	?L87
?L85:
	ldc.i4.3
	stloc.0
	br	?L87
?L86:
?L87:
	ldarg.1
	switch (
		?L88,
		?L89,
		?L90,
		?L91,
		?L92,
		?L93,
		?L94,
		?L95
	)
	br	?L96
?L88:
	ldc.i4.0
	stloc.0
	br	?L97
?L89:
	ldc.i4.1
	stloc.0
	br	?L97
?L90:
	ldc.i4.2
	stloc.0
	br	?L97
?L91:
	ldc.i4.3
	stloc.0
	br	?L97
?L92:
	ldc.i4.4
	stloc.0
	br	?L97
?L93:
	ldc.i4.5
	stloc.0
	br	?L97
?L94:
	ldc.i4.6
	stloc.0
	br	?L97
?L95:
	ldc.i4.7
	stloc.0
	br	?L97
?L96:
?L97:
	ret
	.maxstack 1
} // method m4
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
