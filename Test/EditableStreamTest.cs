using System;
using System.IO;
using NUnit.Framework;

namespace EditableStream
{
    public class EditableStreamTest
    {
        [Test]
        public void DemoInLoop()
        {
            var filename = Path.GetTempPath() + "sample.txt";
            if (File.Exists(filename))
                File.Move(filename, filename + Guid.NewGuid());
            var count = 10;
            var expectedLine = "This is marvelously very nice test." + Environment.NewLine;
            var expected = string.Empty;
            for (int i = 0; i < count; i++)
            {
                Program.Main(null);
                expected += expectedLine;
                var actual = File.ReadAllText(filename);
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
