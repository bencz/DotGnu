using System;

public class SW
{
	public int StringSwitch(String s)
	{
		switch(s)
		{
			case "TOKEN": return 4;break;
			case "TOK"	: return 5;break;
		}
		return 0;
	}
}
