/*
 * <assembly/ICSharpCode.SharpZipLib.h> - Import "ICSharpCode.SharpZipLib.dll".
 *
 * This file is part of the Portable.NET C library.
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

#ifndef _ASSEMBLY_IASSEMBLYCODE_SHARPZIPLIB_H
#define _ASSEMBLY_IASSEMBLYCODE_SHARPZIPLIB_H

#include <csharp.h>

__BEGIN_DECLS

#using <ICSharpCode.SharpZipLib.dll>

using namespace ICSharpCode::SharpZipLib;
using namespace ICSharpCode::SharpZipLib::BZip2;
using namespace ICSharpCode::SharpZipLib::Checksums;
using namespace ICSharpCode::SharpZipLib::GZip;
using namespace ICSharpCode::SharpZipLib::Tar;
using namespace ICSharpCode::SharpZipLib::Zip;
using namespace ICSharpCode::SharpZipLib::Zip::Compression;
using namespace ICSharpCode::SharpZipLib::Zip::Compression::Streams;

__END_DECLS

#endif  /* !_ASSEMBLY_IASSEMBLYCODE_SHARPZIPLIB_H */
