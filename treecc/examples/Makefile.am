## expr_cpp removed because automake 1.6 on cygwin has problems
## processing C++ yacc rule files.

noinst_PROGRAMS = expr_c
## @EXPR_CPP@
## EXTRA_PROGRAMS  = expr_cpp

EXTRA_DIST = expr_c.tc gram_c.h

## expr_c_DEPENDENCIES = $(top_builddir)/treecc
BUILT_SOURCES = expr_c.c  expr_c.h gram_c.c gram_c.h

expr_c_SOURCES = gram_c.y scan_c.l
nodist_expr_c_SOURCES = expr_c.c expr_c.h

expr_c.c expr_c.h: $(srcdir)/expr_c.tc
	$(top_builddir)/treecc -o expr_c.c -h expr_c.h $(srcdir)/expr_c.tc

gram_c.c gram_c.h: gram_c.y

## expr_cpp_SOURCES = expr_cpp.cc gram_cpp.yy scan_cpp.ll
##
## expr_cpp.cc expr_cpp.h: expr_cpp.tc
## 	$(top_builddir)/treecc $(srcdir)/expr_cpp.tc

AM_YFLAGS = -d

CLEANFILES = expr_c.c expr_c.h expr_cpp.cc expr_cpp.h expr_cs.cs \
			 gram_c.c gram_c.h gram_cpp.cc gram_cpp.h \
			 scan_c.c scan_cpp.cc

gram_cpp.cc: gram_cpp.yy

MAINTAINERCLEANFILES = Makefile.in
