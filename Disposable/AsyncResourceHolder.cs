namespace Disposable
{
    public class AsyncResourceHolder : IAsyncDisposable
    {
        private bool _connectionOpen = false;
        private int _id;
        private static int _nextId = 1;

        public AsyncResourceHolder()
        {
            _id = _nextId++;
            Console.WriteLine($"[Async#{_id}] Initializing connection synchronously (for demo simplicity)...");
            // В реальном коде, открытие тоже могло бы быть асинхронным,
            // но для демонстрации DisposeAsync, сделаем конструктор синхронным.
            _connectionOpen = true;
            Console.WriteLine($"[Async#{_id}] Connection {_id} opened.");
        }

        public async Task SendDataAsync(string data)
        {
            if (!_connectionOpen)
                throw new InvalidOperationException($"[Async#{_id}] Connection not open or disposed.");

            Console.WriteLine($"[Async#{_id}] Sending data asynchronously: '{data}'...");
            await Task.Delay(500); // Имитация асинхронной отправки данных
            Console.WriteLine($"[Async#{_id}] Data '{data}' sent.");
        }

        // Реализация IAsyncDisposable
        public async ValueTask DisposeAsync()
        {
            Console.WriteLine($"[Async#{_id}] DisposeAsync() called. Closing connection asynchronously...");
            if (_connectionOpen)
            {
                await Task.Delay(750); // Имитация асинхронного закрытия/очистки
                _connectionOpen = false;
                Console.WriteLine($"[Async#{_id}] Connection {_id} closed asynchronously.");
            }
            else
            {
                Console.WriteLine($"[Async#{_id}] Connection {_id} was already closed.");
            }
        }

        // Опционально: Реализация IDisposable для совместимости
        // Если вы реализуете оба интерфейса, Dispose() должен быть осторожен.
        // Часто он просто вызывает DisposeAsync().AsTask().Wait() или AsTask().GetAwaiter().GetResult(),
        // что, как мы знаем, может привести к дедлокам.
        // Если ваш ресурс требует асинхронной очистки, лучше избегать синхронного Dispose()
        // или по крайней мере ясно документировать его ограничения.
        // public void Dispose()
        // {
        //     Console.WriteLine($"[Async#{_id}] Synchronous Dispose() called. Waiting for async cleanup...");
        //     DisposeAsync().AsTask().Wait(); // Блокировка!
        //     GC.SuppressFinalize(this);
        //     Console.WriteLine($"[Async#{_id}] Synchronous Dispose() finished.");
        // }
    }

}
