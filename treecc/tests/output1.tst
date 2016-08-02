// test good node definitions

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

%node power binary = {;;}
