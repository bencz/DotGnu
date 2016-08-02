SUBDIRS = . etc doc tests examples

bin_PROGRAMS = treecc
lib_LIBRARIES = libtreecc.a

treecc_SOURCES = main.c
treecc_LDADD = libtreecc.a

EXTRA_DIST = mkskel-sh

BUILT_SOURCES = skels.c

libtreecc_a_SOURCES = context.c \
					  errors.c \
					  errors.h \
					  gen.c \
					  gen.h \
					  gen_c.c \
					  gen_cpp.c \
					  gen_cs.c \
					  gen_ruby.c \
					  gen_java.c \
					  gen_php.c \
					  gen_python.c \
					  info.h \
					  input.c \
					  input.h \
					  literal.c \
					  node.c \
					  oper.c \
					  options.c \
					  options.h \
					  parse.c \
					  parse.h \
					  skels.c \
					  skeleton.c \
					  stream.c \
					  stream.h \
					  system.h

SKELETON_FILES = $(top_srcdir)/etc/c_skel.c \
				 $(top_srcdir)/etc/c_skel.h \
				 $(top_srcdir)/etc/cpp_skel.cc \
				 $(top_srcdir)/etc/cpp_skel.h \
				 $(top_srcdir)/etc/c_gc_skel.h \
				 $(top_srcdir)/etc/c_gc_skel.c \
				 $(top_srcdir)/etc/cpp_gc_skel.h \
				 $(top_srcdir)/etc/cpp_gc_skel.cc

skels.c: $(SKELETON_FILES) $(top_srcdir)/mkskel-sh
	$(SHELL) $(top_srcdir)/mkskel-sh $(SKELETON_FILES) >skels.c

CLEANFILES = skels.c

MAINTAINERCLEANFILES = Makefile.in \
					   config.guess \
					   config.sub \
					   configure \
					   install-sh \
					   missing
