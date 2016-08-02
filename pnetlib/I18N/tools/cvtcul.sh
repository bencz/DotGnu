#!/bin/sh
#
# This script can be used to regenerate the CIDnnnn.cs files
# from the ".txt" files.  The list of unsupported locales
# is at the end of this file.
#
# Copyright (C) 2003  Southern Storm Software, Pty Ltd.
#
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

# Make sure that we are in the correct directory to start.
if test -d ../Rare ; then
	cd ..
fi

# Compile the "loc2cul" tool.
if gcc -o tools/loc2cul tools/loc2cul.c ; then
	:
else
	exit 1
fi
LOC2CUL=tools/loc2cul

${LOC2CUL} --region Common --root --name root \
	Common/root.txt >Common/RootCulture.cs

# Afrikaans

${LOC2CUL} --region Other --id 0x0036 --name af \
	--iso2 af --iso3 afr --windows AFK \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ',' \
	Other/af.txt >Other/CID0036.cs

${LOC2CUL} --region Other --id 0x0436 --name af-ZA \
	Other/af_ZA.txt >Other/CID0436.cs

# Arabic

${LOC2CUL} --region MidEast --id 0x0001 --name ar \
	--iso2 ar --iso3 ara --windows ARA \
	--ansi 1256 --ebcdic 20420 --mac 10004 --oem 720 --separator ';' \
	MidEast/ar.txt >MidEast/CID0001.cs

${LOC2CUL} --region MidEast --id 0x3801 --name ar-AE \
	--windows ARU \
	MidEast/ar_AE.txt >MidEast/CID3801.cs

${LOC2CUL} --region MidEast --id 0x3C01 --name ar-BH \
	--windows ARH \
	MidEast/ar_BH.txt >MidEast/CID3c01.cs

${LOC2CUL} --region MidEast --id 0x1401 --name ar-DZ \
	--windows ARG \
	MidEast/ar_DZ.txt >MidEast/CID1401.cs

${LOC2CUL} --region MidEast --id 0x0C01 --name ar-EG \
	--windows ARE \
	MidEast/ar_EG.txt >MidEast/CID0c01.cs

${LOC2CUL} --region MidEast --id 0x0801 --name ar-IQ \
	--windows ARI \
	MidEast/ar_IQ.txt >MidEast/CID0801.cs

${LOC2CUL} --region MidEast --id 0x2C01 --name ar-JO \
	--windows ARJ \
	MidEast/ar_JO.txt >MidEast/CID2c01.cs

${LOC2CUL} --region MidEast --id 0x3401 --name ar-KW \
	--windows ARK \
	MidEast/ar_KW.txt >MidEast/CID3401.cs

${LOC2CUL} --region MidEast --id 0x3001 --name ar-LB \
	--windows ARB \
	MidEast/ar_LB.txt >MidEast/CID3001.cs

${LOC2CUL} --region MidEast --id 0x1001 --name ar-LY \
	--windows ARL \
	MidEast/ar_LY.txt >MidEast/CID1001.cs

${LOC2CUL} --region MidEast --id 0x1801 --name ar-MA \
	--windows ARM \
	MidEast/ar_MA.txt >MidEast/CID1801.cs

${LOC2CUL} --region MidEast --id 0x2001 --name ar-OM \
	--windows ARO \
	MidEast/ar_OM.txt >MidEast/CID2001.cs

${LOC2CUL} --region MidEast --id 0x4001 --name ar-QA \
	--windows ARQ \
	MidEast/ar_QA.txt >MidEast/CID4001.cs

${LOC2CUL} --region MidEast --id 0x0401 --name ar-SA \
	MidEast/ar_SA.txt >MidEast/CID0401.cs

${LOC2CUL} --region MidEast --id 0x2801 --name ar-SY \
	--windows ARS \
	MidEast/ar_SY.txt >MidEast/CID2801.cs

${LOC2CUL} --region MidEast --id 0x1C01 --name ar-TN \
	--windows ART \
	MidEast/ar_TN.txt >MidEast/CID1c01.cs

${LOC2CUL} --region MidEast --id 0x3801 --name ar-YE \
	--windows ARY \
	MidEast/ar_YE.txt >MidEast/CID3801.cs

# Belarusian

${LOC2CUL} --region Other --id 0x0023 --name be \
	--iso2 be --iso3 bel --windows BEL \
	--ansi 1251 --ebcdic 500 --mac 10007 --oem 866 --separator ';' \
	Other/be.txt >Other/CID0023.cs

${LOC2CUL} --region Other --id 0x0423 --name be-BY \
	Other/be_BY.txt >Other/CID0423.cs

# Bulgarian

${LOC2CUL} --region West --id 0x0002 --name bg \
	--iso2 bg --iso3 bul --windows BGR \
	--ansi 1251 --ebcdic 20420 --mac 10007 --oem 866 --separator ';' \
	West/bg.txt >West/CID0002.cs

${LOC2CUL} --region West --id 0x0402 --name bg-BG \
	West/bg_BG.txt >West/CID0402.cs

# Catalan

${LOC2CUL} --region West --id 0x0003 --name ca \
	--iso2 ca --iso3 cat --windows CAT \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ';' \
	West/ca.txt >West/CID0003.cs

${LOC2CUL} --region West --id 0x0403 --name ca-ES \
	West/ca_ES.txt >West/CID0403.cs

# Czech

${LOC2CUL} --region West --id 0x0005 --name cs \
	--iso2 cs --iso3 ces --windows CSY \
	--ansi 1250 --ebcdic 500 --mac 10029 --oem 852 --separator ';' \
	West/cs.txt >West/CID0005.cs

${LOC2CUL} --region West --id 0x0405 --name cs-CZ \
	West/cs_CZ.txt >West/CID0405.cs

# Danish

${LOC2CUL} --region West --id 0x0006 --name da \
	--iso2 da --iso3 dan --windows DAN \
	--ansi 1252 --ebcdic 20277 --mac 10000 --oem 850 --separator ';' \
	West/da.txt >West/CID0006.cs

${LOC2CUL} --region West --id 0x0406 --name da-DK \
	West/da_DK.txt >West/CID0406.cs

# German

${LOC2CUL} --region West --id 0x0007 --name de \
	--iso2 de --iso3 deu --windows DEU \
	--ansi 1252 --ebcdic 20273 --mac 10000 --oem 850 --separator ';' \
	West/de.txt >West/CID0007.cs

${LOC2CUL} --region West --id 0x0C07 --name de-AT \
	--windows DEA \
	West/de_AT.txt >West/CID0c07.cs

${LOC2CUL} --region West --id 0x0807 --name de-CH \
	--windows DES \
	West/de_CH.txt >West/CID0807.cs

${LOC2CUL} --region West --id 0x0407 --name de-DE \
	West/de_DE.txt >West/CID0407.cs

${LOC2CUL} --region West --id 0x1007 --name de-LU \
	--windows DEL \
	West/de_LU.txt >West/CID1007.cs

# Greek

${LOC2CUL} --region West --id 0x0008 --name el \
	--iso2 el --iso3 ell --windows ELL \
	--ansi 1253 --ebcdic 20273 --mac 10006 --oem 737 --separator ';' \
	West/el.txt >West/CID0008.cs

${LOC2CUL} --region West --id 0x0408 --name el-GR \
	West/el_GR.txt >West/CID0408.cs

# English

${LOC2CUL} --region Common --id 0x0009 --name en \
	--iso2 en --iso3 eng --windows ENU \
	--ansi 1252 --ebcdic 37 --mac 10000 --oem 437 --separator ',' \
	Common/en.txt >Common/CID0009.cs

${LOC2CUL} --region Common --id 0x0C09 --name en-AU \
	--windows ENA \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ',' \
	Common/en_AU.txt >Common/CID0c09.cs

${LOC2CUL} --region Common --id 0x1009 --name en-CA \
	--windows ENC \
	--ansi 1252 --ebcdic 37 --mac 10000 --oem 850 --separator ',' \
	Common/en_CA.txt >Common/CID1009.cs

${LOC2CUL} --region Common --id 0x0809 --name en-GB \
	--windows ENG \
	--ansi 1252 --ebcdic 20285 --mac 10000 --oem 850 --separator ',' \
	Common/en_GB.txt >Common/CID0809.cs

${LOC2CUL} --region Common --id 0x1809 --name en-IE \
	--windows ENI \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ',' \
	Common/en_IE.txt >Common/CID1809.cs

${LOC2CUL} --region Common --id 0x1409 --name en-NZ \
	--windows ENZ \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ',' \
	Common/en_NZ.txt >Common/CID1409.cs

${LOC2CUL} --region Common --id 0x3409 --name en-PH \
	--windows ENP \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 437 --separator ',' \
	Common/en_PH.txt >Common/CID3409.cs

${LOC2CUL} --region Common --id 0x0409 --name en-US \
	Common/en_US.txt >Common/CID0409.cs

${LOC2CUL} --region Common --id 0x1C09 --name en-ZA \
	--windows ENS \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 437 --separator ',' \
	Common/en_ZA.txt >Common/CID1c09.cs

${LOC2CUL} --region Common --id 0x3009 --name en-ZW \
	--windows ENW \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 437 --separator ',' \
	Common/en_ZW.txt >Common/CID3009.cs

# Spanish

${LOC2CUL} --region West --id 0x000A --name es \
	--iso2 es --iso3 spa --windows ESP \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ';' \
	West/es.txt >West/CID000a.cs

${LOC2CUL} --region West --id 0x2C0A --name es-AR \
	--windows ESS \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_AR.txt >West/CID2c0a.cs

${LOC2CUL} --region West --id 0x400A --name es-BO \
	--windows ESB \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_BO.txt >West/CID400a.cs

${LOC2CUL} --region West --id 0x340A --name es-CL \
	--windows ESL \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_CL.txt >West/CID340a.cs

${LOC2CUL} --region West --id 0x240A --name es-CO \
	--windows ESO \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_CO.txt >West/CID240a.cs

${LOC2CUL} --region West --id 0x140A --name es-CR \
	--windows ESC \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_CR.txt >West/CID140a.cs

${LOC2CUL} --region West --id 0x1C0A --name es-DO \
	--windows ESD \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_DO.txt >West/CID1c0a.cs

${LOC2CUL} --region West --id 0x300A --name es-EC \
	--windows ESF \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_EC.txt >West/CID300a.cs

${LOC2CUL} --region West --id 0x0C0A --name es-ES \
	--windows ESN \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ';' \
	West/es_ES.txt >West/CID0c0a.cs

${LOC2CUL} --region West --id 0x100A --name es-GT \
	--windows ESG \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_GT.txt >West/CID100a.cs

${LOC2CUL} --region West --id 0x480A --name es-HN \
	--windows ESH \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_HN.txt >West/CID480a.cs

${LOC2CUL} --region West --id 0x080A --name es-MX \
	--windows ESM \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_MX.txt >West/CID080a.cs

${LOC2CUL} --region West --id 0x4C0A --name es-NI \
	--windows ESI \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_NI.txt >West/CID4c0a.cs

${LOC2CUL} --region West --id 0x180A --name es-PA \
	--windows ESA \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_PA.txt >West/CID180a.cs

${LOC2CUL} --region West --id 0x280A --name es-PE \
	--windows ESR \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_PE.txt >West/CID280a.cs

${LOC2CUL} --region West --id 0x500A --name es-PR \
	--windows ESU \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_PR.txt >West/CID500a.cs

${LOC2CUL} --region West --id 0x3C0A --name es-PY \
	--windows ESZ \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_PY.txt >West/CID3c0a.cs

${LOC2CUL} --region West --id 0x440A --name es-SV \
	--windows ESE \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_SV.txt >West/CID440a.cs

${LOC2CUL} --region West --id 0x380A --name es-UY \
	--windows ESY \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_UY.txt >West/CID380a.cs

${LOC2CUL} --region West --id 0x200A --name es-VE \
	--windows ESV \
	--ansi 1252 --ebcdic 20284 --mac 10000 --oem 850 --separator ',' \
	West/es_VE.txt >West/CID200a.cs

# Estonian

${LOC2CUL} --region Other --id 0x0025 --name et \
	--iso2 et --iso3 est --windows ETI \
	--ansi 1257 --ebcdic 500 --mac 10029 --oem 775 --separator ';' \
	Other/et.txt >Other/CID0025.cs

${LOC2CUL} --region Other --id 0x0425 --name et-EE \
	Other/et_EE.txt >Other/CID0425.cs

# Basque

${LOC2CUL} --region West --id 0x002D --name eu \
	--iso2 eu --iso3 eus --windows EUQ \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ';' \
	West/eu.txt >West/CID002D.cs

${LOC2CUL} --region West --id 0x042D --name eu-ES \
	West/eu_ES.txt >West/CID042D.cs

# Farsi

${LOC2CUL} --region MidEast --id 0x0029 --name fa \
	--iso2 fa --iso3 fas --windows FAR \
	--ansi 1256 --ebcdic 20420 --mac 10004 --oem 720 --separator ';' \
	MidEast/fa.txt >MidEast/CID0029.cs

${LOC2CUL} --region MidEast --id 0x0429 --name fa-IR \
	MidEast/fa_IR.txt >MidEast/CID0429.cs

# Finnish

${LOC2CUL} --region West --id 0x000B --name fi \
	--iso2 fi --iso3 fin --windows FIN \
	--ansi 1252 --ebcdic 20278 --mac 10000 --oem 850 --separator ';' \
	West/fi.txt >West/CID000b.cs

${LOC2CUL} --region West --id 0x040B --name fi-FI \
	West/fi_FI.txt >West/CID040b.cs

# Faeroese

${LOC2CUL} --region Other --id 0x0038 --name fo \
	--iso2 fo --iso3 fao --windows FOS \
	--ansi 1252 --ebcdic 20277 --mac 10079 --oem 850 --separator ';' \
	Other/fo.txt >Other/CID0038.cs

${LOC2CUL} --region Other --id 0x0438 --name fo-FO \
	Other/fo_FO.txt >Other/CID0438.cs

# French

${LOC2CUL} --region West --id 0x000C --name fr \
	--iso2 fr --iso3 fra --windows FRA \
	--ansi 1252 --ebcdic 20297 --mac 10000 --oem 850 --separator ';' \
	West/fr.txt >West/CID000c.cs

${LOC2CUL} --region West --id 0x080C --name fr-BE \
	--windows FRB \
	West/fr_BE.txt >West/CID080c.cs

${LOC2CUL} --region West --id 0x0C0C --name fr-CA \
	--windows FRC \
	West/fr_CA.txt >West/CID0c0c.cs

${LOC2CUL} --region West --id 0x100C --name fr-CH \
	--windows FRS \
	West/fr_CH.txt >West/CID100c.cs

${LOC2CUL} --region West --id 0x040C --name fr-FR \
	West/fr_FR.txt >West/CID040c.cs

${LOC2CUL} --region West --id 0x140C --name fr-LU \
	--windows FRL \
	West/fr_LU.txt >West/CID140c.cs

# Galician

${LOC2CUL} --region West --id 0x0056 --name gl \
	--iso2 gl --iso3 glg --windows GLC \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ',' \
	West/gl.txt >West/CID0056.cs

${LOC2CUL} --region West --id 0x0456 --name gl-ES \
	West/gl_ES.txt >West/CID0456.cs

# Gujarati

${LOC2CUL} --region Other --id 0x0047 --name gu \
	--iso2 gu --iso3 guj --windows GUJ \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/gu.txt >Other/CID0047.cs

${LOC2CUL} --region Other --id 0x0447 --name gu-IN \
	Other/gu_IN.txt >Other/CID0447.cs

# Hebrew

${LOC2CUL} --region MidEast --id 0x000D --name he \
	--iso2 he --iso3 heb --windows HEB \
	--ansi 1255 --ebcdic 500 --mac 10005 --oem 862 --separator ',' \
	MidEast/he.txt >MidEast/CID000d.cs

${LOC2CUL} --region MidEast --id 0x040D --name he-IL \
	MidEast/he_IL.txt >MidEast/CID040d.cs

# Hindi

${LOC2CUL} --region Other --id 0x0039 --name hi \
	--iso2 hi --iso3 hin --windows HIN \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/hi.txt >Other/CID0039.cs

${LOC2CUL} --region Other --id 0x0439 --name hi-IN \
	Other/hi_IN.txt >Other/CID0439.cs

# Croatian

${LOC2CUL} --region Other --id 0x001A --name hr \
	--iso2 hr --iso3 hrv --windows HRV \
	--ansi 1250 --ebcdic 500 --mac 10082 --oem 852 --separator ';' \
	Other/hr.txt >Other/CID001a.cs

${LOC2CUL} --region Other --id 0x041A --name hr-HR \
	Other/hr_HR.txt >Other/CID041a.cs

# Hungarian

${LOC2CUL} --region West --id 0x000E --name hu \
	--iso2 hu --iso3 hun --windows HUN \
	--ansi 1250 --ebcdic 500 --mac 10029 --oem 852 --separator ';' \
	West/hu.txt >West/CID000e.cs

${LOC2CUL} --region West --id 0x040E --name hu-HU \
	West/hu_HU.txt >West/CID040e.cs

# Armenian

${LOC2CUL} --region Other --id 0x002B --name hy \
	--iso2 hy --iso3 hye --windows HYE \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/hy.txt >Other/CID002b.cs

${LOC2CUL} --region Other --id 0x042B --name hy-AM \
	Other/hy_AM.txt >Other/CID042b.cs

# Indonesian

${LOC2CUL} --region Other --id 0x0021 --name id \
	--iso2 id --iso3 ind --windows IND \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ';' \
	Other/id.txt >Other/CID0021.cs

${LOC2CUL} --region Other --id 0x0421 --name id-ID \
	Other/id_ID.txt >Other/CID0421.cs

# Icelandic

${LOC2CUL} --region West --id 0x000F --name is \
	--iso2 is --iso3 isl --windows ISL \
	--ansi 1252 --ebcdic 20871 --mac 10079 --oem 850 --separator ';' \
	West/is.txt >West/CID000f.cs

${LOC2CUL} --region West --id 0x040F --name is-IS \
	West/is_IS.txt >West/CID040f.cs

# Italian

${LOC2CUL} --region West --id 0x0010 --name it \
	--iso2 it --iso3 ita --windows ITA \
	--ansi 1252 --ebcdic 20280 --mac 10000 --oem 850 --separator ';' \
	West/it.txt >West/CID0010.cs

${LOC2CUL} --region West --id 0x0810 --name it-CH \
	--windows ITS \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ';' \
	West/it_CH.txt >West/CID0810.cs

${LOC2CUL} --region West --id 0x0410 --name it-IT \
	West/it_IT.txt >West/CID0410.cs

# Japanese

${LOC2CUL} --region CJK --id 0x0011 --name ja \
	--iso2 ja --iso3 jpn --windows JPN \
	--ansi 932 --ebcdic 20290 --mac 10001 --oem 932 --separator ',' \
	CJK/ja.txt >CJK/CID0011.cs

${LOC2CUL} --region CJK --id 0x0411 --name ja-JP \
	CJK/ja_JP.txt >CJK/CID0411.cs

# Kannada

${LOC2CUL} --region Other --id 0x004B --name kn \
	--iso2 kn --iso3 kan --windows KAN \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/kn.txt >Other/CID004b.cs

${LOC2CUL} --region Other --id 0x044B --name kn-IN \
	Other/kn_IN.txt >Other/CID044b.cs

# Konkani

${LOC2CUL} --region Other --id 0x0057 --name kok \
	--iso2 kok --iso3 kok --windows KNK \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/kok.txt >Other/CID0057.cs

${LOC2CUL} --region Other --id 0x0457 --name kok-IN \
	Other/kok_IN.txt >Other/CID0457.cs

# Korean

${LOC2CUL} --region CJK --id 0x0012 --name ko \
	--iso2 ko --iso3 kor --windows KOR \
	--ansi 949 --ebcdic 20833 --mac 10003 --oem 949 --separator ',' \
	CJK/ko.txt >CJK/CID0012.cs

${LOC2CUL} --region CJK --id 0x0412 --name ko-KR \
	CJK/ko_KR.txt >CJK/CID0412.cs

# Lithuanian

${LOC2CUL} --region Other --id 0x0027 --name lt \
	--iso2 lt --iso3 lit --windows LTH \
	--ansi 1257 --ebcdic 500 --mac 10029 --oem 775 --separator ';' \
	Other/lt.txt >Other/CID0027.cs

${LOC2CUL} --region Other --id 0x0427 --name lt-LT \
	Other/lt_LT.txt >Other/CID0427.cs

# Latvian

${LOC2CUL} --region Other --id 0x0026 --name lv \
	--iso2 lv --iso3 lav --windows LVI \
	--ansi 1257 --ebcdic 500 --mac 10029 --oem 775 --separator ';' \
	Other/lv.txt >Other/CID0026.cs

${LOC2CUL} --region Other --id 0x0426 --name lv-LV \
	Other/lv_LV.txt >Other/CID0426.cs

# FYRO Macedonian

${LOC2CUL} --region Other --id 0x002F --name mk \
	--iso2 mk --iso3 mkd --windows MKI \
	--ansi 1251 --ebcdic 500 --mac 10007 --oem 866 --separator ';' \
	Other/mk.txt >Other/CID002f.cs

${LOC2CUL} --region Other --id 0x042F --name mk-MK \
	Other/mk_MK.txt >Other/CID042f.cs

# Marathi

${LOC2CUL} --region Other --id 0x004e --name mr \
	--iso2 mr --iso3 mar --windows MAR \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/mr.txt >Other/CID004e.cs

${LOC2CUL} --region Other --id 0x044E --name mr-IN \
	Other/mr_IN.txt >Other/CID044e.cs

# Norwegian

${LOC2CUL} --region West --id 0x0014 --name no --alias no-NO \
	--iso2 no --iso3 nor --windows NOR \
	--ansi 1252 --ebcdic 20277 --mac 10000 --oem 850 --separator ';' \
	West/no.txt >West/CID0014.cs

${LOC2CUL} --region West --id 0x0414 --name nb-NO --alias nb \
	--iso2 nb --iso3 nob \
	West/nb_NO.txt >West/CID0414.cs

${LOC2CUL} --region West --id 0x0814 --name nn-NO --alias nn \
	--iso2 nn --iso3 nno --windows NON \
	West/nn_NO.txt >West/CID0814.cs

# Dutch

${LOC2CUL} --region West --id 0x0013 --name nl \
	--iso2 nl --iso3 nld --windows NLD \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ';' \
	West/nl.txt >West/CID0013.cs

${LOC2CUL} --region West --id 0x0813 --name nl-BE \
	--windows NLB \
	West/nl_BE.txt >West/CID0813.cs

${LOC2CUL} --region West --id 0x0413 --name nl-NL \
	West/nl_NL.txt >West/CID0413.cs

# Polish

${LOC2CUL} --region West --id 0x0015 --name pl \
	--iso2 pl --iso3 pol --windows PLK \
	--ansi 1250 --ebcdic 20880 --mac 10029 --oem 852 --separator ';' \
	West/pl.txt >West/CID0015.cs

${LOC2CUL} --region West --id 0x0415 --name pl-PL \
	West/pl_PL.txt >West/CID0415.cs

# Portuguese

${LOC2CUL} --region West --id 0x0016 --name pt \
	--iso2 pt --iso3 por --windows PTB \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 850 --separator ';' \
	West/pt.txt >West/CID0016.cs

${LOC2CUL} --region West --id 0x0416 --name pt-BR \
	West/pt_BR.txt >West/CID0416.cs

${LOC2CUL} --region West --id 0x0816 --name pt-PT \
	--windows PTG \
	West/pt_PT.txt >West/CID0816.cs

# Romanian

${LOC2CUL} --region West --id 0x0018 --name ro \
	--iso2 ro --iso3 ron --windows ROM \
	--ansi 1250 --ebcdic 20880 --mac 10029 --oem 852 --separator ';' \
	West/ro.txt >West/CID0018.cs

${LOC2CUL} --region West --id 0x0418 --name ro-RO \
	West/ro_RO.txt >West/CID0418.cs

# Russian

${LOC2CUL} --region Other --id 0x0019 --name ru \
	--iso2 ru --iso3 rus --windows RUS \
	--ansi 1251 --ebcdic 20880 --mac 10007 --oem 866 --separator ';' \
	Other/ru.txt >Other/CID0019.cs

${LOC2CUL} --region Other --id 0x0419 --name ru-RU \
	Other/ru_RU.txt >Other/CID0419.cs

# Slovak

${LOC2CUL} --region West --id 0x001B --name sk \
	--iso2 sk --iso3 slk --windows SKY \
	--ansi 1250 --ebcdic 20880 --mac 10029 --oem 852 --separator ';' \
	West/sk.txt >West/CID001b.cs

${LOC2CUL} --region West --id 0x041B --name sk-SK \
	West/sk_SK.txt >West/CID041b.cs

# Slovenian

${LOC2CUL} --region Other --id 0x0024 --name sl \
	--iso2 sl --iso3 slv --windows SLV \
	--ansi 1250 --ebcdic 20880 --mac 10029 --oem 852 --separator ';' \
	Other/sl.txt >Other/CID0024.cs

${LOC2CUL} --region Other --id 0x0424 --name sl-SI \
	Other/sl_SI.txt >Other/CID0424.cs

# Albanian

${LOC2CUL} --region Other --id 0x001C --name sq \
	--iso2 sq --iso3 sqi --windows SQI \
	--ansi 1250 --ebcdic 20880 --mac 10029 --oem 852 --separator ';' \
	Other/sq.txt >Other/CID001c.cs

${LOC2CUL} --region Other --id 0x041C --name sq-AL \
	Other/sq_AL.txt >Other/CID041c.cs

# Swedish

${LOC2CUL} --region West --id 0x001D --name sv \
	--iso2 sv --iso3 swe --windows SVE \
	--ansi 1252 --ebcdic 20278 --mac 10000 --oem 850 --separator ';' \
	West/sv.txt >West/CID001d.cs

${LOC2CUL} --region West --id 0x041D --name sv-SE \
	West/sv_SE.txt >West/CID041d.cs

${LOC2CUL} --region West --id 0x081D --name sv-FI \
	--windows SVF \
	West/sv_FI.txt >West/CID081d.cs

# Swahili

${LOC2CUL} --region Other --id 0x0041 --name sw \
	--iso2 sw --iso3 swa --windows SWK \
	--ansi 1252 --ebcdic 500 --mac 10000 --oem 437 --separator ',' \
	Other/sw.txt >Other/CID0041.cs

${LOC2CUL} --region Other --id 0x0441 --name sw-KE \
	Other/sw_KE.txt >Other/CID0441.cs

# Tamil

${LOC2CUL} --region Other --id 0x0049 --name ta \
	--iso2 ta --iso3 tam --windows TAM \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/ta.txt >Other/CID0049.cs

${LOC2CUL} --region Other --id 0x0449 --name ta-IN \
	Other/ta_IN.txt >Other/CID0449.cs

# Telugu

${LOC2CUL} --region Other --id 0x004A --name te \
	--iso2 te --iso3 tel --windows TEL \
	--ansi 0 --ebcdic 500 --mac 2 --oem 1 --separator ',' \
	Other/te.txt >Other/CID004a.cs

${LOC2CUL} --region Other --id 0x044A --name te-IN \
	Other/te_IN.txt >Other/CID044a.cs

# Thai

${LOC2CUL} --region Other --id 0x001E --name th \
	--iso2 th --iso3 tha --windows THA \
	--ansi 874 --ebcdic 20838 --mac 10021 --oem 874 --separator ',' \
	Other/th.txt >Other/CID001e.cs

${LOC2CUL} --region Other --id 0x041E --name th-TH \
	Other/th_TH.txt >Other/CID041e.cs

# Turkish

${LOC2CUL} --region MidEast --id 0x001F --name tr \
	--iso2 tr --iso3 tur --windows TRK \
	--ansi 1254 --ebcdic 20905 --mac 10081 --oem 857 --separator ';' \
	MidEast/tr.txt >MidEast/CID001f.cs

${LOC2CUL} --region MidEast --id 0x041F --name tr-TR \
	MidEast/tr_TR.txt >MidEast/CID041f.cs

# Ukrainian

${LOC2CUL} --region Other --id 0x0022 --name uk \
	--iso2 uk --iso3 ukr --windows UKR \
	--ansi 1251 --ebcdic 500 --mac 10017 --oem 866 --separator ';' \
	Other/uk.txt >Other/CID0022.cs

${LOC2CUL} --region Other --id 0x0422 --name uk-UA \
	Other/uk_UA.txt >Other/CID0422.cs

# Vietnamese

${LOC2CUL} --region Other --id 0x002A --name vi \
	--iso2 vi --iso3 vie --windows VIT \
	--ansi 1258 --ebcdic 500 --mac 10000 --oem 1258 --separator ',' \
	Other/vi.txt >Other/CID002a.cs

${LOC2CUL} --region Other --id 0x042A --name vi-VN \
	Other/vi_VN.txt >Other/CID042a.cs

# Chinese

${LOC2CUL} --region CJK --id 0x0004 --name zh --alias zh-CHS \
	--iso2 zh --iso3 zho --windows CHS \
	--ansi 936 --ebcdic 500 --mac 10008 --oem 936 --separator ',' \
	CJK/zh.txt >CJK/CID0004.cs

${LOC2CUL} --region CJK --id 0x0804 --name zh-CN \
	--ansi 936 --ebcdic 500 --mac 10008 --oem 936 --separator ',' \
	CJK/zh_CN.txt >CJK/CID0804.cs

${LOC2CUL} --region CJK --id 0x0C04 --name zh-HK \
	--windows ZHH \
	--ansi 950 --ebcdic 500 --mac 10002 --oem 950 --separator ',' \
	CJK/zh_HK.txt >CJK/CID0c04.cs

${LOC2CUL} --region CJK --id 0x1404 --name zh-MO \
	--windows ZHM \
	--ansi 950 --ebcdic 500 --mac 10002 --oem 950 --separator ',' \
	CJK/zh_MO.txt >CJK/CID1404.cs

${LOC2CUL} --region CJK --id 0x1004 --name zh-SG \
	--windows ZHI \
	--ansi 936 --ebcdic 500 --mac 10008 --oem 936 --separator ',' \
	CJK/zh_SG.txt >CJK/CID1004.cs

${LOC2CUL} --region CJK --id 0x0404 --name zh-TW --alias zh-CHT \
	--windows CHT \
	--ansi 950 --ebcdic 500 --mac 10002 --oem 950 --separator ',' \
	CJK/zh_TW.txt >CJK/CID0404.cs

exit 0

# Missing locales:
#
# az: Azeri
# Cy-az-AZ: Azeri (Cyrillic)
# lt-az-AZ: Azeri (Latin)
# de-LI: German (Liechtenstein)
# div: Dhivehi
# div-MV: Dhivehi (Maldives)
# en-BZ: English (Belize)
# en-CB: English (Caribbean)
# en-JM: English (Jamaica)
# en-TT: English (Trinidad and Tobago)
# fr-MC: (Principality of Monaco)
# ka: Georgian
# ke-GE: Georgian (Georgia)
# kk: Kazakh
# kk-KZ: Kazakh (Kazakhstan)
# ky: Kyrgyz
# ky-KZ: Kyrgyz (Kyrgyzstan)
# ms: Malay
# ms-BN: Malay (Brunei Darussalam)
# ms-MY: Malay (Malaysia)
# mn: Mongolian
# mn-MN: Mongolian (Mongolia)
# pa: Punjabi
# pa-IN: Punjabi (India)
# sa: Sanskrit
# sa-IN: Sanskrit (India)
# Cy-sr-SP: Serbian (Cyrillic)
# Lt-sr-SP: Serbian (Latin)
# syr: Syriac
# syr-SY: Syriac (Syria)
# tt: Tatar
# tt-TA: Tatar (Tatarstan)
# ur: Urdu
# ur-PK: Urdu (Islamic Republic of Pakistan)
# uz: Uzbek
# Cy-uz-UZ: Uzbek (Cyrillic)
# Lt-uz-UZ: Uzbek (Latin)
