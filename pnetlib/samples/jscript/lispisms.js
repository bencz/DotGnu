/*
	Test scoping of functions passed to other functions
	as well as anonymous functions. This is very similar
	to functional-programming (actually it *is*)
*/
function f(str, cb)
{
	if(cb == null)
	{
		// Strange as though it may seem, 
		// the str that is in scope is 
		// the place where the function is
		// defined, not where it was invoked from
		f( "Not OK", function() {
			print("Variable Merge Test: " + str);
		});
	}
	else
	{
		cb();
	}
}
f("OK", null);
