using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    internal class BufferWriterStream<TWriter> : Stream
    where TWriter : IBufferWriter<byte>
    {
        /// <summary>
        /// The target <typeparamref name="TWriter"/> instance to use.
        /// </summary>
        private readonly TWriter bufferWriter;

        /// <summary>
        /// Indicates whether or not the current instance has been disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="IBufferWriterStream{TWriter}"/> class.
        /// </summary>
        /// <param name="bufferWriter">The target <see cref="IBufferWriter{T}"/> instance to use.</param>
        public BufferWriterStream(TWriter bufferWriter)
        {
            this.bufferWriter = bufferWriter;
        }

        /// <inheritdoc/>
        public override bool CanRead => false;

        /// <inheritdoc/>
        public override bool CanSeek => false;

        /// <inheritdoc/>
        public override bool CanWrite
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => !disposed;
        }

        /// <inheritdoc/>
        public override long Length => throw new NotImplementedException();

        /// <inheritdoc/>
        public override long Position
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Flush()
        {
        }

        /// <inheritdoc/>
        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task<int> ReadAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override Task WriteAsync(byte[]? buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }

            try
            {
                Write(buffer, offset, count);

                return Task.CompletedTask;
            }
            catch (OperationCanceledException e)
            {
                return Task.FromCanceled(e.CancellationToken);
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int Read(byte[]? buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override int ReadByte()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Write(byte[]? buffer, int offset, int count)
        {
            if (disposed)
                throw new InvalidOperationException();

            Span<byte> source = buffer.AsSpan(offset, count);
            Span<byte> destination = bufferWriter.GetSpan(count);

            if (!source.TryCopyTo(destination))
            {
                throw new EndOfStreamException();
            }

            bufferWriter.Advance(count);
        }

        /// <inheritdoc/>
        public override void WriteByte(byte value)
        {
            if (disposed)
                throw new InvalidOperationException();

            bufferWriter.GetSpan(1)[0] = value;
            bufferWriter.Advance(1);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            disposed = true;
        }
    }
}
