/*
 * FeatureSupport.cs - Implementation of the
 *			"System.Windows.Forms.FeatureSupport" class.
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

namespace System.Windows.Forms
{

#if !CONFIG_COMPACT_FORMS

using System.Reflection;

public abstract class FeatureSupport : IFeatureSupport
{
	// Constructor.
	protected FeatureSupport() {}

	// Get a feature support object.
	private static IFeatureSupport GetFeatureSupport(String featureClassName)
			{
			#if CONFIG_REFLECTION
				Type type = Type.GetType(featureClassName);
				if(type != null &&
				   typeof(IFeatureSupport).IsAssignableFrom(type))
				{
					ConstructorInfo ctor =
						type.GetConstructor(Type.EmptyTypes);
					if(ctor != null)
					{
						return (ctor.Invoke(new Object [0]) as
										IFeatureSupport);
					}
				}
			#endif
				return null;
			}

	// Get the version of a specific feature which is present.
	public abstract Version GetVersionPresent(Object feature);
	public static Version GetVersionPresent
				(String featureClassName, String featureConstName)
			{
				IFeatureSupport feature;
				feature = GetFeatureSupport(featureClassName);
				if(feature != null)
				{
					return feature.GetVersionPresent(featureConstName);
				}
				else
				{
					return null;
				}
			}

	// Determine if a feature is present.
	public virtual bool IsPresent(Object feature)
			{
				Version version = GetVersionPresent(feature);
				return (version != null);
			}
	public virtual bool IsPresent(Object feature, Version minimumVersion)
			{
				Version version = GetVersionPresent(feature);
				if(minimumVersion != null)
				{
					return (version != null && version >= minimumVersion);
				}
				else
				{
					return (version != null);
				}
			}
	public static bool IsPresent
				(String featureClassName, String featureConstName)
			{
				IFeatureSupport feature;
				feature = GetFeatureSupport(featureClassName);
				if(feature != null)
				{
					return feature.IsPresent(featureConstName);
				}
				else
				{
					return false;
				}
			}
	public static bool IsPresent
				(String featureClassName, String featureConstName,
				 Version minimumVersion)
			{
				IFeatureSupport feature;
				feature = GetFeatureSupport(featureClassName);
				if(feature != null)
				{
					return feature.IsPresent
						(featureConstName, minimumVersion);
				}
				else
				{
					return false;
				}
			}

}; // class FeatureSupport

#endif // !CONFIG_COMPACT_FORMS

}; // namespace System.Windows.Forms
