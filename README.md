InsertModeFileStream
====================
FileStream drop-in replacement with insert-mode as opposed to override-mode.

### Example code

	var filename = Path.GetTempPath() + "sample.txt";
	var ins = "arvelousl";
	using (var stream = new InsertModeFileStream(filename))
	using (var writer = new StreamWriter(stream))
	{
	    writer.WriteLine("This is my very nice test file.");
	    writer.Flush();
	    stream.Position = 9;
	    writer.Write(ins);
	}

### Output

This is marvelously very nice test file.