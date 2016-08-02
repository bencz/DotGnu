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
.class public auto sealed serializable ansi 'E' extends ['.library']'System'.'MulticastDelegate'
{
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'object', native int 'method') runtime managed {}
.method public virtual hidebysig newslot instance int32 'Invoke'() runtime managed {}
.method public virtual hidebysig newslot instance class ['.library']'System'.'IAsyncResult' 'BeginInvoke'(class ['.library']'System'.'AsyncCallback' 'callback', class ['.library']'System'.'Object' 'object') runtime managed {}
.method public virtual hidebysig newslot instance int32 'EndInvoke'(class ['.library']'System'.'IAsyncResult' 'result') runtime managed {}
} // class E
} // namespace Test
.namespace 'Test'
{
.class public auto abstract sealed ansi beforefieldinit 'Test1' extends ['.library']'System'.'Object'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.field private static int32 'g'
.field public static int32 'H'
.field private static literal int32 'I' = int32(0x00000003)
.field public static literal int32 'J' = int32(0x00000004)
.method public static hidebysig specialname int32 'get_G'() cil managed 
{
	ldsfld	int32 'Test'.'Test1'::'g'
	ret
	.maxstack 1
} // method get_G
.method public static hidebysig specialname void 'set_G'(int32 'value') cil managed 
{
	ldarg.0
	stsfld	int32 'Test'.'Test1'::'g'
	ret
	.maxstack 1
} // method set_G
.property int32 'G'()
{
	.get int32 'Test'.'Test1'::'get_G'()
	.set void 'Test'.'Test1'::'set_G'(int32)
} // property G
.method public static hidebysig specialname void 'add_E'(class 'Test'.'E' 'value') cil managed 
{
	ldsfld	class 'Test'.'E' 'Test'.'Test1'::'E'
	ldarg.0
	call	class [.library]System.Delegate [.library]System.Delegate::Combine(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'Test'.'E'
	stsfld	class 'Test'.'E' 'Test'.'Test1'::'E'
	ret
	.maxstack 2
} // method add_E
.method public static hidebysig specialname void 'remove_E'(class 'Test'.'E' 'value') cil managed 
{
	ldsfld	class 'Test'.'E' 'Test'.'Test1'::'E'
	ldarg.0
	call	class [.library]System.Delegate [.library]System.Delegate::Remove(class [.library]System.Delegate, class [.library]System.Delegate)
	castclass	'Test'.'E'
	stsfld	class 'Test'.'E' 'Test'.'Test1'::'E'
	ret
	.maxstack 2
} // method remove_E
.field private static class 'Test'.'E' 'E'
.event 'Test'.'E' 'E'
{
	.addon void 'Test'.'Test1'::'add_E'(class 'Test'.'E')
	.removeon void 'Test'.'Test1'::'remove_E'(class 'Test'.'E')
} // event E
.method public static hidebysig specialname int32 'get_Item'(int32 'i') cil managed 
{
	ldarg.0
	ret
	.maxstack 1
} // method get_Item
.method public static hidebysig specialname void 'set_Item'(int32 'i', int32 'value') cil managed 
{
	ret
	.maxstack 0
} // method set_Item
.property int32 'Item'(int32)
{
	.get int32 'Test'.'Test1'::'get_Item'(int32)
	.set void 'Test'.'Test1'::'set_Item'(int32, int32)
} // property Item
.method private static hidebysig specialname rtspecialname void '.cctor'() cil managed 
{
	ldc.i4.1
	stsfld	int32 'Test'.'Test1'::'g'
	ldc.i4.2
	stsfld	int32 'Test'.'Test1'::'H'
	ret
	.maxstack 1
} // method .cctor
} // class Test1
} // namespace Test
