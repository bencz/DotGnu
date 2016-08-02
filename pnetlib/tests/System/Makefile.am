
all-local:
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/TestSystem.build" all

CLEANFILES = TestSystem.dll

TESTS = TestSystem.dll
TESTS_ENVIRONMENT = $(SHELL) $(top_builddir)/tools/run_test.sh $(top_builddir)
