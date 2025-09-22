using System.Runtime.InteropServices;

namespace NativeHandles
{
    public class NativeHandles
    {
        // [DllImport] для GetCurrentProcess - возвращает псевдо-дескриптор текущего процесса
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();

        // Функция для закрытия дескриптора (для реальных дескрипторов, GetCurrentProcess возвращает псевдо-дескриптор, который не нужно закрывать)
        // Эта функция будет использоваться в следующих примерах для реальных хэндлов
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        public static void Main(string[] args)
        {
            IntPtr processHandle = IntPtr.Zero; // Инициализируем нулевым значением

            try
            {
                processHandle = GetCurrentProcess();
                Console.WriteLine($"Current process handle (pseudo-handle): {processHandle}");

                // Важно: GetCurrentProcess возвращает псевдо-дескриптор, который не нужно закрывать.
                // Если бы это был реальный дескриптор, полученный, например, от CreateFile,
                // мы бы вызвали CloseHandle(processHandle);

            }
            finally
            {
                // В случае реального дескриптора:
                // if (processHandle != IntPtr.Zero && IsRealHandle(processHandle))
                // {
                //     CloseHandle(processHandle);
                //     Console.WriteLine($"Closed handle: {processHandle}");
                // }
            }
        }
    }
}
