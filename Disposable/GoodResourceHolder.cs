namespace Disposable
{
    public class GoodResourceHolder : IDisposable
    {
        private FileStream _file;
        private string _filePath;
        private bool _disposed = false;

        public GoodResourceHolder(string filePath)
        {
            _filePath = filePath;
            _file = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            Console.WriteLine($"[Good] Resource acquired: {_filePath}");
        }

        public void WriteSomeData(string data)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(GoodResourceHolder), "Cannot write to a disposed resource.");

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data + Environment.NewLine);
            _file.Write(bytes, 0, bytes.Length);
            Console.WriteLine($"[Good] Data written to {_filePath}");
        }

        // Публичный метод Dispose, вызываемый пользователем или using
        public void Dispose()
        {
            Dispose(true); // Передаем, что вызван явно (disposing = true)
            GC.SuppressFinalize(this); // Говорим GC не вызывать финализатор, т.к. мы уже очистились
            Console.WriteLine($"[Good] Dispose() called for {_filePath}");
        }

        // Защищенный виртуальный метод для фактической очистки
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы (в данном случае, FileStream)
                    _file?.Dispose();
                    Console.WriteLine($"[Good] Managed resource (FileStream) disposed for {_filePath}.");
                }
                // Здесь можно было бы освободить неуправляемые ресурсы напрямую,
                // но FileStream сам заботится о своем нативном дескрипторе.
                // В следующих секциях увидим прямую работу с неуправляемыми ресурсами.

                _disposed = true;
            }
        }

        // Финализатор (деструктор) - как страховка, если Dispose не вызван явно.
        // Обсудим подробнее в следующей секции.
        ~GoodResourceHolder()
        {
            Console.WriteLine($"[Good] Finalizer called for {_filePath}. Dispose(false)");
            Dispose(false); // Передаем, что вызван финализатором (disposing = false)
        }
    }

}
