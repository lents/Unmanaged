using System.Runtime.InteropServices;

namespace SafeHandleDemo
{
    // Наш собственный SafeHandle для файловых дескрипторов
    public class SafeFileHandleWrapper : SafeHandle
    {
        // Конструктор SafeHandleWrapper
        // Базовому конструктору SafeHandle передаем:
        // 1. Значение, которое считается невалидным для данного дескриптора (INVALID_HANDLE_VALUE)
        // 2. Флаг ownsHandle: true, если этот SafeHandle отвечает за освобождение дескриптора
        public SafeFileHandleWrapper() : base(NativeMethods.INVALID_HANDLE_VALUE, true) { }

        // Свойство IsInvalid: возвращает true, если внутренний дескриптор недействителен
        public override bool IsInvalid => handle == NativeMethods.INVALID_HANDLE_VALUE || handle == IntPtr.Zero;

        // Метод ReleaseHandle: вызывается для освобождения нативного ресурса
        // Должен быть реализован и никогда не должен выбрасывать исключения.
        protected override bool ReleaseHandle()
        {
            // Проверяем, что дескриптор вообще существует и не является невалидным
            if (!IsInvalid)
            {
                Console.WriteLine($"[SafeFileHandleWrapper.ReleaseHandle] Closing native handle: {handle}");
                return NativeMethods.CloseHandle(handle); // Вызываем нативную функцию для закрытия
            }
            return true; // Дескриптор уже невалиден или равен 0, ничего закрывать не нужно
        }
    }

}
