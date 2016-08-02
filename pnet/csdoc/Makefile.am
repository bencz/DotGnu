bin_PROGRAMS       = csdoc2texi csdoc2html csdoc2hier cssrc2html
noinst_PROGRAMS    = csdocvalil csdoc2stub csdoc2test src2xml il2doc
man_MANS           = csdoc2texi.1 csdoc2html.1 csdoc2hier.1 cssrc2html.1
EXTRA_DIST         = $(man_MANS)

CSDOC_COMMON       = doc_destroy.c \
					 doc_load.c \
					 doc_main.c \
					 doc_sort.c \
					 doc_tree.h \
					 doc_backend.h

CSDOC_LIBS         = ../dumpasm/libILDumpAsm.a ../image/libILImage.a \
					 ../support/libILSupport.a $(GCLIBS)

csdoc2texi_SOURCES = doc_texi.c $(CSDOC_COMMON)
csdoc2texi_LDADD   = $(CSDOC_LIBS)

csdoc2html_SOURCES = doc_html.c $(CSDOC_COMMON)
csdoc2html_LDADD   = $(CSDOC_LIBS)

csdoc2hier_SOURCES = doc_hier.c $(CSDOC_COMMON)
csdoc2hier_LDADD   = $(CSDOC_LIBS)

csdocvalil_SOURCES = doc_valil.c $(CSDOC_COMMON)
csdocvalil_LDADD   = $(CSDOC_LIBS)

csdoc2stub_SOURCES = doc_stub.c $(CSDOC_COMMON)
csdoc2stub_LDADD   = $(CSDOC_LIBS)

csdoc2test_SOURCES = doc_test.c $(CSDOC_COMMON)
csdoc2test_LDADD   = $(CSDOC_LIBS)

cssrc2html_SOURCES = cssrc2html.c fontcolors.c scanner.c cssrc2html.h fontcolors.h tokens.h scanner.h

il2doc_SOURCES     = il2doc.c
il2doc_LDADD       = $(CSDOC_LIBS)

src2xml_SOURCES    = src2xml.c

AM_CFLAGS = -I$(top_srcdir)/include
