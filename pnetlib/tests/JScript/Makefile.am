
all-local:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/TestJScript.build" all

CLEANFILES = TestJScript.dll

TESTS = TestJScript.dll
TESTS_ENVIRONMENT = $(SHELL) $(top_builddir)/tools/run_test.sh $(top_builddir)
