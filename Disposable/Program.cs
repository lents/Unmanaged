namespace Disposable
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("--- Демонстрация 1: Утечка без IDisposable ---");
            string badFilePath = "bad_log.txt";

            try
            {
                BadResourceHolder badHolder = new BadResourceHolder(badFilePath);
                badHolder.WriteSomeData("This data should be in bad_log.txt");

                // Здесь мы забываем закрыть файл!
                // badHolder._file.Close(); // Если бы мы это сделали, утечки бы не было.

                Console.WriteLine($"[Bad] File '{badFilePath}' is theoretically still open.");
                // Попробуйте удалить bad_log.txt вручную. Если он еще открыт, получите ошибку.
                System.IO.File.Delete(badFilePath); // Вызовет ошибку, если файл еще открыт
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error in BadResourceHolder demo]: {ex.Message}");
                Console.WriteLine("This error indicates the file might still be in use.");
            }
            Console.WriteLine("-------------------------------------------\n");
            System.Threading.Thread.Sleep(500); // Дадим немного времени


            // --- Демонстрация 2: Корректное освобождение с IDisposable и using ---
            Console.WriteLine("--- Демонстрация 2: Корректное освобождение с IDisposable и using ---");
            string goodFilePath = "good_log.txt";

            try
            {
                using (var goodHolder = new GoodResourceHolder(goodFilePath))
                {
                    goodHolder.WriteSomeData("This data should be in good_log.txt");
                    Console.WriteLine($"[Good] File '{goodFilePath}' is in use within using block.");
                } // <-- Здесь goodHolder.Dispose() вызывается автоматически

                Console.WriteLine($"[Good] File '{goodFilePath}' is now closed. Trying to delete...");
                System.IO.File.Delete(goodFilePath); // Теперь это должно сработать!
                Console.WriteLine($"[Good] File '{goodFilePath}' successfully deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error in GoodResourceHolder demo]: {ex.Message}");
            }
            Console.WriteLine("-------------------------------------------\n");

            // --- Демонстрация 3: using декларация (C# 8.0+) ---
            Console.WriteLine("--- Демонстрация 3: using декларация (C# 8.0+) ---");
            string anotherFilePath = "another_log.txt";
            Demo3(anotherFilePath);
            Console.ReadKey(); // Ждем, чтобы показать, что файл `another_log.txt` будет удален после выхода из Main()
            System.IO.File.Delete(anotherFilePath);
            Console.WriteLine($"[Another] File '{anotherFilePath}' deleted upon exit.");

            Console.WriteLine("--- Демонстрация 4: Асинхронное освобождение с IAsyncDisposable и await using ---");
            Console.WriteLine("Начинаем использование AsyncResourceHolder...");
            try
            {
                await using (var asyncHolder = new AsyncResourceHolder()) // <-- Здесь await using
                {
                    await asyncHolder.SendDataAsync("First message");
                    await asyncHolder.SendDataAsync("Second message");
                    Console.WriteLine("[Async Demo] Inside await using block. Resource is active.");
                    // asyncHolder.DisposeAsync() будет вызван автоматически здесь.
                }
                Console.WriteLine("[Async Demo] Outside await using block. Resource should be closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Async Demo Error]: {ex.Message}");
            }
            Console.WriteLine("--- Демонстрация 4 завершена ---\n");

            // Пример использования с множественными await using декларациями
            Console.WriteLine("--- Демонстрация 5: Множественные await using декларации ---");
            await using var asyncHolder1 = new AsyncResourceHolder();
            await using var asyncHolder2 = new AsyncResourceHolder();
            await asyncHolder1.SendDataAsync("From holder 1");
            await asyncHolder2.SendDataAsync("From holder 2");
            Console.WriteLine("[Async Demo Multiple] All holders active. DisposeAsync will be called in reverse order.");
            // asyncHolder2.DisposeAsync() будет вызван первым, затем asyncHolder1.DisposeAsync()
            // при выходе из метода Main.

            Console.WriteLine("\nНажмите любую клавишу для завершения программы...");
            Console.ReadKey();
            Console.WriteLine("Программа завершается.");

            // Так как asyncHolder1 и asyncHolder2 объявлены как using var,
            // DisposeAsync() для них будет вызван после этой точки при выходе из Main.
            
            
            
            // В идеальном мире, мы всегда используем using
            using (var holder = new SafeResourceHolder())
            {
                // Работаем с ресурсом
                Console.WriteLine("Working with resource...");
            } // Здесь автоматически вызывается Dispose(), и финализатор не нужен.

        }

        private static void Demo3(string anotherFilePath)
        {           
            using var anotherHolder = new GoodResourceHolder(anotherFilePath);
            anotherHolder.WriteSomeData("This is from a using declaration.");
            Console.WriteLine($"[Another] File '{anotherFilePath}' is in use.");

            // В конце метода Main() `anotherHolder.Dispose()` будет вызван автоматически.
            // Но для демонстрации удаления, мы можем вызвать GC.Collect() (для примера, в реальном коде так не делать)
            Console.WriteLine($"[Another] Will attempt to delete '{anotherFilePath}' after Main completes.");
            // Не удаляйте здесь, так как using декларация еще не завершилась.
            // Для полной уверенности в очистке после using декларации
            // System.GC.Collect();
            // System.GC.WaitForPendingFinalizers();
            // System.IO.File.Delete(anotherFilePath); // Это сработает после завершения метода            
        }
    }
}
