/*
 * SoapServices.cs - Implementation of the
 *			"System.Runtime.Remoting.SoapServices" class.
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

namespace System.Runtime.Remoting
{

#if CONFIG_SERIALIZATION

using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Metadata;
using System.Text;

public class SoapServices
{
	// Internal state.
	private static Hashtable methodToAction;
	private static Hashtable actionToMethod;
	private static Hashtable elementToType;
	private static Hashtable typeToElement;
	private static Hashtable xmlTypeToType;
	private static Hashtable typeToXmlType;
	private static Hashtable fields;

	// Information about a field name and type.
	private sealed class FieldNameAndTypeInfo
	{
		// Internal state.
		public String name;
		public Type type;

		// Constructor.
		public FieldNameAndTypeInfo(String name, Type type)
				{
					this.name = name;
					this.type = type;
				}

	}; // class FieldNameAndTypeInfo

	// Information that is stored about a type's fields.
	private sealed class TypeFieldInfo
	{
		// Internal state.
		public Hashtable fieldAttrs;
		public Hashtable fieldElements;

		// Constructor.
		public TypeFieldInfo()
				{
					fieldAttrs = new Hashtable();
					fieldElements = new Hashtable();
				}

		// Determine if this object contains items.
		public bool IsPopulated
				{
					get
					{
						return (fieldAttrs.Count > 0 ||
								fieldElements.Count > 0);
					}
				}

		// Store attribute information for a field.
		public void StoreAttribute(String key, String name, Type type)
				{
					fieldAttrs[key] = new FieldNameAndTypeInfo(name, type);
				}

		// Store element information for a field.
		public void StoreElement(String key, String name, Type type)
				{
					fieldElements[key] = new FieldNameAndTypeInfo(name, type);
				}

		// Get attribute information for a field.
		public FieldNameAndTypeInfo GetAttribute(String key)
				{
					return (FieldNameAndTypeInfo)(fieldAttrs[key]);
				}

		// Get element information for a field.
		public FieldNameAndTypeInfo GetElement(String key)
				{
					return (FieldNameAndTypeInfo)(fieldElements[key]);
				}

	}; // class TypeFieldInfo

	// Cannot create instances of this class.
	private SoapServices() {}

	// Standard namespace prefixes, defined by Microsoft.
	public static String XmlNsForClrType
			{
				get
				{
					return "http://schemas.microsoft.com/clr/";
				}
			}
	public static String XmlNsForClrTypeWithAssembly
			{
				get
				{
					return "http://schemas.microsoft.com/clr/assem/";
				}
			}
	public static String XmlNsForClrTypeWithNs
			{
				get
				{
					return "http://schemas.microsoft.com/clr/ns/";
				}
			}
	public static String XmlNsForClrTypeWithNsAndAssembly
			{
				get
				{
					return "http://schemas.microsoft.com/clr/nsassem/";
				}
			}

	// Encode SOAP names.
	public static String CodeXmlNamespaceForClrTypeNamespace
				(String typeNamespace, String assemblyName)
			{
				StringBuilder builder = new StringBuilder();

				// Add the URI prefix and type name to the builder.
				if(typeNamespace != null && typeNamespace.Length > 0)
				{
					if(assemblyName != null && assemblyName.Length > 0)
					{
						// We have both type and assembly names.
						builder.Append(XmlNsForClrTypeWithNsAndAssembly);
						if(typeNamespace[0] != '.')
						{
							builder.Append(typeNamespace);
						}
						else
						{
							// Strip a leading ".", if present.
							builder.Append(typeNamespace, 1,
										   typeNamespace.Length - 1);
						}
						builder.Append(typeNamespace);
						builder.Append('/');
					}
					else
					{
						// We have only a type name.
						builder.Append(XmlNsForClrTypeWithNs);
						builder.Append(typeNamespace);
						return builder.ToString();
					}
				}
				else if(assemblyName != null && assemblyName.Length > 0)
				{
					// We have only an assembly name.
					builder.Append(XmlNsForClrTypeWithAssembly);
				}
				else
				{
					// Neither name was supplied.
					throw new ArgumentNullException
						("typeNamespace & assemblyName");
				}

				// Encode the assembly name and add it to the builder.
				foreach(char ch in assemblyName)
				{
					if(ch == ' ' || ch == '=' || ch == ',' || ch == '%')
					{
						builder.Append('%');
						BitConverter.AppendHex(builder, (int)ch);
					}
					else
					{
						builder.Append(ch);
					}
				}

				// Return the final string to the caller.
				return builder.ToString();
			}

	// Decode SOAP names.
	public static bool DecodeXmlNamespaceForClrTypeNamespace
				(String inNamespace, out String typeNamespace,
				 out String assemblyName)
			{
				String suffix;
				int index;
				StringBuilder builder;
				char ch;
				int temp;

				// Validate the parameter.
				if(inNamespace == null || inNamespace.Length == 0)
				{
					throw new ArgumentNullException("inNamespace");
				}

				// Clear the return values before we start, in case
				// we have to bail out early.
				typeNamespace = String.Empty;
				assemblyName = null;

				// Determine what form of URI we are dealing with.
				if(inNamespace.StartsWith(XmlNsForClrTypeWithNsAndAssembly))
				{
					// Type and assembly names.
					suffix = inNamespace.Substring
						(XmlNsForClrTypeWithNsAndAssembly.Length);
					index = suffix.IndexOf('/');
					if(index == -1)
					{
						return false;
					}
					typeNamespace = suffix.Substring(0, index);
					suffix = suffix.Substring(index + 1);
				}
				else if(inNamespace.StartsWith(XmlNsForClrTypeWithNs))
				{
					// Just the type name.
					typeNamespace = inNamespace.Substring
						(XmlNsForClrTypeWithNs.Length);
					return true;
				}
				else if(inNamespace.StartsWith(XmlNsForClrTypeWithAssembly))
				{
					// Just the assembly name.
					suffix = inNamespace.Substring
						(XmlNsForClrTypeWithAssembly.Length);
				}
				else
				{
					// This form of URI is not recognised.
					return false;
				}

				// Decode the suffix into an assembly name.
				builder = new StringBuilder();
				index = 0;
				while(index < suffix.Length)
				{
					ch = suffix[index++];
					if(ch == '%')
					{
						if((index + 1) >= suffix.Length)
						{
							return false;
						}
						ch = suffix[index++];
						if(ch >= '0' && ch <= '9')
						{
							temp = (ch - '0') << 4;
						}
						else if(ch >= 'A' && ch <= 'F')
						{
							temp = (ch - 'A' + 10) << 4;
						}
						else if(ch >= 'a' && ch <= 'f')
						{
							temp = (ch - 'a' + 10) << 4;
						}
						else
						{
							return false;
						}
						ch = suffix[index++];
						if(ch >= '0' && ch <= '9')
						{
							temp += ch - '0';
						}
						else if(ch >= 'A' && ch <= 'F')
						{
							temp += ch - 'A' + 10;
						}
						else if(ch >= 'a' && ch <= 'f')
						{
							temp += ch - 'a' + 10;
						}
						else
						{
							return false;
						}
						builder.Append((char)temp);
					}
					else
					{
						builder.Append(ch);
					}
				}
				assemblyName = builder.ToString();
				return true;
			}

	// Get field information from SOAP data.
	public static void GetInteropFieldTypeAndNameFromXmlAttribute
				(Type containingType, String xmlAttribute,
				 String xmlNamespace, out Type type, out String name)
			{
				TypeFieldInfo typeInfo;
				FieldNameAndTypeInfo info;
				if(containingType != null)
				{
					lock(typeof(SoapServices))
					{
						if(fields != null)
						{
							typeInfo = (TypeFieldInfo)(fields[containingType]);
							if(typeInfo != null)
							{
								info = typeInfo.GetAttribute
									(XmlKey(xmlAttribute, xmlNamespace));
								if(info != null)
								{
									name = info.name;
									type = info.type;
									return;
								}
							}
						}
					}
				}
				type = null;
				name = null;
			}
	public static void GetInteropFieldTypeAndNameFromXmlElement
				(Type containingType, String xmlElement,
				 String xmlNamespace, out Type type, out String name)
			{
				TypeFieldInfo typeInfo;
				FieldNameAndTypeInfo info;
				if(containingType != null)
				{
					lock(typeof(SoapServices))
					{
						if(fields != null)
						{
							typeInfo = (TypeFieldInfo)(fields[containingType]);
							if(typeInfo != null)
							{
								info = typeInfo.GetElement
									(XmlKey(xmlElement, xmlNamespace));
								if(info != null)
								{
									name = info.name;
									type = info.type;
									return;
								}
							}
						}
					}
				}
				type = null;
				name = null;
			}

	// Get type information from SOAP data.
	public static Type GetInteropTypeFromXmlElement
				(String xmlElement, String xmlNamespace)
			{
				lock(typeof(SoapServices))
				{
					if(elementToType != null)
					{
						return (Type)(elementToType
							[XmlKey(xmlElement, xmlNamespace)]);
					}
					else
					{
						return null;
					}
				}
			}
	public static Type GetInteropTypeFromXmlType
				(String xmlType, String xmlTypeNamespace)
			{
				lock(typeof(SoapServices))
				{
					if(elementToType != null)
					{
						return (Type)(xmlTypeToType
							[XmlKey(xmlType, xmlTypeNamespace)]);
					}
					else
					{
						return null;
					}
				}
			}

	// Get the SOAP action associated with a method.
	public static String GetSoapActionFromMethodBase(MethodBase mb)
			{
				// See if we have a registered action first.
				lock(typeof(SoapServices))
				{
					if(methodToAction != null)
					{
						String temp = (String)(methodToAction[mb]);
						if(temp != null)
						{
							return temp;
						}
					}
				}

				// Get the action from the method itself.
				return ((SoapMethodAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(mb))
						.SoapAction;
			}

	// Extract the type name from an XML namespace indication.
	private static String ExtractTypeName(String name, out bool hasAssembly)
			{
				String typeName;
				String assemblyName;
				if(!DecodeXmlNamespaceForClrTypeNamespace
						(name, out typeName, out assemblyName))
				{
					hasAssembly = false;
					return null;
				}
				if(assemblyName == null || assemblyName.Length == 0)
				{
					if(name.StartsWith(XmlNsForClrTypeWithNs))
					{
						hasAssembly = true;
						return typeName + ", mscorlib";
					}
					else
					{
						hasAssembly = false;
						return typeName;
					}
				}
				else
				{
					hasAssembly = true;
					return typeName + ", " + assemblyName;
				}
			}

	// Get type and method information from a SOAP action.
	public static bool GetTypeAndMethodNameFromSoapAction
				(String soapAction, out String typeName, out String methodName)
			{
				int index;
				bool hasAssembly;

				// Remove quotes from the action name.
				if(soapAction[0] == '"' && soapAction.EndsWith("\""))
				{
					soapAction = soapAction.Substring
						(1, soapAction.Length - 2);
				}

				// Look in the registered action table.
				lock(typeof(SoapServices))
				{
					if(actionToMethod != null)
					{
						MethodBase mb =
							(MethodBase)(actionToMethod[soapAction]);
						if(mb != null)
						{
							String assembly =
								mb.DeclaringType.Module.Assembly.FullName;
							index = assembly.IndexOf(',');
							if(index != -1)
							{
								assembly = assembly.Substring(0, index);
							}
							typeName = mb.DeclaringType.FullName +
								", " + assembly;
							methodName = mb.Name;
							return true;
						}
					}
				}

				// Split the action into type and method name.
				index = soapAction.IndexOf('#');
				if(index != -1)
				{
					typeName = ExtractTypeName
						(soapAction.Substring(0, index), out hasAssembly);
					if(typeName != null)
					{
						methodName = soapAction.Substring(index + 1);
						return true;
					}
				}

				// Unable to determine the type and method names.
				typeName = null;
				methodName = null;
				return false;
			}

	// Get the XML element information for serializing a type.
	public static bool GetXmlElementForInteropType
				(Type type, out String xmlElement, out String xmlNamespace)
			{
				// Look in the registered element table first.
				lock(typeof(SoapServices))
				{
					if(typeToElement != null)
					{
						String key = (String)(typeToElement[type]);
						if(key != null)
						{
							XmlKeyExpand(key, out xmlElement,
										 out xmlNamespace);
							return true;
						}
					}
				}

				// Check the attribute on the type.
				SoapTypeAttribute tattr = (SoapTypeAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(type);
				if(tattr.xmlElementWasSet)
				{
					xmlElement = tattr.XmlElementName;
					xmlNamespace = tattr.XmlNamespace;
					return true;
				}

				// We were unable to determine the XML element information.
				xmlElement = null;
				xmlNamespace = null;
				return false;
			}

	// Get the namespace to use for a method call.
	public static String GetXmlNamespaceForMethodCall(MethodBase mb)
			{
				return ((SoapMethodAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(mb))
						.XmlNamespace;
			}

	// Get the namespace to use for a method response.
	public static String GetXmlNamespaceForMethodResponse(MethodBase mb)
			{
				return ((SoapMethodAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(mb))
						.ResponseXmlNamespace;
			}

	// Get XML type information for a type.
	public static bool GetXmlTypeForInteropType
				(Type type, out String xmlType, out String xmlTypeNamespace)
			{
				// Look in the registered type table first.
				lock(typeof(SoapServices))
				{
					if(typeToXmlType != null)
					{
						String key = (String)(typeToXmlType[type]);
						if(key != null)
						{
							XmlKeyExpand(key, out xmlType,
										 out xmlTypeNamespace);
							return true;
						}
					}
				}

				// Check the attribute on the type.
				SoapTypeAttribute tattr = (SoapTypeAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(type);
				if(tattr.xmlTypeWasSet)
				{
					xmlType = tattr.XmlTypeName;
					xmlTypeNamespace = tattr.XmlTypeNamespace;
					return true;
				}

				// We were unable to determine the XML element information.
				xmlType = null;
				xmlTypeNamespace = null;
				return false;
			}

	// Determine if a namespace indicates CLR information.
	public static bool IsClrTypeNamespace(String namespaceString)
			{
				if(namespaceString != null)
				{
					return namespaceString.StartsWith
						("http://schemas.microsoft.com/clr/");
				}
				else
				{
					return false;
				}
			}

	// Determine if a SOAP action is valid for a method.
	public static bool IsSoapActionValidForMethodBase
				(String soapAction, MethodBase mb)
			{
				SoapMethodAttribute mattr;
				int index, index2;
				String typeName;
				String mbTypeName;
				String assembly;
				bool hasAssembly;

				// Remove quotes from the action name.
				if(soapAction[0] == '"' && soapAction.EndsWith("\""))
				{
					soapAction = soapAction.Substring
						(1, soapAction.Length - 2);
				}

				// If the action matches the attribute, then return true.
				mattr = (SoapMethodAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(mb);
				if(mattr.SoapAction == soapAction)
				{
					return true;
				}

				// Determine if the action matches the one registered
				// with the method.
				lock(typeof(SoapServices))
				{
					if(methodToAction != null)
					{
						String temp = (String)(methodToAction[mb]);
						if(temp != null && temp == soapAction)
						{
							return true;
						}
					}
				}

				// Pull apart the action and check its components.
				index = soapAction.IndexOf('#');
				if(index != -1)
				{
					typeName = ExtractTypeName
						(soapAction.Substring(0, index), out hasAssembly);
					if(typeName != null)
					{
						if(hasAssembly)
						{
							assembly = mb.DeclaringType.Module
								.Assembly.FullName;
							index2 = assembly.IndexOf(',');
							if(index2 != -1)
							{
								assembly = assembly.Substring(0, index2);
							}
							mbTypeName = mb.DeclaringType.FullName +
								", " + assembly;
						}
						else
						{
							mbTypeName = mb.DeclaringType.FullName;
						}
						if(typeName == mbTypeName)
						{
							return (mb.Name == soapAction.Substring(index + 1));
						}
					}
				}

				// The action string is not valid.
				return false;
			}

	// Preload SOAP handling information.
	public static void PreLoad(Assembly assembly)
			{
				foreach(Type type in assembly.GetTypes())
				{
					PreLoad(type);
				}
			}
	public static void PreLoad(Type type)
			{
				// Register soap actions for the methods in the type.
				foreach(MethodInfo method in type.GetMethods())
				{
					RegisterSoapActionForMethodBase(method);
				}

				// Register XML tags for the type, if specified.
				SoapTypeAttribute tattr = (SoapTypeAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(type);
				if(tattr.xmlElementWasSet)
				{
					RegisterInteropXmlElement
						(tattr.XmlElementName, tattr.XmlNamespace, type);
				}
				if(tattr.xmlTypeWasSet)
				{
					RegisterInteropXmlType
						(tattr.XmlTypeName, tattr.XmlTypeNamespace, type);
				}

				// Load and register the field mapping information.
				lock(typeof(SoapServices))
				{
					if(fields == null)
					{
						fields = new Hashtable();
					}
					TypeFieldInfo typeInfo = new TypeFieldInfo();
					foreach(FieldInfo field in type.GetFields())
					{
						SoapFieldAttribute fattr = (SoapFieldAttribute)
							InternalRemotingServices.GetCachedSoapAttribute
								(field);
						if(fattr.IsInteropXmlElement())
						{
							if(fattr.UseAttribute)
							{
								typeInfo.StoreAttribute
									(XmlKey(fattr.XmlElementName,
											fattr.XmlNamespace),
								 	 field.Name, field.FieldType);
							}
							else
							{
								typeInfo.StoreElement
									(XmlKey(fattr.XmlElementName,
											fattr.XmlNamespace),
								 	 field.Name, field.FieldType);
							}
						}
					}
					if(typeInfo.IsPopulated)
					{
						fields[type] = typeInfo;
					}
				}
			}

	// Build a key value for looking up XML element information.
	private static String XmlKey(String name, String nspace)
			{
				if(nspace != null)
				{
					return name + "\uFFFF" + nspace;
				}
				else
				{
					return name;
				}
			}

	// Convert an XML element key value back into its name and namespace.
	private static void XmlKeyExpand(String key, out String name,
									 out String nspace)
			{
				int index = key.IndexOf('\uFFFF');
				if(index == -1)
				{
					name = key;
					nspace = null;
				}
				else
				{
					name = key.Substring(0, index);
					nspace = key.Substring(index + 1);
				}
			}

	// Register an XML element name with a type.
	public static void RegisterInteropXmlElement
				(String xmlElement, String xmlNamespace, Type type)
			{
				lock(typeof(SoapServices))
				{
					// Create the mapping tables if necessary.
					if(elementToType == null)
					{
						elementToType = new Hashtable();
						typeToElement = new Hashtable();
					}

					// Add the element to type and type to element mappings.
					String key = XmlKey(xmlElement, xmlNamespace);
					elementToType[key] = type;
					typeToElement[type] = key;
				}
			}

	// Register an XML type name with a type.
	public static void RegisterInteropXmlType
				(String xmlType, String xmlTypeNamespace, Type type)
			{
				lock(typeof(SoapServices))
				{
					// Create the mapping tables if necessary.
					if(elementToType == null)
					{
						xmlTypeToType = new Hashtable();
						typeToXmlType = new Hashtable();
					}

					// Add the element to type and type to element mappings.
					String key = XmlKey(xmlType, xmlTypeNamespace);
					xmlTypeToType[key] = type;
					typeToXmlType[type] = key;
				}
			}

	// Register a SOAP action for a method.
	public static void RegisterSoapActionForMethodBase(MethodBase mb)
			{
				// Fetch the action from the method base.
				SoapMethodAttribute mattr;
				mattr = (SoapMethodAttribute)
					InternalRemotingServices.GetCachedSoapAttribute(mb);
				if(mattr.soapActionWasSet)
				{
					RegisterSoapActionForMethodBase(mb, mattr.SoapAction);
				}
			}
	public static void RegisterSoapActionForMethodBase
				(MethodBase mb, String soapAction)
			{
				if(soapAction == null)
				{
					return;
				}
				lock(typeof(SoapServices))
				{
					// Make sure that the tables have been created.
					if(methodToAction == null)
					{
						methodToAction = new Hashtable();
						actionToMethod = new Hashtable();
					}

					// Register the method to action mapping.
					methodToAction[mb] = soapAction;

					// Register the action to method mapping.
					actionToMethod[soapAction] = mb;
				}
			}

}; // class SoapServices

#endif // CONFIG_SERIALIZATION

}; // namespace System.Runtime.Remoting
