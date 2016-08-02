.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method public hidebysig instance void 'm1'(class 'Test' 't') cil managed 
{
	.locals init	(class ['.library']'System'.'String', class ['.library']'System'.'String', class ['.library']'System'.'String')
	ldstr	"Hello"
	stloc.0
	ldstr	" World"
	stloc.1
	ldloc.0
	ldloc.1
	call	class [.library]System.String [.library]System.String::Concat(class [.library]System.String, class [.library]System.String)
	stloc.2
	ldloc.0
	ldarg.1
	call	class [.library]System.String [.library]System.String::Concat(class [.library]System.Object, class [.library]System.Object)
	stloc.2
	ldloc.2
	ldloc.0
	call	class [.library]System.String [.library]System.String::Concat(class [.library]System.String, class [.library]System.String)
	stloc.2
	ldloc.2
	ldarg.1
	call	class [.library]System.String [.library]System.String::Concat(class [.library]System.Object, class [.library]System.Object)
	stloc.2
	ret
	.maxstack 2
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
