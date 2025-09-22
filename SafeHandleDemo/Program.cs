using System.Reflection.Metadata;

namespace SafeHandleDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string testFileName = "safe_handle_test.txt";

            // 1. Демонстрация корректного использования с using (детерминированная очистка)
            Console.WriteLine("\n--- Demo 1: Correct use with 'using' (deterministic cleanup) ---");
            try
            {
                using (var fileStream = new ManagedFileStream(testFileName))
                {
                    fileStream.WriteToFile("Hello SafeHandle!");
                } // Dispose() вызывается здесь, ресурсы освобождаются немедленно
                Console.WriteLine("File stream successfully disposed via 'using' block.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Demo 1: {ex.Message}");
            }

            // 2. Демонстрация забытого Dispose() (недетерминированная очистка через SafeHandle's finalizer)
            Console.WriteLine("\n--- Demo 2: Forgot to call Dispose() (SafeHandle finalizer as fallback) ---");
            ManagedFileStream forgottenFileStream = null;
            try
            {
                forgottenFileStream = new ManagedFileStream(testFileName);
                forgottenFileStream.WriteToFile("This resource will be collected by GC.");
                // Мы забыли вызвать forgottenFileStream.Dispose();
                // SafeHandleWrapper's ReleaseHandle() будет вызван GC в какой-то момент.
                Console.WriteLine("Created file stream but forgot to Dispose(). Waiting for GC...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Demo 2: {ex.Message}");
            }

            // Принудительный GC для демонстрации финализации SafeHandle
            Console.WriteLine("Forcing GC to collect forgotten resource...");
            forgottenFileStream = null; // Устраняем ссылку
            GC.Collect();
            GC.WaitForPendingFinalizers(); // Ждем завершения работы финализаторов
            Console.WriteLine("GC cycle completed. Check if SafeFileHandleWrapper.ReleaseHandle was called.");
                        
        }

    }
}
