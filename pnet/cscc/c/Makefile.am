
noinst_LIBRARIES = libILCLang.a

TREECC_INPUTS = c_ainit.tc \
				c_builtin.tc \
				c_defs.tc \
				c_const.tc \
				c_invoke.tc \
				c_lvalue.tc \
				c_name.tc \
				c_oper.tc \
				c_stmt.tc \
				c_stubs.tc

TREECC_SRCOUT = c_nodes.c c_semantics.c c_name.c

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

TREECC_OUTPUT = $(TREECC_SRCOUT) $(TREECC_UNUSED_SRCOUT) c_defs.h

EXTRA_DIST = $(TREECC_INPUTS)

libILCLang_a_SOURCES = c_coerce.c \
					   c_crt0.c \
					   c_declspec.c \
					   c_declspec.h \
					   c_function.c \
					   c_function.h \
					   c_grammar.y \
					   c_internal.c \
					   c_internal.h \
					   c_lexutils.c \
					   c_lexutils.h \
					   c_library.c \
					   c_rename.h \
					   c_scanner.l \
					   c_scope.c \
					   c_scope.h \
					   c_typeout.c \
					   c_types.c \
					   c_types.h

nodist_libILCLang_a_SOURCES = $(TREECC_SRCOUT)

AM_YFLAGS = -d

AM_CFLAGS = -I$(top_srcdir) -I$(top_builddir) -I$(top_srcdir)/include

STAMP = stamp-treecc

BUILT_SOURCES = $(STAMP) $(TREECC_OUTPUT)

libILCLang_a_DEPENDENCIES = $(STAMP)

# Hack alert: if we build out of tree, copy sources to here,
# because treecc has no option to define the output directory

$(STAMP): $(TREECC_INPUTS) $(top_builddir)/codegen/$(STAMP)
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		cp -f $(srcdir)/*.tc .; \
	fi; \
	codegen_tc=`cd $(top_srcdir)/codegen && find . -name "*.tc"`; \
	for i in $$codegen_tc; do cp $(top_srcdir)/codegen/$$i .; done; \
	$(TREECC) c_defs.tc && touch $@; \
	rm -f $$codegen_tc; \
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		rm -f *.tc; \
	fi

c_scanner.l: c_grammar.c

CLEANFILES = c_grammar.c c_scanner.c c_grammar.h $(TREECC_OUTPUT) $(STAMP)
