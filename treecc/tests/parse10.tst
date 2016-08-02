// test C# operation definition errors

%option lang = "C#"

%node expr %typedef

%operation void op1(expr e)

op1(expr)
{
}

%operation void op2::op2(expr e)

op2(expr)
{
}

%operation void Op3::op3(expr e)

op3(expr)
{
}
