using System.Runtime.InteropServices;

namespace NativeBufferHandle
{
    public class NativeBufferHandle : SafeHandle
    {
        public NativeBufferHandle() : base(IntPtr.Zero, true) { }

        // Конструктор, который принимает уже выделенный указатель
        public NativeBufferHandle(IntPtr preAllocatedPtr, bool ownsHandle = true) : base(IntPtr.Zero, ownsHandle)
        {
            SetHandle(preAllocatedPtr);
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        // Этот метод будет вызван при освобождении SafeHandle (например, при сборке мусора)
        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                Console.WriteLine($"Освобождаем неуправляемый буфер по адресу: {handle}");
                Marshal.FreeHGlobal(handle); // Используем Marshal.FreeHGlobal для освобождения памяти, выделенной через Marshal.AllocHGlobal
                handle = IntPtr.Zero;
                return true;
            }
            return false;
        }

        // Для демонстрации, можем добавить свойство для размера буфера
        public long Size { get; set; }
    }
}
