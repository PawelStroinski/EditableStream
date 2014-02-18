using System.IO;

class InsertModeFileStream : InsertModeStream
{
    public InsertModeFileStream(string path, FileMode mode = FileMode.OpenOrCreate,
        FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
        : base(new FileStream(path, mode, access, share))
    {
    }

    public InsertModeFileStream(FileStream writeStream)
        : base(writeStream)
    {
    }

    protected override Stream CreateReadStream()
    {
        return new FileStream(((FileStream)write).Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }
}