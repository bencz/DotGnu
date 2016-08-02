// test enumerated types
%enum
%enum A
%enum B =
%enum C =
{
	D, E, F, G
}
%enum H = {}
%enum I = {J,
%operation void op1(C c)
%operation void op2(D c)
%operation %virtual void op3(C c)
%operation void op4(C *c)
