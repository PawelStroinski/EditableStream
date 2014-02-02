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

# Credits #
The TransposeHelper class is coming with very small modifications from the article:

[Insert Text into Existing Files in C#, Without Temp Files or Memory Buffers](http://www.codeproject.com/Articles/17716/Insert-Text-into-Existing-Files-in-C-Without-Temp)

By **Paul C Smith**,  29 Jun 2007 