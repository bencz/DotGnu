/*
 * ActivationContext.cs - Implementation of "System.ActivationContext".
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
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

#if CONFIG_FRAMEWORK_2_0

[TODO]
public sealed class ActivationContext
{
	public enum ContextForm
	{
		Loose			= 1,
		Storebounded	= 2
	}

	// TODO

	private ContextForm contextForm;

	internal Object componentManifest;
	internal Object deploymentManifest;

	internal void PrepareForExecution() {}

	[TODO]
	private ActivationContext()
	{
		contextForm = ContextForm.Loose;
	}

	[TODO]
	public static ActivationContext CreatePartialActivationContext(
						ApplicationIdentity identity)
	{
		if(identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		return new ActivationContext();
	}

	[TODO]
	public static ActivationContext CreatePartialActivationContext(
						ApplicationIdentity identity, String[] manifestPaths)
	{
		if(identity == null)
		{
			throw new ArgumentNullException("identity");
		}
		return new ActivationContext();
	}

	public ContextForm Form
	{
		get
		{
			return contextForm;
		}
	}

	[TODO]
	public ApplicationIdentity Identity
	{
		get
		{
			return null;
		}
	}

	[TODO]
	public void Dispose()
	{
	}
}; // class ActivationContext

#endif // CONFIG_FRAMEWORK_2_0

}; // namespace System
