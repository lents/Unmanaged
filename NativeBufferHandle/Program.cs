using System.Runtime.InteropServices;
using System.Text;

namespace NativeBufferHandle
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string message = "Привет, мир! This is a test string.";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            long bufferSize = messageBytes.Length + 1; // +1 для нулевого символа в конце (C-style строка)

            // Используем using для гарантированного освобождения SafeHandle
            using (NativeBufferHandle bufferHandle = new NativeBufferHandle())
            {
                try
                {
                    // 2. Выделяем неуправляемую память с помощью Marshal.AllocHGlobal
                    // В реальных сценариях вы могли бы использовать P/Invoke для вызова нативных функций выделения памяти
                    IntPtr nativeBufferPtr = Marshal.AllocHGlobal((IntPtr)bufferSize);
                    bufferHandle.SetHandle(nativeBufferPtr);
                    bufferHandle.Size = bufferSize;

                    Console.WriteLine($"Выделен неуправляемый буфер по адресу: {bufferHandle.handle} размером: {bufferHandle.Size} байт.");

                    // 3. Записываем данные в неуправляемую память с помощью NativeMemory
                    unsafe
                    {
                        byte* pBuffer = (byte*)bufferHandle.handle;

                        // Копируем байты сообщения
                        NativeMemory.Copy(messageBytes.AsSpan(), pBuffer, (nuint)messageBytes.Length);

                        // Добавляем нулевой символ в конец (опционально, но хорошая практика для строк)
                        pBuffer[messageBytes.Length] = 0;

                        Console.WriteLine("Данные записаны в неуправляемый буфер.");

                        // 4. Читаем данные из неуправляемой памяти с помощью NativeMemory
                        Span<byte> readBuffer = stackalloc byte[(int)bufferSize];
                        NativeMemory.Copy(pBuffer, readBuffer, (nuint)bufferSize);

                        // Находим конец строки (нулевой символ)
                        int nullTerminatorIndex = readBuffer.IndexOf((byte)0);
                        if (nullTerminatorIndex != -1)
                        {
                            readBuffer = readBuffer.Slice(0, nullTerminatorIndex);
                        }

                        string readMessage = Encoding.UTF8.GetString(readBuffer);
                        Console.WriteLine($"Прочитанная строка из неуправляемого буфера: \"{readMessage}\"");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                }
            } // Здесь SafeHandle будет автоматически освобожден, даже если произошла ошибка

            Console.WriteLine("Приложение завершено. SafeHandle должен был быть освобожден.");
            Console.ReadKey();
        }
    }
}
