libILincludedir = $(includedir)/pnet
libILinclude_HEADERS = il_align.h \
					   il_bignum.h \
					   il_coder.h \
					   il_config.h \
					   il_console.h \
					   il_crypt.h \
					   il_debug.h \
					   il_debugger.h \
					   il_decimal.h \
					   il_dumpasm.h \
					   il_engine.h \
					   il_errno.h \
					   il_gc.h \
					   il_image.h \
					   il_jopcodes.h \
					   il_linker.h \
					   il_meta.h \
					   il_opcodes.h \
					   il_profile.h \
					   il_program.h \
					   il_regex.h \
					   il_serial.h \
					   il_serialize.h \
					   il_sysio.h \
					   il_system.h \
					   il_thread.h \
					   il_types.h \
					   il_utils.h \
					   il_values.h \
					   il_varargs.h \
					   il_writer.h \
					   il_xml.h

il_profile.h: $(top_srcdir)/profiles/$(PROFILE_NAME)
	$(SHELL) $(top_srcdir)/profiles/mkprofile.sh $< $@

CLEANFILES = il_profile.h
