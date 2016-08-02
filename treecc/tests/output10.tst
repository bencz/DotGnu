// test output logic for inline non-virtual operations in Java
// that are within a class wrapper.

%option lang = "Java"
%option package = "org.test"

%enum type_code =
{
	int_type,
	float_type
}

%node expression %abstract %typedef =
{
	%nocreate type_code type = {type_code.int_type};
}

%node binary expression %abstract =
{
	expression expr1;
	expression expr2;
}

%node unary expression %abstract =
{
	;
	expression expr;
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

%operation %inline void Infer::infer_type(expression e)

infer_type(binary)
{
	type_code type1, type2;

	infer_type(e.expr1);
	type1 = e.expr1.type;

	infer_type(e.expr2);
	type2 = e.expr2.type;

	if(type1 == type_code.float_type || type_code.type2 == float_type)
	{
		e.type = type_code.float_type;
	}
	else
	{
		e.type = type_code.int_type;
	}
}

infer_type(unary)
{
	infer_type(e.expr);
	e.type = e.expr.type;
}

infer_type(intnum)
{
	e.type = type_code.int_type;
}

infer_type(power)
{
	infer_type(e.expr1);
	infer_type(e.expr2);

	if(e.expr2.type != type_code.int_type)
	{
		error("second sub-expression to `^' is not an integer");
	}

	e.type = e.expr1.type;
}
