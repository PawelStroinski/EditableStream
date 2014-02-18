// The TransposeHelper class is coming with very small modifications from the article:
// 
// Insert Text into Existing Files in C#, Without Temp Files or Memory Buffers
// By Paul C Smith,  29 Jun 2007 
// 
// http://www.codeproject.com/Articles/17716/Insert-Text-into-Existing-Files-in-C-Without-Temp

using System;
using System.IO;
using System.Text;

class InsertModeFileStream : InsertModeStream, IDisposable
{
    readonly FileStream write;

    public InsertModeFileStream(string path, FileMode mode = FileMode.OpenOrCreate,
        FileAccess access = FileAccess.ReadWrite, FileShare share = FileShare.Read)
    {
        write = new FileStream(path, mode, access, share);
    }

    public InsertModeFileStream(FileStream writeStream)
    {
        write = writeStream;
    }

    public override bool CanRead { get { return write.CanRead; } }

    public override bool CanSeek { get { return write.CanSeek; } }

    public override bool CanWrite { get { return write.CanWrite; } }

    public override void Flush() { write.Flush(); }

    public override long Length { get { return write.Length; } }

    public override long Position { get { return write.Position; } set { write.Position = value; } }

    public override int Read(byte[] buffer, int offset, int count) { return write.Read(buffer, offset, count); }

    public override long Seek(long offset, SeekOrigin origin) { return write.Seek(offset, origin); }

    public override void SetLength(long value) { write.SetLength(value); }

    public override void Write(byte[] buffer, int offset, int count)
    {
        TransposeIfNeededForWrite(count);
        write.Write(buffer, offset, count);
    }

    public override void Delete(long count)
    {
        CheckDeleteCount(count);
        var sourcePosition = Position + count;
        var length = Length - sourcePosition;
        var destinationPosition = Position;
        var distance = count;
        if (length > 0)
            using (var read = CreateReadStream())
                new TransposeHelper(
                    write: write,
                    read: read,
                    sourcePosition: sourcePosition,
                    length: length,
                    destinationPosition: destinationPosition,
                    distance: distance).Transpose();
        SetLength(Length - count);
    }

    void TransposeIfNeededForWrite(int distance)
    {
        var sourcePosition = Position;
        var streamLength = Length;
        var isAtTheEnd = sourcePosition == streamLength;
        if (!isAtTheEnd)
        {
            var length = streamLength - sourcePosition;
            var destinationPosition = sourcePosition + distance;
            using (var read = CreateReadStream())
                new TransposeHelper(
                    write: write,
                    read: read,
                    sourcePosition: sourcePosition,
                    length: length,
                    destinationPosition: destinationPosition,
                    distance: distance).Transpose();
            Position = sourcePosition;
        }
    }

    FileStream CreateReadStream()
    {
        return new FileStream(write.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }

    void CheckDeleteCount(long count)
    {
        if (count <= 0)
            throw new ArgumentException("Count cannot be zero or less.");
        if (count > Length - Position)
            throw new ArgumentException("Count cannot exceed number of bytes after current position.");
    }

    void IDisposable.Dispose()
    {
        write.Dispose();
    }

    class TransposeHelper
    {
        const int defaultBufferSize = 32 * 1024; // 32KB is consistently among the most efficient buffer sizes
        readonly FileStream write, read;
        readonly long sourcePosition, length, destinationPosition, distance;
        BinaryReader reader;
        BinaryWriter writer;

        /// <summary>
        /// Creates object which within <paramref name="write"/>, moves a range of <paramref name="length"/> bytes 
        /// starting at <paramref name="sourcePosition"/> to <paramref name="destinationPosition"/>.
        /// </summary>
        /// <param name="write">The target file writable stream</param>
        /// <param name="read">The target file readable stream</param>
        /// <param name="sourcePosition">The starting sourcePosition of the byte range to move</param>
        /// <param name="length">The number of bytes to move</param>
        /// <param name="destinationPosition">The destination position of the byte range</param>
        /// <param name="distance">The distance between destinationPosition and sourcePosition</param>
        public TransposeHelper(FileStream write, FileStream read, long sourcePosition, long length,
            long destinationPosition, long distance)
        {
            this.write = write;
            this.read = read;
            this.sourcePosition = sourcePosition;
            this.length = length;
            this.destinationPosition = destinationPosition;
            this.distance = distance;
        }

        public void Transpose()
        {
            using (reader = new BinaryReader(read, Encoding.UTF8, true))
            using (writer = new BinaryWriter(write, Encoding.UTF8, true))
            {
                read.Position = sourcePosition;
                write.Position = destinationPosition;
                if (destinationPosition > sourcePosition && length > distance)
                    TransposeForward();
                else
                    Copy();
            }
        }

        void TransposeForward()
        {
            int bufferSize = defaultBufferSize;
            bufferSize = (int)Math.Max(bufferSize, distance);
            bufferSize = (int)Math.Min(bufferSize, length);
            long numberOfReads = length / bufferSize;
            int remainingBytes = (int)(length % bufferSize);
            byte[] buffer1 = new byte[bufferSize];
            byte[] buffer2 = new byte[bufferSize];
            reader.Read(buffer2, 0, bufferSize);
            for (long i = 1; i < numberOfReads; i++)
            {
                buffer2.CopyTo(buffer1, 0);
                reader.Read(buffer2, 0, bufferSize);
                writer.Write(buffer1, 0, bufferSize);
            }
            if (remainingBytes > 0)
                reader.Read(buffer1, 0, remainingBytes);
            writer.Write(buffer2, 0, bufferSize);
            if (remainingBytes > 0)
                writer.Write(buffer1, 0, remainingBytes);
            buffer1 = buffer2 = null;
            GC.Collect();
        }

        private void Copy()
        {
            for (long i = 0; i < length; i++)
                writer.Write(reader.ReadByte());
        }
    }
}