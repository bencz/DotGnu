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
.method public hidebysig instance class ['.library']'System'.'String' 'm1'(int32 'x') cil managed 
{
	ldstr	""
	ret
	.maxstack 1
} // method m1
.method public static hidebysig class ['.library']'System'.'String' 'm2'(int32 'x') cil managed 
{
	ldstr	""
	ret
	.maxstack 1
} // method m2
.method public virtual hidebysig newslot instance class ['.library']'System'.'String' 'm3'(int32 'x') cil managed 
{
	ldstr	""
	ret
	.maxstack 1
} // method m3
.method public hidebysig instance class 'D1' 'c1'() cil managed 
{
	ldarg.0
	ldftn	instance class ['.library']'System'.'String' 'Test'::'m1'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	ret
	.maxstack 2
} // method c1
.method public hidebysig instance class 'D1' 'c2'() cil managed 
{
	ldnull
	ldftn	class ['.library']'System'.'String' 'Test'::'m2'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	ret
	.maxstack 2
} // method c2
.method public hidebysig instance class 'D1' 'c3'() cil managed 
{
	ldarg.0
	dup
	ldvirtftn	instance class ['.library']'System'.'String' 'Test'::'m3'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	ret
	.maxstack 2
} // method c3
.method public hidebysig instance class 'D1' 'c4'(class 'Test' 't') cil managed 
{
	ldarg.1
	dup
	ldvirtftn	instance class ['.library']'System'.'String' 'Test'::'m3'(int32)
	newobj	instance void 'D1'::.ctor(class [.library]System.Object, native int)
	ret
	.maxstack 2
} // method c4
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
