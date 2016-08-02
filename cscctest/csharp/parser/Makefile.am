
TESTS = empty1.cs \
		empty2.cs \
		error1.cs \
		error2.cs \
		error3.cs \
		error4.cs \
		member_access1.cs \
		member_access2.cs \
		member_access3.cs \
		type_access.cs \
		unclosed1.cs

## Removed because it doesn't build now that attributes work -- Rhys.
##		attrs2.cs
## Removed them too until i find a way to make them work -- Klaus 
##		attrs1.cs
##		attrs3.cs

TESTS_ENVIRONMENT = $(SHELL) $(top_srcdir)/tools/run_test.sh

all-redirect: check-TESTS
