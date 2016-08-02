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
.field private class 'D1' 'x'
.method public hidebysig specialname instance void 'add_c1'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'c1'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'c1'
	ret
	.maxstack 3
} // method add_c1
.method public hidebysig specialname instance void 'remove_c1'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'c1'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'c1'
	ret
	.maxstack 3
} // method remove_c1
.field private class 'D1' 'c1'
.event 'D1' 'c1'
{
	.addon instance void 'Test'::'add_c1'(class 'D1')
	.removeon instance void 'Test'::'remove_c1'(class 'D1')
} // event c1
.method public static hidebysig specialname void 'add_c2'(class 'D1' 'value') cil managed 
{
	ldsfld	class 'D1' 'Test'::'c2'
	ldarg.0
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stsfld	class 'D1' 'Test'::'c2'
	ret
	.maxstack 2
} // method add_c2
.method public static hidebysig specialname void 'remove_c2'(class 'D1' 'value') cil managed 
{
	ldsfld	class 'D1' 'Test'::'c2'
	ldarg.0
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stsfld	class 'D1' 'Test'::'c2'
	ret
	.maxstack 2
} // method remove_c2
.field private static class 'D1' 'c2'
.event 'D1' 'c2'
{
	.addon void 'Test'::'add_c2'(class 'D1')
	.removeon void 'Test'::'remove_c2'(class 'D1')
} // event c2
.method public virtual hidebysig newslot specialname instance void 'add_c3'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'c3'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'c3'
	ret
	.maxstack 3
} // method add_c3
.method public virtual hidebysig newslot specialname instance void 'remove_c3'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'c3'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'c3'
	ret
	.maxstack 3
} // method remove_c3
.field private class 'D1' 'c3'
.event 'D1' 'c3'
{
	.addon instance void 'Test'::'add_c3'(class 'D1')
	.removeon instance void 'Test'::'remove_c3'(class 'D1')
} // event c3
.method public hidebysig specialname instance void 'add_p1'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'x'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'x'
	ret
	.maxstack 3
} // method add_p1
.method public hidebysig specialname instance void 'remove_p1'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test'::'x'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test'::'x'
	ret
	.maxstack 3
} // method remove_p1
.event 'D1' 'p1'
{
	.addon instance void 'Test'::'add_p1'(class 'D1')
	.removeon instance void 'Test'::'remove_p1'(class 'D1')
} // event p1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
.class public auto ansi 'Test2' extends 'Test'
{
.method public virtual hidebysig specialname instance void 'add_c3'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test2'::'c3'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test2'::'c3'
	ret
	.maxstack 3
} // method add_c3
.method public virtual hidebysig specialname instance void 'remove_c3'(class 'D1' 'value') cil managed 
{
	ldarg.0
	dup
	ldfld	class 'D1' 'Test2'::'c3'
	ldarg.1
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'D1'
	stfld	class 'D1' 'Test2'::'c3'
	ret
	.maxstack 3
} // method remove_c3
.field private class 'D1' 'c3'
.event 'D1' 'c3'
{
	.addon instance void 'Test2'::'add_c3'(class 'D1')
	.removeon instance void 'Test2'::'remove_c3'(class 'D1')
} // event c3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test2
.class public auto ansi 'Test3' extends 'Test2'
{
.method public virtual hidebysig specialname instance void 'add_c3'(class 'D1' 'value') cil managed 
{
	ret
	.maxstack 0
} // method add_c3
.method public virtual hidebysig specialname instance void 'remove_c3'(class 'D1' 'value') cil managed 
{
	ret
	.maxstack 0
} // method remove_c3
.event 'D1' 'c3'
{
	.addon instance void 'Test3'::'add_c3'(class 'D1')
	.removeon instance void 'Test3'::'remove_c3'(class 'D1')
} // event c3
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test2'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test3
.class public auto ansi 'Test4' extends 'Test'
{
.method public hidebysig instance class ['.library']'System'.'String' 'm1'(int32 'x') cil managed 
{
	ldstr	""
	ret
	.maxstack 1
} // method m1
.method public hidebysig instance void 'Add'(class 'Test' 't') cil managed 
{
	ldarg.1
	ldarg.0
	ldftn	instance class ['.library']'System'.'String' 'Test4'::'m1'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	call	instance void 'Test'::'add_c1'(class 'D1')
	ldarg.1
	ldarg.0
	ldftn	instance class ['.library']'System'.'String' 'Test4'::'m1'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	callvirt	instance void 'Test'::'add_c3'(class 'D1')
	ldarg.0
	ldftn	instance class ['.library']'System'.'String' 'Test4'::'m1'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	call	void 'Test'::'add_c2'(class 'D1')
	ret
	.maxstack 3
} // method Add
.method public hidebysig instance void 'Sub'(class 'Test' 't') cil managed 
{
	ldarg.1
	ldarg.0
	ldftn	instance class ['.library']'System'.'String' 'Test4'::'m1'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	call	instance void 'Test'::'remove_c1'(class 'D1')
	ldarg.1
	ldarg.0
	ldftn	instance class ['.library']'System'.'String' 'Test4'::'m1'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	callvirt	instance void 'Test'::'remove_c3'(class 'D1')
	ldarg.0
	ldftn	instance class ['.library']'System'.'String' 'Test4'::'m1'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	call	void 'Test'::'remove_c2'(class 'D1')
	ret
	.maxstack 3
} // method Sub
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void 'Test'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test4
