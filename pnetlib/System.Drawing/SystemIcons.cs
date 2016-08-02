/*
 * SystemIcons.cs - Implementation of the "System.Drawing.SystemIcons" class.
 *
 * Copyright (C) 2003  Neil Cawse.
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
namespace System.Drawing
{
using System;

	public sealed class SystemIcons
	{
		private enum IconType
		{
			Application		= 0,
			Asterisk		= 1,
			Error			= 2,
			Exclamation		= 3,
			Hand			= 4,
			Information		= 5,
			Question		= 6,
			Warning			= 7,
			WinLogo			= 8
		}

		private static Icon applicationIcon;
		private static Icon asteriskIcon;
		private static Icon errorIcon;
		private static Icon exclamationIcon;
		private static Icon handIcon;
		private static Icon informationIcon;
		private static Icon questionIcon;
		private static Icon warningIcon;
		private static Icon windowsLogoIcon;

		public SystemIcons() {}

		public static Icon Application
		{
			get
			{
				if (applicationIcon == null)
					applicationIcon = LoadIcon(IconType.Application);
				return applicationIcon;
			}
		}

		public static Icon Asterisk
		{
			get
			{
				if (asteriskIcon == null)
				{
					asteriskIcon = LoadIcon(IconType.Asterisk);
				}
				return asteriskIcon;
			}
		}

		public static Icon Error
		{
			get
			{
				if (errorIcon == null)
				{
					errorIcon = LoadIcon(IconType.Error);
				}
				return errorIcon;
			}
		}

		public static Icon Exclamation
		{
			get
			{
				if (exclamationIcon == null)
				{
					exclamationIcon = LoadIcon(IconType.Exclamation);
				}
				return exclamationIcon;
			}
		}

		public static Icon Hand
		{
			get
			{
				if (handIcon == null)
				{
					handIcon = LoadIcon(IconType.Hand);
				}
				return handIcon;
			}
		}

		public static Icon Information
		{
			get
			{
				if (informationIcon == null)
				{
					informationIcon = LoadIcon(IconType.Information);
				}
				return informationIcon;
			}
		}

		public static Icon Question
		{
			get
			{
				if (questionIcon == null)
				{
					questionIcon = LoadIcon(IconType.Question);
				}
				return questionIcon;
			}
		}

		public static Icon Warning
		{
			get
			{
				if (warningIcon == null)
				{
					warningIcon = LoadIcon(IconType.Warning);
				}
				return warningIcon;
			}
		}

		public static Icon WinLogo
		{
			get
			{
				if (windowsLogoIcon == null)
				{
					windowsLogoIcon = LoadIcon(IconType.WinLogo);
				}
				return windowsLogoIcon;
			}
		}

		// Load a particular icon from this assembly's resources.
		// Stolen from S.W.F.MessageBox.cs
		private static Icon LoadIcon(SystemIcons.IconType iconType)
		{
			lock(typeof(SystemIcons))
			{
				Icon iconObject = null;
				try
				{
					switch(iconType)
					{
						case IconType.Application:
						{
							if(applicationIcon == null)
							{
								applicationIcon = new Icon
									(typeof(SystemIcons), "application.ico");
							}
							iconObject = applicationIcon;
						}
						break;

						case IconType.Asterisk:
						{
							if(asteriskIcon == null)
							{
								asteriskIcon = new Icon
									(typeof(SystemIcons), "asterisk.ico");
							}
							iconObject = asteriskIcon;
						}
						break;

						case IconType.Error:
						{
							if(errorIcon == null)
							{
								errorIcon = new Icon
									(typeof(SystemIcons), "error.ico");
							}
							iconObject = errorIcon;
						}
						break;

						case IconType.Exclamation:
						{
							if(exclamationIcon == null)
							{
								exclamationIcon = new Icon
									(typeof(SystemIcons), "exclamation.ico");
							}
							iconObject = exclamationIcon;
						}
						break;

						case IconType.Hand:
						{
							if(handIcon == null)
							{
								handIcon = new Icon
									(typeof(SystemIcons), "hand.ico");
							}
							iconObject = handIcon;
						}
						break;

						case IconType.Information:
						{
							if(informationIcon == null)
							{
								informationIcon = new Icon
									(typeof(SystemIcons), "information.ico");
							}
							iconObject = exclamationIcon;
						}
						break;

						case IconType.Question:
						{
							if(questionIcon == null)
							{
								questionIcon = new Icon
									(typeof(SystemIcons), "question.ico");
							}
							iconObject = questionIcon;
						}
						break;

						case IconType.Warning:
						{
							if(warningIcon == null)
							{
								warningIcon = new Icon
									(typeof(SystemIcons), "warning.ico");
							}
							iconObject = warningIcon;
						}
						break;

						case IconType.WinLogo:
						{
							if(windowsLogoIcon == null)
							{
								windowsLogoIcon = new Icon
									(typeof(SystemIcons), "windows.ico");
							}
							iconObject = windowsLogoIcon;
						}
						break;
					}
				}
				catch
				{
					// Could not load the icon - ignore this condition.
				}
				return iconObject;
			}
		}

	}
}
