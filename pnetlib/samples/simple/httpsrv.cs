
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
class SocketListener
{
	public static byte[] GetBytes(String msg)
	{
		byte[] b=new byte[msg.Length];
		for(int i=0;i<msg.Length;i++)
		{
			b[i]=(byte)msg[i];
		}
		return b;
	}
	public static void Main(String []argv)
	{
		IPAddress ip = IPAddress.Loopback;
		Int32 port=1800;
		if(argv.Length>0)
			port = Int32.Parse(argv[0]);	
		IPEndPoint ep = new IPEndPoint(ip,port);
		Socket ss = new Socket(AddressFamily.InterNetwork , 
			SocketType.Stream , ProtocolType.Tcp);
		try
		{
			ss.Bind(ep);
		}
		catch(SocketException err)
		{
			Console.WriteLine("** Error : socket already in use :"+err);
			Console.WriteLine("           Please wait a few secs & try");	
		}
		ss.Listen(-1);
		Console.WriteLine("Server started and running on port {0}.....",port);
		Console.WriteLine("Access URL http://localhost:{0}",port);
		Socket client = null;
		while(true)
		{
			client=ss.Accept();
			SocketMessenger sm=new SocketMessenger(client);
			sm.Start();
		}
		Console.WriteLine(client.LocalEndPoint.ToString()+" CONNECTED ");
		ss.Close();
	}
}

class SocketMessenger
{
	private String index_file="httpsrv.cs";
	public SocketMessenger(Socket client)
	{
		this.client=client;
	}
	public void Start()
	{
		byte[] buffer=new byte[300];
		int read=client.Receive(buffer,0,300,SocketFlags.None);
		this.request=GetString(buffer);
		if(read>0)
		{
			String fname=this.request.Substring(this.request.IndexOf("/"));
			fname=fname.Substring(0,fname.IndexOf(" HTTP/1"));
			if(fname=="/")
				fname="/"+index_file;

			fname="."+fname;
			SendFile(fname);
		}
		client.Shutdown(SocketShutdown.Both);
		client.Close();
	}
	
	public void SendFile(String fname)
	{
		const String CRLF="\r\n";
		String header="HTTP/1.0 200 OK"+CRLF+"Keep-Alive: Close"+CRLF+CRLF;
		Console.WriteLine(fname);
		FileStream fin=new FileStream(fname,FileMode.Open,
			FileAccess.Read);
		client.Send(GetBytes(header));
		byte []buffer=new byte[1024];
		int read=0;
		read=fin.Read(buffer,0,1024);
		while(read>0)
		{
			client.Send(buffer,0,read,SocketFlags.None);
			read=fin.Read(buffer,0,1024);
		}	
	}
	public static String GetString(byte[] msg)
	{
		char[] b=new char[msg.Length];
		for(int i=0;i<msg.Length;i++)
		{
			b[i]=(char)msg[i];
		}
		return new String(b);
	}
	public static byte[] GetBytes(String msg)
	{
		byte[] b=new byte[msg.Length];
		for(int i=0;i<msg.Length;i++)
		{
			b[i]=(byte)msg[i];
		}
		return b;
	}
	Socket client;
	String request;
}
