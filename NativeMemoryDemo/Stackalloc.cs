namespace NativeMemoryDemo
{
    public class StackallocDemo
    {
        public unsafe void ProcessData(ReadOnlySpan<byte> input)
        {
            // Выделение 256 байт на стеке
            Span<byte> tempBuffer = stackalloc byte[256];

            // Копирование части входных данных
            input.Slice(0, Math.Min(input.Length, tempBuffer.Length)).CopyTo(tempBuffer);

            // Работа с tempBuffer
            for (int i = 0; i < tempBuffer.Length; i++)
            {
                tempBuffer[i] = (byte)(tempBuffer[i] ^ 0xFF); // Пример: инвертируем байты
            }

            Console.WriteLine($"Обработано {tempBuffer.Length} байт на стеке.");
            // tempBuffer автоматически освобождается при выходе из метода
        }

        public unsafe void ProcessFixedArray(byte[] data)
        {
            fixed (byte* ptr = data) // Закрепляем массив data
            {
                // Теперь ptr указывает на начало массива data, 
                // и его адрес не изменится, пока мы находимся в этом fixed блоке.
                // Можно передать ptr в неуправляемую функцию.

                // Пример: инвертирование байтов через указатель
                for (int i = 0; i < data.Length; i++)
                {
                    ptr[i] = (byte)(ptr[i] ^ 0xFF);
                }
            } // Массив снова доступен для перемещения GC после выхода из fixed блока
            Console.WriteLine($"Обработано {data.Length} байт в управляемом массиве через fixed блок.");
        }

    }
}
