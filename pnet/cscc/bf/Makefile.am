
noinst_LIBRARIES = libILBf.a

TREECC_INPUTS = bf_defs.tc \
				bf_codegen.tc

TREECC_SRCOUT = bf_nodes.c bf_codegen.c

TREECC_UNUSED_SRCOUT = \
	cg_lvalue.c \
	jv_assign.c \
	cg_logical.c \
	cg_ainit.c \
	cg_attrs.c \
	cg_relop.c \
	cg_decls.c \
	cg_arith.c \
	cg_cast.c \
	jv_arith.c \
	cg_const.c \
	cg_assign.c \
	cg_nodes.h \
	jv_cast.c \
	cg_nodes.c \
	jv_nodes.c \
	cg_stmt.c \
	cg_misc.c

TREECC_OUTPUT = $(TREECC_SRCOUT) $(TREECC_UNUSED_SRCOUT) bf_defs.h

EXTRA_DIST = $(TREECC_INPUTS)

libILBf_a_SOURCES  = bf_grammar.y \
					 bf_scanner.l \
					 bf_optimize.c \
					 bf_internal.h \
					 bf_rename.h

nodist_libILBf_a_SOURCES = \
	$(TREECC_SRCOUT)

AM_YFLAGS = -d

AM_CFLAGS = -I$(top_srcdir) -I$(top_builddir) -I$(top_srcdir)/include

STAMP = stamp-treecc

BUILT_SOURCES = $(STAMP) $(TREECC_OUTPUT)

libILBf_a_DEPENDENCIES = $(STAMP)

# Hack alert: if we build out of tree, copy sources to here,
# because treecc has no option to define the output directory

$(STAMP): $(TREECC_INPUTS) ../../codegen/$(STAMP)
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		cp -f $(srcdir)/*.tc .; \
	fi; \
	codegen_tc=`cd $(top_srcdir)/codegen && find . -name "*.tc"`; \
	for i in $$codegen_tc; do cp $(top_srcdir)/codegen/$$i .; done; \
	$(TREECC) bf_defs.tc && touch $@; \
	rm -f $$codegen_tc; \
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		rm -f *.tc; \
	fi

bf_scanner.l: bf_grammar.c

CLEANFILES = bf_grammar.c bf_scanner.c bf_grammar.h $(TREECC_OUTPUT) $(STAMP)


