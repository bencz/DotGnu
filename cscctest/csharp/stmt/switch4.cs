using System;

public class SW
{
	public int StringSwitch(String s)
	{
		switch(s)
		{
			case "T"	: goto case "TO";
			case "TO"	: goto case "TOK";
			case "TOK"	: goto case "TOKE";
			case "TOKE"	: goto case "TOKEN";
			case "TOKEN": return 5;
		}
		return 0;
	}
}
