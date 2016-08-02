
noinst_LIBRARIES = libILJava.a

TREECC_INPUTS = java_defs.tc \
				java_semantics.tc \
				java_lvalue.tc \
				java_types.tc \
				java_stmt.tc \
				java_decls.tc \
				java_invoke.tc \
				java_const.tc \
				java_oper.tc \
				java_misc.tc \
				java_cast.tc 

TREECC_SRCOUT = java_nodes.c java_semantics.c

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

TREECC_OUTPUT = $(TREECC_SRCOUT) $(TREECC_UNUSED_SRCOUT) java_defs.h

EXTRA_DIST = $(TREECC_INPUTS)

libILJava_a_SOURCES = java_internal.h \
					  java_grammar.y \
					  java_scanner.l \
					  java_gather.c \
					  java_modifiers.c \
					  java_lookup.c \
					  java_rename.h \
					  java_semvalue.c \
					  java_semvalue.h

nodist_libILJava_a_SOURCES = $(TREECC_SRCOUT)

AM_YFLAGS = -d

AM_CFLAGS = -I$(top_srcdir) -I$(top_builddir) -I$(top_srcdir)/include

STAMP = stamp-treecc

BUILT_SOURCES = $(STAMP) $(TREECC_OUTPUT)

libILJava_a_DEPENDENCIES = $(STAMP)

# Hack alert: if we build out of tree, copy sources to here,
# because treecc has no option to define the output directory

$(STAMP): $(TREECC_INPUTS) $(top_builddir)/codegen/$(STAMP)
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		cp -f $(srcdir)/*.tc .; \
	fi; \
	codegen_tc=`cd $(top_srcdir)/codegen && find . -name "*.tc"`; \
	for i in $$codegen_tc; do cp $(top_srcdir)/codegen/$$i .; done; \
	$(TREECC) java_defs.tc && touch $@; \
	rm -f $$codegen_tc; \
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		rm -f *.tc; \
	fi

java_scanner.l: java_grammar.c

CLEANFILES = java_grammar.c java_scanner.c java_grammar.h $(TREECC_OUTPUT) $(STAMP)
