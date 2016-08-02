
.PHONY: System.Xml.dll

all-local: Parser.cs PatternParser.jay PatternParser.cs PatternTokenizer.cs System.XmlMono.dll System.Xml.dll

System.Xml.dll:
	 ilalink  -l mscorlib.dll -l System.dll -o System.Xml.dll System.XmlMono.dll

System.XmlMono.dll:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-b "$(srcdir)" -B . -f System.XmlMono.build all

Parser.cs: $(MONO_SOURCE_PATH)/class/System.XML/System.Xml.XPath/Parser.jay $(top_srcdir)/jay/skeleton.cs
	$(top_builddir)/jay/jay -ct < $(top_srcdir)/jay/skeleton.cs $< >$@

PatternParser.jay: $(MONO_SOURCE_PATH)/class/System.XML/System.Xml.XPath/Parser.jay $(top_srcdir)/jay/skeleton.cs
	sed "s/\%start Expr/\%start Pattern/" $< >$@

PatternParser.cs: PatternParser.jay $(top_srcdir)/jay/skeleton.cs
	echo "#define XSLT_PATTERN" > $@
	$(top_builddir)/jay/jay -ct $< < $(top_srcdir)/jay/skeleton.cs >>$@

PatternTokenizer.cs: $(MONO_SOURCE_PATH)/class/System.XML/System.Xml.XPath/Tokenizer.cs
	echo "#define XSLT_PATTERN" > $@
	cat $< >>$@

CLEANFILES = System.Xml.dll

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/System.XmlMono.build" -i all

uninstall-local:
	@$(NORMALL_UNINSTALL)
	"$(CSANT)" $(CSANT_FLAGS) -b "$(srcdir)" -B . \
			-Dmono.sources="$(MONO_SOURCE_PATH)" \
			-f "$(srcdir)/System.XmlMono.build" -u all
