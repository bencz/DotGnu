lib_LIBRARIES = libILImage.a
noinst_LIBRARIES = libILTestMacros.a

OPCODES_INCLUDES = opdef.c

EXTRA_libILImage_a_SOURCES = $(OPCODES_INCLUDES)

libILImage_a_SOURCES = assembly.c \
					   attr.c \
					   class.c \
					   compact.c \
					   compress.c \
					   context.c \
					   create.c \
					   debug_reader.c \
					   debug_writer.c \
					   errors.c \
					   generic.c \
					   generic_member.c \
					   generic_class.c \
					   image.c \
					   image.h \
					   item.c \
					   java_loader.c \
					   java_writer.c \
					   jopcodes.c \
					   lib_attrs.c \
					   link.c \
					   marshal.c \
					   member.c \
					   meta_build.c \
					   meta_header.c \
					   meta_index.c \
					   meta_types.c \
					   meta_writer.c \
					   misc_token.c \
					   module.c \
					   opcodes.c \
					   opcodes_small.c \
					   pecoff_loader.c \
					   pecoff_writer.c \
					   program.h \
					   ser_parse.c \
					   ser_writer.c \
					   sig_parse.c \
					   sig_writer.c \
					   synthetic.c \
					   uncompress.c \
					   winres_reader.c \
					   winres_writer.c \
					   writer.c

libILTestMacros_a_SOURCES = test_macros.c

AM_CFLAGS = -DCSCC_HOST_TRIPLET=\"$(host_triplet)\" \
		    -DCSCC_HOST_ALIAS=\"$(host_alias)\" \
		    -I$(top_srcdir)/include
