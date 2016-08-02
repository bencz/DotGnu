
.PHONY: System.Xml.dll

all-local: System.Xml.dll

System.Xml.dll: XPath/Private/XPathNodes.cs
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/Xml.build" all

TREECC_INPUTS = $(srcdir)/XPath/Private/XPathNodes.tc \
				$(srcdir)/XPath/Private/XPathEvaluate.tc\
				$(srcdir)/XPath/Private/XPathCompile.tc

XPath/Private/XPathNodes.cs: $(TREECC_INPUTS)
	$(TREECC) -f -o XPath/Private/XPathNodes.cs $(srcdir)/XPath/Private/XPathNodes.tc

CLEANFILES = System.Xml.dll XPath/Private/XPathNodes.cs

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/Xml.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-f "$(srcdir)/Xml.build" -u all

