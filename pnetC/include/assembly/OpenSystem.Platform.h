/*
 * <assembly/OpenSystem.Platform.h> - Import "OpenSystem.Platform.dll".
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

#ifndef _ASSEMBLY_OPENSYSTEM_PLATFORM_H
#define _ASSEMBLY_OPENSYSTEM_PLATFORM_H

#include <csharp.h>

__BEGIN_DECLS

#using <OpenSystem.Platform.dll>

/* Note: we don't explicitly import the "OpenSystem::Platform" namespace
   because the definitions of "size_t", "off_t", etc will interfere
   with the definitions in the standard C headers.  Use a construct
   such as "OpenSystem::Platform::size_t" to access the platform types */

__END_DECLS

#endif  /* !_ASSEMBLY_OPENSYSTEM_PLATFORM_H */
