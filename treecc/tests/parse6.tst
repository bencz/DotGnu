// test operation case definitions

%node toplevel %abstract %typedef
%node expression toplevel %abstract %typedef
%node binary expression %abstract
%node unary expression %abstract
%node intnum expression
%node plus binary
%node minus binary
%node multiply binary
%node divide binary
%node negate unary
%node power binary
%node expression2 %abstract %typedef

// test the normal case
%operation %virtual void op(expression *e)

op(binary)
{
	codeA;
}

op(unary)
{
	codeB;
}

// test for undeclared operations
op2(expression)
{
}

// test for undeclared types
op(notdeclared)
{
}

// test for bad parameter syntax
op(power,)
{
}

// test for invalid number of triggers
op(expression, expression)
{
}

op()
{
}

// test for incorrect inheritance relationship
op(expression2)
{
}

op(toplevel)
{
}

// test for a missing code block
op(minus)
op(plus)
