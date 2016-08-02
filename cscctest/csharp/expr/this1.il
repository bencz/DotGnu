.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private sequential sealed serializable ansi 'Test' extends ['.library']'System'.'ValueType'
{
.field private int32 'x'
.method public hidebysig instance void 'Set'(valuetype 'Test' 'value') cil managed 
{
	ldarg.0
	ldarg.1
	stobj	'Test'
	ret
	.maxstack 2
} // method Set
} // class Test
