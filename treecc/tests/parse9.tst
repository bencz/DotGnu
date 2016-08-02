// test multi-trigger operation case handling

%enum type_code =
{
	int_type,
	float_type
}

%node expression %abstract %typedef =
{
	%nocreate type_code type = {int_type};
}

%node binary expression %abstract =
{
	expression *expr1;
	expression *expr2;
}

%node unary expression %abstract =
{
	;
	expression *expr;
	;
}

%node intnum expression =
{
	int num;
}

%node plus binary
%node minus binary
%node multiply binary
%node divide binary
%node negate unary

%node power binary = {;;}

%operation void gen_code([binary *node], [type_code type])

gen_code(plus, int_type) {}
gen_code(plus, float_type) {}
gen_code(minus, int_type) {}
gen_code(multiply, float_type) {}

%operation void no_cases([binary *node], [type_code type])
