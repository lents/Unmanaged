using System.Runtime.InteropServices;

namespace InvalidHandle
{
    internal class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(
                                                string lpFileName,
                                                uint dwDesiredAccess,
                                                uint dwShareMode,
                                                IntPtr lpSecurityAttributes, // IntPtr.Zero для NULL
                                                uint dwCreationDisposition,
                                                uint dwFlagsAndAttributes,
                                                IntPtr hTemplateFile); // IntPtr.Zero для NULL

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);

        public const uint GENERIC_READ = 0x80000000;
        public const uint OPEN_EXISTING = 3;
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        static void Main(string[] args)
        {
            DemonstrateInvalidHandle();
        }

        public static void DemonstrateInvalidHandle()
        {
            // Попытка открыть несуществующий файл
            IntPtr fileHandle = CreateFile("non_existent_file.txt", GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

            if (fileHandle == INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Failed to open file. Handle is INVALID_HANDLE_VALUE.");
                // Отсутствие проверки здесь приведет к ошибке при попытке использовать fileHandle
            }
            else
            {
                Console.WriteLine($"Successfully opened file. Handle: {fileHandle}");
                CloseHandle(fileHandle);
            }
        }

        public class LeakyFileHandle // АНТИПАТТЕРН!
        {
            private IntPtr _fileHandle;

            public LeakyFileHandle(string filePath)
            {
                _fileHandle = CreateFile(filePath, GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
                if (_fileHandle == INVALID_HANDLE_VALUE)
                {
                    throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
                }
                Console.WriteLine($"LeakyFileHandle created. Handle: {_fileHandle}");
            }

            // Нет Dispose, нет финализатора -> утечка!
            // ~LeakyFileHandle() { CloseHandle(_fileHandle); } // Это нужно, но лучше IDisposable
        }

        // new LeakyFileHandle("some_file.txt"); // Ресурс будет утекать

    
    public static void DemonstrateDoubleFree()
        {
            IntPtr tempHandle = CreateFile("test_file.txt", GENERIC_READ, 0, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (tempHandle == INVALID_HANDLE_VALUE)
            {
                Console.WriteLine("Could not open test_file.txt for double-free demo.");
                return;
            }
            Console.WriteLine($"Opened test_file.txt. Handle: {tempHandle}");

            CloseHandle(tempHandle); // Первое освобождение
            Console.WriteLine($"First close successful for handle: {tempHandle}");

            // Попытка использовать tempHandle после освобождения
            // Это может привести к Use-After-Free или просто ошибке
            // Например, ReadFile(tempHandle, ...)

            CloseHandle(tempHandle); // Второе освобождение (Double-Free) - опасно!
            Console.WriteLine($"Second close attempted for handle: {tempHandle}. This is a double-free risk!");
        } 
    }

}
