using System.IO;

namespace StreamUtils
{
    public class EditableMemoryStream : EditableStream
    {
        public EditableMemoryStream()
            : base(new MemoryStream())
        {
        }

        public EditableMemoryStream(MemoryStream writeStream)
            : base(writeStream)
        {
        }

        protected override Stream CreateReadStream()
        {
            return new MemoryStream(((MemoryStream)write).GetBuffer());
        }
    }
}
