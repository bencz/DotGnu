namespace Platform
{
	internal enum FileType
	{
		unknown=0,
		fifoSpecial=1,
		characterSpecial=2,
		directory=4,
		blockSpecial=6,
		regularFile=8,
		symbolicLink=10,
		socket=12
	}

} //namespace Platform
