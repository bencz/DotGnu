using System;
using System.IO;
using System.Net;
using System.Drawing;
using System.Threading;
using System.Collections;
using System.Net.Sockets;
using System.Windows.Forms;

public class IrdaTest : Form
{
	ListBox lbDevices;
	Label lDevices;
	Button bDiscover;
	TextBox tbLog;
	Button bServer;
	Button bConnect;
	IrDADeviceInfo selectedDevice;
	ArrayList logTail = new ArrayList();
	System.Windows.Forms.Timer timer;

	// Class that holds device info in list box
	public class DevItem
	{
		public IrDADeviceInfo Device;

		public DevItem(IrDADeviceInfo device)
		{
			this.Device = device;
		}

		public override String ToString()
		{
			return String.Format("{0} {1:X8}",
				Device.DeviceName,
				BitConverter.ToUInt32(Device.DeviceID, 0));
		}
	}

	public IrdaTest()
	{
		lDevices = new Label();
		lDevices.Text = "Devices in range";
		lDevices.Bounds = new Rectangle(0, 0, 96, 16);

		lbDevices = new ListBox();
		lbDevices.Bounds = new Rectangle(0, 16, 124, 106);

		bServer = new Button();
		bServer.Text = "Start server";
		bServer.Bounds = new Rectangle(132, 16, 96, 32);
		bServer.Click += new EventHandler(bServer_Click);

		bConnect = new Button();
		bConnect.Text = "Connect";
		bConnect.Bounds = new Rectangle(132, 52, 96, 32);
		bConnect.Click += new EventHandler(bConnect_Click);

		bDiscover = new Button();
		bDiscover.Text = "Discover";
		bDiscover.Bounds = new Rectangle(132, 90, 96, 32);
		bDiscover.Click += new EventHandler(bDiscover_Click);

		tbLog = new TextBox();
		tbLog.Multiline = true;
		tbLog.ScrollBars = ScrollBars.Both;
		tbLog.Bounds = new Rectangle(0, 126, 240, 164);

		this.Text = "IrDA test program";
		this.Size = new Size(240, 320);
		this.Controls.Add(lbDevices);
		this.Controls.Add(lDevices);
		this.Controls.Add(bDiscover);
		this.Controls.Add(tbLog);
		this.Controls.Add(bServer);
		this.Controls.Add(bConnect);

		timer = new System.Windows.Forms.Timer();
		timer.Tick += new EventHandler(timer_Tick);
		timer.Enabled = true;
	}

	void Log(String text)
	{
		Console.WriteLine(text);
		lock(logTail.SyncRoot)
		{
			logTail.Add(text);
		}
	}

	// Function with send/recieve string endless loop on server socket
	void ServerFn()
	{
		Socket socket = null;
		try
		{
			IrDAEndPoint ep = new IrDAEndPoint(
				new byte[]{0, 0, 0, 0}, "Pop");
			socket = new Socket(ep.AddressFamily, SocketType.Stream, 0);
			socket.Bind(ep);
			socket.Listen(int.MaxValue);
			Log("Server started, waiting for client...");

			Socket remote = socket.Accept();
			NetworkStream stream = new NetworkStream(remote, true);
			BinaryWriter bw = new BinaryWriter(stream);
			BinaryReader br = new BinaryReader(stream);
			Log("Client accepted");

			int no = 0;
			while(true)
			{
				Log("Sending bytes");
				try
				{
					bw.Write("hello no. " + (++no) + " from server");
				}
				catch(Exception ex)
				{
					// MSG_NOSIGNAL on irda socket is supported since linux
					// version 2.6.24
					throw new Exception("Error in writing to socket, " +
						"please check if your linux kernel is >= 2.6.24", ex);
				}
				Log("OK");

				Log("Reading bytes");
				Log(br.ReadString());
				Log("");
			}
		}
		catch(Exception ex)
		{
			Log("Error while running server");
			Log(ex.ToString());
		}
		finally
		{
			if(socket != null)
			{
				socket.Close();
			}
		}
	}

	// Function with recieve/send string endless loop on client socket
	void ClientFn()
	{
		Socket socket = null;
		try
		{
			IrDADeviceInfo dev = selectedDevice;
			if(dev == null)
			{
				Log("Device not selected");
				return;
			}

			IrDAEndPoint ep = new IrDAEndPoint
								(new byte[] {0, 0, 0, 0}, "Pop");
			Socket socket = new Socket
								(AddressFamily.Irda, SocketType.Stream, 0);
			IrDAEndPoint ep2 = new IrDAEndPoint(dev.DeviceID, ep.ServiceName);
			socket.Connect(ep2);

			NetworkStream stream = new NetworkStream(socket, true);
			BinaryWriter bw = new BinaryWriter(stream);
			BinaryReader br = new BinaryReader(stream);

			int no = 0;
			while(true)
			{
				Log("Reading");
				Log(br.ReadString());

				Log("Writing");
				bw.Write("hello no. " + (++no) + " from client");
				Log("OK");
			}
		}
		catch(Exception ex)
		{
			Log("Error while running client");
			Log(ex.ToString());
		}
		finally
		{
			if(socket != null)
			{
				socket.Close();
			}
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		lock(logTail.SyncRoot)
		{
			if(logTail.Count > 0)
			{
				if(tbLog.Text.Length > 2048)
				{
					tbLog.Text = "";
				}
				foreach(String text in logTail)
				{
					tbLog.Text += (text + "\r\n");
					tbLog.Update();
				}
				logTail.Clear();
			}
		}
	}

	private void bDiscover_Click(object sender, EventArgs e)
	{
		try
		{
			lbDevices.Items.Clear();
			tbLog.Text = "";

			IrDAClient client = new IrDAClient();
			IrDADeviceInfo[] devices = client.DiscoverDevices(10);
			if(devices.Length == 0)
			{
				Log("No devices found");
				return;
			}
			Log("Found " + devices.Length + " devices");
			foreach(IrDADeviceInfo device in devices)
			{
				lbDevices.Items.Add(new DevItem(device));
			}
			lbDevices.SelectedIndex = 0;
		}
		catch(Exception ex)
		{
			Log("There was exception while discovering devices.             ");
			Log("                                                           ");
			Log("On UNIX please check ifconfig to make sure that your IrDA  ");
			Log("device is up. If not do ifconfig irda0 up as root.         ");
			Log("                                                           ");
			Log("On windows make sure that you compiled pnet with           ");
			Log("--disable-cygwin, because on pure cygwin it wont work.     ");
			Log("                                                           ");
			Log("You can also try to repeat discovery.                      ");
			Log("                                                           ");
			Log("Exception dump:                                            ");
			Log(ex.ToString());
		}
	}

	private void bServer_Click(object sender, EventArgs e)
	{
		tbLog.Text = "";
		Log("Starting server");
		Thread thread = new Thread(new ThreadStart(ServerFn));
		thread.Start();
	}

	private void bConnect_Click(object sender, EventArgs e)
	{
		DevItem item = lbDevices.SelectedItem as DevItem;
		if(item == null)
		{
			MessageBox.Show("Discover and select device first!");
			return;
		}
		selectedDevice = item.Device;

		tbLog.Text = "";
		Log("Connecting to " + item.ToString());
		Thread thread = new Thread(new ThreadStart(ClientFn));
		thread.Start();
	}

	static void Main(String[] args)
	{
		IrdaTest f = new IrdaTest();
		Application.Run(f);
	}
}
