///
/// Copyright (c) 2004 Thong Nguyen (tum@veridicus.com)
///

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ThreadsDemo
{
	/// <summary>
	/// UI view of an abstract developer.
	/// </summary>
	public class DeveloperView : System.Windows.Forms.UserControl
	{		
		private System.Windows.Forms.Label labelState;
		private System.Windows.Forms.NumericUpDown updnCodingDelay;
		private System.Windows.Forms.Label labelCodingDelay;
		private System.Windows.Forms.Label labelThinkingDelay;
		private System.Windows.Forms.NumericUpDown updnThinkingDelay;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.CheckBox checkRandomDelays;
		private Developer m_Developer;
		
		public DeveloperView(Developer philosopher, string imagePath)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			m_Developer = philosopher;
			
			m_Developer.StateChanged += new EventHandler(DeveloperStateChanged);
			updnCodingDelay.Minimum = 0;
			updnCodingDelay.Maximum = 10000;
			updnCodingDelay.Increment = 100;
			updnThinkingDelay.Minimum = 0;
			updnThinkingDelay.Maximum = 10000;
			updnThinkingDelay.Increment = 100;
			
			updnCodingDelay.Value = m_Developer.CodingDelay;
			updnThinkingDelay.Value  = m_Developer.ThinkingDelay;
			checkRandomDelays.Checked = m_Developer.UseRandomDelay;
			pictureBox.Image = Image.FromFile(imagePath);
		}

		private void DisplayState()
		{
			labelState.Text = m_Developer.ToString();

			switch (m_Developer.State)
			{
				case DeveloperState.Coding:
					labelState.BackColor = Color.Green;
					break;
				case DeveloperState.Thinking:
					labelState.BackColor = Color.Orange;
					break;
				case DeveloperState.Waiting:
					labelState.BackColor = Color.Red;
					break;
			}
		}

		private void DeveloperStateChanged(object sender, EventArgs eventArgs)
		{
			DisplayState();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			base.Dispose( disposing );
		}

		#region InitializeComponent
		private void InitializeComponent()
		{
			this.labelState = new System.Windows.Forms.Label();
			this.updnThinkingDelay = new System.Windows.Forms.NumericUpDown();
			this.updnCodingDelay = new System.Windows.Forms.NumericUpDown();
			this.labelCodingDelay = new System.Windows.Forms.Label();
			this.labelThinkingDelay = new System.Windows.Forms.Label();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.checkRandomDelays = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.updnThinkingDelay)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.updnCodingDelay)).BeginInit();
			this.SuspendLayout();
			// 
			// labelState
			// 
			this.labelState.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.labelState.Location = new System.Drawing.Point(4, 104);
			this.labelState.Name = "labelState";
			this.labelState.Size = new System.Drawing.Size(116, 32);
			this.labelState.TabIndex = 0;
			this.labelState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// updnThinkingDelay
			// 
			this.updnThinkingDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.updnThinkingDelay.Location = new System.Drawing.Point(72, 160);
			this.updnThinkingDelay.Name = "updnThinkingDelay";
			this.updnThinkingDelay.Size = new System.Drawing.Size(48, 18);
			this.updnThinkingDelay.TabIndex = 1;
			this.updnThinkingDelay.ValueChanged += new System.EventHandler(this.ThinkingDelay_ValueChanged);
			// 
			// updnCodingDelay
			// 
			this.updnCodingDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.updnCodingDelay.Location = new System.Drawing.Point(72, 184);
			this.updnCodingDelay.Name = "updnCodingDelay";
			this.updnCodingDelay.Size = new System.Drawing.Size(48, 18);
			this.updnCodingDelay.TabIndex = 2;
			this.updnCodingDelay.ValueChanged += new System.EventHandler(this.CodingDelay_ValueChanged);
			// 
			// labelCodingDelay
			// 
			this.labelCodingDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelCodingDelay.Location = new System.Drawing.Point(0, 184);
			this.labelCodingDelay.Name = "labelCodingDelay";
			this.labelCodingDelay.Size = new System.Drawing.Size(72, 16);
			this.labelCodingDelay.TabIndex = 3;
			this.labelCodingDelay.Text = "Coding Delay";
			this.labelCodingDelay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelThinkingDelay
			// 
			this.labelThinkingDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelThinkingDelay.Location = new System.Drawing.Point(0, 160);
			this.labelThinkingDelay.Name = "labelThinkingDelay";
			this.labelThinkingDelay.Size = new System.Drawing.Size(64, 16);
			this.labelThinkingDelay.TabIndex = 4;
			this.labelThinkingDelay.Text = "Think Delay";
			this.labelThinkingDelay.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox
			// 
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(12, 4);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(96, 96);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.pictureBox.TabIndex = 5;
			this.pictureBox.TabStop = false;
			// 
			// checkRandomDelays
			// 
			this.checkRandomDelays.Location = new System.Drawing.Point(8, 140);
			this.checkRandomDelays.Name = "checkRandomDelays";
			this.checkRandomDelays.Size = new System.Drawing.Size(108, 16);
			this.checkRandomDelays.TabIndex = 6;
			this.checkRandomDelays.Text = "Random Delays";
			this.checkRandomDelays.CheckedChanged += new System.EventHandler(this.CheckRandomDelays_CheckedChanged);
			// 
			// DeveloperView
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkRandomDelays);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.labelThinkingDelay);
			this.Controls.Add(this.labelCodingDelay);
			this.Controls.Add(this.updnCodingDelay);
			this.Controls.Add(this.updnThinkingDelay);
			this.Controls.Add(this.labelState);
			this.Name = "DeveloperView";
			this.Size = new System.Drawing.Size(128, 204);
			((System.ComponentModel.ISupportInitialize)(this.updnThinkingDelay)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.updnCodingDelay)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void CodingDelay_ValueChanged(object sender, System.EventArgs e)
		{
			m_Developer.CodingDelay = (int)updnCodingDelay.Value;
		}

		private void ThinkingDelay_ValueChanged(object sender, System.EventArgs e)
		{
			m_Developer.ThinkingDelay = (int)updnThinkingDelay.Value;
		}

		private void CheckRandomDelays_CheckedChanged(object sender, System.EventArgs e)
		{
			m_Developer.UseRandomDelay = checkRandomDelays.Checked;
		}
	}
}
