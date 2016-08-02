/*
 * <csharp.h> - Definitions that ease integration with C# code.
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

#ifndef _CSHARP_H
#define _CSHARP_H

#include <features.h>
#include <stdbool.h>

__BEGIN_DECLS

/*
 * Import the contents of the "System" namespace.
 */
__using__ __namespace__ System;

/*
 * C# keywords.
 */
#undef  null
#define null            __null__
#undef  true
#define true            __true__
#undef  false
#define false           __false__
#undef  new
#define new             __new__
#undef  delete
#define delete          __delete__
#undef  try
#define try             __try__
#undef  catch
#define catch           __catch__
#undef  finally
#define finally         __finally__
#undef  throw
#define throw           __throw__
#undef  lock
#define lock            __lock__
#undef  checked
#define checked         __checked__
#undef  unchecked
#define unchecked       __unchecked__
#undef  params
#define params          __params__
#undef  using
#define using           __using__
#undef  namespace
#define namespace       __namespace__
#undef  object
#define object          System::Object
#undef  string
#define string          System::String
#undef  decimal
#define decimal         System::Decimal

__END_DECLS

#endif  /* !_CSHARP_H */
