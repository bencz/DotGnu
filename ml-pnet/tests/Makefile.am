
all-local:
	"$(CSANT)" $(CSANT_FLAGS) -Dmono.sources="$(MONO_SOURCE_PATH)" \
			-Dpnet.libdir="$(PNETLIB_PATH)" \
			-b "$(srcdir)" -B . -f "$(srcdir)/Test.build" all

CLEANFILES = Testcorlib.System.dll Testcorlib.System.Collections.dll \
		Testcorlib.System.Diagnostics.dll Testcorlib.System.Globalization.dll \
		Testcorlob.System.IO.dll Testcorlib.System.Reflection.dll \
		Testcorlib.System.Reflection.Emit.dll Testcorlib.System.Resources.dll \
		Testcorlib.System.Runtime.CompilerServices.dll \
		Testcorlib.System.Runtime.InteropServices.dll \
		Testcorlib.System.Runtime.Remoting.dll \
		Testcorlib.System.Runtime.Serialization.dll \
		Testcorlib.System.Security.dll Testcorlib.System.Security.Cryptography.dll \
		Testcorlib.System.Security.Permissions.dll \
		Testcorlib.System.Security.Policy.dll \
		Testcorlib.System.Security.Principal.dll Testcorlib.System.Text.dll \
		Testcorlib.System.Threading.dll TestSystem.dll TestSystem.Xml.dll \
		TestSystem.Runtime.Serialization.Formatters.Soap.dll \
		TestSystem.Security.dll TestMono.Security.dll TestSystem.Data.dll \
		TestSystem.Web.Services.dll TestSystem.Runtime.Remoting.dll

TESTS = Testcorlib.System.dll Testcorlib.System.Collections.dll \
		Testcorlib.System.Diagnostics.dll Testcorlib.System.Globalization.dll \
		Testcorlib.System.IO.dll Testcorlib.System.Reflection.dll \
		Testcorlib.System.Reflection.Emit.dll Testcorlib.System.Resources.dll \
		Testcorlib.System.Runtime.CompilerServices.dll \
		Testcorlib.System.Runtime.InteropServices.dll \
		Testcorlib.System.Runtime.Remoting.dll \
		Testcorlib.System.Runtime.Serialization.dll \
		Testcorlib.System.Security.dll Testcorlib.System.Security.Cryptography.dll \
		Testcorlib.System.Security.Permissions.dll \
		Testcorlib.System.Security.Policy.dll \
		Testcorlib.System.Security.Principal.dll Testcorlib.System.Text.dll \
		Testcorlib.System.Threading.dll TestSystem.dll TestSystem.Xml.dll \
		TestSystem.Runtime.Serialization.Formatters.Soap.dll \
		TestSystem.Security.dll TestMono.Security.dll TestSystem.Data.dll \
		TestSystem.Web.Services.dll TestSystem.Runtime.Remoting.dll

TESTS_ENVIRONMENT = $(SHELL) $(top_builddir)/tools/run_test.sh $(top_builddir)
