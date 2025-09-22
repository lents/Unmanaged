namespace NativeMemoryDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Демонстрация NativeBuffer ---");

            // Использование в using блоке для автоматического освобождения
            using (var buffer = new NativeBuffer((nuint)(sizeof(int) + sizeof(float)))) // Буфер для int и float
            {
                int intValue = 12345;
                float floatValue = 3.14159f;

                // Запись данных
                buffer.Write(0, intValue); // int по смещению 0
                buffer.Write((nuint)sizeof(int), floatValue); // float по смещению, следующему за int

                Console.WriteLine($"Записано: int = {intValue}, float = {floatValue}");

                // Чтение данных
                int readIntValue = buffer.Read<int>(0);
                float readFloatValue = buffer.Read<float>((nuint)sizeof(int));

                Console.WriteLine($"Прочитано: int = {readIntValue}, float = {readFloatValue}");

                // Демонстрация MemoryCopy
                Console.WriteLine("\n--- Демонстрация Buffer.MemoryCopy ---");
                using (var sourceBytes = new NativeBuffer(8)) // Буфер для 8 байт
                using (var destBytes = new NativeBuffer(8)) // Буфер для 8 байт
                {
                    // Заполним исходный буфер
                    unsafe
                    {
                        byte* srcPtr = (byte*)sourceBytes.GetPointer();
                        for (int i = 0; i < 8; i++)
                        {
                            srcPtr[i] = (byte)(i + 10);
                        }
                        Console.WriteLine("Source bytes (before copy): " + string.Join(" ", GetBytesFromPointer(srcPtr, 8)));
                    }

                    // Копируем данные
                    unsafe
                    {
                        Buffer.MemoryCopy(sourceBytes.GetPointer(), destBytes.GetPointer(), 8, 8);
                    }

                    unsafe
                    {
                        Console.WriteLine("Destination bytes (after copy): " + string.Join(" ", GetBytesFromPointer((byte*)destBytes.GetPointer(), 8)));
                    }
                } // sourceBytes и destBytes будут освобождены здесь

            } // buffer будет освобожден здесь

            Console.WriteLine("\n--- Демонстрация завершена ---");

            // Попытка использовать освобожденный буфер
            try
            {
                var invalidBuffer = new NativeBuffer(4);
                invalidBuffer.Dispose(); // Явно освобождаем
                invalidBuffer.Read<int>(0); // Это вызовет ObjectDisposedException
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"\n[Ошибка] Попытка использовать освобожденный буфер: {ex.Message}");
            }

            byte[] data = new byte[500];
            new Random().NextBytes(data);
            new StackallocDemo().ProcessData(data);
            byte[] myManagedArray = new byte[100];
            new Random().NextBytes(myManagedArray);
            new StackallocDemo().ProcessFixedArray(myManagedArray);

        }

        // Вспомогательный метод для печати байтов (для демонстрации)
        public static unsafe byte[] GetBytesFromPointer(byte* ptr, int count)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
            {
                bytes[i] = ptr[i];
            }
            return bytes;
        }

    }
}


