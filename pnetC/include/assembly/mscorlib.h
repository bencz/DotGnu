/*
 * <assembly/mscorlib.h> - Import all of the definitions in the core library.
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

#ifndef _ASSEMBLY_MSCORLIB_H
#define _ASSEMBLY_MSCORLIB_H

#include <csharp.h>

__BEGIN_DECLS

#using <mscorlib.dll>

using namespace System;
using namespace System::Collections;
using namespace System::Configuration::Assemblies;
using namespace System::Diagnostics;
using namespace System::Diagnostics::SymbolStore;
using namespace System::Globalization;
using namespace System::IO;
using namespace System::IO::IsolatedStorage;
using namespace System::Reflection;
using namespace System::Reflection::Emit;
using namespace System::Resources;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Runtime::Remoting;
using namespace System::Runtime::Remoting::Activation;
using namespace System::Runtime::Remoting::Channels;
using namespace System::Runtime::Remoting::Contexts;
using namespace System::Runtime::Remoting::Lifetime;
using namespace System::Runtime::Remoting::Messaging;
using namespace System::Runtime::Remoting::Metadata;
using namespace System::Runtime::Remoting::Metadata::W3cXsd2001;
using namespace System::Runtime::Remoting::Proxies;
using namespace System::Runtime::Remoting::Services;
using namespace System::Security;
using namespace System::Security::Cryptography;
using namespace System::Security::Cryptography::X509Certificates;
using namespace System::Security::Permissions;
using namespace System::Security::Policy;
using namespace System::Security::Principal;
using namespace System::Serialization;
using namespace System::Serialization::Formatters;
using namespace System::Serialization::Formatters::Binary;
using namespace System::Text;
using namespace System::Threading;
using namespace Microsoft::Win32;

__END_DECLS

#endif  /* !_ASSEMBLY_MSCORLIB_H */
