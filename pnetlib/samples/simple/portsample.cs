/*
 * portsample.cs - Sample program for Serialport access.
 *
 * Copyright (C) 2004  Southern Storm Software, Pty Ltd.
 *
 * This program is free software, you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY, without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program, if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */


using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

#if CONFIG_SERIAL_PORTS

public class PortSample
{
	String portname;
	SerialPort port = null;	
	Thread reader, writer ;
	
	public PortSample(String portname)
	{
		this.portname = portname;
		try 
		{
			port = new SerialPort(portname, 115200, Parity.None, 8);
			port.Open();
		}
		catch(Exception e)
		{
			Console.WriteLine("Could not Open Serial Port {0} : {1}" , portname, e.Message);
			if(portname[0] != 'C' || portname[0] != 'c')
			{
				Console.WriteLine("Please use Windows portnames such as 'COM1' or 'COM2'");
			}
			return;
		}
	}

	public void Start()
	{
		reader = new Thread(new ThreadStart(Read));
		reader.IsBackground = true;
		reader.Start();
		writer = new Thread(new ThreadStart(Write));
		writer.Start();
		Console.WriteLine("You are now connected to {0} port", portname); 
		Console.WriteLine("Type 'AT' to make sure you are connected to the MODEM"); 
	}

	// Writer thread for Serial port
	public void Write()
	{
		for(;;)
		{
			port.WriteLine(Console.ReadLine());
			Thread.Sleep(200);
		}
	}

	// Reader thread for Serial port
	public void Read()
	{
		for(;;)
		{
			Console.Write(port.ReadExisting());
			Thread.Sleep(200);
		}
	}
	
	public static void Main(String[] args)
	{
		String portname = (args.Length != 0) ? args[0] : @"COM1";
		PortSample inst = new PortSample(portname);
		inst.Start();
	}
}
#endif /*CONFIG_SERIAL_PORTS*/
