if HAS_REFLECTION
if HAS_EXTENDED_NUMERICS
SUBDIRS = tools config resources runtime I18N OpenSystem.Platform \
		  DotGNU.SSL System System.Xml . \
		  System.Net.IrDA csunit csupport compat ISymWrapper \
		  SharpZipLib System.Configuration.Install JScript \
		  DotGNU.Images Xsharp System.Drawing System.Drawing.Xsharp \
		  System.Drawing.Win32 System.Drawing.Postscript \
		  System.EnterpriseServices System.Windows.Forms \
		  System.Design Basic System.Deployment DotGNU.Terminal \
		  DotGNU.XmlRpc Themes tests samples DotGNU.Misc
else
SUBDIRS = tools config resources runtime I18N OpenSystem.Platform \
		  DotGNU.SSL System System.Xml . \
		  System.Net.IrDA csunit csupport compat ISymWrapper \
		  SharpZipLib System.Configuration.Install \
		  DotGNU.Images Xsharp System.EnterpriseServices \
		  Basic System.Deployment DotGNU.Terminal DotGNU.XmlRpc \
		  tests samples DotGNU.Misc
endif
else
SUBDIRS = tools resources runtime OpenSystem.Platform \
		  DotGNU.SSL System System.Xml . System.Net.IrDA samples
endif

## System.dll is built in two phases, because it depends recursively
## on System.Xml.dll.  The following takes care of the second phase.

all-local:
	(cd System; $(MAKE) $(AM_MAKEFLAGS) phase-two)
