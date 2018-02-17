using System;
using System.IO;

namespace Zstandard.Net.Benchmark
{
    class DevNullStream : Stream
    {
        private long length = 0;

        public DevNullStream()
        {
        }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => this.length;

        public override long Position
        {
            get
            {
                return this.length;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            this.length = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.length += count;
        }
    }
}
