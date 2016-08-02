// test operations

%node expression %abstract %typedef
%node binary expression %abstract
%node unary expression %abstract
%node intnum expression
%node plus binary
%node minus binary
%node multiply binary
%node divide binary
%node negate unary
%node power binary

// test multiple declaration
%operation %virtual void op(expression *e)
%operation %virtual void op(expression *e);

// test incorrect use of %virtual with %inline
%operation %virtual %inline void op1(expression *e)
%operation %inline %virtual void op2(expression *e)

// test missing return type, name, or arguments
%operation
%operation int()
%operation void op3

// test different parameter lists
%operation %virtual int op4(expression *e, const char *name)

// test triggers on virtual operations
%operation %virtual int op5(expression *e, [intnum num])
%operation %virtual int op6(void)
%operation %virtual int op7()

// test non-virtual operation declarations
%operation void *op8(plus *p)
%operation %inline void *op9(int, [expression *e]);

// test that trigger parameters are node types
%operation void op10(int x);
%operation void op11(int x, [const char *name])
%operation void op12([int x], [const expression *e])

// test invalid parameter lists
%operation void op13([])
%operation void op14([)
%operation void op15(int, [expression *)
%operation void op16(,)
%operation void op17(plus *,)

// test lack of '*' on trigger parameters in C
%operation void op18(expression e)
%operation void op19(expression e, [expression *e2])
%operation void op20(expression e, [expression e2])

// test class names
%operation void class21::op21(expression e)
%operation void class21::(expression e)
