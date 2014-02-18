EditableStream
==============
Stream & FileStream drop-in replacement with insert-mode as opposed to override-mode and with delete operation.

### Example code

	var filename = Path.GetTempPath() + "sample.txt";
	var ins = "arvelousl";
	using (var stream = new EditableFileStream(filename))
	using (var writer = new StreamWriter(stream))
	{
	    stream.InsertMode = true;
	    writer.WriteLine("This is my very nice test file.");
	    writer.Flush();
	    stream.Position = 9;
	    writer.Write(ins);
	    writer.Flush();
	    stream.Position = 34;
	    stream.Delete(5);
	}

### Output

This is marvelously very nice test.

### API

`EditableStream` class inherits from `Stream` and adds `InsertMode` flag and `Delete(long count)` method.

`EditableFileStream` class inherits from `EditableStream`.


# Credits #
The TransposeHelper class is coming with very small modifications from the article:

[Insert Text into Existing Files in C#, Without Temp Files or Memory Buffers](http://www.codeproject.com/Articles/17716/Insert-Text-into-Existing-Files-in-C-Without-Temp)

By **Paul C Smith**,  29 Jun 2007 