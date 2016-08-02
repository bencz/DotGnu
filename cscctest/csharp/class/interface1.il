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
.method public virtual hidebysig newslot abstract instance bool 'MoveNext'() cil managed 
{
} // method MoveNext
.method public virtual hidebysig newslot abstract instance void 'Reset'() cil managed 
{
} // method Reset
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Current'() cil managed 
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
.method public hidebysig specialname rtspecialname instance void '.ctor'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	class ['.library']'System'.'Object' 'DictionaryEntry'::'key__'
	ldarg.0
	ldarg.2
	stfld	class ['.library']'System'.'Object' 'DictionaryEntry'::'value__'
	ret
	.maxstack 2
} // method .ctor
.method public hidebysig specialname instance class ['.library']'System'.'Object' 'get_Key'() cil managed 
{
	ldarg.0
	ldfld	class ['.library']'System'.'Object' 'DictionaryEntry'::'key__'
	ret
	.maxstack 1
} // method get_Key
.method public hidebysig specialname instance void 'set_Key'(class ['.library']'System'.'Object' 'value') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	class ['.library']'System'.'Object' 'DictionaryEntry'::'key__'
	ret
	.maxstack 2
} // method set_Key
.property class ['.library']'System'.'Object' 'Key'()
{
	.get instance class ['.library']'System'.'Object' 'DictionaryEntry'::'get_Key'()
	.set instance void 'DictionaryEntry'::'set_Key'(class ['.library']'System'.'Object')
} // property Key
.method public hidebysig specialname instance class ['.library']'System'.'Object' 'get_Value'() cil managed 
{
	ldarg.0
	ldfld	class ['.library']'System'.'Object' 'DictionaryEntry'::'value__'
	ret
	.maxstack 1
} // method get_Value
.method public hidebysig specialname instance void 'set_Value'(class ['.library']'System'.'Object' 'value') cil managed 
{
	ldarg.0
	ldarg.1
	stfld	class ['.library']'System'.'Object' 'DictionaryEntry'::'value__'
	ret
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
.method public virtual hidebysig newslot abstract specialname instance valuetype 'DictionaryEntry' 'get_Entry'() cil managed 
{
} // method get_Entry
.property valuetype 'DictionaryEntry' 'Entry'()
{
	.get instance valuetype 'DictionaryEntry' 'IDictionaryEnumerator'::'get_Entry'()
} // property Entry
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Key'() cil managed 
{
} // method get_Key
.property class ['.library']'System'.'Object' 'Key'()
{
	.get instance class ['.library']'System'.'Object' 'IDictionaryEnumerator'::'get_Key'()
} // property Key
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Value'() cil managed 
{
} // method get_Value
.property class ['.library']'System'.'Object' 'Value'()
{
	.get instance class ['.library']'System'.'Object' 'IDictionaryEnumerator'::'get_Value'()
} // property Value
} // class IDictionaryEnumerator
.class public auto interface abstract ansi 'IEnumerable'
{
.method public virtual hidebysig newslot abstract instance class 'IEnumerator' 'GetEnumerator'() cil managed 
{
} // method GetEnumerator
} // class IEnumerable
.class public auto interface abstract ansi 'ICollection' implements 'IEnumerable'
{
.method public virtual hidebysig newslot abstract specialname instance int32 'get_Count'() cil managed 
{
} // method get_Count
.property int32 'Count'()
{
	.get instance int32 'ICollection'::'get_Count'()
} // property Count
.method public virtual hidebysig newslot abstract specialname instance bool 'get_IsSynchronized'() cil managed 
{
} // method get_IsSynchronized
.property bool 'IsSynchronized'()
{
	.get instance bool 'ICollection'::'get_IsSynchronized'()
} // property IsSynchronized
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_SyncRoot'() cil managed 
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
.method public virtual hidebysig newslot abstract instance void 'Add'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed 
{
} // method Add
.method public virtual hidebysig newslot abstract instance void 'Clear'() cil managed 
{
} // method Clear
.method public virtual hidebysig newslot abstract instance bool 'Contains'(class ['.library']'System'.'Object' 'key') cil managed 
{
} // method Contains
.method public virtual hidebysig newslot abstract instance class 'IDictionaryEnumerator' 'GetEnumerator'() cil managed 
{
} // method GetEnumerator
.method public virtual hidebysig newslot abstract instance void 'Remove'(class ['.library']'System'.'Object' 'key') cil managed 
{
} // method Remove
.method public virtual hidebysig newslot abstract specialname instance bool 'get_IsFixedSize'() cil managed 
{
} // method get_IsFixedSize
.property bool 'IsFixedSize'()
{
	.get instance bool 'IDictionary'::'get_IsFixedSize'()
} // property IsFixedSize
.method public virtual hidebysig newslot abstract specialname instance bool 'get_IsReadOnly'() cil managed 
{
} // method get_IsReadOnly
.property bool 'IsReadOnly'()
{
	.get instance bool 'IDictionary'::'get_IsReadOnly'()
} // property IsReadOnly
.method public virtual hidebysig newslot abstract specialname instance class ['.library']'System'.'Object' 'get_Item'(class ['.library']'System'.'Object' 'key') cil managed 
{
} // method get_Item
.method public virtual hidebysig newslot abstract specialname instance void 'set_Item'(class ['.library']'System'.'Object' 'key', class ['.library']'System'.'Object' 'value') cil managed 
{
} // method set_Item
.property class ['.library']'System'.'Object' 'Item'(class ['.library']'System'.'Object')
{
	.get instance class ['.library']'System'.'Object' 'IDictionary'::'get_Item'(class ['.library']'System'.'Object')
	.set instance void 'IDictionary'::'set_Item'(class ['.library']'System'.'Object', class ['.library']'System'.'Object')
} // property Item
.method public virtual hidebysig newslot abstract specialname instance class 'ICollection' 'get_Keys'() cil managed 
{
} // method get_Keys
.property class 'ICollection' 'Keys'()
{
	.get instance class 'ICollection' 'IDictionary'::'get_Keys'()
} // property Keys
.method public virtual hidebysig newslot abstract specialname instance class 'ICollection' 'get_Values'() cil managed 
{
} // method get_Values
.property class 'ICollection' 'Values'()
{
	.get instance class 'ICollection' 'IDictionary'::'get_Values'()
} // property Values
} // class IDictionary
.class private auto ansi 'Test' extends ['.library']'System'.'Object'
{
.method private hidebysig instance void 'm1'(class 'IDictionary' 'dict') cil managed 
{
	.locals init	(class 'IDictionaryEnumerator')
	ldarg.1
	callvirt	instance class 'IDictionaryEnumerator' 'IDictionary'::'GetEnumerator'()
	stloc.0
	ret
	.maxstack 1
} // method m1
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed 
{
	ldarg.0
	call	instance void ['.library']'System'.'Object'::'.ctor'()
	ret
	.maxstack 1
} // method .ctor
} // class Test
