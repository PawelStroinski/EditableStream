using System.IO;

abstract class InsertModeStream : Stream
{
    public abstract void Delete(long count);
}