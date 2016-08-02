OPT_SUBDIRS =

if USING_LIBFFI
OPT_SUBDIRS += libffi
endif

if USING_LIBGC
OPT_SUBDIRS += libgc
endif

if PNET_TOOLS

OPT_SUBDIRS += dumpasm \
			   ilasm \
			   ildasm \
			   ilalink \
			   ilsize \
			   engine \
			   ilnative \
			   ilheader \
			   ilfind \
			   ildd \
			   codegen \
			   cscc \
			   resgen \
			   ilstrip \
			   ilranlib \
			   ilgac \
			   csant \
			   clrwrap \
			   samples \
			   tests


if !CROSS_COMPILING
OPT_SUBDIRS += csdoc doc
endif

else

## We only want to build the runtime engine.
OPT_SUBDIRS += engine

endif

SUBDIRS = . \
		  include \
		  support \
		  image \
		  profiles \
		  $(OPT_SUBDIRS)

EXTRA_DIST = config-ml.in

MAINTAINERCLEANFILES = Makefile.in \
					   configure

