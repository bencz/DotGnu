#!/bin/sh
#
# This script is an example of how to use "csdocvalil" to output an
# XML file that describes the current implementation status of pnetlib,
# with respect to the "All.xml" file from ECMA.
#
exec csdocvalil -fxml -fignore-assembly-names -fextra-members-ok -fextra-types-ok -fimage=mscorlib.dll -fimage=System.dll -fimage=System.Xml.dll All.xml
