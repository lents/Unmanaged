using System.Runtime.InteropServices;

namespace NativeInvoke
{
    public class NativeMethods
    {
        // [DllImport] атрибут указывает, из какой DLL мы импортируем функцию.
        // "kernel32.dll" - это одна из основных системных библиотек Windows.
        // Метод должен быть статическим и extern (реализация находится вовне).
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void OutputDebugString(string lpOutputString);

        // Другой пример: получение текущего системного времени
        [DllImport("kernel32.dll")]
        public static extern void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        // Структура для GetSystemTime (должна точно соответствовать нативной)
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }
    
        static void Main(string[] args)
        {
            // Вызов нативной функции
            OutputDebugString("Hello from P/Invoke! This message goes to debug output.\n");
            Console.WriteLine("OutputDebugString called. Check your debugger's output window.");

            // Получение системного времени
            SYSTEMTIME st = new();
            GetSystemTime(ref st);
            Console.WriteLine($"Current System Time (UTC): {st.wHour:D2}:{st.wMinute:D2}:{st.wSecond:D2}");
            Console.ReadKey(); // Ждем, чтобы увидеть результат перед выходом
        }
    }
}
