using System.Runtime.InteropServices;

namespace NativeMemoryDemo
{
    // Необходимо включить <AllowUnsafeBlocks>true</AllowUnsafeBlocks> в .csproj
    public unsafe class NativeBuffer : IDisposable
    {
        private void* _buffer;
        private nuint _sizeInBytes;
        private bool _disposed = false;

        public NativeBuffer(nuint sizeInBytes)
        {
            if (sizeInBytes == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeInBytes), "Размер буфера должен быть больше нуля.");
            }

            _sizeInBytes = sizeInBytes;
            _buffer = NativeMemory.AllocZeroed(sizeInBytes); // Выделяем и инициализируем нулями
            if (_buffer == null)
            {
                throw new OutOfMemoryException($"Не удалось выделить нативную память размером {sizeInBytes} байт.");
            }
            Console.WriteLine($"[NativeBuffer] Выделено {_sizeInBytes} байт по адресу: {(long)_buffer:X}");
        }

        // Метод для записи значения типа T по смещению
        public void Write<T>(nuint offset, T value) where T : unmanaged // unmanaged constraint для указателей
        {
            ThrowIfDisposed();
            if (offset + (nuint)sizeof(T) > _sizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Выход за границы буфера.");
            }
            *(T*)((byte*)_buffer + offset) = value;
        }

        // Метод для чтения значения типа T по смещению
        public T Read<T>(nuint offset) where T : unmanaged
        {
            ThrowIfDisposed();
            if (offset + (nuint)sizeof(T) > _sizeInBytes)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Выход за границы буфера.");
            }
            return *(T*)((byte*)_buffer + offset);
        }

        // Метод для прямого доступа к указателю (если требуется)
        public void* GetPointer()
        {
            ThrowIfDisposed();
            return _buffer;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(NativeBuffer), "Буфер уже освобожден.");
            }
        }

        // Реализация IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Предотвращаем вызов финализатора, так как ресурсы уже освобождены
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_buffer != null)
                {
                    NativeMemory.Free(_buffer); // Освобождаем нативную память
                    Console.WriteLine($"[NativeBuffer] Освобождено {_sizeInBytes} байт по адресу: {(long)_buffer:X}");
                    _buffer = null; // Обнуляем указатель, чтобы избежать повторного освобождения
                }
                _disposed = true;
            }
        }

        // Финализатор (деструктор) - как страховка на случай, если Dispose не был вызван явно
        ~NativeBuffer()
        {
            Dispose(false);
        }
    }
}

