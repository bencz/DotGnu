/*
 * <assembly/System.Web.h> - Import the definitions in "System.Web.dll".
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

#ifndef _ASSEMBLY_SYSTEM_WEB_H
#define _ASSEMBLY_SYSTEM_WEB_H

#include <csharp.h>

__BEGIN_DECLS

#using <System.Web.dll>

using namespace System::Web;
using namespace System::Web::Caching;
using namespace System::Web::Compilation;
using namespace System::Web::Configuration;
using namespace System::Web::Handlers;
using namespace System::Web::Hosting;
using namespace System::Web::Mail;
using namespace System::Web::Security;
using namespace System::Web::SessionState;
using namespace System::Web::UI;
using namespace System::Web::UI::HtmlControls;
using namespace System::Web::UI::Util;
using namespace System::Web::UI::WebControls;
using namespace System::Web::Util;

__END_DECLS

#endif  /* !_ASSEMBLY_SYSTEM_WEB_H */
