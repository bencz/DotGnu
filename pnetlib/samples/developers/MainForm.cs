///
/// Copyright (c) 2004 Thong Nguyen (tum@veridicus.com)
///

using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;

[assembly:AssemblyVersion("0.0.0.1")]

namespace ThreadsDemo
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm
		: System.Windows.Forms.Form, DeveloperController
	{
		private System.ComponentModel.IContainer components;
		public virtual event EventHandler Start;
		private int m_NumberOfDevelopers;
		private int m_DefaultDeveloperWidth;
		private int m_DefaultDeveloperHeight;
		private System.Windows.Forms.Panel topPanel;
		private System.Windows.Forms.Panel mainPanel;
		private System.Windows.Forms.Label labelTitle;
		private System.Windows.Forms.Label labelDescription;
		public const double DegreesPerRadian = 360 / (2 * Math.PI);
		private double angleSpin = 0;
		private double angleSpinAdjust = 0.5;
		private System.Windows.Forms.Timer timer;
		private int m_CodingDelay = 1500, m_ThinkingDelay = 1500;
		private System.Windows.Forms.HScrollBar scrollSpinSpeed;
		Regex fileNameRegex = new Regex(@"^Developer[_](?<filename>(?<name>.*)\..*)$", RegexOptions.IgnoreCase);
		
		private void StartControllers()
		{
			if (Start != null)
			{
				Start(this, EventArgs.Empty);			
			}
		}

		public MainForm()
		{
			const string filename = "DotGNU_Logo.png";

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			LoadOptions();

			if (!CreateDevelopers())
			{
				Environment.Exit(1);
			}
			
			// Display the form so that handles are created.
			Show();
			
			BackColor = Color.White;

			try
			{
				// Load the DotGNU logo
				
				PictureBox picture = new PictureBox();

				picture.Image = Image.FromFile(filename);
				
				mainPanel.Controls.Add(picture);
				picture.SendToBack();
				picture.Anchor = AnchorStyles.None;
				picture.SizeMode = PictureBoxSizeMode.StretchImage;	
				picture.Width = 250;
				picture.Height = (int)(picture.Width * ((double)picture.Image.Height / (double)picture.Image.Width));

				picture.Left = (mainPanel.Width - picture.Width) / 2;
				picture.Top = (mainPanel.Height - picture.Height) / 2;				
			}
			catch (FileNotFoundException)
			{
#if CONFIG_SMALL_CONSOLE
				Console.WriteLine("Warning: Couldn't find {0}", filename);
#else
				Console.Error.WriteLine("Warning: Couldn't find {0}", filename);
#endif
			}

			// Layout the developers.
			mainPanel.Layout += new LayoutEventHandler(DeveloperViews_Layout);			
			mainPanel.PerformLayout();

			scrollSpinSpeed.Value = scrollSpinSpeed.Maximum / 2;
			UpdateSpin();
			
			StartControllers();
		}
				
		/// <summary>
		/// Load up the command line options.
		/// </summary>
		private void LoadOptions()
		{
			string[] args = Environment.GetCommandLineArgs();

			for (int i = 1; i < args.Length; i++)
			{
				switch (args[i])
				{
					case "-codingdelay":
					case "--codingdelay":

						i++;

						if (i >= args.Length)
						{
#if CONFIG_SMALL_CONSOLE
							Console.Write("Missing value for coding delay.");
#else
							Console.Error.Write("Missing value for coding delay.");
#endif
						}

						m_CodingDelay = int.Parse(args[i]);

						break;

					case "-thinkingdelay":
					case "--thinkingdelay":

						i++;

						if (i >= args.Length)
						{
#if CONFIG_SMALL_CONSOLE
							Console.Write("Missing value for coding delay.");
#else
							Console.Error.Write("Missing value for coding delay.");
#endif
						}

						m_ThinkingDelay = int.Parse(args[i]);

						break;

					case "-help":
					case "--help":

						ShowHelp();
						Environment.Exit(0);

						break;

					default:

						Console.WriteLine("Unknown option {0}\n", args[i]);

						break;
				}
			}
		}

		/// <summary>
		/// Show help.
		/// </summary>
		private void ShowHelp()
		{
			AssemblyName name = GetType().Assembly.GetName();

			Console.WriteLine("Coding Developers ({0}) {1}", GetType().Module.Name, name.Version);
			Console.WriteLine();
			Console.WriteLine("This program is an implementation of the \"dining Developers\" problem and");
			Console.WriteLine("is used to to test and demonstrate Portable.NET's Threading and UI libraries");
			Console.WriteLine("as well as attach faces to names of some DotGNU developers.");
			Console.WriteLine();
			Console.WriteLine("Usage:");
			Console.WriteLine();
			Console.WriteLine("--codingdelay [delay]    Default amount of time a developer spends coding");
			Console.WriteLine("--thinkingdelay [delay]  Default amount of time a developer spends thinking");
			Console.WriteLine("--help                   Display this help text");
			Console.WriteLine();
			Console.WriteLine("Coddev will pick up developer image files from the current directory.  The");
			Console.WriteLine("pictures must use the naming pattern: Developer_<name>.*");
			Console.WriteLine();
		}


		/// <summary>
		/// Callback for the timer that makes the developers spin in a circle.
		/// </summary>
		private void Timer_Tick(object sender, System.EventArgs e)
		{
			angleSpin = (angleSpin + angleSpinAdjust) % 360;
			mainPanel.PerformLayout();
		}

		/// <summary>
		/// Callback for when the spin speed scrollbar changes..
		/// </summary>
		private void ScrollSpinSpeed_ValueChanged(object sender, System.EventArgs e)
		{
			UpdateSpin();
		}

		/// <summary>
		/// Update he spin speed and angle adjust based on the spin speed scrollbar value.
		/// </summary>
		private void UpdateSpin()
		{
			int value;
			int max, middle;

			max = scrollSpinSpeed.Maximum - scrollSpinSpeed.LargeChange;

			value = scrollSpinSpeed.Value ;
			
			if (value > max / 2)
			{
				value = (max / 2) - (value - (max / 2));
				angleSpinAdjust = 0.75;
			}
			else
			{
				angleSpinAdjust = -0.75;
			}
			
			if (value <= 0)
			{
				value = 1;
			}
			
			value = (int)(Math.Log(value, 2) * 3);

			if (value == 0)
			{
				value = 1;
			}

			middle = max / 2;

			if (scrollSpinSpeed.Value < middle - scrollSpinSpeed.LargeChange
				|| scrollSpinSpeed.Value > middle + scrollSpinSpeed.LargeChange)
			{
				timer.Interval = value;				
				timer.Enabled = true;
			}
			else
			{
				timer.Enabled = false;
			}
		}

		/// <summary>
		/// Layout the developers in an ellipse formation.
		/// </summary>
		private void DeveloperViews_Layout(object sender, LayoutEventArgs e)
		{			
			DeveloperView view;			
			int x, y, centrex, centrey;
			double radiusx, radiusy, angle, angleDelta;

			Control panel = sender as Control;

			if (m_NumberOfDevelopers == 0)
			{
				return;
			}

			angle = 0;
			
			centrex = mainPanel.ClientRectangle.Width / 2;
			centrey = mainPanel.ClientRectangle.Height / 2;

			radiusx = centrex - m_DefaultDeveloperWidth / 2;
			radiusy = centrey - m_DefaultDeveloperHeight / 2;
			
			angle = -90 + angleSpin;
			angleDelta = 360 / m_NumberOfDevelopers;
			
			foreach (Control ctl in panel.Controls)
			{
				view = ctl as DeveloperView;

				if (view != null)
				{
					x = (int)(radiusx * Math.Cos(angle / DegreesPerRadian ) + centrex - (view.Width / 2));
					y = (int)(radiusy * Math.Sin(angle / DegreesPerRadian) + centrey) - (view.Height / 2);

					view.Left = x;
					view.Top = y;

					angle += angleDelta;
				}
			}
		}

		/// <summary>
		/// Create the developers.
		/// </summary>
		private bool CreateDevelopers()
		{
			int x;			
			FileInfo[] files;
			DeveloperView view;			
			DeveloperFactory factory;
			Random random = new Random();
			Developer first, prev, developer;
			DirectoryInfo dirInfo = new DirectoryInfo(Environment.CurrentDirectory);
			
			factory = DeveloperFactory.GetFactory();
			
			prev = first = developer = null;
			
			files = dirInfo.GetFiles();

			for (int i = 0; i < files.Length; i++)
			{
				FileInfo fileinfo;

				x = random.Next(i, files.Length - 1);

				fileinfo = files[i];
				files[i] = files[x];
				files[x] = fileinfo;
			}

			for (int i = 0; i < files.Length; i++)
			{				
				Match match;
				string filename, name;

				name = files[i].Name;

				match = fileNameRegex.Match(name);
				
				if (match.Length == 0)
				{
					continue;
				}

				filename = match.Groups["filename"].Value;
				name = match.Groups["name"].Value;


				m_NumberOfDevelopers++;

				Console.WriteLine("Found " + files[i].Name);

				developer = factory.NewDeveloper(name, prev, null, this);
				developer.CodingDelay = m_CodingDelay;
				developer.ThinkingDelay = m_ThinkingDelay;
				
				if (prev != null)
				{
					prev.Right = developer;
				}

				if (m_NumberOfDevelopers == 1)
				{
					first = developer;
				}

				view = new DeveloperView(developer, files[i].FullName);
				
				mainPanel.Controls.Add(view);

				m_DefaultDeveloperWidth = view.Width;
				m_DefaultDeveloperHeight = view.Height;

				prev = developer;

			}
			
			if (m_NumberOfDevelopers < 1)
			{
				string message = "No developer pictures found.";

#if CONFIG_SMALL_CONSOLE
				Console.WriteLine(message);
#else
				Console.Error.WriteLine(message);
#endif
				MessageBox.Show(message);

				Application.Exit();

				return false;
			}

			first.Left = developer;
			developer.Right = first;

			return true;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region InitializeComponent
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.topPanel = new System.Windows.Forms.Panel();
			this.scrollSpinSpeed = new System.Windows.Forms.HScrollBar();
			this.labelDescription = new System.Windows.Forms.Label();
			this.labelTitle = new System.Windows.Forms.Label();
			this.mainPanel = new System.Windows.Forms.Panel();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.topPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// topPanel
			// 
			this.topPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.topPanel.Controls.Add(this.scrollSpinSpeed);
			this.topPanel.Controls.Add(this.labelDescription);
			this.topPanel.Controls.Add(this.labelTitle);
			this.topPanel.Location = new System.Drawing.Point(8, 8);
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(976, 112);
			this.topPanel.TabIndex = 0;
			// 
			// scrollSpinSpeed
			// 
			this.scrollSpinSpeed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.scrollSpinSpeed.Location = new System.Drawing.Point(216, 72);
			this.scrollSpinSpeed.Maximum = 200;
			this.scrollSpinSpeed.Name = "scrollSpinSpeed";
			this.scrollSpinSpeed.Size = new System.Drawing.Size(528, 24);
			this.scrollSpinSpeed.TabIndex = 2;
			this.scrollSpinSpeed.Value = 100;
			this.scrollSpinSpeed.ValueChanged += new System.EventHandler(this.ScrollSpinSpeed_ValueChanged);
			// 
			// labelDescription
			// 
			this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelDescription.Location = new System.Drawing.Point(8, 32);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(960, 32);
			this.labelDescription.TabIndex = 1;
			this.labelDescription.Text = "System.Threading, System.Threading.Monitor and System.Windows.Forms demo (based on the Dini" +
				"ng Philosophers problem)";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelTitle
			// 
			this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.labelTitle.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelTitle.Location = new System.Drawing.Point(16, 8);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(952, 24);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "DotGNU \"Coding Developers\"";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// mainPanel
			// 
			this.mainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mainPanel.Location = new System.Drawing.Point(8, 128);
			this.mainPanel.Name = "mainPanel";
			this.mainPanel.Size = new System.Drawing.Size(976, 648);
			this.mainPanel.TabIndex = 0;
			// 
			// timer
			// 
			this.timer.Interval = 10;
			this.timer.Tick += new System.EventHandler(this.Timer_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(992, 792);
			this.Controls.Add(this.mainPanel);
			this.Controls.Add(this.topPanel);
			this.Name = "MainForm";
			this.Text = "DotGNU \"Coding Developers\"";
			this.topPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		[STAThread]
		public static void Main()
		{
			Application.Run(new MainForm());
		}
	}
}
