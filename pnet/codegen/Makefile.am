lib_LIBRARIES = libILCodeGen.a

TREECC_INPUTS = cg_ainit.tc \
				cg_all.tc \
				cg_arith.tc \
				cg_assign.tc \
				cg_attrs.tc \
				cg_cast.tc \
				cg_const.tc \
				cg_decls.tc \
				cg_logical.tc \
				cg_lvalue.tc \
				cg_misc.tc \
				cg_nodes.tc \
				cg_relop.tc \
				cg_stmt.tc \
				jv_ainit.tc \
				jv_arith.tc \
				jv_assign.tc \
				jv_attrs.tc \
				jv_cast.tc \
				jv_const.tc \
				jv_decls.tc \
				jv_logical.tc \
				jv_lvalue.tc \
				jv_misc.tc \
				jv_nodes.tc \
				jv_relop.tc \
				jv_stmt.tc
##				pm_nodes.tc

TREECC_SRCOUT = cg_ainit.c \
				cg_arith.c \
				cg_assign.c \
				cg_attrs.c \
				cg_cast.c \
				cg_const.c \
				cg_decls.c \
				cg_logical.c \
				cg_lvalue.c \
				cg_misc.c \
				cg_nodes.c \
				cg_relop.c \
				cg_stmt.c \
				jv_arith.c \
				jv_assign.c \
				jv_cast.c \
				jv_nodes.c
##				pm_arith.c
##				pm_nodes.c

TREECC_OUTPUT = $(TREECC_SRCOUT) cg_nodes.h

EXTRA_DIST = $(TREECC_INPUTS)

libILCodeGen_a_SOURCES = cg_coerce.c \
						 cg_coerce.h \
						 cg_defs.h \
						 cg_errors.c \
						 cg_errors.h \
						 cg_gen.c \
						 cg_gen.h \
						 cg_genattr.c \
						 cg_genattr.h \
						 cg_interface.c \
						 cg_intl.h \
						 cg_library.c \
						 cg_nodemap.c \
						 cg_nodemap.h \
						 cg_optable.c \
						 cg_output.c \
						 cg_output.h \
						 cg_rbtree.c \
						 cg_rbtree.h \
						 cg_resolve.c \
						 cg_resolve.h \
						 cg_scope.c \
						 cg_scope.h \
						 cg_utils.c \
						 cg_utils.h \
						 cg_varusage.c \
						 cg_varusage.h \
						 jv_output.c \
						 jv_output.h

nodist_libILCodeGen_a_SOURCES = $(TREECC_SRCOUT)

AM_CFLAGS = -I$(top_srcdir) -I$(top_builddir) -I$(top_srcdir)/include

STAMP = stamp-treecc

BUILT_SOURCES = $(STAMP) $(TREECC_OUTPUT)

libILCodeGen_a_DEPENDENCIES = $(STAMP)

# Hack alert: if we build out of tree, copy sources to here,
# because treecc has no option to define the output directory

$(STAMP): $(TREECC_INPUTS)
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		cp $(srcdir)/*.tc .; \
	fi
	$(TREECC) cg_all.tc && touch $@
	if [ "`cd $(srcdir) && pwd`" != "`pwd`" ]; then \
		rm -f *.tc; \
	fi

CLEANFILES = $(TREECC_OUTPUT) $(STAMP)
