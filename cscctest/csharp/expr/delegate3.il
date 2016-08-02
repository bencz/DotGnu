.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'System'
{
.class public auto interface abstract ansi 'IAsyncResult'
{
} // class IAsyncResult
} // namespace System
.namespace 'System'
{
.class public auto sealed serializable ansi 'AsyncCallback' extends ['.library']'System'.'MulticastDelegate'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'object', native int 'method') runtime managed {}
.method public virtual hidebysig newslot instance void 'Invoke'(class 'System'.'IAsyncResult' 'result') runtime managed {}
.method public virtual hidebysig newslot instance class 'System'.'IAsyncResult' 'BeginInvoke'(class 'System'.'IAsyncResult' 'result', class 'System'.'AsyncCallback' 'callback', class ['.library']'System'.'Object' 'object') runtime managed {}
.method public virtual hidebysig newslot instance void 'EndInvoke'(class 'System'.'IAsyncResult' 'result') runtime managed {}
} // class AsyncCallback
} // namespace System
.class public auto sealed serializable ansi 'D1' extends ['.library']'System'.'MulticastDelegate'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'object', native int 'method') runtime managed {}
.method public virtual hidebysig newslot instance class ['.library']'System'.'String' 'Invoke'(int32 'x') runtime managed {}
.method public virtual hidebysig newslot instance class 'System'.'IAsyncResult' 'BeginInvoke'(int32 'x', class 'System'.'AsyncCallback' 'callback', class ['.library']'System'.'Object' 'object') runtime managed {}
.method public virtual hidebysig newslot instance class ['.library']'System'.'String' 'EndInvoke'(class 'System'.'IAsyncResult' 'result') runtime managed {}
} // class D1
.class public auto ansi 'Test' extends ['.library']'System'.'Object'
{
.field private class 'D1' 'f'
.method public hidebysig instance class 'D1' 'Add'(class 'D1' 'x', class 'D1' 'y') cil managed 
{
	ldarg.1
	ldarg.2
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	ret
	.maxstack 2
} // method Add
.method public hidebysig instance class 'D1' 'Sub'(class 'D1' 'x', class 'D1' 'y') cil managed 
{
	ldarg.1
	ldarg.2
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	ret
	.maxstack 2
} // method Sub
.method public hidebysig instance void 'Assign'(class 'D1' 'x') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'f'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'f'
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'f'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'f'
	ret
	.maxstack 3
} // method Assign
.method public hidebysig instance bool 'Eq'(class 'D1' 'x', class 'D1' 'y') cil managed 
{
	ldarg.1
	ldarg.2
	call	bool ['.library']'System'.'MulticastDelegate'::'op_Equality'(class ['.library']'System'.'MulticastDelegate', class ['.library']'System'.'MulticastDelegate')
	ret
	.maxstack 2
} // method Eq
.method public hidebysig instance bool 'Ne'(class 'D1' 'x', class 'D1' 'y') cil managed 
{
	ldarg.1
	ldarg.2
	call	bool ['.library']'System'.'MulticastDelegate'::'op_Inequality'(class ['.library']'System'.'MulticastDelegate', class ['.library']'System'.'MulticastDelegate')
	ret
	.maxstack 2
} // method Ne
.method public hidebysig instance bool 'IsNull'(class 'D1' 'x') cil managed 
{
	ldarg.1
	ldnull
	call	bool ['.library']'System'.'MulticastDelegate'::'op_Equality'(class ['.library']'System'.'MulticastDelegate', class ['.library']'System'.'MulticastDelegate')
	ret
	.maxstack 2
} // method IsNull
.method public hidebysig instance bool 'IsNonNull'(class 'D1' 'x') cil managed 
{
	ldarg.1
	ldnull
	call	bool ['.library']'System'.'MulticastDelegate'::'op_Inequality'(class ['.library']'System'.'MulticastDelegate', class ['.library']'System'.'MulticastDelegate')
	ret
	.maxstack 2
} // method IsNonNull
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
