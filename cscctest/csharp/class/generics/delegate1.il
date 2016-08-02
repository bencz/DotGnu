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
.class public auto sealed serializable ansi 'Action`1'<'T'> extends ['.library']'System'.'MulticastDelegate'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'object', native int 'method') runtime managed {}
.method public virtual hidebysig newslot instance void 'Invoke'(!0 'obj') runtime managed {}
.method public virtual hidebysig newslot instance class ['.library']'System'.'IAsyncResult' 'BeginInvoke'(!0 'obj', class ['.library']'System'.'AsyncCallback' 'callback', class ['.library']'System'.'Object' 'object') runtime managed {}
.method public virtual hidebysig newslot instance void 'EndInvoke'(class ['.library']'System'.'IAsyncResult' 'result') runtime managed {}
} // class Action`1
} // namespace System
.namespace 'System'
{
.class public auto sealed serializable ansi 'Comparision`1'<'T'> extends ['.library']'System'.'MulticastDelegate'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'object', native int 'method') runtime managed {}
.method public virtual hidebysig newslot instance int32 'Invoke'(!0 'x', !0 'y') runtime managed {}
.method public virtual hidebysig newslot instance class ['.library']'System'.'IAsyncResult' 'BeginInvoke'(!0 'x', !0 'y', class ['.library']'System'.'AsyncCallback' 'callback', class ['.library']'System'.'Object' 'object') runtime managed {}
.method public virtual hidebysig newslot instance int32 'EndInvoke'(class ['.library']'System'.'IAsyncResult' 'result') runtime managed {}
} // class Comparision`1
} // namespace System
.namespace 'System'
{
.class public auto sealed serializable ansi 'Converter`2'<'T', 'U'> extends ['.library']'System'.'MulticastDelegate'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'object', native int 'method') runtime managed {}
.method public virtual hidebysig newslot instance !1 'Invoke'(!0 'from') runtime managed {}
.method public virtual hidebysig newslot instance class ['.library']'System'.'IAsyncResult' 'BeginInvoke'(!0 'from', class ['.library']'System'.'AsyncCallback' 'callback', class ['.library']'System'.'Object' 'object') runtime managed {}
.method public virtual hidebysig newslot instance !1 'EndInvoke'(class ['.library']'System'.'IAsyncResult' 'result') runtime managed {}
} // class Converter`2
} // namespace System
