
## The skeleton files are wrapped up into the treecc binary using
## the "mkskel-sh" script.  They aren't installed any more.

noinst_DATA = cpp_skel.cc cpp_skel.h c_skel.c c_skel.h \
			  cpp_gc_skel.cc cpp_gc_skel.h c_gc_skel.c c_gc_skel.h

## but we need to put them into the distribution

EXTRA_DIST = $(noinst_DATA)

MAINTAINERCLEANFILES = Makefile.in
