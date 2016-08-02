.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
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
	.get instance class ['.library']'System'.'Object' 'IEnumerator'::'get_Current'()
} // property Current
} // class IEnumerator
.class public sequential sealed serializable ansi 'DictionaryEntry' extends ['.library']'System'.'ValueType'
{
.field private class ['.library']'System'.'Object' 'key__'
.field private class ['.library']'System'.'Object' 'value__'
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed java 
{
	aload_0
	aload_1
	putfield	class ['.library']'System'.'Object' 'DictionaryEntry'::'key__'
	aload_0
	aload_2
	putfield	class ['.library']'System'.'Object' 'DictionaryEntry'::'value__'
	return
	.locals 3
	.maxstack 2
} // method .ctor
.method public hidebysig specialname instance class ['.library']'System'.'Object' 'get_Key'() cil managed java 
{
	aload_0
	getfield	class ['.library']'System'.'Object' 'DictionaryEntry'::'key__'
	areturn
	.locals 1
	.maxstack 1
} // method get_Key
.method public hidebysig specialname instance void 'set_Key'(class ['.library']'System'.'Object' 'value') cil managed java 
{
	aload_0
	aload_1
	putfield	class ['.library']'System'.'Object' 'DictionaryEntry'::'key__'
	return
	.locals 2
	.maxstack 2
} // method set_Key
.property class ['.library']'System'.'Object' 'Key'()
{
	.get instance class ['.library']'System'.'Object' 'DictionaryEntry'::'get_Key'()
	.set instance void 'DictionaryEntry'::'set_Key'(class ['.library']'System'.'Object')
} // property Key
.method public hidebysig specialname instance class ['.library']'System'.'Object' 'get_Value'() cil managed java 
{
	aload_0
	getfield	class ['.library']'System'.'Object' 'DictionaryEntry'::'value__'
	areturn
	.locals 1
	.maxstack 1
} // method get_Value
.method public hidebysig specialname instance void 'set_Value'(class ['.library']'System'.'Object' 'value') cil managed java 
{
	aload_0
	aload_1
	putfield	class ['.library']'System'.'Object' 'DictionaryEntry'::'value__'
	return
	.locals 2
	.maxstack 2
} // method set_Value
.property class ['.library']'System'.'Object' 'Value'()
{
	.get instance class ['.library']'System'.'Object' 'DictionaryEntry'::'get_Value'()
	.set instance void 'DictionaryEntry'::'set_Value'(class ['.library']'System'.'Object')
} // property Value
} // class DictionaryEntry
.class public auto interface abstract ansi 'IDictionaryEnumerator' implements 'IEnumerator'
{
.method public virtual hidebysig newslot abstract specialname instance valuetype 'DictionaryEntry' 'get_Entry'() cil managed java 
{
} // method get_Entry
.property valuetype 'DictionaryEntry' 'Entry'()
{
	.get instance valuetype 'DictionaryEntry' 'IDictionaryEnumerator'::'get_Entry'()
} // property Entry
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Key'() cil managed java 
{
} // method get_Key
.property class ['.library']'System'.'Object' 'Key'()
{
	.get instance class ['.library']'System'.'Object' 'IDictionaryEnumerator'::'get_Key'()
} // property Key
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Value'() cil managed java 
{
} // method get_Value
.property class ['.library']'System'.'Object' 'Value'()
{
	.get instance class ['.library']'System'.'Object' 'IDictionaryEnumerator'::'get_Value'()
} // property Value
} // class IDictionaryEnumerator
.class public auto interface abstract ansi 'IEnumerable'
{
.method public virtual hidebysig newslot abstract instance class 'IEnumerator' 'GetEnumerator'() cil managed java 
{
} // method GetEnumerator
} // class IEnumerable
.class public auto interface abstract ansi 'ICollection' implements 'IEnumerable'
{
.method public virtual hidebysig newslot abstract specialname instance int32 'get_Count'() cil managed java 
{
} // method get_Count
.property int32 'Count'()
{
	.get instance int32 'ICollection'::'get_Count'()
} // property Count
.method public virtual hidebysig newslot abstract specialname instance bool 'get_IsSynchronized'() cil managed java 
{
} // method get_IsSynchronized
.property bool 'IsSynchronized'()
{
	.get instance bool 'ICollection'::'get_IsSynchronized'()
} // property IsSynchronized
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_SyncRoot'() cil managed java 
{
} // method get_SyncRoot
.property class ['.library']'System'.'Object' 'SyncRoot'()
{
	.get instance class ['.library']'System'.'Object' 'ICollection'::'get_SyncRoot'()
} // property SyncRoot
} // class ICollection
.class public auto interface abstract ansi 'IDictionary' implements 'IEnumerable', 'ICollection'
{
.custom instance void ['.library']'System.Reflection'.'DefaultMemberAttribute'::'.ctor'(class ['.library']'System'.'String') = (01 00 04 49 74 65 6D 00 00)
.method public virtual hidebysig newslot abstract instance void 'Add'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed java 
{
} // method Add
.method public virtual hidebysig newslot abstract instance void 'Clear'() cil managed java 
{
} // method Clear
.method public virtual hidebysig newslot abstract instance bool 'Contains'(class ['.library']'System'.'Object' 'key') cil managed java 
{
} // method Contains
.method public virtual hidebysig newslot abstract instance class 'IDictionaryEnumerator' 'GetEnumerator'() cil managed java 
{
} // method GetEnumerator
.method public virtual hidebysig newslot abstract instance void 'Remove'(class ['.library']'System'.'Object' 'key') cil managed java 
{
} // method Remove
.method public virtual hidebysig newslot abstract specialname instance bool 'get_IsFixedSize'() cil managed java 
{
} // method get_IsFixedSize
.property bool 'IsFixedSize'()
{
	.get instance bool 'IDictionary'::'get_IsFixedSize'()
} // property IsFixedSize
.method public virtual hidebysig newslot abstract specialname instance bool 'get_IsReadOnly'() cil managed java 
{
} // method get_IsReadOnly
.property bool 'IsReadOnly'()
{
	.get instance bool 'IDictionary'::'get_IsReadOnly'()
} // property IsReadOnly
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Item'(class ['.library']'System'.'Object' 'key') cil managed java 
{
} // method get_Item
.method public virtual hidebysig newslot abstract specialname instance void 'set_Item'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed java 
{
} // method set_Item
.property class ['.library']'System'.'Object' 'Item'(class ['.library']'System'.'Object')
{
	.get instance class ['.library']'System'.'Object' 'IDictionary'::'get_Item'(class ['.library']'System'.'Object')
	.set instance void 'IDictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
} // property Item
.method public virtual hidebysig newslot abstract specialname instance class 'ICollection' 'get_Keys'() cil managed java 
{
} // method get_Keys
.property class 'ICollection' 'Keys'()
{
	.get instance class 'ICollection' 'IDictionary'::'get_Keys'()
} // property Keys
.method public virtual hidebysig newslot abstract specialname instance class 'ICollection' 'get_Values'() cil managed java 
{
} // method get_Values
.property class 'ICollection' 'Values'()
{
	.get instance class 'ICollection' 'IDictionary'::'get_Values'()
} // property Values
} // class IDictionary
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(class 'IDictionary' 'dict') cil managed java 
{
	aload_1
	invokeinterface	instance class 'IDictionaryEnumerator' 'IDictionary'::'GetEnumerator'() 1
	astore_2
	return
	.locals 3
	.maxstack 1
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
