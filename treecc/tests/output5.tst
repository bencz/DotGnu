// test output logic for virtual operations, with C++ output

%option lang = "C++"
%option reentrant

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

%operation %virtual void infer_type(expression *this)

infer_type(binary)
{
	type_code type1, type2;

	expr1->infer_type();
	type1 = expr1->type;

	expr2->infer_type();
	type2 = expr2->type;

	if(type1 == float_type || type2 == float_type)
	{
		type = float_type;
	}
	else
	{
		type = int_type;
	}
}

infer_type(unary)
{
	expr->infer_type();
	type = expr->type;
}

infer_type(intnum)
{
	type = int_type;
}

infer_type(power)
{
	expr1->infer_type();
	expr2->infer_type();

	if(expr2->type != int_type)
	{
		error("second sub-expression to `^' is not an integer");
	}

	type = expr1->type;
}
