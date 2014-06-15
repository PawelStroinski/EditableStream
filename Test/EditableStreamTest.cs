using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace StreamUtils
{
    public class EditableStreamTest
    {
        [Test]
        public void DemoInLoop()
        {
            var filename = Path.GetTempPath() + Guid.NewGuid() + ".txt";
            var count = 10;
            var expectedLine = "This is marvelously very nice test." + Environment.NewLine;
            var expected = string.Empty;
            var actualFromMemory = string.Empty;
            for (int i = 0; i < count; i++)
            {
                using (var stream = new EditableFileStream(filename))
                    Program.Demo(stream);
                expected += expectedLine;
                var actualFromFile = File.ReadAllText(filename);
                Assert.AreEqual(expected, actualFromFile);
                using (var stream = new EditableMemoryStream())
                {
                    using (var writer = new StreamWriter(stream, Encoding.ASCII, 512, leaveOpen: true))
                        writer.Write(actualFromMemory);
                    stream.Position = 0;
                    Program.Demo(stream);
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                        actualFromMemory = reader.ReadToEnd();
                }
                Assert.AreEqual(expected, actualFromMemory);
            }
        }
    }
}
