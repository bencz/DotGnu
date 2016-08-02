using System;
using System.Drawing;

namespace System.Windows.Forms
{
	public class Help
	{

		public static void ShowHelp(Control parent, string url)
		{
		}

		public static void ShowHelp(Control parent, string url, HelpNavigator navigator)
		{
		}

		public static void ShowHelp(Control parent, string url, string keyword)
		{
		}

		public static void ShowHelp(Control parent, string url, HelpNavigator command, object param)
		{
		}

		public static void ShowHelpIndex(Control parent, string url)
		{
			ShowHelp(parent, url, HelpNavigator.Index, null);
		}

		public static void ShowPopup(Control parent, string caption, Point location)
		{
		}

	}

}
