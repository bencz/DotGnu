
DISTCLEANFILES = run_test.sh pinvoke.map

install-data-local:
	@$(NORMAL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/lib/$(FRAMEWORK_VERSION)
	$(INSTALL_DATA) pinvoke.map $(DESTDIR)$(libdir)/cscc/lib/$(FRAMEWORK_VERSION)/pinvoke.map
	if test "x$(INSTALL_AS_DEFAULT)" = "xtrue" ; then \
		rm -f $(DESTDIR)$(libdir)/cscc/lib/pinvoke.map ; \
		$(LN_S) $(DESTDIR)$(libdir)/cscc/lib/$(FRAMEWORK_VERSION)/pinvoke.map $(DESTDIR)$(libdir)/cscc/lib/pinvoke.map ; \
	fi

uninstall-local:
	@$(NORMALL_UNINSTALL)
	rm -f $(DESTDIR)$(libdir)/cscc/lib/$(FRAMEWORK_VERSION)/pinvoke.map
	if test "x$(INSTALL_AS_DEFAULT)" = "xtrue" ; then \
		rm -f $(DESTDIR)$(libdir)/cscc/lib/pinvoke.map ; \
	fi
