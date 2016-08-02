/*
 * XsltSettings.cs - Implementation of "System.Xml.Xsl.XsltSettings" class.
 *
 * Copyright (C) 2003  Southern Storm Software, Pty Ltd.
 * 
 * Contributed by Klaus.T.
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

#if !ECMA_COMPAT
#if CONFIG_FRAMEWORK_2_0
#if CONFIG_XPATH && CONFIG_XSL
namespace System.Xml.Xsl
{
	public sealed class XsltSettings
	{
		private static XsltSettings defaultSettings;
		private static XsltSettings trustedXslt;

		Boolean readOnly;
		Boolean enableDocumentFunction;
		Boolean enableScript;

		static XsltSettings ()
		{
			defaultSettings = new XsltSettings (true);
		}

		public static XsltSettings Default
		{
			get
			{
				if(defaultSettings == null)
				{
					defaultSettings = new XsltSettings(true);
				}
				return defaultSettings;
			}
		}

		public static XsltSettings TrustedXslt
		{
			get
			{
				if(trustedXslt == null)
				{
					trustedXslt = new XsltSettings(true);
					trustedXslt.enableDocumentFunction = true;
					trustedXslt.enableScript = true;
				}					
				return trustedXslt;
			}
		}

		public XsltSettings()
		{
		}

		public XsltSettings(Boolean enableDocumentFunction, bool enableScript)
		{
			this.enableDocumentFunction = enableDocumentFunction;
			this.enableScript = enableScript;
		}

		private XsltSettings(bool readOnly)
		{
			this.readOnly = readOnly;
		}

		public bool EnableDocumentFunction
		{
			get
			{
				return enableDocumentFunction;
			}
			set
			{
				if(!readOnly)
				{
					enableDocumentFunction = value;
				}
			}
		}

		public bool EnableScript
		{
			get
			{
				return enableScript;
			}
			set
			{
				if(!readOnly)
				{
					enableScript = value;
				}
			}
		}
	}
} // namespace
#endif // CONFIG_XPATH && CONFIG_XSL
#endif // CONFIG_FRAMEWORK_2_0
#endif // !ECMA_COMPAT
