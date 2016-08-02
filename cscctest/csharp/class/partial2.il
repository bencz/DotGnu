.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'Test1'
{
.class public auto interface abstract ansi 'ITest'
{
.method public virtual hidebysig newslot abstract instance void 'f1'(class 'Test1'.'ITest' 'i') cil managed 
{
} // method f1
} // class ITest
} // namespace Test1
.namespace 'Test'
{
.class public auto ansi 'Test1' extends ['.library']'System'.'Object' implements 'Test1'.'ITest'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
.method public final virtual hidebysig newslot instance void 'f1'(class 'Test1'.'ITest' 'i') cil managed 
{
	ret
	.maxstack 0
} // method f1
} // class Test1
} // namespace Test
