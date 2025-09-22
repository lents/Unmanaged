using System.Runtime.InteropServices;

namespace SafeHandleDemo
{
    public class ManagedFileStream : IDisposable
    {
        private SafeFileHandleWrapper _safeFileHandle;
        private bool _disposed = false;

        public ManagedFileStream(string filePath)
        {
            // Создаем нативный файл и получаем SafeFileHandleWrapper
            _safeFileHandle = NativeMethods.CreateFile(
                filePath,
                NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE,
                NativeMethods.FILE_SHARE_READ,
                IntPtr.Zero,
                NativeMethods.OPEN_EXISTING, // Или CREATE_ALWAYS
                NativeMethods.FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero);

            if (_safeFileHandle.IsInvalid)
            {
                // Здесь можно получить код ошибки через Marshal.GetLastWin32Error()
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error(), "Failed to open file.");
            }
            Console.WriteLine($"ManagedFileStream created with handle: {_safeFileHandle.DangerousGetHandle()}");
        }

        // Методы для работы с файлом (чтение, запись), использующие _safeFileHandle.DangerousGetHandle()
        public void WriteToFile(string content)
        {
            if (_safeFileHandle.IsClosed || _safeFileHandle.IsInvalid)
            {
                throw new ObjectDisposedException(nameof(ManagedFileStream));
            }
            // Здесь можно было бы использовать P/Invoke для WriteFile, передавая _safeFileHandle.DangerousGetHandle()
            // Однако это нужно делать очень осторожно, оборачивая в DangerousAddRef/DangerousRelease.
            // Для простоты примера, просто имитируем
            Console.WriteLine($"Writing '{content}' to file via handle {_safeFileHandle.DangerousGetHandle()}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Подавляем финализатор для ManagedFileStream, если он был
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Освобождаем управляемые ресурсы, если таковые имеются
                Console.WriteLine("[ManagedFileStream.Dispose] Disposing managed resources.");
            }

            // Освобождаем SafeFileHandleWrapper. Это вызовет ReleaseHandle() в SafeFileHandleWrapper
            // ИЛИ подавит его финализацию, если она еще не сработала.
            if (_safeFileHandle != null && !_safeFileHandle.IsClosed)
            {
                _safeFileHandle.Dispose();
                Console.WriteLine($"[ManagedFileStream.Dispose] Disposed SafeFileHandleWrapper with handle: {_safeFileHandle.DangerousGetHandle()}");
            }

            _disposed = true;
        }

        // Нет необходимости в финализаторе для ManagedFileStream, если он просто содержит SafeHandle.
        // Если ManagedFileStream сам владеет другими неуправляемыми ресурсами напрямую,
        // тогда финализатор ManagedFileStream может вызывать Dispose(false).

    }
}
