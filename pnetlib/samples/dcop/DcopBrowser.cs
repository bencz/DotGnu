using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Xsharp.Dcop;

public class DcopBrowser : Form
{
	private TreeView treeView;
	private const int MethodIcon = 1;
	public DcopBrowser() 
	{
		this.Text = "Dcop Browser";
		this.Visible = true;
		treeView = new TreeView();
		treeView.Visible = true;
		treeView.Location = ClientRectangle.Location;
		treeView.Size = ClientRectangle.Size;
		this.Controls.Add(treeView);
	}
	public static DcopClient GetDcopClient(String [] args)
	{
		try
		{
			Xsharp.Application app = 
					new Xsharp.Application("DcopBrowser", args);
			return new DcopClient(app.Display, null);		
		}
		catch
		{
			return null;
		}
	}

	private ImageList GetImageList()
	{
		ImageList imageList = new ImageList();
		imageList.Images.Add(new Bitmap(typeof(DcopBrowser), "Class.bmp"));
		imageList.Images.Add(new Bitmap(typeof(DcopBrowser), "Method.bmp"));
		return imageList;
	}
	
	private void BrowseDcop(DcopClient client)
	{
		String[] apps = client.registeredApplications();
		treeView.BeginUpdate();
		treeView.ImageIndex = 0;
		treeView.SelectedImageIndex = -1;
		treeView.ImageList = GetImageList();
		foreach(String app in apps)
		{
			BrowseApp(client, app);
		}
		treeView.EndUpdate();
	}
	private void BrowseApp(DcopClient client, String appName)
	{
		if(appName.StartsWith("anonymous")) return;
		
		string appDisplayName = appName;
		if(appName.LastIndexOf("-") != -1)
		{
			appDisplayName = appName.Substring(0, appName.LastIndexOf("-"));
		}
		TreeNode node = treeView.Nodes.Add(appDisplayName);
		node.ImageIndex = -1;
		
		DcopRef appRef = new DcopRef(); 
		appRef.DiscoverApplication(appName, false, false);
		foreach(String obj in appRef.objects())
		{
			BrowseObject(appRef, obj, node);
		}
	}

	private void BrowseObject(DcopRef app, String objName, TreeNode parent)
	{
		TreeNode node = parent.Nodes.Add(objName);
		DcopRef objRef = (DcopRef)(app.Clone());
		objRef.Obj = objName;
		objRef.Initialise();
		foreach(String name in objRef.functions())
		{
			BrowseFunction(objRef, name, node);
		}
	}

	private void BrowseFunction(DcopRef obj, String funcName, TreeNode parent)
	{
		TreeNode node = parent.Nodes.Add(funcName);
		node.ImageIndex = MethodIcon;
	}

	protected override void OnResize(EventArgs ea)
	{
		this.treeView.Size = ClientRectangle.Size;
	}

	public static void Main(String[] args)
	{
		DcopBrowser db = new DcopBrowser();
		// Note call GetDcopClient after creating the Form
		DcopClient client = GetDcopClient(args);
		DcopRef dr = null ;
		if(client == null)
		{
			MessageBox.Show("Make sure that KDE is running");
			return;
		}
		db.BrowseDcop(client);
		Application.Run(db);
	}
}
