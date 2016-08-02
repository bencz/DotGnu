.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'Test'
{
.class public auto interface abstract ansi 'ITest'
{
} // class ITest
} // namespace Test
.namespace 'Test'
{
.class public auto interface abstract ansi 'ITest`1'<'T'>
{
} // class ITest`1
} // namespace Test
.namespace 'Test'
{
.class public auto ansi 'Test1' extends ['.library']'System'.'Object' implements 'Test'.'ITest'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test1
} // namespace Test
.namespace 'Test'
{
.class public auto ansi 'Test2' extends ['.library']'System'.'Object' implements class 'Test'.'ITest`1'<int32>
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
} // namespace Test
