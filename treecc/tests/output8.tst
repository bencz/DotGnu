// test enumerated types
%enum C =
{
	D, E, F, G
}
%operation void op1(C c, int value)

op1(D)
{
	code1;
}

op1(E)
{
	code2;
}

op1(F)
{
	code3;
}

op1(G)
{
	code4;
}

%operation %inline void op2(C c, int value)

op2(D)
{
	code1;
}

op2(E)
{
	code2;
}

op2(C)
{
	code5;
}
