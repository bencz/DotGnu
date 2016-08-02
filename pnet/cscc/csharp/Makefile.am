
noinst_LIBRARIES = libILCSharp.a

TREECC_INPUTS = cs_cast.tc \
				cs_const.tc \
				cs_decls.tc \
			    cs_defs.tc \
				cs_lvalue.tc \
				cs_misc.tc \
				cs_invoke.tc \
				cs_oper.tc \
				cs_semantics.tc \
				cs_stmt.tc \
				cs_types.tc

TREECC_SRCOUT = cs_nodes.c cs_semantics.c

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

TREECC_OUTPUT = $(TREECC_SRCOUT) $(TREECC_UNUSED_SRCOUT) cs_defs.h

CS_LOOKUP_INPUTS = cs_lookup_member.c

EXTRA_DIST = $(TREECC_INPUTS) $(CS_LOOKUP_INPUTS)

libILCSharp_a_SOURCES  = cs_attrs.c \
						 cs_gather.c \
						 cs_grammar.y \
						 cs_internal.h \
						 cs_lookup.c \
						 cs_lookup_member.h \
						 cs_modifiers.c \
						 cs_rename.h \
						 cs_scanner.l \
						 cs_semvalue.c \
						 cs_semvalue.h \
						 cs_valdoc.c

nodist_libILCSharp_a_SOURCES = $(TREECC_SRCOUT)

AM_YFLAGS = -d

AM_CFLAGS = -I$(top_srcdir) -I$(top_builddir) -I$(top_srcdir)/include

STAMP = stamp-treecc

BUILT_SOURCES = $(STAMP) $(TREECC_OUTPUT)

libILCSharp_a_DEPENDENCIES = $(STAMP)

# Hack alert: if we build out of tree, copy sources to here,
# because treecc has no option to define the output directory

$(STAMP): $(TREECC_INPUTS) ../../codegen/$(STAMP)
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		cp -f $(srcdir)/*.tc .; \
	fi; \
	codegen_tc=`cd $(top_srcdir)/codegen && find . -name "*.tc"`; \
	for i in $$codegen_tc; do cp $(top_srcdir)/codegen/$$i .; done; \
	$(TREECC) cs_defs.tc && touch $@; \
	rm -f $$codegen_tc; \
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		rm -f *.tc; \
	fi

cs_scanner.l: cs_grammar.c

CLEANFILES = cs_grammar.c cs_scanner.c cs_grammar.h $(TREECC_OUTPUT) $(STAMP)

