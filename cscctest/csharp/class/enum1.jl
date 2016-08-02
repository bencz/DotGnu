.assembly extern '.library'
{
	.ver 0:0:0:0
}
.assembly '<Assembly>'
{
	.ver 0:0:0:0
}
.module '<Module>'
.class private auto sealed serializable ansi 'Color' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'Color' 'Red' = int32(0x00000000)
.field public static literal valuetype 'Color' 'Green' = int32(0x00000001)
.field public static literal valuetype 'Color' 'Blue' = int32(0x00000002)
.field public specialname rtspecialname int32 'value__'
} // class Color
.class private auto sealed serializable ansi 'ColorMask' extends ['.library']'System'.'Enum'
{
.field public static literal valuetype 'ColorMask' 'Red' = int64(0x0000FFFF00000000)
.field public static literal valuetype 'ColorMask' 'Green' = int64(0x00000000FFFF0000)
.field public static literal valuetype 'ColorMask' 'Blue' = int64(0x000000000000FFFF)
.field public specialname rtspecialname unsigned int64 'value__'
} // class ColorMask
