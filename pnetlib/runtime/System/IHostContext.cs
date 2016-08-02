/*
 * IHostContext.cs - Implementation of the "System.IHostContext" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

namespace System
{

#if !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

public interface IHostContext
{
	// Determine if the launching code can assume trust in the application.
	bool AssumeTrust { get; }

	// Determine if permission sets should only grant what is required.
	bool ExclusiveGrant { get; }

	// True to remove any previous cached trust information.
	bool IsFirstTimeInstall { get; }

	// Determine if the trust manager should suppress prompts.
	bool NoPrompt { get; }

	// Determine if permission information should be persisted.
	bool Persist { get; }

}; // interface IHostContext

#endif // !ECMA_COMPAT && CONFIG_FRAMEWORK_1_2

}; // namespace System
