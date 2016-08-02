TESTS = run_tests
TEST_ENVIRONMENT = $(SHELL)

EXTRA_DIST= run_tests \
			input1.out \
			input1.tst \
			input2.out \
			input2.tst \
			input3.out \
			input3.tst \
			input4.out \
			input4.tst \
			input5.out \
			input5.tst \
			input6.out \
			input6.tst \
			input7.out \
			input7.tst \
			input8.out \
			input8.tst \
			input9.out \
			input9.tst \
			input10.out \
			input10.tst \
			output1.out \
			output1.tst \
			output2.out \
			output2.tst \
			output3.out \
			output3.tst \
			output4.out \
			output4.tst \
			output5.out \
			output5.tst \
			output6.out \
			output6.tst \
			output7.out \
			output7.tst \
			output8.out \
			output8.tst \
			output9.out \
			output9.tst \
			output10.out \
			output10.tst \
			output11.out \
			output11.tst \
			output12.out \
			output12.tst \
			output13.out \
			output13.tst \
			output14.out \
			output14.tst \
			output15.out \
			output15.tst \
			output16.out \
			output16.tst \
			output17.out \
			output17.tst \
			output18.out \
			output18.tst \
			parse1.out \
			parse1.tst \
			parse2.out \
			parse2.tst \
			parse3.out \
			parse3.tst \
			parse4.out \
			parse4.tst \
			parse5.out \
			parse5.tst \
			parse6.out \
			parse6.tst \
			parse7.out \
			parse7.tst \
			parse8.out \
			parse8.tst \
			parse9.out \
			parse9.tst \
			parse10.out \
			parse10.tst \
			test_list
	
noinst_PROGRAMS = test_input test_parse test_output normalize

test_input_SOURCES = test_input.c
test_input_LDADD = $(top_builddir)/libtreecc.a

test_parse_SOURCES = test_parse.c
test_parse_LDADD = $(top_builddir)/libtreecc.a

test_output_SOURCES = test_output.c
test_output_LDADD = $(top_builddir)/libtreecc.a

normalize_SOURCES = normalize.c

AM_CFLAGS = -I$(top_srcdir)

MAINTAINERCLEANFILES = Makefile.in
