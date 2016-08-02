.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.namespace 'System2'
{
.class public auto ansi 'Uri' extends ['.library']'System'.'Object'
{
.method public static hidebysig valuetype 'System2'.'UriHostNameType' 'CheckHostName'(class ['.library']'System'.'String' 'name') cil managed java 
{
	aload_0
	ifnonnull	?L1
	iconst_0
	ireturn
?L1:
	iconst_3
	ireturn
	.locals 1
	.maxstack 1
} // method CheckHostName
.method public hidebysig specialname rtspecialname instance void '.ctor'() cil managed java 
{
	aload_0
	invokespecial	instance void ['.library']'System'.'Object'::'.ctor'()
	return
	.locals 1
	.maxstack 1
} // method .ctor
} // class Uri
} // namespace System2
.namespace 'System2'
{
.class public auto sealed serializable ansi 'UriHostNameType' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'System2'.'UriHostNameType' 'Dns' = int32(0x00000002)
.field public static literal valuetype 'System2'.'UriHostNameType' 'IPv4' = int32(0x00000003)
.field public static literal valuetype 'System2'.'UriHostNameType' 'IPv6' = int32(0x00000004)
.field public static literal valuetype 'System2'.'UriHostNameType' 'Unknown' = int32(0x00000000)
.field public specialname rtspecialname int32 'value__'
} // class UriHostNameType
} // namespace System2
