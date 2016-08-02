
SUBDIRS = common csharp c vb java bf cpp

noinst_PROGRAMS = cscc-cs cscc-c-s cscc-vb cscc-java cscc csdoc cscc-bf
man_MANS        = cscc.1 csdoc.1
EXTRA_DIST      = $(man_MANS)

cscc_SOURCES = cscc.c
cscc_LDADD   = common/libILCCommon.a ../ilasm/libILAsm.a \
			   ../ilalink/libILLink.a \
			   ../dumpasm/libILDumpAsm.a ../image/libILImage.a \
			   ../support/libILSupport.a $(GCLIBS)

cscc_cs_SOURCES = cs_main.c
cscc_cs_LDADD   = csharp/libILCSharp.a common/libILCCommon.a \
				  ../codegen/libILCodeGen.a ../dumpasm/libILDumpAsm.a \
				  ../image/libILImage.a ../support/libILSupport.a \
				  $(GCLIBS)	

cscc_c_s_SOURCES = c_main.c
cscc_c_s_LDADD   = c/libILCLang.a common/libILCCommon.a \
				   ../codegen/libILCodeGen.a ../dumpasm/libILDumpAsm.a \
				   ../image/libILImage.a ../support/libILSupport.a \
				   $(GCLIBS)

cscc_vb_SOURCES = vb_main.c
cscc_vb_LDADD   = vb/libILVB.a csharp/libILCSharp.a common/libILCCommon.a \
				  ../codegen/libILCodeGen.a ../dumpasm/libILDumpAsm.a \
				  ../image/libILImage.a ../support/libILSupport.a \
				  $(GCLIBS)

cscc_java_SOURCES = java_main.c
cscc_java_LDADD   = java/libILJava.a common/libILCCommon.a \
				  ../codegen/libILCodeGen.a ../dumpasm/libILDumpAsm.a \
				  ../image/libILImage.a ../support/libILSupport.a $(GCLIBS)

csdoc_SOURCES = csdoc.c
csdoc_LDADD   = csharp/libILCSharp.a common/libILCCommon.a \
				../codegen/libILCodeGen.a ../dumpasm/libILDumpAsm.a \
				../image/libILImage.a ../support/libILSupport.a $(GCLIBS)

cscc_bf_SOURCES = bf_main.c
cscc_bf_LDADD   = bf/libILBf.a common/libILCCommon.a \
				  ../codegen/libILCodeGen.a ../dumpasm/libILDumpAsm.a \
				  ../image/libILImage.a ../support/libILSupport.a $(GCLIBS)

AM_CFLAGS = -I$(top_srcdir) -I$(top_builddir) -I$(top_srcdir)/include

## The "cscc" compiler is symlinked as "cli-unknown-gcc" to fool
## autoconf-using cross-compiler systems into using "cscc" to compile
## C code into IL.  It isn't a real gcc implementation, of course.

install-data-local:
	@$(NORMALL_INSTALL)
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-gcc.1.gz
	$(LN_S) cscc.1.gz $(DESTDIR)$(man1dir)/cli-unknown-gcc.1.gz

install-exec-local:
	@$(NORMALL_INSTALL)
	$(mkinstalldirs) $(DESTDIR)$(libdir)/cscc/plugins
	$(mkinstalldirs) $(DESTDIR)$(bindir)
	$(LIBTOOL) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --mode=install $(INSTALL_PROGRAM) cscc-cs$(EXEEXT) $(DESTDIR)$(libdir)/cscc/plugins/cscc-cs$(EXEEXT)
	$(LIBTOOL) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --mode=install $(INSTALL_PROGRAM) cscc-c-s$(EXEEXT) $(DESTDIR)$(libdir)/cscc/plugins/cscc-c-s$(EXEEXT)
	$(LIBTOOL) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --mode=install $(INSTALL_PROGRAM) cscc-vb$(EXEEXT) $(DESTDIR)$(libdir)/cscc/plugins/cscc-vb$(EXEEXT)
	$(LIBTOOL) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --mode=install $(INSTALL_PROGRAM) cscc-java$(EXEEXT) $(DESTDIR)$(libdir)/cscc/plugins/cscc-java$(EXEEXT)
	$(LIBTOOL) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --mode=install $(INSTALL_PROGRAM) cscc-bf$(EXEEXT) $(DESTDIR)$(libdir)/cscc/plugins/cscc-bf$(EXEEXT)
	$(LIBTOOL) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --mode=install $(INSTALL_PROGRAM) cscc$(EXEEXT) $(DESTDIR)$(bindir)/cscc$(EXEEXT)
	$(LIBTOOL) $(AM_LIBTOOLFLAGS) $(LIBTOOLFLAGS) --mode=install $(INSTALL_PROGRAM) csdoc$(EXEEXT) $(DESTDIR)$(bindir)/csdoc$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-csharp$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-b$(EXEEXT)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-gcc$(EXEEXT)
	$(LN_S) cscc-cs$(EXEEXT) $(DESTDIR)$(libdir)/cscc/plugins/cscc-csharp$(EXEEXT)
	$(LN_S) cscc-bf$(EXEEXT) $(DESTDIR)$(libdir)/cscc/plugins/cscc-b$(EXEEXT)
	$(LN_S) cscc$(EXEEXT) $(DESTDIR)$(bindir)/cli-unknown-gcc$(EXEEXT)

uninstall-local:
	@$(NORMALL_UNINSTALL)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-csharp$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-cs$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-c-s$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-vb$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-java$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-bf$(EXEEXT)
	rm -f $(DESTDIR)$(libdir)/cscc/plugins/cscc-b$(EXEEXT)
	rm -f $(DESTDIR)$(bindir)/cscc$(EXEEXT)
	rm -f $(DESTDIR)$(bindir)/csdoc$(EXEEXT)
	rm -f $(DESTDIR)$(bindir)/cli-unknown-gcc$(EXEEXT)
	rm -f $(DESTDIR)$(man1dir)/cli-unknown-gcc.1.gz
