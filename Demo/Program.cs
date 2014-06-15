using System.IO;

namespace StreamUtils
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var filename = Path.GetTempPath() + "sample.txt";
            using (var stream = new EditableFileStream(filename))
                Demo(stream);
            using (var stream = new EditableMemoryStream())
                Demo(stream);
        }

        public static void Demo(EditableStream stream)
        {
            var ins = "arvelousl";
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
        }
    }
}