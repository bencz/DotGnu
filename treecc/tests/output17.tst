// test output logic for libgc

%option gc_allocator

%node expression %abstract %typedef =
{
	%nocreate type_code type;
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
%node power binary

%operation %inline void infer_type(expression *e)

infer_type(binary)
{
	type_code type1, type2;

	infer_type(e->expr1);
	type1 = e->expr1->type;

	infer_type(e->expr2);
	type2 = e->expr2->type;

	if(type1 == float_type || type2 == float_type)
	{
		e->type = float_type;
	}
	else
	{
		e->type = int_type;
	}
}

infer_type(unary)
{
	infer_type(e->expr);
	e->type = e->expr->type;
}

infer_type(intnum)
{
	e->type = int_type;
}

infer_type(power)
{
	infer_type(e->expr1);
	infer_type(e->expr2);

	if(e->expr2->type != int_type)
	{
		error("second sub-expression to `^' is not an integer");
	}

	e->type = e->expr1->type;
}
