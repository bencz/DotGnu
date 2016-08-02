
.PHONY: System.Data.dll

all-local: Parser.cs System.Data.dll

System.Data.dll:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-b "$(srcdir)" -B . -f System.Data.build all

Parser.cs : $(MONO_SOURCE_PATH)/class/System.Data/Mono.Data.SqlExpressions/Parser.jay $(top_srcdir)/jay/skeleton.cs
	$(top_builddir)/jay/jay -ct < $(top_srcdir)/jay/skeleton.cs $< >$@

CLEANFILES = System.Data.dll Parser.cs

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/System.Data.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/System.Data.build" -u all
