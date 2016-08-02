/*
 * ThemePainterXP.cs - XP visual style theme engine
 * Implements "System.Windows.Forms.Themes.DefaultThemePainter" class.
 *
 * Copyright (C) 2004 by Maciek Plewa
 * Portions Copyright (C) 2004 by Nordic Compona Solutions
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
 * As a special exception, the copyright holders of this library give you
 * permission to link this library with independent modules to produce an
 * executable, regardless of the license terms of these independent modules,
 * and to copy and distribute the resulting executable under terms of your
 * choice, provided that you also meet, for each linked independent module,
 * the terms and conditions of the license of that module.  An independent
 * module is a module which is not derived from or based on this library.
 * If you modify this library, you may extend this exception to your
 * version of the library, but you are not obligated to do so.  If you do
 * not wish to do so, delete this exception statement from your version.
 * 
 * Short-term TODO list:
 * 
 * 1) Support hover (onmouseover) effects, modify core SWF classes if necessary
 * 2) Add theming support for ListView, TabPage and ComboBox - this will require
 * fixing SWF
 * 
 * Long-term TODO list:
 * 
 * 1) Profile and optimize
 * 2) Load 3rd party styles from file
 * 3) Use WINE
 */

namespace System.Windows.Forms.Themes
{
	using System;
	using System.Text;
	using System.Drawing;
	using System.Drawing.Win32;
	using System.Drawing.Toolkit;
	using System.Drawing.Drawing2D;
	using System.Windows.Forms;
	using System.Windows.Forms.Themes;
	using System.Runtime.InteropServices;

	using ThemeXP;
	using ThemeXP.UxTheme;

	/// <summary>
	/// Implements XP visual styles for System.Windows.Forms controls
	/// This is ALPHA version, all methods should be considered as TODO
	/// </summary>
	public class ThemePainter : DefaultThemePainter
	{
		private bool enableTheme = false;

		public ThemePainter()
		{
			// Leave enableTheme in default value (false) if the platform is not NT
			if (System.Environment.OSVersion.Platform != System.PlatformID.Win32NT)
				return;

			// System is NT, proceed with checking the version
			System.Version osVersion = WinAPI.GetSystemVersion();

			#if DEBUG
			Console.WriteLine("ThemePainterXP() OS platform - {0}, version - {1}"
				+ osVersion, System.Environment.OSVersion.Platform);
			#endif

			// This theme supports Windows XP (5.1) and Windows 2003 (5.2) only
			if (osVersion.Major >= 5 && osVersion.Minor >= 1)
				this.enableTheme = true;
		}


		/// <summary>
		/// Draws a button control
		/// </summary>
		public override void DrawButton
			(System.Drawing.Graphics graphics, int x, int y, int width, int height,
			ButtonState state, System.Drawing.Color foreColor, System.Drawing.Color backColor,
			bool isDefault)
		{
			// revert to default theme if necessary
			if (!enableTheme) // && !FlatStyle.System
			{
				base.DrawButton(graphics, x, y, width, height, state, foreColor, backColor, isDefault);
				return;
			}

			PushButtonStates ctlState = PushButtonStates.PBS_NORMAL;

			#if DEBUG
			Console.WriteLine("DrawButton() state: {0:X}, default: {1}", state, isDefault);
			#endif

			if ((state & (ButtonState)0x20000) != 0)
				ctlState = PushButtonStates.PBS_DEFAULTED;
			else if ((state & ButtonState.Flat) != 0)
				ctlState = PushButtonStates.PBS_HOT;
			if ((state & ButtonState.Pushed) != 0)
				ctlState = PushButtonStates.PBS_PRESSED;

			Theme.DrawControl(graphics, "Button", x, y, width, height,
				(int)ButtonParts.BP_PUSHBUTTON, (int)ctlState);
		}


		/// <summary>
		/// Draws a check box control
		/// </summary>
		public override void DrawCheckBox(Graphics graphics, int x, int y,
			int width, int height, ButtonState state)
		{
			// revert to default theme if necessary
			if (!enableTheme) // && !FlatStyle.System
			{
				base.DrawCheckBox(graphics, x, y, width, height, state);
				return;
			}

			CheckBoxStates ctlState = CheckBoxStates.CBS_UNCHECKEDNORMAL;

			if ((state & ButtonState.Inactive) != 0)
			{
				ctlState = CheckBoxStates.CBS_UNCHECKEDDISABLED;

				if ((state & ButtonState.Checked) != 0)
					ctlState = CheckBoxStates.CBS_CHECKEDDISABLED;
			}
			else
			{
				if ((state & ButtonState.Checked) != 0)
					ctlState = CheckBoxStates.CBS_CHECKEDNORMAL;
			}

			Theme.DrawControl(graphics, "Button", x, y, width, height,
				(int)ButtonParts.BP_CHECKBOX, (int)ctlState);
		}


		/// <summary>
		/// Draws a radio button control
		/// </summary>
		public override void DrawRadioButton
			(Graphics graphics, int x, int y, int width, int height,
			ButtonState state, Color foreColor, Color backColor,
			Brush backgroundBrush)
		{
			// revert to default theme if necessary
			if (!enableTheme) // && !FlatStyle.System
			{
				base.DrawRadioButton(graphics, x, y, width, height,
					state, foreColor, backColor, backgroundBrush);
				return;
			}

			RadioButtonStates ctlState = RadioButtonStates.RBS_UNCHECKEDNORMAL;

			if ((state & ButtonState.Inactive) != 0)
			{
				ctlState = RadioButtonStates.RBS_UNCHECKEDDISABLED;

				if ((state & ButtonState.Checked) != 0)
					ctlState = RadioButtonStates.RBS_CHECKEDDISABLED;
			}
			else
			{
				if ((state & ButtonState.Checked) != 0)
					ctlState = RadioButtonStates.RBS_CHECKEDNORMAL;
			}

			Theme.DrawControl(graphics, "Button", x, y, width, height,
				(int)ButtonParts.BP_RADIOBUTTON, (int)ctlState);
		}


		/// <summary>
		/// Draws a group box control
		/// </summary>
		public override void DrawGroupBox
			(Graphics graphics, Rectangle bounds, Color foreColor,
			Color backColor, Brush backgroundBrush, bool enabled,
			bool entered, FlatStyle style, String text, Font font,
			StringFormat format)
		{
			// revert to default theme if necessary
			if (!enableTheme || style != FlatStyle.System)
			{
				base.DrawGroupBox(graphics, bounds, foreColor,
					backColor, backgroundBrush, enabled,
					entered, style, text, font, format);
				return;
			}

			// draw the groupbox rectangle
			Theme.DrawControl(graphics, "Button",
				bounds.X, bounds.Y + 8, bounds.Width, bounds.Height - 8,
				(int)ButtonParts.BP_GROUPBOX, 1);

			// this is only temporary,
			// DrawThemeParentBackground should be used instead
			bounds.X += 8;
			bounds.Width -= 16;

			Size textSize = Size.Ceiling(graphics.MeasureString(text, font,
				bounds.Width,
				format));

			Rectangle textbounds = new Rectangle(bounds.X - 2, bounds.Y,
				textSize.Width + 4, textSize.Height);

			graphics.FillRectangle(new SolidBrush(backColor), textbounds);

			graphics.DrawString(text, font, new SolidBrush(foreColor),
				(RectangleF)bounds, format);
		}


		/// <summary>
		/// Draws a combo box button - need to fix Combobox.cs
		/// </summary>
		public override void DrawComboButton(Graphics graphics, int x, int y,
			int width, int height, ButtonState state)
		{
			// revert to default theme if necessary
			if (!enableTheme) // && FlatStyle.System
			{
				base.DrawComboButton(graphics, x, y, width, height, state);
				return;
			}

			ComboBoxStates ctlState = ComboBoxStates.CBXS_NORMAL;

			if ((state & ButtonState.Flat) != 0)
				ctlState = ComboBoxStates.CBXS_HOT;
			if ((state & ButtonState.Pushed) != 0)
				ctlState = ComboBoxStates.CBXS_PRESSED;
				
			Theme.DrawControl(graphics, "ComboBox", x, y, width, height,
				(int)ComboBoxParts.CP_DROPDOWNBUTTON, (int)ctlState);
		}


		/// <summary>
		/// Draw a scroll bar control.
		/// TODO: draw vertical scrollbars!
		/// </summary>
		public override void DrawScrollBar
			(Graphics graphics, Rectangle bounds,
			Rectangle drawBounds,
			Color foreColor, Color backColor,
			bool vertical, bool enabled,
			Rectangle bar, Rectangle track,
			Rectangle decrement, bool decDown,
			Rectangle increment, bool incDown)
		{
			// revert to default theme if necessary
			if (!enableTheme) // && FlatStyle.System
			{
				base.DrawScrollBar(graphics, bounds, drawBounds,
					foreColor, backColor, vertical, enabled,
					bar, track,	decrement, decDown,	increment, incDown);
				return;
			}

			ScrollBarStates barState = ScrollBarStates.SCRBS_DISABLED;
			ArrowButtonStates decState = ArrowButtonStates.ABS_LEFTDISABLED;
			ArrowButtonStates incState = ArrowButtonStates.ABS_RIGHTDISABLED;

			if (enabled)
			{
				decState = (decDown ?
					ArrowButtonStates.ABS_LEFTPRESSED :
					ArrowButtonStates.ABS_LEFTNORMAL);

				incState = (incDown ?
					ArrowButtonStates.ABS_RIGHTPRESSED :
					ArrowButtonStates.ABS_RIGHTNORMAL);

				barState = ScrollBarStates.SCRBS_NORMAL;
			}

			Theme.DrawControl(graphics, "Scrollbar", track,
				(int)ScrollBarParts.SBP_UPPERTRACKHORZ, (int)barState);

			Theme.DrawControl(graphics, "Scrollbar", bar,
				(int)ScrollBarParts.SBP_THUMBBTNHORZ, (int)barState);

			Theme.DrawControl(graphics, "Scrollbar", bar,
				(int)ScrollBarParts.SBP_GRIPPERHORZ, (int)barState);

			Theme.DrawControl(graphics, "Scrollbar", decrement,
				(int)ScrollBarParts.SBP_ARROWBTN, (int)decState);

			Theme.DrawControl(graphics, "Scrollbar", increment,
				(int)ScrollBarParts.SBP_ARROWBTN, (int)incState);
		}


		/// <summary>
		/// Draws a progress bar
		/// </summary>
		public override void DrawProgressBar(Graphics graphics, int x, int y,
			int width, int height, 
			int steps, int step,
			int progressvalue, bool enabled)
		{
			// revert to default theme if necessary
			if (!enableTheme) // && FlatStyle.System
			{
				base.DrawProgressBar(graphics, x, y, width, height, steps, step, progressvalue, enabled);
				return;
			}

			// note: there's no such thing as disabled progress bar in uxtheme
			Theme.DrawControl(graphics, "Progress",
				x, y, width, height,
				(int)ProgressParts.PP_BAR, 1);

			int blockWidth = width/steps, blockHeight = height;

			for (int i = 0; i < steps; i++)
			{
				if((i*step) < progressvalue)
				{
					// temp. workaround - add 2 to width,
					// since there's not supposed to be any physical
					// breaks between blocks
					DrawBlock(graphics, x, y, 
						blockWidth + 2,
						blockHeight);
				}
				x += blockWidth;
			}
		}


		/// <summary>
		/// Draw a progress bar block
		/// </summary>
		private void DrawBlock(Graphics graphics,
			int x, int y, int width, int height)
		{
			Theme.DrawControl(graphics, "Progress",
				x, y, width, height,
				(int)ProgressParts.PP_CHUNK, 1);
		}


		/// <summary>
		/// Draws a focus rectangle
		/// </summary>
		public override void DrawFocusRectangle
			(Graphics graphics, Rectangle rectangle,
			Color foreColor, Color backColor)
		{
			// revert to default theme if necessary
			if (!enableTheme) // && FlatStyle.System
			{
				base.DrawFocusRectangle(graphics, rectangle, foreColor, backColor);
				return;
			}

			Pen pen = new Pen(foreColor, 1);
			pen.DashStyle = DashStyle.Dot;

			graphics.DrawRectangle(pen, rectangle.X - 1, rectangle.Y-1,
				rectangle.Width+1, rectangle.Height+1);

			pen.Dispose();
		}

	}; // class ThemePainterXP

}; // namespace System.Windows.Forms.Themes

 