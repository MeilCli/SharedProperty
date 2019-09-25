using System;
using System.Buffers;

namespace SharedProperty.Serializer.SystemTextJson
{
    internal sealed class ArrayBufferWriter<T> : IBufferWriter<T>, IDisposable
    {
        private const int defaultCapacity = byte.MaxValue;

        private T[] buffer;
        private int index;
        private bool isDisposed;

        public ArrayBufferWriter(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException(nameof(capacity));
            }

            buffer = ArrayPool<T>.Shared.Rent(capacity);
            index = 0;
        }

        public ReadOnlyMemory<T> WrittenMemory => buffer.AsMemory(0, index);

        public ReadOnlySpan<T> WrittenSpan => buffer.AsSpan(0, index);

        public int WrittenCount => index;

        public int Capacity => buffer.Length;

        public int FreeCapacity => buffer.Length - index;

        public void Clear()
        {
            buffer.AsSpan(0, index).Clear();
            index = 0;
        }

        public void Advance(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException(nameof(count));
            }
            if (buffer.Length - count < index)
            {
                throw new InvalidOperationException("cannot advance buffer");
            }

            index += count;
        }

        public Memory<T> GetMemory(int sizeHint = 0)
        {
            checkAndResizeBuffer(sizeHint);
            return buffer.AsMemory(index);
        }

        public Span<T> GetSpan(int sizeHint = 0)
        {
            checkAndResizeBuffer(sizeHint);
            return buffer.AsSpan(index);
        }

        private void checkAndResizeBuffer(int sizeHint)
        {
            if (sizeHint < 0)
            {
                throw new ArgumentException(nameof(sizeHint));
            }

            if (sizeHint == 0)
            {
                sizeHint = 1;
            }

            if (FreeCapacity < sizeHint)
            {
                int grouBy = Math.Max(sizeHint, buffer.Length);

                if (buffer.Length == 0)
                {
                    grouBy = Math.Max(grouBy, defaultCapacity);
                }

                int newSize = checked(buffer.Length + grouBy);

                T[] newArray = ArrayPool<T>.Shared.Rent(newSize);
                Array.Copy(buffer, 0, newArray, 0, buffer.Length);
                ArrayPool<T>.Shared.Return(buffer);
                buffer = newArray;
            }
        }

        public void Dispose()
        {
            if (isDisposed is false)
            {
                ArrayPool<T>.Shared.Return(buffer);
                isDisposed = true;
            }
        }
    }
}
