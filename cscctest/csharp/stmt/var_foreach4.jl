.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'System.Collections'
{
.class public auto interface abstract ansi 'IEnumerator'
{
.method public virtual hidebysig newslot abstract instance bool 'MoveNext'() cil managed java 
{
} // method MoveNext
.method public virtual hidebysig newslot abstract instance void 'Reset'() cil managed java 
{
} // method Reset
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Current'() cil managed java 
{
} // method get_Current
.property class ['.library']'System'.'Object' 'Current'()
{
	.get instance class ['.library']'System'.'Object' 'System.Collections'.'IEnumerator'::'get_Current'()
} // property Current
} // class IEnumerator
} // namespace System.Collections
.namespace 'System.Collections'
{
.class public auto interface abstract ansi 'IEnumerable'
{
.method public virtual hidebysig newslot abstract instance class 'System.Collections'.'IEnumerator' 'GetEnumerator'() cil managed java 
{
} // method GetEnumerator
} // class IEnumerable
} // namespace System.Collections
.class private auto ansi 'TestEnumerator' extends ['.library']'System'.'Object' implements 'System.Collections'.'IEnumerator'
{
.method private final virtual hidebysig newslot instance bool 'System.Collections.IEnumerator.MoveNext'() cil managed java 
{
	.override	'System.Collections'.'IEnumerator'::'MoveNext'
	iconst_0
	ireturn
	.locals 1
	.maxstack 1
} // method System.Collections.IEnumerator.MoveNext
.method public hidebysig instance bool 'MoveNext'() cil managed java 
{
	iconst_0
	ireturn
	.locals 1
	.maxstack 1
} // method MoveNext
.method private final virtual hidebysig newslot instance void 'System.Collections.IEnumerator.Reset'() cil managed java 
{
	.override	'System.Collections'.'IEnumerator'::'Reset'
	return
	.locals 1
	.maxstack 0
} // method System.Collections.IEnumerator.Reset
.method public hidebysig instance void 'Reset'() cil managed java 
{
	return
	.locals 1
	.maxstack 0
} // method Reset
.method private final virtual hidebysig newslot specialname instance class ['.library']'System'.'Object' 'System.Collections.IEnumerator.get_Current'() cil managed java 
{
	.override	'System.Collections'.'IEnumerator'::'get_Current'
	iconst_0
	invokestatic	"System/Int32" "copyIn__" "(I)LSystem/Int32;"
	areturn
	.locals 1
	.maxstack 1
} // method System.Collections.IEnumerator.get_Current
.property class ['.library']'System'.'Object' 'System.Collections.IEnumerator.Current'()
{
	.get instance class ['.library']'System'.'Object' 'TestEnumerator'::'System.Collections.IEnumerator.get_Current'()
} // property System.Collections.IEnumerator.Current
.method public hidebysig specialname instance unsigned int8 'get_Current'() cil managed java 
{
	iconst_0
	sipush	255
	iand
	ireturn
	.locals 1
	.maxstack 2
} // method get_Current
.property unsigned int8 'Current'()
{
	.get instance unsigned int8 'TestEnumerator'::'get_Current'()
} // property Current
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class TestEnumerator
.class private auto ansi 'TestEnumerable' extends ['.library']'System'.'Object' implements 'System.Collections'.'IEnumerable'
{
.method private final virtual hidebysig newslot instance class 'System.Collections'.'IEnumerator' 'System.Collections.IEnumerable.GetEnumerator'() cil managed java 
{
	.override	'System.Collections'.'IEnumerable'::'GetEnumerator'
	aconst_null
	areturn
	.locals 1
	.maxstack 1
} // method System.Collections.IEnumerable.GetEnumerator
.method public hidebysig instance class 'TestEnumerator' 'GetEnumerator'() cil managed java 
{
	aconst_null
	areturn
	.locals 1
	.maxstack 1
} // method GetEnumerator
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class TestEnumerable
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance int32 'm1'(class 'TestEnumerable' 'en') cil managed java 
{
	ldc	""
	invokestatic	"System/String" "__FromJavaString" "(Ljava/lang/String;)LSystem/String;"
	astore_2
	aload_1
	invokespecial	instance class 'TestEnumerator' 'TestEnumerable'::'GetEnumerator'()
	astore	4
	.try {
	goto	?L1
	aload	4
	invokespecial	instance unsigned int8 'TestEnumerator'::'get_Current'()
	istore_3
	aload_2
	iload_3
	invokestatic	"System/Byte" "copyIn__B" "(I)LSystem/Byte;"
	invokestatic	"System/String" "Concat" "(LSystem/Object;LSystem/Object;)LSystem/String;"
	astore_2
?L1:
	aload	4
	invokespecial	instance bool 'TestEnumerator'::'MoveNext'()
	ifne	?L2
?L3:
	jsr	?L4
	goto	?L5
	}
	catch {
	astore	5
	jsr	?L4
	aload	5
	athrow
	}
	finally {
	astore	6
	aload	4
	ifeq	?L6
	aload	4
	invokeinterface	"System/IDisposable" "Dispose" "()V" 1
?L6:
	ret	6
	}
?L5:
	iconst_0
	ireturn
	.locals 7
	.maxstack 2
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Test
