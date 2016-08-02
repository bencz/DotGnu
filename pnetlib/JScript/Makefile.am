
.PHONY: Microsoft.JScript.dll

all-local: Microsoft.JScript.dll

Microsoft.JScript.dll: Nodes/JNode.cs
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/JScript.build" all

TREECC_INPUTS = $(srcdir)/Nodes/JNode.tc \
				$(srcdir)/Nodes/JExpr.tc \
				$(srcdir)/Nodes/JStmt.tc

Nodes/JNode.cs: $(TREECC_INPUTS)
	test -d Nodes || mkdir Nodes
	$(TREECC) -f -o Nodes/JNode.cs $(srcdir)/Nodes/JNode.tc

CLEANFILES = Microsoft.JScript.dll JScript.Test.dll Nodes/JNode.cs jsrun.exe

install-exec-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/JScript.build" -i all
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	rm -f $(DESTDIR)$(bindir)/jsrun.exe
	rm -f $(DESTDIR)$(bindir)/jsrun
	$(SHELL) $(top_builddir)/tools/wrapper.sh \
			$(DESTDIR)$(bindir)/jsrun$(EXEEXT)

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/JScript.build" -u all
	rm -f $(DESTDIR)$(bindir)/jsrun.exe
	rm -f $(DESTDIR)$(bindir)/jsrun
