using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Text;
using System.Resources;
using System.Collections;

namespace FormsTest
{
	public class FormsTest : Form
	{
		#region declares
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private TabPage tabPage3;
		private TabPage tabPage4;
		private TabPage tabPage5;
		private TabPage tabPage6;
		private TabPage tabPage7;
		private TabPage tabPage8;
		private TabPage tabPage9;
		private TabPage tabPage10;
		private TabPage tabPage11;
		private TabPage tabPage12;
		private TabPage tabPage13;
		private TabPage tabPage14;
		private TabPage tabPage15;
		private TabPage tabPage16;
		private TabPage tabPage17;
		private TabPage tabPage18;
		private TabPage tabPage19;
		private TabPage tabPage20;
		private TabPage tabPage21;
		private TabPage tabPage22;
		private TabPage tabPage23;
		private TabPage tabPage24;
		private TabPage tabPage25;
		private TabPage tabPage26;
		private TabPage tabPage27;
		private TabPage tabPage28;
		private TabPage tabPage29;
		private TabPage tabPage30;
		private TabPage tabPage31;
		private StatusBar statusBar;

		// Tab1 Labels Test
		private Label label;
		private Label label2;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label label7;
		private Label label8;
		private Label label9;
		private Label label10;
		private Label label11;
		private Label label12;
		private Label label13;
		private Label label14;
		private Label label15;
		private Label label16;
		private Label label17;
		private Label label18;
		private Label label19;
		private Label label20;
		private Label label21;
		private Label label22;
		private Label label23;
		private Label label24;
		private Label label25;

		// Tab2 Button Test
		private Button button;
		private Button button2;
		private Button button3;
		private Button button4;
		private Button button5;
		private Button button6;
		private Button button7;
		private Button button12;
		private Button button13;
		private Button button14;
		private Button button15;
		private Button button16;
		private Button button17;
		private Button button18;
		private Button button19;
		private Button button20;
		private Button button21;
		private Button button22;
		private Button button23;

		// Tab3 TextBox Test
		private TextBox textBox;
		private TextBox textBox1;
		private TextBox textBox2;
		private TextBox textBox3;
		private TextBox textBox4;
		private TextBox textBox5;
		private TextBox textBox6;
		private TextBox textBox7;
		private TextBox textBox8;
		private TextBox textBox9;
		private TextBox textBox10;
		private TextBox textBox14;
		private TextBox textBox11;
		private TextBox textBox12;
		private TextBox textBox13;
		private TextBox textBox15;
		private TextBox textBox16;
		private TextBox textBox17;
		private TextBox textBox18;
		private TextBox textBox19;
		private TextBox textBox20;
		private Button textBoxLinesButton;
		private Button textBoxTextButton;
		private Button textBoxSelectedTextButton;

		// Tab4 RadioButtons Test
		private RadioButton radioButton;
		private RadioButton radioButton2;
		private RadioButton radioButton3;
		private RadioButton radioButton4;
		private RadioButton radioButton5;
		private RadioButton radioButton6;
		private RadioButton radioButton7;
		private RadioButton radioButton8;
		private RadioButton radioButton9;
		private RadioButton radioButton10;
		private RadioButton radioButton11;
		private RadioButton radioButton12;
		private RadioButton radioButton13;
		private RadioButton radioButton14;
		private RadioButton radioButton15;
		private RadioButton radioButton16;
		private RadioButton radioButton17;
		private RadioButton radioButton18;
		private RadioButton radioButton19;
		private RadioButton radioButton20;
		private RadioButton radioButton21;
		private RadioButton radioButton22;
		private RadioButton radioButton23;
		private RadioButton radioButton24;
		private RadioButton radioButton25;
		private RadioButton radioButton26;
		private RadioButton radioButton27;
		private RadioButton radioButton28;
		private RadioButton radioButton29;
		private RadioButton radioButton30;
		private RadioButton radioButton31;
		private RadioButton radioButton32;
		private RadioButton radioButton33;

		//Tab6 TabTest
		private TabPage tabPageT1;
		private TabPage tabPageT2;
		private TabPage tabPageT3;
		private TabPage tabPageT4;
		private TabPage tabPageT5;
		private TabControl tabControlT2;
		private TabPage tabPageT6;
		private TabControl tabControlT3;
		private TabPage tabPageT7;
		private TabPage tabPageT8;
		private Button buttonT1;
		private Label labelT1;
		private Label labelT2;
		private TabPage tabPageT9;
		private TabPage tabPageT10;
		private TabControl tabControlT4;
		private TabPage tabPageT11;
		private TabPage tabPageT12;
		private TabPage tabPageT13;
		private TabPage tabPageT14;
		private TabPage tabPageT15;
		private TabPage tabPageT16;
		private TabPage tabPageT17;
		private TabPage tabPageT18;
		private TabPage tabPageT19;
		private TabPage tabPageT20;
		private TabPage tabPageT21;
		private TabPage tabPageT22;
		private TabPage tabPageT23;
		private TabPage tabPageT24;
		private TabPage tabPageT25;
		private TabControl tabControlT5;
		private TabPage tabPageT26;
		private TabPage tabPageT27;
		private TabPage tabPageT28;
		private TabControl tabControlT6;
		private TabPage tabPageT29;
		private TabPage tabPageT30;
		private TabPage tabPageT31;
		private TabControl tabControlT7;
		private TabPage tabPageT32;
		private TabPage tabPageT33;
		private TabControl Docked;
		private TabPage tabPageT34;
		private TabControl tabControlT1;
		private TabPage tabPageT35;
		private TabPage tabPageT36;
		private TabControl tabControlT8;
		private TabPage tabPageT37;
		private TabPage tabPageT38;
		private Label labelT3;
		private Label labelT4;

		private MainMenu mainMenu;
		private MenuItem fileMenuItem;
		private MenuItem editMenuItem;
		private MenuItem helpMenuItem;
		private MenuItem newMenuItem;
		private MenuItem openMenuItem;
		private MenuItem exitMenuItem;
		private MenuItem thisMenuItem, thatMenuItem, otherMenuItem, otherAMenuItem, otherBMenuItem, otherCMenuItem, cutMenuItem, copyMenuItem, pasteMenuItem, aboutMenuItem, seperatorMenuItem;
		private ContextMenu contextMenu;
		private Label contextMenuLabel1;

		private Image imageOld;
		private Image imageNew;

		private ComboBox comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7;

		private TreeView treeView1;
		private ImageList treeImageList;
		private Label treeLabelBoundsDescription;
		private Label treeLabelBounds;
		private Label treeLabelCheckBox;
		private Label treeLabelRightClickDescription;
		private Button treeButtonAddNodes;
		private CheckBox treeCheckBox;
		private ContextMenu treeMenu;

		private ListBox listBox1, listBox2;

		private Button formsButton1;
		private Button formsButton2;
		private Button formsButton3;
		private Button formsButton4;
		private Button formsButton5;
		private Button formsButton6;

		private VScrollBar vScrollBar;
		private HScrollBar hScrollBar;
		private Label scroll1LabelMin, scroll1LabelMax, scroll1LabelValue, scroll1LabelLarge, scroll1LabelSmall;
		private Label scroll2LabelMin, scroll2LabelMax, scroll2LabelValue, scroll2LabelLarge, scroll2LabelSmall;
		private TextBox scroll1TextBoxMin, scroll1TextBoxMax, scroll1TextBoxValue, scroll1TextBoxLarge, scroll1TextBoxSmall;
		private TextBox scroll2TextBoxMin, scroll2TextBoxMax, scroll2TextBoxValue, scroll2TextBoxLarge, scroll2TextBoxSmall;

		private TrackBar vTrackBar;
		private TrackBar hTrackBar;
		private Label track1LabelMin, track1LabelMax, track1LabelValue, track1LabelLarge, track1LabelSmall;
		private Label track2LabelMin, track2LabelMax, track2LabelValue, track2LabelLarge, track2LabelSmall;
		private TextBox track1TextBoxMin, track1TextBoxMax, track1TextBoxValue, track1TextBoxLarge, track1TextBoxSmall;
		private TextBox track2TextBoxMin, track2TextBoxMax, track2TextBoxValue, track2TextBoxLarge, track2TextBoxSmall;

		private TextBox textBoxTest2a, textBoxTest2b;

		private PictureBox pictureBox1;
		private PictureBox pictureBox2;
		private PictureBox pictureBox3;
		private PictureBox pictureBox4;
		private PictureBox pictureBox5;

		private Button buttonResXRead;
		private Button buttonResXWrite;
		private TextBox textBoxResXData;

		private ImageList imageList1;
		private Button buttonImageListRead;
		private Button buttonImageListWrite;
		private Button buttonImageListSet;
		private Label labelImageListSize;
		private TextBox textBoxImageListSize;
		private Label labelImageListColorDepth;
		private TextBox textBoxImageListColorDepth;

		private  Button buttonImageLoad24bpp;
		private  Button buttonImageLoad15bpp;
		private  Button buttonImageLoad16bpp;
		private  Button buttonImageLoad8bpp;
		private  Button buttonImageLoad4bpp;
		private  Button buttonImageLoad1bpp;
		private  Button buttonImageLoad32bppIcon;
		private  Button buttonImageConvert24bpp;
		private  Button buttonImageConvert16bpp;
		private  Button buttonImageConvert15bpp;
		private  Button buttonImageConvert8bpp;
		private  Button buttonImageConvert4bpp;
		private  Button buttonImageConvert1bpp;
		private Label labelImageWidth;
		private TextBox textBoxImageWidth;
		private Label labelImageHeight;
		private TextBox textBoxImageHeight;
		private Label labelImageFileName;
		private TextBox textBoxImageFileName;
		private Button buttonImageSave;

		private Timer timerTransform;
		private PointF[] transformTestPoints;
		private int transformRotation = 0;
		private int transformRotationOffSet = 5;
		private int transformX = 100;
		private int transformXOffset = -1;
		private int transformY = 100;
		private int transformYOffset = 1;
		private float transformScaleX = 1F;
		private float transformScaleXOffset = 0.03F;
		private float transformScaleY = 1F;
		private float transformScaleYOffset = 0.03F;

		//private PropertyGrid propertyGrid;

		private Button messageBox1;
		private Button messageBox2;
		private Button messageBox3;
		private Button messageBox4;

		private Button dialog1Button;
		private OpenFileDialog openFileDialog1;

		// Tab27 UpDown Test
		private DomainUpDown upDown1;
		private DomainUpDown upDown2;
		private DomainUpDown upDown1ro;
		private DomainUpDown upDown2ro;
		private string upDownString1 = "Test 1";
		private string upDownString2 = "Test 2";
		private string upDownString3 = "Test 3";
		private string upDownString4 = "Test 4";
		private NumericUpDown upDown3;
		private NumericUpDown upDown4;
		private NumericUpDown upDown3ro;
		private NumericUpDown upDown4ro;

		// tabPage29 Datagrid
		private DataGrid dataGrid;
		#endregion

		public static void Main(String[] args)
		{
			FormsTest form = new FormsTest();
			Application.Run(form);
		}

		public FormsTest()
		{
			ClientSize = new Size(500, 650);
			Text = "System.Windows.Forms Tests";
			HelpButton = true;
			HelpRequested += new HelpEventHandler(ShowHelp);
			Icon = new Icon(typeof(FormsTest), "dotgnu.ico");

			SuspendLayout();
			tabControl1 = new TabControl();
			tabControl1.Size = new Size(500, 628);
			tabControl1.Multiline = true;
			tabControl1.SizeMode = TabSizeMode.FillToRight;
			tabControl1.Name = "Main Tab";

			tabPage12 = new TabPage();
			tabPage12.Text = "TreeView";
			tabControl1.Controls.Add(tabPage12);
			tabPage19 = new TabPage();
			tabPage19.Text = "PropertyGrid";
			tabControl1.Controls.Add(tabPage19);
			tabPage26 = new TabPage();
			tabPage26.Text = "MessageBox";
			tabControl1.Controls.Add(tabPage26);
			tabPage17 = new TabPage();
			tabPage17.Text = "DrawString";
			tabControl1.Controls.Add(tabPage17);
			tabPage11 = new TabPage();
			tabPage11.Text = "ComboBox";
			tabControl1.Controls.Add(tabPage11);
			tabPage10 = new TabPage();
			tabPage10.Text = "Image";
			tabControl1.Controls.Add(tabPage10);
			tabPage25 = new TabPage();
			tabPage25.Text = "ImageList";
			tabControl1.Controls.Add(tabPage25);
			tabPage24 = new TabPage();
			tabPage24.Text = "ResX";
			tabControl1.Controls.Add(tabPage24);
			tabPage23 = new TabPage();
			tabPage23.Text = "ControlPaint 2";
			tabControl1.Controls.Add(tabPage23);
			tabPage22 = new TabPage();
			tabPage22.Text = "ControlPaint";
			tabControl1.Controls.Add(tabPage22);
			tabPage21 = new TabPage();
			tabPage21.Text = "PictureBox";
			tabControl1.Controls.Add(tabPage21);
			tabPage20 = new TabPage();
			tabPage20.Text = "TextBox 2";
			tabControl1.Controls.Add(tabPage20);
			tabPage3 = new TabPage();
			tabPage3.Text = "TextBox";
			tabControl1.Controls.Add(tabPage3);
			tabPage18 = new TabPage();
			tabPage18.Text = "ScrollBar";
			tabControl1.Controls.Add(tabPage18);
			tabPage15 = new TabPage();
			tabPage15.Text = "Transform";
			tabControl1.Controls.Add(tabPage15);
			tabPage13 = new TabPage();
			tabPage13.Text = "ListBox";
			tabControl1.Controls.Add(tabPage13);
			tabPage1 = new TabPage();
			tabPage1.Text = "Label";
			tabControl1.Controls.Add(tabPage1);
			tabControl1.SelectedIndex = 0;
			tabPage2 = new TabPage();
			tabPage2.Text = "Button";
			tabControl1.Controls.Add(tabPage2);
			tabPage4 = new TabPage();
			tabPage4.Text = "RadioButton";
			tabControl1.Controls.Add(tabPage4);
			tabPage5 = new TabPage();
			tabPage5.Text = "Region";
			tabControl1.Controls.Add(tabPage5);
			tabPage6 = new TabPage();
			tabPage6.Text = "TabControl";
			tabControl1.Controls.Add(tabPage6);
			tabPage7 = new TabPage();
			tabPage7.Text = "Primitives";
			tabControl1.Controls.Add(tabPage7);
			tabPage14 = new TabPage();
			tabPage14.Text = "Form";
			tabControl1.Controls.Add(tabPage14);
			tabPage8 = new TabPage();
			tabPage8.Text = "Graphics";
			tabControl1.Controls.Add(tabPage8);
			tabPage16 = new TabPage();
			tabPage16.Text = "Path";
			tabControl1.Controls.Add(tabPage16);
			tabPage9 = new TabPage();
			tabPage9.Text = "ContextMenu";
			tabControl1.Controls.Add(tabPage9);
			tabPage27 = new TabPage();
			tabPage27.Text = "UpDown";
			tabControl1.Controls.Add(tabPage27);
			tabPage28 = new TabPage();
			tabPage28.Text = "TrackBar";
			tabControl1.Controls.Add(tabPage28);
			tabPage29 = new TabPage();
			tabPage29.Text = "DataGrid";
			tabControl1.Controls.Add(tabPage29);
			tabPage30 = new TabPage();
			tabPage30.Text = "Path Tiger";
			tabControl1.Controls.Add(tabPage30);
			tabPage31 = new TabPage();
			tabPage31.Text = "CheckedListBox";
			tabControl1.Controls.Add(tabPage31);

			statusBar = new StatusBar();
			statusBar.Dock = DockStyle.Bottom;
			statusBar.Name = "statusBar";
			statusBar.Text = "FormsTest Ready";
			statusBar.Location = new Point(0, 628);
			statusBar.Size = new Size(500, 22);

			Controls.Add(statusBar);
			Controls.Add(tabControl1);

			AddTreeViewTest(tabPage12);
			AddLabelTest(tabPage1);
			AddButtonTest(tabPage2);
			AddTextBoxTest(tabPage3);
			AddTextBoxTest2(tabPage20);
			AddRadioButtonsTest(tabPage4);
			AddRegionsTest(tabPage5);
			AddTabControlsTest(tabPage6);
			AddPrimitivesTest(tabPage7);
			AddMenuTest();
			AddGraphicsTest(tabPage8);
			AddGraphicsPathTest(tabPage16);
			AddGraphicsDrawStringTest(tabPage17);
			AddContextTest(tabPage9);
			AddImageTest(tabPage10);
			AddComboTest(tabPage11);
			AddListBoxTest(tabPage13);
			AddFormsTest(tabPage14);
			//TransformsTest chews too much CPU - remove for now - Rhys.
			//AddTransformsTest(tabPage15);
			AddScrollbarTest(tabPage18);
			AddPropertyGridTest(tabPage19);
			AddPictureBoxTest(tabPage21);
			AddControlPaintTest(tabPage22);
			AddControlPaintTest2(tabPage23);
			AddResXTest(tabPage24);
			AddImageListTest(tabPage25);
			AddMessageBoxTest(tabPage26);
			AddUpDownTest(tabPage27);
			AddTrackbarTest(tabPage28);
			AddDataGrid(tabPage29);
			AddTiger(tabPage30);
			AddCheckedListBox(tabPage31);

			// Add the events here after the controls have been added
			// to the pages, otherwise the events will not be raised
			tabControl1.Click += new System.EventHandler(this.UpdateStatusBar);

			ResumeLayout(false);
			MinimumSize = new Size(300, 300);
		}

		private void AddLabelTest(Control control)
		{
			label22 = new Label();
			label23 = new Label();
			label8 = new Label();
			label9 = new Label();
			label4 = new Label();
			label5 = new Label();
			label6 = new Label();
			label7 = new Label();
			label2 = new Label();
			label3 = new Label();
			label15 = new Label();
			label14 = new Label();
			label17 = new Label();
			label16 = new Label();
			label11 = new Label();
			label10 = new Label();
			label13 = new Label();
			label20 = new Label();
			label21 = new Label();
			label24 = new Label();
			label25 = new Label();
			label19 = new Label();
			label18 = new Label();
			label12 = new Label();
			label = new Label();

			label22.Dock = DockStyle.Top;
			label22.Location = new Point(10, 0);
			label22.Name = "label22";
			label22.Width = 465;
			label22.TabIndex = 21;
			label22.Text = "A normal label : Dock=Top";

			label.Location = new Point(10, 25);
			label.Name = "label";
			label.Size = new Size(465, 16);
			label.TabIndex = 0;
			label.Text = "A normal label";

			label2.BorderStyle = BorderStyle.FixedSingle;
			label2.Location = new Point(10, 50);
			label2.Name = "label2";
			label2.Width = 465;
			label2.TabIndex = 1;
			label2.Text = "A normal label : BorderStyle=FixedSingle";

			label3.BorderStyle = BorderStyle.Fixed3D;
			label3.Location = new Point(10, 75);
			label3.Name = "label3";
			label3.Width = 465;
			label3.TabIndex = 2;
			label3.Text = "A normal label : BorderStyle=Fixed3D";

			label4.Location = new Point(10, 100);
			label4.Name = "label4";
			label4.Width = 465;
			label4.TabIndex = 3;
			label4.Text = "A normal label : TextAlign=TopCenter";
			label4.TextAlign = ContentAlignment.TopCenter;

			label7.Location = new Point(10, 125);
			label7.Name = "label7";
			label7.Width = 465;
			label7.TabIndex = 6;
			label7.Text = "A normal label : TextAlign=TopRight";
			label7.TextAlign = ContentAlignment.TopRight;

			label8.Location = new Point(10, 150);
			label8.Name = "label8";
			label8.Width = 465;
			label8.TabIndex = 7;
			label8.Text = "A normal label : TextAlign=MiddleLeft";
			label8.TextAlign = ContentAlignment.MiddleLeft;

			label5.Location = new Point(10, 175);
			label5.Name = "label5";
			label5.Width = 465;
			label5.TabIndex = 4;
			label5.Text = "A normal label : TextAlign=MiddleCenter";
			label5.TextAlign = ContentAlignment.MiddleCenter;

			label9.Location = new Point(10, 200);
			label9.Name = "label9";
			label9.Width = 465;
			label9.TabIndex = 8;
			label9.Text = "A normal label : TextAlign=BottomLeft";
			label9.TextAlign = ContentAlignment.BottomLeft;

			label6.Location = new Point(10, 225);
			label6.Name = "label6";
			label6.Width = 465;
			label6.TabIndex = 5;
			label6.Text = "A normal label : TextAlign=MiddleRight";
			label6.TextAlign = ContentAlignment.MiddleRight;

			label10.Location = new Point(10, 250);
			label10.Name = "label10";
			label10.Size = new Size(465, 16);
			label10.TabIndex = 9;
			label10.Text = "A normal label : TextAlign=BottomCenter";
			label10.TextAlign = ContentAlignment.BottomCenter;

			label11.Location = new Point(10, 275);
			label11.Name = "label11";
			label11.Size = new Size(465, 16);
			label11.TabIndex = 10;
			label11.Text = "A normal label : TextAlign=BottomRight";
			label11.TextAlign = ContentAlignment.BottomRight;

			label12.Location = new Point(10, 300);
			label12.Name = "label12";
			label12.Size = new Size(465, 16);
			label12.TabIndex = 11;
			label12.Text = "A normal label : With a &shortcut letter (Alt+S)";

			label13.ForeColor = Color.Red;
			label13.Location = new Point(10, 325);
			label13.Name = "label13";
			label13.Size = new Size(465, 16);
			label13.TabIndex = 12;
			label13.Text = "A normal label : ForeColor=Red";

			label14.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((System.Byte)(0)));
			label14.Location = new Point(10, 350);
			label14.Name = "label14";
			label14.Size = new Size(465, 16);
			label14.TabIndex = 13;
			label14.Text = "A normal label : Font.Bold=true";

			label15.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic, GraphicsUnit.Point, ((System.Byte)(0)));
			label15.Location = new Point(10, 375);
			label15.Name = "label15";
			label15.Size = new Size(465, 16);
			label15.TabIndex = 14;
			label15.Text = "A normal label : Font.Italic=true";

			label17.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline, GraphicsUnit.Point, ((System.Byte)(0)));
			label17.Location = new Point(10, 400);
			label17.Name = "label17";
			label17.Size = new Size(465, 16);
			label17.TabIndex = 16;
			label17.Text = "A normal label : Font.Underline=true";

			label16.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Strikeout, GraphicsUnit.Point, ((System.Byte)(0)));
			label16.Location = new Point(10, 425);
			label16.Name = "label16";
			label16.Size = new Size(465, 16);
			label16.TabIndex = 15;
			label16.Text = "A normal label : Font.Strikeout=true";

			label18.Font = new Font("Verdana", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0)));
			label18.Location = new Point(10, 450);
			label18.Name = "label18";
			label18.Size = new Size(465, 16);
			label18.TabIndex = 17;
			label18.Text = "A normal label : Font=Verdana; 9.75pt";

			label19.BackColor = Color.Red;
			label19.Location = new Point(10, 475);
			label19.Name = "label19";
			label19.Size = new Size(465, 16);
			label19.TabIndex = 18;
			label19.Text = "A normal label : BackColor=Red";

			label20.BackColor = SystemColors.Control;
			label20.Enabled = false;
			label20.Location = new Point(10, 500);
			label20.Name = "label20";
			label20.Size = new Size(465, 16);
			label20.TabIndex = 19;
			label20.Text = "A normal label : Enabled=false";

			label24.BorderStyle = BorderStyle.FixedSingle;
			label24.Location = new Point(10, 525);
			label24.Name = "label24";
			label24.Size = new Size(465, 40);
			label24.TabIndex = 23;
			label24.Text = "A normal label : BorderStyle=FixedSingle , Height=40" + Environment.NewLine + "wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww";

			label25.AutoSize = true;
			label25.BorderStyle = BorderStyle.FixedSingle;
			label25.Location = new Point(10, 550);
			label25.Name = "label25";
			label25.Size = new Size(465, 19);
			label25.TabIndex = 24;
			label25.Text = "A normal label : BorderStyle=FixedSingle , AutoSize=true";

			label21.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			label21.BackColor = SystemColors.Control;
			label21.Location = new Point(10, 575);
			label21.Name = "label21";
			label21.Size = new Size(465, 15);
			label21.TabIndex = 20;
			label21.Text = "A normal label : Anchor=All";


			label23.Dock = DockStyle.Bottom;
			label23.Location = new Point(10, 600);
			label23.Name = "label23";
			label23.Width = 465;
			label23.TabIndex = 22;
			label23.Text = "A normal label : Dock=Bottom";

			control.Controls.AddRange(new Control[] {
														label25,
														label24,
														label23,
														label22,
														label21,
														label20,
														label19,
														label18,
														label17,
														label16,
														label15,
														label14,
														label13,
														label12,
														label11,
														label10,
														label9,
														label8,
														label7,
														label6,
														label5,
														label4,
														label3,
														label2,
														label});
		}

		private void AddButtonTest(Control control)
		{
			button = new Button();
			button.Dock = DockStyle.Top;
			button.Location = new Point(0, 0);
			button.Size = new Size(496, 24);
			button.TabIndex = 0;
			button.Text = "A normal button : Dock=Top";

			button2 = new Button();
			button2.Location = new Point(8, 32);
			button2.Size = new Size(480, 24);
			button2.TabIndex = 1;
			button2.Text = "A normal button";

			button3 = new Button();
			button3.Dock = DockStyle.Bottom;
			button3.Location = new Point(0, 32);
			button3.Size = new Size(496, 24);
			button3.TabIndex = 2;
			button3.Text = "A normal button : Dock=Bottom";

			button4 = new Button();
			button4.Enabled = false;
			button4.Location = new Point(8, 64);
			button4.Size = new Size(480, 24);
			button4.TabIndex = 3;
			button4.Text = "A normal button : Enabled=false";

			button5 = new Button();
			button5.BackColor = Color.Red;
			button5.Location = new Point(8, 96);
			button5.Size = new Size(480, 24);
			button5.TabIndex = 4;
			button5.Text = "A normal button : BackColor=Red";

			button6 = new Button();
			button6.FlatStyle = FlatStyle.Flat;
			button6.Location = new Point(8, 128);
			button6.Size = new Size(480, 24);
			button6.TabIndex = 5;
			button6.Text = "A normal button : FlatStyle=Flat";

			button7 = new Button();
			button7.FlatStyle = FlatStyle.Popup;
			button7.Location = new Point(8, 160);
			button7.Size = new Size(480, 24);
			button7.TabIndex = 6;
			button7.Text = "A normal button : FlatStyle=Popup";

			button12 = new Button();
			button12.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0)));
			button12.Location = new Point(8, 192);
			button12.Size = new Size(480, 24);
			button12.TabIndex = 11;
			button12.Text = "A normal button : Font.Size=10";

			button13 = new Button();
			button13.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0)));
			button13.Location = new Point(8, 224);
			button13.Size = new Size(480, 24);
			button13.TabIndex = 12;
			button13.Text = "A normal button : Font=Verdana; 8,25pt";

			button14 = new Button();
			button14.ForeColor = Color.Red;
			button14.Location = new Point(8, 256);
			button14.Size = new Size(480, 24);
			button14.TabIndex = 13;
			button14.Text = "A normal button : ForeColor=Red";

			button15 = new Button();
			button15.Location = new Point(8, 288);
			button15.Size = new Size(480, 32);
			button15.TabIndex = 14;
			button15.Text = "A normal button : TextAlign=TopLeft";
			button15.TextAlign = ContentAlignment.TopLeft;

			button16 = new Button();
			button16.Location = new Point(8, 320);
			button16.Size = new Size(480, 32);
			button16.TabIndex = 15;
			button16.Text = "A normal button : TextAlign=TopCenter";
			button16.TextAlign = ContentAlignment.TopCenter;

			button17 = new Button();
			button17.Location = new Point(8, 352);
			button17.Size = new Size(480, 32);
			button17.TabIndex = 16;
			button17.Text = "A normal button : TextAlign=TopRight";
			button17.TextAlign = ContentAlignment.TopRight;

			button18 = new Button();
			button18.Location = new Point(8, 384);
			button18.Size = new Size(480, 32);
			button18.TabIndex = 17;
			button18.Text = "A normal button : TextAlign=MiddleLeft";
			button18.TextAlign =ContentAlignment.MiddleLeft;

			button19 = new Button();
			button19.Location = new Point(8, 418);
			button19.Size = new Size(480, 32);
			button19.TabIndex = 18;
			button19.Text = "A normal button : TextAlign=MiddleRight";
			button19.TextAlign = ContentAlignment.MiddleRight;

			button20 = new Button();
			button20.Location = new Point(8, 450);
			button20.Size = new Size(480, 32);
			button20.TabIndex = 19;
			button20.Text = "A normal button : TextAlign=BottomLeft";
			button20.TextAlign = ContentAlignment.BottomLeft;

			button21 = new Button();
			button21.Location = new Point(8, 482);
			button21.Size = new Size(480, 32);
			button21.TabIndex = 20;
			button21.Text = "A normal button : TextAlign=BottomCenter";
			button21.TextAlign = ContentAlignment.BottomCenter;

			button22 = new Button();
			button22.Location = new Point(8, 514);
			button22.Size = new Size(480, 32);
			button22.TabIndex = 21;
			button22.Text = "A normal button : TextAlign=BottomRight";
			button22.TextAlign = ContentAlignment.BottomRight;

			button23 = new Button();
			button23.Anchor = (AnchorStyles)AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			button23.Location = new Point(8, 546);
			button23.Name = "button23";
			button23.Size = new Size(480, 24);
			button23.TabIndex = 22;
			button23.Text = "A normal button : Anchor=All";

			control.Controls.AddRange(new Control[] { button, button2, button3, button4, button5, button6, button7, button12, button13, button14, button15, button16, button17, button18, button19, button20, button21, button22, button23});
		}

		private void AddTextBoxTest(Control control)
		{
			textBox = new TextBox();
			textBox1 = new TextBox();
			textBox2 = new TextBox();
			textBox3 = new TextBox();
			textBox4 = new TextBox();
			textBox5 = new TextBox();
			textBox6 = new TextBox();
			textBox7 = new TextBox();
			textBox8 = new TextBox();
			textBox9 = new TextBox();
			textBox10 = new TextBox();
			textBox11 = new TextBox();
			textBox12 = new TextBox();
			textBox13 = new TextBox();
			textBox14 = new TextBox();
			textBox15 = new TextBox();
			textBox16 = new TextBox();
			textBox17 = new TextBox();
			textBox18 = new TextBox();
			textBox19 = new TextBox();
			textBox20 = new TextBox();
			textBoxLinesButton = new Button();
			textBoxTextButton = new Button();
			textBoxSelectedTextButton = new Button();

			textBox19.Dock = DockStyle.Top;
			textBox19.Location = new Point(8, 0);
			textBox19.Name = "textBox19";
			textBox19.Size = new Size(450, 20);
			textBox19.TabIndex = 0;
			textBox19.Text = "A normal textbox - MS Sans Serif : Dock=Top";

			textBox.Location = new Point(8, 25);
			textBox.Name = "textBox";
			textBox.Size = new Size(450, 20);
			textBox.TabIndex = 1;
			textBox.Text = "A normal textbox - MS Sans Serif";

			textBox1.Location = new Point(8, 45);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(450, 20);
			textBox1.TabIndex = 2;
			textBox1.Text = "A normal textbox - MS Sans Serif : TextAlign=Right";
			textBox1.TextAlign = HorizontalAlignment.Right;


			textBox2.Location = new Point(8, 70);
			textBox2.Name = "textBox2";
			textBox2.Size = new Size(450, 20);
			textBox2.TabIndex = 3;
			textBox2.Text = "A normal textbox - MS Sans Serif : TextAlign=Center";
			textBox2.TextAlign = HorizontalAlignment.Center;

			textBox3.BorderStyle = BorderStyle.FixedSingle;
			textBox3.Location = new Point(8, 95);
			textBox3.Name = "textBox3";
			textBox3.Size = new Size(450, 20);
			textBox3.TabIndex = 4;
			textBox3.Text = "A normal textbox - MS Sans Serif : BorderStyle=FixedSingle";

			textBox4.Location = new Point(8, 120);
			textBox4.Multiline = true;
			textBox4.Name = "textBox4";
			textBox4.Size = new Size(450, 40);
			textBox4.TabIndex = 5;
			textBox4.Text = "Multiline = true:\nOh give me a home\nWhere the buffalo roam\nAnd the deer and 45 (forty-five) antelope play\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day.\nHome, Home on the Range\nWhere the deer and the antelope play.\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day....";

			textBox5.Location = new Point(8, 165);
			textBox5.Name = "textBox5";
			textBox5.Size = new Size(450, 21);
			textBox5.TabIndex = 6;
			textBox5.Multiline = true;
			textBox5.ScrollBars = ScrollBars.Both;
			textBox5.WordWrap = false;
			textBox5.Text = "Multiline = true,ScrollBars=Both, WordWrap = false:\nOh give me a home\nWhere the buffalo roam\nAnd the deer and 45 (forty-five) antelope play\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day.\nHome, Home on the Range\nWhere the deer and the antelope play.\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day....";

			textBox6.Location = new Point(8, 190);
			textBox6.Name = "textBox6";
			textBox6.Size = new Size(450, 40);
			textBox6.TabIndex = 7;
			textBox6.Multiline = true;
			textBox6.ScrollBars = ScrollBars.Both;
			textBox6.WordWrap = false;
			textBox6.Text = "Multiline = true,ScrollBars=Both,WordWrap = false:\nOh give me a home\nWhere the buffalo roam\nAnd the deer and 45 (forty-five) antelope play\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day.\nHome, Home on the Range\nWhere the deer and the antelope play.\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day....";

			textBox8.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic);
			textBox8.Location = new Point(8, 240);
			textBox8.Name = "textBox8";
			textBox8.Size = new Size(450, 20);
			textBox8.TabIndex = 8;
			textBox8.Text = "A normal textbox - MS Sans Serif : Font.Italic=true";

			textBox9.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Strikeout);
			textBox9.Location = new Point(8, 265);
			textBox9.Name = "textBox9";
			textBox9.Size = new Size(450, 20);
			textBox9.TabIndex = 9;
			textBox9.Text = "A normal textbox - MS Sans Serif : Font.StrikeOut=true";

			textBox10.Location = new Point(8, 290);
			textBox10.Name = "textBox10";
			textBox10.Size = new Size(450, 20);
			textBox10.TabIndex = 10;
			textBox10.Multiline = true;
			textBox10.ScrollBars = ScrollBars.Both;
			textBox10.WordWrap = false;
			textBox10.Text = "Multiline = true,ScrollBars=Both, WordWrap=false:\nOh give me a home\nWhere the buffalo roam\nAnd the deer and 45 (forty-five) antelope play\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day.\nHome, Home on the Range\nWhere the deer and the antelope play.\nWhere seldom is heard\nA discouraging word\nAnd the skies are not cloudy all day....";

			textBox11.Font = new Font("Microsoft Sans Serif", 8.25F);
			textBox11.ScrollBars = ScrollBars.Vertical;
			textBox11.Location = new Point(8, 315);
			textBox11.Name = "textBox11";
			textBox11.Size = new Size(450, 20);
			textBox11.TabIndex = 11;
			textBox11.Text = "ScrollBars=Vertical:";

			textBox12.Font = new Font("Microsoft Sans Serif", 8.25F);
			textBox12.Location = new Point(8, 340);
			textBox12.Name = "textBox12";
			textBox12.PasswordChar = '*';
			textBox12.Size = new Size(450, 20);
			textBox12.TabIndex = 12;
			textBox12.Text = "A normal textbox - MS Sans Serif : PasswordChar=*";

			textBox13.Font = new Font("Microsoft Sans Serif", 8.25F);
			textBox13.Location = new Point(8, 365);
			textBox13.Name = "textBox13";
			textBox13.ReadOnly = true;
			textBox13.Size = new Size(450, 20);
			textBox13.TabIndex = 13;
			textBox13.Text = "A normal textbox - MS Sans Serif : ReadOnly=true";

			textBox20.Enabled = false;
			textBox20.Location = new Point(8, 390);
			textBox20.Name = "textBox20";
			textBox20.Size = new Size(450, 20);
			textBox20.TabIndex = 14;
			textBox20.Text = "A normal textbox - MS Sans Serif : Enabled=false";

			textBox14.Font = new Font("Microsoft Sans Serif", 8.25F);
			textBox14.Location = new Point(8, 415);
			textBox14.MaxLength = 50;
			textBox14.Name = "textBox14";
			textBox14.Size = new Size(450, 20);
			textBox14.TabIndex = 15;
			textBox14.Text = "A normal textbox - MS Sans Serif : MaxLength=50";

			textBox15.BackColor = Color.Red;
			textBox15.Location = new Point(8, 440);
			textBox15.Name = "textBox15";
			textBox15.Size = new Size(450, 20);
			textBox15.TabIndex = 16;
			textBox15.Text = "A normal textbox - MS Sans Serif : BackColor=Red";

			textBox16.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
				| AnchorStyles.Left)
				| AnchorStyles.Right)));
			textBox16.Location = new Point(8, 465);
			textBox16.Name = "textBox16";
			textBox16.Size = new Size(450, 20);
			textBox16.TabIndex = 17;
			textBox16.Text = "A normal textbox - MS Sans Serif : Anchor=All";

			textBox17.AcceptsTab = true;
			textBox17.Location = new Point(8, 490);
			textBox17.Name = "textBox17";
			textBox17.Size = new Size(450, 20);
			textBox17.TabIndex = 18;
			textBox17.Text = "A normal textbox - MS Sans Serif : AllowTab=true";

			textBox18.Dock = DockStyle.Bottom;
			textBox18.Location = new Point(8, 518);
			textBox18.Name = "textBox18";
			textBox18.Size = new Size(450, 20);
			textBox18.TabIndex = 19;
			textBox18.Text = "A normal textbox - MS Sans Serif : Dock=Bottom";

			textBoxLinesButton.Bounds = new Rectangle( 8, 510, 150, 30);
			textBoxLinesButton.Text = "Lines[] for Multiline";
			textBoxLinesButton.Click+=new EventHandler(textBoxLinesButton_Click);

			textBoxTextButton.Bounds = new Rectangle( 180, 510, 70, 30);
			textBoxTextButton.Text = "Add Text";
			textBoxTextButton.Click+=new EventHandler(textBoxTextButton_Click);

			textBoxSelectedTextButton.Bounds = new Rectangle( 250, 510, 150, 30);
			textBoxSelectedTextButton.Text = "Set Selected Text";
			textBoxSelectedTextButton.Click+=new EventHandler(textBoxSelectedTextButton_Click);

			control.Controls.AddRange(new Control[] {
														textBox19,
														textBox20,
														textBox18,
														textBox17,
														textBox16,
														textBox15,
														textBox14,
														textBox13,
														textBox12,
														textBox11,
														textBox10,
														textBox9,
														textBox8,
														textBox6,
														textBox5,
														textBox4,
														textBox3,
														textBox2,
														textBox,
														textBox1,
														textBoxLinesButton,
														textBoxTextButton,
														textBoxSelectedTextButton
													});
		}

		private void textBoxLinesButton_Click(object sender, EventArgs e)
		{
			foreach(String s in textBox4.Lines)
				Console.WriteLine(s);
		}

		private void textBoxTextButton_Click(object sender, EventArgs e)
		{
			textBox4.AppendText("1234 123456 123");
		}
		private void textBoxSelectedTextButton_Click(object sender, EventArgs e)
		{
			textBox4.SelectedText = "aaaa bbbbb cccc";
		}

		private void AddTextBoxTest2(Control c)
		{
			textBoxTest2b = new TextBox();
			textBoxTest2b.Bounds = new Rectangle(250,10, 200, 300);
			textBoxTest2b.Multiline = true;
			textBoxTest2b.ReadOnly = true;
			textBoxTest2b.ScrollBars = ScrollBars.Both;

			textBoxTest2a = new TextBox();
			textBoxTest2a.Bounds = new Rectangle(10,10, 200, 300);
			textBoxTest2a.Multiline = true;
			textBoxTest2a.WordWrap = false;
			textBoxTest2a.ScrollBars = ScrollBars.Both;
			textBoxTest2a.TextChanged+=new EventHandler(textBoxTest2a_TextChanged);
			textBoxTest2a.Text = "Play around...\r\n";

			c.Controls.Add(textBoxTest2a);
			c.Controls.Add(textBoxTest2b);

		}

		private void textBoxTest2a_TextChanged(object sender, EventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < textBoxTest2a.Text.Length; i++)
			{
				if (textBoxTest2a.Text[i] == '\n')
					sb.Append("\\n\r\n");
				else if (textBoxTest2a.Text[i] == '\r')
					sb.Append("\\r");
				else
					sb.Append(textBoxTest2a.Text[i]);
			}
			textBoxTest2b.Text = sb.ToString();

		}

		private void AddRadioButtonsTest(Control control)
		{
			radioButton18 = new RadioButton();
			radioButton19 = new RadioButton();
			radioButton14 = new RadioButton();
			radioButton15 = new RadioButton();
			radioButton16 = new RadioButton();
			radioButton17 = new RadioButton();
			radioButton10 = new RadioButton();
			radioButton11 = new RadioButton();
			radioButton12 = new RadioButton();
			radioButton13 = new RadioButton();
			radioButton29 = new RadioButton();
			radioButton28 = new RadioButton();
			radioButton27 = new RadioButton();
			radioButton24 = new RadioButton();
			radioButton23 = new RadioButton();
			radioButton32 = new RadioButton();
			radioButton9 = new RadioButton();
			radioButton8 = new RadioButton();
			radioButton5 = new RadioButton();
			radioButton4 = new RadioButton();
			radioButton7 = new RadioButton();
			radioButton6 = new RadioButton();
			radioButton26 = new RadioButton();
			radioButton25 = new RadioButton();
			radioButton3 = new RadioButton();
			radioButton2 = new RadioButton();
			radioButton22 = new RadioButton();
			radioButton21 = new RadioButton();
			radioButton20 = new RadioButton();
			radioButton = new RadioButton();
			radioButton33 = new RadioButton();
			radioButton30 = new RadioButton();
			radioButton31 = new RadioButton();

			radioButton18.Location = new Point(8, 368);
			radioButton18.Name = "radioButton18";
			radioButton18.RightToLeft = RightToLeft.Yes;
			radioButton18.Size = new Size(288, 24);
			radioButton18.TabIndex = 17;
			radioButton18.Text = "RightToLeft=True";

			radioButton19.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, ((System.Byte)(0)));
			radioButton19.Location = new Point(240, 352);
			radioButton19.Name = "radioButton19";
			radioButton19.Size = new Size(200, 24);
			radioButton19.TabIndex = 18;
			radioButton19.Text = "Font.Bold=True";

			radioButton14.CheckAlign = ContentAlignment.TopLeft;
			radioButton14.Location = new Point(8, 128);
			radioButton14.Name = "radioButton14";
			radioButton14.Size = new Size(200, 24);
			radioButton14.TabIndex = 13;
			radioButton14.Text = " CheckAlign=TopLeft";

			radioButton15.Dock = DockStyle.Top;
			radioButton15.Location = new Point(0, 0);
			radioButton15.Name = "radioButton15";
			radioButton15.Size = new Size(608, 24);
			radioButton15.TabIndex = 14;
			radioButton15.Text = "Dock=Top";

			radioButton16.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Underline, GraphicsUnit.Point, ((System.Byte)(0)));
			radioButton16.Location = new Point(240, 448);
			radioButton16.Name = "radioButton16";
			radioButton16.Size = new Size(200, 24);
			radioButton16.TabIndex = 39;
			radioButton16.Text = "Font.Underline=True";

			radioButton17.FlatStyle = FlatStyle.Flat;
			radioButton17.Location = new Point(240, 200);
			radioButton17.Name = "radioButton17";
			radioButton17.Size = new Size(200, 24);
			radioButton17.TabIndex = 16;
			radioButton17.Text = "FlatStyle=Flat";

			radioButton10.BackColor = Color.SeaGreen;
			radioButton10.Location = new Point(8, 400);
			radioButton10.Name = "radioButton10";
			radioButton10.Size = new Size(200, 24);
			radioButton10.TabIndex = 9;
			radioButton10.Text = "BackColor=SeaGreen";

			radioButton11.Location = new Point(240, 32);
			radioButton11.Name = "radioButton11";
			radioButton11.Size = new Size(200, 24);
			radioButton11.TabIndex = 10;
			radioButton11.Text = "TextAlign=MiddleCenter";
			radioButton11.TextAlign = ContentAlignment.MiddleCenter;

			radioButton12.FlatStyle = FlatStyle.Popup;
			radioButton12.Location = new Point(240, 320);
			radioButton12.Name = "radioButton12";
			radioButton12.Size = new Size(200, 24);
			radioButton12.TabIndex = 37;
			radioButton12.Text = "FlatStyle=Popup";

			radioButton13.CheckAlign = ContentAlignment.BottomLeft;
			radioButton13.Location = new Point(8, 232);
			radioButton13.Name = "radioButton13";
			radioButton13.Size = new Size(200, 24);
			radioButton13.TabIndex = 12;
			radioButton13.Text = "CheckAlign=BottomLeft";

			radioButton29.Location = new Point(240, 256);
			radioButton29.Name = "radioButton29";
			radioButton29.Size = new Size(200, 24);
			radioButton29.TabIndex = 36;
			radioButton29.Text = "TextAlign=BottomRight";
			radioButton29.TextAlign = ContentAlignment.BottomRight;

			radioButton28.Location = new Point(240, 224);
			radioButton28.Name = "radioButton28";
			radioButton28.Size = new Size(200, 24);
			radioButton28.TabIndex = 35;
			radioButton28.Text = "TextAlign=BottomCenter";
			radioButton28.TextAlign = ContentAlignment.BottomCenter;

			radioButton27.Location = new Point(240, 192);
			radioButton27.Name = "radioButton27";
			radioButton27.Size = new Size(200, 24);
			radioButton27.TabIndex = 34;
			radioButton27.Text = "TextAlign=BottomLeft";
			radioButton27.TextAlign = ContentAlignment.BottomLeft;

			radioButton24.Location = new Point(240, 160);
			radioButton24.Name = "radioButton24";
			radioButton24.Size = new Size(200, 24);
			radioButton24.TabIndex = 33;
			radioButton24.Text = "TextAlign=TopRight";
			radioButton24.TextAlign = ContentAlignment.TopRight;

			radioButton23.CheckAlign = ContentAlignment.MiddleRight;
			radioButton23.Location = new Point(8, 96);
			radioButton23.Name = "radioButton23";
			radioButton23.Size = new Size(200, 24);
			radioButton23.TabIndex = 26;
			radioButton23.Text = "CheckAlign=MiddleRight";

			radioButton32.ForeColor = Color.Green;
			radioButton32.Location = new Point(8, 432);
			radioButton32.Name = "radioButton32";
			radioButton32.Size = new Size(200, 24);
			radioButton32.TabIndex = 42;
			radioButton32.Text = "ForeColor=Green";

			radioButton9.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Italic, GraphicsUnit.Point, ((System.Byte)(0)));
			radioButton9.Location = new Point(240, 384);
			radioButton9.Name = "radioButton9";
			radioButton9.Size = new Size(200, 24);
			radioButton9.TabIndex = 8;
			radioButton9.Text = "Font.Italic=True";

			radioButton8.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Strikeout, GraphicsUnit.Point, ((System.Byte)(0)));
			radioButton8.Location = new Point(240, 416);
			radioButton8.Name = "radioButton8";
			radioButton8.Size = new Size(200, 24);
			radioButton8.TabIndex = 38;
			radioButton8.Text = "Font.Strikeout=True";

			radioButton5.Dock = DockStyle.Bottom;
			radioButton5.Location = new Point(0, 541);
			radioButton5.Name = "radioButton5";
			radioButton5.Size = new Size(608, 24);
			radioButton5.TabIndex = 4;
			radioButton5.Text = "Dock = Bottom";

			radioButton4.CheckAlign = ContentAlignment.TopRight;
			radioButton4.Location = new Point(8, 200);
			radioButton4.Name = "radioButton4";
			radioButton4.Size = new Size(200, 24);
			radioButton4.TabIndex = 3;
			radioButton4.Text = "CheckAlign=TopRight";

			radioButton7.Checked = true;
			radioButton7.Location = new Point(8, 336);
			radioButton7.Name = "radioButton7";
			radioButton7.Size = new Size(200, 24);
			radioButton7.TabIndex = 6;
			radioButton7.TabStop = true;
			radioButton7.Text = "Checked=True";

			radioButton6.Location = new Point(8, 32);
			radioButton6.Name = "radioButton6";
			radioButton6.Size = new Size(200, 24);
			radioButton6.TabIndex = 22;
			radioButton6.Text = "A normal RadioButton";

			radioButton26.Location = new Point(240, 96);
			radioButton26.Name = "radioButton26";
			radioButton26.Size = new Size(200, 24);
			radioButton26.TabIndex = 31;
			radioButton26.Text = "TextAlign=TopLeft";
			radioButton26.TextAlign = ContentAlignment.TopLeft;

			radioButton25.Location = new Point(240, 128);
			radioButton25.Name = "radioButton25";
			radioButton25.Size = new Size(200, 24);
			radioButton25.TabIndex = 32;
			radioButton25.Text = "TextAlign=TopCenter";
			radioButton25.TextAlign = ContentAlignment.TopCenter;

			radioButton3.CheckAlign = ContentAlignment.BottomRight;
			radioButton3.Location = new Point(8, 304);
			radioButton3.Name = "radioButton3";
			radioButton3.Size = new Size(200, 24);
			radioButton3.TabIndex = 2;
			radioButton3.Text = "CheckAlign=BottomRight";

			radioButton2.CheckAlign = ContentAlignment.TopCenter;
			radioButton2.Location = new Point(8, 160);
			radioButton2.Name = "radioButton2";
			radioButton2.Size = new Size(200, 32);
			radioButton2.TabIndex = 1;
			radioButton2.Text = "CheckAlign=TopCenter";

			radioButton22.CheckAlign = ContentAlignment.MiddleCenter;
			radioButton22.Location = new Point(8, 64);
			radioButton22.Name = "radioButton22";
			radioButton22.Size = new Size(200, 24);
			radioButton22.TabIndex = 25;
			radioButton22.Text = "CheckAlign=MiddleCenter";

			radioButton21.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom)
				| AnchorStyles.Left)
				| AnchorStyles.Right)));
			radioButton21.Location = new Point(8, 496);
			radioButton21.Name = "radioButton21";
			radioButton21.Size = new Size(200, 24);
			radioButton21.TabIndex = 21;
			radioButton21.Text = "Anchor=All";

			radioButton20.CheckAlign = ContentAlignment.BottomCenter;
			radioButton20.Location = new Point(8, 264);
			radioButton20.Name = "radioButton20";
			radioButton20.Size = new Size(200, 32);
			radioButton20.TabIndex = 23;
			radioButton20.Text = "CheckAlign=BottomCenter";

			radioButton.Location = new Point(240, 64);
			radioButton.Name = "radioButton";
			radioButton.Size = new Size(200, 24);
			radioButton.TabIndex = 0;
			radioButton.Text = "TextAlign=MiddleRight";
			radioButton.TextAlign = ContentAlignment.MiddleRight;

			radioButton33.Appearance = Appearance.Button;
			radioButton33.Location = new Point(240, 512);
			radioButton33.Name = "radioButton33";
			radioButton33.Size = new Size(200, 24);
			radioButton33.TabIndex = 43;
			radioButton33.Text = "Appearance=Button";

			radioButton30.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0)));
			radioButton30.Location = new Point(8, 464);
			radioButton30.Name = "radioButton30";
			radioButton30.Size = new Size(200, 24);
			radioButton30.TabIndex = 40;
			radioButton30.Text = "Font.Size=10";

			radioButton31.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((System.Byte)(0)));
			radioButton31.Location = new Point(240, 480);
			radioButton31.Name = "radioButton31";
			radioButton31.Size = new Size(200, 24);
			radioButton31.TabIndex = 41;
			radioButton31.Text = "Font=Verdana; 8,25pt";

			control.Controls.AddRange(new Control[] {
														radioButton33,
														radioButton32,
														radioButton31,
														radioButton30,
														radioButton16,
														radioButton8,
														radioButton12,
														radioButton29,
														radioButton28,
														radioButton27,
														radioButton24,
														radioButton25,
														radioButton26,
														radioButton23,
														radioButton22,
														radioButton20,
														radioButton6,
														radioButton21,
														radioButton19,
														radioButton18,
														radioButton17,
														radioButton15,
														radioButton14,
														radioButton13,
														radioButton11,
														radioButton10,
														radioButton9,
														radioButton7,
														radioButton5,
														radioButton4,
														radioButton3,
														radioButton2,
														radioButton});
		}
		private void AddRegionsTest(Control control)
		{
			control.Paint+=new PaintEventHandler(Regions_Paint);
		}

		private void Regions_Paint(object sender, PaintEventArgs e)
		{
			using (Brush b = new SolidBrush(Color.LightGray), bl = new SolidBrush(Color.Blue))
			{
				e.Graphics.FillRectangle(b, new Rectangle(0,0,Width, Height));
				int count = 10;

				Region region;
				Region[] r = new Region[count];
				r[0] = new Region(new Rectangle(0,0,50,50));
				r[1] = new Region(new Rectangle(25,25,20,20));
				r[2] = new Region(new Rectangle(0,0,30,30));
				r[3] = new Region(new Rectangle(20,20,30,30));
				r[4] = new Region(new Rectangle(5,5,10,10));
				r[5] = new Region(new Rectangle(35,35,10,10));
				r[6] = new Region(new Rectangle(0,0,0,0));
				r[7] = new Region(new Rectangle(35,35,10,10));
				r[8] = new Region(new Rectangle(5,5,10,10));
				r[9] = new Region(new Rectangle(0,0,0,0));


				for (int i=0;i<count;i+=2)
				{
					region = r[i].Clone();
					region.Union(r[i+1]);
					DrawRegions(e.Graphics, i/2+count/2*0, "Union " + (i/2+1), r[i], r[i+1], region);
				}

				for (int i=0;i<count;i+=2)
				{
					region = r[i].Clone();
					region.Intersect(r[i+1]);
					DrawRegions(e.Graphics, i/2+count/2*1, "Intersect " + (i/2+1), r[i], r[i+1], region);
				}

				for (int i=0;i<count;i+=2)
				{
					region = r[i].Clone();
					region.Complement(r[i+1]);
					DrawRegions(e.Graphics, i/2+count/2*2, "Complement " + (i/2+1), r[i], r[i+1], region);
				}

				for (int i=0;i<count;i+=2)
				{
					region = r[i].Clone();
					region.Exclude(r[i+1]);
					DrawRegions(e.Graphics, i/2+count/2*3, "Exclude " + (i/2+1), r[i], r[i+1], region);
				}

				for (int i=0;i<count;i+=2)
				{
					region = r[i].Clone();
					region.Xor(r[i+1]);
					DrawRegions(e.Graphics, i/2+count/2*4, "Xor " + (i/2+1), r[i], r[i+1], region);
				}

			}
		}

		private void DrawRegions( Graphics g, int offset, string s, Region r1, Region r2, Region r3)
		{
			using (Pen p = new Pen(Color.Black))
			{
				using (Brush b1 = new SolidBrush(Color.Red), b2 = new SolidBrush(Color.Green), bl = new SolidBrush(Color.Blue))
				{
					Region region1 = r1.Clone();
					Region region2 = r2.Clone();
					Region region3 = r3.Clone();
					int x = (offset%4) * 110;
					int y = (int)offset / 4*80;
					g.DrawString(s,Font, bl, x, y);
					region1.Translate(x, y + 20);
					region2.Translate(x, y + 20);
					region3.Translate(x + 55, y + 20);
					g.FillRegion(b1,region1);
					g.DrawRectangle(p, Rectangle.Truncate(region1.GetBounds(g)));
					g.FillRegion(b2,region2);
					g.DrawRectangle(p, Rectangle.Truncate(region2.GetBounds(g)));
					g.FillRegion(b1,region3);
					g.DrawRectangle(p, Rectangle.Truncate(region3.GetBounds(g)));
				}
			}
		}

		private void AddTabControlsTest(Control control)
		{
			Docked = new TabControl();
			tabPageT1 = new TabPage();
			tabControlT2 = new TabControl();
			tabPageT6 = new TabPage();
			labelT2 = new Label();
			labelT1 = new Label();
			buttonT1 = new Button();
			tabPageT22 = new TabPage();
			tabPageT23 = new TabPage();
			tabPageT21 = new TabPage();
			tabPageT9 = new TabPage();
			tabPageT24 = new TabPage();
			tabPageT25 = new TabPage();
			tabPageT10 = new TabPage();
			tabControlT8 = new TabControl();
			tabPageT37 = new TabPage();
			tabPageT38 = new TabPage();
			tabPageT20 = new TabPage();
			tabControlT5 = new TabControl();
			tabPageT26 = new TabPage();
			tabPageT27 = new TabPage();
			tabPageT28 = new TabPage();
			tabPageT2 = new TabPage();
			tabControlT3 = new TabControl();
			tabPageT7 = new TabPage();
			tabPageT8 = new TabPage();
			tabPageT3 = new TabPage();
			tabControlT4 = new TabControl();
			tabPageT11 = new TabPage();
			tabPageT12 = new TabPage();
			tabPageT13 = new TabPage();
			tabPageT14 = new TabPage();
			tabPageT15 = new TabPage();
			tabPageT16 = new TabPage();
			tabPageT17 = new TabPage();
			tabPageT18 = new TabPage();
			tabPageT4 = new TabPage();
			tabControlT6 = new TabControl();
			tabPageT29 = new TabPage();
			tabPageT30 = new TabPage();
			tabPageT31 = new TabPage();
			tabPageT5 = new TabPage();
			tabControlT7 = new TabControl();
			tabPageT32 = new TabPage();
			tabPageT33 = new TabPage();
			tabPageT19 = new TabPage();
			tabPageT34 = new TabPage();
			tabControlT1 = new TabControl();
			tabPageT35 = new TabPage();
			tabPageT36 = new TabPage();
			labelT3 = new Label();
			labelT4 = new Label();

			Docked.Controls.Add(tabPageT1);
			Docked.Controls.Add(tabPageT10);
			Docked.Controls.Add(tabPageT5);
			Docked.Controls.Add(tabPageT19);
			Docked.Controls.Add(tabPageT20);
			Docked.Controls.Add(tabPageT2);
			Docked.Controls.Add(tabPageT3);
			Docked.Controls.Add(tabPageT4);
			Docked.Controls.Add(tabPageT34);
			Docked.Location = new Point(24, 16);
			Docked.Name = "Docked";
			Docked.SelectedIndex = 0;
			Docked.Size = new Size(384, 320);
			Docked.TabIndex = 0;

			tabPageT1.Controls.Add(tabControlT2);
			tabPageT1.Location = new Point(4, 22);
			tabPageT1.Name = "tabPage1";
			tabPageT1.Size = new Size(376, 294);
			tabPageT1.TabIndex = 0;
			tabPageT1.Text = "FillToRight";

			tabControlT2.Controls.Add(tabPageT6);
			tabControlT2.Controls.Add(tabPageT22);
			tabControlT2.Controls.Add(tabPageT23);
			tabControlT2.Controls.Add(tabPageT21);
			tabControlT2.Controls.Add(tabPageT9);
			tabControlT2.Controls.Add(tabPageT24);
			tabControlT2.Controls.Add(tabPageT25);
			tabControlT2.Location = new Point(16, 24);
			tabControlT2.Multiline = true;
			tabControlT2.Name = "tabControl2";
			tabControlT2.SelectedIndex = 0;
			tabControlT2.Size = new Size(344, 248);
			tabControlT2.SizeMode = TabSizeMode.FillToRight;
			tabControlT2.TabIndex = 1;

			tabPageT6.Controls.Add(labelT2);
			tabPageT6.Controls.Add(labelT1);
			tabPageT6.Controls.Add(buttonT1);
			tabPageT6.Location = new Point(4, 40);
			tabPageT6.Name = "tabPage6";
			tabPageT6.Size = new Size(336, 204);
			tabPageT6.TabIndex = 0;
			tabPageT6.Text = "First";

			labelT2.Location = new Point(144, 160);
			labelT2.Name = "label2";
			labelT2.Size = new Size(112, 40);
			labelT2.TabIndex = 2;
			labelT2.Text = "label2";

			labelT1.Location = new Point(144, 88);
			labelT1.Name = "label1";
			labelT1.Size = new Size(112, 40);
			labelT1.TabIndex = 1;
			labelT1.Text = "label1";

			buttonT1.Location = new Point(24, 16);
			buttonT1.Name = "button1";
			buttonT1.Size = new Size(96, 40);
			buttonT1.TabIndex = 0;
			buttonT1.Text = "button1";

			tabPageT22.Location = new Point(4, 40);
			tabPageT22.Name = "tabPage22";
			tabPageT22.Size = new Size(336, 204);
			tabPageT22.TabIndex = 3;
			tabPageT22.Text = "tabPage22";

			tabPageT23.Location = new Point(4, 40);
			tabPageT23.Name = "tabPage23";
			tabPageT23.Size = new Size(336, 204);
			tabPageT23.TabIndex = 4;
			tabPageT23.Text = "tabPage23";

			tabPageT21.Location = new Point(4, 40);
			tabPageT21.Name = "tabPage21";
			tabPageT21.Size = new Size(336, 204);
			tabPageT21.TabIndex = 2;
			tabPageT21.Text = "tabPage21";

			tabPageT9.Location = new Point(4, 40);
			tabPageT9.Name = "tabPage9";
			tabPageT9.Size = new Size(336, 204);
			tabPageT9.TabIndex = 1;
			tabPageT9.Text = "Second";

			tabPageT24.Location = new Point(4, 40);
			tabPageT24.Name = "tabPage24";
			tabPageT24.Size = new Size(336, 204);
			tabPageT24.TabIndex = 5;
			tabPageT24.Text = "tabPage24";

			tabPageT25.Location = new Point(4, 40);
			tabPageT25.Name = "tabPage25";
			tabPageT25.Size = new Size(336, 204);
			tabPageT25.TabIndex = 6;
			tabPageT25.Text = "tabPage25";

			tabPageT10.Controls.Add(tabControlT8);
			tabPageT10.Location = new Point(4, 22);
			tabPageT10.Name = "tabPage10";
			tabPageT10.Size = new Size(376, 294);
			tabPageT10.TabIndex = 5;
			tabPageT10.Text = "OwnerDraw";

			tabControlT8.Controls.Add(tabPageT37);
			tabControlT8.Controls.Add(tabPageT38);
			tabControlT8.DrawMode = TabDrawMode.OwnerDrawFixed;
			tabControlT8.Location = new Point(32, 24);
			tabControlT8.Name = "tabControl8";
			tabControlT8.SelectedIndex = 0;
			tabControlT8.Size = new Size(328, 248);
			tabControlT8.TabIndex = 0;
			tabControlT8.DrawItem += new DrawItemEventHandler(tabControlT8_DrawItem);

			tabPageT37.Location = new Point(4, 22);
			tabPageT37.Name = "tabPage37";
			tabPageT37.Size = new Size(320, 222);
			tabPageT37.TabIndex = 0;
			tabPageT37.Text = "tabPage37";

			tabPageT38.Location = new Point(4, 22);
			tabPageT38.Name = "tabPage38";
			tabPageT38.Size = new Size(320, 222);
			tabPageT38.TabIndex = 1;
			tabPageT38.Text = "tabPage38";

			tabPageT20.Controls.Add(tabControlT5);
			tabPageT20.Location = new Point(4, 22);
			tabPageT20.Name = "tabPage20";
			tabPageT20.Size = new Size(376, 294);
			tabPageT20.TabIndex = 7;
			tabPageT20.Text = "FixedSize";

			tabControlT5.Controls.Add(tabPageT26);
			tabControlT5.Controls.Add(tabPageT27);
			tabControlT5.Controls.Add(tabPageT28);
			tabControlT5.ItemSize = new Size(100, 30);
			tabControlT5.Location = new Point(24, 32);
			tabControlT5.Name = "tabControl5";
			tabControlT5.SelectedIndex = 0;
			tabControlT5.Size = new Size(336, 256);
			tabControlT5.SizeMode = TabSizeMode.Fixed;
			tabControlT5.TabIndex = 0;

			tabPageT26.Location = new Point(4, 34);
			tabPageT26.Name = "tabPage26";
			tabPageT26.Size = new Size(328, 218);
			tabPageT26.TabIndex = 0;
			tabPageT26.Text = "a";

			tabPageT27.Location = new Point(4, 34);
			tabPageT27.Name = "tabPage27";
			tabPageT27.Size = new Size(328, 218);
			tabPageT27.TabIndex = 1;
			tabPageT27.Text = "second";

			tabPageT28.Location = new Point(4, 34);
			tabPageT28.Name = "tabPage28";
			tabPageT28.Size = new Size(328, 218);
			tabPageT28.TabIndex = 2;
			tabPageT28.Text = "3rd";

			tabPageT2.Controls.Add(tabControlT3);
			tabPageT2.Location = new Point(4, 22);
			tabPageT2.Name = "tabPage2";
			tabPageT2.Size = new Size(376, 294);
			tabPageT2.TabIndex = 1;
			tabPageT2.Text = "AlignBot";

			tabControlT3.Alignment = TabAlignment.Bottom;
			tabControlT3.Controls.Add(tabPageT7);
			tabControlT3.Controls.Add(tabPageT8);
			tabControlT3.Location = new Point(32, 32);
			tabControlT3.Name = "tabControl3";
			tabControlT3.SelectedIndex = 0;
			tabControlT3.Size = new Size(232, 208);
			tabControlT3.TabIndex = 1;

			tabPageT7.Location = new Point(4, 4);
			tabPageT7.Name = "tabPage7";
			tabPageT7.Size = new Size(224, 182);
			tabPageT7.TabIndex = 0;
			tabPageT7.Text = "tabPage7";

			tabPageT8.Location = new Point(4, 4);
			tabPageT8.Name = "tabPage8";
			tabPageT8.Size = new Size(224, 182);
			tabPageT8.TabIndex = 1;
			tabPageT8.Text = "tabPage8";

			tabPageT3.Controls.Add(tabControlT4);
			tabPageT3.Location = new Point(4, 22);
			tabPageT3.Name = "tabPage3";
			tabPageT3.Size = new Size(376, 294);
			tabPageT3.TabIndex = 2;
			tabPageT3.Text = "MultiLine";

			tabControlT4.Controls.Add(tabPageT11);
			tabControlT4.Controls.Add(tabPageT12);
			tabControlT4.Controls.Add(tabPageT13);
			tabControlT4.Controls.Add(tabPageT14);
			tabControlT4.Controls.Add(tabPageT15);
			tabControlT4.Controls.Add(tabPageT16);
			tabControlT4.Controls.Add(tabPageT17);
			tabControlT4.Controls.Add(tabPageT18);
			tabControlT4.Location = new Point(16, 16);
			tabControlT4.Multiline = true;
			tabControlT4.Name = "tabControl4";
			tabControlT4.SelectedIndex = 0;
			tabControlT4.Size = new Size(344, 256);
			tabControlT4.TabIndex = 0;

			tabPageT11.Location = new Point(4, 40);
			tabPageT11.Name = "tabPage11";
			tabPageT11.Size = new Size(336, 212);
			tabPageT11.TabIndex = 0;
			tabPageT11.Text = "tabPage11";

			tabPageT12.Location = new Point(4, 40);
			tabPageT12.Name = "tabPage12";
			tabPageT12.Size = new Size(336, 212);
			tabPageT12.TabIndex = 1;
			tabPageT12.Text = "tabPage12";

			tabPageT13.Location = new Point(4, 40);
			tabPageT13.Name = "tabPageT13";
			tabPageT13.Size = new Size(336, 212);
			tabPageT13.TabIndex = 2;
			tabPageT13.Text = "tabPageT13";

			tabPageT14.Location = new Point(4, 40);
			tabPageT14.Name = "tabPageT14";
			tabPageT14.Size = new Size(336, 212);
			tabPageT14.TabIndex = 3;
			tabPageT14.Text = "tabPageT14";

			tabPageT15.Location = new Point(4, 40);
			tabPageT15.Name = "tabPageT15";
			tabPageT15.Size = new Size(336, 212);
			tabPageT15.TabIndex = 4;
			tabPageT15.Text = "tabPageT15";

			tabPageT16.Location = new Point(4, 40);
			tabPageT16.Name = "tabPageT16";
			tabPageT16.Size = new Size(336, 212);
			tabPageT16.TabIndex = 5;
			tabPageT16.Text = "tabPageT16";

			tabPageT17.Location = new Point(4, 40);
			tabPageT17.Name = "tabPageT17";
			tabPageT17.Size = new Size(336, 212);
			tabPageT17.TabIndex = 6;
			tabPageT17.Text = "tabPageT17";

			tabPageT18.BackColor = Color.Red;
			tabPageT18.Location = new Point(4, 40);
			tabPageT18.Name = "tabPageT18";
			tabPageT18.Size = new Size(336, 212);
			tabPageT18.TabIndex = 7;
			tabPageT18.Text = "tabPageT18";

			tabPageT4.Controls.Add(tabControlT6);
			tabPageT4.Location = new Point(4, 22);
			tabPageT4.Name = "tabPageT4";
			tabPageT4.Size = new Size(376, 294);
			tabPageT4.TabIndex = 3;
			tabPageT4.Text = "Hottrack";

			tabControlT6.Controls.Add(tabPageT29);
			tabControlT6.Controls.Add(tabPageT30);
			tabControlT6.Controls.Add(tabPageT31);
			tabControlT6.HotTrack = true;
			tabControlT6.Location = new Point(8, 24);
			tabControlT6.Name = "tabControlT6";
			tabControlT6.SelectedIndex = 0;
			tabControlT6.Size = new Size(352, 248);
			tabControlT6.TabIndex = 0;

			tabPageT29.Location = new Point(4, 22);
			tabPageT29.Name = "tabPageT29";
			tabPageT29.Size = new Size(344, 222);
			tabPageT29.TabIndex = 0;
			tabPageT29.Text = "tabPageT29";

			tabPageT30.Location = new Point(4, 22);
			tabPageT30.Name = "tabPageT30";
			tabPageT30.Size = new Size(344, 222);
			tabPageT30.TabIndex = 1;
			tabPageT30.Text = "tabPageT30";

			tabPageT31.Location = new Point(4, 22);
			tabPageT31.Name = "tabPageT31";
			tabPageT31.Size = new Size(344, 222);
			tabPageT31.TabIndex = 2;
			tabPageT31.Text = "tabPageT31";

			tabPageT5.Controls.Add(labelT3);
			tabPageT5.Controls.Add(tabControlT7);
			tabPageT5.Location = new Point(4, 22);
			tabPageT5.Name = "tabPageT5";
			tabPageT5.Size = new Size(376, 294);
			tabPageT5.TabIndex = 4;
			tabPageT5.Text = "Right";

			tabControlT7.Alignment = TabAlignment.Right;
			tabControlT7.Controls.Add(tabPageT32);
			tabControlT7.Controls.Add(tabPageT33);
			tabControlT7.Location = new Point(8, 48);
			tabControlT7.Multiline = true;
			tabControlT7.Name = "tabControlT7";
			tabControlT7.SelectedIndex = 0;
			tabControlT7.Size = new Size(360, 232);
			tabControlT7.TabIndex = 0;

			tabPageT32.Location = new Point(4, 4);
			tabPageT32.Name = "tabPageT32";
			tabPageT32.Size = new Size(333, 224);
			tabPageT32.TabIndex = 0;
			tabPageT32.Text = "tabPageT32";

			tabPageT33.Location = new Point(4, 4);
			tabPageT33.Name = "tabPageT33";
			tabPageT33.Size = new Size(333, 256);
			tabPageT33.TabIndex = 1;
			tabPageT33.Text = "tabPageT33";

			tabPageT19.Controls.Add(labelT4);
			tabPageT19.Location = new Point(4, 22);
			tabPageT19.Name = "tabPageT19";
			tabPageT19.Size = new Size(376, 294);
			tabPageT19.TabIndex = 6;
			tabPageT19.Text = "ImageList";

			tabPageT34.Controls.Add(tabControlT1);
			tabPageT34.Location = new Point(4, 22);
			tabPageT34.Name = "tabPageT34";
			tabPageT34.Size = new Size(376, 294);
			tabPageT34.TabIndex = 8;
			tabPageT34.Text = "Docked";

			tabControlT1.Controls.Add(tabPageT35);
			tabControlT1.Controls.Add(tabPageT36);
			tabControlT1.Dock = DockStyle.Fill;
			tabControlT1.Location = new Point(0, 0);
			tabControlT1.Name = "tabControlT1";
			tabControlT1.SelectedIndex = 0;
			tabControlT1.Size = new Size(376, 294);
			tabControlT1.TabIndex = 0;

			tabPageT35.Location = new Point(4, 22);
			tabPageT35.Name = "tabPageT35";
			tabPageT35.Size = new Size(368, 268);
			tabPageT35.TabIndex = 0;
			tabPageT35.Text = "tabPageT35";

			tabPageT36.Location = new Point(4, 22);
			tabPageT36.Name = "tabPageT36";
			tabPageT36.Size = new Size(208, 158);
			tabPageT36.TabIndex = 1;
			tabPageT36.Text = "tabPageT36";

			labelT3.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((System.Byte)(0)));
			labelT3.Location = new Point(112, 8);
			labelT3.Name = "label3";
			labelT3.Size = new Size(104, 24);
			labelT3.TabIndex = 1;
			labelT3.Text = "TODO:";

			labelT4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((System.Byte)(0)));
			labelT4.Location = new Point(128, 80);
			labelT4.Name = "label4";
			labelT4.Size = new Size(104, 24);
			labelT4.TabIndex = 2;
			labelT4.Text = "TODO:";

			control.Controls.Add(Docked);

		}

		private void tabControlT8_DrawItem(object sender, DrawItemEventArgs e)
		{
			Graphics g = e.Graphics;
			using (Pen p = new Pen(Color.Blue))
			{
				using (Font font = new Font("Arial", 9.0f))
				{
					using (SolidBrush brush = new SolidBrush(Color.Red))
					{
						Rectangle b = e.Bounds;
						b.Inflate(-2,-2);
						g.DrawRectangle(p, b);
						g.DrawString("OWNER", font, brush, b);
					}
				}
			}

		}

		private void AddPrimitivesTest(Control control)
		{
			control.Paint+=new PaintEventHandler(DrawPrimitives);
		}
		private void DrawPrimitives(object sender, PaintEventArgs e)
		{
			using( Brush b = new SolidBrush(Color.Red))
			{
				using( Pen p = new Pen(Color.Red))
				{
					Graphics g= e.Graphics;
					int x, y;
					DrawPrimitive(g, 0, "(0,0)-(0,10)", out x, out y);
					g.DrawLine(p,x, y, x, y+10);

					DrawPrimitive(g, 1, "(0,0)-(0,1)", out x, out y);
					g.DrawLine(p,x, y, x, y+1);

					DrawPrimitive(g, 2, "(0,0)-(0,0)", out x, out y);
					g.DrawLine(p,x, y, x, y);

					DrawPrimitive(g, 3, "DrawRect(0,0,0,0)", out x, out y);
					g.DrawRectangle(p,x, y, 0, 0);

					DrawPrimitive(g, 4, "DrawRect(0,0,0,1)", out x, out y);
					g.DrawRectangle(p,x, y, 0, 1);

					DrawPrimitive(g, 5, "DrawRect(0,0,1,1)", out x, out y);
					g.DrawRectangle(p,x, y, 1, 1);

					DrawPrimitive(g, 6, "FillRect(0,0,1,0)", out x, out y);
					g.FillRectangle(b,x, y, 1, 0);

					DrawPrimitive(g, 7, "FillRect(0,0,1,1)", out x, out y);
					g.FillRectangle(b,x, y, 1, 1);

					DrawPrimitive(g, 8, "FillRect(0,0,2,2)", out x, out y);
					g.FillRectangle(b,x, y, 2, 2);

					DrawPrimitive(g, 9, "FillPoly(0,0,0,0)", out x, out y);
					g.FillPolygon(b, new PointF[4] {new PointF(x,y), new PointF(x,y), new PointF(x,y), new PointF(x,y)});

					DrawPrimitive(g, 10, "FillPoly(0,0,0,1)", out x, out y);
					g.FillPolygon(b, new PointF[4] {new PointF(x,y), new PointF(x,y), new PointF(x,y+1),new PointF(x,y+1)});

					DrawPrimitive(g, 11, "FillPoly(0,0,1,1)", out x, out y);
					g.FillPolygon(b, new PointF[4] {new PointF(x,y), new PointF(x+1,y), new PointF(x+1,y+1),new PointF(x,y+1)});

					DrawPrimitive(g, 12, "FillPoly(0,0,2,2)", out x, out y);
					g.FillPolygon(b, new PointF[4] {new PointF(x,y), new PointF(x+2,y), new PointF(x+2,y+2), new PointF(x,y+2)});

					DrawPrimitive(g, 13, "DrawPoly(0,0,0,0)", out x, out y);
					g.DrawPolygon(p, new PointF[4] {new PointF(x,y), new PointF(x,y), new PointF(x,y), new PointF(x,y)});

					DrawPrimitive(g, 14, "DrawPoly(0,0,0,1)", out x, out y);
					g.DrawPolygon(p, new PointF[4] {new PointF(x,y), new PointF(x,y), new PointF(x,y+1),new PointF(x,y+1)});

					DrawPrimitive(g, 15, "DrawPoly(0,0,1,1)", out x, out y);
					g.DrawPolygon(p, new PointF[4] {new PointF(x,y), new PointF(x+1,y), new PointF(x+1,y+1),new PointF(x,y+1)});

					DrawPrimitive(g, 16, "DrawPoly(0,0,2,2)", out x, out y);
					g.DrawPolygon(p, new PointF[4] {new PointF(x,y), new PointF(x+2,y), new PointF(x+2,y+2), new PointF(x,y+2)});

					Font f = new Font("Arial", 10);
					DrawPrimitive(g, 17, "Arial 10", out x, out y);
					g.DrawString("Height=" + f.Height, f, b, x, y);

					DrawPrimitive(g, 18, "Arial 10", out x, out y);
					StringFormat sf = new StringFormat();
					sf.SetMeasurableCharacterRanges(new CharacterRange[] { new CharacterRange(0,1), new CharacterRange(1,1) });
					RectangleF re = g.MeasureCharacterRanges("jM",f,new Rectangle(0,0,100,100),sf)[0].GetBounds(g);
#if CONFIG_EXTENDED_NUMERICS
					g.DrawString("jM Meas:" + re.Left +"," + re.Top + "," + re.Width + "," + re.Height, f, b, x, y);
#endif
					y+=20;
					g.DrawString("jM", f, b, x, y, sf);
					re.Offset(x,y);
					g.DrawRectangle(p,Rectangle.Truncate(re));

					DrawPrimitive(g, 19, "Arial 10", out x, out y);
					re = g.MeasureCharacterRanges("jM",f,new Rectangle(0,0,100,100),sf)[1].GetBounds(g);
#if CONFIG_EXTENDED_NUMERICS
					g.DrawString("jM Meas:" + re.Left +"," + re.Top + "," + re.Width + "," + re.Height, f, b, x, y);
#endif
					y+=20;
					g.DrawString("jM", f, b, x, y, sf);
					re.Offset(x,y);
					g.DrawRectangle(p,Rectangle.Truncate(re));

					DrawPrimitive(g, 20, "Pen width - with transform", out x, out y);
					g.DrawLine(p, x, y, x + 10, y);
					g.PageScale = 2;
					g.DrawLine(p, x/2, (y + 5)/2F, (x + 10)/2F, (y + 5)/2F);
					g.PageScale = 1;
					float xScale = 3;
					float yScale = 2;
					g.ScaleTransform(xScale, yScale);
					g.DrawLine(p, x/xScale, (y + 10)/yScale, (x + 10)/xScale, (y + 10)/yScale);
					g.DrawLine(p, (x + 15)/xScale, (y + 10)/yScale, (x + 15)/xScale, (y + 20)/yScale);
					g.ResetTransform();

					Graphics g1 = this.CreateGraphics();
					f = new Font("Arial",7);
					DrawPrimitive(g, 22, "VisibleClipBounds", out x, out y);
					String s = "Pixels:"+g1.VisibleClipBounds.ToString();
					g.DrawString(s, f, b, x, y);

					g1.PageUnit = GraphicsUnit.Inch;
					s = "Inches:"+g1.VisibleClipBounds.ToString();
					g.DrawString(s, f, b, x, y + 10);

					g1.ResetTransform();
					g1.PageUnit = GraphicsUnit.Millimeter;
					s = "mm:"+g1.VisibleClipBounds.ToString();
					g.DrawString(s, f, b, x, y + 20);

					g1.ResetTransform();
					g1.PageUnit = GraphicsUnit.Point;
					s = "Point:"+g1.VisibleClipBounds.ToString();
					g.DrawString(s, f, b, x, y + 30);

					g1.ResetTransform();
					g1.PageUnit = GraphicsUnit.Document;
					s = "Document:"+g1.VisibleClipBounds.ToString();
					g.DrawString(s, f, b, x, y + 40);
				}
			}

		}

		private void DrawPrimitive( Graphics g, int offset, string s, out int x, out int y)
		{
			x = (offset%4) * 110;
			y = (int)offset / 4*80;
			using (Brush bl = new SolidBrush(Color.Blue))
			{
				g.DrawString(s,Font, bl, x, y);
				y+=20;
			}
		}

		private void AddMenuTest()
		{
			seperatorMenuItem = new MenuItem("-");
			thisMenuItem = new MenuItem("This");
			thatMenuItem = new MenuItem("That");
			otherAMenuItem = new MenuItem("OtherA");
			otherBMenuItem = new MenuItem("OtherB");
			otherCMenuItem = new MenuItem("OtherC");
			otherMenuItem = new MenuItem("Other", new MenuItem[]{otherAMenuItem, otherBMenuItem, seperatorMenuItem, otherCMenuItem});
			newMenuItem = new MenuItem("New", new MenuItem[]{thisMenuItem, thatMenuItem, otherMenuItem});
			openMenuItem = new MenuItem("Open");
			exitMenuItem = new MenuItem("Exit");
			exitMenuItem.Click +=new EventHandler(exitMenuItem_Click);
			fileMenuItem = new MenuItem("File",new MenuItem[] {newMenuItem, openMenuItem, exitMenuItem});
			cutMenuItem = new MenuItem("Cut");
			copyMenuItem = new MenuItem("Copy");
			pasteMenuItem = new MenuItem("Paste");
			editMenuItem = new MenuItem("Edit", new MenuItem[] {cutMenuItem, copyMenuItem, seperatorMenuItem, pasteMenuItem});
			aboutMenuItem = new MenuItem("About");
			helpMenuItem = new MenuItem("Help", new MenuItem[] {aboutMenuItem});
			mainMenu = new MainMenu(new MenuItem[] { fileMenuItem, editMenuItem, helpMenuItem });
			Menu = mainMenu;
		}

		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void AddGraphicsTest(Control c)
		{
			c.Paint+=new PaintEventHandler(GraphicsTestPaint);
		}
		private void GraphicsTestPaint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			boundsX = boundsY = boundsPad;
			Rectangle b;

			using (Brush bb = new SolidBrush(Color.Black), rb = new SolidBrush(Color.Red), gb = new SolidBrush(Color.Green))
			using (Pen bp = new Pen(Color.Black), rp = new Pen(Color.Red), gp = new Pen(Color.Green))
			{
				b = NextBounds(g, "DrawArc");
				g.DrawArc(rp, b.Left, b.Top, b.Width, b.Height, 45f, 90f);
				b = NextBounds(g, "DrawArc");
				g.DrawArc(rp, b.Left, b.Top, b.Width, b.Height, 45f, 270f);
				b = NextBounds(g, "DrawBezier");
				g.DrawBezier(rp, b.Left, b.Top, b.Left, b.Top + .1F * b.Height, b.Right, b.Top + .2F * b. Height, b.Right, b.Bottom );
				b = NextBounds(g, "DrawClosedCurve");
				g.DrawClosedCurve(rp, new Point[] {b.Location, new Point(b.Right, b.Top), new Point(b.Right, b.Bottom), new Point(b.Left, b.Bottom)});
				b = NextBounds(g, "DrawCurve");
				g.DrawCurve(rp, new Point[]{b.Location, new Point(b.Right, b.Top), new Point(b.Right, b.Bottom), new Point(b.Left, b.Bottom)});
				b = NextBounds(g, "DrawCurve");
				g.DrawCurve(rp, new Point[]{b.Location, new Point(b.Right, b.Top), new Point(b.Right, b.Bottom), new Point(b.Left, b.Bottom)}, 0.5F);
				b = NextBounds(g, "DrawEllipse");
				g.DrawEllipse(rp, new Rectangle(b.X, b.Y, b.Width, b.Height/2));
				b = NextBounds(g, "DrawIcon");
				//g.DrawIcon()
				b = NextBounds(g, "DrawIconUnstretched");
				//g.DrawIconUnstretched();
				b = NextBounds(g, "DrawImage");
				//g.DrawImage();
				b = NextBounds(g, "DrawImageUnscaled");
				//g.DrawImageUnscaled();
				b = NextBounds(g, "DrawLine");
				g.DrawLine(rp, b.Left, b.Top, b.Right, b.Bottom);
				b = NextBounds(g, "DrawLines");
				g.DrawLines(rp, new Point[] { b.Location, new Point (b.Right, b.Bottom), new Point(b.Right, b.Top), new Point(b.Left, b.Bottom)});
				b = NextBounds(g, "DrawPie");
				g.DrawPie(rp, b, 0, 45);
				b = NextBounds(g, "DrawPie");
				g.DrawPie(rp, b, 135, 270);
				b = NextBounds(g, "DrawPolygon");
				g.DrawPolygon(rp, new Point[] { b.Location, new Point (b.Right, b.Bottom), new Point(b.Right, b.Top), new Point(b.Left, b.Bottom)});
				b = NextBounds(g, "DrawRectangle");
				g.DrawRectangle(rp, b);
				b = NextBounds(g, "DrawRectangles");
				g.DrawRectangles(rp, new Rectangle[] { b, new Rectangle(b.Right - 5, b.Bottom - 5, 10, 10)});
				b = NextBounds(g, "FillClosedCurve");
				g.FillClosedCurve(rb, new Point[] {b.Location, new Point(b.Right, b.Top), new Point(b.Right, b.Bottom), new Point(b.Left, b.Bottom)});
				b = NextBounds(g, "FillEllipse");
				g.FillEllipse(rb, new Rectangle(b.X, b.Y, b.Width, b.Height/2));
				b = NextBounds(g, "FillPie");
				g.FillPie(rb, b, 135, 270);
				b = NextBounds(g, "FillPolygon");
				g.FillPolygon(rb, new Point[] { b.Location, new Point (b.Right, b.Bottom), new Point(b.Right, b.Top), new Point(b.Left, b.Bottom)});
				b = NextBounds(g, "FillRectangle");
				g.FillRectangle(rb, b);
				b = NextBounds(g, "FillRectangles");
				g.FillRectangles(rb, new Rectangle[] { b, new Rectangle(b.Right - 5, b.Bottom - 5, 10, 10)});
			}


			/*g.DrawString("BeginContainer", Font, bb, 0, 0);
				g.SetClip(new Rectangle(20,20, 50, 50));
				g.FillRectangle(rb, 0, 0, 100, 100);
				System.Drawing.Drawing2D.GraphicsContainer gc =  g.BeginContainer();
				g.SetClip(new Rectangle(30, 30, 50, 50));
				g.ResetClip();
				g.FillRectangle(gb, 0, 0, 100, 100);
				g.EndContainer(gc);
				*/
		}

		private void AddGraphicsPathTest(Control c)
		{
			c.Paint+=new PaintEventHandler(GraphicsPathTestPaint);
		}
		private void GraphicsPathTestPaint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			boundsX = boundsY = boundsPad;
			Rectangle b;

			using (Brush bb = new SolidBrush(Color.Black), rb = new SolidBrush(Color.Red), gb = new SolidBrush(Color.Green))
			using (Pen bp = new Pen(Color.Black), rp = new Pen(Color.Red), gp = new Pen(Color.Green))
			{
				GraphicsPath path;

				b = NextBounds(g, "DrawPath");
				path = new GraphicsPath();
				path.AddArc(b, 0, 135);
				path.AddLine(b.Left, b.Top, b.Right, b.Bottom);
				g.DrawPath(rp, path);
				b = NextBounds(g, "FillPath");
				path = new GraphicsPath();
				path.AddArc(b, 0, 135);
				path.AddLine(b.Left, b.Top, b.Right, b.Bottom);
				g.FillPath(rb, path);
			}
		}

		private void AddGraphicsDrawStringTest(Control c)
		{
			c.Paint+=new PaintEventHandler(GraphicsDrawStringTest);
		}
		private void GraphicsDrawStringTest(object sender, PaintEventArgs e)
		{
			// TODO
			Graphics g = e.Graphics;
			boundsX = boundsY = boundsPad;
			Rectangle b;

			using (Brush bb = new SolidBrush(Color.Black), rb = new SolidBrush(Color.Red), gb = new SolidBrush(Color.Green))
			using (Pen bp = new Pen(Color.Black), rp = new Pen(Color.Red), gp = new Pen(Color.Green))
			{

				string s = "Hello\r\n1234 1&2345& 1&&23456 123 12 123&45";
				string s1 = s + s;
				StringFormat sf = new StringFormat();
				Font f = new Font("Arial", 6);

				b = NextBounds(g, "DrawString");
				g.DrawString("Hello 1234 1&2345 1&&23", f, rb, b.Left, b.Top);

				b = NextBounds(g, "DrawString default");
				g.DrawString(s, f, rb, b);

				sf = new StringFormat(StringFormatFlags.NoWrap);
				b = NextBounds(g, ":NoWrap");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat(StringFormatFlags.NoWrap);
				sf.Alignment = StringAlignment.Far;
				b = NextBounds(g, ":NoWrap, Far");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.Alignment = StringAlignment.Far;
				b = NextBounds(g, ":Far");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.Alignment = StringAlignment.Center;
				b = NextBounds(g, ":Center");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.LineAlignment = StringAlignment.Far;
				b = NextBounds(g, ":LineFar");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.LineAlignment = StringAlignment.Center;
				b = NextBounds(g, ":LineCenter");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.LineAlignment = StringAlignment.Far;
				sf.Alignment = StringAlignment.Far;
				b = NextBounds(g, ":LineFar Far");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.LineAlignment = StringAlignment.Far;
				sf.Alignment = StringAlignment.Center;
				b = NextBounds(g, ":LineCenter Center");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
				b = NextBounds(g, ":Hotkey Hide");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
				b = NextBounds(g, ":Hotkey None");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				b = NextBounds(g, ":Hotkey Show");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Hide;
				b = NextBounds(g, ":Hotkey Hide");
				g.DrawString(s, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.LineLimit;
				sf.Trimming = StringTrimming.Word;
				b = NextBounds(g, ":Trim word, LineLimit");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.LineLimit;
				sf.Trimming = StringTrimming.EllipsisWord;
				b = NextBounds(g, ":Trim ellipsisWord");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.LineLimit;
				sf.Trimming = StringTrimming.Character;
				b = NextBounds(g, ":Character");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.LineLimit;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				b = NextBounds(g, ":EllipisCharacter");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.LineLimit;
				sf.Trimming = StringTrimming.EllipsisPath;
				b = NextBounds(g, ":EllipsisPath");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				b = NextBounds(g, "No Line Limit");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.LineLimit;
				b = NextBounds(g, "Line Limit");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
				b = NextBounds(g, "DirectionRightToLeft");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.DirectionVertical;
				b = NextBounds(g, "DirectionVertical");
				g.DrawString(s1, f, rb, b, sf);

				sf = new StringFormat();
				sf.FormatFlags |= StringFormatFlags.LineLimit;
				int charactersFitted, linesFilled;
				SizeF size = g.MeasureString(s1, f, b.Size, sf, out charactersFitted, out linesFilled);
				b = NextBounds(g, "MeasString cf:" + charactersFitted +", lf: " + linesFilled);
				g.DrawString(s1, f, rb, b, sf);
				using (Pen p = new Pen(Color.Green))
					g.DrawRectangle(p, b.X, b.Y, size.Width, size.Height);

				sf = new StringFormat();
				b = NextBounds(g, "Measure and Layout");
				s1 = "Hello";
				SizeF size1 = g.MeasureString(s1, f);
				b.Size = new Size((int)size1.Width+1, (int)size1.Height+1);
				g.DrawString(s1, f, rb, b, sf);
			}
		}

		private const int boundsPad = 15;
		private const int boundsSize = 80;
		private int boundsX;
		private int boundsY;
		private Rectangle NextBounds(Graphics g, string text)
		{
			Rectangle r = new Rectangle(boundsX, boundsY, boundsSize, boundsSize);
			boundsX += boundsSize + boundsPad;
			if (boundsX + boundsSize > Width - 10)
			{
				boundsX = boundsPad;
				boundsY += boundsSize + boundsPad;
			}
			g.DrawRectangle(SystemPens.ControlLightLight, r);
			g.DrawString(text, new Font("Arial", 7),SystemBrushes.ControlText, r.X, r.Bottom);
			return r;
		}

		private void AddContextTest(Control c)
		{
			contextMenuLabel1 = new Label();
			contextMenuLabel1.Location = new Point(0, 0);
			contextMenuLabel1.Text = "Right click in this tab page for context menu.";
			contextMenuLabel1.Dock = DockStyle.Top;
			c.Controls.Add(contextMenuLabel1);
			contextMenu = new ContextMenu( new MenuItem[] { cutMenuItem, copyMenuItem, seperatorMenuItem, pasteMenuItem });
			c.ContextMenu = contextMenu;
		}

		private void AddImageTest(Control c)
		{
			int gap = 10;
			int x = 10;
			Rectangle r = new Rectangle(x, 425, 70, 24);
			buttonImageLoad24bpp = new Button();
			buttonImageLoad24bpp.Text = "Load 24bpp";
			buttonImageLoad24bpp.Bounds = r;
			buttonImageLoad24bpp.Click+=new EventHandler(buttonImageLoad24bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageLoad16bpp = new Button();
			buttonImageLoad16bpp.Text = "Load 16bpp";
			buttonImageLoad16bpp.Bounds = r;
			buttonImageLoad16bpp.Click+=new EventHandler(buttonImageLoad16bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageLoad15bpp = new Button();
			buttonImageLoad15bpp.Text = "Load 15bpp";
			buttonImageLoad15bpp.Bounds = r;
			buttonImageLoad15bpp.Click+=new EventHandler(buttonImageLoad15bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageLoad8bpp = new Button();
			buttonImageLoad8bpp.Text = "Load 8bpp";
			buttonImageLoad8bpp.Bounds = r;
			buttonImageLoad8bpp.Click+=new EventHandler(buttonImageLoad8bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageLoad4bpp = new Button();
			buttonImageLoad4bpp.Text = "Load 4bpp";
			buttonImageLoad4bpp.Bounds = r;
			buttonImageLoad4bpp.Click+=new EventHandler(buttonImageLoad4bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageLoad1bpp = new Button();
			buttonImageLoad1bpp.Text = "Load 1bpp";
			buttonImageLoad1bpp.Bounds = r;
			buttonImageLoad1bpp.Click+=new EventHandler(buttonImageLoad1bpp_Click);
			r = new Rectangle(x, r.Y + r.Height + gap, r.Width, r.Height);
			buttonImageConvert24bpp = new Button();
			buttonImageConvert24bpp.Text = "-> 24bpp";
			buttonImageConvert24bpp.Bounds = r;
			buttonImageConvert24bpp.Click+=new EventHandler(buttonImageConvert24bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageConvert16bpp = new Button();
			buttonImageConvert16bpp.Text = "-> 16bpp";
			buttonImageConvert16bpp.Bounds = r;
			buttonImageConvert16bpp.Click+=new EventHandler(buttonImageConvert16bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageConvert15bpp = new Button();
			buttonImageConvert15bpp.Text = "-> 15bpp";
			buttonImageConvert15bpp.Bounds = r;
			buttonImageConvert15bpp.Click+=new EventHandler(buttonImageConvert15bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageConvert8bpp = new Button();
			buttonImageConvert8bpp.Text = "-> 8bpp";
			buttonImageConvert8bpp.Bounds = r;
			buttonImageConvert8bpp.Click+=new EventHandler(buttonImageConvert8bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageConvert4bpp = new Button();
			buttonImageConvert4bpp.Text = "-> 4bpp";
			buttonImageConvert4bpp.Bounds = r;
			buttonImageConvert4bpp.Click+=new EventHandler(buttonImageConvert4bpp_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageConvert1bpp = new Button();
			buttonImageConvert1bpp.Text = "-> 1bpp";
			buttonImageConvert1bpp.Bounds = r;
			buttonImageConvert1bpp.Click+=new EventHandler(buttonImageConvert1bpp_Click);
			r = new Rectangle(x, r.Y + r.Height + gap, r.Width, r.Height);
			buttonImageSave = new Button();
			buttonImageSave.Text = "Save";
			buttonImageSave.Bounds = r;
			buttonImageSave.Click+=new EventHandler(buttonImageSave_Click);
			r.Offset(gap + r.Width, 0);
			buttonImageLoad32bppIcon = new Button();
			buttonImageLoad32bppIcon.Text = "Ld 32b Icon";
			buttonImageLoad32bppIcon.Bounds = r;
			buttonImageLoad32bppIcon.Click+=new EventHandler(buttonImageLoad32bppIcon_Click);
			r = new Rectangle(x, r.Y + r.Height + gap, 40, 20);
			labelImageWidth = new Label();
			labelImageWidth.Text = "Width:";
			labelImageWidth.Bounds = r;
			r.Offset(gap + r.Width,0);
			textBoxImageWidth = new TextBox();
			textBoxImageWidth.Text = "64";
			textBoxImageWidth.Bounds = r;
			r.Offset(gap + r.Width,0);
			labelImageHeight = new Label();
			labelImageHeight.Text = "Height:";
			labelImageHeight.Bounds = r;
			r.Offset(gap + r.Width,0);
			textBoxImageHeight = new TextBox();
			textBoxImageHeight.Text = "64";
			textBoxImageHeight.Bounds = r;
			r.Width *=2;
			r.Offset(gap + r.Width,0);
			labelImageFileName = new Label();
			labelImageFileName.Text = "FileName:";
			labelImageFileName.Bounds = r;
			r.Offset(gap + r.Width,0);
			textBoxImageFileName = new TextBox();
			textBoxImageFileName.Text = "new";
			textBoxImageFileName.Bounds = r;
			c.Paint += new PaintEventHandler(Image_Paint);
			c.Controls.AddRange(new Control[] {buttonImageLoad24bpp, buttonImageLoad16bpp, buttonImageLoad15bpp, buttonImageLoad8bpp, buttonImageLoad4bpp, buttonImageLoad1bpp, buttonImageConvert24bpp, buttonImageConvert16bpp, buttonImageConvert15bpp, buttonImageConvert8bpp, buttonImageConvert4bpp, buttonImageConvert1bpp, buttonImageSave, buttonImageLoad32bppIcon, labelImageWidth, textBoxImageWidth, labelImageHeight,  textBoxImageHeight, labelImageFileName, textBoxImageFileName});
		}


		private void buttonImageSave_Click(object sender, EventArgs e)
		{
			textBoxImageFileName.Text = textBoxImageFileName.Text.Trim();
			int i = textBoxImageFileName.Text.IndexOf('.');
			if (i>0)
				textBoxImageFileName.Text = textBoxImageFileName.Text.Substring(0,i);
			textBoxImageFileName.Text += ".bmp";
			/*
						ImageCodecInfo bmpImageCodecInfo = null;
						ImageCodecInfo[] encoders =  ImageCodecInfo.GetImageEncoders();
						for(int j = 0; j < encoders.Length; ++j)
						{
							//Console.WriteLine(encoders[j].MimeType);
							if(encoders[j].MimeType == "image/bmp")
							{
								bmpImageCodecInfo = encoders[j];
								//break;
							}
						}
			*/
			if (imageNew != null)
				imageNew.Save(textBoxImageFileName.Text);
		}

		private void Image_Paint(object sender, PaintEventArgs e)
		{
			Image_Draw(e.Graphics);
		}

		private void Image_Draw(Graphics g)
		{
			if (imageOld != null)
			{
				using (Brush b = new SolidBrush(Color.Black))
					g.DrawString("Original: " + imageOld.PixelFormat.ToString(), Font, b, 2, 2);
				g.DrawImage(imageOld, 10, 25);
			}
			if (imageNew != null)
			{
				using (Brush b = new SolidBrush(Color.Black))
					g.DrawString("Converted: " + imageNew.PixelFormat.ToString(), Font, b, 202, 2);
				g.DrawImage(imageNew, 200, 25);
			}
		}

		private void ConvertImage(System.Drawing.Imaging.PixelFormat p)
		{
			int width = int.Parse(textBoxImageWidth.Text);
			int height = int.Parse(textBoxImageHeight.Text);
			Region r = new Region();
			r.MakeEmpty();
			if (imageNew != null)
				r = new Region(new Rectangle(200, 25, imageNew.Width, imageNew.Height));
			imageNew = new Bitmap(width, height, p);
			r.Union(new Rectangle(200, 25, imageNew.Width, imageNew.Height));
			r.Union(new Rectangle(0,0, 400, 25));
			try
			{
				using (Graphics g = Graphics.FromImage(imageNew))
					g.DrawImage(imageOld, 0, 0, width, height);
			}
			catch
			{
				imageNew.Dispose();
				imageNew = null;
			}
			tabPage10.Invalidate(r, true);
		}

		private void LoadImage(String file)
		{
			Region r = new Region();
			r.MakeEmpty();
			if (imageOld != null)
				r = new Region(new Rectangle(10, 25, imageOld.Width, imageOld.Height));
			imageOld = new Bitmap(this.GetType(), file);//Image.FromFile(file);
			r.Union(new Rectangle(10, 25, imageOld.Width, imageOld.Height));
			r.Union(new Rectangle(0,0, 400, 25));
			textBoxImageWidth.Text = imageOld.Width.ToString();
			textBoxImageHeight.Text = imageOld.Height.ToString();
			tabPage10.Invalidate(r, true);
		}

		private void buttonImageLoad24bpp_Click(object sender, EventArgs e)
		{
			LoadImage("test.bmp");
		}

		private void buttonImageLoad16bpp_Click(object sender, EventArgs e)
		{
			LoadImage("test 16bpp.bmp");
		}

		private void buttonImageLoad15bpp_Click(object sender, EventArgs e)
		{
			LoadImage("test 15bpp.bmp");
		}

		private void buttonImageLoad8bpp_Click(object sender, EventArgs e)
		{
			LoadImage("test 8bpp.bmp");
		}

		private void buttonImageLoad4bpp_Click(object sender, EventArgs e)
		{
			LoadImage("test 4bpp.bmp");
		}

		private void buttonImageLoad1bpp_Click(object sender, EventArgs e)
		{
			LoadImage("test 1bpp.bmp");
		}

		private void buttonImageLoad32bppIcon_Click(object sender, EventArgs e)
		{
			LoadImage("test.ico");
		}

		private void buttonImageConvert24bpp_Click(object sender, EventArgs e)
		{
			ConvertImage(System.Drawing.Imaging.PixelFormat.Format24bppRgb);
		}

		private void buttonImageConvert16bpp_Click(object sender, EventArgs e)
		{
			ConvertImage(System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
		}

		private void buttonImageConvert15bpp_Click(object sender, EventArgs e)
		{
			ConvertImage(System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
		}

		private void buttonImageConvert8bpp_Click(object sender, EventArgs e)
		{
			ConvertImage(System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
		}

		private void buttonImageConvert4bpp_Click(object sender, EventArgs e)
		{
			ConvertImage(System.Drawing.Imaging.PixelFormat.Format4bppIndexed);
		}

		private void buttonImageConvert1bpp_Click(object sender, EventArgs e)
		{
			ConvertImage(System.Drawing.Imaging.PixelFormat.Format1bppIndexed);
		}

		private void AddComboTest(Control c)
		{
			comboBox1 = new ComboBox();
			comboBox2 = new ComboBox();
			comboBox3 = new ComboBox();
			comboBox4 = new ComboBox();
			comboBox5 = new ComboBox();
			comboBox6 = new ComboBox();
			comboBox7 = new ComboBox();
			comboBox1.Location = new Point(10, 10);
			comboBox2.Location = new Point(10, 40);
			comboBox3.Location = new Point(10, 70);
			comboBox4.Location = new Point(10, 100);
			comboBox5.Location = new Point(10, 130);
			comboBox6.Location = new Point(10, 160);
			comboBox7.Location = new Point(10, 190);

			comboBox1.Enabled = false;
			comboBox1.Items.AddRange(new object[] { "Item", });

			// Add test items
			comboBox2.BeginUpdate();
			comboBox3.BeginUpdate();
			comboBox4.BeginUpdate();
			comboBox5.BeginUpdate();
			comboBox6.BeginUpdate();
			comboBox7.BeginUpdate();
			for (int i = 0; i < 100; i++)
			{
				String s = "Item " + i.ToString();
				if ((i & 4) == 0)
					s += "wide wide wide wide";
				comboBox2.Items.Add(s);
				comboBox3.Items.Add(s);
				comboBox4.Items.Add(s);
				comboBox5.Items.Add(s);
				comboBox6.Items.Add(s);
				comboBox7.Items.Add(s);
			}
			comboBox2.EndUpdate();
			comboBox3.EndUpdate();
			comboBox4.EndUpdate();
			comboBox5.EndUpdate();
			comboBox6.EndUpdate();
			comboBox7.EndUpdate();

			comboBox3.BackColor = Color.Blue;
			comboBox3.ForeColor = Color.White;
			comboBox3.DropDownWidth = 200;
			comboBox3.ItemHeight = 30;
			comboBox3.MaxDropDownItems = 20;
			comboBox3.MaxLength = 2;

			comboBox4.DrawMode = DrawMode.OwnerDrawFixed;
			comboBox4.MeasureItem+=new MeasureItemEventHandler(comboBoxMeasureItem);
			comboBox4.DrawItem+=new DrawItemEventHandler(comboBoxDrawItem);
			comboBox5.MeasureItem+=new MeasureItemEventHandler(comboBoxMeasureItem);
			comboBox5.DrawItem+=new DrawItemEventHandler(comboBoxDrawItem);
			comboBox5.DrawMode = DrawMode.OwnerDrawVariable;
			comboBox6.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox7.DropDownStyle = ComboBoxStyle.Simple;

			c.Controls.AddRange(new Control[] {comboBox1, comboBox2, comboBox3, comboBox4, comboBox5, comboBox6, comboBox7});

		}

		private void comboBoxMeasureItem(object sender, MeasureItemEventArgs e)
		{
			e.ItemWidth = (e.Index * 10) % 100;
			e.ItemHeight = (e.Index * 30 + 10) % 50;
		}

		private void comboBoxDrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index % 5 == 0)
			{
				e.DrawBackground();
			}
			else
			{
				using (Brush b = new SolidBrush(Color.FromArgb((e.Index * 20 + 128) % 256, 160, 160)))
					e.Graphics.FillRectangle(b, e.Bounds);
			}
			if (e.Index % 10 == 9)
			{
				e.DrawFocusRectangle();
			}
			String s = e.State.ToString() + " ";
			using (Brush b = new SolidBrush(Color.FromArgb(128, (e.Index * 20) % 128, 128)))
				e.Graphics.DrawString(s + s,e.Font, b, e.Bounds);
			e.Graphics.DrawRectangle(SystemPens.ControlText, e.Bounds);
		}

		int pos = 0;

		private void AddTreeViewTest(Control c)
		{
			treeImageList = new ImageList();
			treeImageList.ColorDepth = ColorDepth.Depth24Bit;
			treeImageList.Images.Add(new Icon(this.GetType(), "small_folder.ico"));
			treeImageList.Images.Add(new Icon(this.GetType(), "small_text.ico"));
			treeView1 = new TreeView();
			treeView1.BeginUpdate();
			treeView1.ImageList = treeImageList;
			treeView1.ImageIndex = -1;
			treeView1.SelectedImageIndex = 1;
			AddTestNodes(ref pos);
			treeView1.EndUpdate();

			treeView1.Bounds = new Rectangle(10,10, 150, 400);
			treeView1.LabelEdit = true;
			treeView1.AfterSelect+=new TreeViewEventHandler(treeView1_AfterSelect);
			treeView1.MouseMove+=new MouseEventHandler(treeView1_MouseMove);

			treeLabelBoundsDescription = new Label();
			treeLabelBoundsDescription.Bounds = new Rectangle(treeView1.Right + 10, 10, 150, 20);
			treeLabelBoundsDescription.Text = "SelectedNode.Bounds:";
			treeLabelBounds = new Label();
			treeLabelBounds.Bounds = new Rectangle(treeLabelBoundsDescription.Left + 5, treeLabelBoundsDescription.Bottom + 5, 200, 20);

			treeLabelCheckBox = new Label();
			treeLabelCheckBox.Text = "CheckBoxes:";
			treeLabelCheckBox.Bounds = new Rectangle(treeLabelBoundsDescription.Left, treeLabelBounds.Bottom + 5, 80, 20);
			treeCheckBox = new CheckBox();
			treeCheckBox.Location = new Point(treeLabelCheckBox.Right, treeLabelCheckBox.Top);
			treeCheckBox.CheckedChanged+=new EventHandler(treeCheckBox_CheckedChanged);

			treeLabelRightClickDescription = new Label();
			treeLabelRightClickDescription.Text = "Right Click nodes for menu..";
			treeLabelRightClickDescription.Bounds = new Rectangle(treeLabelBoundsDescription.Left, treeLabelCheckBox.Bottom + 5, 200, 20);

			treeButtonAddNodes = new Button();
			treeButtonAddNodes.Text = "Add 1000 more Nodes!!";
			treeButtonAddNodes.Bounds = new Rectangle(treeLabelBoundsDescription.Left, treeLabelRightClickDescription.Bottom + 5, 150, 20);
			treeButtonAddNodes.Click+=new EventHandler(treeButtonAddNodes_Click);

			MenuItem edit = new MenuItem("Edit", new EventHandler(treeEdit));
			MenuItem add = new MenuItem("Add", new EventHandler(treeAdd));
			MenuItem delete = new MenuItem("Delete", new EventHandler(treeDelete));
			treeMenu = new ContextMenu(new MenuItem[] {edit, add, delete});
			treeView1.ContextMenu = treeMenu;

			c.Controls.AddRange(new Control[] {treeView1, treeLabelBounds, treeLabelBoundsDescription, treeLabelCheckBox, treeCheckBox, treeLabelRightClickDescription, treeButtonAddNodes});
		}

		private void treeEdit(Object sender, EventArgs e)
		{
			treeView1.SelectedNode.BeginEdit();
		}

		private void treeAdd(Object sender, EventArgs e)
		{
			TreeNode node = treeView1.SelectedNode.Nodes.Add("new node..");
			treeView1.SelectedNode = node;
		}

		private void treeDelete(Object sender, EventArgs e)
		{
			treeView1.SelectedNode.Remove();
		}

		private void treeButtonAddNodes_Click(object sender, EventArgs e)
		{
			treeView1.BeginUpdate();
			int endPos = pos + 1000;
			while (pos < endPos)
			{
				AddTestNodes(ref pos);
			}
			treeView1.EndUpdate();
		}

		private void AddTestNodes(ref int pos)
		{
			pos++;
			const String n = "Node";
			TreeNode node1 = treeView1.Nodes.Add(n + pos++);
			node1.ImageIndex = 1;
			TreeNode node11 = node1.Nodes.Add(n + pos++);
			TreeNode node111 = node11.Nodes.Add(n + pos++);
			TreeNode node1111 = node111.Nodes.Add(n + pos++);
			node1111.Nodes.Add(n + pos++);
			node1.Nodes.Add(n + pos++);
			treeView1.Nodes.Add(n + pos++);
			treeView1.Nodes.Add(n + pos++);
			TreeNode node4 = treeView1.Nodes.Add(n + pos++);
			TreeNode node41 = node4.Nodes.Add(n + pos++);
			node41.Nodes.Add(n + pos++);
			node41.Nodes.Add(n + pos++);
			node4.Nodes.Add(n + pos++);
			node4.Nodes.Add(n + pos++);
		}

		private void treeCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			treeView1.CheckBoxes = treeCheckBox.Checked;
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			treeLabelBounds.Text = treeView1.SelectedNode.Bounds.ToString();
		}

		private void treeView1_MouseMove(object sender, MouseEventArgs e)
		{
			TreeNode node = treeView1.GetNodeAt(e.X, e.Y);
			if (node == null)
				treeLabelBounds.Text = String.Empty;
			else
				treeLabelBounds.Text = node.Text;
		}

		private void AddListBoxTest(Control c)
		{
			return;
			listBox1 = new ListBox();
			listBox1.Bounds = new Rectangle(10, 10, 200, 100);
			listBox2 = new ListBox();
			listBox2.Bounds = new Rectangle(10, 150, 200, 100);
			c.Controls.Add(listBox1);
			c.Controls.Add(listBox2);
			listBox1.MultiColumn = true;
			listBox1.SelectionMode = SelectionMode.MultiExtended;
			listBox1.ScrollAlwaysVisible = true;
			listBox1.BeginUpdate();
			for (int x = 1; x <= 50; x++)
			{
				listBox1.Items.Add("Item " + x.ToString());
			}
			listBox1.EndUpdate();
			listBox1.SetSelected(1, true);
			listBox1.SetSelected(3, true);
			listBox1.SetSelected(5, true);

			//Console.WriteLine(listBox1.SelectedItems[1].ToString());
			//Console.WriteLine(listBox1.SelectedIndices[0].ToString());

			listBox2.Items.Add("Item 1");
			listBox2.Items.Add("Item 2aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
			listBox2.Enabled = false;

		}

		private void AddFormsTest(Control c)
		{
			formsButton1 = new Button();
			formsButton1.Bounds = new Rectangle(20, 20, 120, 20);
			formsButton1.Text = "New Normal &Form";
			formsButton1.Click+=new EventHandler(FormsTestClick);

			formsButton2 = new Button();
			formsButton2.Bounds = new Rectangle(20, 50, 120, 20);
			formsButton2.Text = "Restrict Min Max";
			formsButton2.Click+=new EventHandler(FormsMinMax);

			formsButton3 = new Button();
			formsButton3.Bounds = new Rectangle(20, 80, 120, 20);
			formsButton3.Text = "Maximize";
			formsButton3.Click+=new EventHandler(FormsMaximize);

			formsButton4 = new Button();
			formsButton4.Bounds = new Rectangle(20, 110, 120, 20);
			formsButton4.Text = "Minimize";
			formsButton4.Click+=new EventHandler(FormsMinimize);

			formsButton5 = new Button();
			formsButton5.Bounds = new Rectangle(20, 140, 120, 20);
			formsButton5.Text = "Restore";
			formsButton5.Click+=new EventHandler(FormsRestore);

			formsButton6 = new Button();
			formsButton6.Bounds = new Rectangle(20, 170, 120, 20);
			formsButton6.Text = "Set Icon";
			formsButton6.Click+=new EventHandler(FormsSetIcon);

			c.Controls.AddRange(new Control[] {formsButton1, formsButton2, formsButton3, formsButton4, formsButton5, formsButton6});
		}

		private void FormsTestClick(object sender, EventArgs e)
		{
			Form f = new Form();
			f.Show();
		}

		private void FormsMinMax(object sender, EventArgs e)
		{
			if (MaximumSize == Size.Empty)
			{
				MaximumSize = new Size(400, 600);
				MinimumSize = new Size(200, 300);
			}
			else
				MinimumSize = MaximumSize = Size.Empty;
		}

		private void FormsMaximize(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Maximized;
		}

		private void FormsMinimize(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Minimized;
		}

		private void FormsRestore(object sender, EventArgs e)
		{
			WindowState = FormWindowState.Normal;
		}

		private void FormsSetIcon(object sender, EventArgs e)
		{
			Icon = new Icon(this.GetType(), "small_folder.ico");
		}

		private void AddTransformsTest(Control c)
		{
			transformTestPoints = new PointF[24]
			{
				new PointF(5, 0), new PointF(25, 0),
				new PointF(25, 0), new PointF(30, 5),
				new PointF(30, 5), new PointF(30, 25),
				new PointF(30, 25), new PointF(25, 30),
				new PointF(25, 30), new PointF(5, 30),
				new PointF(5, 30), new PointF(0, 25),
				new PointF(0, 25), new PointF(0, 5),
				new PointF(0, 5), new PointF(5, 0),
				new PointF(15, 10), new PointF(15, 15),
				new PointF(5, 20), new PointF(10, 25),
				new PointF(10, 25), new PointF(20, 25),
				new PointF(20, 25), new PointF(25, 20),
			};

			for (int i = 0; i < transformTestPoints.Length;i++)
			{
				transformTestPoints[i].X -= 30;
				transformTestPoints[i].Y -= 30;
			}

			c.Paint+=new PaintEventHandler(TransformsTestPaint);
			timerTransform = new Timer();
			timerTransform.Interval = 20;
			timerTransform.Tick +=new EventHandler(t_Tick);
			timerTransform.Start();
		}

		private void t_Tick(object sender, EventArgs e)
		{
			if (tabControl1.SelectedTab == tabPage15)
				using (Graphics g = tabPage15.CreateGraphics())
					TransformsTestDraw(g);
		}

		private void TransformsTestPaint(object sender, PaintEventArgs e)
		{
			TransformsTestDraw(e.Graphics);
		}
		private void TransformsTestDraw(Graphics g)
		{
			g.FillRectangle(SystemBrushes.Control, tabPage15.ClientRectangle);
			boundsX = boundsY = boundsPad;
			using (Pen p = new Pen(Color.Blue))
			{

				int mid = (Height-50)/2;
				if (mid > (Width-10)/2)
					mid = (Width-10)/2;

				PointF[] f = (PointF[])transformTestPoints.Clone();
				g.RotateTransform(transformRotation);
				g.TranslateTransform(transformX, transformY);
				g.ScaleTransform(transformScaleX, transformScaleY);
				g.TransformPoints(CoordinateSpace.Page,
					CoordinateSpace.World,
					f);

				using (Brush b = new SolidBrush(Color.CadetBlue))
				{
					g.FillEllipse(b, -20, -20, 20, 10);
					g.DrawString("Hello", Font, b, 0, 0);
				}
				g.ResetTransform();

				if (transformX < -200 || transformX > 200)
				{
					transformXOffset = -transformXOffset;
					transformScaleXOffset = -transformScaleXOffset;
				}

				if (transformY < -200 || transformY > 200)
				{
					transformYOffset = -transformYOffset;
					transformScaleYOffset = -transformScaleYOffset;
				}

				for (int i = 0; i < transformTestPoints.Length; i+=2)
					g.DrawLine(p, f[i].X + mid, f[i].Y + mid, f[i+1].X + mid, f[i+1].Y + mid);

				transformX += transformXOffset;
				transformY += transformYOffset;
				transformRotation += transformRotationOffSet;
				transformScaleX += transformScaleXOffset;
				transformScaleY += transformScaleYOffset;

				/*				Font f = new Font("Arial", 6);

								Rectangle b = NextBounds(g, "Rectangle");
								Rectangle b1 = new Rectangle(b.Left + 5, b.Top + 5, b.Width - 10, b.Height - 10);
								g.DrawRectangle(p, b1);

								g.RotateTransform(45);
								b = NextBounds(g, "RotateTransform(45)");
								b1 = new Rectangle(b.Left + 5, b.Top + 5, b.Width - 10, b.Height - 10);
								g.DrawRectangle(p, b1);
				*/

			}

		}

		private void AddScrollbarTest(Control c)
		{
			vScrollBar = new VScrollBar();
			vScrollBar.Bounds = new Rectangle(10, 10, 20, 400);
			vScrollBar.ValueChanged+=new EventHandler(vScrollBar_ValueChanged);
			c.Controls.Add(vScrollBar);

			hScrollBar = new HScrollBar();
			hScrollBar.Bounds = new Rectangle(200, 10, 250, 20);
			hScrollBar.ValueChanged+=new EventHandler(hScrollBar_ValueChanged);
			c.Controls.Add(hScrollBar);
			scroll1LabelMin = new Label();
			scroll1LabelMin.Text = "min";
			scroll1LabelMin.Bounds = new Rectangle(vScrollBar.Right + 10, vScrollBar.Top + 100, 35, 20);
			c.Controls.Add(scroll1LabelMin);
			scroll1TextBoxMin = new TextBox();
			scroll1TextBoxMin.Text = vScrollBar.Minimum.ToString();
			scroll1TextBoxMin.Bounds = new Rectangle(scroll1LabelMin.Right + 5, scroll1LabelMin.Top, 30, 20);
			scroll1TextBoxMin.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll1TextBoxMin);

			scroll1LabelMax = new Label();
			scroll1LabelMax.Text = "max";
			scroll1LabelMax.Bounds = new Rectangle(scroll1LabelMin.Left, scroll1LabelMin.Bottom + 10, 35, 20);
			c.Controls.Add(scroll1LabelMax);
			scroll1TextBoxMax = new TextBox();
			scroll1TextBoxMax.Text = vScrollBar.Maximum.ToString();
			scroll1TextBoxMax.Bounds = new Rectangle(scroll1LabelMax.Right + 5, scroll1LabelMax.Top, 30, 20);
			scroll1TextBoxMax.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll1TextBoxMax);

			scroll1LabelValue = new Label();
			scroll1LabelValue.Text = "value";
			scroll1LabelValue.Bounds = new Rectangle(scroll1LabelMax.Left, scroll1LabelMax.Bottom + 10, 35, 20);
			c.Controls.Add(scroll1LabelValue);
			scroll1TextBoxValue = new TextBox();
			scroll1TextBoxValue.Text = vScrollBar.Value.ToString();
			scroll1TextBoxValue.Bounds = new Rectangle(scroll1LabelValue.Right + 5, scroll1LabelValue.Top, 30, 20);
			scroll1TextBoxValue.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll1TextBoxValue);

			scroll1LabelLarge = new Label();
			scroll1LabelLarge.Text = "large";
			scroll1LabelLarge.Bounds = new Rectangle(scroll1LabelValue.Left, scroll1LabelValue.Bottom + 10, 35, 20);
			c.Controls.Add(scroll1LabelLarge);
			scroll1TextBoxLarge = new TextBox();
			scroll1TextBoxLarge.Text = vScrollBar.LargeChange.ToString();
			scroll1TextBoxLarge.Bounds = new Rectangle(scroll1LabelLarge.Right + 5, scroll1LabelLarge.Top, 30, 20);
			scroll1TextBoxLarge.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll1TextBoxLarge);

			scroll1LabelSmall = new Label();
			scroll1LabelSmall.Text = "small";
			scroll1LabelSmall.Bounds = new Rectangle(scroll1LabelLarge.Left, scroll1LabelLarge.Bottom + 10, 35, 20);
			c.Controls.Add(scroll1LabelSmall);
			scroll1TextBoxSmall = new TextBox();
			scroll1TextBoxSmall.Text = vScrollBar.SmallChange.ToString();
			scroll1TextBoxSmall.Bounds = new Rectangle(scroll1LabelSmall.Right + 5, scroll1LabelSmall.Top, 30, 20);
			scroll1TextBoxSmall.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll1TextBoxSmall);

			scroll2LabelMin = new Label();
			scroll2LabelMin.Text = "min";
			scroll2LabelMin.Bounds = new Rectangle(hScrollBar.Left + 90, hScrollBar.Bottom + 10, 35, 20);
			c.Controls.Add(scroll2LabelMin);
			scroll2TextBoxMin = new TextBox();
			scroll2TextBoxMin.Text = hScrollBar.Minimum.ToString();
			scroll2TextBoxMin.Bounds = new Rectangle(scroll2LabelMin.Right + 5, scroll2LabelMin.Top, 30, 20);
			scroll2TextBoxMin.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll2TextBoxMin);

			scroll2LabelMax = new Label();
			scroll2LabelMax.Text = "max";
			scroll2LabelMax.Bounds = new Rectangle(scroll2LabelMin.Left, scroll2LabelMin.Bottom + 10, 35, 20);
			c.Controls.Add(scroll2LabelMax);
			scroll2TextBoxMax = new TextBox();
			scroll2TextBoxMax.Text = hScrollBar.Maximum.ToString();
			scroll2TextBoxMax.Bounds = new Rectangle(scroll2LabelMax.Right + 5, scroll2LabelMax.Top, 30, 20);
			scroll2TextBoxMax.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll2TextBoxMax);

			scroll2LabelValue = new Label();
			scroll2LabelValue.Text = "value";
			scroll2LabelValue.Bounds = new Rectangle(scroll2LabelMax.Left, scroll2LabelMax.Bottom + 10, 35, 20);
			c.Controls.Add(scroll2LabelValue);
			scroll2TextBoxValue = new TextBox();
			scroll2TextBoxValue.Text = hScrollBar.Value.ToString();
			scroll2TextBoxValue.Bounds = new Rectangle(scroll2LabelValue.Right + 5, scroll2LabelValue.Top, 30, 20);
			scroll2TextBoxValue.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll2TextBoxValue);

			scroll2LabelLarge = new Label();
			scroll2LabelLarge.Text = "large";
			scroll2LabelLarge.Bounds = new Rectangle(scroll2LabelValue.Left, scroll2LabelValue.Bottom + 10, 35, 20);
			c.Controls.Add(scroll2LabelLarge);
			scroll2TextBoxLarge = new TextBox();
			scroll2TextBoxLarge.Text = vScrollBar.LargeChange.ToString();
			scroll2TextBoxLarge.Bounds = new Rectangle(scroll2LabelLarge.Right + 5, scroll2LabelLarge.Top, 30, 20);
			scroll2TextBoxLarge.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll2TextBoxLarge);

			scroll2LabelSmall = new Label();
			scroll2LabelSmall.Text = "small";
			scroll2LabelSmall.Bounds = new Rectangle(scroll2LabelLarge.Left, scroll2LabelLarge.Bottom + 10, 35, 20);
			c.Controls.Add(scroll2LabelSmall);
			scroll2TextBoxSmall = new TextBox();
			scroll2TextBoxSmall.Text = vScrollBar.SmallChange.ToString();
			scroll2TextBoxSmall.Bounds = new Rectangle(scroll2LabelSmall.Right + 5, scroll2LabelSmall.Top, 30, 20);
			scroll2TextBoxSmall.TextChanged+=new EventHandler(scrollTextBox_TextChanged);
			c.Controls.Add(scroll2TextBoxSmall);

		}

		private void vScrollBar_ValueChanged(object sender, EventArgs e)
		{
			scroll1TextBoxValue.Text = vScrollBar.Value.ToString();
		}

		private void hScrollBar_ValueChanged(object sender, EventArgs e)
		{
			scroll2TextBoxValue.Text = hScrollBar.Value.ToString();
		}

		private void scrollTextBox_TextChanged(object sender, EventArgs e)
		{
			try //Lazy!!
			{
				vScrollBar.Minimum = int.Parse(scroll1TextBoxMin.Text);
				vScrollBar.Maximum = int.Parse(scroll1TextBoxMax.Text);
				vScrollBar.Value = int.Parse(scroll1TextBoxValue.Text);
				vScrollBar.LargeChange = int.Parse(scroll1TextBoxLarge.Text);
				vScrollBar.SmallChange = int.Parse(scroll1TextBoxSmall.Text);
				hScrollBar.Minimum = int.Parse(scroll2TextBoxMin.Text);
				hScrollBar.Maximum = int.Parse(scroll2TextBoxMax.Text);
				hScrollBar.Value = int.Parse(scroll2TextBoxValue.Text);
				hScrollBar.LargeChange = int.Parse(scroll2TextBoxLarge.Text);
				hScrollBar.SmallChange = int.Parse(scroll2TextBoxSmall.Text);
			}
			catch
			{}

		}

		private void AddPropertyGridTest(Control c)
		{
			/*propertyGrid = new PropertyGrid();
			propertyGrid.CommandsVisibleIfAvailable = true;
			propertyGrid.Location = new Point(50, 20);
			propertyGrid.Size = new System.Drawing.Size(400, 500);
			propertyGrid.Text = "Property Grid";

			c.Controls.Add(propertyGrid);
			propertyGrid.SelectedObject = propertyGrid;
			propertyGrid.DumpPropsToConsole();*/

		}

		private void AddPictureBoxTest(Control c)
		{
			pictureBox1 = new PictureBox();
			pictureBox1.BorderStyle = BorderStyle.Fixed3D;
			pictureBox1.Bounds = new Rectangle(10,10,100,100);
			pictureBox1.Image = new Bitmap(this.GetType(),"test.bmp");
			c.Controls.Add(pictureBox1);
			pictureBox2 = new PictureBox();
			pictureBox2.BorderStyle = BorderStyle.FixedSingle;
			pictureBox2.Bounds = new Rectangle(150,10,100,100);
			pictureBox2.Image = pictureBox1.Image;
			c.Controls.Add(pictureBox2);
			pictureBox3 = new PictureBox();
			pictureBox3.BorderStyle = BorderStyle.FixedSingle;
			pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
			pictureBox3.Bounds = new Rectangle(10,150,120,120);
			pictureBox3.Image =  pictureBox1.Image;
			c.Controls.Add(pictureBox3);
			pictureBox4 = new PictureBox();
			pictureBox4.BorderStyle = BorderStyle.FixedSingle;
			pictureBox4.SizeMode = PictureBoxSizeMode.AutoSize;
			pictureBox4.Bounds = new Rectangle(150,150,120,100);
			pictureBox4.Image =  pictureBox1.Image;
			c.Controls.Add(pictureBox4);
			pictureBox5 = new PictureBox();
			pictureBox5.BorderStyle = BorderStyle.FixedSingle;
			pictureBox5.SizeMode = PictureBoxSizeMode.StretchImage;
			pictureBox5.Bounds = new Rectangle(10,300,120,140);
			pictureBox5.Image =  pictureBox1.Image;
			c.Controls.Add(pictureBox5);
		}

		private void AddControlPaintTest(Control c)
		{
			c.Paint+=new PaintEventHandler(ControlPaintTest_Paint);
		}

		private void ControlPaintTest_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			boundsX = boundsY = boundsPad;
			Rectangle b;

			using (Brush bg = new SolidBrush(Color.Blue), rb = new SolidBrush(Color.Red), gb = new SolidBrush(Color.Green))
			using (Pen bp = new Pen(Color.Black), rp = new Pen(Color.Red), gp = new Pen(Color.Green))
			{
				g.FillRectangle(bg, e.ClipRectangle);

				b = NextBoundsPaint(g, "DrawBorder - Solid");
				ControlPaint.DrawBorder(g, b, Color.Red, ButtonBorderStyle.Solid);
				b = NextBoundsPaint(g, "- Inset");
				ControlPaint.DrawBorder(g, b, Color.Red, ButtonBorderStyle.Inset);
				b = NextBoundsPaint(g, "- Outset");
				ControlPaint.DrawBorder(g, b, Color.Red, ButtonBorderStyle.Outset);
				b = NextBoundsPaint(g, "- Dashed");
				ControlPaint.DrawBorder(g, b, Color.Red, ButtonBorderStyle.Dashed);
				b = NextBoundsPaint(g, "- Dotted");
				ControlPaint.DrawBorder(g, b, Color.Red, ButtonBorderStyle.Dotted);
				b = NextBoundsPaint(g, "- Solid");
				ControlPaint.DrawBorder(g, b, Color.Red, ButtonBorderStyle.Solid);
				b = NextBoundsPaint(g, "- None");
				ControlPaint.DrawBorder(g, b, Color.Red, ButtonBorderStyle.None);

				b = NextBoundsPaint(g, "DrawBorder3D");
				ControlPaint.DrawBorder3D(g, b);
				b = NextBoundsPaint(g, "- Left Adjust");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.Adjust, Border3DSide.Left);
				b = NextBoundsPaint(g, "- Top Bump");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.Bump, Border3DSide.Top);
				b = NextBoundsPaint(g, "- Right Etched");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.Etched, Border3DSide.Right);
				b = NextBoundsPaint(g, "- Bottom Flat");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.Flat, Border3DSide.Bottom);
				b = NextBoundsPaint(g, "- Raised");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.Raised, Border3DSide.All);
				b = NextBoundsPaint(g, "- RaisedInner");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.RaisedInner, Border3DSide.All);
				b = NextBoundsPaint(g, "- RaisedOuter");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.RaisedOuter, Border3DSide.All);
				b = NextBoundsPaint(g, "- Sunken");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.Sunken, Border3DSide.All);
				b = NextBoundsPaint(g, "- SunkenInner");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.SunkenInner, Border3DSide.All);
				b = NextBoundsPaint(g, "- SunkenOuter");
				ControlPaint.DrawBorder3D(g, b, Border3DStyle.SunkenOuter, Border3DSide.All);

				b = NextBoundsPaint(g, "DrawButton - Pushed");
				ControlPaint.DrawButton(g, b, ButtonState.Pushed);
				b = NextBoundsPaint(g, "- Normal");
				ControlPaint.DrawButton(g, b, ButtonState.Normal);
				b = NextBoundsPaint(g, "- Inactive");
				ControlPaint.DrawButton(g, b, ButtonState.Inactive);
				b = NextBoundsPaint(g, "- Flat");
				ControlPaint.DrawButton(g, b, ButtonState.Flat);
				b = NextBoundsPaint(g, "- Checked");
				ControlPaint.DrawButton(g, b, ButtonState.Checked);
				b = NextBoundsPaint(g, "- All");
				ControlPaint.DrawButton(g, b, ButtonState.All);

				b = NextBoundsPaint(g, "DrawCaptionB - Close");
				ControlPaint.DrawCaptionButton(g,b, CaptionButton.Close, ButtonState.Normal);
				b = NextBoundsPaint(g, "- Help Pushed");
				ControlPaint.DrawCaptionButton(g,b, CaptionButton.Help, ButtonState.Pushed);
				b = NextBoundsPaint(g, "- Max Inactive");
				ControlPaint.DrawCaptionButton(g,b, CaptionButton.Maximize, ButtonState.Inactive);
				b = NextBoundsPaint(g, "- Minimize");
				ControlPaint.DrawCaptionButton(g,b, CaptionButton.Minimize, ButtonState.Normal);
				b = NextBoundsPaint(g, "- Restore");
				ControlPaint.DrawCaptionButton(g,b, CaptionButton.Restore, ButtonState.Normal);

				b = NextBoundsPaint(g, "DCheckBox");
				ControlPaint.DrawCheckBox(g, new Rectangle(b.X, b.Y, 20, 20),ButtonState.Normal );
				b = NextBoundsPaint(g, "- Pushed");
				ControlPaint.DrawCheckBox(g, new Rectangle(b.X, b.Y, 20, 20),ButtonState.Pushed );
				b = NextBoundsPaint(g, "- Inactive");
				ControlPaint.DrawCheckBox(g, new Rectangle(b.X, b.Y, 20, 20),ButtonState.Inactive );
				b = NextBoundsPaint(g, "- Flat");
				ControlPaint.DrawCheckBox(g, new Rectangle(b.X, b.Y, 20, 20),ButtonState.Flat );
				b = NextBoundsPaint(g, "- Checked");
				ControlPaint.DrawCheckBox(g, new Rectangle(b.X, b.Y, 20, 20),ButtonState.Checked );
				b = NextBoundsPaint(g, "- All");
				ControlPaint.DrawCheckBox(g, new Rectangle(b.X, b.Y, 20, 20),ButtonState.All );

				b = NextBoundsPaint(g, "DComboButton");
				ControlPaint.DrawComboButton(g, b, ButtonState.Normal);
				b = NextBoundsPaint(g, "- Pushed");
				ControlPaint.DrawComboButton(g, b, ButtonState.Pushed);
				b = NextBoundsPaint(g, "- Inactive");
				ControlPaint.DrawComboButton(g, b, ButtonState.Inactive);

				b = NextBoundsPaint(g, "DContGrabHand");
				ControlPaint.DrawContainerGrabHandle(g, new Rectangle(b.X, b.Y, 20, 20));

				b = NextBoundsPaint(g, "dFocusRectangle");
				ControlPaint.DrawFocusRectangle(g, new Rectangle(b.X, b.Y, 20, 20));
				b = NextBoundsPaint(g, " - Green/Yellow");
				ControlPaint.DrawFocusRectangle(g, new Rectangle(b.X, b.Y, 20, 20), Color.Green, Color.Yellow);
			}

		}


		private void AddControlPaintTest2(Control c)
		{
			c.Paint+=new PaintEventHandler(ControlPaintTest2_Paint);
		}

		private void ControlPaintTest2_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			boundsX = boundsY = boundsPad;
			Rectangle b;

			using (Brush bb = new SolidBrush(Color.Blue), rb = new SolidBrush(Color.Red), gb = new SolidBrush(Color.Green))
			using (Pen bp = new Pen(Color.Black), rp = new Pen(Color.Red), gp = new Pen(Color.Green))
			{
				g.FillRectangle(bb, e.ClipRectangle);

				b = NextBoundsPaint(g, "DGrabHandle");
				ControlPaint.DrawGrabHandle(g, new Rectangle(b.X, b.Y, 20, 20),true, true);
				b = NextBoundsPaint(g, "- !primary !enabled");
				ControlPaint.DrawGrabHandle(g, new Rectangle(b.X, b.Y, 20, 20), false, false);

				b = NextBoundsPaint(g, "DGrid - (1,1)");
				ControlPaint.DrawGrid(g, b, new Size(2, 2), Color.Green);
				b = NextBoundsPaint(g, "- (10,10)");
				ControlPaint.DrawGrid(g, b, new Size(10, 10), Color.Green);

				b = NextBoundsPaint(g, "DImageDisabled");
				ControlPaint.DrawImageDisabled(g, new Bitmap(this.GetType(),"test.bmp"), b.X, b.Y, Color.Blue);

				b = NextBoundsPaint(g, "DLockedFrame");
				ControlPaint.DrawLockedFrame(g, b, true);
				b = NextBoundsPaint(g, "- !primary");
				ControlPaint.DrawLockedFrame(g, b, false);

				b = NextBoundsPaint(g, "DMenuGlyph - Arrow");
				ControlPaint.DrawMenuGlyph(g, b, MenuGlyph.Arrow);
				b = NextBoundsPaint(g, "- Bullet");
				ControlPaint.DrawMenuGlyph(g, b, MenuGlyph.Bullet);
				b = NextBoundsPaint(g, "- Checkmark");
				ControlPaint.DrawMenuGlyph(g, b, MenuGlyph.Checkmark);
				b = NextBoundsPaint(g, "- Max");
				ControlPaint.DrawMenuGlyph(g, b, MenuGlyph.Max);
				b = NextBoundsPaint(g, "- Min");
				ControlPaint.DrawMenuGlyph(g, b, MenuGlyph.Min);

				b = NextBoundsPaint(g, "DMixedCheckBox");
				ControlPaint.DrawMixedCheckBox(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Normal);
				b = NextBoundsPaint(g, "- All");
				ControlPaint.DrawMixedCheckBox(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.All);
				b = NextBoundsPaint(g, "- Checked");
				ControlPaint.DrawMixedCheckBox(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Checked);
				b = NextBoundsPaint(g, "- Flat");
				ControlPaint.DrawMixedCheckBox(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Flat);
				b = NextBoundsPaint(g, "- Inactive");
				ControlPaint.DrawMixedCheckBox(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Inactive);
				b = NextBoundsPaint(g, "- Pushed");
				ControlPaint.DrawMixedCheckBox(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Pushed);

				b = NextBoundsPaint(g, "DRadioButton");
				ControlPaint.DrawRadioButton(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Normal);
				b = NextBoundsPaint(g, "- Pushed");
				ControlPaint.DrawRadioButton(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Pushed);
				b = NextBoundsPaint(g, "- Flat");
				ControlPaint.DrawRadioButton(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Flat);
				b = NextBoundsPaint(g, "- Inactive");
				ControlPaint.DrawRadioButton(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Inactive);
				b = NextBoundsPaint(g, "- Checked");
				ControlPaint.DrawRadioButton(g, new Rectangle(b.X, b.Y, 20, 20), ButtonState.Checked);

				b = NextBoundsPaint(g, "DRevFrame - Dashed");
				Point pt = PointToScreen(new Point(b.Left + 5, b.Top + 5));
				ControlPaint.DrawReversibleFrame(new Rectangle(pt,b.Size), Color.Gray, FrameStyle.Dashed);
				b = NextBoundsPaint(g, "- Thick");
				pt = PointToScreen(new Point(b.Left + 5, b.Top + 5));
				ControlPaint.DrawReversibleFrame(new Rectangle(pt,b.Size), Color.Gray, FrameStyle.Thick);

				b = NextBoundsPaint(g, "DReversibleLine");
				pt = PointToScreen(new Point(b.Left + 5, b.Top + 5));
				ControlPaint.DrawReversibleLine(pt, new Point(pt.X + b.Width, pt.Y + b.Height),Color.Gray);

				b = NextBoundsPaint(g, "DScrollButton - Down");
				ControlPaint.DrawScrollButton(g, b, ScrollButton.Down, ButtonState.Normal);
				b = NextBoundsPaint(g, "- Left Pushed");
				ControlPaint.DrawScrollButton(g, b, ScrollButton.Left, ButtonState.Pushed);
				b = NextBoundsPaint(g, "- Max Inactive");
				ControlPaint.DrawScrollButton(g, b, ScrollButton.Max, ButtonState.Inactive);
				b = NextBoundsPaint(g, "- Min");
				ControlPaint.DrawScrollButton(g, b, ScrollButton.Min, ButtonState.Normal);
				b = NextBoundsPaint(g, "- Right");
				ControlPaint.DrawScrollButton(g, b, ScrollButton.Right, ButtonState.Normal);
				b = NextBoundsPaint(g, "- Up");
				ControlPaint.DrawScrollButton(g, b, ScrollButton.Up, ButtonState.Normal);

				b = NextBoundsPaint(g, "DSelectionFrame");
				ControlPaint.DrawSelectionFrame(g, true, b, new Rectangle(b.Left + 5, b.Top + 5, b.Width - 10, b.Height - 10), Color.Blue);

				b = NextBoundsPaint(g, "DSizeGrip");
Console.WriteLine("DSizeGrip bounds: " + b.ToString());
				ControlPaint.DrawSizeGrip(g, Color.Blue, b);

				b = NextBoundsPaint(g, "DStringDisabled");
				ControlPaint.DrawStringDisabled(g, "disabled", Font, Color.Blue, b, StringFormat.GenericDefault);

				b = NextBoundsPaint(g, "FillReverRect");
				g.DrawString("Hello", Font, bb, b.X + 5, b.Y + 5);
				pt = PointToScreen(b.Location);
				ControlPaint.FillReversibleRectangle(new Rectangle(pt.X, pt.Y, b.Width, b.Height), Color.Blue);

				b = NextBoundsPaint(g, "Light - 10%");
				using (Brush bc= new SolidBrush(ControlPaint.Light(Color.Blue, .1f)))
					g.FillRectangle(bc, b);
				b = NextBoundsPaint(g, "Light 90%");
				using (Brush bc= new SolidBrush(ControlPaint.Light(Color.Blue, .9f)))
					g.FillRectangle(bc, b);

				b = NextBoundsPaint(g, "LightLight");
				using (Brush bc= new SolidBrush(ControlPaint.LightLight(Color.Blue)))
					g.FillRectangle(bc, b);
			}

		}
		private Rectangle NextBoundsPaint(Graphics g, string text)
		{
			int boundSizePaint = 60;
			Rectangle r = new Rectangle(boundsX, boundsY, boundSizePaint, boundSizePaint);
			using (Brush b = new SolidBrush(Color.LightGray))
				g.FillRectangle(b, r);
			boundsX += boundSizePaint + boundsPad;
			if (boundsX + boundsSize > Width - 10)
			{
				boundsX = boundsPad;
				boundsY += boundSizePaint + boundsPad;
			}
			g.DrawRectangle(SystemPens.ControlLightLight, r);
			g.DrawString(text, new Font("Arial", 7),SystemBrushes.ControlLightLight, r.X, r.Bottom);
			return r;
		}

		private void AddMessageBoxTest(Control c)
		{
			messageBox1 = new Button();
			messageBox2 = new Button();
			messageBox3 = new Button();
			messageBox4 = new Button();
			messageBox1.SetBounds(10, 10, 120, 20);
			messageBox1.Text = "Hello";
			messageBox1.Click+=new EventHandler(messageBox1_Click);
			messageBox2.SetBounds(10, 50, 120, 20);
			messageBox2.Text = "OKCancelQuestion";
			messageBox2.Click+=new EventHandler(messageBox2_Click);
			messageBox3.SetBounds(10, 90, 120, 20);
			messageBox3.Text = "YesNoCancel";
			messageBox3.Click+=new EventHandler(messageBox3_Click);
			messageBox4.SetBounds(10, 130, 120, 20);
			messageBox4.Text = "OKInfo";
			messageBox4.Click+=new EventHandler(messageBox4_Click);

			dialog1Button = new Button();
			dialog1Button.SetBounds(10, 180, 120, 20);
			dialog1Button.Text = "Open File";
			dialog1Button.Click+=new EventHandler(dialog1Button_Click);

			c.Controls.AddRange(new Control[] {messageBox1, messageBox2, messageBox3, messageBox4, dialog1Button});
		}

		private void messageBox1_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "Hello");
		}

		private void messageBox2_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "Hello","Caption", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);

		}

		private void messageBox3_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "Hello loooooooooooooooooooooooooooooooooooooooooong","Caption", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
		}

		private void messageBox4_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "Hello","Caption looooooooooooooooooooooooooooooooooooong", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void dialog1Button_Click(object sender, EventArgs e)
		{
			if ( openFileDialog1 == null)
			{
				openFileDialog1 = new OpenFileDialog();
				String filter = "Text Files|*.txt|";
				filter += "All Files|*.*";
				openFileDialog1.Filter = filter;
			}
			openFileDialog1.ShowDialog();
		}

		private void AddResXTest(Control c)
		{
			int height = 400;
			int width = 300;
			buttonResXWrite = new Button();
			buttonResXWrite.Location = new Point(20, height - 40);
			buttonResXWrite.Text = "Write ResX";
			buttonResXWrite.Click+=new EventHandler(buttonResXWrite_Click);
			buttonResXRead = new Button();
			buttonResXRead.Location = new Point(buttonResXRead.Right + 20, height - 40);
			buttonResXRead.Text = "Read ResX";
			buttonResXRead.Click+=new EventHandler(buttonResXRead_Click);
			textBoxResXData = new TextBox();
			textBoxResXData.Multiline = true;
			textBoxResXData.Bounds = new Rectangle(10, 10, width - 10, height - 50);
			textBoxResXData.ReadOnly = true;
			c.Controls.AddRange(new Control[3] {textBoxResXData, buttonResXWrite, buttonResXRead});
		}

		private void buttonResXWrite_Click(object sender, EventArgs e)
		{
		#if !ECMA_COMPAT && CONFIG_SERIALIZATION
			ResXResourceWriter w = new ResXResourceWriter("test.resx");
			w.AddResource("my string", "Hello");
			w.AddResource("my color", Color.Red);
			w.AddResource("my byte array", new Byte[3] { 255, 254, 253 });
			w.Generate();
			w.Close();
		#endif
		}

		private void buttonResXRead_Click(object sender, EventArgs e)
		{
		#if !ECMA_COMPAT && CONFIG_SERIALIZATION
			ResXResourceSet rs = new ResXResourceSet("test.resx");
			textBoxResXData.AppendText("my string:" + rs.GetObject("my string") + "\r\n");
			textBoxResXData.AppendText("my color:" + rs.GetObject("my color") + "\r\n");
			byte[] b = (byte[])rs.GetObject("my byte array") ;
			textBoxResXData.AppendText("my byte array:" + b.ToString()+ "\r\n");
			textBoxResXData.AppendText("{" + b[0] +"," + b[1]+"," + b[2]+"}");
		#endif
		}

		private void AddImageListTest(Control c)
		{
			imageList1 = new ImageList();

			int height = 400;
			buttonImageListWrite = new Button();
			buttonImageListWrite.Location = new Point(20, height - 40);
			buttonImageListWrite.Text = "Add Image";
			buttonImageListWrite.Click+=new EventHandler(buttonImageListWrite_Click);
			c.Controls.Add(buttonImageListWrite);
			buttonImageListRead = new Button();
			buttonImageListRead.Location = new Point(buttonImageListWrite.Right + 20, height - 40);
			buttonImageListRead.Text = "Read Image";
			buttonImageListRead.Click+=new EventHandler(buttonImageListRead_Click);
			c.Controls.Add(buttonImageListRead);
			labelImageListSize = new Label();
			labelImageListSize.Bounds = new Rectangle(buttonImageListRead.Right + 20, height - 40, 28, 20);
			labelImageListSize.Text = "Size";
			c.Controls.Add(labelImageListSize);
			textBoxImageListSize = new TextBox();
			textBoxImageListSize.Bounds = new Rectangle(labelImageListSize.Right + 20, height - 40, 30, 20);
			textBoxImageListSize.Text = imageList1.ImageSize.Width.ToString();
			c.Controls.Add(textBoxImageListSize);
			labelImageListColorDepth = new Label();
			labelImageListColorDepth.Bounds = new Rectangle(textBoxImageListSize.Right + 20, height - 40, 40, 20);
			labelImageListColorDepth.Text = "Depth";
			c.Controls.Add(labelImageListColorDepth);
			textBoxImageListColorDepth = new TextBox();
			textBoxImageListColorDepth.Bounds = new Rectangle(labelImageListColorDepth.Right + 20, height - 40, 30, 20);
			textBoxImageListColorDepth.Text = ((int)imageList1.ColorDepth).ToString();
			c.Controls.Add(textBoxImageListColorDepth);
			buttonImageListSet = new Button();
			buttonImageListSet.Bounds = new Rectangle(textBoxImageListColorDepth.Right + 20, height - 40, 50, 22);
			buttonImageListSet.Text = "Set";
			buttonImageListSet.Click+=new EventHandler(buttonImageListSet_Click);
			c.Controls.Add(buttonImageListSet);

			c.Paint+=new PaintEventHandler(c_Paint);

			//ResourceManager resources = new ResourceManager(typeof(FormsTest));

			//object o = resources.GetObject("hearts.ImageStream");
			//imageList1.ImageStream = (System.Windows.Forms.ImageListStreamer)o;
		}

		private void buttonImageListWrite_Click(object sender, EventArgs e)
		{
			imageList1.Images.Add(new Bitmap(this.GetType(),"test.bmp"));
			Invalidate(true);
		}
		private void buttonImageListRead_Click(object sender, EventArgs e)
		{
			Console.WriteLine(imageList1.Images.Count);
		}

		private void c_Paint(object sender, PaintEventArgs e)
		{
			int x = 10;
			for (int i = 0; i < imageList1.Images.Count; i++)
			{
				imageList1.Draw(e.Graphics,x,10, i);
				x += imageList1.Images[i].Width + 10;
			}
		}

		private void buttonImageListSet_Click(object sender, EventArgs e)
		{
			int s = int.Parse(textBoxImageListSize.Text);
			imageList1.ImageSize = new Size(s,s );
			imageList1.ColorDepth = (ColorDepth)int.Parse(textBoxImageListColorDepth.Text);
			Invalidate(true);
		}

		private void ShowHelp(object sender, HelpEventArgs e)
		{
			MessageBox.Show(this, "This is a help message.", "Help");
		}

		private void AddUpDownTest(Control c)
		{
			upDown1 = new DomainUpDown();
			upDown1.Location = new Point(10, 10);
			// upDown1.Size = new Size(80, 30);
			upDown2 = new DomainUpDown();
			upDown2.Location = new Point(10, 50);
			// upDown2.Size = new Size(80, 30);
			upDown2.UpDownAlign = LeftRightAlignment.Left;
			upDown3 = new NumericUpDown();
			upDown3.Location = new Point(10, 90);
			upDown4 = new NumericUpDown();
			upDown4.Location = new Point(10, 130);
			upDown4.UpDownAlign = LeftRightAlignment.Left;
			upDown1ro = new DomainUpDown();
			upDown1ro.Location = new Point(150, 10);
			upDown1ro.ReadOnly = true;
			upDown2ro = new DomainUpDown();
			upDown2ro.Location = new Point(150, 50);
			upDown2ro.UpDownAlign = LeftRightAlignment.Left;
			upDown2ro.ReadOnly = true;
			upDown3ro = new NumericUpDown();
			upDown3ro.Location = new Point(150, 90);
			upDown3ro.ReadOnly = true;
			upDown4ro = new NumericUpDown();
			upDown4ro.Location = new Point(150, 130);
			upDown4ro.UpDownAlign = LeftRightAlignment.Left;
			upDown4ro.ReadOnly = true;
			c.Controls.AddRange(new Control[] {upDown1, upDown1ro, upDown2, upDown2ro
						, upDown3, upDown3ro, upDown4, upDown4ro});
			upDown1.Items.Add(upDownString1);
			upDown1.Items.Add(upDownString2);
			upDown1.Items.Add(upDownString3);
			upDown1.Items.Add(upDownString4);
			upDown2.Items.Add(upDownString1);
			upDown2.Items.Add(upDownString2);
			upDown2.Items.Add(upDownString3);
			upDown2.Items.Add(upDownString4);
			upDown1ro.Items.Add(upDownString1);
			upDown1ro.Items.Add(upDownString2);
			upDown1ro.Items.Add(upDownString3);
			upDown1ro.Items.Add(upDownString4);
			upDown2ro.Items.Add(upDownString1);
			upDown2ro.Items.Add(upDownString2);
			upDown2ro.Items.Add(upDownString3);
			upDown2ro.Items.Add(upDownString4);

		}

		private void AddTrackbarTest(Control c)
		{
			vTrackBar = new TrackBar();
			vTrackBar.Bounds = new Rectangle(10, 10, 20, 160);
			vTrackBar.Orientation = Orientation.Vertical;
			vTrackBar.ValueChanged+=new EventHandler(vTrackBar_ValueChanged);
			vTrackBar.Maximum = 100;
			vTrackBar.TickFrequency = 5;
			vTrackBar.TickStyle = TickStyle.Both;
			c.Controls.Add(vTrackBar);

			hTrackBar = new TrackBar();
			hTrackBar.Bounds = new Rectangle(200, 10, 250, 20);
			hTrackBar.Orientation = Orientation.Horizontal;
			hTrackBar.ValueChanged+=new EventHandler(hTrackBar_ValueChanged);
			hTrackBar.Maximum = 100;
			hTrackBar.TickFrequency = 5;
			hTrackBar.TickStyle = TickStyle.Both;
			c.Controls.Add(hTrackBar);

			track1LabelMin = new Label();
			track1LabelMin.Text = "min";
			track1LabelMin.Bounds = new Rectangle(vTrackBar.Right + 10, vTrackBar.Top + 100, 35, 20);
			c.Controls.Add(track1LabelMin);
			track1TextBoxMin = new TextBox();
			track1TextBoxMin.Text = vTrackBar.Minimum.ToString();
			track1TextBoxMin.Bounds = new Rectangle(track1LabelMin.Right + 5, track1LabelMin.Top, 30, 20);
			track1TextBoxMin.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track1TextBoxMin);

			track1LabelMax = new Label();
			track1LabelMax.Text = "max";
			track1LabelMax.Bounds = new Rectangle(track1LabelMin.Left, track1LabelMin.Bottom + 10, 35, 20);
			c.Controls.Add(track1LabelMax);
			track1TextBoxMax = new TextBox();
			track1TextBoxMax.Text = vTrackBar.Maximum.ToString();
			track1TextBoxMax.Bounds = new Rectangle(track1LabelMax.Right + 5, track1LabelMax.Top, 30, 20);
			track1TextBoxMax.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track1TextBoxMax);

			track1LabelValue = new Label();
			track1LabelValue.Text = "value";
			track1LabelValue.Bounds = new Rectangle(track1LabelMax.Left, track1LabelMax.Bottom + 10, 35, 20);
			c.Controls.Add(track1LabelValue);
			track1TextBoxValue = new TextBox();
			track1TextBoxValue.Text = vTrackBar.Value.ToString();
			track1TextBoxValue.Bounds = new Rectangle(track1LabelValue.Right + 5, track1LabelValue.Top, 30, 20);
			track1TextBoxValue.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track1TextBoxValue);

			track1LabelLarge = new Label();
			track1LabelLarge.Text = "large";
			track1LabelLarge.Bounds = new Rectangle(track1LabelValue.Left, track1LabelValue.Bottom + 10, 35, 20);
			c.Controls.Add(track1LabelLarge);
			track1TextBoxLarge = new TextBox();
			track1TextBoxLarge.Text = vTrackBar.LargeChange.ToString();
			track1TextBoxLarge.Bounds = new Rectangle(track1LabelLarge.Right + 5, track1LabelLarge.Top, 30, 20);
			track1TextBoxLarge.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track1TextBoxLarge);

			track1LabelSmall = new Label();
			track1LabelSmall.Text = "small";
			track1LabelSmall.Bounds = new Rectangle(track1LabelLarge.Left, track1LabelLarge.Bottom + 10, 35, 20);
			c.Controls.Add(track1LabelSmall);
			track1TextBoxSmall = new TextBox();
			track1TextBoxSmall.Text = vTrackBar.SmallChange.ToString();
			track1TextBoxSmall.Bounds = new Rectangle(track1LabelSmall.Right + 5, track1LabelSmall.Top, 30, 20);
			track1TextBoxSmall.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track1TextBoxSmall);

			track2LabelMin = new Label();
			track2LabelMin.Text = "min";
			track2LabelMin.Bounds = new Rectangle(hTrackBar.Left + 90, hTrackBar.Bottom + 10, 35, 20);
			c.Controls.Add(track2LabelMin);
			track2TextBoxMin = new TextBox();
			track2TextBoxMin.Text = hTrackBar.Minimum.ToString();
			track2TextBoxMin.Bounds = new Rectangle(track2LabelMin.Right + 5, track2LabelMin.Top, 30, 20);
			track2TextBoxMin.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track2TextBoxMin);

			track2LabelMax = new Label();
			track2LabelMax.Text = "max";
			track2LabelMax.Bounds = new Rectangle(track2LabelMin.Left, track2LabelMin.Bottom + 10, 35, 20);
			c.Controls.Add(track2LabelMax);
			track2TextBoxMax = new TextBox();
			track2TextBoxMax.Text = hTrackBar.Maximum.ToString();
			track2TextBoxMax.Bounds = new Rectangle(track2LabelMax.Right + 5, track2LabelMax.Top, 30, 20);
			track2TextBoxMax.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track2TextBoxMax);

			track2LabelValue = new Label();
			track2LabelValue.Text = "value";
			track2LabelValue.Bounds = new Rectangle(track2LabelMax.Left, track2LabelMax.Bottom + 10, 35, 20);
			c.Controls.Add(track2LabelValue);
			track2TextBoxValue = new TextBox();
			track2TextBoxValue.Text = hTrackBar.Value.ToString();
			track2TextBoxValue.Bounds = new Rectangle(track2LabelValue.Right + 5, track2LabelValue.Top, 30, 20);
			track2TextBoxValue.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track2TextBoxValue);

			track2LabelLarge = new Label();
			track2LabelLarge.Text = "large";
			track2LabelLarge.Bounds = new Rectangle(track2LabelValue.Left, track2LabelValue.Bottom + 10, 35, 20);
			c.Controls.Add(track2LabelLarge);
			track2TextBoxLarge = new TextBox();
			track2TextBoxLarge.Text = vTrackBar.LargeChange.ToString();
			track2TextBoxLarge.Bounds = new Rectangle(track2LabelLarge.Right + 5, track2LabelLarge.Top, 30, 20);
			track2TextBoxLarge.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track2TextBoxLarge);

			track2LabelSmall = new Label();
			track2LabelSmall.Text = "small";
			track2LabelSmall.Bounds = new Rectangle(track2LabelLarge.Left, track2LabelLarge.Bottom + 10, 35, 20);
			c.Controls.Add(track2LabelSmall);
			track2TextBoxSmall = new TextBox();
			track2TextBoxSmall.Text = vTrackBar.SmallChange.ToString();
			track2TextBoxSmall.Bounds = new Rectangle(track2LabelSmall.Right + 5, track2LabelSmall.Top, 30, 20);
			track2TextBoxSmall.TextChanged+=new EventHandler(trackTextBox_TextChanged);
			c.Controls.Add(track2TextBoxSmall);

		}

		private void vTrackBar_ValueChanged(object sender, EventArgs e)
		{
			track1TextBoxValue.Text = vTrackBar.Value.ToString();
		}

		private void hTrackBar_ValueChanged(object sender, EventArgs e)
		{
			track2TextBoxValue.Text = hTrackBar.Value.ToString();
		}

		private void trackTextBox_TextChanged(object sender, EventArgs e)
		{
			try //Lazy!!
			{
				vTrackBar.Minimum = int.Parse(track1TextBoxMin.Text);
				vTrackBar.Maximum = int.Parse(track1TextBoxMax.Text);
				vTrackBar.Value = int.Parse(track1TextBoxValue.Text);
				vTrackBar.LargeChange = int.Parse(track1TextBoxLarge.Text);
				vTrackBar.SmallChange = int.Parse(track1TextBoxSmall.Text);
				hTrackBar.Minimum = int.Parse(track2TextBoxMin.Text);
				hTrackBar.Maximum = int.Parse(track2TextBoxMax.Text);
				hTrackBar.Value = int.Parse(track2TextBoxValue.Text);
				hTrackBar.LargeChange = int.Parse(track2TextBoxLarge.Text);
				hTrackBar.SmallChange = int.Parse(track2TextBoxSmall.Text);
			}
			catch
			{}

		}

		private void UpdateStatusBar(object sender, System.EventArgs e)
		{
			TabPage tabPage = (sender as TabControl).SelectedTab;
			statusBar.Text = tabPage.Text;
		}

		private void AddDataGrid(Control c)
		{
			dataGrid = new DataGrid();
			dataGrid.Location = new Point(50, 20);
			dataGrid.Size = new System.Drawing.Size(400, 500);
			dataGrid.Text = "Data Grid";
			c.Controls.Add(dataGrid);
		}

		private void AddTiger( Control c ) {
			c.Paint += new PaintEventHandler( this.PaintTiger );
		}
		void PaintTiger( object sender, PaintEventArgs e ) {
			Tiger(e.Graphics);
		}

		public class TigerPath {
			GraphicsPath path;
			Pen			 pen;
			Brush		    brush;

			public TigerPath( Pen p, Brush b, string data ) {
				pen = p;
				brush = b;
				path = Interpret( data );
			}

			float FloatParse( string s ) {
				return float.Parse( s, System.Globalization.NumberFormatInfo.InvariantInfo );
			}

			GraphicsPath Interpret( string s ) {
				GraphicsPath p = new GraphicsPath();
				s = s.Replace( "z", "" );
				s = s.Replace( "Z", "" );
				s = s.Replace( "M", " M " );
				s = s.Replace( "C", " C " );
				s = s.Replace( "L", " L " );
				s = s.Replace( "  ", " " );
				char [] cplit = new char[] { ' ' };
				string [] split = s.Split( cplit );

				int iCount = split.Length;
				int iPos = 0;
				int i;
				float x = 0, y = 0;
				float [] f = new float[6];;

				while( iPos < iCount ) {
					switch( split[iPos++] ) {
						case "": break;
						case "M" :
							x = FloatParse( split[iPos++] );
							y = FloatParse( split[iPos++] );
							break;
						case "C" :
							for( i = 0; i< 6; i++ ) {
								f[i] = FloatParse( split[iPos++] );
							}
							p.AddBezier( x,y, f[0], f[1], f[2], f[3], f[4], f[5]  );
							x = f[4];
							y = f[5];
							break;
						case "L" :
							for( i = 0; i< 2; i++ ) {
								f[i] = FloatParse( split[iPos++] );
							}
							p.AddLine( x,y, f[0], f[1] );
							x = f[0];
							y = f[1];
							break;
					}
				}
				return p;
			}

			public void Paint( Graphics g ) {
				if( null != brush ) g.FillPath( brush, path );
				if( null != pen   ) {
#if WINDOWS_ONLY
				if( pen.Width < 1.0F ) {
					int iVal = pen.Color.ToArgb();

					float fVal = pen.Width;
					if( fVal > 1.0F ) fVal = 1.0F;
					if( fVal < 0.0F ) fVal = 0.0F;
					float RealVal = 255 * fVal;
					int iPenWidth2 = Convert.ToInt16( RealVal );

					iVal = (iVal & 0xFFFFFF) + ( iPenWidth2 << 24 );
					Pen pen2 = new Pen( Color.FromArgb(iVal), pen.Width );
					g.DrawPath( pen2, path );
				}
				else {
#endif
					g.DrawPath( pen, path );
#if WINDOWS_ONLY
				}
#endif
			}

			}
		}

		ArrayList GetTiger() {

			DateTime dt = DateTime.Now;

			ArrayList tigerData = new ArrayList();

Pen   p = new Pen(Color.FromArgb(0,0,0), 0.172f );
Brush b = new SolidBrush( Color.FromArgb(0xFF,0xFF,0xFF) );

tigerData.Add( new TigerPath( p, b, @"M-122.304 84.285C-122.304 84.285 -122.203 86.179 -123.027 86.16C-123.851 86.141 -140.305 38.066 -160.833 40.309C-160.833 40.309 -143.05 32.956 -122.304 84.285z" ) );
tigerData.Add( new TigerPath( p, b, @"M-118.774 81.262C-118.774 81.262 -119.323 83.078 -120.092 82.779C-120.86 82.481 -119.977 31.675 -140.043 26.801C-140.043 26.801 -120.82 25.937 -118.774 81.262z" ) );
tigerData.Add( new TigerPath( p, b, @"M-91.284 123.59C-91.284 123.59 -89.648 124.55 -90.118 125.227C-90.589 125.904 -139.763 113.102 -149.218 131.459C-149.218 131.459 -145.539 112.572 -91.284 123.59z" ) );
tigerData.Add( new TigerPath( p, b, @"M-94.093 133.801C-94.093 133.801 -92.237 134.197 -92.471 134.988C-92.704 135.779 -143.407 139.121 -146.597 159.522C-146.597 159.522 -149.055 140.437 -94.093 133.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M-98.304 128.276C-98.304 128.276 -96.526 128.939 -96.872 129.687C-97.218 130.435 -147.866 126.346 -153.998 146.064C-153.998 146.064 -153.646 126.825 -98.304 128.276z" ) );
tigerData.Add( new TigerPath( p, b, @"M-109.009 110.072C-109.009 110.072 -107.701 111.446 -108.34 111.967C-108.979 112.488 -152.722 86.634 -166.869 101.676C-166.869 101.676 -158.128 84.533 -109.009 110.072z" ) );
tigerData.Add( new TigerPath( p, b, @"M-116.554 114.263C-116.554 114.263 -115.098 115.48 -115.674 116.071C-116.25 116.661 -162.638 95.922 -174.992 112.469C-174.992 112.469 -168.247 94.447 -116.554 114.263z" ) );
tigerData.Add( new TigerPath( p, b, @"M-119.154 118.335C-119.154 118.335 -117.546 119.343 -118.036 120.006C-118.526 120.669 -167.308 106.446 -177.291 124.522C-177.291 124.522 -173.066 105.749 -119.154 118.335z" ) );
tigerData.Add( new TigerPath( p, b, @"M-108.42 118.949C-108.42 118.949 -107.298 120.48 -107.999 120.915C-108.7 121.35 -148.769 90.102 -164.727 103.207C-164.727 103.207 -153.862 87.326 -108.42 118.949z" ) );
tigerData.Add( new TigerPath( p, b, @"M-128.2 90C-128.2 90 -127.6 91.8 -128.4 92C-129.2 92.2 -157.8 50.2 -177.001 57.8C-177.001 57.8 -161.8 46 -128.2 90z" ) );
tigerData.Add( new TigerPath( p, b, @"M-127.505 96.979C-127.505 96.979 -126.53 98.608 -127.269 98.975C-128.007 99.343 -164.992 64.499 -182.101 76.061C-182.101 76.061 -169.804 61.261 -127.505 96.979z" ) );
tigerData.Add( new TigerPath( p, b, @"M-127.62 101.349C-127.62 101.349 -126.498 102.88 -127.199 103.315C-127.9 103.749 -167.969 72.502 -183.927 85.607C-183.927 85.607 -173.062 69.726 -127.62 101.349z" ) );

p = new Pen(Color.FromArgb(0,0,0) );
tigerData.Add( new TigerPath( p, b, @"M-129.83 103.065C-129.327 109.113 -128.339 115.682 -126.6 118.801C-126.6 118.801 -130.2 131.201 -121.4 144.401C-121.4 144.401 -121.8 151.601 -120.2 154.801C-120.2 154.801 -116.2 163.201 -111.4 164.001C-107.516 164.648 -98.793 167.717 -88.932 169.121C-88.932 169.121 -71.8 183.201 -75 196.001C-75 196.001 -75.4 212.401 -79 214.001C-79 214.001 -67.4 202.801 -77 219.601L-81.4 238.401C-81.4 238.401 -55.8 216.801 -71.4 235.201L-81.4 261.201C-81.4 261.201 -61.8 242.801 -69 251.201L-72.2 260.001C-72.2 260.001 -29 232.801 -59.8 262.401C-59.8 262.401 -51.8 258.801 -47.4 261.601C-47.4 261.601 -40.6 260.401 -41.4 262.001C-41.4 262.001 -62.2 272.401 -65.8 290.801C-65.8 290.801 -57.4 280.801 -60.6 291.601L-60.2 303.201C-60.2 303.201 -56.2 281.601 -56.6 319.201C-56.6 319.201 -37.4 301.201 -49 322.001L-49 338.801C-49 338.801 -33.8 322.401 -40.2 335.201C-40.2 335.201 -30.2 326.401 -34.2 341.601C-34.2 341.601 -35 352.001 -30.6 340.801C-30.6 340.801 -14.6 310.201 -20.6 336.401C-20.6 336.401 -21.4 355.601 -16.6 340.801C-16.6 340.801 -16.2 351.201 -7 358.401C-7 358.401 -8.2 307.601 4.6 343.601L8.6 360.001C8.6 360.001 11.4 350.801 11 345.601C11 345.601 25.8 329.201 19 353.601C19 353.601 34.2 330.801 31 344.001C31 344.001 23.4 360.001 25 364.801C25 364.801 41.8 330.001 43 328.401C43 328.401 41 370.802 51.8 334.801C51.8 334.801 57.4 346.801 54.6 351.201C54.6 351.201 62.6 343.201 61.8 340.001C61.8 340.001 66.4 331.801 69.2 345.401C69.2 345.401 71 354.801 72.6 351.601C72.6 351.601 76.6 375.602 77.8 352.801C77.8 352.801 79.4 339.201 72.2 327.601C72.2 327.601 73 324.401 70.2 320.401C70.2 320.401 83.8 342.001 76.6 313.201C76.6 313.201 87.801 321.201 89.001 321.201C89.001 321.201 75.4 298.001 84.2 302.801C84.2 302.801 79 292.401 97.001 304.401C97.001 304.401 81 288.401 98.601 298.001C98.601 298.001 106.601 304.401 99.001 294.401C99.001 294.401 84.6 278.401 106.601 296.401C106.601 296.401 118.201 312.801 119.001 315.601C119.001 315.601 109.001 286.401 104.601 283.601
					C104.601 283.601 113.001 247.201 154.201 262.801C154.201 262.801 161.001 280.001 165.401 261.601C165.401 261.601 178.201 255.201 189.401 282.801C189.401 282.801 193.401 269.201 192.601 266.401C192.601 266.401 199.401 267.601 198.601 266.401C198.601 266.401 211.801 270.801 213.001 270.001C213.001 270.001 219.801 276.801 220.201 273.201C220.201 273.201 229.401 276.001 227.401 272.401C227.401 272.401 236.201 288.001 236.601 291.601L239.001 277.601L241.001 280.401C241.001 280.401 242.601 272.801 241.801 271.601C241.001 270.401 261.801 278.401 266.601 299.201L268.601 307.601C268.601 307.601 274.601 292.801 273.001 288.801C273.001 288.801 278.201 289.601 278.601 294.001C278.601 294.001 282.601 270.801 277.801 264.801C277.801 264.801 282.201 264.001 283.401 267.601L283.401 260.401C283.401 260.401 290.601 261.201 290.601 258.801C290.601 258.801 295.001 254.801 297.001 259.601C297.001 259.601 284.601 224.401 303.001 243.601C303.001 243.601 310.201 254.401 306.601 235.601C303.001 216.801 299.001 215.201 303.801 214.801C303.801 214.801 304.601 211.201 302.601 209.601C300.601 208.001 303.801 209.601 303.801 209.601C303.801 209.601 308.601 213.601 303.401 191.601C303.401 191.601 309.801 193.201 297.801 164.001C297.801 164.001 300.601 161.601 296.601 153.201C296.601 153.201 304.601 157.601 307.401 156.001C307.401 156.001 307.001 154.401 303.801 150.401C303.801 150.401 282.201 95.6 302.601 117.601C302.601 117.601 314.451 131.151 308.051 108.351C308.051 108.351 298.94 84.341 299.717 80.045L-129.83 103.065z" ) );

p = null;
b = new SolidBrush( Color.FromArgb(0xCC,0x72,0x26) );
tigerData.Add( new TigerPath( p, b, @"M299.717 80.245C300.345 80.426 302.551 81.55 303.801 83.2C303.801 83.2 310.601 94 305.401 75.6C305.401 75.6 296.201 46.8 305.001 58C305.001 58 311.001 65.2 307.801 51.6C303.936 35.173 301.401 28.8 301.401 28.8C301.401 28.8 313.001 33.6 286.201 -6L295.001 -2.4C295.001 -2.4 275.401 -42 253.801 -47.2L245.801 -53.2C245.801 -53.2 284.201 -91.2 271.401 -128C271.401 -128 264.601 -133.2 255.001 -124C255.001 -124 248.601 -119.2 242.601 -120.8C242.601 -120.8 211.801 -119.6 209.801 -119.6C207.801 -119.6 173.001 -156.8 107.401 -139.2C107.401 -139.2 102.201 -137.2 97.801 -138.4C97.801 -138.4 79.4 -154.4 30.6 -131.6C30.6 -131.6 20.6 -129.6 19 -129.6C17.4 -129.6 14.6 -129.6 6.6 -123.2C-1.4 -116.8 -1.8 -116 -3.8 -114.4C-3.8 -114.4 -20.2 -103.2 -25 -102.4C-25 -102.4 -36.6 -96 -41 -86L-44.6 -84.8C-44.6 -84.8 -46.2 -77.6 -46.6 -76.4C-46.6 -76.4 -51.4 -72.8 -52.2 -67.2C-52.2 -67.2 -61 -61.2 -60.6 -56.8C-60.6 -56.8 -62.2 -51.6 -63 -46.8C-63 -46.8 -70.2 -42 -69.4 -39.2C-69.4 -39.2 -77 -25.2 -75.8 -18.4C-75.8 -18.4 -82.2 -18.8 -85 -16.4C-85 -16.4 -85.8 -11.6 -87.4 -11.2C-87.4 -11.2 -90.2 -10 -87.8 -6C-87.8 -6 -89.4 -3.2 -89.8 -1.6C-89.8 -1.6 -89 1.2 -93.4 6.8C-93.4 6.8 -99.8 25.6 -97.8 30.8C-97.8 30.8 -97.4 35.6 -100.2 37.2C-100.2 37.2 -103.8 36.8 -95.4 48.8C-95.4 48.8 -94.6 50 -97.8 52.4C-97.8 52.4 -115 56 -117.4 72.4C-117.4 72.4 -131 87.2 -131 92.4C-131 94.705 -130.729 97.852 -130.03 102.465C-130.03 102.465 -130.6 110.801 -103 111.601C-75.4 112.401 299.717 80.245 299.717 80.245z" ) );
tigerData.Add( new TigerPath( p, b, @"M-115.6 102.6C-140.6 63.2 -126.2 119.601 -126.2 119.601C-117.4 154.001 12.2 116.401 12.2 116.401C12.2 116.401 181.001 86 192.201 82C203.401 78 298.601 84.4 298.601 84.4L293.001 67.6C228.201 21.2 209.001 44.4 195.401 40.4C181.801 36.4 184.201 46 181.001 46.8C177.801 47.6 138.601 22.8 132.201 23.6C125.801 24.4 100.459 0.649 115.401 32.4C131.401 66.4 57 71.6 40.2 60.4C23.4 49.2 47.4 78.8 47.4 78.8C65.8 98.8 31.4 82 31.4 82C-3 69.2 -27 94.8 -30.2 95.6C-33.4 96.4 -38.2 99.6 -39 93.2C-39.8 86.8 -47.31 70.099 -79 96.4C-99 113.001 -112.8 91 -112.8 91L-115.6 102.6z" ) );

b = new SolidBrush( Color.FromArgb(0xe8,0x7f,0x3a) );
tigerData.Add( new TigerPath( p, b, @"M133.51 25.346C127.11 26.146 101.743 2.407 116.71 34.146C133.31 69.346 58.31 73.346 41.51 62.146C24.709 50.946 48.71 80.546 48.71 80.546C67.11 100.546 32.709 83.746 32.709 83.746C-1.691 70.946 -25.691 96.546 -28.891 97.346C-32.091 98.146 -36.891 101.346 -37.691 94.946C-38.491 88.546 -45.87 72.012 -77.691 98.146C-98.927 115.492 -112.418 94.037 -112.418 94.037L-115.618 104.146C-140.618 64.346 -125.546 122.655 -125.546 122.655C-116.745 157.056 13.509 118.146 13.509 118.146C13.509 118.146 182.31 87.746 193.51 83.746C204.71 79.746 299.038 86.073 299.038 86.073L293.51 68.764C228.71 22.364 210.31 46.146 196.71 42.146C183.11 38.146 185.51 47.746 182.31 48.546C179.11 49.346 139.91 24.546 133.51 25.346z" ) );

b = new SolidBrush( Color.FromArgb(0xea,0x8C,0x4d) );
tigerData.Add( new TigerPath( p, b, @"M134.819 27.091C128.419 27.891 103.685 3.862 118.019 35.891C134.219 72.092 59.619 75.092 42.819 63.892C26.019 52.692 50.019 82.292 50.019 82.292C68.419 102.292 34.019 85.492 34.019 85.492C-0.381 72.692 -24.382 98.292 -27.582 99.092C-30.782 99.892 -35.582 103.092 -36.382 96.692C-37.182 90.292 -44.43 73.925 -76.382 99.892C-98.855 117.983 -112.036 97.074 -112.036 97.074L-115.636 105.692C-139.436 66.692 -124.891 125.71 -124.891 125.71C-116.091 160.11 14.819 119.892 14.819 119.892C14.819 119.892 183.619 89.492 194.819 85.492C206.019 81.492 299.474 87.746 299.474 87.746L294.02 69.928C229.219 23.528 211.619 47.891 198.019 43.891C184.419 39.891 186.819 49.491 183.619 50.292C180.419 51.092 141.219 26.291 134.819 27.091z" ) );

b = new SolidBrush( Color.FromArgb(0xec,0x99,0x61) );
tigerData.Add( new TigerPath( p, b, @"M136.128 28.837C129.728 29.637 104.999 5.605 119.328 37.637C136.128 75.193 60.394 76.482 44.128 65.637C27.328 54.437 51.328 84.037 51.328 84.037C69.728 104.037 35.328 87.237 35.328 87.237C0.928 74.437 -23.072 100.037 -26.272 100.837C-29.472 101.637 -34.272 104.837 -35.072 98.437C-35.872 92.037 -42.989 75.839 -75.073 101.637C-98.782 120.474 -111.655 100.11 -111.655 100.11L-115.655 107.237C-137.455 70.437 -124.236 128.765 -124.236 128.765C-115.436 163.165 16.128 121.637 16.128 121.637C16.128 121.637 184.928 91.237 196.129 87.237C207.329 83.237 299.911 89.419 299.911 89.419L294.529 71.092C229.729 24.691 212.929 49.637 199.329 45.637C185.728 41.637 188.128 51.237 184.928 52.037C181.728 52.837 142.528 28.037 136.128 28.837z" ) );

b = new SolidBrush( Color.FromArgb(0xee,0xa5,0x75) );
tigerData.Add( new TigerPath( p, b, @"M137.438 30.583C131.037 31.383 106.814 7.129 120.637 39.383C137.438 78.583 62.237 78.583 45.437 67.383C28.637 56.183 52.637 85.783 52.637 85.783C71.037 105.783 36.637 88.983 36.637 88.983C2.237 76.183 -21.763 101.783 -24.963 102.583C-28.163 103.383 -32.963 106.583 -33.763 100.183C-34.563 93.783 -41.548 77.752 -73.763 103.383C-98.709 122.965 -111.273 103.146 -111.273 103.146L-115.673 108.783C-135.473 73.982 -123.582 131.819 -123.582 131.819C-114.782 166.22 17.437 123.383 17.437 123.383C17.437 123.383 186.238 92.983 197.438 88.983C208.638 84.983 300.347 91.092 300.347 91.092L295.038 72.255C230.238 25.855 214.238 51.383 200.638 47.383C187.038 43.383 189.438 52.983 186.238 53.783C183.038 54.583 143.838 29.783 137.438 30.583z" ) );

b = new SolidBrush( Color.FromArgb(0xf1,0xb2,0x88) );
tigerData.Add( new TigerPath( p, b, @"M138.747 32.328C132.347 33.128 106.383 9.677 121.947 41.128C141.147 79.928 63.546 80.328 46.746 69.128C29.946 57.928 53.946 87.528 53.946 87.528C72.346 107.528 37.946 90.728 37.946 90.728C3.546 77.928 -20.454 103.528 -23.654 104.328C-26.854 105.128 -31.654 108.328 -32.454 101.928C-33.254 95.528 -40.108 79.665 -72.454 105.128C-98.636 125.456 -110.891 106.183 -110.891 106.183L-115.691 110.328C-133.691 77.128 -122.927 134.874 -122.927 134.874C-114.127 169.274 18.746 125.128 18.746 125.128C18.746 125.128 187.547 94.728 198.747 90.728C209.947 86.728 300.783 92.764 300.783 92.764L295.547 73.419C230.747 27.019 215.547 53.128 201.947 49.128C188.347 45.128 190.747 54.728 187.547 55.528C184.347 56.328 145.147 31.528 138.747 32.328z" ) );

b = new SolidBrush( Color.FromArgb(0xf3,0xbf,0x9c) );
tigerData.Add( new TigerPath( p, b, @"M140.056 34.073C133.655 34.873 107.313 11.613 123.255 42.873C143.656 82.874 64.855 82.074 48.055 70.874C31.255 59.674 55.255 89.274 55.255 89.274C73.655 109.274 39.255 92.474 39.255 92.474C4.855 79.674 -19.145 105.274 -22.345 106.074C-25.545 106.874 -30.345 110.074 -31.145 103.674C-31.945 97.274 -38.668 81.578 -71.145 106.874C-98.564 127.947 -110.509 109.219 -110.509 109.219L-115.709 111.874C-131.709 81.674 -122.273 137.929 -122.273 137.929C-113.473 172.329 20.055 126.874 20.055 126.874C20.055 126.874 188.856 96.474 200.056 92.474C211.256 88.474 301.22 94.437 301.22 94.437L296.056 74.583C231.256 28.183 216.856 54.874 203.256 50.874C189.656 46.873 192.056 56.474 188.856 57.274C185.656 58.074 146.456 33.273 140.056 34.073z" ) );

b = new SolidBrush( Color.FromArgb(0xf5,0xcc,0xb0) );
tigerData.Add( new TigerPath( p, b, @"M141.365 35.819C134.965 36.619 107.523 13.944 124.565 44.619C146.565 84.219 66.164 83.819 49.364 72.619C32.564 61.419 56.564 91.019 56.564 91.019C74.964 111.019 40.564 94.219 40.564 94.219C6.164 81.419 -17.836 107.019 -21.036 107.819C-24.236 108.619 -29.036 111.819 -29.836 105.419C-30.636 99.019 -37.227 83.492 -69.836 108.619C-98.491 130.438 -110.127 112.256 -110.127 112.256L-115.727 113.419C-130.128 85.019 -121.618 140.983 -121.618 140.983C-112.818 175.384 21.364 128.619 21.364 128.619C21.364 128.619 190.165 98.219 201.365 94.219C212.565 90.219 301.656 96.11 301.656 96.11L296.565 75.746C231.765 29.346 218.165 56.619 204.565 52.619C190.965 48.619 193.365 58.219 190.165 59.019C186.965 59.819 147.765 35.019 141.365 35.819z" ) );

b = new SolidBrush( Color.FromArgb(0xf8,0xd8,0xc4) );
tigerData.Add( new TigerPath( p, b, @"M142.674 37.565C136.274 38.365 108.832 15.689 125.874 46.365C147.874 85.965 67.474 85.565 50.674 74.365C33.874 63.165 57.874 92.765 57.874 92.765C76.274 112.765 41.874 95.965 41.874 95.965C7.473 83.165 -16.527 108.765 -19.727 109.565C-22.927 110.365 -27.727 113.565 -28.527 107.165C-29.327 100.765 -35.786 85.405 -68.527 110.365C-98.418 132.929 -109.745 115.293 -109.745 115.293L-115.745 114.965C-129.346 88.564 -120.963 144.038 -120.963 144.038C-112.163 178.438 22.673 130.365 22.673 130.365C22.673 130.365 191.474 99.965 202.674 95.965C213.874 91.965 302.093 97.783 302.093 97.783L297.075 76.91C232.274 30.51 219.474 58.365 205.874 54.365C192.274 50.365 194.674 59.965 191.474 60.765C188.274 61.565 149.074 36.765 142.674 37.565z" ) );

b = new SolidBrush( Color.FromArgb(0xfa,0xe5,0xd7) );
tigerData.Add( new TigerPath( p, b, @"M143.983 39.31C137.583 40.11 110.529 17.223 127.183 48.11C149.183 88.91 68.783 87.31 51.983 76.11C35.183 64.91 59.183 94.51 59.183 94.51C77.583 114.51 43.183 97.71 43.183 97.71C8.783 84.91 -15.217 110.51 -18.417 111.31C-21.618 112.11 -26.418 115.31 -27.218 108.91C-28.018 102.51 -34.346 87.318 -67.218 112.11C-98.345 135.42 -109.363 118.329 -109.363 118.329L-115.764 116.51C-128.764 92.51 -120.309 147.093 -120.309 147.093C-111.509 181.493 23.983 132.11 23.983 132.11C23.983 132.11 192.783 101.71 203.983 97.71C215.183 93.71 302.529 99.456 302.529 99.456L297.583 78.074C232.783 31.673 220.783 60.11 207.183 56.11C193.583 52.11 195.983 61.71 192.783 62.51C189.583 63.31 150.383 38.51 143.983 39.31z" ) );

b = new SolidBrush( Color.FromArgb(0xfc,0xf2,0xeb) );
tigerData.Add( new TigerPath( p, b, @"M145.292 41.055C138.892 41.855 112.917 18.411 128.492 49.855C149.692 92.656 70.092 89.056 53.292 77.856C36.492 66.656 60.492 96.256 60.492 96.256C78.892 116.256 44.492 99.456 44.492 99.456C10.092 86.656 -13.908 112.256 -17.108 113.056C-20.308 113.856 -25.108 117.056 -25.908 110.656C-26.708 104.256 -32.905 89.232 -65.908 113.856C-98.273 137.911 -108.982 121.365 -108.982 121.365L-115.782 118.056C-128.582 94.856 -119.654 150.147 -119.654 150.147C-110.854 184.547 25.292 133.856 25.292 133.856C25.292 133.856 194.093 103.456 205.293 99.456C216.493 95.456 302.965 101.128 302.965 101.128L298.093 79.237C233.292 32.837 222.093 61.856 208.493 57.856C194.893 53.855 197.293 63.456 194.093 64.256C190.892 65.056 151.692 40.255 145.292 41.055z" ) );

b = new SolidBrush( Color.FromArgb(0xff,0xff,0xff) );
tigerData.Add( new TigerPath( p, b, @"M-115.8 119.601C-128.6 97.6 -119 153.201 -119 153.201C-110.2 187.601 26.6 135.601 26.6 135.601C26.6 135.601 195.401 105.2 206.601 101.2C217.801 97.2 303.401 102.8 303.401 102.8L298.601 80.4C233.801 34 223.401 63.6 209.801 59.6C196.201 55.6 198.601 65.2 195.401 66C192.201 66.8 153.001 42 146.601 42.8C140.201 43.6 114.981 19.793 129.801 51.6C152.028 99.307 69.041 89.227 54.6 79.6C37.8 68.4 61.8 98 61.8 98C80.2 118.001 45.8 101.2 45.8 101.2C11.4 88.4 -12.6 114.001 -15.8 114.801C-19 115.601 -23.8 118.801 -24.6 112.401C-25.4 106 -31.465 91.144 -64.6 115.601C-98.2 140.401 -108.6 124.401 -108.6 124.401L-115.8 119.601z" ) );

b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-74.2 149.601C-74.2 149.601 -81.4 161.201 -60.6 174.401C-60.6 174.401 -59.2 175.801 -77.2 171.601C-77.2 171.601 -83.4 169.601 -85 159.201C-85 159.201 -89.8 154.801 -94.6 149.201C-99.4 143.601 -74.2 149.601 -74.2 149.601z" ) );

b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M65.8 102C65.8 102 83.498 128.821 82.9 133.601C81.6 144.001 81.4 153.601 84.6 157.601C87.801 161.601 96.601 194.801 96.601 194.801C96.601 194.801 96.201 196.001 108.601 158.001C108.601 158.001 120.201 142.001 100.201 123.601C100.201 123.601 65 94.8 65.8 102z" ) );

b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-54.2 176.401C-54.2 176.401 -43 183.601 -57.4 214.801L-51 212.401C-51 212.401 -51.8 223.601 -55 226.001L-47.8 222.801C-47.8 222.801 -43 230.801 -47 235.601C-47 235.601 -30.2 243.601 -31 250.001C-31 250.001 -24.6 242.001 -28.6 235.601C-32.6 229.201 -39.8 233.201 -39 214.801L-47.8 218.001C-47.8 218.001 -42.2 209.201 -42.2 202.801L-50.2 205.201C-50.2 205.201 -34.731 178.623 -45.4 177.201C-51.4 176.401 -54.2 176.401 -54.2 176.401z" ) );

b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-21.8 193.201C-21.8 193.201 -19 188.801 -21.8 189.601C-24.6 190.401 -55.8 205.201 -61.8 214.801C-61.8 214.801 -27.4 190.401 -21.8 193.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M-11.4 201.201C-11.4 201.201 -8.6 196.801 -11.4 197.601C-14.2 198.401 -45.4 213.201 -51.4 222.801C-51.4 222.801 -17 198.401 -11.4 201.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M1.8 186.001C1.8 186.001 4.6 181.601 1.8 182.401C-1 183.201 -32.2 198.001 -38.2 207.601C-38.2 207.601 -3.8 183.201 1.8 186.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-21.4 229.601C-21.4 229.601 -21.4 223.601 -24.2 224.401C-27 225.201 -63 242.801 -69 252.401C-69 252.401 -27 226.801 -21.4 229.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M-20.2 218.801C-20.2 218.801 -19 214.001 -21.8 214.801C-23.8 214.801 -50.2 226.401 -56.2 236.001C-56.2 236.001 -26.6 214.401 -20.2 218.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M-34.6 266.401L-44.6 274.001C-44.6 274.001 -34.2 266.401 -30.6 267.601C-30.6 267.601 -37.4 278.801 -38.2 284.001C-38.2 284.001 -27.8 271.201 -22.2 271.601C-22.2 271.601 -14.6 272.001 -14.6 282.801C-14.6 282.801 -9 272.401 -5.8 272.801C-5.8 272.801 -4.6 279.201 -5.8 286.001C-5.8 286.001 -1.8 278.401 2.2 280.001C2.2 280.001 8.6 278.001 7.8 289.601C7.8 289.601 7.8 300.001 7 302.801C7 302.801 12.6 276.401 15 276.001C15 276.001 23 274.801 27.8 283.601C27.8 283.601 23.8 276.001 28.6 278.001C28.6 278.001 39.4 279.601 42.6 286.401C42.6 286.401 35.8 274.401 41.4 277.601C41.4 277.601 48.2 277.601 49.4 284.001C49.4 284.001 57.8 305.201 59.8 306.801C59.8 306.801 52.2 285.201 53.8 285.201C53.8 285.201 51.8 273.201 57 288.001C57 288.001 53.8 274.001 59.4 274.801C65 275.601 69.4 285.601 77.8 283.201C77.8 283.201 87.401 288.801 89.401 219.601L-34.6 266.401z" ) );

b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-29.8 173.601C-29.8 173.601 -15 167.601 25 173.601C25 173.601 32.2 174.001 39 165.201C45.8 156.401 72.6 149.201 79 151.201L88.601 157.601L89.401 158.801C89.401 158.801 101.801 169.201 102.201 176.801C102.601 184.401 87.801 232.401 78.2 248.401C68.6 264.401 59 276.801 39.8 274.401C39.8 274.401 19 270.401 -6.6 274.401C-6.6 274.401 -35.8 272.801 -38.6 264.801C-41.4 256.801 -27.4 241.601 -27.4 241.601C-27.4 241.601 -23 233.201 -24.2 218.801C-25.4 204.401 -25 176.401 -29.8 173.601z" ) );

b = new SolidBrush( Color.FromArgb(0xe5, 0x66, 0x8c) );
tigerData.Add( new TigerPath( p, b, @"M-7.8 175.601C0.6 194.001 -29 259.201 -29 259.201C-31 260.801 -16.34 266.846 -6.2 264.401C4.746 261.763 45 266.001 45 266.001C68.6 250.401 81.4 206.001 81.4 206.001C81.4 206.001 91.801 182.001 74.2 178.801C56.6 175.601 -7.8 175.601 -7.8 175.601z" ) );

b = new SolidBrush( Color.FromArgb(0xb2, 0x32, 0x59) );
tigerData.Add( new TigerPath( p, b, @"M-9.831 206.497C-6.505 193.707 -4.921 181.906 -7.8 175.601C-7.8 175.601 54.6 182.001 65.8 161.201C70.041 153.326 84.801 184.001 84.4 193.601C84.4 193.601 21.4 208.001 6.6 196.801L-9.831 206.497z" ) );

b = new SolidBrush( Color.FromArgb(0xa5, 0x26, 0x4c) );
tigerData.Add( new TigerPath( p, b, @"M-5.4 222.801C-5.4 222.801 -3.4 230.001 -5.8 234.001C-5.8 234.001 -7.4 234.801 -8.6 235.201C-8.6 235.201 -7.4 238.801 -1.4 240.401C-1.4 240.401 0.6 244.801 3 245.201C5.4 245.601 10.2 251.201 14.2 250.001C18.2 248.801 29.4 244.801 29.4 244.801C29.4 244.801 35 241.601 43.8 245.201C43.8 245.201 46.175 244.399 46.6 240.401C47.1 235.701 50.2 232.001 52.2 230.001C54.2 228.001 63.8 215.201 62.6 214.801C61.4 214.401 -5.4 222.801 -5.4 222.801z" ) );

b = new SolidBrush( Color.FromArgb(0xff, 0x72, 0x7f) );
tigerData.Add( new TigerPath( p, b, @"M-9.8 174.401C-9.8 174.401 -12.6 196.801 -9.4 205.201C-6.2 213.601 -7 215.601 -7.8 219.601C-8.6 223.601 -4.2 233.601 1.4 239.601L13.4 241.201C13.4 241.201 28.6 237.601 37.8 240.401C37.8 240.401 46.794 241.744 50.2 226.801C50.2 226.801 55 220.401 62.2 217.601C69.4 214.801 76.6 173.201 72.6 165.201C68.6 157.201 54.2 152.801 38.2 168.401C22.2 184.001 20.2 167.201 -9.8 174.401z" ) );

p = new Pen( Color.FromArgb(0,0,0), 0.5f );
b = new SolidBrush( Color.FromArgb(0xff, 0xFF, 0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-8.2 249.201C-8.2 249.201 -9 247.201 -13.4 246.801C-13.4 246.801 -35.8 243.201 -44.2 230.801C-44.2 230.801 -51 225.201 -46.6 236.801C-46.6 236.801 -36.2 257.201 -29.4 260.001C-29.4 260.001 -13 264.001 -8.2 249.201z" ) );

p = null;
b = new SolidBrush( Color.FromArgb(0xcc, 0x3f, 0x4c) );
tigerData.Add( new TigerPath( p, b, @"M71.742 185.229C72.401 177.323 74.354 168.709 72.6 165.201C66.154 152.307 49.181 157.695 38.2 168.401C22.2 184.001 20.2 167.201 -9.8 174.401C-9.8 174.401 -11.545 188.364 -10.705 198.376C-10.705 198.376 26.6 186.801 27.4 192.401C27.4 192.401 29 189.201 38.2 189.201C47.4 189.201 70.142 188.029 71.742 185.229z" ) );

p = new Pen( Color.FromArgb(0xa5,0x19,0x26), 2.0f );
b = null;
tigerData.Add( new TigerPath( p, b, @"M28.6 175.201C28.6 175.201 33.4 180.001 29.8 189.601C29.8 189.601 15.4 205.601 17.4 219.601" ) );

p = new Pen( Color.FromArgb(0,0,0), 0.5f );
b = new SolidBrush( Color.FromArgb(0xff, 0xff, 0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-19.4 260.001C-19.4 260.001 -23.8 247.201 -15 254.001C-15 254.001 -10.2 256.001 -11.4 257.601C-12.6 259.201 -18.2 263.201 -19.4 260.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-14.36 261.201C-14.36 261.201 -17.88 250.961 -10.84 256.401C-10.84 256.401 -6.419 258.849 -7.96 259.281C-12.52 260.561 -7.96 263.121 -14.36 261.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M-9.56 261.201C-9.56 261.201 -13.08 250.961 -6.04 256.401C-6.04 256.401 -1.665 258.711 -3.16 259.281C-6.52 260.561 -3.16 263.121 -9.56 261.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M-2.96 261.401C-2.96 261.401 -6.48 251.161 0.56 256.601C0.56 256.601 4.943 258.933 3.441 259.481C0.48 260.561 3.441 263.321 -2.96 261.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M3.52 261.321C3.52 261.321 0 251.081 7.041 256.521C7.041 256.521 10.881 258.121 9.921 259.401C8.961 260.681 9.921 263.241 3.52 261.321z" ) );
tigerData.Add( new TigerPath( p, b, @"M10.2 262.001C10.2 262.001 5.4 249.601 14.6 256.001C14.6 256.001 19.4 258.001 18.2 259.601C17 261.201 18.2 264.401 10.2 262.001z" ) );

p = new Pen( Color.FromArgb(0xa5,0x26,0x4c), 2f );
b = null;
tigerData.Add( new TigerPath( p, b, @"M-18.2 244.801C-18.2 244.801 -5 242.001 1 245.201C1 245.201 7 246.401 8.2 246.001C9.4 245.601 12.6 245.201 12.6 245.201" ) );
tigerData.Add( new TigerPath( p, b, @"M15.8 253.601C15.8 253.601 27.8 240.001 39.8 244.401C46.816 246.974 45.8 243.601 46.6 240.801C47.4 238.001 47.6 233.801 52.6 230.801" ) );

p = new Pen( Color.FromArgb(0,0,0), 0.5f );
b = new SolidBrush( Color.FromArgb(0xff, 0xff, 0xcc) );
tigerData.Add( new TigerPath( p, b, @"M33 237.601C33 237.601 29 226.801 26.2 239.601C23.4 252.401 20.2 256.001 18.6 258.801C18.6 258.801 18.6 264.001 27 263.601C27 263.601 37.8 263.201 38.2 260.401C38.6 257.601 37 246.001 33 237.601z" ) );

p = new Pen( Color.FromArgb(0xa5,0x26,0x4c), 2f );
b = null;
tigerData.Add( new TigerPath( p, b, @"M47 244.801C47 244.801 50.6 242.401 53 243.601" ) );
tigerData.Add( new TigerPath( p, b, @"M53.5 228.401C53.5 228.401 56.4 223.501 61.2 222.701" ) );

p = null; // new Pen( Color.FromArgb(0,0,0) );
b = new SolidBrush( Color.FromArgb(0xb2, 0xb2, 0xb2) );
tigerData.Add( new TigerPath( p, b, @"M-25.8 265.201C-25.8 265.201 -7.8 268.401 -3.4 266.801C-3.4 266.801 5.4 266.801 -3 268.801C-3 268.801 -15.8 268.801 -23.8 267.601C-23.8 267.601 -35.4 262.001 -25.8 265.201z" ) );

p = new Pen( Color.FromArgb(0,0,0), 0.5f );
b = new SolidBrush( Color.FromArgb(0xff, 0xff, 0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-11.8 172.001C-11.8 172.001 5.8 172.001 7.8 172.801C7.8 172.801 15 203.601 11.4 211.201C11.4 211.201 10.2 214.001 7.4 208.401C7.4 208.401 -11 175.601 -14.2 173.601C-17.4 171.601 -13 172.001 -11.8 172.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-88.9 169.301C-88.9 169.301 -80 171.001 -67.4 173.601C-67.4 173.601 -62.6 196.001 -59.4 200.801C-56.2 205.601 -59.8 205.601 -63.4 202.801C-67 200.001 -81.8 186.001 -83.8 181.601C-85.8 177.201 -88.9 169.301 -88.9 169.301z" ) );
tigerData.Add( new TigerPath( p, b, @"M-67.039 173.818C-67.039 173.818 -61.239 175.366 -60.23 177.581C-59.222 179.795 -61.432 183.092 -61.432 183.092C-61.432 183.092 -62.432 186.397 -63.634 184.235C-64.836 182.072 -67.708 174.412 -67.039 173.818z" ) );

p = null; // new Pen( Color.FromArgb(0,0,0) );
b = new SolidBrush( Color.FromArgb(0x00, 0x00, 0x00) );
tigerData.Add( new TigerPath( p, b, @"M-67 173.601C-67 173.601 -63.4 178.801 -59.8 178.801C-56.2 178.801 -55.818 178.388 -53 179.001C-48.4 180.001 -48.8 178.001 -42.2 179.201C-39.56 179.681 -37 178.801 -34.2 180.001C-31.4 181.201 -28.2 180.401 -27 178.401C-25.8 176.401 -21 172.201 -21 172.201C-21 172.201 -33.8 174.001 -36.6 174.801C-36.6 174.801 -59 176.001 -67 173.601z" ) );

p = new Pen( Color.FromArgb(0,0,0), 0.5f );
b = new SolidBrush( Color.FromArgb(0xff, 0xff, 0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-22.4 173.801C-22.4 173.801 -28.85 177.301 -29.25 179.701C-29.65 182.101 -24 185.801 -24 185.801C-24 185.801 -21.25 190.401 -20.65 188.001C-20.05 185.601 -21.6 174.201 -22.4 173.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M-59.885 179.265C-59.885 179.265 -52.878 190.453 -52.661 179.242C-52.661 179.242 -52.104 177.984 -53.864 177.962C-59.939 177.886 -58.418 173.784 -59.885 179.265z" ) );
tigerData.Add( new TigerPath( p, b, @"M-52.707 179.514C-52.707 179.514 -44.786 190.701 -45.422 179.421C-45.422 179.421 -45.415 179.089 -47.168 178.936C-51.915 178.522 -51.57 174.004 -52.707 179.514z" ) );
tigerData.Add( new TigerPath( p, b, @"M-45.494 179.522C-45.494 179.522 -37.534 190.15 -38.203 180.484C-38.203 180.484 -38.084 179.251 -39.738 178.95C-43.63 178.244 -43.841 174.995 -45.494 179.522z" ) );
tigerData.Add( new TigerPath( p, b, @"M-38.618 179.602C-38.618 179.602 -30.718 191.163 -30.37 181.382C-30.37 181.382 -28.726 180.004 -30.472 179.782C-36.29 179.042 -35.492 174.588 -38.618 179.602z" ) );

p = null; // new Pen( Color.FromArgb(0,0,0) );
b = new SolidBrush( Color.FromArgb(0xe5, 0xe5, 0xb2) );
tigerData.Add( new TigerPath( p, b, @"M-74.792 183.132L-82.45 181.601C-85.05 176.601 -87.15 170.451 -87.15 170.451C-87.15 170.451 -80.8 171.451 -68.3 174.251C-68.3 174.251 -67.424 177.569 -65.952 183.364L-74.792 183.132z" ) );
tigerData.Add( new TigerPath( p, b, @"M-9.724 178.47C-11.39 175.964 -12.707 174.206 -13.357 173.8C-16.37 171.917 -12.227 172.294 -11.098 172.294C-11.098 172.294 5.473 172.294 7.356 173.047C7.356 173.047 7.88 175.289 8.564 178.68C8.564 178.68 -1.524 176.67 -9.724 178.47z" ) );

b = new SolidBrush( Color.FromArgb(0xcc, 0x72, 0x26) );
tigerData.Add( new TigerPath( p, b, @"M43.88 40.321C71.601 44.281 97.121 8.641 98.881 -1.04C100.641 -10.72 90.521 -22.6 90.521 -22.6C91.841 -25.68 87.001 -39.76 81.721 -49C76.441 -58.24 60.54 -57.266 43 -58.24C27.16 -59.12 8.68 -35.8 7.36 -34.04C6.04 -32.28 12.2 6.001 13.52 11.721C14.84 17.441 12.2 43.841 12.2 43.841C46.44 34.741 16.16 36.361 43.88 40.321z" ) );

b = new SolidBrush( Color.FromArgb(0xea, 0x8e, 0x51) );
tigerData.Add( new TigerPath( p, b, @"M8.088 -33.392C6.792 -31.664 12.84 5.921 14.136 11.537C15.432 17.153 12.84 43.073 12.84 43.073C45.512 34.193 16.728 35.729 43.944 39.617C71.161 43.505 96.217 8.513 97.945 -0.992C99.673 -10.496 89.737 -22.16 89.737 -22.16C91.033 -25.184 86.281 -39.008 81.097 -48.08C75.913 -57.152 60.302 -56.195 43.08 -57.152C27.528 -58.016 9.384 -35.12 8.088 -33.392z" ) );

b = new SolidBrush( Color.FromArgb(0xef, 0xaa, 0x7c) );
tigerData.Add( new TigerPath( p, b, @"M8.816 -32.744C7.544 -31.048 13.48 5.841 14.752 11.353C16.024 16.865 13.48 42.305 13.48 42.305C44.884 33.145 17.296 35.097 44.008 38.913C70.721 42.729 95.313 8.385 97.009 -0.944C98.705 -10.272 88.953 -21.72 88.953 -21.72C90.225 -24.688 85.561 -38.256 80.473 -47.16C75.385 -56.064 60.063 -55.125 43.16 -56.064C27.896 -56.912 10.088 -34.44 8.816 -32.744z" ) );

b = new SolidBrush( Color.FromArgb(0xf4, 0xc6, 0xa8) );
tigerData.Add( new TigerPath( p, b, @"M9.544 -32.096C8.296 -30.432 14.12 5.761 15.368 11.169C16.616 16.577 14.12 41.537 14.12 41.537C43.556 32.497 17.864 34.465 44.072 38.209C70.281 41.953 94.409 8.257 96.073 -0.895C97.737 -10.048 88.169 -21.28 88.169 -21.28C89.417 -24.192 84.841 -37.504 79.849 -46.24C74.857 -54.976 59.824 -54.055 43.24 -54.976C28.264 -55.808 10.792 -33.76 9.544 -32.096z" ) );

b = new SolidBrush( Color.FromArgb(0xf9, 0xe2, 0xd3) );
tigerData.Add( new TigerPath( p, b, @"M10.272 -31.448C9.048 -29.816 14.76 5.681 15.984 10.985C17.208 16.289 14.76 40.769 14.76 40.769C42.628 31.849 18.432 33.833 44.136 37.505C69.841 41.177 93.505 8.129 95.137 -0.848C96.769 -9.824 87.385 -20.84 87.385 -20.84C88.609 -23.696 84.121 -36.752 79.225 -45.32C74.329 -53.888 59.585 -52.985 43.32 -53.888C28.632 -54.704 11.496 -33.08 10.272 -31.448z" ) );

b = new SolidBrush( Color.FromArgb(0xff, 0xff, 0xFF) );
tigerData.Add( new TigerPath( p, b, @"M44.2 36.8C69.4 40.4 92.601 8 94.201 -0.8C95.801 -9.6 86.601 -20.4 86.601 -20.4C87.801 -23.2 83.4 -36 78.6 -44.4C73.8 -52.8 59.346 -51.914 43.4 -52.8C29 -53.6 12.2 -32.4 11 -30.8C9.8 -29.2 15.4 5.6 16.6 10.8C17.8 16 15.4 40 15.4 40C40.9 31.4 19 33.2 44.2 36.8z" ) );

b = new SolidBrush( Color.FromArgb(0xcc, 0xcc, 0xcc) );
tigerData.Add( new TigerPath( p, b, @"M90.601 2.8C90.601 2.8 62.8 10.4 51.2 8.8C51.2 8.8 35.4 2.2 26.6 24C26.6 24 23 31.2 21 33.2C19 35.2 90.601 2.8 90.601 2.8z" ) );

b = new SolidBrush( Color.FromArgb(0x00, 0x00, 0x00) );
tigerData.Add( new TigerPath( p, b, @"M94.401 0.6C94.401 0.6 65.4 12.8 55.4 12.4C55.4 12.4 39 7.8 30.6 22.4C30.6 22.4 22.2 31.6 19 33.2C19 33.2 18.6 34.8 25 30.8L35.4 36C35.4 36 50.2 45.6 59.8 29.6C59.8 29.6 63.8 18.4 63.8 16.4C63.8 14.4 85 8.8 86.601 8.4C88.201 8 94.801 3.8 94.401 0.6z" ) );

b = new SolidBrush( Color.FromArgb(0x99, 0xcc, 0x32) );
tigerData.Add( new TigerPath( p, b, @"M47 36.514C40.128 36.514 31.755 32.649 31.755 26.4C31.755 20.152 40.128 13.887 47 13.887C53.874 13.887 59.446 18.952 59.446 25.2C59.446 31.449 53.874 36.514 47 36.514z" ) );

b = new SolidBrush( Color.FromArgb(0x65, 0x99, 0x00) );
tigerData.Add( new TigerPath( p, b, @"M43.377 19.83C38.531 20.552 33.442 22.055 33.514 21.839C35.054 17.22 41.415 13.887 47 13.887C51.296 13.887 55.084 15.865 57.32 18.875C57.32 18.875 52.004 18.545 43.377 19.83z" ) );

b = new SolidBrush( Color.FromArgb(0xff, 0xff, 0xff) );
tigerData.Add( new TigerPath( p, b, @"M55.4 19.6C55.4 19.6 51 16.4 51 18.6C51 18.6 54.6 23 55.4 19.6z" ) );

b = new SolidBrush( Color.FromArgb(0x00, 0x00, 0x00) );
tigerData.Add( new TigerPath( p, b, @"M45.4 27.726C42.901 27.726 40.875 25.7 40.875 23.2C40.875 20.701 42.901 18.675 45.4 18.675C47.9 18.675 49.926 20.701 49.926 23.2C49.926 25.7 47.9 27.726 45.4 27.726z" ) );

b = new SolidBrush( Color.FromArgb(0xcc, 0x72, 0x26) );
tigerData.Add( new TigerPath( p, b, @"M-58.6 14.4C-58.6 14.4 -61.8 -6.8 -59.4 -11.2C-59.4 -11.2 -48.6 -21.2 -49 -24.8C-49 -24.8 -49.4 -42.8 -50.6 -43.6C-51.8 -44.4 -59.4 -50.4 -65.4 -44C-65.4 -44 -75.8 -26 -75 -19.6L-75 -17.6C-75 -17.6 -82.6 -18 -84.2 -16C-84.2 -16 -85.4 -10.8 -86.6 -10.4C-86.6 -10.4 -89.4 -8 -87.4 -5.2C-87.4 -5.2 -89.4 -2.8 -89 1.2L-81.4 5.2C-81.4 5.2 -79.4 19.6 -68.6 24.8C-63.764 27.129 -60.6 20.4 -58.6 14.4z" ) );

b = new SolidBrush( Color.FromArgb(0xff, 0xff, 0xff) );
tigerData.Add( new TigerPath( p, b, @"M-59.6 12.56C-59.6 12.56 -62.48 -6.52 -60.32 -10.48C-60.32 -10.48 -50.6 -19.48 -50.96 -22.72C-50.96 -22.72 -51.32 -38.92 -52.4 -39.64C-53.48 -40.36 -60.32 -45.76 -65.72 -40C-65.72 -40 -75.08 -23.8 -74.36 -18.04L-74.36 -16.24C-74.36 -16.24 -81.2 -16.6 -82.64 -14.8C-82.64 -14.8 -83.72 -10.12 -84.8 -9.76C-84.8 -9.76 -87.32 -7.6 -85.52 -5.08C-85.52 -5.08 -87.32 -2.92 -86.96 0.68L-80.12 4.28C-80.12 4.28 -78.32 17.24 -68.6 21.92C-64.248 24.015 -61.4 17.96 -59.6 12.56z" ) );

b = new SolidBrush( Color.FromArgb(0xeb, 0x95, 0x5c) );
tigerData.Add( new TigerPath( p, b, @"M-51.05 -42.61C-52.14 -43.47 -59.63 -49.24 -65.48 -43C-65.48 -43 -75.62 -25.45 -74.84 -19.21L-74.84 -17.26C-74.84 -17.26 -82.25 -17.65 -83.81 -15.7C-83.81 -15.7 -84.98 -10.63 -86.15 -10.24C-86.15 -10.24 -88.88 -7.9 -86.93 -5.17C-86.93 -5.17 -88.88 -2.83 -88.49 1.07L-81.08 4.97C-81.08 4.97 -79.13 19.01 -68.6 24.08C-63.886 26.35 -60.8 19.79 -58.85 13.94C-58.85 13.94 -61.97 -6.73 -59.63 -11.02C-59.63 -11.02 -49.1 -20.77 -49.49 -24.28C-49.49 -24.28 -49.88 -41.83 -51.05 -42.61z" ) );
b = new SolidBrush( Color.FromArgb(0xf2,0xb8,0x92) );
tigerData.Add( new TigerPath( p, b, @"M-51.5 -41.62C-52.48 -42.54 -59.86 -48.08 -65.56 -42C-65.56 -42 -75.44 -24.9 -74.68 -18.82L-74.68 -16.92C-74.68 -16.92 -81.9 -17.3 -83.42 -15.4C-83.42 -15.4 -84.56 -10.46 -85.7 -10.08C-85.7 -10.08 -88.36 -7.8 -86.46 -5.14C-86.46 -5.14 -88.36 -2.86 -87.98 0.94L-80.76 4.74C-80.76 4.74 -78.86 18.42 -68.6 23.36C-64.006 25.572 -61 19.18 -59.1 13.48C-59.1 13.48 -62.14 -6.66 -59.86 -10.84C-59.86 -10.84 -49.6 -20.34 -49.98 -23.76C-49.98 -23.76 -50.36 -40.86 -51.5 -41.62z" ) );
b = new SolidBrush( Color.FromArgb(0xf8,0xdc,0xc8) );
tigerData.Add( new TigerPath( p, b, @"M-51.95 -40.63C-52.82 -41.61 -60.09 -46.92 -65.64 -41C-65.64 -41 -75.26 -24.35 -74.52 -18.43L-74.52 -16.58C-74.52 -16.58 -81.55 -16.95 -83.03 -15.1C-83.03 -15.1 -84.14 -10.29 -85.25 -9.92C-85.25 -9.92 -87.84 -7.7 -85.99 -5.11C-85.99 -5.11 -87.84 -2.89 -87.47 0.81L-80.44 4.51C-80.44 4.51 -78.59 17.83 -68.6 22.64C-64.127 24.794 -61.2 18.57 -59.35 13.02C-59.35 13.02 -62.31 -6.59 -60.09 -10.66C-60.09 -10.66 -50.1 -19.91 -50.47 -23.24C-50.47 -23.24 -50.84 -39.89 -51.95 -40.63z" ) );
b = new SolidBrush( Color.FromArgb(0xff,0xff,0xff) );
tigerData.Add( new TigerPath( p, b, @"M-59.6 12.46C-59.6 12.46 -62.48 -6.52 -60.32 -10.48C-60.32 -10.48 -50.6 -19.48 -50.96 -22.72C-50.96 -22.72 -51.32 -38.92 -52.4 -39.64C-53.16 -40.68 -60.32 -45.76 -65.72 -40C-65.72 -40 -75.08 -23.8 -74.36 -18.04L-74.36 -16.24C-74.36 -16.24 -81.2 -16.6 -82.64 -14.8C-82.64 -14.8 -83.72 -10.12 -84.8 -9.76C-84.8 -9.76 -87.32 -7.6 -85.52 -5.08C-85.52 -5.08 -87.32 -2.92 -86.96 0.68L-80.12 4.28C-80.12 4.28 -78.32 17.24 -68.6 21.92C-64.248 24.015 -61.4 17.86 -59.6 12.46z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-62.7 6.2C-62.7 6.2 -84.3 -4 -85.2 -4.8C-85.2 -4.8 -76.1 3.4 -75.3 3.4C-74.5 3.4 -62.7 6.2 -62.7 6.2z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-79.8 0C-79.8 0 -61.4 3.6 -61.4 8C-61.4 10.912 -61.643 24.331 -67 22.8C-75.4 20.4 -71.8 6 -79.8 0z" ) );
b = new SolidBrush( Color.FromArgb(0x99,0xcc,0x32) );
tigerData.Add( new TigerPath( p, b, @"M-71.4 3.8C-71.4 3.8 -62.422 5.274 -61.4 8C-60.8 9.6 -60.137 17.908 -65.6 19C-70.152 19.911 -72.382 9.69 -71.4 3.8z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M14.595 46.349C14.098 44.607 15.409 44.738 17.2 44.2C19.2 43.6 31.4 39.8 32.2 37.2C33 34.6 46.2 39 46.2 39C48 39.8 52.4 42.4 52.4 42.4C57.2 43.6 63.8 44 63.8 44C66.2 45 69.6 47.8 69.6 47.8C84.2 58 96.601 50.8 96.601 50.8C116.601 44.2 110.601 27 110.601 27C107.601 18 110.801 14.6 110.801 14.6C111.001 10.8 118.201 17.2 118.201 17.2C120.801 21.4 121.601 26.4 121.601 26.4C129.601 37.6 126.201 19.8 126.201 19.8C126.401 18.8 123.601 15.2 123.601 14C123.601 12.8 121.801 9.4 121.801 9.4C118.801 6 121.201 -1 121.201 -1C123.001 -14.8 120.801 -13 120.801 -13C119.601 -14.8 110.401 -4.8 110.401 -4.8C108.201 -1.4 102.201 0.2 102.201 0.2C99.401 2 96.001 0.6 96.001 0.6C93.401 0.2 87.801 7.2 87.801 7.2C90.601 7 93.001 11.4 95.401 11.6C97.801 11.8 99.601 9.2 101.201 8.6C102.801 8 105.601 13.8 105.601 13.8C106.001 16.4 100.401 21.2 100.401 21.2C100.001 25.8 98.401 24.2 98.401 24.2C95.401 23.6 94.201 27.4 93.201 32C92.201 36.6 88.001 37 88.001 37C86.401 44.4 85.2 41.4 85.2 41.4C85 35.8 79 41.6 79 41.6C77.8 43.6 73.2 41.4 73.2 41.4C66.4 39.4 68.8 37.4 68.8 37.4C70.6 35.2 81.8 37.4 81.8 37.4C84 35.8 76 31.8 76 31.8C75.4 30 76.4 25.6 76.4 25.6C77.6 22.4 84.4 16.8 84.4 16.8C93.801 15.6 91.001 14 91.001 14C84.801 8.8 79 16.4 79 16.4C76.8 22.6 59.4 37.6 59.4 37.6C54.6 41 57.2 34.2 53.2 37.6C49.2 41 28.6 32 28.6 32C17.038 30.807 14.306 46.549 10.777 43.429C10.777 43.429 16.195 51.949 14.595 46.349z" ) );
tigerData.Add( new TigerPath( p, b, @"M209.401 -120C209.401 -120 183.801 -112 181.001 -93.2C181.001 -93.2 178.601 -70.4 199.001 -52.8C199.001 -52.8 199.401 -46.4 201.401 -43.2C201.401 -43.2 199.801 -38.4 218.601 -46L245.801 -54.4C245.801 -54.4 252.201 -56.8 257.401 -65.6C262.601 -74.4 277.801 -93.2 274.201 -118.4C274.201 -118.4 275.401 -129.6 269.401 -130C269.401 -130 261.001 -131.6 253.801 -124C253.801 -124 247.001 -120.8 244.601 -121.2L209.401 -120z" ) );
tigerData.Add( new TigerPath( p, b, @"M264.022 -120.99C264.022 -120.99 266.122 -129.92 261.282 -125.08C261.282 -125.08 254.242 -119.36 246.761 -119.36C246.761 -119.36 232.241 -117.16 227.841 -103.96C227.841 -103.96 223.881 -77.12 231.801 -71.4C231.801 -71.4 236.641 -63.92 243.681 -70.52C250.722 -77.12 266.222 -107.35 264.022 -120.99z" ) );
b = new SolidBrush( Color.FromArgb(0x32,0x32,0x32) );
tigerData.Add( new TigerPath( p, b, @"M263.648 -120.632C263.648 -120.632 265.738 -129.376 260.986 -124.624C260.986 -124.624 254.074 -119.008 246.729 -119.008C246.729 -119.008 232.473 -116.848 228.153 -103.888C228.153 -103.888 224.265 -77.536 232.041 -71.92C232.041 -71.92 236.793 -64.576 243.705 -71.056C250.618 -77.536 265.808 -107.24 263.648 -120.632z" ) );
b = new SolidBrush( Color.FromArgb(0x66,0x66,0x66) );
tigerData.Add( new TigerPath( p, b, @"M263.274 -120.274C263.274 -120.274 265.354 -128.832 260.69 -124.168C260.69 -124.168 253.906 -118.656 246.697 -118.656C246.697 -118.656 232.705 -116.536 228.465 -103.816C228.465 -103.816 224.649 -77.952 232.281 -72.44C232.281 -72.44 236.945 -65.232 243.729 -71.592C250.514 -77.952 265.394 -107.13 263.274 -120.274z" ) );
b = new SolidBrush( Color.FromArgb(0x99,0x99,0x99) );
tigerData.Add( new TigerPath( p, b, @"M262.9 -119.916C262.9 -119.916 264.97 -128.288 260.394 -123.712C260.394 -123.712 253.738 -118.304 246.665 -118.304C246.665 -118.304 232.937 -116.224 228.777 -103.744C228.777 -103.744 225.033 -78.368 232.521 -72.96C232.521 -72.96 237.097 -65.888 243.753 -72.128C250.41 -78.368 264.98 -107.02 262.9 -119.916z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M262.526 -119.558C262.526 -119.558 264.586 -127.744 260.098 -123.256C260.098 -123.256 253.569 -117.952 246.633 -117.952C246.633 -117.952 233.169 -115.912 229.089 -103.672C229.089 -103.672 225.417 -78.784 232.761 -73.48C232.761 -73.48 237.249 -66.544 243.777 -72.664C250.305 -78.784 264.566 -106.91 262.526 -119.558z" ) );
b = new SolidBrush( Color.FromArgb(0xff,0xff,0xff) );
tigerData.Add( new TigerPath( p, b, @"M262.151 -119.2C262.151 -119.2 264.201 -127.2 259.801 -122.8C259.801 -122.8 253.401 -117.6 246.601 -117.6C246.601 -117.6 233.401 -115.6 229.401 -103.6C229.401 -103.6 225.801 -79.2 233.001 -74C233.001 -74 237.401 -67.2 243.801 -73.2C250.201 -79.2 264.151 -106.8 262.151 -119.2z" ) );
b = new SolidBrush( Color.FromArgb(0x99,0x26,0x00) );
tigerData.Add( new TigerPath( p, b, @"M50.6 84C50.6 84 30.2 64.8 22.2 64C22.2 64 -12.2 60 -27 78C-27 78 -9.4 57.6 18.2 63.2C18.2 63.2 -3.4 58.8 -15.8 62C-15.8 62 -32.6 62 -42.2 76L-45 80.8C-45 80.8 -41 66 -22.6 60C-22.6 60 0.2 55.2 11 60C11 60 -10.6 53.2 -20.6 55.2C-20.6 55.2 -51 52.8 -63.8 79.2C-63.8 79.2 -59.8 64.8 -45 57.6C-45 57.6 -31.4 48.8 -11 51.6C-11 51.6 3.4 54.8 8.6 57.2C13.8 59.6 12.6 56.8 4.2 52C4.2 52 -1.4 42 -15.4 42.4C-15.4 42.4 -58.2 46 -68.6 58C-68.6 58 -55 46.8 -44.6 44C-44.6 44 -22.2 36 -13.8 36.8C-13.8 36.8 11 37.8 18.6 33.8C18.6 33.8 7.4 38.8 10.6 42C13.8 45.2 20.6 52.8 20.6 54C20.6 55.2 44.8 77.3 48.4 81.7L50.6 84z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M189 278C189 278 173.5 241.5 161 232C161 232 187 248 190.5 266C190.5 266 190.5 276 189 278z" ) );
tigerData.Add( new TigerPath( p, b, @"M236 285.5C236 285.5 209.5 230.5 191 206.5C191 206.5 234.5 244 239.5 270.5L240 276L237 273.5C237 273.5 236.5 282.5 236 285.5z" ) );
tigerData.Add( new TigerPath( p, b, @"M292.5 237C292.5 237 230 177.5 228.5 175C228.5 175 289 241 292 248.5C292 248.5 290 239.5 292.5 237z" ) );
tigerData.Add( new TigerPath( p, b, @"M104 280.5C104 280.5 123.5 228.5 142.5 251C142.5 251 157.5 261 157 264C157 264 153 257.5 135 258C135 258 116 255 104 280.5z" ) );
tigerData.Add( new TigerPath( p, b, @"M294.5 153C294.5 153 249.5 124.5 242 123C230.193 120.639 291.5 152 296.5 162.5C296.5 162.5 298.5 160 294.5 153z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M143.801 259.601C143.801 259.601 164.201 257.601 171.001 250.801L175.401 254.401L193.001 216.001L196.601 221.201C196.601 221.201 211.001 206.401 210.201 198.401C209.401 190.401 223.001 204.401 223.001 204.401C223.001 204.401 222.201 192.801 229.401 199.601C229.401 199.601 227.001 184.001 235.401 192.001C235.401 192.001 224.864 161.844 247.401 187.601C253.001 194.001 248.601 187.201 248.601 187.201C248.601 187.201 222.601 139.201 244.201 153.601C244.201 153.601 246.201 130.801 245.001 126.401C243.801 122.001 241.801 99.6 237.001 94.4C232.201 89.2 237.401 87.6 243.001 92.8C243.001 92.8 231.801 68.8 245.001 80.8C245.001 80.8 241.401 65.6 237.001 62.8C237.001 62.8 231.401 45.6 246.601 56.4C246.601 56.4 242.201 44 239.001 40.8C239.001 40.8 227.401 13.2 234.601 18L239.001 21.6C239.001 21.6 232.201 7.6 238.601 12C245.001 16.4 245.001 16 245.001 16C245.001 16 223.801 -17.2 244.201 0.4C244.201 0.4 236.042 -13.518 232.601 -20.4C232.601 -20.4 213.801 -40.8 228.201 -34.4L233.001 -32.8C233.001 -32.8 224.201 -42.8 216.201 -44.4C208.201 -46 218.601 -52.4 225.001 -50.4C231.401 -48.4 247.001 -40.8 247.001 -40.8C247.001 -40.8 259.801 -22 263.801 -21.6C263.801 -21.6 243.801 -29.2 249.801 -21.2C249.801 -21.2 264.201 -7.2 257.001 -7.6C257.001 -7.6 251.001 -0.4 255.801 8.4C255.801 8.4 237.342 -9.991 252.201 15.6L259.001 32C259.001 32 234.601 7.2 245.801 29.2C245.801 29.2 263.001 52.8 265.001 53.2C267.001 53.6 271.401 62.4 271.401 62.4L267.001 60.4L272.201 69.2C272.201 69.2 261.001 57.2 267.001 70.4L272.601 84.8C272.601 84.8 252.201 62.8 265.801 92.4C265.801 92.4 249.401 87.2 258.201 104.4C258.201 104.4 256.601 120.401 257.001 125.601C257.401 130.801 258.601 159.201 254.201 167.201C249.801 175.201 260.201 194.401 262.201 198.401C264.201 202.401 267.801 213.201 259.001 204.001C250.201 194.801 254.601 200.401 256.601 209.201C258.601 218.001 264.601 233.601 263.801 239.201C263.801 239.201 262.601 240.401 259.401 236.801C259.401 236.801 244.601 214.001 246.201 228.401C246.201 228.401 245.001 236.401 241.801 245.2
							C241.801 245.201 238.601 256.001 238.601 247.201C238.601 247.201 235.401 230.401 232.601 238.001C229.801 245.601 226.201 251.601 223.401 254.001C220.601 256.401 215.401 233.601 214.201 244.001C214.201 244.001 202.201 231.601 197.401 248.001L185.801 264.401C185.801 264.401 185.401 252.001 184.201 258.001C184.201 258.001 154.201 264.001 143.801 259.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M109.401 -97.2C109.401 -97.2 97.801 -105.2 93.801 -104.8C89.801 -104.4 121.401 -113.6 162.601 -86C162.601 -86 167.401 -83.2 171.001 -83.6C171.001 -83.6 174.201 -81.2 171.401 -77.6C171.401 -77.6 162.601 -68 173.801 -56.8C173.801 -56.8 192.201 -50 186.601 -58.8C186.601 -58.8 197.401 -54.8 199.801 -50.8C202.201 -46.8 201.001 -50.8 201.001 -50.8C201.001 -50.8 194.601 -58 188.601 -63.2C188.601 -63.2 183.401 -65.2 180.601 -73.6C177.801 -82 175.401 -92 179.801 -95.2C179.801 -95.2 175.801 -90.8 176.601 -94.8C177.401 -98.8 181.001 -102.4 182.601 -102.8C184.201 -103.2 200.601 -119 207.401 -119.4C207.401 -119.4 198.201 -118 195.201 -119C192.201 -120 165.601 -131.4 159.601 -132.6C159.601 -132.6 142.801 -139.2 154.801 -137.2C154.801 -137.2 190.601 -133.4 208.801 -120.2C208.801 -120.2 201.601 -128.6 183.201 -135.6C183.201 -135.6 161.001 -148.2 125.801 -143.2C125.801 -143.2 108.001 -140 100.201 -138.2C100.201 -138.2 97.601 -138.8 97.001 -139.2C96.401 -139.6 84.6 -148.6 57 -141.6C57 -141.6 40 -137 31.4 -132.2C31.4 -132.2 16.2 -131 12.6 -127.8C12.6 -127.8 -6 -113.2 -8 -112.4C-10 -111.6 -21.4 -104 -22.2 -103.6C-22.2 -103.6 2.4 -110.2 4.8 -112.6C7.2 -115 24.6 -117.6 27 -116.2C29.4 -114.8 37.8 -115.4 28.2 -114.8C28.2 -114.8 103.801 -100 104.601 -98C105.401 -96 109.401 -97.2 109.401 -97.2z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0x72,0x26) );
tigerData.Add( new TigerPath( p, b, @"M180.801 -106.4C180.801 -106.4 170.601 -113.8 168.601 -113.8C166.601 -113.8 154.201 -124 150.001 -123.6C145.801 -123.2 133.601 -133.2 106.201 -125C106.201 -125 105.601 -127 109.201 -127.8C109.201 -127.8 115.601 -130 116.001 -130.6C116.001 -130.6 136.201 -134.8 143.401 -131.2C143.401 -131.2 152.601 -128.6 158.801 -122.4C158.801 -122.4 170.001 -119.2 173.201 -120.2C173.201 -120.2 182.001 -118 182.401 -116.2C182.401 -116.2 188.201 -113.2 186.401 -110.6C186.401 -110.6 186.801 -109 180.801 -106.4z" ) );
tigerData.Add( new TigerPath( p, b, @"M168.33 -108.509C169.137 -107.877 170.156 -107.779 170.761 -106.97C170.995 -106.656 170.706 -106.33 170.391 -106.233C169.348 -105.916 168.292 -106.486 167.15 -105.898C166.748 -105.691 166.106 -105.873 165.553 -106.022C163.921 -106.463 162.092 -106.488 160.401 -105.8C158.416 -106.929 156.056 -106.345 153.975 -107.346C153.917 -107.373 153.695 -107.027 153.621 -107.054C150.575 -108.199 146.832 -107.916 144.401 -110.2C141.973 -110.612 139.616 -111.074 137.188 -111.754C135.37 -112.263 133.961 -113.252 132.341 -114.084C130.964 -114.792 129.507 -115.314 127.973 -115.686C126.11 -116.138 124.279 -116.026 122.386 -116.546C122.293 -116.571 122.101 -116.227 122.019 -116.254C121.695 -116.362 121.405 -116.945 121.234 -116.892C119.553 -116.37 118.065 -117.342 116.401 -117C115.223 -118.224 113.495 -117.979 111.949 -118.421C108.985 -119.269 105.831 -117.999 102.801 -119C106.914 -120.842 111.601 -119.61 115.663 -121.679C117.991 -122.865 120.653 -121.763 123.223 -122.523C123.71 -122.667 124.401 -122.869 124.801 -122.2C124.935 -122.335 125.117 -122.574 125.175 -122.546C127.625 -121.389 129.94 -120.115 132.422 -119.049C132.763 -118.903 133.295 -119.135 133.547 -118.933C135.067 -117.717 137.01 -117.82 138.401 -116.6C140.099 -117.102 141.892 -116.722 143.621 -117.346C143.698 -117.373 143.932 -117.032 143.965 -117.054C145.095 -117.802 146.25 -117.531 147.142 -117.227C147.48 -117.112 148.143 -116.865 148.448 -116.791C149.574 -116.515 150.43 -116.035 151.609 -115.852C151.723 -115.834 151.908 -116.174 151.98 -116.146C153.103 -115.708 154.145 -115.764 154.801 -114.6C154.936 -114.735 155.101 -114.973 155.183 -114.946C156.21 -114.608 156.859 -113.853 157.96 -113.612C158.445 -113.506 159.057 -112.88 159.633 -112.704C162.025 -111.973 163.868 -110.444 166.062 -109.549C166.821 -109.239 167.697 -109.005 168.33 -108.509z" ) );
tigerData.Add( new TigerPath( p, b, @"M91.696 -122.739C89.178 -124.464 86.81 -125.57 84.368 -127.356C84.187 -127.489 83.827 -127.319 83.625 -127.441C82.618 -128.05 81.73 -128.631 80.748 -129.327C80.209 -129.709 79.388 -129.698 78.88 -129.956C76.336 -131.248 73.707 -131.806 71.2 -133C71.882 -133.638 73.004 -133.394 73.6 -134.2C73.795 -133.92 74.033 -133.636 74.386 -133.827C76.064 -134.731 77.914 -134.884 79.59 -134.794C81.294 -134.702 83.014 -134.397 84.789 -134.125C85.096 -134.078 85.295 -133.555 85.618 -133.458C87.846 -132.795 90.235 -133.32 92.354 -132.482C93.945 -131.853 95.515 -131.03 96.754 -129.755C97.006 -129.495 96.681 -129.194 96.401 -129C96.789 -129.109 97.062 -128.903 97.173 -128.59C97.257 -128.351 97.257 -128.049 97.173 -127.81C97.061 -127.498 96.782 -127.397 96.408 -127.346C95.001 -127.156 96.773 -128.536 96.073 -128.088C94.8 -127.274 95.546 -125.868 94.801 -124.6C94.521 -124.794 94.291 -125.012 94.401 -125.4C94.635 -124.878 94.033 -124.588 93.865 -124.272C93.48 -123.547 92.581 -122.132 91.696 -122.739z" ) );
tigerData.Add( new TigerPath( p, b, @"M59.198 -115.391C56.044 -116.185 52.994 -116.07 49.978 -117.346C49.911 -117.374 49.688 -117.027 49.624 -117.054C48.258 -117.648 47.34 -118.614 46.264 -119.66C45.351 -120.548 43.693 -120.161 42.419 -120.648C42.095 -120.772 41.892 -121.284 41.591 -121.323C40.372 -121.48 39.445 -122.429 38.4 -123C40.736 -123.795 43.147 -123.764 45.609 -124.148C45.722 -124.166 45.867 -123.845 46 -123.845C46.136 -123.845 46.266 -124.066 46.4 -124.2C46.595 -123.92 46.897 -123.594 47.154 -123.848C47.702 -124.388 48.258 -124.198 48.798 -124.158C48.942 -124.148 49.067 -123.845 49.2 -123.845C49.336 -123.845 49.467 -124.156 49.6 -124.156C49.736 -124.155 49.867 -123.845 50 -123.845C50.136 -123.845 50.266 -124.066 50.4 -124.2C51.092 -123.418 51.977 -123.972 52.799 -123.793C53.837 -123.566 54.104 -122.418 55.178 -122.12C59.893 -120.816 64.03 -118.671 68.393 -116.584C68.7 -116.437 68.91 -116.189 68.8 -115.8C69.067 -115.8 69.38 -115.888 69.57 -115.756C70.628 -115.024 71.669 -114.476 72.366 -113.378C72.582 -113.039 72.253 -112.632 72.02 -112.684C67.591 -113.679 63.585 -114.287 59.198 -115.391z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0x72,0x26) );
tigerData.Add( new TigerPath( p, b, @"M45.338 -71.179C43.746 -72.398 43.162 -74.429 42.034 -76.221C41.82 -76.561 42.094 -76.875 42.411 -76.964C42.971 -77.123 43.514 -76.645 43.923 -76.443C45.668 -75.581 47.203 -74.339 49.2 -74.2C51.19 -71.966 55.45 -71.581 55.457 -68.2C55.458 -67.341 54.03 -68.259 53.6 -67.4C51.149 -68.403 48.76 -68.3 46.38 -69.767C45.763 -70.148 46.093 -70.601 45.338 -71.179z" ) );
tigerData.Add( new TigerPath( p, b, @"M17.8 -123.756C17.935 -123.755 24.966 -123.522 24.949 -123.408C24.904 -123.099 17.174 -122.05 16.81 -122.22C16.646 -122.296 9.134 -119.866 9 -120C9.268 -120.135 17.534 -123.756 17.8 -123.756z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M33.2 -114C33.2 -114 18.4 -112.2 14 -111C9.6 -109.8 -9 -102.2 -12 -100.2C-12 -100.2 -25.4 -94.8 -42.4 -74.8C-42.4 -74.8 -34.8 -78.2 -32.6 -81C-32.6 -81 -19 -93.6 -19.2 -91C-19.2 -91 -7 -99.6 -7.6 -97.4C-7.6 -97.4 16.8 -108.6 14.8 -105.4C14.8 -105.4 36.4 -110 35.4 -108C35.4 -108 54.2 -103.6 51.4 -103.4C51.4 -103.4 45.6 -102.2 52 -98.6C52 -98.6 48.6 -94.2 43.2 -98.2C37.8 -102.2 40.8 -100 35.8 -99C35.8 -99 33.2 -98.2 28.6 -102.2C28.6 -102.2 23 -106.8 14.2 -103.2C14.2 -103.2 -16.4 -90.6 -18.4 -90C-18.4 -90 -22 -87.2 -24.4 -83.6C-24.4 -83.6 -30.2 -79.2 -33.2 -77.8C-33.2 -77.8 -46 -66.2 -47.2 -64.8C-47.2 -64.8 -50.6 -59.6 -51.4 -59.2C-51.4 -59.2 -45 -63 -43 -65C-43 -65 -29 -75 -23.6 -75.8C-23.6 -75.8 -19.2 -78.8 -18.4 -80.2C-18.4 -80.2 -4 -89.4 0.2 -89.4C0.2 -89.4 9.4 -84.2 11.8 -91.2C11.8 -91.2 17.6 -93 23.2 -91.8C23.2 -91.8 26.4 -94.4 25.6 -96.6C25.6 -96.6 27.2 -98.4 28.2 -94.6C28.2 -94.6 31.6 -91 36.4 -93C36.4 -93 40.4 -93.2 38.4 -90.8C38.4 -90.8 34 -87 22.2 -86.8C22.2 -86.8 9.8 -86.2 -6.6 -78.6C-6.6 -78.6 -36.4 -68.2 -45.6 -57.8C-45.6 -57.8 -52 -49 -57.4 -47.8C-57.4 -47.8 -63.2 -47 -69.2 -39.6C-69.2 -39.6 -59.4 -45.4 -50.4 -45.4C-50.4 -45.4 -46.4 -47.8 -50.2 -44.2C-50.2 -44.2 -53.8 -36.6 -52.2 -31.2C-52.2 -31.2 -52.8 -26 -53.6 -24.4C-53.6 -24.4 -61.4 -11.6 -61.4 -9.2C-61.4 -6.8 -60.2 3 -59.8 3.6C-59.4 4.2 -60.8 2 -57 4.4C-53.2 6.8 -50.4 8.4 -49.6 11.2C-48.8 14 -51.6 5.8 -51.8 4C-52 2.2 -56.2 -5 -55.4 -7.4C-55.4 -7.4 -54.4 -6.4 -53.6 -5C-53.6 -5 -54.2 -5.6 -53.6 -9.2C-53.6 -9.2 -52.8 -14.4 -51.4 -17.6C-50 -20.8 -48 -24.6 -47.6 -25.4C-47.2 -26.2 -47.2 -32 -45.8 -29.4L-42.4 -26.8C-42.4 -26.8 -45.2 -29.4 -43 -31.6C-43 -31.6 -44 -37.2 -42.2 -39.8C-42.2 -39.8 -35.2 -48.2 -33.6 -49.2C-32 -50.2 -33.4 -49.8 -33.4 -49.8C-33.4 -49.8 -27.4 -54 -33.2 -52.4C-33.2 -52.4 -37.2 -50.8 -40.2 -50.8C-40.2 -50.8 -47.8 -48.8 -43.8 -53C-39.8 -57.2 -29.8 -62.6 -26 -62.4L-25.2 -60.8L-14 -63.2L-15.2 -62.4C-15.2 -62.4 -15.4 -62.6 -11.2 -63C-7 -63.4 -1.2 -62 0.2 -63.8C1.6 -65.6 5 -66.6 4.6 -65.2
									C4.2 -63.8 4 -61.8 4 -61
											C4 -61.8 9 -67.6 8.4 -65.4C7.8 -63.2 -0.4 -58 -1.8 -51.8L8.6 -60L12.2 -63C12.2 -63 15.8 -60.8 16 -62.4C16.2 -64 20.8 -69.8 22 -69.6C23.2 -69.4 25.2 -72.2 25 -69.6C24.8 -67 32.4 -61.6 32.4 -61.6C32.4 -61.6 35.6 -63.4 37 -62C38.4 -60.6 42.6 -81.8 42.6 -81.8L67.6 -92.4L111.201 -95.8L94.201 -102.6L33.2 -114z" ) );
p = new Pen( Color.FromArgb(0x4c,0x00,0x00), 2.0f );
b = null;
tigerData.Add( new TigerPath( p, b, @"M51.4 85C51.4 85 36.4 68.2 28 65.6C28 65.6 14.6 58.8 -10 66.6" ) );
tigerData.Add( new TigerPath( p, b, @"M24.8 64.2C24.8 64.2 -0.4 56.2 -15.8 60.4C-15.8 60.4 -34.2 62.4 -42.6 76.2" ) );
tigerData.Add( new TigerPath( p, b, @"M21.2 63C21.2 63 4.2 55.8 -10.6 53.6C-10.6 53.6 -27.2 51 -43.8 58.2C-43.8 58.2 -56 64.2 -61.4 74.4" ) );
tigerData.Add( new TigerPath( p, b, @"M22.2 63.4C22.2 63.4 6.8 52.4 5.8 51C5.8 51 -1.2 40 -14.2 39.6C-14.2 39.6 -35.6 40.4 -52.8 48.4" ) );
p = null;
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M20.895 54.407C22.437 55.87 49.4 84.8 49.4 84.8C84.6 121.401 56.6 87.2 56.6 87.2C49 82.4 39.8 63.6 39.8 63.6C38.6 60.8 53.8 70.8 53.8 70.8C57.8 71.6 71.4 90.8 71.4 90.8C64.6 88.4 69.4 95.6 69.4 95.6C72.2 97.6 92.601 113.201 92.601 113.201C96.201 117.201 100.201 118.801 100.201 118.801C114.201 113.601 107.801 126.801 107.801 126.801C110.201 133.601 115.801 122.001 115.801 122.001C127.001 105.2 110.601 107.601 110.601 107.601C80.6 110.401 73.8 94.4 73.8 94.4C71.4 92 80.2 94.4 80.2 94.4C88.601 96.4 73 82 73 82C75.4 82 84.6 88.8 84.6 88.8C95.001 98 97.001 96 97.001 96C115.001 87.2 125.401 94.8 125.401 94.8C127.401 96.4 121.801 103.2 123.401 108.401C125.001 113.601 129.801 126.001 129.801 126.001C127.401 127.601 127.801 138.401 127.801 138.401C144.601 161.601 135.001 159.601 135.001 159.601C119.401 159.201 134.201 166.801 134.201 166.801C137.401 168.801 146.201 176.001 146.201 176.001C143.401 174.801 141.801 180.001 141.801 180.001C146.601 184.001 143.801 188.801 143.801 188.801C137.801 190.001 136.601 194.001 136.601 194.001C143.401 202.001 133.401 202.401 133.401 202.401C137.001 206.801 132.201 218.801 132.201 218.801C127.401 218.801 121.001 224.401 121.001 224.401C123.401 229.201 113.001 234.801 113.001 234.801C104.601 236.401 107.401 243.201 107.401 243.201C99.401 249.201 97.001 265.201 97.001 265.201C96.201 275.601 93.801 278.801 99.001 276.801C104.201 274.801 103.401 262.401 103.401 262.401C98.601 246.801 141.401 230.801 141.401 230.801C145.401 229.201 146.201 224.001 146.201 224.001C148.201 224.401 157.001 232.001 157.001 232.001C164.601 243.201 165.001 234.001 165.001 234.001C166.201 230.401 164.601 224.401 164.601 224.401C170.601 202.801 156.601 196.401 156.601 196.401C146.601 162.801 160.601 171.201 160.601 171.201C163.401 176.801 174.201 182.001 174.201 182.001L177.801 179.601C176.201 174.801 184.601 168.801 184.601 168.801C187.401 175.201 193.401 167.201 193.401 167.201C197.001 142.801 209.401 157.201 209.401 157.201C213.401 158.401 214.601 151.601 214.601 151.6
											C218.201 141.201 214.601 127.601 214.601 127.601C218.201 127.201 227.801 133.201 227.801 133.201C230.601 129.601 221.401 112.801 225.401 115.201C229.401 117.601 233.801 119.201 233.801 119.201C234.601 117.201 224.601 104.801 224.601 104.801C220.201 102 215.001 81.6 215.001 81.6C222.201 85.2 212.201 70 212.201 70C212.201 66.8 218.201 55.6 218.201 55.6C217.401 48.8 218.201 49.2 218.201 49.2C221.001 50.4 229.001 52 222.201 45.6C215.401 39.2 223.001 34.4 223.001 34.4C227.401 31.6 213.801 32 213.801 32C208.601 27.6 209.001 23.6 209.001 23.6C217.001 25.6 202.601 11.2 200.201 7.6C197.801 4 207.401 -1.2 207.401 -1.2C220.601 -4.8 209.001 -8 209.001 -8C189.401 -7.6 200.201 -18.4 200.201 -18.4C206.201 -18 204.601 -20.4 204.601 -20.4C199.401 -21.6 189.801 -28 189.801 -28C185.801 -31.6 189.401 -30.8 189.401 -30.8C206.201 -29.6 177.401 -40.8 177.401 -40.8C185.401 -40.8 167.401 -51.2 167.401 -51.2C165.401 -52.8 162.201 -60.4 162.201 -60.4C156.201 -65.6 151.401 -72.4 151.401 -72.4C151.001 -76.8 146.201 -81.6 146.201 -81.6C134.601 -95.2 129.001 -94.8 129.001 -94.8C114.201 -98.4 109.001 -97.6 109.001 -97.6L56.2 -93.2C29.8 -80.4 37.6 -59.4 37.6 -59.4C44 -51 53.2 -54.8 53.2 -54.8C57.8 -61 69.4 -58.8 69.4 -58.8C89.801 -55.6 87.201 -59.2 87.201 -59.2C84.801 -63.8 68.6 -70 68.4 -70.6C68.2 -71.2 59.4 -74.6 59.4 -74.6C56.4 -75.8 52 -85 52 -85C48.8 -88.4 64.6 -82.6 64.6 -82.6C63.4 -81.6 70.8 -77.6 70.8 -77.6C88.201 -78.6 98.801 -67.8 98.801 -67.8C109.601 -51.2 109.801 -59.4 109.801 -59.4C112.601 -68.8 100.801 -90 100.801 -90C101.201 -92 109.401 -85.4 109.401 -85.4C110.801 -87.4 111.601 -81.6 111.601 -81.6C111.801 -79.2 115.601 -71.2 115.601 -71.2C118.401 -58.2 122.001 -65.6 122.001 -65.6L126.601 -56.2C128.001 -53.6 122.001 -46 122.001 -46C121.801 -43.2 122.601 -43.4 117.001 -35.8C111.401 -28.2 114.801 -23.8 114.801 -23.8C113.401 -17.2 122.201 -17.6 122.201 -17.6C124.801 -15.4 128.201 -15.4 128.201 -15.4C130.001 -13.4 132.401 -14 132.401 -14C134.001 -17.8 140.201 -15.8 140.201 -15.8C141.601 -18.2 149.801 -18.6 149.801 -18.6
													C150.801 -21.2 151.201 -22.8 154.601 -23.4C158.001 -24 133.401 -67 133.401 -67C139.801 -67.8 131.601 -80.2 131.601 -80.2C129.401 -86.8 140.801 -72.2 143.001 -70.8C145.201 -69.4 146.201 -67.2 144.601 -67.4C143.001 -67.6 141.201 -65.4 142.601 -65.2C144.001 -65 157.001 -50 160.401 -39.8C163.801 -29.6 169.801 -25.6 176.001 -19.6C182.201 -13.6 181.401 10.6 181.401 10.6C181.001 19.4 187.001 30 187.001 30C189.001 33.8 184.801 52 184.801 52C182.801 54.2 184.201 55 184.201 55C185.201 56.2 192.001 69.4 192.001 69.4C190.201 69.2 193.801 72.8 193.801 72.8C199.001 78.8 192.601 75.8 192.601 75.8C186.601 74.2 193.601 84 193.601 84C194.801 85.8 185.801 81.2 185.801 81.2C176.601 80.6 188.201 87.8 188.201 87.8C196.801 95 185.401 90.6 185.401 90.6C180.801 88.8 184.001 95.6 184.001 95.6C187.201 97.2 204.401 104.2 204.401 104.2C204.801 108.001 201.801 113.001 201.801 113.001C202.201 117.001 200.001 120.401 200.001 120.401C198.801 128.601 198.201 129.401 198.201 129.401C194.001 129.601 186.601 143.401 186.601 143.401C184.801 146.001 174.601 158.001 174.601 158.001C172.601 165.001 154.601 157.801 154.601 157.801C148.001 161.201 150.001 157.801 150.001 157.801C149.601 155.601 154.401 149.601 154.401 149.601C161.401 147.001 158.801 136.201 158.801 136.201C162.801 134.801 151.601 132.001 151.801 130.801C152.001 129.601 157.801 128.201 157.801 128.201C165.801 126.201 161.401 123.801 161.401 123.801C160.801 119.801 163.801 114.201 163.801 114.201C175.401 113.401 163.801 97.2 163.801 97.2C153.001 89.6 152.001 83.8 152.001 83.8C164.601 75.6 156.401 63.2 156.601 59.6C156.801 56 158.001 34.4 158.001 34.4C156.001 28.2 153.001 14.6 153.001 14.6C155.201 9.4 162.601 -3.2 162.601 -3.2C165.401 -7.4 174.201 -12.2 172.001 -15.2C169.801 -18.2 162.001 -16.4 162.001 -16.4C154.201 -17.8 154.801 -12.6 154.801 -12.6C153.201 -11.6 152.401 -6.6 152.401 -6.6C151.68 1.333 142.801 7.6 142.801 7.6C131.601 13.8 140.801 17.8 140.801 17.8C146.801 24.4 137.001 24.6 137.001 24.6C126.001 22.8 134.201 33 134.201 33C145.001 45.8 142.001 48.6 142.001 48.6
													C131.801 49.6 144.401 58.8 144.401 58.8C144.401 58.8 143.601 56.8 143.801 58.6C144.001 60.4 147.001 64.6 147.801 66.6C148.601 68.6 144.601 68.8 144.601 68.8C145.201 78.4 129.801 74.2 129.801 74.2C129.801 74.2 129.801 74.2 128.201 74.4C126.601 74.6 115.401 73.8 109.601 71.6C103.801 69.4 97.001 69.4 97.001 69.4C97.001 69.4 93.001 71.2 85.4 71C77.8 70.8 69.8 73.6 69.8 73.6C65.4 73.2 74 68.8 74.2 69C74.4 69.2 80 63.6 72 64.2C50.203 65.835 39.4 55.6 39.4 55.6C37.4 54.2 34.8 51.4 34.8 51.4C24.8 49.4 36.2 63.8 36.2 63.8C37.4 65.2 36 66.2 36 66.2C35.2 64.6 27.4 59.2 27.4 59.2C24.589 58.227 23.226 56.893 20.895 54.407z" ) );
b = new SolidBrush( Color.FromArgb(0x4c,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-3 42.8C-3 42.8 8.6 48.4 11.2 51.2C13.8 54 27.8 65.4 27.8 65.4C27.8 65.4 22.4 63.4 19.8 61.6C17.2 59.8 6.4 51.6 6.4 51.6C6.4 51.6 2.6 45.6 -3 42.8z" ) );
b = new SolidBrush( Color.FromArgb(0x99,0xcc,0x32) );
tigerData.Add( new TigerPath( p, b, @"M-61.009 11.603C-60.672 11.455 -61.196 8.743 -61.4 8.2C-62.422 5.474 -71.4 4 -71.4 4C-71.627 5.365 -71.682 6.961 -71.576 8.599C-71.576 8.599 -66.708 14.118 -61.009 11.603z" ) );
b = new SolidBrush( Color.FromArgb(0x65,0x99,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-61.009 11.403C-61.458 11.561 -61.024 8.669 -61.2 8.2C-62.222 5.474 -71.4 3.9 -71.4 3.9C-71.627 5.265 -71.682 6.861 -71.576 8.499C-71.576 8.499 -67.308 13.618 -61.009 11.403z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-65.4 11.546C-66.025 11.546 -66.531 10.406 -66.531 9C-66.531 7.595 -66.025 6.455 -65.4 6.455C-64.775 6.455 -64.268 7.595 -64.268 9C-64.268 10.406 -64.775 11.546 -65.4 11.546z" ) );
tigerData.Add( new TigerPath( p, b, @"M-65.4 9z" ) );
tigerData.Add( new TigerPath( p, b, @"M-111 109.601C-111 109.601 -116.6 119.601 -91.8 113.601C-91.8 113.601 -77.8 112.401 -75.4 110.001C-74.2 110.801 -65.834 113.734 -63 114.401C-56.2 116.001 -47.8 106 -47.8 106C-47.8 106 -43.2 95.5 -40.4 95.5C-37.6 95.5 -40.8 97.1 -40.8 97.1C-40.8 97.1 -47.4 107.201 -47 108.801C-47 108.801 -52.2 128.801 -68.2 129.601C-68.2 129.601 -84.35 130.551 -83 136.401C-83 136.401 -74.2 134.001 -71.8 136.401C-71.8 136.401 -61 136.001 -69 142.401L-75.8 154.001C-75.8 154.001 -75.66 157.919 -85.8 154.401C-95.6 151.001 -105.9 138.101 -105.9 138.101C-105.9 138.101 -121.85 123.551 -111 109.601z" ) );
b = new SolidBrush( Color.FromArgb(0xe5,0x99,0x99) );
tigerData.Add( new TigerPath( p, b, @"M-112.2 113.601C-112.2 113.601 -114.2 123.201 -77.4 112.801C-77.4 112.801 -73 112.801 -70.6 113.601C-68.2 114.401 -56.2 117.201 -54.2 116.001C-54.2 116.001 -61.4 129.601 -73 128.001C-73 128.001 -86.2 129.601 -85.8 134.401C-85.8 134.401 -81.8 141.601 -77 144.001C-77 144.001 -74.2 146.401 -74.6 149.601C-75 152.801 -77.8 154.401 -79.8 155.201C-81.8 156.001 -85 152.801 -86.6 152.801C-88.2 152.801 -96.6 146.401 -101 141.601C-105.4 136.801 -113.8 124.801 -113.4 122.001C-113 119.201 -112.2 113.601 -112.2 113.601z" ) );
b = new SolidBrush( Color.FromArgb(0xb2,0x65,0x65) );
tigerData.Add( new TigerPath( p, b, @"M-109 131.051C-106.4 135.001 -103.2 139.201 -101 141.601C-96.6 146.401 -88.2 152.801 -86.6 152.801C-85 152.801 -81.8 156.001 -79.8 155.201C-77.8 154.401 -75 152.801 -74.6 149.601C-74.2 146.401 -77 144.001 -77 144.001C-80.066 142.468 -82.806 138.976 -84.385 136.653C-84.385 136.653 -84.2 139.201 -89.4 138.401C-94.6 137.601 -99.8 134.801 -101.4 131.601C-103 128.401 -105.4 126.001 -103.8 129.601C-102.2 133.201 -99.8 136.801 -98.2 137.201C-96.6 137.601 -97 138.801 -99.4 138.401C-101.8 138.001 -104.6 137.601 -109 132.401z" ) );
b = new SolidBrush( Color.FromArgb(0x99,0x26,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-111.6 110.001C-111.6 110.001 -109.8 96.4 -108.6 92.4C-108.6 92.4 -109.4 85.6 -107 81.4C-104.6 77.2 -102.6 71 -99.6 65.6C-96.6 60.2 -96.4 56.2 -92.4 54.6C-88.4 53 -82.4 44.4 -79.6 43.4C-76.8 42.4 -77 43.2 -77 43.2C-77 43.2 -70.2 28.4 -56.6 32.4C-56.6 32.4 -72.8 29.6 -57 20.2C-57 20.2 -61.8 21.3 -58.5 14.3C-56.299 9.632 -56.8 16.4 -67.8 28.2C-67.8 28.2 -72.8 36.8 -78 39.8C-83.2 42.8 -95.2 49.8 -96.4 53.6C-97.6 57.4 -100.8 63.2 -102.8 64.8C-104.8 66.4 -107.6 70.6 -108 74C-108 74 -109.2 78 -110.6 79.2C-112 80.4 -112.2 83.6 -112.2 85.6C-112.2 87.6 -114.2 90.4 -114 92.8C-114 92.8 -113.2 111.801 -113.6 113.801L-111.6 110.001z" ) );
b = new SolidBrush( Color.FromArgb(0xff,0xff,0xff) );
tigerData.Add( new TigerPath( p, b, @"M-120.2 114.601C-120.2 114.601 -122.2 113.201 -126.6 119.201C-126.6 119.201 -119.3 152.201 -119.3 153.601C-119.3 153.601 -118.2 151.501 -119.5 144.301C-120.8 137.101 -121.7 124.401 -121.7 124.401L-120.2 114.601z" ) );
b = new SolidBrush( Color.FromArgb(0x99,0x26,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-98.6 54C-98.6 54 -116.2 57.2 -115.8 86.4L-116.6 111.201C-116.6 111.201 -117.8 85.6 -119 84C-120.2 82.4 -116.2 71.2 -119.4 77.2C-119.4 77.2 -133.4 91.2 -125.4 112.401C-125.4 112.401 -123.9 115.701 -126.9 111.101C-126.9 111.101 -131.5 98.5 -130.4 92.1C-130.4 92.1 -130.2 89.9 -128.3 87.1C-128.3 87.1 -119.7 75.4 -117 73.1C-117 73.1 -115.2 58.7 -99.8 53.5C-99.8 53.5 -94.1 51.2 -98.6 54z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M40.8 -12.2C41.46 -12.554 41.451 -13.524 42.031 -13.697C43.18 -14.041 43.344 -15.108 43.862 -15.892C44.735 -17.211 44.928 -18.744 45.51 -20.235C45.782 -20.935 45.809 -21.89 45.496 -22.55C44.322 -25.031 43.62 -27.48 42.178 -29.906C41.91 -30.356 41.648 -31.15 41.447 -31.748C40.984 -33.132 39.727 -34.123 38.867 -35.443C38.579 -35.884 39.104 -36.809 38.388 -36.893C37.491 -36.998 36.042 -37.578 35.809 -36.552C35.221 -33.965 36.232 -31.442 37.2 -29C36.418 -28.308 36.752 -27.387 36.904 -26.62C37.614 -23.014 36.416 -19.662 35.655 -16.188C35.632 -16.084 35.974 -15.886 35.946 -15.824C34.724 -13.138 33.272 -10.693 31.453 -8.312C30.695 -7.32 29.823 -6.404 29.326 -5.341C28.958 -4.554 28.55 -3.588 28.8 -2.6C25.365 0.18 23.115 4.025 20.504 7.871C20.042 8.551 20.333 9.76 20.884 10.029C21.697 10.427 22.653 9.403 23.123 8.557C23.512 7.859 23.865 7.209 24.356 6.566C24.489 6.391 24.31 5.972 24.445 5.851C27.078 3.504 28.747 0.568 31.2 -1.8C33.15 -2.129 34.687 -3.127 36.435 -4.14C36.743 -4.319 37.267 -4.07 37.557 -4.265C39.31 -5.442 39.308 -7.478 39.414 -9.388C39.464 -10.272 39.66 -11.589 40.8 -12.2z" ) );
tigerData.Add( new TigerPath( p, b, @"M31.959 -16.666C32.083 -16.743 31.928 -17.166 32.037 -17.382C32.199 -17.706 32.602 -17.894 32.764 -18.218C32.873 -18.434 32.71 -18.814 32.846 -18.956C35.179 -21.403 35.436 -24.427 34.4 -27.4C35.424 -28.02 35.485 -29.282 35.06 -30.129C34.207 -31.829 34.014 -33.755 33.039 -35.298C32.237 -36.567 30.659 -37.811 29.288 -36.508C28.867 -36.108 28.546 -35.321 28.824 -34.609C28.888 -34.446 29.173 -34.3 29.146 -34.218C29.039 -33.894 28.493 -33.67 28.487 -33.398C28.457 -31.902 27.503 -30.391 28.133 -29.062C28.905 -27.433 29.724 -25.576 30.4 -23.8C29.166 -21.684 30.199 -19.235 28.446 -17.358C28.31 -17.212 28.319 -16.826 28.441 -16.624C28.733 -16.138 29.139 -15.732 29.625 -15.44C29.827 -15.319 30.175 -15.317 30.375 -15.441C30.953 -15.803 31.351 -16.29 31.959 -16.666z" ) );
tigerData.Add( new TigerPath( p, b, @"M94.771 -26.977C96.16 -25.185 96.45 -22.39 94.401 -21C94.951 -17.691 98.302 -19.67 100.401 -20.2C100.292 -20.588 100.519 -20.932 100.802 -20.937C101.859 -20.952 102.539 -21.984 103.601 -21.8C104.035 -23.357 105.673 -24.059 106.317 -25.439C108.043 -29.134 107.452 -33.407 104.868 -36.653C104.666 -36.907 104.883 -37.424 104.759 -37.786C104.003 -39.997 101.935 -40.312 100.001 -41C98.824 -44.875 98.163 -48.906 96.401 -52.6C94.787 -52.85 94.089 -54.589 92.752 -55.309C91.419 -56.028 90.851 -54.449 90.892 -53.403C90.899 -53.198 91.351 -52.974 91.181 -52.609C91.105 -52.445 90.845 -52.334 90.845 -52.2C90.846 -52.065 91.067 -51.934 91.201 -51.8C90.283 -50.98 88.86 -50.503 88.565 -49.358C87.611 -45.648 90.184 -42.523 91.852 -39.322C92.443 -38.187 91.707 -36.916 90.947 -35.708C90.509 -35.013 90.617 -33.886 90.893 -33.03C91.645 -30.699 93.236 -28.96 94.771 -26.977z" ) );
tigerData.Add( new TigerPath( p, b, @"M57.611 -8.591C56.124 -6.74 52.712 -4.171 55.629 -2.243C55.823 -2.114 56.193 -2.11 56.366 -2.244C58.387 -3.809 60.39 -4.712 62.826 -5.294C62.95 -5.323 63.224 -4.856 63.593 -5.017C65.206 -5.72 67.216 -5.662 68.4 -7C72.167 -6.776 75.732 -7.892 79.123 -9.2C80.284 -9.648 81.554 -10.207 82.755 -10.709C84.131 -11.285 85.335 -12.213 86.447 -13.354C86.58 -13.49 86.934 -13.4 87.201 -13.4C87.161 -14.263 88.123 -14.39 88.37 -15.012C88.462 -15.244 88.312 -15.64 88.445 -15.742C90.583 -17.372 91.503 -19.39 90.334 -21.767C90.049 -22.345 89.8 -22.963 89.234 -23.439C88.149 -24.35 87.047 -23.496 86 -23.8C85.841 -23.172 85.112 -23.344 84.726 -23.146C83.867 -22.707 82.534 -23.292 81.675 -22.854C80.313 -22.159 79.072 -21.99 77.65 -21.613C77.338 -21.531 76.56 -21.627 76.4 -21C76.266 -21.134 76.118 -21.368 76.012 -21.346C74.104 -20.95 72.844 -20.736 71.543 -19.044C71.44 -18.911 70.998 -19.09 70.839 -18.955C69.882 -18.147 69.477 -16.913 68.376 -16.241C68.175 -16.118 67.823 -16.286 67.629 -16.157C66.983 -15.726 66.616 -15.085 65.974 -14.638C65.645 -14.409 65.245 -14.734 65.277 -14.99C65.522 -16.937 66.175 -18.724 65.6 -20.6C67.677 -23.12 70.194 -25.069 72 -27.8C72.015 -29.966 72.707 -32.112 72.594 -34.189C72.584 -34.382 72.296 -35.115 72.17 -35.462C71.858 -36.316 72.764 -37.382 71.92 -38.106C70.516 -39.309 69.224 -38.433 68.4 -37C66.562 -36.61 64.496 -35.917 62.918 -37.151C61.911 -37.938 61.333 -38.844 60.534 -39.9C59.549 -41.202 59.884 -42.638 59.954 -44.202C59.96 -44.33 59.645 -44.466 59.645 -44.6C59.646 -44.735 59.866 -44.866 60 -45C59.294 -45.626 59.019 -46.684 58 -47C58.305 -48.092 57.629 -48.976 56.758 -49.278C54.763 -49.969 53.086 -48.057 51.194 -47.984C50.68 -47.965 50.213 -49.003 49.564 -49.328C49.132 -49.544 48.428 -49.577 48.066 -49.311C47.378 -48.807 46.789 -48.693 46.031 -48.488C44.414 -48.052 43.136 -46.958 41.656 -46.103C40.171 -45.246 39.216 -43.809 38.136 -42.489C37.195 -41.337 37.059 -38.923 38.479 -38.423C40.322 -37.773 41.626 -40.476 43.592 -40.15C43.904 -40.099 44.11 -39.788 44 -39
													C44.389 -39.291 44.607 -39.52 44.8 -39.8C45.658 -38.781 46.822 -38.444 47.76 -37.571C48.73 -36.667 50.476 -37.085 51.491 -36.088C53.02 -34.586 52.461 -31.905 54.4 -30.6C53.814 -29.287 53.207 -28.01 52.872 -26.583C52.59 -25.377 53.584 -24.18 54.795 -24.271C56.053 -24.365 56.315 -25.124 56.8 -26.2C57.067 -25.933 57.536 -25.636 57.495 -25.42C57.038 -23.033 56.011 -21.04 55.553 -18.609C55.494 -18.292 55.189 -18.09 54.8 -18.2C54.332 -14.051 50.28 -11.657 47.735 -8.492C47.332 -7.99 47.328 -6.741 47.737 -6.338C49.14 -4.951 51.1 -6.497 52.8 -7C53.013 -8.206 53.872 -9.148 55.204 -9.092C55.46 -9.082 55.695 -9.624 56.019 -9.754C56.367 -9.892 56.869 -9.668 57.155 -9.866C58.884 -11.061 60.292 -12.167 62.03 -13.356C62.222 -13.487 62.566 -13.328 62.782 -13.436C63.107 -13.598 63.294 -13.985 63.617 -14.17C63.965 -14.37 64.207 -14.08 64.4 -13.8C63.754 -13.451 63.75 -12.494 63.168 -12.292C62.393 -12.024 61.832 -11.511 61.158 -11.064C60.866 -10.871 60.207 -11.119 60.103 -10.94C59.505 -9.912 58.321 -9.474 57.611 -8.591z" ) );
tigerData.Add( new TigerPath( p, b, @"M2.2 -58C2.2 -58 -7.038 -60.872 -18.2 -35.2C-18.2 -35.2 -20.6 -30 -23 -28C-25.4 -26 -36.6 -22.4 -38.6 -18.4L-49 -2.4C-49 -2.4 -34.2 -18.4 -31 -20.8C-31 -20.8 -23 -29.2 -26.2 -22.4C-26.2 -22.4 -40.2 -11.6 -39 -2.4C-39 -2.4 -44.6 12 -45.4 14C-45.4 14 -29.4 -18 -27 -19.2C-24.6 -20.4 -23.4 -20.4 -24.6 -16.8C-25.8 -13.2 -26.2 3.2 -29 5.2C-29 5.2 -21 -15.2 -21.8 -18.4C-21.8 -18.4 -18.6 -22 -16.2 -16.8L-17.4 -0.8L-13 11.2C-13 11.2 -15.4 0 -13.8 -15.6C-13.8 -15.6 -15.8 -26 -11.8 -20.4C-7.8 -14.8 1.8 -8.8 1.8 -4C1.8 -4 -3.4 -21.6 -12.6 -26.4L-16.6 -20.4L-17.8 -22.4C-17.8 -22.4 -21.4 -23.2 -17 -30C-12.6 -36.8 -13 -37.6 -13 -37.6C-13 -37.6 -6.6 -30.4 -5 -30.4C-5 -30.4 8.2 -38 9.4 -13.6C9.4 -13.6 16.2 -28 7 -34.8C7 -34.8 -7.8 -36.8 -6.6 -42L0.6 -54.4C4.2 -59.6 2.6 -56.8 2.6 -56.8z" ) );
tigerData.Add( new TigerPath( p, b, @"M-17.8 -41.6C-17.8 -41.6 -30.6 -41.6 -33.8 -36.4L-41 -26.8C-41 -26.8 -23.8 -36.8 -19.8 -38C-15.8 -39.2 -17.8 -41.6 -17.8 -41.6z" ) );
tigerData.Add( new TigerPath( p, b, @"M-57.8 -35.2C-57.8 -35.2 -59.8 -34 -60.2 -31.2C-60.6 -28.4 -63 -28 -62.2 -25.2C-61.4 -22.4 -59.4 -20 -59.4 -24C-59.4 -28 -57.8 -30 -57 -31.2C-56.2 -32.4 -54.6 -36.8 -57.8 -35.2z" ) );
tigerData.Add( new TigerPath( p, b, @"M-66.6 26C-66.6 26 -75 22 -78.2 18.4C-81.4 14.8 -80.948 19.966 -85.8 19.6C-91.647 19.159 -90.6 3.2 -90.6 3.2L-94.6 10.8C-94.6 10.8 -95.8 25.2 -87.8 22.8C-83.893 21.628 -82.6 23.2 -84.2 24C-85.8 24.8 -78.6 25.2 -81.4 26.8C-84.2 28.4 -69.8 23.2 -72.2 33.6L-66.6 26z" ) );
tigerData.Add( new TigerPath( p, b, @"M-79.2 40.4C-79.2 40.4 -94.6 44.8 -98.2 35.2C-98.2 35.2 -103 37.6 -100.8 40.6C-98.6 43.6 -97.4 44 -97.4 44C-97.4 44 -92 45.2 -92.6 46C-93.2 46.8 -95.6 50.2 -95.6 50.2C-95.6 50.2 -85.4 44.2 -79.2 40.4z" ) );
b = new SolidBrush( Color.FromArgb(0xff,0xff,0xff) );
tigerData.Add( new TigerPath( p, b, @"M149.201 118.601C148.774 120.735 147.103 121.536 145.201 122.201C143.284 121.243 140.686 118.137 138.801 120.201C138.327 119.721 137.548 119.661 137.204 118.999C136.739 118.101 137.011 117.055 136.669 116.257C136.124 114.985 135.415 113.619 135.601 112.201C137.407 111.489 138.002 109.583 137.528 107.82C137.459 107.563 137.03 107.366 137.23 107.017C137.416 106.694 137.734 106.467 138.001 106.2C137.866 106.335 137.721 106.568 137.61 106.548C137 106.442 137.124 105.805 137.254 105.418C137.839 103.672 139.853 103.408 141.201 104.6C141.457 104.035 141.966 104.229 142.401 104.2C142.351 103.621 142.759 103.094 142.957 102.674C143.475 101.576 145.104 102.682 145.901 102.07C146.977 101.245 148.04 100.546 149.118 101.149C150.927 102.162 152.636 103.374 153.835 105.115C154.41 105.949 154.65 107.23 154.592 108.188C154.554 108.835 153.173 108.483 152.83 109.412C152.185 111.16 154.016 111.679 154.772 113.017C154.97 113.366 154.706 113.67 154.391 113.768C153.98 113.896 153.196 113.707 153.334 114.16C154.306 117.353 151.55 118.031 149.201 118.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M139.6 138.201C139.593 136.463 137.992 134.707 139.201 133.001C139.336 133.135 139.467 133.356 139.601 133.356C139.736 133.356 139.867 133.135 140.001 133.001C141.496 135.217 145.148 136.145 145.006 138.991C144.984 139.438 143.897 140.356 144.801 141.001C142.988 142.349 142.933 144.719 142.001 146.601C140.763 146.315 139.551 145.952 138.401 145.401C138.753 143.915 138.636 142.231 139.456 140.911C139.89 140.213 139.603 139.134 139.6 138.201z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-26.6 129.201C-26.6 129.201 -43.458 139.337 -29.4 124.001C-20.6 114.401 -10.6 108.801 -10.6 108.801C-10.6 108.801 -0.2 104.4 3.4 103.2C7 102 22.2 96.8 25.4 96.4C28.6 96 38.2 92 45 96C51.8 100 59.8 104.4 59.8 104.4C59.8 104.4 43.4 96 39.8 98.4C36.2 100.8 29 100.4 23 103.6C23 103.6 8.2 108.001 5 110.001C1.8 112.001 -8.6 123.601 -10.2 122.801C-11.8 122.001 -9.8 121.601 -8.6 118.801C-7.4 116.001 -9.4 114.401 -17.4 120.801C-25.4 127.201 -26.6 129.201 -26.6 129.201z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-19.195 123.234C-19.195 123.234 -17.785 110.194 -9.307 111.859C-9.307 111.859 -1.081 107.689 1.641 105.721C1.641 105.721 9.78 104.019 11.09 103.402C29.569 94.702 44.288 99.221 44.835 98.101C45.381 96.982 65.006 104.099 68.615 108.185C69.006 108.628 58.384 102.588 48.686 100.697C40.413 99.083 18.811 100.944 7.905 106.48C4.932 107.989 -4.013 113.773 -6.544 113.662C-9.075 113.55 -19.195 123.234 -19.195 123.234z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-23 148.801C-23 148.801 -38.2 146.401 -21.4 144.801C-21.4 144.801 -3.4 142.801 0.6 137.601C0.6 137.601 14.2 128.401 17 128.001C19.8 127.601 49.8 120.401 50.2 118.001C50.6 115.601 56.2 115.601 57.8 116.401C59.4 117.201 58.6 118.401 55.8 119.201C53 120.001 21.8 136.401 15.4 137.601C9 138.801 -2.6 146.401 -7.4 147.601C-12.2 148.801 -23 148.801 -23 148.801z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-3.48 141.403C-3.48 141.403 -12.062 140.574 -3.461 139.755C-3.461 139.755 5.355 136.331 7.403 133.668C7.403 133.668 14.367 128.957 15.8 128.753C17.234 128.548 31.194 124.861 31.399 123.633C31.604 122.404 65.67 109.823 70.09 113.013C73.001 115.114 63.1 113.437 53.466 117.847C52.111 118.467 18.258 133.054 14.981 133.668C11.704 134.283 5.765 138.174 3.307 138.788C0.85 139.403 -3.48 141.403 -3.48 141.403z" ) );
tigerData.Add( new TigerPath( p, b, @"M-11.4 143.601C-11.4 143.601 -6.2 143.201 -7.4 144.801C-8.6 146.401 -11 145.601 -11 145.601L-11.4 143.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M-18.6 145.201C-18.6 145.201 -13.4 144.801 -14.6 146.401C-15.8 148.001 -18.2 147.201 -18.2 147.201L-18.6 145.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M-29 146.801C-29 146.801 -23.8 146.401 -25 148.001C-26.2 149.601 -28.6 148.801 -28.6 148.801L-29 146.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M-36.6 147.601C-36.6 147.601 -31.4 147.201 -32.6 148.801C-33.8 150.401 -36.2 149.601 -36.2 149.601L-36.6 147.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M1.8 108.001C1.8 108.001 6.2 108.001 5 109.601C3.8 111.201 0.6 110.801 0.6 110.801L1.8 108.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-8.2 113.601C-8.2 113.601 -1.694 111.46 -4.2 114.801C-5.4 116.401 -7.8 115.601 -7.8 115.601L-8.2 113.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M-19.4 118.401C-19.4 118.401 -14.2 118.001 -15.4 119.601C-16.6 121.201 -19 120.401 -19 120.401L-19.4 118.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M-27 124.401C-27 124.401 -21.8 124.001 -23 125.601C-24.2 127.201 -26.6 126.401 -26.6 126.401L-27 124.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M-33.8 129.201C-33.8 129.201 -28.6 128.801 -29.8 130.401C-31 132.001 -33.4 131.201 -33.4 131.201L-33.8 129.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M5.282 135.598C5.282 135.598 12.203 135.066 10.606 137.195C9.009 139.325 5.814 138.26 5.814 138.26L5.282 135.598z" ) );
tigerData.Add( new TigerPath( p, b, @"M15.682 130.798C15.682 130.798 22.603 130.266 21.006 132.395C19.409 134.525 16.214 133.46 16.214 133.46L15.682 130.798z" ) );
tigerData.Add( new TigerPath( p, b, @"M26.482 126.398C26.482 126.398 33.403 125.866 31.806 127.995C30.209 130.125 27.014 129.06 27.014 129.06L26.482 126.398z" ) );
tigerData.Add( new TigerPath( p, b, @"M36.882 121.598C36.882 121.598 43.803 121.066 42.206 123.195C40.609 125.325 37.414 124.26 37.414 124.26L36.882 121.598z" ) );
tigerData.Add( new TigerPath( p, b, @"M9.282 103.598C9.282 103.598 16.203 103.066 14.606 105.195C13.009 107.325 9.014 107.06 9.014 107.06L9.282 103.598z" ) );
tigerData.Add( new TigerPath( p, b, @"M19.282 100.398C19.282 100.398 26.203 99.866 24.606 101.995C23.009 104.125 18.614 103.86 18.614 103.86L19.282 100.398z" ) );
tigerData.Add( new TigerPath( p, b, @"M-3.4 140.401C-3.4 140.401 1.8 140.001 0.6 141.601C-0.6 143.201 -3 142.401 -3 142.401L-3.4 140.401z" ) );
b = new SolidBrush( Color.FromArgb(0x99,0x26,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-76.6 41.2C-76.6 41.2 -81 50 -81.4 53.2C-81.4 53.2 -80.6 44.4 -79.4 42.4C-78.2 40.4 -76.6 41.2 -76.6 41.2z" ) );
tigerData.Add( new TigerPath( p, b, @"M-95 55.2C-95 55.2 -98.2 69.6 -97.8 72.4C-97.8 72.4 -99 60.8 -98.6 59.6C-98.2 58.4 -95 55.2 -95 55.2z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-74.2 -19.4L-74.4 -16.2L-76.6 -16C-76.6 -16 -62.4 -3.4 -61.8 4.2C-61.8 4.2 -61 -4 -74.2 -19.4z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-70.216 -18.135C-70.647 -18.551 -70.428 -19.296 -70.836 -19.556C-71.645 -20.072 -69.538 -20.129 -69.766 -20.845C-70.149 -22.051 -69.962 -22.072 -70.084 -23.348C-70.141 -23.946 -69.553 -25.486 -69.168 -25.926C-67.722 -27.578 -69.046 -30.51 -67.406 -32.061C-67.102 -32.35 -66.726 -32.902 -66.441 -33.32C-65.782 -34.283 -64.598 -34.771 -63.648 -35.599C-63.33 -35.875 -63.531 -36.702 -62.962 -36.61C-62.248 -36.495 -61.007 -36.625 -61.052 -35.784C-61.165 -33.664 -62.494 -31.944 -63.774 -30.276C-63.323 -29.572 -63.781 -28.937 -64.065 -28.38C-65.4 -25.76 -65.211 -22.919 -65.385 -20.079C-65.39 -19.994 -65.697 -19.916 -65.689 -19.863C-65.336 -17.528 -64.752 -15.329 -63.873 -13.1C-63.507 -12.17 -63.036 -11.275 -62.886 -10.348C-62.775 -9.662 -62.672 -8.829 -63.08 -8.124C-61.045 -5.234 -62.354 -2.583 -61.185 0.948C-60.978 1.573 -59.286 3.487 -59.749 3.326C-62.262 2.455 -62.374 2.057 -62.551 1.304C-62.697 0.681 -63.027 -0.696 -63.264 -1.298C-63.328 -1.462 -63.499 -3.346 -63.577 -3.468C-65.09 -5.85 -63.732 -5.674 -65.102 -8.032C-66.53 -8.712 -67.496 -9.816 -68.619 -10.978C-68.817 -11.182 -67.674 -11.906 -67.855 -12.119C-68.947 -13.408 -70.1 -14.175 -69.764 -15.668C-69.609 -16.358 -69.472 -17.415 -70.216 -18.135z" ) );
tigerData.Add( new TigerPath( p, b, @"M-73.8 -16.4C-73.8 -16.4 -73.4 -9.6 -71 -8C-68.6 -6.4 -69.8 -7.2 -73 -8.4C-76.2 -9.6 -75 -10.4 -75 -10.4C-75 -10.4 -77.8 -10 -75.4 -8C-73 -6 -69.4 -3.6 -71 -3.6C-72.6 -3.6 -80.2 -7.6 -80.2 -10.4C-80.2 -13.2 -81.2 -17.3 -81.2 -17.3C-81.2 -17.3 -80.1 -18.1 -75.3 -18C-75.3 -18 -73.9 -17.3 -73.8 -16.4z" ) );
p = new Pen( Color.FromArgb(0x00,0x00,0x00), 0.1f );
b = new SolidBrush( Color.FromArgb(0xFF,0xFF,0xFF) );
tigerData.Add( new TigerPath( p, b, @"M-74.6 2.2C-74.6 2.2 -83.12 -0.591 -101.6 2.8C-101.6 2.8 -92.569 0.722 -73.8 3C-63.5 4.25 -74.6 2.2 -74.6 2.2z" ) );
tigerData.Add( new TigerPath( p, b, @"M-72.502 2.129C-72.502 2.129 -80.748 -1.389 -99.453 0.392C-99.453 0.392 -90.275 -0.897 -71.774 2.995C-61.62 5.131 -72.502 2.129 -72.502 2.129z" ) );
tigerData.Add( new TigerPath( p, b, @"M-70.714 2.222C-70.714 2.222 -78.676 -1.899 -97.461 -1.514C-97.461 -1.514 -88.213 -2.118 -70.052 3.14C-60.086 6.025 -70.714 2.222 -70.714 2.222z" ) );
tigerData.Add( new TigerPath( p, b, @"M-69.444 2.445C-69.444 2.445 -76.268 -1.862 -93.142 -2.96C-93.142 -2.96 -84.803 -2.79 -68.922 3.319C-60.206 6.672 -69.444 2.445 -69.444 2.445z" ) );
tigerData.Add( new TigerPath( p, b, @"M45.84 12.961C45.84 12.961 44.91 13.605 45.124 12.424C45.339 11.243 73.547 -1.927 77.161 -1.677C77.161 -1.677 46.913 11.529 45.84 12.961z" ) );
tigerData.Add( new TigerPath( p, b, @"M42.446 13.6C42.446 13.6 41.57 14.315 41.691 13.121C41.812 11.927 68.899 -3.418 72.521 -3.452C72.521 -3.452 43.404 12.089 42.446 13.6z" ) );
tigerData.Add( new TigerPath( p, b, @"M39.16 14.975C39.16 14.975 38.332 15.747 38.374 14.547C38.416 13.348 58.233 -2.149 68.045 -4.023C68.045 -4.023 50.015 4.104 39.16 14.975z" ) );
tigerData.Add( new TigerPath( p, b, @"M36.284 16.838C36.284 16.838 35.539 17.532 35.577 16.453C35.615 15.373 53.449 1.426 62.28 -0.26C62.28 -0.26 46.054 7.054 36.284 16.838z" ) );
p = null; // new Pen( Color.FromArgb(0x00,0x00,0x00) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M4.6 164.801C4.6 164.801 -10.6 162.401 6.2 160.801C6.2 160.801 24.2 158.801 28.2 153.601C28.2 153.601 41.8 144.401 44.6 144.001C47.4 143.601 63.8 140.001 64.2 137.601C64.6 135.201 70.6 132.801 72.2 133.601C73.8 134.401 73.8 143.601 71 144.401C68.2 145.201 49.4 152.401 43 153.601C36.6 154.801 25 162.401 20.2 163.601C15.4 164.801 4.6 164.801 4.6 164.801z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M77.6 127.401C77.6 127.401 74.6 129.001 73.4 131.601C73.4 131.601 67 142.201 52.8 145.401C52.8 145.401 29.8 154.401 22 156.401C22 156.401 8.6 161.401 1.2 160.601C1.2 160.601 -5.8 160.801 0.4 162.401C0.4 162.401 20.6 160.401 24 158.601C24 158.601 39.6 153.401 42.6 150.801C45.6 148.201 63.8 143.201 66 141.201C68.2 139.201 78 130.801 77.6 127.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M18.882 158.911C18.882 158.911 24.111 158.685 22.958 160.234C21.805 161.784 19.357 160.91 19.357 160.91L18.882 158.911z" ) );
tigerData.Add( new TigerPath( p, b, @"M11.68 160.263C11.68 160.263 16.908 160.037 15.756 161.586C14.603 163.136 12.155 162.263 12.155 162.263L11.68 160.263z" ) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M1.251 161.511C1.251 161.511 6.48 161.284 5.327 162.834C4.174 164.383 1.726 163.51 1.726 163.51L1.251 161.511z" ) );
tigerData.Add( new TigerPath( p, b, @"M-6.383 162.055C-6.383 162.055 -1.154 161.829 -2.307 163.378C-3.46 164.928 -5.908 164.054 -5.908 164.054L-6.383 162.055z" ) );
tigerData.Add( new TigerPath( p, b, @"M35.415 151.513C35.415 151.513 42.375 151.212 40.84 153.274C39.306 155.336 36.047 154.174 36.047 154.174L35.415 151.513z" ) );
tigerData.Add( new TigerPath( p, b, @"M45.73 147.088C45.73 147.088 51.689 143.787 51.155 148.849C50.885 151.405 46.362 149.749 46.362 149.749L45.73 147.088z" ) );
tigerData.Add( new TigerPath( p, b, @"M54.862 144.274C54.862 144.274 62.021 140.573 60.287 146.035C59.509 148.485 55.493 146.935 55.493 146.935L54.862 144.274z" ) );
tigerData.Add( new TigerPath( p, b, @"M64.376 139.449C64.376 139.449 68.735 134.548 69.801 141.21C70.207 143.748 65.008 142.11 65.008 142.11L64.376 139.449z" ) );
tigerData.Add( new TigerPath( p, b, @"M26.834 155.997C26.834 155.997 32.062 155.77 30.91 157.32C29.757 158.869 27.308 157.996 27.308 157.996L26.834 155.997z" ) );
p = new Pen( Color.FromArgb(0x00,0x00,0x00), 0.1f );
b = new SolidBrush( Color.FromArgb(0xFF,0xFF,0xFF) );
tigerData.Add( new TigerPath( p, b, @"M62.434 34.603C62.434 34.603 61.708 35.268 61.707 34.197C61.707 33.127 79.191 19.863 88.034 18.479C88.034 18.479 71.935 25.208 62.434 34.603z" ) );
p = new Pen( Color.FromArgb(0x00,0x00,0x00) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M65.4 98.4C65.4 98.4 87.401 120.801 96.601 124.401C96.601 124.401 105.801 135.601 101.801 161.601C101.801 161.601 98.601 169.201 95.401 148.401C95.401 148.401 98.601 123.201 87.401 139.201C87.401 139.201 79 129.301 85.4 129.601C85.4 129.601 88.601 131.601 89.001 130.001C89.401 128.401 81.4 114.801 64.2 100.4C47 86 65.4 98.4 65.4 98.4z" ) );
p = new Pen( Color.FromArgb(0x00,0x00,0x00), 0.1f );
b = new SolidBrush( Color.FromArgb(0xFF,0xFF,0xFF) );
tigerData.Add( new TigerPath( p, b, @"M7 137.201C7 137.201 6.8 135.401 8.6 136.201C10.4 137.001 104.601 143.201 136.201 167.201C136.201 167.201 91.001 144.001 7 137.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M17.4 132.801C17.4 132.801 17.2 131.001 19 131.801C20.8 132.601 157.401 131.601 181.001 164.001C181.001 164.001 159.001 138.801 17.4 132.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M29 128.801C29 128.801 28.8 127.001 30.6 127.801C32.4 128.601 205.801 115.601 229.401 148.001C229.401 148.001 219.801 122.401 29 128.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M39 124.001C39 124.001 38.8 122.201 40.6 123.001C42.4 123.801 164.601 85.2 188.201 117.601C188.201 117.601 174.801 93 39 124.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-19 146.801C-19 146.801 -19.2 145.001 -17.4 145.801C-15.6 146.601 2.2 148.801 4.2 187.601C4.2 187.601 -3 145.601 -19 146.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M-27.8 148.401C-27.8 148.401 -28 146.601 -26.2 147.401C-24.4 148.201 -10.2 143.601 -13 182.401C-13 182.401 -11.8 147.201 -27.8 148.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M-35.8 148.801C-35.8 148.801 -36 147.001 -34.2 147.801C-32.4 148.601 -17 149.201 -29.4 171.601C-29.4 171.601 -19.8 147.601 -35.8 148.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M11.526 104.465C11.526 104.465 11.082 106.464 12.631 105.247C28.699 92.622 61.141 33.72 116.826 28.086C116.826 28.086 78.518 15.976 11.526 104.465z" ) );
tigerData.Add( new TigerPath( p, b, @"M22.726 102.665C22.726 102.665 21.363 101.472 23.231 100.847C25.099 100.222 137.541 27.72 176.826 35.686C176.826 35.686 149.719 28.176 22.726 102.665z" ) );
tigerData.Add( new TigerPath( p, b, @"M1.885 108.767C1.885 108.767 1.376 110.366 3.087 109.39C12.062 104.27 15.677 47.059 59.254 45.804C59.254 45.804 26.843 31.09 1.885 108.767z" ) );
tigerData.Add( new TigerPath( p, b, @"M-18.038 119.793C-18.038 119.793 -19.115 121.079 -17.162 120.825C-6.916 119.493 14.489 78.222 58.928 83.301C58.928 83.301 26.962 68.955 -18.038 119.793z" ) );
tigerData.Add( new TigerPath( p, b, @"M-6.8 113.667C-6.8 113.667 -7.611 115.136 -5.742 114.511C4.057 111.237 17.141 66.625 61.729 63.078C61.729 63.078 27.603 55.135 -6.8 113.667z" ) );
tigerData.Add( new TigerPath( p, b, @"M-25.078 124.912C-25.078 124.912 -25.951 125.954 -24.369 125.748C-16.07 124.669 1.268 91.24 37.264 95.354C37.264 95.354 11.371 83.734 -25.078 124.912z" ) );
tigerData.Add( new TigerPath( p, b, @"M-32.677 130.821C-32.677 130.821 -33.682 131.866 -32.091 131.748C-27.923 131.439 2.715 98.36 21.183 113.862C21.183 113.862 9.168 95.139 -32.677 130.821z" ) );
tigerData.Add( new TigerPath( p, b, @"M36.855 98.898C36.855 98.898 35.654 97.543 37.586 97.158C39.518 96.774 160.221 39.061 198.184 51.927C198.184 51.927 172.243 41.053 36.855 98.898z" ) );
tigerData.Add( new TigerPath( p, b, @"M3.4 163.201C3.4 163.201 3.2 161.401 5 162.201C6.8 163.001 22.2 163.601 9.8 186.001C9.8 186.001 19.4 162.001 3.4 163.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M13.8 161.601C13.8 161.601 13.6 159.801 15.4 160.601C17.2 161.401 35 163.601 37 202.401C37 202.401 29.8 160.401 13.8 161.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M20.6 160.001C20.6 160.001 20.4 158.201 22.2 159.001C24 159.801 48.6 163.201 72.2 195.601C72.2 195.601 36.6 158.801 20.6 160.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M28.225 157.972C28.225 157.972 27.788 156.214 29.678 156.768C31.568 157.322 52.002 155.423 90.099 189.599C90.099 189.599 43.924 154.656 28.225 157.972z" ) );
tigerData.Add( new TigerPath( p, b, @"M38.625 153.572C38.625 153.572 38.188 151.814 40.078 152.368C41.968 152.922 76.802 157.423 128.499 192.399C128.499 192.399 54.324 150.256 38.625 153.572z" ) );
tigerData.Add( new TigerPath( p, b, @"M-1.8 142.001C-1.8 142.001 -2 140.201 -0.2 141.001C1.6 141.801 55 144.401 85.4 171.201C85.4 171.201 50.499 146.426 -1.8 142.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-11.8 146.001C-11.8 146.001 -12 144.201 -10.2 145.001C-8.4 145.801 16.2 149.201 39.8 181.601C39.8 181.601 4.2 144.801 -11.8 146.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M49.503 148.962C49.503 148.962 48.938 147.241 50.864 147.655C52.79 148.068 87.86 150.004 141.981 181.098C141.981 181.098 64.317 146.704 49.503 148.962z" ) );
tigerData.Add( new TigerPath( p, b, @"M57.903 146.562C57.903 146.562 57.338 144.841 59.264 145.255C61.19 145.668 96.26 147.604 150.381 178.698C150.381 178.698 73.317 143.904 57.903 146.562z" ) );
tigerData.Add( new TigerPath( p, b, @"M67.503 141.562C67.503 141.562 66.938 139.841 68.864 140.255C70.79 140.668 113.86 145.004 203.582 179.298C203.582 179.298 82.917 138.904 67.503 141.562z" ) );
p = null; // new Pen( Color.FromArgb(0x00,0x00,0x00) );
b = new SolidBrush( Color.FromArgb(0x00,0x00,0x00) );
tigerData.Add( new TigerPath( p, b, @"M-43.8 148.401C-43.8 148.401 -38.6 148.001 -39.8 149.601C-41 151.201 -43.4 150.401 -43.4 150.401L-43.8 148.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M-13 162.401C-13 162.401 -7.8 162.001 -9 163.601C-10.2 165.201 -12.6 164.401 -12.6 164.401L-13 162.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M-21.8 162.001C-21.8 162.001 -16.6 161.601 -17.8 163.201C-19 164.801 -21.4 164.001 -21.4 164.001L-21.8 162.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-117.169 150.182C-117.169 150.182 -112.124 151.505 -113.782 152.624C-115.439 153.744 -117.446 152.202 -117.446 152.202L-117.169 150.182z" ) );
tigerData.Add( new TigerPath( p, b, @"M-115.169 140.582C-115.169 140.582 -110.124 141.905 -111.782 143.024C-113.439 144.144 -115.446 142.602 -115.446 142.602L-115.169 140.582z" ) );
tigerData.Add( new TigerPath( p, b, @"M-122.369 136.182C-122.369 136.182 -117.324 137.505 -118.982 138.624C-120.639 139.744 -122.646 138.202 -122.646 138.202L-122.369 136.182z" ) );
b = new SolidBrush( Color.FromArgb(0xcc,0xcc,0xcc) );
tigerData.Add( new TigerPath( p, b, @"M-42.6 211.201C-42.6 211.201 -44.2 211.201 -48.2 213.201C-50.2 213.201 -61.4 216.801 -67 226.801C-67 226.801 -54.6 217.201 -42.6 211.201z" ) );
tigerData.Add( new TigerPath( p, b, @"M45.116 303.847C45.257 304.105 45.312 304.525 45.604 304.542C46.262 304.582 47.495 304.883 47.37 304.247C46.522 299.941 45.648 295.004 41.515 293.197C40.876 292.918 39.434 293.331 39.36 294.215C39.233 295.739 39.116 297.088 39.425 298.554C39.725 299.975 41.883 299.985 42.8 298.601C43.736 300.273 44.168 302.116 45.116 303.847z" ) );
tigerData.Add( new TigerPath( p, b, @"M34.038 308.581C34.786 309.994 34.659 311.853 36.074 312.416C36.814 312.71 38.664 311.735 38.246 310.661C37.444 308.6 37.056 306.361 35.667 304.55C35.467 304.288 35.707 303.755 35.547 303.427C34.953 302.207 33.808 301.472 32.4 301.801C31.285 304.004 32.433 306.133 33.955 307.842C34.091 307.994 33.925 308.37 34.038 308.581z" ) );
tigerData.Add( new TigerPath( p, b, @"M-5.564 303.391C-5.672 303.014 -5.71 302.551 -5.545 302.23C-5.014 301.197 -4.221 300.075 -4.558 299.053C-4.906 297.997 -6.022 298.179 -6.672 298.748C-7.807 299.742 -7.856 301.568 -8.547 302.927C-8.743 303.313 -8.692 303.886 -9.133 304.277C-9.607 304.698 -10.047 306.222 -9.951 306.793C-9.898 307.106 -10.081 317.014 -9.859 316.751C-9.24 316.018 -6.19 306.284 -6.121 305.392C-6.064 304.661 -5.332 304.196 -5.564 303.391z" ) );
tigerData.Add( new TigerPath( p, b, @"M-31.202 296.599C-28.568 294.1 -25.778 291.139 -26.22 287.427C-26.336 286.451 -28.111 286.978 -28.298 287.824C-29.1 291.449 -31.139 294.11 -33.707 296.502C-35.903 298.549 -37.765 304.893 -38 305.401C-34.303 300.145 -32.046 297.399 -31.202 296.599z" ) );
tigerData.Add( new TigerPath( p, b, @"M-44.776 290.635C-44.253 290.265 -44.555 289.774 -44.338 289.442C-43.385 287.984 -42.084 286.738 -42.066 285C-42.063 284.723 -42.441 284.414 -42.776 284.638C-43.053 284.822 -43.395 284.952 -43.503 285.082C-45.533 287.531 -46.933 290.202 -48.376 293.014C-48.559 293.371 -49.703 297.862 -49.39 297.973C-49.151 298.058 -47.431 293.877 -47.221 293.763C-45.958 293.077 -45.946 291.462 -44.776 290.635z" ) );
tigerData.Add( new TigerPath( p, b, @"M-28.043 310.179C-27.599 309.31 -26.023 308.108 -26.136 307.219C-26.254 306.291 -25.786 304.848 -26.698 305.536C-27.955 306.484 -31.404 307.833 -31.674 313.641C-31.7 314.212 -28.726 311.519 -28.043 310.179z" ) );
tigerData.Add( new TigerPath( p, b, @"M-13.6 293.001C-13.2 292.333 -12.492 292.806 -12.033 292.543C-11.385 292.171 -10.774 291.613 -10.482 290.964C-9.512 288.815 -7.743 286.995 -7.6 284.601C-9.091 283.196 -9.77 285.236 -10.4 286.201C-11.723 284.554 -12.722 286.428 -14.022 286.947C-14.092 286.975 -14.305 286.628 -14.38 286.655C-15.557 287.095 -16.237 288.176 -17.235 288.957C-17.406 289.091 -17.811 288.911 -17.958 289.047C-18.61 289.65 -19.583 289.975 -19.863 290.657C-20.973 293.364 -24.113 295.459 -26 303.001C-25.619 303.91 -21.488 296.359 -21.001 295.661C-20.165 294.465 -20.047 297.322 -18.771 296.656C-18.72 296.629 -18.534 296.867 -18.4 297.001C-18.206 296.721 -17.988 296.492 -17.6 296.601C-17.6 296.201 -17.734 295.645 -17.533 295.486C-16.296 294.509 -16.38 293.441 -15.6 292.201C-15.142 292.99 -14.081 292.271 -13.6 293.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M46.2 347.401C46.2 347.401 53.6 327.001 49.2 315.801C49.2 315.801 60.6 337.401 56 348.601C56 348.601 55.6 338.201 51.6 333.201C51.6 333.201 47.6 346.001 46.2 347.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M31.4 344.801C31.4 344.801 36.8 336.001 28.8 317.601C28.8 317.601 28 338.001 21.2 349.001C21.2 349.001 35.4 328.801 31.4 344.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M21.4 342.801C21.4 342.801 21.2 322.801 21.6 319.801C21.6 319.801 17.8 336.401 7.6 346.001C7.6 346.001 22 334.001 21.4 342.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M11.8 310.801C11.8 310.801 17.8 324.401 7.8 342.801C7.8 342.801 14.2 330.601 9.4 323.601C9.4 323.601 12 320.201 11.8 310.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M-7.4 342.401C-7.4 342.401 -8.4 326.801 -6.6 324.601C-6.6 324.601 -6.4 318.201 -6.8 317.201C-6.8 317.201 -2.8 311.001 -2.6 318.401C-2.6 318.401 -1.2 326.201 1.6 330.801C1.6 330.801 5.2 336.201 5 342.601C5 342.601 -5 312.401 -7.4 342.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M-11 314.801C-11 314.801 -17.6 325.601 -19.4 344.601C-19.4 344.601 -20.8 338.401 -17 324.001C-17 324.001 -12.8 308.601 -11 314.801z" ) );
tigerData.Add( new TigerPath( p, b, @"M-32.8 334.601C-32.8 334.601 -27.8 329.201 -26.4 324.201C-26.4 324.201 -22.8 308.401 -29.2 317.001C-29.2 317.001 -29 325.001 -37.2 332.401C-37.2 332.401 -32.4 330.001 -32.8 334.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M-38.6 329.601C-38.6 329.601 -35.2 312.201 -34.4 311.401C-34.4 311.401 -32.6 308.001 -35.4 311.201C-35.4 311.201 -44.2 330.401 -48.2 337.001C-48.2 337.001 -40.2 327.801 -38.6 329.601z" ) );
tigerData.Add( new TigerPath( p, b, @"M-44.4 313.001C-44.4 313.001 -32.8 290.601 -54.6 316.401C-54.6 316.401 -43.6 306.601 -44.4 313.001z" ) );
tigerData.Add( new TigerPath( p, b, @"M-59.8 298.401C-59.8 298.401 -55 279.601 -52.4 279.801C-52.4 279.801 -44.2 270.801 -50.8 281.401C-50.8 281.401 -56.8 291.001 -56.2 300.801C-56.2 300.801 -56.8 291.201 -59.8 298.401z" ) );
tigerData.Add( new TigerPath( p, b, @"M270.5 287C270.5 287 258.5 277 256 273.5C256 273.5 269.5 292 269.5 299C269.5 299 272 291.5 270.5 287z" ) );
tigerData.Add( new TigerPath( p, b, @"M276 265C276 265 255 250 251.5 242.5C251.5 242.5 278 272 278 276.5C278 276.5 278.5 267.5 276 265z" ) );
tigerData.Add( new TigerPath( p, b, @"M293 111C293 111 281 103 279.5 105C279.5 105 290 111.5 292.5 120C292.5 120 291 111 293 111z" ) );
tigerData.Add( new TigerPath( p, b, @"M301.5 191.5L284 179.5C284 179.5 303 196.5 303.5 200.5L301.5 191.5z" ) );
p = new Pen( Color.FromArgb( 0x00,0x00,0x00 ) );
b = null;
tigerData.Add( new TigerPath( p, b, @"M-89.25 169L-67.25 173.75" ) );
tigerData.Add( new TigerPath( p, b, @"M-39 331C-39 331 -39.5 327.5 -48.5 338" ) );
tigerData.Add( new TigerPath( p, b, @"M-33.5 336C-33.5 336 -31.5 329.5 -38 334" ) );
tigerData.Add( new TigerPath( p, b, @"M20.5 344.5C20.5 344.5 22 333.5 10.5 346.5" ) );

			TimeSpan diff = DateTime.Now - dt;
			Console.WriteLine( "CreateTigerData={0}", (int)diff.TotalMilliseconds);

			return tigerData;
		}

		static ArrayList TheTigerData = null;

		void Tiger( Graphics g ) {

			Matrix m = new Matrix();
			m.Translate(170,150);
			g.Transform = m;
			g.SmoothingMode     = SmoothingMode.AntiAlias;
			g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			g.PixelOffsetMode   = PixelOffsetMode.Half;

			if( null == TheTigerData ) TheTigerData = GetTiger();

			DateTime dt = DateTime.Now;

			foreach( TigerPath t in TheTigerData ) {
				t.Paint( g );
			}

			TimeSpan diff = DateTime.Now - dt;
			Console.WriteLine( "PaintTigerData={0}", (int)diff.TotalMilliseconds);
		}

		private void AddCheckedListBox(Control c)
		{
			CheckedListBox clb = new CheckedListBox();
			clb.Items.Add("Row 1", true);
			clb.Items.Add("Row 2", false);
			clb.Items.Add("Row 3", true);
			clb.Size = new Size(128, 128);
			c.Controls.Add(clb);
		}
	}
}
