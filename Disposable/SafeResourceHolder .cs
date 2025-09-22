namespace Disposable
{
    public class SafeResourceHolder : IDisposable
    {
        private IntPtr _unmanagedHandle; // Неуправляемый ресурс
        private bool _disposed = false;

        public SafeResourceHolder()
        {
            _unmanagedHandle = OpenSomeNativeResource();
            Console.WriteLine($"MySafeResourceHolder created. Handle: {_unmanagedHandle}");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Очень важно!
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Освобождение управляемых ресурсов, если они есть
                    Console.WriteLine("Disposing managed resources (if any).");
                }

                // Освобождение неуправляемых ресурсов
                Console.WriteLine($"Releasing unmanaged handle: {_unmanagedHandle}");
                CloseSomeNativeResource(_unmanagedHandle);
                _unmanagedHandle = IntPtr.Zero; // Обнуляем, чтобы избежать повторного использования
                _disposed = true;
            }
        }

        // Финализатор как страховка!
        ~SafeResourceHolder()
        {
            Console.WriteLine($"MySafeResourceHolder finalizer called as a fallback. Releasing handle: {_unmanagedHandle}");
            Dispose(false); // Вызываем Dispose с параметром false, так как GC вызвал финализатор
        }

        private IntPtr OpenSomeNativeResource() => new IntPtr(456);
        private void CloseSomeNativeResource(IntPtr handle) => Console.WriteLine($"Native resource {handle} closed by Dispose.");
    }

}
