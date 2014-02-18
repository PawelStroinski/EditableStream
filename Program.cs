using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var filename = Path.GetTempPath() + "sample.txt";
        var ins = "arvelousl";
        using (var stream = new InsertModeFileStream(filename))
        using (var writer = new StreamWriter(stream))
        {
            writer.WriteLine("This is my very nice test file.");
            writer.Flush();
            stream.Position = 9;
            writer.Write(ins);
            writer.Flush();
            stream.Position = 34;
            stream.Delete(5);
        }
    }
}
