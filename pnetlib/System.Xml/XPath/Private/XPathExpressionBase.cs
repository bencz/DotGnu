/*
 * XPathExpressionBase.cs - base class for compiled XPathExpressions
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

#if CONFIG_XPATH

namespace System.Xml.XPath.Private
{

using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

internal abstract class XPathExpressionBase : XPathExpression
{
	private XmlNamespaceManager nsManager = null;

	private XPathResultType resultType = XPathResultType.Error;

	public override void AddSort(Object expr, IComparer comparer){}

	public override void AddSort(Object expr, XmlSortOrder order, 
								XmlCaseOrder caseOrder, 
								String lang, XmlDataType dataType){}
	
	public override XPathExpression Clone()
	{
		return null; 
	}

	public override void SetContext(XmlNamespaceManager nsManager)
	{
		this.nsManager = nsManager;
	}

	public override String Expression
	{
		get
		{
			return "";
		}
	}

	public override XPathResultType ReturnType
	{
		get
		{
			try
			{
				if(resultType == XPathResultType.Error)
				{
					if(this is Expression)
					{
						return (this as Expression).Compile();
					}
				}
			}
			catch(XPathException)
			{
				throw;
			}
			catch(Exception e)
			{
				throw new XPathException("Error during compile", e);
			}
			return resultType;
		}
	}

	protected XmlNamespaceManager NamespaceManager 
	{
		get
		{
			return this.nsManager;
		}
	}

	internal Object Evaluate(XPathNodeIterator iterator)
	{
		if(this is Expression)
		{
			return ((Expression)this).EvaluateInternal(iterator);
		}
		throw new Exception("Unknown type to Eval:" + this.GetType());
	}

	// Note: this would have been much cleaner if I could use
	// TreeCC's coercion system.
	internal Object EvaluateAs(XPathNodeIterator iterator, XPathResultType type)
	{
		Object result = Evaluate(iterator);
		XPathResultType resultType = ReturnType;
		if(ReturnType == XPathResultType.Any)
		{
			if(result is XPathNodeIterator)
			{
				resultType = XPathResultType.NodeSet;
			}
			else if(result is String) 
			{
				resultType = XPathResultType.String; 
			}
			else if(result is bool)
			{
				resultType = XPathResultType.Boolean;
			}
			else if(result is double)
			{
				resultType = XPathResultType.Number;
			}
			else if(result is XPathNavigator)
			{
				resultType = XPathResultType.Navigator;
			}
			else
			{
				throw new XPathException("unknown node type: " + result.GetType(), null); 
			}
		}
		
		// easy cases first :)
		if(resultType == type)
		{
			return result;
		}
		
		switch(resultType)
		{
			case XPathResultType.Boolean:
			{
				bool boolValue = (bool)result;
				switch(type)
				{
					case XPathResultType.Number:
					{
						return boolValue ? 1 : 0;
					}
					break;
					case XPathResultType.String:
					{
						return boolValue ? "true" : "false";
					}
					break;
				}
			}
			break;
			case XPathResultType.String:
			{
				String strValue = String.Empty;
				if(result is XPathNavigator)
				{
					result = (result as XPathNavigator).Value;
				}
				else 
				{
					result = (String) result;
				}

				switch(type)
				{
					case XPathResultType.Boolean:
					{
						return (strValue.Length != 0);
					}
					break;
					case XPathResultType.Number:
					{
						// TODO
					}
					break;
				}
			}
			break;
			case XPathResultType.Number:
			{
				double val = (double)result;
				switch(type)
				{
					case XPathResultType.Boolean:
					{
						return (val != 0.0 && val != -0.0 && !Double.IsNaN(val));
					}
					break;
					case XPathResultType.String:
					{
						return val.ToString();
					}
					break;
				}
			}
			break;
			case XPathResultType.NodeSet:
			{
				XPathNodeIterator iterValue = (XPathNodeIterator) result;
				switch(type)
				{
					case XPathResultType.Boolean:
					{
						return (iterValue != null && iterValue.MoveNext()) ;
					}
					break;
					case XPathResultType.String:
					{
						if(iterValue != null && iterValue.MoveNext())
						{
							return iterValue.Current.Value;
						}
						return String.Empty;
					}
					break;
					case XPathResultType.Number:
					{
						// TODO:
					}
					break;
				}
			}
			break;
		}
		throw new Exception("Unknown type to Eval:" + this.GetType());
	}

}
}//namespace

#endif // CONFIG_XPATH
