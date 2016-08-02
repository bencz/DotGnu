## Build the example programs.

noinst_DATA = evenodd.exe hello.exe phone.exe

EXTRA_DIST = evenodd.il hello.il phone.il

evenodd.exe: evenodd.il
	$(ILASM) -o evenodd.exe $(srcdir)/evenodd.il

hello.exe: hello.il
	$(ILASM) -o hello.exe $(srcdir)/hello.il

phone.exe: phone.il
	$(ILASM) -o phone.exe $(srcdir)/phone.il

CLEANFILES = $(noinst_DATA)
