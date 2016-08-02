// test split non-virtual operations

%enum C =
{
	D, E, F, G
}

%operation %inline %split void coerce([C x], [C y])

coerce(D, D)
{
	printf("Hello 1\n");
}
coerce(D, C)
{
	printf("Hello 2\n");
}
coerce(C, C)
{
	printf("Hello 3\n");
}
